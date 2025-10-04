using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using V2.Core;
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Projectiles
{
	public static class ProjectileExtensions
	{
		public static GeneralProjectile AsV2Proj(this Projectile projectile, bool risky = false)
		{
			if (!projectile.TryGetGlobalProjectile(out GeneralProjectile V2Projectile))
			{
				if (risky)
					return null;

				throw new Exception("this projectile hasn't been properly recognized by Voraria: Second Course! oh no! anyway");
			}
			return V2Projectile;
		}
		public static PredProjectile AsPred(this Projectile projectile) => projectile.GetGlobalProjectile<PredProjectile>();
		public static PreyProjectile AsFood(this Projectile projectile, bool risky = false)
		{
			if (!projectile.TryGetGlobalProjectile(out PreyProjectile preyProjectile))
			{
				if (risky)
					return null;

				throw new Exception("this projectile can't be eaten, and thus, doesn't have a PreyProjectile global attached to them. look for your favorite snack elsewhere");
			}
			return preyProjectile;
		}

		/// <summary>
		/// Deals the given amount of DIRECT digestion damage to the given projectile, respecting damage variation and luck.<br/>
		/// </summary>
		/// <param name="pred">The pred currently digesting this player.</param>
		/// <param name="digestionDamage">The total amount of digestion damage to be dealt, before damage variation calculations.</param>
		/// <returns>Whether or not the resulting digestion tick kills the projectile.</returns>
		public static bool TakeDigestionDamage(this Projectile projectile, Entity pred, double digestionDamage)
		{
			int trueDigestionDamage = Main.DamageVar((float)digestionDamage);

            //Baelz digestion crit (we are so fuckin good at making content)
            bool digestionCrit = false;
            Color DigestionTextColor = Color.DarkGreen;
            if (pred is Player)
            {
                Player predPlayer = pred as Player;
                int chance = Main.rand.Next(101);
                int critChance = BaelzTransformation.GetCritChanceForDigestionTicks(predPlayer);
                if (chance <= critChance)
                {
                    digestionCrit = true;
                    trueDigestionDamage *= 2;
                    DigestionTextColor = Color.FromNonPremultiplied(125, 175, 0, 255);
                }
            }

            projectile.AsFood().Health -= trueDigestionDamage;
			switch (Main.netMode)
			{
				case NetmodeID.SinglePlayer:
					if (!ModContent.GetInstance<V2ClientConfig>().ShowChurnDamageNumbers)
						break;

					CombatText digestionDamageText = Main.combatText[CombatText.NewText(
						projectile.Hitbox,
                        DigestionTextColor,
						trueDigestionDamage,
                        digestionCrit,
						true
					)];
					digestionDamageText.position.X = pred.Center.X + (pred.direction * 28);
					digestionDamageText.position.Y = projectile.Center.Y + (projectile.height / 5f);
					digestionDamageText.velocity.X = pred.direction * 2.5f;
					digestionDamageText.velocity.Y = -4f;
					break;
				case NetmodeID.Server:
					ModPacket digestionDamageTextPacket = V2.Instance.GetPacket();
					digestionDamageTextPacket.Write((byte)V2.MessageType.SyncDigestionCombatTextForPreyProjectile);
					digestionDamageTextPacket.Write(projectile.whoAmI);
					digestionDamageTextPacket.Write(trueDigestionDamage);
					digestionDamageTextPacket.Write(pred.Center.X + (pred.direction * 28));
					digestionDamageTextPacket.Write(projectile.Center.Y + (projectile.height / 5f));
					digestionDamageTextPacket.Write(pred.direction * 2.5f);
					digestionDamageTextPacket.Write(-4f);
					digestionDamageTextPacket.Send();
					break;
				case NetmodeID.MultiplayerClient:
					// here we do nothing because the packet takes care of this
					break;
			}
			if (projectile.AsFood().Health <= 0)
			{
				projectile.AsFood().OnKilledByDigestion?.Invoke(projectile, pred);
				projectile.AsFood().Digested = true;
				projectile.Kill();
				return true;
			}
			return false;
		}

		public static void DoContactGulpage(this Projectile projectile, List<(PreyType, int)> specificWhitelist = null)
		{
			if (projectile.CurrentCaptor() is not null)
				return;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC preyNPC = Main.npc[i];
				if (preyNPC.active && preyNPC.life > 0)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((PreyType type, int ID) in specificWhitelist)
						{
							if (type == PreyType.NPC && ID == preyNPC.type)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (projectile.Hitbox.Intersects(preyNPC.Hitbox) && PredProjectile.CanSwallow(projectile, preyNPC))
						PredProjectile.Swallow(projectile, preyNPC);
				}
			}
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player preyPlayer = Main.player[i];
				if (preyPlayer.active && !preyPlayer.dead)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((PreyType type, int ID) in specificWhitelist)
						{
							if (type == PreyType.Player)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (projectile.Hitbox.Intersects(preyPlayer.Hitbox) && PredProjectile.CanSwallow(projectile, preyPlayer))
						PredProjectile.Swallow(projectile, preyPlayer);
				}
			}
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile preyProjectile = Main.projectile[i];
				if (preyProjectile.active)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((PreyType type, int ID) in specificWhitelist)
						{
							if (type == PreyType.Projectile && ID == preyProjectile.type)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (projectile.Hitbox.Intersects(preyProjectile.Hitbox) && PredProjectile.CanSwallow(projectile, preyProjectile))
						PredProjectile.Swallow(projectile, preyProjectile);
				}
			}
			for (int i = 0; i < Main.maxItems; i++)
			{
				Item preyItem = Main.item[i];
				if (preyItem.active)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((PreyType type, int ID) in specificWhitelist)
						{
							if (type == PreyType.Item && ID == preyItem.type)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (projectile.Hitbox.Intersects(preyItem.Hitbox) && PredProjectile.CanSwallow(projectile, preyItem))
						PredProjectile.Swallow(projectile, preyItem);
				}
			}
		}
	}
}

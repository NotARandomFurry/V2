using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using V2.Core;

namespace V2.Projectiles
{
	public static class ProjectileExtensions
	{
		public static V2Projectile AsV2Proj(this Projectile projectile, bool risky = false)
		{
			if (!projectile.TryGetGlobalProjectile(out V2Projectile V2Projectile))
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
		/// Deals the given amount of DIRECT digestion damage to the given item, respecting damage variation and luck.<br/>
		/// Should not be used for items worn by eaten players; for them, use TakeIndirectDigestionDamage instead.
		/// </summary>
		/// <param name="pred">The pred currently digesting this player.</param>
		/// <param name="digestionDamage">The total amount of digestion damage to be dealt, before damage variation calculations.</param>
		/// <returns>Whether or not the resulting digestion tick "kills" (depletes the durability of) the item.</returns>
		public static bool TakeDigestionDamage(this Projectile projectile, Entity pred, double digestionDamage)
		{
			int trueDigestionDamage = Main.DamageVar((float)digestionDamage);
			projectile.AsFood().Health -= trueDigestionDamage;
			if (projectile.AsFood().Health <= 0)
			{
				projectile.AsFood().OnKilledByDigestion?.Invoke(projectile, pred);
				projectile.AsFood().Digested = true;
				projectile.Kill();
				return true;
			}
			else
			{
				CombatText digestionText = Main.combatText[CombatText.NewText(
					projectile.Hitbox,
					Color.DarkGreen,
					trueDigestionDamage,
					false,
					true
				)];
				digestionText.position.X = pred.Center.X;
				digestionText.position.X += pred.direction * 14;
				if (pred.direction == -1)
					digestionText.position.X -= ChatManager.GetStringSize(FontAssets.CombatText[0].Value, digestionText.text, new Vector2(digestionText.scale)).X;
				digestionText.position.Y = projectile.Center.Y;
				digestionText.position.Y += projectile.height / 5f;
				digestionText.velocity.X = pred.direction * 2.5f;
				digestionText.velocity.Y = -4f;
				return false;
			}
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

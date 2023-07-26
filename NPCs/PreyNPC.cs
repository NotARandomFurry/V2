using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.StatusEffects.Debuffs;

namespace V2.NPCs
{
	public static class PreyNPCStuff
	{
		public static PreyNPC AsFood(this NPC npc, bool risky = false)
		{
			if (!npc.TryGetGlobalNPC(out PreyNPC preyNPC))
			{
				if (risky)
					return null;

				throw new Exception("this NPC can't be eaten, and thus, doesn't have a PreyNPC global attached to them. look for your favorite food elsewhere");
			}
			return preyNPC;
		}
	}

	public partial class PreyNPC : GlobalNPC
	{
		public List<FoodTypeTag> FoodTypeTags { get; set; }
		public List<string> FoodFlavorTags { get; set; }

		public bool IsCurrentlyEaten { get; set; }
		public int EatenSafetyFrames { get; set; }
		public bool Digested { get; set; }
		public PredEntityReference? CurrentCaptor { get; set; }

		public delegate void DelegatePreyAI(NPC npc, Entity pred);
		public DelegatePreyAI PreyAIMethod { get; set; }

		public delegate double DelegatePreyBaseSizeOverride(NPC npc);
		public DelegatePreyBaseSizeOverride PreyBaseSizeOverrideMethod { get; set; }

		public StatModifier StruggleStrengthModifier { get; set; }

		public bool CanChatAsPrey { get; set; }

		public SoundStyle? DigestingHitSound;
		public SoundStyle? DigestedDeathSound;
		
		public StatModifier TakenDigestionDamageModifier { get; set; }

		public double SoftenedDigestionDamageTaken { get; set; }
		public StatModifier SoftenedDigestionDamageModifier { get; set; }
		public int SoftenedWearoffDelay { get; set; }
		public static int SoftenedWearoffMaxDelay => V2Utils.SensibleTime(seconds: 2, frames: 30);
		public StatModifier SoftenedWearoffRateModifier { get; set; }
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;

		public PreyNPC()
		{
			FoodTypeTags = null;
			FoodFlavorTags = null;

			PreyAIMethod = null;
			PreyBaseSizeOverrideMethod = null;

			CanChatAsPrey = false;
			DigestingHitSound = null;
			DigestedDeathSound = null;
		}

		public override void ResetEffects(NPC npc)
		{
			npc.AsFood().IsCurrentlyEaten = false;
			npc.AsFood().Digested = false;
			npc.AsFood().CurrentCaptor = null;

			StruggleStrengthModifier = StatModifier.Default;


			npc.AsFood().TakenDigestionDamageModifier = StatModifier.Default;

			if (!npc.HasBuff(ModContent.BuffType<Softened>()))
				npc.AddBuff(ModContent.BuffType<Softened>(), 3);
			npc.AsFood().SoftenedDigestionDamageModifier = StatModifier.Default;
			npc.AsFood().SoftenedWearoffRateModifier = StatModifier.Default;
			if (npc.AsFood().SoftenedWearoffDelay > 0)
				npc.AsFood().SoftenedWearoffDelay--;
			else if (npc.AsFood().SoftenedDigestionDamageTaken > 0)
				npc.AsFood().SoftenedDigestionDamageTaken -= npc.AsFood().SoftenedWearoffRateModifier.ApplyTo((float)(25.0 / 60.0));

			UpdateNPCEatenStatus(npc);
			DetermineDigestingSounds(npc);
		}

		public static void UpdateNPCEatenStatus(NPC npc)
		{
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC potentialPred = Main.npc[i];
				if (potentialPred is null || !potentialPred.active)
					continue;

				if (!potentialPred.TryGetGlobalNPC(out PredNPC pred))
					continue;

				if ((pred.stomachContents is null || pred.stomachContents.Count <= 0)
				 && (pred.stomachContentsQueue is null || pred.stomachContentsQueue.Count <= 0))
					continue;

				if (pred.stomachContents.FirstOrDefault(x => !x.Dead && x.Type == PreyType.NPC && (x.Instance.whoAmI == npc.whoAmI || x.Instance.whoAmI == npc.realLife)) is Prey prey)
				{
					npc.AsFood().IsCurrentlyEaten = true;
					npc.position = potentialPred.Center - (npc.Size / 2f);
					npc.AsFood().CurrentCaptor = new PredEntityReference()
					{
						Predator = potentialPred,
						PreyInstance = prey
					};
					break;
				}
				if (pred.stomachContentsQueue.FirstOrDefault(x => !x.Dead && x.Type == PreyType.NPC && (x.Instance.whoAmI == npc.whoAmI || x.Instance.whoAmI == npc.realLife)) is Prey queuedPrey)
				{
					npc.AsFood().IsCurrentlyEaten = true;
					npc.position = potentialPred.Center - (npc.Size / 2f);
					npc.AsFood().CurrentCaptor = new PredEntityReference()
					{
						Predator = potentialPred,
						PreyInstance = queuedPrey
					};
					break;
				}
			}
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player potentialPred = Main.player[i];
				if (potentialPred is null || !potentialPred.active || potentialPred.dead)
					continue;

				if ((potentialPred.AsPred().stomachContents is null || potentialPred.AsPred().stomachContents.Count <= 0)
				 && (potentialPred.AsPred().stomachContentsQueue is null || potentialPred.AsPred().stomachContentsQueue.Count <= 0))
					continue;

				if (potentialPred.AsPred().stomachContents.FirstOrDefault(x => !x.Dead && x.Type == PreyType.NPC && (x.Instance.whoAmI == npc.whoAmI || x.Instance.whoAmI == npc.realLife)) is Prey prey)
				{
					npc.AsFood().IsCurrentlyEaten = true;
					npc.position = potentialPred.Center - (npc.Size / 2f);
					npc.AsFood().CurrentCaptor = new PredEntityReference()
					{
						Predator = potentialPred,
						PreyInstance = prey
					};
					break;
				}
				if (potentialPred.AsPred().stomachContentsQueue.FirstOrDefault(x => !x.Dead && x.Type == PreyType.NPC && (x.Instance.whoAmI == npc.whoAmI || x.Instance.whoAmI == npc.realLife)) is Prey queuedPrey)
				{
					npc.AsFood().IsCurrentlyEaten = true;
					npc.position = potentialPred.Center - (npc.Size / 2f);
					npc.AsFood().CurrentCaptor = new PredEntityReference()
					{
						Predator = potentialPred,
						PreyInstance = queuedPrey
					};
					break;
				}
			}
		}

		public static void DetermineDigestingSounds(NPC npc)
		{
			if (npc.HitSound is not null && DigestingHitSoundDatabase.ContainsKey(npc.HitSound.Value))
				npc.AsFood().DigestingHitSound = DigestingHitSoundDatabase[npc.HitSound.Value];
			if (npc.DeathSound is not null && DigestedDeathSoundDatabase.ContainsKey(npc.DeathSound.Value))
				npc.AsFood().DigestedDeathSound = DigestedDeathSoundDatabase[npc.DeathSound.Value];
		}

		public override bool CanHitNPC(NPC npc, NPC target)
		{
			if (npc.AsFood().IsCurrentlyEaten || target.AsFood().IsCurrentlyEaten || npc.AsFood().EatenSafetyFrames > 0)
				return false;

			return true;
		}

		public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
		{
			if (npc.AsFood().IsCurrentlyEaten || npc.AsFood().EatenSafetyFrames > 0)
				return false;

			return true;
		}

		public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				return false;

			return null;
		}

		public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				return false;

			return null;
		}

		public override bool? CanBeCaughtBy(NPC npc, Item item, Player player)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				return false;

			return null;
		}

		public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				boundingBox = Rectangle.Empty;
		}

		public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (target.type == ModContent.NPCType<Lucinda>() && PredNPC.CanSwallow(target, npc))
			{
				modifiers.FinalDamage *= 0;
				modifiers.Knockback.Base = 0f;
				modifiers.DisableCrit();
			}
		}

		public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit)
		{
			if (target.type == ModContent.NPCType<Lucinda>())
			{
				PredNPC.Swallow(target, npc);
			}
		}

		public override bool? CanChat(NPC npc)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				return npc.AsFood().CanChatAsPrey;

			return null;
		}

		public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				return false;

			return null;
		}

		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			binaryWriter.Write(npc.AsFood().IsCurrentlyEaten);
			binaryWriter.Write(npc.AsFood().EatenSafetyFrames);
			binaryWriter.Write(npc.AsFood().Digested);
			if (npc.AsFood().IsCurrentlyEaten && npc.AsFood().CurrentCaptor.HasValue)
			{
				binaryWriter.Write(true);
				Entity pred = npc.AsFood().CurrentCaptor.Value.Predator;
				if (pred is NPC predNPC)
				{
					binaryWriter.Write("NPC pred");
					binaryWriter.Write(predNPC.whoAmI);
				}
				else if (pred is Player predPlayer)
				{
					binaryWriter.Write("Player pred");
					binaryWriter.Write(predPlayer.whoAmI);
				}
			}
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			npc.AsFood().IsCurrentlyEaten = binaryReader.ReadBoolean();
			npc.AsFood().EatenSafetyFrames = binaryReader.ReadInt32();
			npc.AsFood().Digested = binaryReader.ReadBoolean();
			if (npc.AsFood().IsCurrentlyEaten && binaryReader.ReadBoolean())
			{
				switch (binaryReader.ReadString())
				{
					case "NPC pred":
						int npcPredIndex = binaryReader.ReadInt32();
						npc.AsFood().CurrentCaptor = new PredEntityReference()
						{
							Predator = Main.npc[npcPredIndex],
							PreyInstance = Main.npc[npcPredIndex].AsPred().stomachContents.FirstOrDefault(x => x.Type == PreyType.NPC && x.Instance.whoAmI == npc.whoAmI)
						};
						break;
					case "Player pred":
						int playerPredIndex = binaryReader.ReadInt32();
						npc.AsFood().CurrentCaptor = new PredEntityReference()
						{
							Predator = Main.player[playerPredIndex],
							PreyInstance = Main.player[playerPredIndex].AsPred().stomachContents.FirstOrDefault(x => x.Type == PreyType.NPC && x.Instance.whoAmI == npc.whoAmI)
						};
						break;
				}
			}
		}

		/// <summary>
		/// Deals the given amount of digestion damage to the NPC, respecting damage variation and, if their predator is a player, said player's luck.
		/// </summary>
		/// <param name="pred">The pred currently digesting this NPC.</param>
		/// <param name="digestionDamage">The total amount of digestion damage to be dealt, before damage variation calculations.</param>
		/// <returns>Whether or not the resulting digestion tick kills the NPC.</returns>
		public static bool TakeDigestionDamage(NPC npc, Entity pred, double digestionDamage)
		{
			if (npc.life <= 0)
				return true;

			int trueDigestionDamage = Main.DamageVar((float)digestionDamage, (pred is Player playerPred) ? -playerPred.luck : 0);
			if (ModContent.GetInstance<V2ServerConfig>().DefenseInDigestionCalcs)
			{
				trueDigestionDamage -= npc.defense / 2;
				if (trueDigestionDamage < 0)
					trueDigestionDamage = 0;
			}
			trueDigestionDamage = (int)Math.Floor(npc.AsFood().TakenDigestionDamageModifier.ApplyTo(trueDigestionDamage));
			npc.AsFood().SoftenedDigestionDamageTaken += npc.AsFood().SoftenedDigestionDamageModifier.ApplyTo(trueDigestionDamage);
			npc.AsFood().SoftenedWearoffDelay = SoftenedWearoffMaxDelay;
			if (npc.realLife != -1)
				Main.npc[npc.realLife].life -= trueDigestionDamage;
			else
				npc.life -= trueDigestionDamage;
			CombatText digestionText = Main.combatText[CombatText.NewText(
				npc.Hitbox,
				npc.friendly ? Color.DarkGreen : Color.LimeGreen,
				trueDigestionDamage,
				false,
				true
			)];
			digestionText.position.X = pred.Center.X;
			digestionText.position.X += pred.direction * 14;
			if (pred.direction == -1)
				digestionText.position.X -= ChatManager.GetStringSize(FontAssets.CombatText[0].Value, digestionText.text, new Vector2(digestionText.scale)).X;
			digestionText.position.Y = npc.Center.Y;
			digestionText.position.Y += npc.height / 5f;
			digestionText.velocity.X = pred.direction * 2.5f;
			digestionText.velocity.Y = -4f;
			if (npc.AsFood().DigestingHitSound.HasValue)
				SoundEngine.PlaySound(npc.AsFood().DigestingHitSound.Value with { Volume = 1f }, pred.position);
			else
				SoundEngine.PlaySound(npc.HitSound.Value with { Volume = 0.35f }, pred.position);
			if (npc.realLife != -1)
			{
				if (Main.npc[npc.realLife].life <= 0)
				{
					Main.npc[npc.realLife].life = 0;
					return true;
				}
			}
			else if (npc.life <= 0)
			{
				npc.life = 0;
				npc.checkDead();
				return true;
			}

			return false;
		}

		public static double GetCurrentTotalWeight(NPC npc)
		{
			double baseWeight = Prey.GetInitialPreySize(npc);
			double bellyWeight = PredNPC.GetCurrentBellyWeight(npc);
			return baseWeight + bellyWeight;
		}

		public override bool CheckActive(NPC npc)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				return false;

			return true;
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				return false;

			return true;
		}
	}
}

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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;

namespace V2.NPCs
{
	public static class PreyNPCStuff
	{
		public static PreyNPC AsPrey(this NPC npc, bool risky = false)
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

		public StatModifier StruggleStrength { get; set; }

		public bool CanChatAsPrey { get; set; }

		public SoundStyle? DigestingHitSound;
		public SoundStyle? DigestedDeathSound;

		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;

		public PreyNPC()
		{
			FoodTypeTags = null;
			FoodFlavorTags = null;

			PreyAIMethod = null;
			PreyBaseSizeOverrideMethod = null;

			StruggleStrength = StatModifier.Default;

			CanChatAsPrey = false;
			DigestingHitSound = null;
			DigestedDeathSound = null;
		}

		public override void ResetEffects(NPC npc)
		{
			npc.AsPrey().IsCurrentlyEaten = false;
			npc.AsPrey().Digested = false;
			npc.AsPrey().CurrentCaptor = null;
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

				if (pred.stomachContents.FirstOrDefault(x => !x.Dead && x.Type == PreyType.NPC && (x.Index == npc.whoAmI || x.Index == npc.realLife)) is Prey prey)
				{
					npc.AsPrey().IsCurrentlyEaten = true;
					npc.position = potentialPred.Center - (npc.Size / 2f);
					npc.AsPrey().CurrentCaptor = new PredEntityReference()
					{
						Predator = potentialPred,
						PreyInstance = prey
					};
					break;
				}
				if (pred.stomachContentsQueue.FirstOrDefault(x => !x.Dead && x.Type == PreyType.NPC && (x.Index == npc.whoAmI || x.Index == npc.realLife)) is Prey queuedPrey)
				{
					npc.AsPrey().IsCurrentlyEaten = true;
					npc.position = potentialPred.Center - (npc.Size / 2f);
					npc.AsPrey().CurrentCaptor = new PredEntityReference()
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

				if (potentialPred.AsPred().stomachContents.FirstOrDefault(x => !x.Dead && x.Type == PreyType.NPC && (x.Index == npc.whoAmI || x.Index == npc.realLife)) is Prey prey)
				{
					npc.AsPrey().IsCurrentlyEaten = true;
					npc.position = potentialPred.Center - (npc.Size / 2f);
					npc.AsPrey().CurrentCaptor = new PredEntityReference()
					{
						Predator = potentialPred,
						PreyInstance = prey
					};
					break;
				}
				if (potentialPred.AsPred().stomachContentsQueue.FirstOrDefault(x => !x.Dead && x.Type == PreyType.NPC && (x.Index == npc.whoAmI || x.Index == npc.realLife)) is Prey queuedPrey)
				{
					npc.AsPrey().IsCurrentlyEaten = true;
					npc.position = potentialPred.Center - (npc.Size / 2f);
					npc.AsPrey().CurrentCaptor = new PredEntityReference()
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
				npc.AsPrey().DigestingHitSound = DigestingHitSoundDatabase[npc.HitSound.Value];
			if (npc.DeathSound is not null && DigestedDeathSoundDatabase.ContainsKey(npc.DeathSound.Value))
				npc.AsPrey().DigestedDeathSound = DigestedDeathSoundDatabase[npc.DeathSound.Value];
		}

		public override bool CanHitNPC(NPC npc, NPC target)
		{
			if (npc.AsPrey().IsCurrentlyEaten || target.AsPrey().IsCurrentlyEaten || npc.AsPrey().EatenSafetyFrames > 0)
				return false;

			return true;
		}

		public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
		{
			if (npc.AsPrey().IsCurrentlyEaten || npc.AsPrey().EatenSafetyFrames > 0)
				return false;

			return true;
		}

		public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
		{
			if (npc.AsPrey().IsCurrentlyEaten)
				return false;

			return null;
		}

		public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
		{
			if (npc.AsPrey().IsCurrentlyEaten)
				return false;

			return null;
		}

		public override bool? CanBeCaughtBy(NPC npc, Item item, Player player)
		{
			if (npc.AsPrey().IsCurrentlyEaten)
				return false;

			return null;
		}

		public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
		{
			if (npc.AsPrey().IsCurrentlyEaten)
				boundingBox = Rectangle.Empty;
		}

		public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (target.type == ModContent.NPCType<Succubus>() && PredNPC.CanSwallow(target, npc))
			{
				modifiers.FinalDamage *= 0;
				modifiers.Knockback.Base = 0f;
				modifiers.DisableCrit();
			}
		}

		public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit)
		{
			if (target.type == ModContent.NPCType<Succubus>())
			{
				PredNPC.Swallow(target, npc);
			}
		}

		public override bool? CanChat(NPC npc)
		{
			if (npc.AsPrey().IsCurrentlyEaten)
				return npc.AsPrey().CanChatAsPrey;

			return null;
		}

		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			binaryWriter.Write(npc.AsPrey().IsCurrentlyEaten);
			binaryWriter.Write(npc.AsPrey().EatenSafetyFrames);
			binaryWriter.Write(npc.AsPrey().Digested);
			if (npc.AsPrey().IsCurrentlyEaten && npc.AsPrey().CurrentCaptor.HasValue)
			{
				binaryWriter.Write(true);
				Entity pred = npc.AsPrey().CurrentCaptor.Value.Predator;
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
			npc.AsPrey().IsCurrentlyEaten = binaryReader.ReadBoolean();
			npc.AsPrey().EatenSafetyFrames = binaryReader.ReadInt32();
			npc.AsPrey().Digested = binaryReader.ReadBoolean();
			if (npc.AsPrey().IsCurrentlyEaten && binaryReader.ReadBoolean())
			{
				switch (binaryReader.ReadString())
				{
					case "NPC pred":
						int npcPredIndex = binaryReader.ReadInt32();
						npc.AsPrey().CurrentCaptor = new PredEntityReference()
						{
							Predator = Main.npc[npcPredIndex],
							PreyInstance = Main.npc[npcPredIndex].AsPred().stomachContents.FirstOrDefault(x => x.Type == PreyType.NPC && x.Index == npc.whoAmI)
						};
						break;
					case "Player pred":
						int playerPredIndex = binaryReader.ReadInt32();
						npc.AsPrey().CurrentCaptor = new PredEntityReference()
						{
							Predator = Main.player[playerPredIndex],
							PreyInstance = Main.player[playerPredIndex].AsPred().stomachContents.FirstOrDefault(x => x.Type == PreyType.NPC && x.Index == npc.whoAmI)
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
		public bool TakeDigestionDamage(NPC npc, Entity pred, double digestionDamage)
		{
			if (npc.life <= 0)
				return true;

			int trueDigestionDamage = Main.DamageVar((float)digestionDamage, (pred is Player playerPred) ? -playerPred.luck : 0);
			if (ModContent.GetInstance<V2ServerSideConfigs>().DefenseInDigestionCalcs)
			{
				trueDigestionDamage -= npc.defense / 2;
				if (trueDigestionDamage < 0)
					trueDigestionDamage = 0;
			}
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
			digestionText.position.Y = npc.Center.Y;
			digestionText.position.Y += npc.height / 5f;
			digestionText.velocity.X = pred.direction * 2.5f;
			digestionText.velocity.Y = -4f;
			SoundEngine.PlaySound(npc.HitSound.Value with { Volume = 0.5f }, pred.position);
			if (npc.realLife != -1)
			{
				if (Main.npc[npc.realLife].life <= 0)
				{
					Main.npc[npc.realLife].life = 0;
					Main.npc[npc.realLife].checkDead();
					npc.AsPrey().Digested = true;
					return true;
				}
			}
			else if (npc.life <= 0)
			{
				npc.life = 0;
				npc.checkDead();
				npc.AsPrey().Digested = true;
				return true;
			}

			return false;
		}

		public static double GetCurrentTotalWeight(NPC npc)
		{
			double baseWeight = Prey.GetInitialPreyWeight(npc);
			double bellyWeight = PredNPC.GetCurrentBellyWeight(npc);
			return baseWeight + bellyWeight;
		}

		public override bool CheckActive(NPC npc)
		{
			if (npc.AsPrey().IsCurrentlyEaten)
				return false;

			return true;
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.AsPrey().IsCurrentlyEaten)
				return false;

			return true;
		}
	}
}

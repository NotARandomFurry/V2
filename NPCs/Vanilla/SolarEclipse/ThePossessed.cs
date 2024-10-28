using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.SolarEclipse
{
	public static class ThePossessedStuff
	{
		public static ThePossessed AsThePossessed(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out ThePossessed lacewing))
				throw new Exception("this instance of a The Possessed, supposedly, doesn't exist");

			return lacewing;
		}
	}

	public partial class ThePossessed : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.ThePossessed;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsV2NPC().NewAIMethod = V2ThePossessedAI;

			npc.AsFood().DefinedBaseSize = 1.10;
			npc.AsPred().MaxStomachCapacity = 4.44;
			npc.AsPred().BaseStomachacheMeterCapacity = 800.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.40;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().CanBeForceFed = CanPossessedBeForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 0.40;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
			npc.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
		}

		public static bool CanPossessedBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange([
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.SolarEclipse.ThePossessed.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.SolarEclipse.ThePossessed.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.SolarEclipse.ThePossessed.3",
			]);
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.SolarEclipse.ThePossessed.Hardcore");
			}
		}

		public override void PostAI(NPC npc)
		{
			List<(TargetType, int)> diet =
			[
				// Town NPCs
				(TargetType.NPC, NPCID.Guide),
				(TargetType.NPC, NPCID.Merchant),
				(TargetType.NPC, NPCID.Nurse),
				(TargetType.NPC, NPCID.Demolitionist),
				(TargetType.NPC, NPCID.DyeTrader),
				(TargetType.NPC, NPCID.BestiaryGirl),
				(TargetType.NPC, NPCID.Dryad),
				(TargetType.NPC, ModContent.NPCType<LucindaBound>()),
				(TargetType.NPC, ModContent.NPCType<Lucinda>()),
				(TargetType.NPC, NPCID.Painter),
				(TargetType.NPC, NPCID.GolferRescue),
				(TargetType.NPC, NPCID.Golfer),
				(TargetType.NPC, NPCID.ArmsDealer),
				(TargetType.NPC, NPCID.TravellingMerchant),
				(TargetType.NPC, NPCID.BartenderUnconscious),
				(TargetType.NPC, NPCID.DD2Bartender),
				(TargetType.NPC, NPCID.WebbedStylist),
				(TargetType.NPC, NPCID.Stylist),
				(TargetType.NPC, NPCID.Clothier),
				(TargetType.NPC, NPCID.BoundMechanic),
				(TargetType.NPC, NPCID.Mechanic),
				(TargetType.NPC, NPCID.PartyGirl),
				(TargetType.NPC, NPCID.BoundWizard),
				(TargetType.NPC, NPCID.Wizard),
				(TargetType.NPC, NPCID.TaxCollector),
				(TargetType.NPC, NPCID.Pirate),
				(TargetType.NPC, NPCID.Steampunker),

				// Other meals
				(TargetType.NPC, NPCID.Harpy),
				(TargetType.NPC, NPCID.Harpy),

				// Pirates
				(TargetType.NPC, NPCID.PirateCorsair),
				(TargetType.NPC, NPCID.PirateCrossbower),
				(TargetType.NPC, NPCID.PirateDeadeye),
				(TargetType.NPC, NPCID.PirateDeckhand),
				(TargetType.NPC, NPCID.PirateCaptain),

				// Lamia
				(TargetType.NPC, NPCID.DesertLamiaDark),
				(TargetType.NPC, NPCID.DesertLamiaLight),

				// Misc. humanoid NPCs
				(TargetType.NPC, NPCID.LostGirl),
				(TargetType.NPC, NPCID.Nymph),

				// Players, of course
				(TargetType.Player, -1),
			];
			if (PredNPC.GetCurrentBellyWeight(npc) < npc.AsPred().MaxStomachCapacity * 0.80f)
				npc.DoContactGulpage(diet);
		}

		public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
		{
			base.ModifyHitNPC(npc, target, ref modifiers);
		}

		public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
		{
			double fullnessRatio = PredNPC.GetCurrentBellyWeight(npc) / npc.AsPred().MaxStomachCapacity;
			modifiers.FinalDamage *= 0.20f + Math.Max(0.80f - (float)fullnessRatio, 0f);
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 2.5;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			double baseDigestionTickDamage = 24.0;
			return baseDigestionTickDamage;
		}
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 2,
				seconds: 0
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.375 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				10
			);
		}

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(4.0 * Math.Sqrt(npc.AsPred().ExtraWeight)),
				0
			);
		}

		public static int GetEmpressDigestionStage(NPC npc)
		{
			if (PredNPC.GetStomachTracker(npc) is null)
				return 0;

			PreyData candyFairy = PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss);
			if (candyFairy is null || candyFairy.WeightLeftToDigest < 6.0)
				return 0;
			else
			{
				if (!candyFairy.NoHealth)
					return 1;
				else
				{
					if (candyFairy.WeightLeftToDigest > 34.0)
						return 1;
					else if (candyFairy.WeightLeftToDigest > 28.0)
						return 2;
					else if (candyFairy.WeightLeftToDigest > 16.0)
						return 3;
					else if (candyFairy.WeightLeftToDigest > 6.0)
						return 4;
					else
						return 0;
				}
			}
		}

		public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
		{
			if (PredNPC.GetCurrentBellyWeight(npc) > 0)
			{
				boundingBox = new Rectangle(
					(int)npc.Center.X - (npc.AsPred().GetVisualBellySize.Invoke(npc) switch
					{
						0 => 21,
						1 => 21,
						2 => 21,
						3 => 21,
						4 => 21,
						5 => 21,
						6 => 22,
						7 => 25,
						8 => 28,
						9 => 31,
						10 => 34,
						_ => 21,
					}),
					(int)npc.Center.Y - (npc.AsPred().GetVisualBellySize.Invoke(npc) switch
					{
						0 => 16,
						1 => 16,
						2 => 16,
						3 => 18,
						4 => 19,
						5 => 20,
						6 => 22,
						7 => 24,
						8 => 26,
						9 => 30,
						10 => 32,
						_ => 16,
					}),
					npc.AsPred().GetVisualBellySize.Invoke(npc) switch
					{
						0 => 42,
						1 => 42,
						2 => 42,
						3 => 42,
						4 => 42,
						5 => 42,
						6 => 44,
						7 => 50,
						8 => 56,
						9 => 62,
						10 => 66,
						_ => 42,
					},
					npc.AsPred().GetVisualBellySize.Invoke(npc) switch
					{
						0 => 32,
						1 => 32,
						2 => 32,
						3 => 36,
						4 => 38,
						5 => 40,
						6 => 44,
						7 => 48,
						8 => 52,
						9 => 60,
						10 => 64,
						_ => 32,
					}
				);
			}
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.CurrentCaptor() is not null)
				return false;
			
			if (PredNPC.GetCurrentBellyWeight(npc) > 0)
			{
				string possessedGluttonTummyTypeString = "_BasicBellySheet";
				string possessedGluttonWeightStageString = GetVisualWeightStage(npc) > 0 ? ("_WeightGain" + GetVisualWeightStage(npc)) : "_BaseWeight";
				string exactPossessedGluttonTextureString = "V2/NPCs/Vanilla/SolarEclipse/ThePossessed" + possessedGluttonWeightStageString + possessedGluttonTummyTypeString;

				SpriteEffects spriteEffects = npc.direction switch
				{
					-1 => SpriteEffects.None,
					_ => SpriteEffects.FlipHorizontally,
				};
				Texture2D texture = ModContent.Request<Texture2D>(exactPossessedGluttonTextureString, AssetRequestMode.ImmediateLoad).Value;
				Rectangle sourceRectangle = new Rectangle(72 * npc.AsPred().GetVisualBellySize.Invoke(npc), 0, 70, 70);
				Vector2 origin = new Vector2(35f, 55f);
				spriteBatch.Draw
				(
					texture,
					npc.Center - screenPos + new Vector2(0f, npc.gfxOffY),
					sourceRectangle,
					drawColor,
					npc.rotation,
					origin,
					1,
					spriteEffects,
					0f
				);
				
				return false;
			}
			return true;
		}
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.BloodMoon
{
	public static partial class TheBrideStuff
	{
		public static float GroundedAccel(NPC npc) => TheBride.GetVisualWeightStage(npc) switch
		{
			0 => Main.GameModeInfo.IsMasterMode ? 0.100f : (Main.GameModeInfo.IsExpertMode ? 0.075f : 0.050f),
			1 => Main.GameModeInfo.IsMasterMode ? 0.080f : (Main.GameModeInfo.IsExpertMode ? 0.060f : 0.040f),
			2 => Main.GameModeInfo.IsMasterMode ? 0.060f : (Main.GameModeInfo.IsExpertMode ? 0.045f : 0.030f),
			_ => 0.07f,
		};
		public static float GroundedMaxSpeed(NPC npc) => TheBride.GetVisualWeightStage(npc) switch
		{
			0 => Main.GameModeInfo.IsMasterMode ? 1.250f : (Main.GameModeInfo.IsExpertMode ? 1.100f : 1.000f),
			1 => Main.GameModeInfo.IsMasterMode ? 1.125f : (Main.GameModeInfo.IsExpertMode ? 0.990f : 0.900f),
			2 => Main.GameModeInfo.IsMasterMode ? 1.000f : (Main.GameModeInfo.IsExpertMode ? 0.880f : 0.750f),
			_ => 0.07f,
		};
		public static float InitJumpSpeed(NPC npc) => TheBride.GetVisualWeightStage(npc) switch
		{
			0 => Main.GameModeInfo.IsMasterMode ? 8.000f : (Main.GameModeInfo.IsExpertMode ? 7.000f : 6.000f),
			1 => Main.GameModeInfo.IsMasterMode ? 6.400f : (Main.GameModeInfo.IsExpertMode ? 5.600f : 4.800f),
			2 => Main.GameModeInfo.IsMasterMode ? 4.800f : (Main.GameModeInfo.IsExpertMode ? 4.200f : 3.600f),
			_ => 0.07f,
		};
		public static class ItemTheftRules
		{
			public static DigestionLootRule WeddingVeil => new DigestionLootRule(
				type: (npc, pred) => ItemID.TheBrideHat,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					if (pred is not Player predPlayer)
						return 0.40;

					return predPlayer.AsPred().PreyStealLootLevel switch
					{
						0 => 0.20,
						1 => 0.40,
						2 => 0.75,
						_ => 1.00,
					};
				}
			);
			public static DigestionLootRule WeddingDress => new DigestionLootRule(
				type: (npc, pred) => ItemID.TheBrideDress,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					if (pred is not Player predPlayer)
						return 0.40;

					return predPlayer.AsPred().PreyStealLootLevel switch
					{
						0 => 0.20,
						1 => 0.40,
						2 => 0.75,
						_ => 1.00,
					};
				}
			);
		}

		public static TheBride AsTheBride(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out TheBride hungryZombieWife))
				throw new Exception("this instance of The Bride, supposedly, doesn't exist");

			return hungryZombieWife;
		}
	}

	public partial class TheBride : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.TheBride;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.aiStyle = -1;
			npc.AsV2NPC().NewAIMethod = V2TheBrideAI;
			npc.AsV2NPC().TargetRange = V2Utils.TileCountAsPixelCount(15.0);
			npc.AsV2NPC().TargetRequiresLineOfSight = true;

			npc.AsFood().DefinedBaseSize = 1.04;
			npc.AsPred().MaxStomachCapacity = 2.45;
			npc.AsPred().BaseStomachacheMeterCapacity = 90.0;
			npc.AsPred().GetStomachacheSootheRate = GetStomachacheSootheRate;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.5;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(8.0);
			npc.AsPred().CanBeForceFed = CanTheBrideBeForceFed;
			npc.AsPred().OnForceFed = OnTheBrideForceFed;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
			npc.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			npc.AsPred().AssociatedStruggleChart = new TheBrideStuff.TheBrideStruggleChart();

			npc.AsPred().StandardBurps = Burps.Humanoid.Zombie.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantBrideAndGroomGoal;
			npc.AsFood().ItemTheftRules =
			[
				TheBrideStuff.ItemTheftRules.WeddingVeil,
				TheBrideStuff.ItemTheftRules.WeddingDress,
			];
		}

		public override void OnSpawn(NPC npc, IEntitySource source)
		{
			npc.direction = Main.rand.NextBool().ToDirectionInt();
			npc.target = -1;
			npc.AsV2NPC().BehaviorPattern = Main.rand.NextBool() ? new TheBrideAI.AimlessWanderingWalking() : new TheBrideAI.AimlessWanderingStill();
		}

		public static double GetStomachacheSootheRate(NPC npc)
		{
			if (PredNPC.AnyPreyStillAlive(npc))
				return 0.0;

			return 1.0;
		}

		public static bool CanTheBrideBeForceFed(NPC npc) => true;

		public static void OnTheBrideForceFed(NPC npc, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(
			[
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.Zombie.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.Zombie.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.Zombie.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.BloodMoon.GroomAndBride.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.BloodMoon.GroomAndBride.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.BloodMoon.GroomAndBride.TheBride.1",
			]);
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.BloodMoon.GroomAndBride.TheBride.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.8;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 17;

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 12,
				seconds: 0
			);
			return baseAbsorptionRate;
		}

		public static int GetEmpressDigestionStage(NPC npc)
		{
			if (PredNPC.GetStomachTracker(npc) is null)
				return 0;

			PreyData candyFairy = PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss);
			if (candyFairy is null || candyFairy.WeightLeftToDigest < 4.0)
				return 0;
			else
			{
				if (!candyFairy.NoHealth)
					return 1;
				else
				{
					if (candyFairy.WeightLeftToDigest > 37.0)
						return 1;
					else if (candyFairy.WeightLeftToDigest > 34.0 && candyFairy.WeightLeftToDigest <= 37.0)
						return 2;
					else if (candyFairy.WeightLeftToDigest > 28.5 && candyFairy.WeightLeftToDigest <= 34.0)
						return 3;
					else if (candyFairy.WeightLeftToDigest > 4.0)
						return 4;
					else
						return 0;
				}
			}
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				6
			);
		}

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(0.20 * Math.Sqrt(npc.AsPred().ExtraWeight)),
				2
			);
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
			npc.frame.Width = 150;
			npc.spriteDirection = npc.direction;
		}

		public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
		{
			if (GetEmpressDigestionStage(npc) > 0)
			{
				boundingBox = new Rectangle(
					(int)npc.Left.X,
					(int)npc.Top.Y,
					114,
					54
				);
			}
			else
			{
				boundingBox = new Rectangle(
					(int)npc.Center.X - 12,
					(int)npc.Center.Y - 25,
					34,
					50
				);
			}
		}

		public static void OnKilledByDigestion_GrantBrideAndGroomGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				bool eatenGroom = predPlayer.AsPred().mealCount.ContainsKey("Terraria: The Groom") && predPlayer.AsPred().mealCount["Terraria: The Groom"] > 0;
				if (eatenGroom)
					ModContent.GetInstance<EatBrideAndGroom>().TrySetCompletion(predPlayer);
			}
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			int weightStage = npc.AsPred().GetVisualWeightStage.Invoke(npc);
			string weightString = "_Weight" + (weightStage == 0 ? "Base" : weightStage);
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);

			string exactMainBodyTexture = "V2/NPCs/Vanilla/BloodMoon/TheBride" + weightString + bellyString;
			TextureAssets.Npc[NPCID.TheBride] = ModContent.Request<Texture2D>(exactMainBodyTexture, AssetRequestMode.ImmediateLoad);
			return true;
		}
	}
}

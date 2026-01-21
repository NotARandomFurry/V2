using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Vanilla.Placeables.PlaceableTiles;
using V2.NPCs.Vanilla.Sky;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;
using V2.Sounds.Vore;
using static System.Net.Mime.MediaTypeNames;

namespace V2.NPCs.Vanilla.Rain
{
	public static class FlyingFishStuff
	{
		public static FlyingFish AsFlyingFish(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out FlyingFish flyingFish))
				throw new Exception("this instance of a Flying Fish, supposedly, doesn't exist");

			return flyingFish;
		}
	}

	public partial class FlyingFish : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.FlyingFish;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 0.6;
			npc.AsFood().WellFedPower = 0.25;

			npc.lifeMax = 100;

			npc.aiStyle = -1;
			npc.AsV2NPC().NewAIMethod = V2FlyingFishAI;
			npc.AsV2NPC().TargetRange = V2Utils.TileCountAsPixelCount(70.0);
			npc.AsV2NPC().TargetRequiresLineOfSight = true;

			npc.AsFood().DefinedBaseSize = 1.335;
			npc.AsPred().MaxStomachCapacity = 1.9;
			npc.AsPred().BaseStomachacheMeterCapacity = 250.0;
			npc.AsPred().WeightGainRatio = 0.15;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.35;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(4.7);
			npc.AsPred().CanBeForceFed = CanHarpyBeForceFed;
			npc.AsPred().OnForceFed = OnHarpyForceFed;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
			npc.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 0.35;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantFlyingFishGoal;

			npc.AsFood().ItemTheftRules = [
				HarpyStuff.ItemTheftRules.GiantHarpyFeather,
			];

		}

		public static bool V2FlyingFishAI(NPC npc)
		{
			npc.noGravity = true;
			npc.ai[3]++;
			if (npc.ai[3] > 15) npc.ai[3] = 0;
			Entity targetEntity = null;
			if (npc.AsV2NPC().TargetIndex == -1)
				npc.TryFindNewTarget(Diet);
			else
				npc.TryVerifyRemainingTarget(Diet);
			if (npc.AsV2NPC().TargetIndex != -1)
			{
				targetEntity = npc.AsV2NPC().TargetType switch
				{
					TargetType.Player => Main.player[npc.AsV2NPC().TargetIndex],
					TargetType.NPC => Main.npc[npc.AsV2NPC().TargetIndex],
					TargetType.Projectile => Main.projectile[npc.AsV2NPC().TargetIndex],
					_ => null,
				};
			}

			if (npc.AsV2NPC().BehaviorPattern is not null)
				npc.AsV2NPC().BehaviorPattern.DoBehavior(npc, targetEntity);
			else
				npc.SwitchToPattern<HarpyAI.MainFlying>(targetEntity);

			return false;
		}

		public static bool CanHarpyBeForceFed(NPC npc) => true;

		public static void OnHarpyForceFed(NPC npc, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange([
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.5",
			]);
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 1.25;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			return Main.GameMode switch
			{
				GameModeID.Creative => 8 * CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>().StrengthMultiplierToGiveNPCs,
				GameModeID.Master => 15,
				GameModeID.Expert => 10,
				_ => 8,
			};
		}

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{

		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 6,
				seconds: 45
			);
			baseAbsorptionRate *= Math.Pow(1.2, GetVisualWeightStage(npc));
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(6.5 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				4
			);
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
		}

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(Math.Sqrt(8) * Math.Sqrt(npc.AsPred().ExtraWeight)),
				0
			);
		}

		public static void OnKilledByDigestion_GrantFlyingFishGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatFlyingFish>().TrySetCompletion(predPlayer);
		}
	}
}

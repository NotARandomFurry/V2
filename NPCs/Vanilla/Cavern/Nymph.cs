using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Cavern
{
	public static class NymphStuff
	{
		public static class ItemTheftRules
		{
			public static ItemTheftRule NymphHairStrands => new ItemTheftRule(
				type: (npc, pred) => ModContent.ItemType<NymphHairStrand>(),
				amount: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 2,
						GameModeID.Expert => Main.rand.Next(1, 2 + 1),
						_ => 1,
					};
				},
				chance: (npc, pred) => 1f
			);
			public static ItemTheftRule MetalDetector => new ItemTheftRule(
				type: (npc, pred) => ItemID.MetalDetector,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 0.175f,
						GameModeID.Expert => 0.14f,
						_ => 0.10f,
					};
				}
			);
		}
		public static Nymph AsNymph(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Nymph cuteGirlLure))
				throw new Exception("this instance of a Nymph, supposedly, doesn't exist");

			return cuteGirlLure;
		}
	}

	public class Nymph : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type is NPCID.LostGirl or NPCID.Nymph;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsFood().Size = 1.04;
			npc.AsPred().MaxStomachCapacity = 5.5;
			npc.AsPred().BaseStomachacheMeterCapacity = 275.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.5;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(4.7);
			npc.AsPred().CanBeForceFed = CanNymphBeForceFed;
			npc.AsPred().OnForceFed = OnNymphForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 0.65;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
			npc.AsFood().OnKilledByDigestion += OnKilledByDigestion_GrantNymphGoal;

			npc.AsFood().ItemTheftRules = new List<ItemTheftRule>()
			{
				NymphStuff.ItemTheftRules.NymphHairStrands,
				NymphStuff.ItemTheftRules.MetalDetector,
			};
		}

		public static bool CanNymphBeForceFed(NPC npc) => false;

		public static void OnNymphForceFed(NPC npc, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Cavern.Nymph.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Cavern.Nymph.2",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Cavern.Nymph.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 1.4;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 22;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{

		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 1,
				seconds: 15
			);
			return baseAbsorptionRate;
		}

		public static void OnKilledByDigestion_GrantNymphGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<EatNymph>().TrySetCompletion(predPlayer);
			}
		}
	}
}

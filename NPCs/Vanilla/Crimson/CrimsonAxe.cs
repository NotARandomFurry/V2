using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria;
using V2.Items.Voraria.Charms;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Crimson
{
	public static class CrimsonAxeStuff
	{
		public static class ItemTheftRules
		{
			public static ItemTheftRule Crimtane => new ItemTheftRule(
				type: (npc, pred) => ItemID.CrimtaneOre,
				amount: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => Main.rand.Next(15, 25 + 1),
						GameModeID.Expert => Main.rand.Next(12, 20 + 1),
						_ => Main.rand.Next(9, 15 + 1),
					};
				},
				chance: (npc, pred) => 1.0
			);
		}

		public static CrimsonAxe AsCrimsonAxe(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out CrimsonAxe fatFuckAxe))
				throw new Exception("this instance of a Crimson Axe, supposedly, doesn't exist");

			return fatFuckAxe;
		}
	}

	public partial class CrimsonAxe : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.CrimsonAxe;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsFood().DefinedBaseSize = 1.04;
			npc.AsPred().MaxStomachCapacity = 5.5;
			npc.AsPred().BaseStomachacheMeterCapacity = 275.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.5;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(4.7);
			npc.AsPred().CanBeForceFed = CanCrimsonAxeBeForceFed;
			npc.AsPred().OnForceFed = OnCrimsonAxeForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().SmallBurps = null;
			npc.AsPred().SmallBurpThreshold = 0.50;
			npc.AsPred().StandardBurps = null;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().ItemTheftRules = [
				CrimsonAxeStuff.ItemTheftRules.Crimtane,
			];
		}

		public static bool CanCrimsonAxeBeForceFed(NPC npc) => true;

		public static void OnCrimsonAxeForceFed(NPC npc, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Crimson.CrimsonAxe.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Crimson.CrimsonAxe.2",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Crimson.CrimsonAxe.Hardcore");
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

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			npcLoot.Add(
				new V2CommonDropRules.DifficultyScalingDrop(
					new CommonDrop(
						itemId: ModContent.ItemType<CharmFatass>(),
						chanceNumerator: 1,
						chanceDenominator: 10
					),
					new CommonDrop(
						itemId: ModContent.ItemType<CharmFatass>(),
						chanceNumerator: 3,
						chanceDenominator: 20
					),
					new CommonDrop(
						itemId: ModContent.ItemType<CharmFatass>(),
						chanceNumerator: 1,
						chanceDenominator: 5
					)
				)
			);
		}
	}
}

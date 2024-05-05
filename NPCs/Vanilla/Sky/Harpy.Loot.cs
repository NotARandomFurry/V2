using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Consumables;

namespace V2.NPCs.Vanilla.Sky
{
	public partial class Harpy : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			npcLoot.RemoveWhere(x => x is CommonDropNotScalingWithLuck drop && drop.itemId == ItemID.Feather);
			npcLoot.RemoveWhere(x => x is CommonDrop drop && drop.itemId == ItemID.GiantHarpyFeather);
			npcLoot.RemoveWhere(x => x is ItemDropWithConditionRule drop && drop.itemId == ItemID.ChickenNugget);

			npcLoot.Add(new V2CommonDropRules.DifficultyScalingDrop(
				new CommonDrop(
					itemId: ItemID.Feather,
					chanceNumerator: 1,
					chanceDenominator: 1,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 3
				),
				new CommonDrop(
					itemId: ItemID.Feather,
					chanceNumerator: 1,
					chanceDenominator: 1,
					amountDroppedMinimum: 2,
					amountDroppedMaximum: 3
				),
				new CommonDrop(
					itemId: ItemID.Feather,
					chanceNumerator: 1,
					chanceDenominator: 1,
					amountDroppedMinimum: 3,
					amountDroppedMaximum: 3
				)
			));
			npcLoot.Add(new V2CommonDropRules.DifficultyScalingDrop(
				new V2CommonDropRules.RerollsBasedOnWeightLevelRule(
					itemId: ItemID.GiantHarpyFeather,
					chanceNumerator: 1,
					chanceDenominator: 50,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1,
					minimumWeightLevel: 1
				),
				new V2CommonDropRules.RerollsBasedOnWeightLevelRule(
					itemId: ItemID.GiantHarpyFeather,
					chanceNumerator: 1,
					chanceDenominator: 40,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1,
					minimumWeightLevel: 1
				),
				new V2CommonDropRules.RerollsBasedOnWeightLevelRule(
					itemId: ItemID.GiantHarpyFeather,
					chanceNumerator: 1,
					chanceDenominator: 30,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1,
					minimumWeightLevel: 1
				)
			));
			npcLoot.Add(new V2CommonDropRules.DifficultyScalingDrop(
				new V2CommonDropRules.RerollsBasedOnWeightLevelRule(
					itemId: ItemID.ChickenNugget,
					chanceNumerator: 1,
					chanceDenominator: 40,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1,
					minimumWeightLevel: 0
				),
				new V2CommonDropRules.RerollsBasedOnWeightLevelRule(
					itemId: ItemID.ChickenNugget,
					chanceNumerator: 1,
					chanceDenominator: 30,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1,
					minimumWeightLevel: 0
				),
				new V2CommonDropRules.RerollsBasedOnWeightLevelRule(
					itemId: ItemID.ChickenNugget,
					chanceNumerator: 1,
					chanceDenominator: 25,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1,
					minimumWeightLevel: 0
				)
			));
			npcLoot.Add(new V2CommonDropRules.DifficultyScalingDrop(
				new CommonDrop(
					itemId: ModContent.ItemType<FeatherDuster>(),
					chanceNumerator: 1,
					chanceDenominator: 10,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1
				),
				new CommonDrop(
					itemId: ModContent.ItemType<FeatherDuster>(),
					chanceNumerator: 1,
					chanceDenominator: 8,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1
				),
				new CommonDrop(
					itemId: ModContent.ItemType<FeatherDuster>(),
					chanceNumerator: 1,
					chanceDenominator: 6,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1
				)
			));
		}
	}
}

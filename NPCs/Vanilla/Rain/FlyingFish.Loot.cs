using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria;
using V2.Items.Voraria.Consumables;

namespace V2.NPCs.Vanilla.Rain
{
	public partial class FlyingFish : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			npcLoot.RemoveWhere(x => x is CommonDropNotScalingWithLuck drop && drop.itemId == ItemID.Glowstick);
			npcLoot.RemoveWhere(x => x is CommonDrop drop && drop.itemId == ItemID.CarbonGuitar);
			npcLoot.RemoveWhere(x => x is ItemDropWithConditionRule drop && drop.itemId == ItemID.Fries);

			npcLoot.Add(new V2CommonDropRules.DifficultyScalingDrop(
				new CommonDrop(
					itemId: ItemID.CarbonGuitar,
					chanceNumerator: 1,
					chanceDenominator: 100,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1
				),
				new CommonDrop(
					itemId: ItemID.CarbonGuitar,
					chanceNumerator: 1,
					chanceDenominator: 100,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1
				),
				new CommonDrop(
					itemId: ItemID.CarbonGuitar,
					chanceNumerator: 1,
					chanceDenominator: 100,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1
				)
			));
			npcLoot.Add(new V2CommonDropRules.DifficultyScalingDrop(
				new CommonDrop(
					itemId: ItemID.Glowstick,
					chanceNumerator: 1,
					chanceDenominator: 1,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 4
				),
				new CommonDrop(
					itemId: ItemID.Glowstick,
					chanceNumerator: 1,
					chanceDenominator: 1,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 5
				),
				new CommonDrop(
					itemId: ItemID.Glowstick,
					chanceNumerator: 1,
					chanceDenominator: 1,
					amountDroppedMinimum: 2,
					amountDroppedMaximum: 5
				)
			));
			npcLoot.Add(new V2CommonDropRules.DifficultyScalingDrop(
				new CommonDrop(
					itemId: ItemID.Fries,
					chanceNumerator: 1,
					chanceDenominator: 50,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1
				),
				new CommonDrop(
					itemId: ItemID.Fries,
					chanceNumerator: 2,
					chanceDenominator: 75,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1
				),
				new CommonDrop(
					itemId: ItemID.Fries,
					chanceNumerator: 1,
					chanceDenominator: 30,
					amountDroppedMinimum: 1,
					amountDroppedMaximum: 1
				)
			));
			npcLoot.Add(new V2CommonDropRules.DifficultyScalingDrop(
				new CommonDrop(
					itemId: ModContent.ItemType<FlyingFishScale>(),
					chanceNumerator: 1,
					chanceDenominator: 1,
					amountDroppedMinimum: 3,
					amountDroppedMaximum: 7
				),
				new CommonDrop(
					itemId: ModContent.ItemType<FlyingFishScale>(),
					chanceNumerator: 1,
					chanceDenominator: 1,
					amountDroppedMinimum: 4,
					amountDroppedMaximum: 9
				),
				new CommonDrop(
					itemId: ModContent.ItemType<FlyingFishScale>(),
					chanceNumerator: 1,
					chanceDenominator: 1,
					amountDroppedMinimum: 6,
					amountDroppedMaximum: 12
				)
			));
		}
	}
}

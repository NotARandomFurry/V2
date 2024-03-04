using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Consumables.Potions;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Buffs
{
	public class StomachacheMeterCapacityPotionBuff : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.StomachacheMeterCapacityPotionBuff.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.StomachacheMeterCapacityPotionBuff.Description");

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			rare = ItemRarityID.Yellow;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Voraria.Buffs.StomachacheMeterCapacityPotionBuff.Description",
				new
				{
					StomachacheMeterCapacityPotionMeterCapacityBonus = StomachacheMeterCapacityPotion.StomachacheMeterCapacityBonus.ToPercentage(3),
					StomachacheMeterCapacityPotionUneaseDefenseBonus = StomachacheMeterCapacityPotion.StomachacheDefenseBonus,
				}
			);
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.AsPred().StomachacheMeterCapacityModifier += (float)StomachacheMeterCapacityPotion.StomachacheMeterCapacityBonus;
			player.AsPred().StomachacheDefense.Base += StomachacheMeterCapacityPotion.StomachacheDefenseBonus;
		}
	}
}

using Microsoft.Xna.Framework.Input;
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
using V2.PlayerHandling;

namespace V2.Items.Voraria.Charms
{
	public class CharmLessStomachWeight : ModItem
	{
		public static double MaxWeightReduction => 0.20;
		public static double FullnessEffectivenessLossThreshold => 0.75;
		public static double WeightReductionEffectiveness(Player player)
		{
			double stomachCapacityPercent = player.AsPred().StomachFullness / player.AsPred().StomachCapacity;
			return Math.Min(1.0 - stomachCapacityPercent, 1.0 - FullnessEffectivenessLossThreshold) / (1.0 - FullnessEffectivenessLossThreshold);
		}

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Charms.LessStomachWeight");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Charms.LessStomachWeight.Short");

		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.AsCharm().IsValidCharm = true;
			Item.AsCharm().CharmEffects = CharmEffects;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(
				gold: 5
			);
		}

		public static void CharmEffects(Player player)
		{
			player.AsPred().StomachWeightModifier -= (float)(MaxWeightReduction * WeightReductionEffectiveness(player));
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicTooltip(
				"Voraria.Charms.LessStomachWeight",
				new
				{
					WeightReduction = MaxWeightReduction.ConvertToPercentageString(2),
					FullnessEffectivenessLossThreshold = FullnessEffectivenessLossThreshold.ConvertToPercentageString(2),
					CurrentWeightReduction = (MaxWeightReduction * WeightReductionEffectiveness(Main.LocalPlayer)).ConvertToPercentageString(2),
				}
			);
		}
	}
}

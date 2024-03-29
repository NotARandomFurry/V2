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
	public class CharmRegenFromAbsorption : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public static double HealthRegenerationRatio => 1.5;
		public static double ManaRegenerationRatio => 4.25;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Charms.RegenFromAbsorption");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Charms.RegenFromAbsorption.Short");

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
			if (player.AsPred().StomachTracker?.Prey.Count > 0)
			{
				double effectiveness = (double)player.AsPred().StomachTracker?.Prey.FindAll(x => x.NoHealth).Count / (double)player.AsPred().StomachTracker?.Prey.Count;
				player.AsPred().specialHealthRegenCount += HealthRegenerationRatio * player.AsPred().PreyAbsorptionRatePerTick * effectiveness;
				player.AsPred().specialManaRegenCount += ManaRegenerationRatio * player.AsPred().PreyAbsorptionRatePerTick * effectiveness;
			}
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			double regenEffectiveness = 0.0;
			if (player.AsPred().StomachTracker?.Prey.Count > 0)
				regenEffectiveness = (double)player.AsPred().StomachTracker?.Prey.FindAll(x => x.NoHealth).Count / (double)player.AsPred().StomachTracker?.Prey.Count;
			tooltips.AddVorariaDynamicItemTooltip(
				"Voraria.Charms.RegenFromAbsorption",
				new
				{
					HealthRegenerationRatio = HealthRegenerationRatio.ToPercentage(0),
					ManaRegenerationRatio = ManaRegenerationRatio.ToPercentage(0),
					RegenEffectiveness = regenEffectiveness.ToPercentage(2),
					LivePreyRemaining = player.AsPred().StomachTracker?.Prey.FindAll(x => !x.NoHealth).Count,
					PreyRemaining = player.AsPred().StomachTracker?.Prey.Count,
					CurrentHealthRegen = ((regenEffectiveness > 0.0 ? HealthRegenerationRatio * player.AsPred().PreyAbsorptionRatePerTick * regenEffectiveness : 0.0) * 60.0).CastToDecimalPlaces(2),
					CurrentManaRegen = ((regenEffectiveness > 0.0 ? ManaRegenerationRatio * player.AsPred().PreyAbsorptionRatePerTick * regenEffectiveness : 0.0) * 60.0).CastToDecimalPlaces(2),
				}
			);
		}
	}
}

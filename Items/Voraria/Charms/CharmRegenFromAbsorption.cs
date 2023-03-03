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
		public static double HealthRegenerationPerPlayerSizeDigested => 18.0;

		public override void SetDefaults()
		{
			Item.SetNameOverride(Language.GetTextValue("Mods.V2.ItemName.Voraria.Charms.RegenFromAbsorption"));

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
			if (player.AsPred().stomachContents.Count > 0)
			{
				double effectiveness = (double)player.AsPred().stomachContents.FindAll(x => x.Dead).Count / (double)player.AsPred().stomachContents.Count;
				player.AsPred().specialHealthRegenCount += (HealthRegenerationPerPlayerSizeDigested * player.AsPred().PreyAbsorptionRate * effectiveness).CastToDecimalPlaces(2);
				player.AsPred().specialManaRegenCount += (HealthRegenerationPerPlayerSizeDigested * player.AsPred().PreyAbsorptionRate * effectiveness).CastToDecimalPlaces(2);
			}
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.RemoveAll(x => x.Name.Contains("Tooltip"));
			if (V2Utils.FindLastTooltipLineBeforeFlavorText(tooltips, out TooltipLine line))
			{
				Player player = Main.LocalPlayer;
				double regenEffectiveness = 0.0;
				if (player.AsPred().stomachContents.Count > 0)
					regenEffectiveness = (double)player.AsPred().stomachContents.FindAll(x => x.Dead).Count / (double)player.AsPred().stomachContents.Count;
				V2Utils.InsertNewTooltipLine(
					ref tooltips,
					line,
					1,
					"Tooltip",
					!Main.keyState.IsKeyDown(Keys.LeftShift)
					  ? Language.GetTextValue("Mods.V2.ItemTooltip.Voraria.Charms.RegenFromAbsorption.Short")
					  : Language.GetTextValueWith("Mods.V2.ItemTooltip.Voraria.Charms.RegenFromAbsorption.Long",
						new
						{
							BaseRegen = HealthRegenerationPerPlayerSizeDigested,
							MaxRegen = (HealthRegenerationPerPlayerSizeDigested * player.AsPred().PreyAbsorptionRate).CastToDecimalPlaces(2),
							CurrentRegen = (regenEffectiveness > 0.0 ? HealthRegenerationPerPlayerSizeDigested * player.AsPred().PreyAbsorptionRate * regenEffectiveness : 0.0).CastToDecimalPlaces(2)
						})
				);
			}
		}
	}
}

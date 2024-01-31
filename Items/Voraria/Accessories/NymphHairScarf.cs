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
using V2.PlayerHandling;

namespace V2.Items.Voraria.Accessories
{
	public class NymphHairScarf : ModItem
	{
		public static int AcidStrengthBonus => 12;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Accessories.NymphScarf");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Accessories.NymphScarf.Short");
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(
				gold: 5
			);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicTooltip(
				"Voraria.Charms.BetterDigestion",
				new
				{
					AcidStrengthBonus
				}
			);
		}
	}
}

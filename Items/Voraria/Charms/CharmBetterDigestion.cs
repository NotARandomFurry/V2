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

namespace V2.Items.Voraria.Charms
{
	public class CharmBetterDigestion : ModItem
	{
		public static int AcidStrengthBonus => 12;
		public override void SetDefaults()
		{
			Item.SetNameOverride(Language.GetTextValue("Mods.V2.ItemName.Voraria.Charms.BetterDigestion"));

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
			player.AsPred().ACI.Extra += 12;
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

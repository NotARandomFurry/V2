using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.Items.Vanilla.Accessories.Informational
{
	public class TallyCounterVoreDisplay : InfoDisplay
	{
		public override string Texture => "V2/Items/Vanilla/Accessories/Informational/TallyCounter_VoreInfoDisplay_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.TallyCounterVore");

		public override bool Active() => Main.player[Main.myPlayer].accJarOfSouls;
 
		public override string DisplayValue(ref Color displayColor)
		{
			Player player = Main.player[Main.myPlayer];
			if (player.AsPred().lastEntitySwallowed is null || player.AsPred().lastEntitySwallowedMod is null)
			{
				displayColor = InactiveInfoTextColor;
				return Language.GetTextValue("Mods.V2.InfoDisplayText.TallyCounterVore.InvalidMeal");
			}

			if (!player.AsPred().mealCount.ContainsKey(player.AsPred().lastEntitySwallowedMod + ": " + player.AsPred().lastEntitySwallowed))
				return player.AsPred().lastEntitySwallowed + ": 0";

			return player.AsPred().lastEntitySwallowed + ": " + player.AsPred().mealCount[player.AsPred().lastEntitySwallowedMod + ": " + player.AsPred().lastEntitySwallowed];
		}
	}
}

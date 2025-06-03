using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Vanilla.Accessories.Informational
{
	public class WeightDisplayer1SizeDisplay : InfoDisplay
	{
		public override string Texture => "V2/Items/Vanilla/Accessories/Informational/TallyCounter_VoreInfoDisplay_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Size");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Size = PreyData.GetPreySize(player);
			return "Current Size: " + Size.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer2SaturationDisplay : InfoDisplay
	{
		public override string Texture => "V2/Items/Vanilla/Accessories/Informational/TallyCounter_VoreInfoDisplay_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Saturation");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Amount = player.AsPred().ActuallyReasonableAmountOfFood;
			return "Saturation: " + Amount.CastToDecimalPlaces(3).ToString();
		}
	}
}

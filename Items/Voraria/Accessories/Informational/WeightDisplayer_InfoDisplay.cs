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

namespace V2.Items.Voraria.Accessories.Informational
{
	public class WeightDisplayer_Weight_Unknown : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_UnknownWeight_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Weight");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && !Main.player[Main.myPlayer].AsV2Player().HasTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Size = PlayerGaining.GetPlayerWeight(player, true, false);
			return "Current Weight: " + Size.CastToDecimalPlaces(3).ToString();
		}
	}

	public class WeightDisplayer_Weight_Baelz : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_BaelzWeight_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Weight");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().BaeTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Size = PlayerGaining.GetPlayerWeight(player, true, false);
			return "Current Weight: " + Size.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer_Weight_Kronii : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_KroniiWeight_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Weight");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().KroniiTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Size = PlayerGaining.GetPlayerWeight(player, true, false);
			return "Current Weight: " + Size.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer_Weight_Ollie : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_OllieWeight_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Weight");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().OllieTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Size = PlayerGaining.GetPlayerWeight(player, true, false);
			return "Current Weight: " + Size.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer_Weight_Sora : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_SoraWeight_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Weight");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().SoraTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Size = PlayerGaining.GetPlayerWeight(player, true, false);
			return "Current Weight: " + Size.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer_Weight_Mint : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_MintWeight_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Weight");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().MintTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Size = PlayerGaining.GetPlayerWeight(player, true, false);
			return "Current Weight: " + Size.CastToDecimalPlaces(3).ToString();
		}
	}
	//---
	public class WeightDisplayer_Saturation_Unknown : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_UnknownSatu_Icon";
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Saturation");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && !Main.player[Main.myPlayer].AsV2Player().HasTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Amount = player.AsPred().ActuallyReasonableAmountOfFood;
			return "Saturation: " + Amount.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer_Saturation_Baelz : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_BaelzSatu_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Saturation");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().BaeTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Amount = player.AsPred().ActuallyReasonableAmountOfFood;
			return "Saturation: " + Amount.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer_Saturation_Kronii : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_KroniiSatu_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Saturation");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().KroniiTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Amount = player.AsPred().ActuallyReasonableAmountOfFood;
			return "Saturation: " + Amount.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer_Saturation_Ollie : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_OllieSatu_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Saturation");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().OllieTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Amount = player.AsPred().ActuallyReasonableAmountOfFood;
			return "Saturation: " + Amount.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer_Saturation_Sora : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_SoraSatu_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Saturation");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().SoraTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Amount = player.AsPred().ActuallyReasonableAmountOfFood;
			return "Saturation: " + Amount.CastToDecimalPlaces(3).ToString();
		}
	}
	public class WeightDisplayer_Saturation_Mint : InfoDisplay
	{
		public override string Texture => "V2/Items/Voraria/Accessories/Informational/WeightDisplayerIcons/WeightDisplayer_MintSatu_Icon";

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.InfoDisplayName.WeightDisplayer.Saturation");

		public override bool Active() => Main.player[Main.myPlayer].AsPred().WeightDisplay && Main.player[Main.myPlayer].AsV2Player().MintTransformation;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			double Amount = player.AsPred().ActuallyReasonableAmountOfFood;
			return "Saturation: " + Amount.CastToDecimalPlaces(3).ToString();
		}
	}
}

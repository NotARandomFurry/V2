using System.ComponentModel;
using Terraria.ModLoader.Config;
using V2.UI.StruggleSystem;

namespace V2
{
	public class V2ClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Header("$Mods.V2.Configs.Client.Visual.Header")]

		[LabelKey("$Mods.V2.Configs.Client.Visual.SkipPredStatMenuAnims.Label")]
		[TooltipKey("$Mods.V2.Configs.Client.Visual.SkipPredStatMenuAnims.Tooltip")]
		[DefaultValue(false)]
		public bool SkipPredStatMenuAnims { get; set; }

		[LabelKey("$Mods.V2.Configs.Client.Visual.StruggleSystemBackdropType.Label")]
		[TooltipKey("$Mods.V2.Configs.Client.Visual.StruggleSystemBackdropType.Tooltip")]
		[DefaultValue(StruggleSystemUI.StruggleUIOrientation.Horizontal)]
		public StruggleSystemUI.StruggleUIOrientation StruggleSystemBackdropOrientation { get; set; }

		[LabelKey("$Mods.V2.Configs.Client.Visual.ShowChurnDamageNumbers.Label")]
		[TooltipKey("$Mods.V2.Configs.Client.Visual.ShowChurnDamageNumbers.Tooltip")]
		[DefaultValue(true)]
		public bool ShowChurnDamageNumbers { get; set; }

		[LabelKey("$Mods.V2.Configs.Client.Visual.TheGutSlutVisionOMatic.Label")]
		[TooltipKey("$Mods.V2.Configs.Client.Visual.TheGutSlutVisionOMatic.Tooltip")]
		[DefaultValue(false)]
		public bool TheGutSlutVisionOMatic { get; set; }

		[LabelKey("$Mods.V2.Configs.Client.Visual.StreamerMode.Label")]
		[TooltipKey("$Mods.V2.Configs.Client.Visual.StreamerMode.Tooltip")]
		[DefaultValue(true)]
		public bool StreamerMode { get; set; }
	}
}

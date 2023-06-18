using Humanizer;
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

namespace V2.StatusEffects.Debuffs
{
	public class Softened : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Debuffs.Softened.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Debuffs.Softened.Description");

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			buffName += " " + Main.LocalPlayer.AsFood().softenedStacks.ToRoman();
			rare = ItemRarityID.Lime;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.AsFood().softenedStacks = (int)Math.Ceiling((double)player.buffTime[buffIndex] / 60.0);
		}
	}
}

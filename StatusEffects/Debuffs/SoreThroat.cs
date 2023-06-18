using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.StatusEffects.Debuffs
{
	public class SoreThroat : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Debuffs.SoreThroat.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Debuffs.SoreThroat.Description");

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			rare = ItemRarityID.Quest;
		}
	}
}

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
	public class MoltenStomach : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Debuffs.MoltenStomach.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Debuffs.MoltenStomach.Description");

		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			rare = ItemRarityID.Red;
		}
	}
}

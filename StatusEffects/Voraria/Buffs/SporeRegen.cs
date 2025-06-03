using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Items.Vanilla.Accessories;
using V2.Items.Voraria.Accessories;
using V2.Items.Voraria.Consumables.Potions;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Buffs
{
	public class SporeRegen : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.SporeRegen.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.SporeRegen.Description");
		public override bool RightClick(int buffIndex) => false;

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			rare = ItemRarityID.Green;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Voraria.Buffs.SporeRegen.Description",
				new
				{

				}
			);
		}

		public override bool ReApply(Player player, int time, int buffIndex)
		{
			player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex] + time, 3600);
			return true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.AddHealthRegenEffect(
				healthPerSecond: Math.Min((int)Math.Ceiling(player.buffTime[buffIndex] / 180f), 10)
			);
		}
	}
}

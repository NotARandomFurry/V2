using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
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

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			Player player = Main.LocalPlayer;
			int Power = player.buffTime[player.FindBuffIndex(ModContent.BuffType<SporeRegen>())];

			rare = ItemRarityID.Green;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Voraria.Buffs.SporeRegen.Description",
				new
				{
					Regen = (0.5 + Power / 1800f).CastToDecimalPlaces(1).ToString(),
				}
			);
		}
		public override bool ReApply(Player player, int time, int buffIndex)
		{
			player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex] + time, 10800);
			return true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.AddHealthRegenEffect(
				healthPerSecond: 0.5 + (double)(player.buffTime[buffIndex] / 1800f)
			);
		}
	}
}

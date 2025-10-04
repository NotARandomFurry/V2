using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Buffs
{
	public class OllieShootSpeed : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.OllieShootSpeed.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.OllieShootSpeed.Description");
        public override string Texture => "V2/StatusEffects/Voraria/Buffs/BuffPlaceholder";

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.LocalPlayer;
            int Power = Math.Min(player.buffTime[player.FindBuffIndex(ModContent.BuffType<OllieShootSpeed>())], 300);
            float AtkSpd = Power / 300f;

            rare = ItemRarityID.Blue;
            string statTip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Buffs.OllieShootSpeed.Description.StatChanges",
                new
                {
                    FiringSpeed = V2Utils.GetStatChangeString((int)(AtkSpd * 100)),

                }
            );
            tip = statTip;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
		{
			player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex] + time, 300);
			return true;
		}

		public override void Update(Player player, ref int buffIndex)
        {
			int Power = Math.Min(player.buffTime[buffIndex], 1800);
            player.GetAttackSpeed(DamageClass.Ranged) += Power / 300f;
        }
	}
}

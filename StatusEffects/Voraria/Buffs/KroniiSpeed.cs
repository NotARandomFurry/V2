using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Buffs
{
	public class KroniiSpeed : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.KroniiSpeed.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.KroniiSpeed.Description");
		public override string Texture => "V2/StatusEffects/Voraria/Buffs/BuffPlaceholder";

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			Player player = Main.LocalPlayer;
			int Power = Math.Min(player.buffTime[player.FindBuffIndex(ModContent.BuffType<KroniiSpeed>())], 1800);
			float AtkSpd = Power / 2400f;
			float Digest = Power / 3600f;
			float Absorb = Power / 1800f;

			rare = ItemRarityID.Blue;
			string statTip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Voraria.Buffs.KroniiSpeed.Description.StatChanges",
				new
				{
					AttackSpeed = V2Utils.GetStatChangeString((int)(AtkSpd * 100)),
					Digestion = V2Utils.GetStatChangeString((int)(Digest * 100), IsVoreStat: true),
					Absorption = V2Utils.GetStatChangeString((int)(Absorb * 100), IsVoreStat: true)

				}
			);
			tip = statTip;
		}

		public override bool ReApply(Player player, int time, int buffIndex)
		{
			player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex] + time, 1850);
			return true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			int Power = Math.Min(player.buffTime[buffIndex], 1800);
			player.GetAttackSpeed(DamageClass.Melee) += Power / 2400f;
			player.AsPred().DigestionTickDamageModifier += Power / 3600f;
			player.AsPred().DigestionTickRateModifier += Power / 3600f;
			player.AsPred().PreyAbsorptionRateModifier += Power / 1800f;
		}
	}
}

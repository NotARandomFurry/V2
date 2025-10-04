using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.TransformationItems.Baelz;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Buffs
{
	public class BaelzTransformation : ModBuff
	{
<<<<<<< Updated upstream
        public float SpeedBoost = 0.25f;
        public float BaseDmg = -0.4f;
        public float DmgBoost = 0.05f;
        public float BaseSpd = 0.15f;
        public float SpdBoost = -0.03f;
        public int BaseCrit = 10;
        public int CritBoost = 1;
        public int BaseDef = -10;
        public int DefBoost = 1;
=======
		public float SpeedBoost = 0.3f;
		public float BaseDmg = -0.35f;
		public float BaseSpd = 0.15f;
		public int BaseCrit = 10;
		public float CritBoost = 1.25f;
		public float BaseEndu = -0.15f;
>>>>>>> Stashed changes

        public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.BaelzTransformation.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.BaelzTransformation.Description");
        public override bool RightClick(int buffIndex) => false;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
            int weightMul = GetWeightMul(Main.LocalPlayer);
            float dmg = BaseDmg + (DmgBoost * weightMul);
            float spd = BaseSpd + (SpdBoost * weightMul);
            int crit = BaseCrit + (CritBoost * weightMul);
            int def = BaseDef + (DefBoost * weightMul);

            int chosenName = BaeTransformationItem.GetVisualWeightStage(Main.LocalPlayer);

            rare = ItemRarityID.Red;

            buffName = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Buffs.BaelzTransformation.Name." + chosenName.ToString(),
                new
                {

                }
            );

            string baseTooltip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Buffs.BaelzTransformation.Description.Base." + chosenName.ToString(),
                new
                {

                }
            );
            string statTip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Buffs.BaelzTransformation.Description.StatChanges",
                new
                {
                    Damage = WellFed.DecideIfPositive((int)(dmg * 100)),
                    Defense = WellFed.DecideIfPositive(def, true),
                    Critical = WellFed.DecideIfPositive(crit),
                    AttackSpeed = WellFed.DecideIfPositive((int)(spd * 100)),
                    RunSpeed = WellFed.DecideIfPositive((int)(SpeedBoost * 100))

                }
            );
            tip = baseTooltip + "\n" + statTip;
		}

<<<<<<< Updated upstream
        public int GetWeightMul(Player player)
        {
            return (int)Math.Floor(player.AsPred().BaeTransformation_ExtraWeight * 4);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.AsPred().BaseWeightGainRatio = BaeTransformationItem.WeightGainRatio;
            player.moveSpeed += SpeedBoost;
            int weightMul = GetWeightMul(player);
            float dmg = BaseDmg + (DmgBoost * weightMul);
            float spd = BaseSpd + (SpdBoost * weightMul);
            int crit = BaseCrit + (CritBoost * weightMul);
            int def = BaseDef + (DefBoost * weightMul);

            player.GetAttackSpeed(DamageClass.Generic) += spd;
            player.GetDamage(DamageClass.Generic) += dmg;
            player.GetCritChance(DamageClass.Generic) += crit;
            player.statDefense += def;
        }
=======
		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            float weightMul = GetWeightMul(Main.LocalPlayer);
            int crit = (int)Math.Round(BaseCrit + (CritBoost * weightMul));

			PlayerGaining.GetPlayerWeightGainStats(Main.LocalPlayer, out float dmgMult, out float atkspdMult, out int maxlife);

            int chosenName = BaelzInfo.GetVisualWeightStage(Main.LocalPlayer);

			rare = ItemRarityID.Red;

			buffName = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Voraria.Buffs.BaelzTransformation.Name." + chosenName.ToString(),
				new
				{

				}
			);

			string baseTooltip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Voraria.Buffs.BaelzTransformation.Description.Base." + chosenName.ToString(),
				new
				{

				}
			);
			string statTip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Voraria.Buffs.BaelzTransformation.Description.StatChanges",
				new
                {
                    Damage = V2Utils.GetStatChangeString((int)(BaseDmg * 100)),
                    DamageReduction = V2Utils.GetStatChangeString((int)(BaseEndu * 100)),
                    Critical = V2Utils.GetStatChangeString(crit),
                    AttackSpeed = V2Utils.GetStatChangeString((int)(BaseSpd * 100)),
                    RunSpeed = V2Utils.GetStatChangeString((int)(SpeedBoost * 100))

                }
			);
			string weightTip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Buffs.GeneralWeightGainStatChanges",
                new
                {
                    Damage = V2Utils.GetStatChangeString(dmgMult.CastToDecimalPlaces(2), IsMultiplier: true),
                    AttackSpeed = V2Utils.GetStatChangeString(atkspdMult.CastToDecimalPlaces(2), IsMultiplier: true),
                    MaxHealth = V2Utils.GetStatChangeString(maxlife, true),
                }
            );
            tip = baseTooltip + "\n" + statTip;
			if (weightMul > 0.1)
				tip += "\n" + weightTip;
        }

		public float GetWeightMul(Player player)
		{
			return (float)player.AsPred().BaeTransformation_ExtraWeight * 4;
		}

		public static int GetCritChanceForDigestionTicks(Player player)
		{
            return 10 + (int)Math.Round(player.AsPred().BaeTransformation_ExtraWeight * 4);
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.AsPred().BurpPitchOffset += 0.1f;

			player.AsPred().BaseWeightGainRatio = BaelzInfo.WeightGainRatio;
			player.moveSpeed += SpeedBoost;
			player.endurance += BaseEndu;
            player.GetAttackSpeed(DamageClass.Generic) += BaseSpd;
            player.GetDamage(DamageClass.Generic) += BaseDmg;


            float weightMul = GetWeightMul(player);

			int crit = (int)Math.Round(BaseCrit + (CritBoost * weightMul));

			player.GetCritChance(DamageClass.Generic) += crit;
		}
>>>>>>> Stashed changes
	}
}

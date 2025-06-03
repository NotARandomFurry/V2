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
using V2.Items.Voraria.Accessories.Transformations;
using V2.Items.Voraria.Accessories.Transformations.Baelz;
using V2.Items.Voraria.Consumables.Potions;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Buffs
{
	public class BaelzTransformation : ModBuff
	{
		public float SpeedBoost = 0.25f;
		public float BaseDmg = -0.4f;
		public float DmgBoost = 0.05f;
		public float BaseSpd = 0.15f;
		public float SpdBoost = -0.03f;
		public int BaseCrit = 10;
		public int CritBoost = 1;
		public int BaseDef = -10;
		public int DefBoost = 1;

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
	}
}

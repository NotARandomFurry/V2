using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class TipsyDebuff : GlobalBuff
	{
		public static float MeleeDamageBoost => 0.10f;
		public static float MeleeCritChanceBoost => 0.10f;
		public static float MeleeAttackSpeedBoost => 0.10f;
		public static int DefensePenalty => 5;
		public static int FishPowerBoost => 5;
		public static float CollectiveCapacityBoost => 0.10f;
		public static float CollectiveChurnRatePenalty => 0.10f;
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Tipsy, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Tipsy)
				return;

			player.tipsy = true;
			player.GetDamage(DamageClass.Melee) += MeleeDamageBoost;
			player.GetCritChance(DamageClass.Melee) += MeleeCritChanceBoost;
			player.GetAttackSpeed(DamageClass.Melee) += MeleeAttackSpeedBoost;
			player.statDefense -= DefensePenalty;
			player.fishingSkill += FishPowerBoost;
			player.AsPred().SwallowCapacityModifier += CollectiveCapacityBoost;
			player.AsPred().StomachCapacityModifier += CollectiveCapacityBoost;
			player.AsPred().DigestionTickRateModifier *= 1f - CollectiveChurnRatePenalty;
			player.AsPred().PreyAbsorptionRateModifier *= 1f - CollectiveChurnRatePenalty;
		}

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.Tipsy)
				return;

			rare = ItemRarityID.LightRed;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Vanilla.Debuffs.Tipsy.Description",
				new
				{
					TipsyMeleeDamageBonus = MeleeDamageBoost.ToPercentage(2),
					TipsyMeleeCritChanceBonus = MeleeCritChanceBoost.ToPercentage(2),
					TipsyMeleeAttackSpeedBonus = MeleeAttackSpeedBoost.ToPercentage(2),
					TipsyDefenseCut = DefensePenalty,
					TipsyFishPowerBonus = FishPowerBoost,
					TipsyCapacityBonus = CollectiveCapacityBoost.ToPercentage(2),
					TipsyChurnRateCut = CollectiveChurnRatePenalty.ToPercentage(2),
				}
			);
		}
	}
}
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Buffs
{
	public class RapidHealingBuff : GlobalBuff
	{
		public static double HealthRegenFlat => 2.5;
		public static double NaturalRegenTimeBonus => 0.15;
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.RapidHealing, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.RapidHealing)
				return;

			player.AddHealthRegenEffect(
				healthPerSecond: HealthRegenFlat,
				modifyHealthRegenTimeMethod: ModifyHealthRegenTime
			);
		}

		public static void ModifyHealthRegenTime(Player player, ref double healthRegenTime) => healthRegenTime += NaturalRegenTimeBonus;

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.RapidHealing)
				return;

			rare = ItemRarityID.Orange;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Vanilla.Buffs.RapidHealing.Description",
				new
				{
					RapidHealingRegenFlat = HealthRegenFlat.CastToDecimalPlaces(2),
					RapidHealingNaturalRegenBuildupSpeed = NaturalRegenTimeBonus.ToPercentage(2)
				}
			);
		}
	}
}
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Buffs
{
	public class CampfireBuff : GlobalBuff
	{
		public static double HealthRegenerationPerSecond => 2.0;
		public static double NaturalHealthRegenerationBuildSpeedIncrease => 0.20;
		public static float DigestionRateIncrease => 0.15f;
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Campfire, this);
		}

		public override bool RightClick(int type, int buffIndex) => type != BuffID.Campfire;

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Campfire)
				return;

			player.AddHealthRegenEffect(
				healthPerSecond: HealthRegenerationPerSecond,
				natural: true,
				modifyHealthRegenTimeMethod: CampfireModifyHealthRegenTime
			);
			player.AsPred().DigestionTickRateModifier += DigestionRateIncrease;
		}

		public static void CampfireModifyHealthRegenTime(Player player, ref double healthRegenTime)
		{
			healthRegenTime += NaturalHealthRegenerationBuildSpeedIncrease;
		}

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.Campfire)
				return;

			rare = ItemRarityID.Orange;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Vanilla.Buffs.Campfire.Description",
				new
				{
					CampfireRegenFlat = HealthRegenerationPerSecond.CastToDecimalPlaces(2),
					CampfireNaturalRegenBuildupSpeed = NaturalHealthRegenerationBuildSpeedIncrease.ToPercentage(2),
					CampfireDigestionSpeedBonus = DigestionRateIncrease.ToPercentage(2),
				}
			);
		}
	}
}
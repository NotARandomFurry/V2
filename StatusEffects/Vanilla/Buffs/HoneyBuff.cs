using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Buffs
{
	public class HoneyBuff : GlobalBuff
	{
		public static double HealthRegenFlat => 2.0;
		public static double NaturalRegenTimeBonus => 0.2;
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Honey, this);
		}

		public override bool RightClick(int type, int buffIndex) => type != BuffID.Honey;

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Honey)
				return;

			player.AddHealthRegenEffect(
				healthPerSecond: HealthRegenFlat,
				natural: true,
				modifyHealthRegenTimeMethod: ModifyHealthRegenTime
			);
		}

		public static void ModifyHealthRegenTime(Player player, ref double healthRegenTime)
		{
			healthRegenTime += NaturalRegenTimeBonus;
		}

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.Honey)
				return;

			rare = ItemRarityID.Quest;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Vanilla.Buffs.Honey.Description",
				new
				{
					HoneyRegenFlat = HealthRegenFlat.CastToDecimalPlaces(2),
					HoneyNaturalRegenBuildupSpeed = NaturalRegenTimeBonus.ToPercentage(2),
				}
			);
		}
	}
}
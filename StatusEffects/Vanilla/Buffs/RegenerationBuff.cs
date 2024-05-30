using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Buffs
{
	public class RegenerationBuff : GlobalBuff
	{
		public static double HealthRegenFlat => 2.0;
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Regeneration, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Regeneration)
				return;

			player.AddHealthRegenEffect(
				healthPerSecond: HealthRegenFlat
			);
		}

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.Regeneration)
				return;

			rare = ItemRarityID.LightRed;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Vanilla.Buffs.RapidHealing.Description",
				new
				{
					RegenPotionRegenFlat = HealthRegenFlat.CastToDecimalPlaces(2),
				}
			);
		}
	}
}
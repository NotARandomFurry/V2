using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Buffs
{
	public class HeartLanternBuff : GlobalBuff
	{
		public static double HealthRegenerationBoost => 2.0;
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.HeartLamp, this);
		}

		public override bool RightClick(int type, int buffIndex) => type != BuffID.HeartLamp;

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.HeartLamp)
				return;

			player.AddHealthRegenEffect(
				healthPerSecond: HealthRegenerationBoost
			);
		}

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.HeartLamp)
				return;

			rare = ItemRarityID.LightRed;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Vanilla.Buffs.HeartLantern.Description",
				new
				{
					HeartLanternRegenFlat = HealthRegenerationBoost.CastToDecimalPlaces(2),
				}
			);
		}
	}
}
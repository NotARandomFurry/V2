using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class StarvingDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Starving, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Starving)
				return;

			player.starving = true;
			player.AddHealthRegenEffect(
				healthPerSecond: StarvingHealthPerSecond,
				natural: true
			);
		}

		public static double StarvingHealthPerSecond(Player player)
		{
			double num2 = 60.0 * (double)player.statLifeMax2 / 3000.0;
			if (num2 < 2.0)
				num2 = 2.0;

			return -num2;
		}
	}
}
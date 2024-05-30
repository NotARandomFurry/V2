using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class ElectrifiedDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Electrified, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Electrified)
				return;

			player.electrified = true;
			player.AddHealthRegenEffect(
				healthPerSecond: (player) =>
				{
					double baseRate = 4.0;
					if (player.wet)
						baseRate *= 10.0;

					if (player.velocity.Length() >= 1.0f)
						baseRate *= 4.0;

					return -4.0;
				}
			);
		}
	}
}
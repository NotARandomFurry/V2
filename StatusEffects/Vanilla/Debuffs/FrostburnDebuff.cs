using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class FrostburnDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Frostburn, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Frostburn)
				return;

			player.onFrostBurn = true;
			player.AddHealthRegenEffect(
				healthPerSecond: -6.0
			);
		}
	}
}
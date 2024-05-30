using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class FrostbiteDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Frostburn2, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Frostburn2)
				return;

			player.onFrostBurn2 = true;
			player.AddHealthRegenEffect(
				healthPerSecond: -25.0
			);
		}
	}
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class CursedInfernoDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.CursedInferno, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.CursedInferno)
				return;

			player.onFire2 = true;
			player.AddHealthRegenEffect(
				healthPerSecond: -6.0
			);
		}
	}
}
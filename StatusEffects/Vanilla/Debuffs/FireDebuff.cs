using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class FireDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.OnFire, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.OnFire)
				return;

			player.onFire = true;
			player.AddHealthRegenEffect(
				healthPerSecond: -4.0
			);
		}
	}
}
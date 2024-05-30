using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class StrongFireDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.OnFire3, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.OnFire3)
				return;

			player.onFire3 = true;
			player.AddHealthRegenEffect(
				healthPerSecond: -15.0
			);
		}
	}
}
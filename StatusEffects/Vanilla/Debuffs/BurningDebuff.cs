using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class BurningDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Burning, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Burning)
				return;

			player.burned = true;
			player.moveSpeed *= 0.50f;
			player.AddHealthRegenEffect(
				healthPerSecond: -30.0
			);
		}
	}
}
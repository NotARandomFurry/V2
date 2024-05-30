using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class PoisonDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Poisoned, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Poisoned)
				return;

			player.poisoned = true;
			player.AddHealthRegenEffect(
				healthPerSecond: -2.0,
				natural: true
			);
		}
	}
}
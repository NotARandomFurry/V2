using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Debuffs
{
	public class VenomDebuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Venom, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Venom)
				return;

			player.venom = true;
			player.AddHealthRegenEffect(
				healthPerSecond: -15.0,
				natural: true
			);
		}
	}
}
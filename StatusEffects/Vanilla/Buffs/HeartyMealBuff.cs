using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Buffs
{
	public class HeartyMealBuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.HeartyMeal, this);
		}

		public override bool RightClick(int type, int buffIndex) => type != BuffID.HeartyMeal;

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.HeartyMeal)
				return;

			player.AddHealthRegenEffect(
				healthPerSecond: 3.0
			);
		}
	}
}
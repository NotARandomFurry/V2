using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Buffs
{
	public class DryadsWardBuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.DryadsWard, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.DryadsWard)
				return;

			player.dryadWard = true;
			player.AddHealthRegenEffect(
				healthPerSecond: DryadBlessingHealthPerSecond,
				natural: true
			);
			player.statDefense += DryadBlessingDefenseBoost(player);
			player.thorns += DryadBlessingDamageReflectionBoost(player);
		}

		public static double DryadBlessingHealthPerSecond(Entity blessedEntity)
		{
			double healthRegenPerSecond = 4.0;
			if (NPC.combatBookWasUsed)
				healthRegenPerSecond += 1.0;
			return healthRegenPerSecond;
		}

		public static int DryadBlessingDefenseBoost(Entity blessedEntity)
		{
			int defense = 6;
			if (NPC.combatBookWasUsed)
				defense += 6;
			return defense;
		}

		public static float DryadBlessingDamageReflectionBoost(Entity blessedEntity)
		{
			float thorns = 0.8f;
			if (NPC.combatBookWasUsed)
				thorns += 0.7f;
			return thorns;
		}
	}
}

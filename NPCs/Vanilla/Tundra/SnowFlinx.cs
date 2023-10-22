using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Vanilla.Tundra
{
	public static class SnowFlinxStuff
	{
		public static SnowFlinx AsSnowFlinx(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out SnowFlinx snowFlinx))
				throw new Exception("this instance of a Snow Flinx, supposedly, doesn't exist");

			return snowFlinx;
		}
	}

	public class SnowFlinx : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.SnowFlinx;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().Size = 0.72;

			npc.AsFood().OnKilledByDigestion += PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += OnKilledByDigestion_GrantSnowFlinxGoal;
		}

		public static void OnKilledByDigestion_GrantSnowFlinxGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatSnowFlinx>().TrySetCompletion(predPlayer);
		}
	}
}

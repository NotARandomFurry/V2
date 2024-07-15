using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Vanilla.Graveyard
{
    public static class GhostStuff
	{
		public static Ghost AsGhost(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Ghost Ghost))
				throw new Exception("this instance of a Ghost, supposedly, doesn't exist");

			return Ghost;
		}
	}

	public class Ghost : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Ghost;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 0.45;

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantGhostGoal;
		}

		public static void OnKilledByDigestion_GrantGhostGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<EatGhost>().TrySetCompletion(predPlayer);
			}
		}
	}
}

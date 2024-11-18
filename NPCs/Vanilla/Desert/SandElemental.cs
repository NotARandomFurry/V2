using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;

namespace V2.NPCs.Vanilla.Desert
{
    public static class SandElementalStuff
	{
		public static SandElemental AsSandElemental(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out SandElemental sandElemental))
				throw new Exception("this instance of a SandElemental, supposedly, doesn't exist");

			return sandElemental;
		}
	}

	public class SandElemental : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.SandElemental;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 3.72;

			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantSandElementalGoal;
		}

		public static void OnKilledByDigestion_GrantSandElementalGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatSandElemental>().TrySetCompletion(predPlayer);
		}
	}
}

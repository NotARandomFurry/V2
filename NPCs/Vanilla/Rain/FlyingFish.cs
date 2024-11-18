using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;

namespace V2.NPCs.Vanilla.Rain
{
    public static class FlyingFishStuff
	{
		public static FlyingFish AsFlyingFish(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out FlyingFish flyingFish))
				throw new Exception("this instance of a Flying Fish, supposedly, doesn't exist");

			return flyingFish;
		}
	}

	public class FlyingFish : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.FlyingFish;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 0.6;

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantFlyingFishGoal;
		}

		public static void OnKilledByDigestion_GrantFlyingFishGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatFlyingFish>().TrySetCompletion(predPlayer);
		}
	}
}

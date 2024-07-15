using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Vanilla.Forest
{
	public static class AngryDandelionStuff
	{
		public static AngryDandelion AsAngryDandelion(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out AngryDandelion angryDandelion))
				throw new Exception("this instance of an Angry Dandelion, supposedly, doesn't exist");

			return angryDandelion;
		}
	}

	public class AngryDandelion : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Dandelion;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 0.475;

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantAngryDandelionGoal;
		}

		public static void OnKilledByDigestion_GrantAngryDandelionGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<EatAngyFlower>().TrySetCompletion(predPlayer);
			}
		}
	}
}

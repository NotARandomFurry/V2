using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;

namespace V2.NPCs.Vanilla.Tundra
{
	public static class IceGolemStuff
	{
		public static IceGolem AsIceGolem(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out IceGolem IceGolem))
				throw new Exception("this instance of a Snow Flinx, supposedly, doesn't exist");

			return IceGolem;
		}
	}

	public class IceGolem : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.IceGolem;

		public override void SetDefaults(NPC NPC)
		{
			NPC.AsV2NPC().Gender = EntityGender.Other;

			NPC.AsFood().DefinedBaseSize = 4.18;

			NPC.AsFood().OnDigestedBy += OnKilledByDigestion_GrantIceGolemGoal;
		}

		public static void OnKilledByDigestion_GrantIceGolemGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatIceGolem>().TrySetCompletion(predPlayer);
		}
	}
}

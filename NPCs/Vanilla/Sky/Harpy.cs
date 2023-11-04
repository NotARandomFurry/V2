using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Vanilla.Sky
{
	public static class HarpyStuff
	{
		public static Harpy AsHarpy(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Harpy harpy))
				throw new Exception("this instance of a Harpy, supposedly, doesn't exist");

			return harpy;
		}
	}

	public class Harpy : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Harpy;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsFood().Size = 1.45;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += OnKilledByDigestion_GrantHarpyGoal;
		}

		public static void OnKilledByDigestion_GrantHarpyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatHarpy>().TrySetCompletion(predPlayer);
		}
	}
}

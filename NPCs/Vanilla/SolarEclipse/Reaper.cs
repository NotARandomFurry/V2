using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Skilled;

namespace V2.NPCs.Vanilla.SolarEclipse
{
	public static class ReaperStuff
	{
		public static Reaper AsReaper(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Reaper Reaper))
				throw new Exception("this instance of a Reaper, supposedly, doesn't exist");

			return Reaper;
		}
	}

	public class Reaper : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Reaper;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 0.72;

			npc.AsFood().IsAGhostlySnackForACertainMaid = true;

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantReaperGoal;
		}

		public static void OnKilledByDigestion_GrantReaperGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatReaper>().TrySetCompletion(predPlayer);
		}
	}
}

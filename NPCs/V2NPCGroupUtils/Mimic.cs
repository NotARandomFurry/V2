using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria;
using V2.NPCs.Vanilla.Cavern;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;
using V2.Sounds.Vore;

namespace V2.NPCs.V2NPCGroupUtils
{
	public static class MimicStuff
	{
		public static Mimic AsMimic(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Mimic tastySparklySnack))
				throw new Exception("this instance of a gem critter, supposedly, doesn't exist");

			return tastySparklySnack;
		}
	}

	public class Mimic : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => V2Utils.NPCIDSets.Mimics.Contains(entity.type);

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().OnDigestedBy += OnDigestedBy_GrantMimicGoal;
		}

		public static void OnDigestedBy_GrantMimicGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<EatMimic>().TrySetCompletion(predPlayer);
			}
		}
	}
}

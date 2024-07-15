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

namespace V2.NPCs.NPCGroupUtils
{
	public static class MiniFairyStuff
	{
		public static MiniFairy AsMiniFairy(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out MiniFairy tastySparklySnack))
				throw new Exception("this instance of a gem critter, supposedly, doesn't exist");

			return tastySparklySnack;
		}
	}

	public class MiniFairy : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => V2Utils.NPCIDSets.MiniFairies.Contains(entity.type);

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = Main.rand.NextBool() ? EntityGender.Male : EntityGender.Female;

			npc.AsFood().OnDigestedBy += OnDigestedBy_GrantMiniFairyGoal;
		}

		public static void OnDigestedBy_GrantMiniFairyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				bool canCatchFairy = npc.ai[2] <= 1f;
				if (canCatchFairy)
					ModContent.GetInstance<EatHelpfulFairy>().TrySetCompletion(predPlayer);
			}
		}
	}
}

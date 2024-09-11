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

namespace V2.NPCs.GroupDefinitions
{
	public static class ButterflyGroupStuff
	{
		public static AnyButterfly AsAButterfly(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out AnyButterfly tastySparklySnack))
				throw new Exception("this instance of a gem critter, supposedly, doesn't exist");

			return tastySparklySnack;
		}
	}

	public class AnyButterfly : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => V2Utils.NPCIDSets.Butterflies.Contains(entity.type);

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 0.055;

			npc.AsFood().OnSwallowedBy += OnSwallowedBy_GrantButterflyGroupMultiPreyGoal;
		}

		public static void OnSwallowedBy_GrantButterflyGroupMultiPreyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				List<int> possibleButterflies = V2Utils.NPCIDSets.Butterflies;
				int butterfliesInTummy = 0;
				foreach (PreyData prey in predPlayer.AsPred().StomachTracker.Prey)
				{
					if (prey.Type != PreyType.NPC)
						continue;

					int preyNPCID = prey.ExactType;
					if (possibleButterflies.Contains(preyNPCID))
						butterfliesInTummy++;
				}
				if (butterfliesInTummy >= 3)
					ModContent.GetInstance<StomachButterflies>().TrySetCompletion(predPlayer);
			}
		}
	}
}

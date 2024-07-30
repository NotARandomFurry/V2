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
	public static class GemCritterStuff
	{
		public static GemCritter AsGemCritter(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out GemCritter tastySparklySnack))
				throw new Exception("this instance of a gem critter, supposedly, doesn't exist");

			return tastySparklySnack;
		}
	}

	public class GemCritter : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => V2Utils.NPCIDSets.GemCritters.Contains(entity.type);

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 0.42;

			npc.AsFood().OnSwallowedBy += OnSwallowedBy_GrantGemCritterMultiPreyGoal;
		}

		public static void OnSwallowedBy_GrantGemCritterMultiPreyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				List<int> distinctGemCritters = V2Utils.NPCIDSets.GemCritters;
				int distinctGemCrittersInTummy = 0;
				foreach (PreyData prey in predPlayer.AsPred().StomachTracker.Prey)
				{
					if (prey.Type != PreyType.NPC)
						continue;

					int preyNPCID = prey.ExactType;
					if (distinctGemCritters.Contains(preyNPCID))
					{
						distinctGemCrittersInTummy++;
						distinctGemCritters.Remove(preyNPCID);
					}
				}
				if (distinctGemCrittersInTummy >= 7)
					ModContent.GetInstance<HoardGemCritters>().TrySetCompletion(predPlayer);
			}
		}
	}
}

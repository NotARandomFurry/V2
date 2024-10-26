using System;
using Terraria;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Sets
{
	public static class AnyGoldCritterStuff
	{
		public static AnyGoldCritter AsAGoldCritter(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out AnyGoldCritter tastySparklySnack))
				throw new Exception("this instance of a gold critter, supposedly, doesn't exist");

			return tastySparklySnack;
		}
	}

	public class AnyGoldCritter : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => V2Utils.NPCIDSets.GoldCritters.Contains(entity.type);

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().OnDigestedBy += OnDigestedBy_GrantGoldCritterGoal;
		}

		public static void OnDigestedBy_GrantGoldCritterGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<EatGoldCritter>().TrySetCompletion(predPlayer);
			}
		}
	}
}

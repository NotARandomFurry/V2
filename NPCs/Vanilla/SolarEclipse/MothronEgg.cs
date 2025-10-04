using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Skilled;

namespace V2.NPCs.Vanilla.SolarEclipse
{
	public static class MothronEggStuff
	{
		public static MothronEgg AsMothronEgg(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out MothronEgg MothronEgg))
				throw new Exception("this instance of a Mothron Egg, supposedly, doesn't exist. hope you didn't want omelettes");

			return MothronEgg;
		}
	}

	public class MothronEgg : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.MothronEgg;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 1.33;
			npc.AsFood().CalorieMultiplier = 2;
			npc.AsFood().WellFedPower = 0.9;

			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantMothronEggGoal;
		}

		public static void OnKilledByDigestion_GrantMothronEggGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatMothronEgg>().TrySetCompletion(predPlayer);
		}
	}
}

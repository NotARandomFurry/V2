using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Amateur;

namespace V2.NPCs.Vanilla.Dungeon
{
	public static class BlazingWheelStuff
	{
		public static BlazingWheel AsBlazingWheel(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out BlazingWheel BlazingWheel))
				throw new Exception("this instance of a Blazing Wheel, supposedly, doesn't exist");

			return BlazingWheel;
		}
	}

	public class BlazingWheel : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.BlazingWheel;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.lifeMax = 475;
			npc.defense = 25;
			npc.AsFood().DefinedBaseSize = 0.6;

			npc.AsFood().OnSwallowDamage = npc.damage;
			npc.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.BlazingWheel";


			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantBlazingWheelGoal;
		}

		public static void OnKilledByDigestion_GrantBlazingWheelGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatBlazingWheel>().TrySetCompletion(predPlayer);
		}
	}
}

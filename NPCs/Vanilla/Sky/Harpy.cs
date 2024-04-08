using System;
using System.Collections.Generic;
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
		public static class ItemTheftRules
		{
			public static ItemTheftRule GiantHarpyFeather => new ItemTheftRule(
				type: (npc, pred) => ItemID.GiantHarpyFeather,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 1.0 / 30.0,
						GameModeID.Expert => 1.0 / 40.0,
						_ => 1.0 / 50.0,
					};
				}
			);
		}
		public static Harpy AsHarpy(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Harpy harpy))
				throw new Exception("this instance of a Harpy, supposedly, doesn't exist");

			return harpy;
		}
	}

	public class Harpy : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Harpy;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsFood().DefinedBaseSize = 1.335;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
			npc.AsFood().OnKilledByDigestion += OnKilledByDigestion_GrantHarpyGoal;
			npc.AsFood().ItemTheftRules = new List<ItemTheftRule>()
			{
				HarpyStuff.ItemTheftRules.GiantHarpyFeather,
			};
		}

		public static void OnKilledByDigestion_GrantHarpyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatHarpy>().TrySetCompletion(predPlayer);
		}
	}
}

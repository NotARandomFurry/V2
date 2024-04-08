using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.NPCGroupUtils;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Vanilla.Forest
{
	public partial class BlueSlime : GlobalNPC
	{
		public static void V2BlueSlimeFirstFrameAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeFirstFrameAI(npc, 18);
		public static bool V2BlueSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 1.0f, 24, 18);

		public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

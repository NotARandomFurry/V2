using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Sets;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Voraria.Hallow
{
	public partial class GlobalGumdropSlime : GlobalNPC
	{
		public static bool V2GumdropSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 1f, 13, 12);
	}
}

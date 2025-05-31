using Terraria;
using Terraria.ModLoader;
using V2.NPCs.Sets;

namespace V2.NPCs.Vanilla.Hallow
{
	public partial class RainbowSlime : GlobalNPC
	{
		public static void V2RainbowSlimeFirstFrameAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeFirstFrameAI(npc, (int)((float)42));
		public static bool V2RainbowSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 1f, (int)((float)60), (int)((float)42));

		// public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

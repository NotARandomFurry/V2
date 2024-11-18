using Terraria;
using Terraria.ModLoader;
using V2.NPCs.Sets;

namespace V2.NPCs.Vanilla.Cavern
{
	public partial class RedSlime : GlobalNPC
	{
		public static void V2RedSlimeFirstFrameAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeFirstFrameAI(npc, (int)((float)18 * 1.025f));
		public static bool V2RedSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 1.025f, (int)((float)24 * 1.025f), (int)((float)18 * 1.025f));

		// public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

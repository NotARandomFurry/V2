using Terraria;
using Terraria.ModLoader;
using V2.NPCs.NPCGroupUtils;

namespace V2.NPCs.Vanilla.Cavern
{
	public partial class BlackSlime : GlobalNPC
	{
		public static void V2BlackSlimeFirstFrameAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeFirstFrameAI(npc, (int)((float)18 * 1.05f));
		public static bool V2BlackSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 1.05f, (int)((float)24 * 1.05f), (int)((float)18 * 1.05f));

		public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

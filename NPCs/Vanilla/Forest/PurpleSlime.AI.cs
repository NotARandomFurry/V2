using Terraria;
using Terraria.ModLoader;
using V2.NPCs.Sets;

namespace V2.NPCs.Vanilla.Forest
{
	public partial class PurpleSlime : GlobalNPC
	{
		public static void V2PurpleSlimeFirstFrameAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeFirstFrameAI(npc, (int)((float)18 * 1.2f));
		public static bool V2PurpleSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 1.2f, (int)((float)24 * 1.2f), (int)((float)18 * 1.2f));

		public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

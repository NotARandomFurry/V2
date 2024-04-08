using Terraria;
using Terraria.ModLoader;
using V2.NPCs.NPCGroupUtils;

namespace V2.NPCs.Vanilla.Forest
{
	public partial class GreenSlime : GlobalNPC
	{
		public static void V2GreenSlimeFirstFrameAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeFirstFrameAI(npc, (int)((float)18 * 0.9f));
		public static bool V2GreenSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 0.9f, (int)((float)24 * 0.9f), (int)((float)18 * 0.9f));

		public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

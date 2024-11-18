using Terraria;
using Terraria.ModLoader;
using V2.NPCs.Sets;

namespace V2.NPCs.Vanilla.Jungle
{
	public partial class JungleSlime : GlobalNPC
	{
		public static void V2JungleSlimeFirstFrameAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeFirstFrameAI(npc, (int)((float)18 * 1.1f));
		public static bool V2JungleSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 1.05f, (int)((float)24 * 1.1f), (int)((float)18 * 1.1f));

		// public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

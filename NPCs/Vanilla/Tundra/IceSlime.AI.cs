using Terraria;
using Terraria.ModLoader;
using V2.NPCs.V2NPCGroupUtils;

namespace V2.NPCs.Vanilla.Tundra
{
	public partial class IceSlime : GlobalNPC
	{
		public static void V2IceSlimeFirstFrameAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeFirstFrameAI(npc, 18);
		public static bool V2IceSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 1f, 24, 18);

		public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

using Terraria;
using Terraria.ModLoader;
using V2.NPCs.V2NPCGroupUtils;

namespace V2.NPCs.Vanilla.Desert
{
	public partial class SandSlime : GlobalNPC
	{
		public static void V2SandSlimeFirstFrameAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeFirstFrameAI(npc, 18);
		public static bool V2SandSlimeAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 1f, 24, 18);

		public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

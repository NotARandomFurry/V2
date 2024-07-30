using Terraria;
using Terraria.ModLoader;
using V2.NPCs.V2NPCGroupUtils;

namespace V2.NPCs.Vanilla.Forest
{
	public partial class Pinky : GlobalNPC
	{
		public static bool V2PinkyAI(NPC npc) => GeneralizedAIOverrides.SimpleSlimeAI(npc, 0.6f, (int)((float)24 * 0.6f), (int)((float)18 * 0.6f));

		public override void PostAI(NPC npc) => npc.DoContactGulpage();
	}
}

using Terraria;
using Terraria.ModLoader;
using V2.NPCs.Sets;

namespace V2.NPCs.Vanilla.Forest
{
	public partial class NormalButterfly : GlobalNPC
	{
		public static bool V2NormalButterflyAI(NPC npc) => GeneralizedAIOverrides.SimpleButterflyAI(npc);
	}
}

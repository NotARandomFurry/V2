using Terraria;
using Terraria.ModLoader;
using V2.NPCs.Sets;

namespace V2.NPCs.Vanilla.Hallow
{
	public partial class PrismaticLacewing : GlobalNPC
	{
		public static bool V2PrismaticLacewingAI(NPC npc) => GeneralizedAIOverrides.SimpleButterflyAI(npc);
	}
}

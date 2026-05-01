using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Vanilla.Jungle;

public class Bee : GlobalNPC
{
	public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Bee;

	public override void SetDefaults(NPC entity)
	{
		
		entity.AsFood().OnDigestedBy += OnDigestedBy;
	}

	private static void OnDigestedBy(NPC npc, Entity pred)
	{

	}
}
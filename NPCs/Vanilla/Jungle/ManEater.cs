using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Vanilla.Jungle;

public class ManEater : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.ManEater;
    }

    public override void SetDefaults(NPC entity)
    {
        entity.AsFood().OnDigestedBy += OnDigestedBy;
    }

    private static void OnDigestedBy(NPC npc, Entity pred)
    {
        if (pred is Player predPlayer)
        {
            ModContent.GetInstance<EatTheManEater>().TrySetCompletion(predPlayer);
        }
    }
}
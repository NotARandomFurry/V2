using Terraria;
using Terraria.ModLoader;

namespace V2.Items.ItemGroupUtils;

public class Baits : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
        V2Utils.ItemIDSets.Baits.Contains(entity.type);

    public override void SetDefaults(Item entity)
    {
        entity.AsFood().MaxHealth = 20;
        entity.AsFood().Size = 0.03;
        entity.AsFood().WellFedPower = 0.001d;
    }
}
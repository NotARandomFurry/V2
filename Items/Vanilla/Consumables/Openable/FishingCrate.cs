using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core.MainDetours;
using V2.PlayerHandling;

namespace V2.Items.Vanilla.Placeables.Crates;

public class FishingCrate : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
        ItemID.Sets.IsFishingCrate[entity.type];

    public override void SetDefaults(Item entity)
    {
        PreyItem preyItem = entity.AsFood();
        preyItem.MaxHealth = 650;
        preyItem.Size = 2.4d;
        preyItem.WellFedPower = 0.12d;
        //preyItem.OnSwallowSoreThroatTime = V2Utils.SensibleTime(seconds: 10);

        preyItem.OnBreak += OnBreak;
    }

    private static bool OnBreak(Item item, Entity pred, bool direct)
    {
        if (pred is not Player predPlayer) return true;
        
        predPlayer.AsPred().LootWasJustDigested = true;
        predPlayer.DropFromItem(item.type);
        predPlayer.AsPred().LootWasJustDigested = false;
        
        return true;
    }
}
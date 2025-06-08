using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Crates;

public class CratesItem : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return ItemID.Sets.IsFishingCrate[entity.type];
    }

    public override void SetDefaults(Item entity)
    {
        var preyItem = entity.AsFood();
        preyItem.MaxHealth = 350;
        preyItem.Size = 2.4d;
        preyItem.WellFedPower = 0.12d;
        //preyItem.OnSwallowSoreThroatTime// = //V2Utils.SensibleTime(seconds: 10);

        preyItem.OnBreak += OnBreak;
    }

    private static bool OnBreak(Item item, Entity pred, bool direct)
    {
        if (pred is Player predPlayer)
            // var rulesForItemId = Main.ItemDropsDB.GetRulesForItemID(item.type);
            // foreach (var itemDropRule in rulesForItemId)
            // {
            //     V2.Instance.Logger.Info($"{itemDropRule}");
            // }
            // TODO: Intercept drops from NewItem_Inner in Item.cs (Terraria core file)
            predPlayer.DropFromItem(item.type);
        ;
        return true;
    }
}
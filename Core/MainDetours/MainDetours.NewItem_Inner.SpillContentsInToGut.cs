using System.Linq;
using Terraria;
using Terraria.DataStructures;
using V2.Items;
using V2.PlayerHandling;

namespace V2.Core.MainDetours;

public static partial class MainDetours
{
    public static int SpillLootInToGut(On_Item.orig_NewItem_Inner orig, IEntitySource source, int x, int y, int width,
        int height, Item itemToClone, int type, int stack, bool noBroadcast, int pfix, bool noGrabDelay,
        bool reverseLookup)
    {
        int itemIdx = orig(source, x, y, width, height, itemToClone, type, stack, noBroadcast, pfix,
            noGrabDelay, reverseLookup);
        Item newItem = Main.item[itemIdx];

        Player pred = source switch
        {
            EntitySource_TileInteraction te_src => te_src.Entity as Player,
            EntitySource_ItemOpen io_src => io_src.Player,
            EntitySource_Loot l_src => l_src.Entity as Player,
            _ => null
        };

        if ((pred?.AsPred().LootWasJustDigested ?? false) && newItem.AsFood().MaxHealth > 0)
        {
            PredPlayer.AddNewPrey(pred, PreyData.NewData(newItem));
        }


        return itemIdx;
    }
}
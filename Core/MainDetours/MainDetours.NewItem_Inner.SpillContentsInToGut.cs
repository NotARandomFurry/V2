using System.Linq;
using Terraria;
using Terraria.DataStructures;
using V2.Items;
using V2.PlayerHandling;

namespace V2.Core.MainDetours;

public static partial class MainDetours
{
    /// <summary>
    ///     What a stupid way to fix this issue:
    ///     "So like, sure this works but it throws loot content if you right click as usual while digesting a nice loot meal?
    ///     how tf fix this???!?!"
    /// </summary>
    public static bool CrateWasJustDigested { get; set; }

    public static int SpillLootInToGut(On_Item.orig_NewItem_Inner orig, IEntitySource source, int x, int y, int width,
        int height, Item itemToClone, int type, int stack, bool noBroadcast, int pfix, bool noGrabDelay,
        bool reverseLookup)
    {
        if (source is not EntitySource_ItemOpen src)
            return orig(source, x, y, width, height, itemToClone, type, stack, noBroadcast, pfix, noGrabDelay,
                reverseLookup);
        
        bool lootSourceWasInGut = CrateWasJustDigested &&
                                  (src.Player.AsPred().StomachTracker?.Prey.Any(e => e.ExactType == src.ItemType) ??
                                   false);
        if (!lootSourceWasInGut)
            return orig(source, x, y, width, height, itemToClone, type, stack, noBroadcast, pfix, noGrabDelay,
                reverseLookup);

        V2.Instance.Logger.Info("spill the loot in to the gut");
        // Item spilledItem = new Item(type, stack, pfix);
        // spilledItem.SetDefaults(type);
        int itemIdx = orig(source, x, y, width, height, itemToClone, type, stack, noBroadcast, pfix,
            noGrabDelay, reverseLookup);

        Item item = Main.item[itemIdx];
        if (item.AsFood().MaxHealth > 0) PredPlayer.AddNewPrey(src.Player, PreyData.NewData(item));
        return itemIdx;
    }
}
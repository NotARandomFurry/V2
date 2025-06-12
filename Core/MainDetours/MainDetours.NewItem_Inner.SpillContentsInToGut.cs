using System.Linq;
using Terraria;
using Terraria.DataStructures;
using V2.Items;
using V2.PlayerHandling;

namespace V2.Core.MainDetours;

public static partial class MainDetours
{
    /// <summary>
    /// Set this flag to <b>True</b> when an openable loot (bag, silt or similar) has been digested. This will cause additional items to try to be forced in to the gut
    /// Make sure to set it to <b>False</b> using a detour in the appropriate "event".
    /// </summary>
    
    // What a garbage way to make this feature work regarding digesting crates and other openable loot... Oh well, it works at least
    public static bool LootWasJustDigested { get; set; }

    public static int SpillLootInToGut(On_Item.orig_NewItem_Inner orig, IEntitySource source, int x, int y, int width,
        int height, Item itemToClone, int type, int stack, bool noBroadcast, int pfix, bool noGrabDelay,
        bool reverseLookup)
    {

        bool lootSourceWasInGut = LootWasJustDigested; //&&
                                  // (src.Player.AsPred().StomachTracker?.Prey.Any(e => e.ExactType == src.ItemType) ??
                                  //  false);
        if (!lootSourceWasInGut)
            return orig(source, x, y, width, height, itemToClone, type, stack, noBroadcast, pfix, noGrabDelay,
                reverseLookup);

        V2.Instance.Logger.Info("spill the loot in to the gut");
        // Item spilledItem = new Item(type, stack, pfix);
        // spilledItem.SetDefaults(type);
        int itemIdx = orig(source, x, y, width, height, itemToClone, type, stack, noBroadcast, pfix,
            noGrabDelay, reverseLookup);

        Item item = Main.item[itemIdx];
        if (item.AsFood().MaxHealth > 0)
        {
            Player pred = source switch
            {
                EntitySource_TileInteraction te_src => te_src.Entity as Player,
                EntitySource_ItemOpen io_src => io_src.Player
            };
            
            if (pred is not null)
                PredPlayer.AddNewPrey(pred, PreyData.NewData(item));
        }
        return itemIdx;
    }
}
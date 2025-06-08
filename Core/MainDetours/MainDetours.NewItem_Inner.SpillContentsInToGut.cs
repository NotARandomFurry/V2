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
        int result;

        if (source is EntitySource_ItemOpen src)
        {
            // So like, sure this works but it throws loot content if you right click as usual while digesting a nice boxy meal?
            // how tf fix this???!?!
            var lootSourceWasInGut =
                src.Player.AsPred().StomachTracker?.Prey.Any(e => e.ExactType == src.ItemType) ?? false;
            if (lootSourceWasInGut)
            {
                V2.Instance.Logger.Info("spill the loot in to the gut");
                var item = new Item();
                item.SetDefaults(type);
                item.stack = stack;
                if (item.AsFood().MaxHealth >= 0)
                {
                    PredPlayer.AddNewPrey(src.Player, PreyData.NewData(item));
                    return 0;
                }
            }
        }


        result = orig(source, x, y, width, height, itemToClone, type, stack, noBroadcast, pfix, noGrabDelay,
            reverseLookup);
        return result;
    }
}
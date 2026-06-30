using System.Linq;
using Terraria;
using Terraria.DataStructures;
using V2.Items;
using V2.NPCs;
using V2.PlayerHandling;

namespace V2.Core.MainDetours;

public static partial class MainDetours
{
    /// <summary>
    /// Captures any newly spawned entity and throws them into the predator's gut if it was loot that was digested. </br>
    /// Before calling any loot or item-dropping methods. Ensure to call <see cref="V2Utils.MarkLootDigested()"/>
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="source"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="itemToClone"></param>
    /// <param name="type"></param>
    /// <param name="stack"></param>
    /// <param name="noBroadcast"></param>
    /// <param name="pfix"></param>
    /// <param name="noGrabDelay"></param>
    /// <param name="reverseLookup"></param>
    /// <returns>Item index</returns>
    public static int SpillLootInToGutFromItemDrops(On_Item.orig_NewItem_Inner orig, IEntitySource source, int x, int y, int width,
        int height, Item itemToClone, int type, int stack, bool noBroadcast, int pfix, bool noGrabDelay,
        bool reverseLookup)
    {
        int itemIdx = orig(source, x, y, width, height, itemToClone, type, stack, noBroadcast, pfix,
            noGrabDelay, reverseLookup);
        
        Item newItem = Main.item[itemIdx];

        Entity pred = source switch
        {
            EntitySource_TileInteraction te_src => te_src.Entity,
            EntitySource_ItemOpen io_src => io_src.Player,
            EntitySource_Loot l_src => l_src.Entity,
            _ => null
        };

        if (pred is null || newItem.AsFood().MaxHealth <= 0)
            return itemIdx;

        switch (pred)
        {
            case Player player:
            {
                if(player.AsPred().LootDigested())
                    PredPlayer.AddNewPrey(player, PreyData.NewData(newItem));
                break;
            }
            case NPC npc:
            {
                if (npc.AsPred().LootDigested())
                    PredNPC.AddNewPrey(npc, PreyData.NewData(newItem));
                break;
            }
        }
        return itemIdx;
    }
}
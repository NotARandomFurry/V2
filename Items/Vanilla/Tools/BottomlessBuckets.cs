using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items;
using V2.Items.Voraria.Consumables.PermanentUpgrades;
using V2.Items.Voraria.Consumables.PermanentUpgrades.Jujus;
using V2.NPCs;
using V2.PlayerHandling.PredPlayerGoals;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Starter;
using V2.Projectiles;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Buffs;
using V2.StatusEffects.Voraria.Debuffs;
using V2.Items.Voraria.Accessories.Transformations;
using V2.Items.Voraria.Accessories.Transformations.Baelz;
using V2.PlayerHandling;

namespace V2.Items.Vanilla.Tools
{
    public class BottomlessBuckets : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
            entity.type == ItemID.BottomlessBucket || entity.type == ItemID.BottomlessLavaBucket || entity.type == ItemID.BottomlessHoneyBucket || entity.type == ItemID.BottomlessShimmerBucket;

        public override void SetDefaults(Item item)
        {
            item.AsFood().PreSwallow = PreSwallow;
        }

        public static bool PreSwallow(Item item, Entity pred)
        {
            int liquid = item.type switch
            {
                ItemID.BottomlessBucket => LiquidID.Water,
                ItemID.BottomlessLavaBucket => LiquidID.Lava,
                ItemID.BottomlessHoneyBucket => LiquidID.Honey,
                ItemID.BottomlessShimmerBucket => LiquidID.Shimmer,
                _ => LiquidID.Water,
            };
            if (pred is Player predPlayer)
            {
                //if (predPlayer.AsPred().Rose || predPlayer.AsPred().StomachCapacity - predPlayer.AsPred().StomachFullness >= predPlayer.AsPred().EffectiveLiquidSwallowSize(liquid))
                //{
                    PredPlayer.Drink(predPlayer, liquid, predPlayer.AsPred().LiquidSwallowSize);
                //}
            }
            return true;
        }
    }
}

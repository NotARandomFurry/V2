using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;

namespace V2.Items.Vanilla.Consumables.Food
{
    //this is hella lazily done i should probably do them one by one later, but for now it also gets modded food items so thats good i think
    public class FoodT1 : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.buffType is BuffID.WellFed;

        public override void SetDefaults(Item item)
        {
            item.AsFood().MaxHealth = (int)(10 * Math.Log(item.buffTime / 8d, 2.08));
            item.AsFood().Size = (0.01 * Math.Log(item.buffTime / 8d, 2.16)).CastToDecimalPlaces(3);

            item.AsFood().CalorieMultiplier = 1.2;

            item.AsFood().VanillaWellFedDuration = item.buffTime;

            item.AsFood().UpdateInStomach += UpdateInStomach;

            item.buffType = 0;
            item.buffTime = 0;

            item.AsFood().EdibleOnUse = true;
            item.AsFood().AlwaysEatenByUse = true;
        }

        public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
        {
            if (prey is Item)
            {
                Item preyItem = prey as Item;
                pred.AddStatus(BuffID.WellFed, preyItem.AsFood().VanillaWellFedDuration, true);
            }
        }
    }
    public class FoodT2 : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.buffType is BuffID.WellFed2;

        public override void SetDefaults(Item item)
        {
            item.AsFood().MaxHealth = (int)(10 * Math.Log(item.buffTime / 4d, 2.08));
            item.AsFood().Size = (0.01 * Math.Log(item.buffTime / 4d, 2.16)).CastToDecimalPlaces(3);

            item.AsFood().CalorieMultiplier = 1.2;

            item.AsFood().VanillaWellFedDuration = item.buffTime;

            item.AsFood().UpdateInStomach += UpdateInStomach;

            item.buffType = 0;
            item.buffTime = 0;

            item.AsFood().EdibleOnUse = true;
            item.AsFood().AlwaysEatenByUse = true;
        }

        public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
        {
            if (prey is Item)
            {
                Item preyItem = prey as Item;
                pred.AddStatus(BuffID.WellFed2, preyItem.AsFood().VanillaWellFedDuration, true);
            }
        }
    }
    public class FoodT3 : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.buffType is BuffID.WellFed3;

        public override void SetDefaults(Item item)
        {
            item.AsFood().MaxHealth = (int)(10 * Math.Log(item.buffTime / 1.5d, 2.08));
            item.AsFood().Size = (0.01 * Math.Log(item.buffTime / 1.5d, 2.16)).CastToDecimalPlaces(3);

            item.AsFood().CalorieMultiplier = 1.2;

            item.AsFood().VanillaWellFedDuration = item.buffTime;

            item.AsFood().UpdateInStomach += UpdateInStomach;

            item.buffType = 0;
            item.buffTime = 0;

            item.AsFood().EdibleOnUse = true;
            item.AsFood().AlwaysEatenByUse = true;
        }

        public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
        {
            if (prey is Item)
            {
                Item preyItem = prey as Item;
                pred.AddStatus(BuffID.WellFed3, preyItem.AsFood().VanillaWellFedDuration, true);
            }
        }
    }
}

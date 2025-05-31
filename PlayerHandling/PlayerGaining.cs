using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
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


namespace V2.PlayerHandling
{
    internal class PlayerGaining
    {
        public static double CalculateGain(Player pred, double amount, PreyData prey)
        {
            double effectiveSize = PredPlayer.GetCurrentWeight(pred);
            return amount * prey.CalorieMultiplier * (1 / effectiveSize) * pred.AsPred().BaseWeightGainRatio;
        }
        public static double CalculateGain(Player pred, double amount)
        {
            double effectiveSize = PredPlayer.GetCurrentWeight(pred);
            return amount * (1 / effectiveSize) * pred.AsPred().BaseWeightGainRatio;
        }
        public static void AddWeight(Player pred, double amount, PreyData prey)
        {
            if (amount > 0 && pred.AsPred().ActuallyReasonableAmountOfFood < 0.1)
            {
                pred.AsPred().ActuallyReasonableAmountOfFood = Math.Max(pred.AsPred().ActuallyReasonableAmountOfFood + CalculateGain(pred, amount, prey), 0);
            }
            else
            {
                if (pred.AsV2Player().BeeTransformation == true)
                {
                    pred.AsPred().BeeTransformation_ExtraWeight = Math.Max((pred.AsPred().BeeTransformation_ExtraWeight + CalculateGain(pred, amount, prey)) * pred.AsPred().WeightGainMultiplier, 0);
                }
                else if (pred.AsV2Player().BaeTransformation == true)
                {
                    pred.AsPred().BaeTransformation_ExtraWeight = Math.Max((pred.AsPred().BaeTransformation_ExtraWeight + CalculateGain(pred, amount, prey)) * pred.AsPred().WeightGainMultiplier, 0);
                }
            }
        }
        public static void AddWeight(Player pred, double amount)
        {
            if (amount > 0 && pred.AsPred().ActuallyReasonableAmountOfFood < 0.1)
            {
                pred.AsPred().ActuallyReasonableAmountOfFood = Math.Max(pred.AsPred().ActuallyReasonableAmountOfFood + CalculateGain(pred, amount), 0);
            }
            else
            {
                if (pred.AsV2Player().BeeTransformation == true)
                {
                    pred.AsPred().BeeTransformation_ExtraWeight = Math.Max((pred.AsPred().BeeTransformation_ExtraWeight + CalculateGain(pred, amount)) * pred.AsPred().WeightGainMultiplier, 0);
                }
                else if (pred.AsV2Player().BaeTransformation == true)
                {
                    pred.AsPred().BaeTransformation_ExtraWeight = Math.Max((pred.AsPred().BaeTransformation_ExtraWeight + CalculateGain(pred, amount)) * pred.AsPred().WeightGainMultiplier, 0);
                }
            }
        }
        public static void ReduceWeight(Player pred, double amount)
        {
            if (pred.AsPred().ActuallyReasonableAmountOfFood > 0)
            {
                pred.AsPred().ActuallyReasonableAmountOfFood = Math.Max(pred.AsPred().ActuallyReasonableAmountOfFood - amount * pred.AsPred().WeightLossMultiplier, 0);
            }
            else
            {
                if (pred.AsV2Player().BeeTransformation == true)
                {
                    pred.AsPred().BeeTransformation_ExtraWeight = Math.Max(pred.AsPred().BeeTransformation_ExtraWeight - amount * pred.AsPred().WeightLossMultiplier, 0);
                }
                else if (pred.AsV2Player().BaeTransformation == true)
                {
                    pred.AsPred().BaeTransformation_ExtraWeight = Math.Max(pred.AsPred().BaeTransformation_ExtraWeight - amount * pred.AsPred().WeightLossMultiplier, 0);
                }
            }
        }
    }
}

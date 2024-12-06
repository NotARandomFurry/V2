using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Consumables.Potions;

public class IronSkinPotion : PotionTemplate
{
    public override string TooltipTranslationKey => "Vanilla.Consumables.Potions.IronSkinPotion";
    public override int DigestedPotionEffectID => BuffID.Ironskin;
    public override int DigestedPotionEffectDuration => V2Utils.SensibleTime(minutes: 8);
    public override int AppliesToPotionItem => ItemID.IronskinPotion;

    public override dynamic TooltipVariables()
    {
        int defenseValue = Main.masterMode ? 8 : Main.expertMode ? 6 : 4;
        return new
        {
            DefenseValue = defenseValue
        };
    }
}
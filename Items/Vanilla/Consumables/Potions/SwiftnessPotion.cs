using Terraria.ID;
using V2.Core;
using V2.StatusEffects.Vanilla.Buffs;

namespace V2.Items.Vanilla.Consumables.Potions
{
	public class SwiftnessPotion : PotionTemplate
	{
		public override string TooltipTranslationKey => "Vanilla.Consumables.Potions.Swiftness";
		public override int DigestedPotionEffectID => BuffID.Swiftness;
		public override int DigestedPotionEffectDuration => V2Utils.SensibleTime(minutes: 8);
		public override int AppliesToPotionItem => ItemID.SwiftnessPotion;

		public override dynamic TooltipVariables()
		{
			return new
			{
				SwiftnessPotionMoveSpeedBonus = SwiftnessBuff.MoveSpeedBonus.ToPercentage(2),
				SwiftnessPotionStomachWeightReduction = SwiftnessBuff.StomachWeightReduction.ToPercentage(2),
			};
		}
	}
}
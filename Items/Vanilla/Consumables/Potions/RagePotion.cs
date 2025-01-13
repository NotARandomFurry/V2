using Terraria.ID;
using V2.Core;
using V2.StatusEffects.Vanilla.Buffs;

namespace V2.Items.Vanilla.Consumables.Potions
{
	public class RagePotion : PotionTemplate
	{
		public override string TooltipTranslationKey => "Vanilla.Consumables.Potions.Rage";
		public override int DigestedPotionEffectID => BuffID.Rage;
		public override int DigestedPotionEffectDuration => V2Utils.SensibleTime(minutes: 3);
		public override int AppliesToPotionItem => ItemID.RagePotion;

		public override dynamic TooltipVariables()
		{
			return new
			{
				RagePotionCritChanceBoost = RageBuff.CritChanceBonus.ToPercentage(2),
				RagePotionGLPBoost = RageBuff.GLPBonus,
				RagePotionABSBoost = RageBuff.ABSBonus,
			};
		}
	}
}

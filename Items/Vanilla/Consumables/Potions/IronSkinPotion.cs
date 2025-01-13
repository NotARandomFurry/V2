using Terraria.ID;

namespace V2.Items.Vanilla.Consumables.Potions
{
	public class IronskinPotion : PotionTemplate
	{
		public override string TooltipTranslationKey => "Vanilla.Consumables.Potions.Ironskin";
		public override int DigestedPotionEffectID => BuffID.Ironskin;
		public override int DigestedPotionEffectDuration => V2Utils.SensibleTime(minutes: 8);
		public override int AppliesToPotionItem => ItemID.IronskinPotion;

		public override dynamic TooltipVariables()
		{
			int defenseValue = 8;
			return new
			{
				IronskinPotionDefenseValue = defenseValue
			};
		}
	}
}
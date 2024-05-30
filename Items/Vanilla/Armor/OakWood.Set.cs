using Terraria;
using Terraria.ID;
using Terraria.Localization;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Vanilla.Armor
{
	public class OakWoodSet : ArmorSetDefinition
	{
		public override (int? head, int? body, int? legs) RequiredEquipment => (
			head: ItemID.WoodHelmet,
			body: ItemID.WoodBreastplate,
			legs: ItemID.WoodGreaves
		);

		public override string SetBonusDescriptionKey => "Vanilla.Armor.OakWood.SetBonus";

		public override object SetBonusDescriptionVariables => new { };

		public override void ApplySetBonus(Player player)
		{
			player.AddHealthRegenEffect(
				healthPerSecond: 0.0,
				modifyTotalHealthRegenMethod: ModifyTotalHealthRegen
			);
		}

		public static void ModifyTotalHealthRegen(Player player, ref double naturalRegenAdditive, ref double naturalRegenMultiplicative, ref double artificialRegenAdditive, ref double artificialRegenMultiplicative)
		{
			naturalRegenAdditive += 0.05f;	
			if (player.position.Y < Main.worldSurface && player.behindBackWall && Main.dayTime)
				naturalRegenAdditive += 0.05f;
		}
	}
}

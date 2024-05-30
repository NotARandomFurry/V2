using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.NormalAssWood.Furniture
{
	public class OakWoodDoor : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodenDoor;

		public override void SetDefaults(Item item)
		{
			item.SetNameOverride(Language.GetTextValue("Mods.V2.ItemName.Vanilla.Placeables.NormalAssWood.Furniture.Door"));

			item.AsFood().MaxHealth = 250;
			item.AsFood().Size = 1.00;
		}
	}
}

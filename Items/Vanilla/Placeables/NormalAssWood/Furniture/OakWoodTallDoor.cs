using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.NormalAssWood.Furniture
{
	public class OakWoodTallDoor : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.TallGate;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 500;
			item.AsFood().Size = 1.7;
		}
	}
}

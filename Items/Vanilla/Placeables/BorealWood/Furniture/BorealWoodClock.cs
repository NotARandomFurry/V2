using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.BorealWood.Furniture
{
	public class BorealWoodClock : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodClock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 323;
			item.AsFood().Size = 1.84;
		}
	}
}

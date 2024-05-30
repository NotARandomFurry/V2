using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.BorealWood.Furniture
{
	public class BorealWoodBookcase : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodBookcase;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 485;
			item.AsFood().Size = 3.12;
		}
	}
}

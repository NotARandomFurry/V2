using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.NormalAssWood.Furniture
{
	public class OakWoodWall : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodWall;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 500;
			item.AsFood().Size = 2.0;
		}
	}
}

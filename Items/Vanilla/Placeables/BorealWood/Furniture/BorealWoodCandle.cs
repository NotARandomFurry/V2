using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.BorealWood.Furniture
{
	public class BorealWoodCandle : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodCandle;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 84;
			item.AsFood().Size = 0.28;
		}
	}
}

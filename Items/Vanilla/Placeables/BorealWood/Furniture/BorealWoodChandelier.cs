using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.BorealWood.Furniture
{
	public class BorealWoodChandelier : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodChandelier;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 479;
			item.AsFood().Size = 1.152;
		}
	}
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Paintings
{
	internal class DoNotEatTheVileMushroom : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.DoNotEattheVileMushroom;
        public override void SetDefaults(Item item)
        {
            item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Vanilla.Paintings.DoNotEatTheVileMushroom>());

			item.AsFood().MaxHealth = 1200;
			item.AsFood().Size = 3.0;

			item.AsAnItem().PlaceableCanBeHungry = true;
			item.AsAnItem().PlaceableHungryByDefault = true;
		}

    }
}

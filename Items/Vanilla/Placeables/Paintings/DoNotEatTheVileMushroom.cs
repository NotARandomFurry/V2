using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Paintings
{
	internal class DoNotEatTheVileMushroom : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.DoNotEattheVileMushroom;
        public override void SetDefaults(Item entity)
        {
            entity.DefaultToPlaceableTile(ModContent.TileType<Tiles.Vanilla.Paintings.DoNotEatTheVileMushroom>());
        }

    }
}

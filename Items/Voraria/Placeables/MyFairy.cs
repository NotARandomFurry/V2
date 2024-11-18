using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Voraria.Placeables
{
	public class MyFairy : ModItem
	{
		public override void SetDefaults() {
			// Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.MyFairy>());

			Item.width = 32;
			Item.height = 32;
			Item.rare = ItemRarityID.Purple;
			Item.value = Item.buyPrice(1);
		}
	}
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Voraria.Placeables
{
	public class MyFairy : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Voraria.Paintings.MyFairy>());

			Item.width = 32;
			Item.height = 32;
			Item.rare = ItemRarityID.Purple;
			Item.value = Item.buyPrice(1);
		}
	}
}

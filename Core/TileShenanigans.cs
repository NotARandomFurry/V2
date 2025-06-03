using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace V2.TileBullshit
{
	public static partial class HelperMethods
	{
		//definitely didnt steal this from somewhere (TERRARIA SUCKS ASS YOU CANT BLAME ME !!!!!)
		internal static int PickItem(Tile tile)
		{
			Player player = Main.LocalPlayer;
			Item item = new Item();
			int itemID = -1;
			bool foundItem = false;

			if (tile.HasTile && tile.TileType >= 0)
			{
				for (int i = 0; i < ItemLoader.ItemCount; i++)
				{
					item.SetDefaults(i);
					if (item.createTile == tile.TileType)
					{
						foundItem = true;
						itemID = i;
						if (itemID == ItemID.GrassSeeds || itemID == ItemID.CorruptSeeds || itemID == ItemID.CrimsonSeeds || itemID == ItemID.HallowedSeeds)
							itemID = ItemID.DirtBlock;
						else if(itemID == ItemID.JungleGrassSeeds || itemID == ItemID.MushroomGrassSeeds)
							itemID = ItemID.MudBlock;
						else if (itemID == ItemID.AshGrassSeeds)
							itemID = ItemID.AshBlock;
						break;
					}
				}
			}
			else if (tile.TileType >= 0 && tile.WallType >= 0)
			{
				for (int i = 0; i < ItemLoader.ItemCount; i++)
				{
					item.SetDefaults(i);
					if (item.createWall == tile.WallType)
					{
						foundItem = true;
						itemID = i;
						break;
					}
				}
			}

			//organize inventory
			if (foundItem)
			{
				//Furniture Check
				//If it is a furniture and has a different frame, item will be changed to the correct frame item
				int furnitureTileType = FindFurniture(tile, ref item);
				if (furnitureTileType != -1)
					itemID = furnitureTileType;
			}

			return itemID;
		}

		internal static int FindFurniture(Tile tile, ref Item item)
		{
			int tilePlaceStyle = TileObjectData.GetTileStyle(tile);
			int originalItemType = item.type;
			for (int i = 0; i < ItemLoader.ItemCount; i++)
			{
				item.SetDefaults(i);
				if (item.createTile == tile.TileType && item.placeStyle == tilePlaceStyle)
					return i;
			}

			//if it reaches here, didn't find any matches
			item.SetDefaults(originalItemType);
			return -1;
		}
	}
}
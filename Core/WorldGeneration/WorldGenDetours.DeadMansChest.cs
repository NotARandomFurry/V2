using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using V2.Items.Voraria.Charms;

namespace V2.Core.WorldGeneration
{
	public static partial class WorldGenDetours
	{
		public static void TurnGoldChestIntoDeadMansChest(Point position)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int num = position.X + i;
					int num2 = position.Y + j;
					Tile tile = Main.tile[num, num2];
					tile.TileType = 467;
					tile.TileFrameX = (short)(144 + i * 18);
					tile.TileFrameY = (short)(j * 18);
				}
			}

			int num3 = Chest.FindChest(position.X, position.Y);
			if (num3 <= -1)
				return;

			Item[] item = Main.chest[num3].item;
			if (!WorldGen.genRand.NextBool(3))
				goto tryGenItemTheftCharm;

			for (int num4 = item.Length - 2; num4 > 0; num4--)
			{
				Item item2 = item[num4];
				if (item2.stack != 0)
					item[num4 + 1] = item2.Clone();
			}

			item[1] = new Item();
			item[1].SetDefaults(ItemID.DeadMansSweater);
			Main.chest[num3].item = item;

			tryGenItemTheftCharm:
			if (WorldGen.genRand.NextBool(4))
				return;

			for (int num4 = item.Length - 2; num4 > 0; num4--)
			{
				Item item2 = item[num4];
				if (item2.stack != 0)
					item[num4 + 1] = item2.Clone();
			}

			item[1] = new Item();
			item[1].SetDefaults(ModContent.ItemType<CharmPreyItemTheft>());
			Main.chest[num3].item = item;
		}
	}
}

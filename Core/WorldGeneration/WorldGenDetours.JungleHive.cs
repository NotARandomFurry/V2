using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using V2.Items.Voraria.Charms;
using V2.NPCs.Voraria.Jungle;
using V2.Tiles.Voraria;

namespace V2.Core.WorldGeneration
{
	public static partial class WorldGenDetours
	{
		public static bool HiveBiome_Place(HiveBiome self, Point origin, Terraria.WorldBuilding.StructureMap structures)
		{
			bool HasAComicallyFatFairy = Main.rand.NextBool(2);
			if (!structures.CanPlace(new Rectangle(origin.X - 50, origin.Y - 50, 100, 100), 0))
			{
				return false;
			}
			if (TooCloseToImportantLocations(origin))
			{
				return false;
			}
			Ref<int> @ref = new Ref<int>(0);
			Ref<int> ref2 = new Ref<int>(0);
			Ref<int> ref3 = new Ref<int>(0);
			WorldUtils.Gen(origin, new Shapes.Circle(15), Actions.Chain(new GenAction[]
			{
				new Modifiers.IsSolid(),
				new Actions.Scanner(@ref),
				new Modifiers.OnlyTiles(new ushort[]
				{
					60,
					59
				}),
				new Actions.Scanner(ref2),
				new Modifiers.OnlyTiles(new ushort[]
				{
					60
				}),
				new Actions.Scanner(ref3)
			}));
			if ((double)ref2.Value / (double)@ref.Value < 0.75 || ref3.Value < 2)
			{
				return false;
			}
			int num = 0;
			int[] array = new int[1000];
			int[] array2 = new int[1000];
			Vector2D vector2D = origin.ToVector2D();
			int num2 = WorldGen.genRand.Next(2, 5);
			if (WorldGen.drunkWorldGen)
			{
				num2 += WorldGen.genRand.Next(7, 10);
			}
			else if (WorldGen.remixWorldGen)
			{
				num2 += WorldGen.genRand.Next(2, 5);
			}
			for (int i = 0; i < num2; i++)
			{
				Vector2D vector2D2 = vector2D;
				int num3 = WorldGen.genRand.Next(2, 5);
				for (int j = 0; j < num3; j++)
				{
					vector2D2 = CreateHiveTunnel((int)vector2D.X, (int)vector2D.Y, WorldGen.genRand, HasAComicallyFatFairy);
				}
				vector2D = vector2D2;
				array[num] = (int)vector2D.X;
				array2[num] = (int)vector2D.Y;
				num++;
			}
			FrameOutAllHiveContents(origin, 50);
			for (int k = 0; k < num; k++)
			{
				int num4 = array[k];
				int y = array2[k];
				int num5 = 1;
				if (WorldGen.genRand.NextBool(2))
				{
					num5 = -1;
				}
				bool flag = false;
				while (WorldGen.InWorld(num4, y, 10) && BadSpotForHoneyFall(num4, y))
				{
					num4 += num5;
					if (Math.Abs(num4 - array[k]) > 50)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					num4 += num5;
					if (!SpotActuallyNotInHive(num4, y))
					{
						CreateBlockedHoneyCube(num4, y);
						CreateDentForHoneyFall(num4, y, num5);
					}
				}
			}
			HiveBiome.CreateStandForLarva(vector2D);
			if (WorldGen.drunkWorldGen)
			{
				for (int l = 0; l < 1000; l++)
				{
					Vector2D vector2D3 = vector2D;
					vector2D3.X += (double)WorldGen.genRand.Next(-50, 51);
					vector2D3.Y += (double)WorldGen.genRand.Next(-50, 51);
					if (WorldGen.InWorld((int)vector2D3.X, (int)vector2D3.Y, 0) && Vector2D.Distance(vector2D, vector2D3) > 10.0 && !Main.tile[(int)vector2D3.X, (int)vector2D3.Y].HasTile && Main.tile[(int)vector2D3.X, (int)vector2D3.Y].WallType == WallID.HiveUnsafe)
					{
						HiveBiome.CreateStandForLarva(vector2D3);
						break;
					}
				}
			}
			Tile spawnerTile = Main.tile[origin.X, origin.Y];
			spawnerTile.HasTile = true;
			spawnerTile.TileType = ((ushort)ModContent.TileType<Spawner>());
			structures.AddProtectedStructure(new Rectangle(origin.X - 50, origin.Y - 50, 100, 100), 5);
			return true;
		}
		public static void FrameOutAllHiveContents(Point origin, int squareHalfWidth)
		{
			int num5 = Math.Max(10, origin.X - squareHalfWidth);
			int num2 = Math.Min(Main.maxTilesX - 10, origin.X + squareHalfWidth);
			int num3 = Math.Max(10, origin.Y - squareHalfWidth);
			int num4 = Math.Min(Main.maxTilesY - 10, origin.Y + squareHalfWidth);
			for (int i = num5; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.HasTile && tile.TileType == TileID.Hive)
					{
						WorldGen.SquareTileFrame(i, j, true);
					}
					if (tile.WallType == WallID.HiveUnsafe)
					{
						WorldGen.SquareWallFrame(i, j, true);
					}
				}
			}
		}
		public static Vector2D CreateHiveTunnel(int i, int j, UnifiedRandom random, bool drained)
		{
			double num = (double)random.Next(12, 21);
			double num2 = (double)random.Next(10, 21);
			if (WorldGen.drunkWorldGen)
			{
				num = (double)random.Next(8, 26);
				num2 = (double)random.Next(10, 41);
				double num3 = (double)Main.maxTilesX / 4200.0;
				num3 = (num3 + 1.0) / 2.0;
				num *= num3;
				num2 *= num3;
			}
			else if (WorldGen.remixWorldGen)
			{
				num += (double)random.Next(3);
			}
			double num4 = num;
			Vector2D result = default(Vector2D);
			result.X = (double)i;
			result.Y = (double)j;
			Vector2D vector2D = default(Vector2D);
			vector2D.X = (double)random.Next(-10, 11) * 0.2;
			vector2D.Y = (double)random.Next(-10, 11) * 0.2;
			while (num > 0.0 && num2 > 0.0)
			{
				if (result.Y > (double)(Main.maxTilesY - 250))
				{
					num2 = 0.0;
				}
				num = num4 * (1.0 + (double)random.Next(-20, 20) * 0.01);
				num2 -= 1.0;
				int num5 = (int)(result.X - num);
				int num6 = (int)(result.X + num);
				int num7 = (int)(result.Y - num);
				int num8 = (int)(result.Y + num);
				if (num5 < 1)
				{
					num5 = 1;
				}
				if (num6 > Main.maxTilesX - 1)
				{
					num6 = Main.maxTilesX - 1;
				}
				if (num7 < 1)
				{
					num7 = 1;
				}
				if (num8 > Main.maxTilesY - 1)
				{
					num8 = Main.maxTilesY - 1;
				}
				for (int k = num5; k < num6; k++)
				{
					for (int l = num7; l < num8; l++)
					{
						if (!WorldGen.InWorld(k, l, 50))
						{
							num2 = 0.0;
						}
						else
						{
							if (Main.tile[k - 10, l].WallType == WallID.LihzahrdBrickUnsafe)
							{
								num2 = 0.0;
							}
							if (Main.tile[k + 10, l].WallType == WallID.LihzahrdBrickUnsafe)
							{
								num2 = 0.0;
							}
							if (Main.tile[k, l - 10].WallType == WallID.LihzahrdBrickUnsafe)
							{
								num2 = 0.0;
							}
							if (Main.tile[k, l + 10].WallType == WallID.LihzahrdBrickUnsafe)
							{
								num2 = 0.0;
							}
						}
						if ((double)l < Main.worldSurface && Main.tile[k, l - 5].WallType == WallID.None)
						{
							num2 = 0.0;
						}
						double num11 = Math.Abs((double)k - result.X);
						double num9 = Math.Abs((double)l - result.Y);
						double num10 = Math.Sqrt(num11 * num11 + num9 * num9);
						Tile thisTile = Main.tile[k, l];
						if (num10 < num4 * 0.4 * (1.0 + (double)random.Next(-10, 11) * 0.005))
						{
							if (random.NextBool(3) && !drained)
							{
								thisTile.LiquidAmount = byte.MaxValue;
							}
							if (WorldGen.drunkWorldGen && !drained)
							{
								thisTile.LiquidAmount = byte.MaxValue;
							}
							thisTile.LiquidType = LiquidID.Honey;
							thisTile.WallType = WallID.HiveUnsafe;
							thisTile.HasTile = false;
							thisTile.IsHalfBlock = false;
							thisTile.Slope = SlopeType.Solid;
						}
						else if (num10 < num4 * 0.75 * (1.0 + (double)random.Next(-10, 11) * 0.005))
						{
							thisTile.LiquidAmount = 0;
							if (thisTile.WallType != WallID.HiveUnsafe)
							{
								thisTile.HasTile = true;
								thisTile.IsHalfBlock = false;
								thisTile.Slope = SlopeType.Solid;
								thisTile.TileType = TileID.Hive;
							}
						}
						if (num10 < num4 * 0.6 * (1.0 + (double)random.Next(-10, 11) * 0.005))
						{
							thisTile.WallType = WallID.HiveUnsafe;
							if (WorldGen.drunkWorldGen && random.NextBool(2))
							{
								thisTile.LiquidAmount = byte.MaxValue;
								thisTile.LiquidType = LiquidID.Honey;
							}
						}
					}
				}
				result += vector2D;
				num2 -= 1.0;
				vector2D.Y += (double)random.Next(-10, 11) * 0.05;
				vector2D.X += (double)random.Next(-10, 11) * 0.05;
			}
			return result;
		}
		public static bool TooCloseToImportantLocations(Point origin)
		{
			int x = origin.X;
			int y = origin.Y;
			int num = 150;
			for (int i = x - num; i < x + num; i += 10)
			{
				if (i > 0 && i <= Main.maxTilesX - 1)
				{
					for (int j = y - num; j < y + num; j += 10)
					{
						if (j > 0 && j <= Main.maxTilesY - 1)
						{
							if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.LihzahrdBrick)
							{
								return true;
							}
							if (Main.tile[i, j].WallType == WallID.CrimstoneUnsafe || Main.tile[i, j].WallType == WallID.EbonstoneUnsafe || Main.tile[i, j].WallType == WallID.LihzahrdBrickUnsafe)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		public static void CreateDentForHoneyFall(int x, int y, int dir)
		{
			dir *= -1;
			y++;
			int num = 0;
			while ((num < 4 || WorldGen.SolidTile(x, y, false)) && x > 10 && x < Main.maxTilesX - 10)
			{
				num++;
				x += dir;
				if (WorldGen.SolidTile(x, y, false))
				{
					WorldGen.PoundTile(x, y);
					Tile thisTile = Main.tile[x, y];
					if (!thisTile.HasTile)
					{
						thisTile.HasTile = true;
						thisTile.TileType = TileID.Hive;
					}
				}
			}
		}
		public static void CreateBlockedHoneyCube(int x, int y)
		{
			for (int i = x - 1; i <= x + 2; i++)
			{
				for (int j = y - 1; j <= y + 2; j++)
				{
					Tile thisTile = Main.tile[i, j];
					if (i >= x && i <= x + 1 && j >= y && j <= y + 1)
					{
						thisTile.HasTile = false;
						thisTile.LiquidAmount = byte.MaxValue;
						thisTile.LiquidType = LiquidID.Honey;
					}
					else
					{
						thisTile.HasTile = true;
						thisTile.TileType = TileID.Hive;
					}
				}
			}
		}
		public static bool SpotActuallyNotInHive(int x, int y)
		{
			for (int i = x - 1; i <= x + 2; i++)
			{
				for (int j = y - 1; j <= y + 2; j++)
				{
					if (i < 10 || i > Main.maxTilesX - 10)
					{
						return true;
					}
					if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType != TileID.Hive)
					{
						return true;
					}
				}
			}
			return false;
		}
		public static bool BadSpotForHoneyFall(int x, int y)
		{
			return !Main.tile[x, y].HasTile || !Main.tile[x, y + 1].HasTile || !Main.tile[x + 1, y].HasTile || !Main.tile[x + 1, y + 1].HasTile;
		}
	}
}

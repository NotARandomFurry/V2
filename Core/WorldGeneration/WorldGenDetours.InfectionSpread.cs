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
using V2.Tiles.Vanilla;
using V2.Tiles.Voraria;

namespace V2.Core.WorldGeneration
{
	public static partial class WorldGenDetours
	{
		public static int CountNearBlocksTypes(int i, int j, int radius, int cap = 0, params int[] tiletypes)
		{
			if (tiletypes.Length == 0)
				return 0;

			int value = i - radius;
			int value2 = i + radius;
			int value3 = j - radius;
			int value4 = j + radius;
			int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			int num2 = 0;
			for (int k = num; k <= value2; k++)
			{
				for (int l = value3; l <= value4; l++)
				{
					if (!Main.tile[k, l].HasTile)
						continue;

					for (int m = 0; m < tiletypes.Length; m++)
					{
						if (tiletypes[m] == Main.tile[k, l].TileType)
						{
							num2++;
							if (cap <= 0 || num2 < cap)
								break;

							return num2;
						}
					}
				}
			}

			return num2;
		}
		public static void SpreadInfectionToNearbyTile(On_WorldGen.orig_SpreadInfectionToNearbyTile orig, int x, int y, int conversionType, int range)
		{
			if (!WorldGen.AllowedToSpreadInfections)
			{
				return;
			}
			if (!Main.hardMode || (NPC.downedPlantBoss && WorldGen.genRand.NextBool(2)))
			{
				return;
			}
			bool keepSpreading = true;
			while (keepSpreading)
			{
				keepSpreading = false;
				int testX = x + WorldGen.genRand.Next(-range, range + 1);
				int testY = y + WorldGen.genRand.Next(-range, range + 1);
				if (!WorldGen.InWorld(testX, testY, 10))
				{
					return;
				}
				if (WorldGen.nearbyChlorophyte(testX, testY))
				{
					WorldGen.ChlorophyteDefense(testX, testY);
					return;
				}
				if (CountNearBlocksTypes(testX, testY, 2, 1, [
					TileID.Sunflower,
					ModContent.TileType<Sunflower>(),
				]) > 0)
				{
					return;
				}
				ushort num = Main.tile[testX, testY].TileType;
				WorldGen.Convert(testX, testY, conversionType, 0, true, false);
				if (num != Main.tile[testX, testY].TileType)
				{
					keepSpreading = WorldGen.genRand.NextBool(2);
				}
			}
		}

		public static void SpreadGrass(On_WorldGen.orig_SpreadGrass orig, int i, int j, int dirt, int grass, bool repeat, TileColorCache color)
		{
			try
			{
				if (WorldGen.InWorld(i, j, 10) && Main.tile[i, j].HasTile && (int)(Main.tile[i, j].TileType) == dirt)
				{
					if (WorldGen.gen && (grass == TileID.CrimsonGrass || grass == TileID.CorruptGrass))
					{
						int num = WorldGen.beachDistance;
						if ((!WorldGen.tenthAnniversaryWorldGen && (double)i > (double)Main.maxTilesX * 0.45 && (double)i <= (double)Main.maxTilesX * 0.55) || i < num || i >= Main.maxTilesX - num)
						{
							return;
						}
					}
					else if ((WorldGen.gen || (grass != TileID.CrimsonGrass && grass != TileID.CorruptGrass && grass != 661 && grass != 662)) && (Main.tile[i, j].TileType != dirt || !Main.tile[i, j].HasTile || ((double)j >= Main.worldSurface && dirt == 0)) && !Main.remixWorld)
					{
						return;
					}
					int num2 = i - 1;
					int num3 = i + 2;
					int num4 = j - 1;
					int num5 = j + 2;
					if (num2 < 0)
					{
						num2 = 0;
					}
					if (num3 > Main.maxTilesX)
					{
						num3 = Main.maxTilesX;
					}
					if (num4 < 0)
					{
						num4 = 0;
					}
					if (num5 > Main.maxTilesY)
					{
						num5 = Main.maxTilesY;
					}
					bool flag = true;
					for (int k = num2; k < num3; k++)
					{
						for (int l = num4; l < num5; l++)
						{
							if (!Main.tile[k, l].HasTile || !Main.tileSolid[Main.tile[k, l].TileType])
							{
								flag = false;
							}
							if (Main.tile[k, l].LiquidType == LiquidID.Lava && Main.tile[k, l].LiquidAmount > 0)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag || !TileID.Sets.CanBeClearedDuringGeneration[Main.tile[i, j].TileType] || ((grass == 23 || grass == 661) && (Main.tile[i, j - 1].TileType == 27 || Main.tile[i, j - 1].TileType == ModContent.TileType<Sunflower>())) || ((grass == 199 || grass == 662) && (Main.tile[i, j - 1].TileType == 27 || Main.tile[i, j - 1].TileType == ModContent.TileType<Sunflower>())) || (grass == 109 && (Main.tile[i, j - 1].TileType == 27 || Main.tile[i, j - 1].TileType == ModContent.TileType<Sunflower>())))
						return;

					Main.tile[i, j].TileType = (ushort)grass;
					Main.tile[i, j].UseBlockColors(color);
					for (int m = num2; m < num3; m++)
					{
						for (int n = num4; n < num5; n++)
						{
							if (Main.tile[m, n].HasTile && Main.tile[m, n].TileType == dirt)
							{
								try
								{
									if (repeat && WorldGen.grassSpread < 1000)
									{
										WorldGen.grassSpread++;
										SpreadGrass(orig, m, n, dirt, grass, true, default);
										WorldGen.grassSpread--;
									}
								}
								catch
								{
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}
		public static void hardUpdateWorld(On_WorldGen.orig_hardUpdateWorld orig, int i, int j)
		{
			if (!Main.hardMode || !Main.tile[i, j].HasTile)
			{
				return;
			}
			int type = Main.tile[i, j].TileType;
			if (type > 0 && TileID.Sets.CanGrowCrystalShards[type] && ((double)j > Main.rockLayer || Main.remixWorld) && WorldGen.genRand.NextBool(5))
			{
				int num = WorldGen.genRand.Next(4);
				int num2 = 0;
				int num3 = 0;
				if (num != 0)
				{
					if (num != 1)
					{
						num3 = ((num != 0) ? 1 : -1);
					}
					else
					{
						num2 = 1;
					}
				}
				else
				{
					num2 = -1;
				}
				if (!Main.tile[i + num2, j + num3].HasTile)
				{
					int num4 = 0;
					int num5 = 6;
					for (int k = i - num5; k <= i + num5; k++)
					{
						for (int l = j - num5; l <= j + num5; l++)
						{
							if (Main.tile[k, l].HasTile && Main.tile[k, l].TileType == 129)
							{
								num4++;
							}
						}
					}
					if (num4 < 2)
					{
						int style = WorldGen.genRand.Next(18);
						if (WorldGen.genRand.Next(50) == 0)
						{
							style = 18 + WorldGen.genRand.Next(6);
						}
						WorldGen.PlaceTile(i + num2, j + num3, 129, true, false, -1, style);
						NetMessage.SendTileSquare(-1, i + num2, j + num3, TileChangeType.None);
					}
				}
			}
			if ((double)j > (Main.worldSurface + Main.rockLayer) / 2.0 || Main.remixWorld)
			{
				if (type == 60 && WorldGen.genRand.Next(300) == 0)
				{
					int num6 = i + WorldGen.genRand.Next(-10, 11);
					int num7 = j + WorldGen.genRand.Next(-10, 11);
					if (WorldGen.InWorld(num6, num7, 2) && Main.tile[num6, num7].HasTile && Main.tile[num6, num7].TileType == 59 && (!Main.tile[num6, num7 - 1].HasTile || (Main.tile[num6, num7 - 1].TileType != 5 && Main.tile[num6, num7 - 1].TileType != 236 && Main.tile[num6, num7 - 1].TileType != 238)) && WorldGen.Chlorophyte(num6, num7))
					{
						Main.tile[num6, num7].TileType = TileID.Chlorophyte;
						WorldGen.SquareTileFrame(num6, num7, true);
						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendTileSquare(-1, num6, num7, TileChangeType.None);
						}
					}
				}
				if (type == 211 || type == 346)
				{
					int num8 = i;
					int num9 = j;
					if (WorldGen.genRand.Next(3) != 0)
					{
						int num16 = WorldGen.genRand.Next(4);
						if (num16 == 0)
						{
							num8++;
						}
						if (num16 == 1)
						{
							num8--;
						}
						if (num16 == 2)
						{
							num9++;
						}
						if (num16 == 3)
						{
							num9--;
						}
						if (WorldGen.InWorld(num8, num9, 2) && Main.tile[num8, num9].HasTile && (Main.tile[num8, num9].TileType == 59 || Main.tile[num8, num9].TileType == 60) && WorldGen.Chlorophyte(num8, num9))
						{
							Main.tile[num8, num9].TileType = 211;
							WorldGen.SquareTileFrame(num8, num9, true);
							if (Main.netMode == 2)
							{
								NetMessage.SendTileSquare(-1, num8, num9, TileChangeType.None);
							}
						}
					}
					bool flag = true;
					while (flag)
					{
						flag = false;
						num8 = i + Main.rand.Next(-6, 7);
						num9 = j + Main.rand.Next(-6, 7);
						if (WorldGen.InWorld(num8, num9, 2) && Main.tile[num8, num9].HasTile)
						{
							ushort num17 = Main.tile[num8, num9].TileType;
							bool convertTile = TileLoader.Convert(num8, num9, 9);
							if (num17 != Main.tile[num8, num9].TileType)
							{
								flag = true;
							}
							else if (convertTile)
							{
								if (Main.tile[num8, num9].TileType == 661 || Main.tile[num8, num9].TileType == 662 || Main.tile[num8, num9].TileType == TileID.CorruptGrass || Main.tile[num8, num9].TileType == TileID.CrimsonGrass || Main.tile[num8, num9].TileType == 2 || Main.tile[num8, num9].TileType == 477 || Main.tile[num8, num9].TileType == 492 || Main.tile[num8, num9].TileType == 109)
								{
									Main.tile[num8, num9].TileType = 60;
									WorldGen.SquareTileFrame(num8, num9, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num8, num9, TileChangeType.None);
									}
									flag = true;
								}
								else if (Main.tile[num8, num9].TileType == 0)
								{
									Main.tile[num8, num9].TileType = 59;
									WorldGen.SquareTileFrame(num8, num9, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num8, num9, TileChangeType.None);
									}
									flag = true;
								}
								else if (Main.tile[num8, num9].TileType == 25 || Main.tile[num8, num9].TileType == TileID.Crimstone)
								{
									Main.tile[num8, num9].TileType = TileID.Stone;
									WorldGen.SquareTileFrame(num8, num9, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num8, num9, TileChangeType.None);
									}
									flag = true;
								}
								else if (Main.tile[num8, num9].TileType == 112 || Main.tile[num8, num9].TileType == 234)
								{
									Main.tile[num8, num9].TileType = 53;
									WorldGen.SquareTileFrame(num8, num9, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num8, num9, TileChangeType.None);
									}
									flag = true;
								}
								else if (Main.tile[num8, num9].TileType == 398 || Main.tile[num8, num9].TileType == 399)
								{
									Main.tile[num8, num9].TileType = TileID.HardenedSand;
									WorldGen.SquareTileFrame(num8, num9, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num8, num9, TileChangeType.None);
									}
									flag = true;
								}
								else if (Main.tile[num8, num9].TileType == 400 || Main.tile[num8, num9].TileType == 401)
								{
									Main.tile[num8, num9].TileType = TileID.Sandstone;
									WorldGen.SquareTileFrame(num8, num9, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num8, num9, TileChangeType.None);
									}
									flag = true;
								}
								else if (Main.tile[num8, num9].TileType == 24 || Main.tile[num8, num9].TileType == 201 || Main.tile[num8, num9].TileType == 32 || Main.tile[num8, num9].TileType == 352 || Main.tile[num8, num9].TileType == 636 || Main.tile[num8, num9].TileType == 205)
								{
									WorldGen.KillTile(num8, num9, false, false, false);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num8, num9, TileChangeType.None);
									}
									flag = true;
								}
							}
						}
					}
				}
			}
			if ((NPC.downedPlantBoss && WorldGen.genRand.NextBool()) || !WorldGen.AllowedToSpreadInfections)
			{
				return;
			}
			if (type == TileID.CorruptGrass || type == TileID.Ebonstone || type == TileID.CorruptThorns || type == 112 || type == 163 || type == 400 || type == 398 || type == 636 || type == 661)
			{
				bool flag2 = true;
				while (flag2)
				{
					flag2 = false;
					int num10 = i + WorldGen.genRand.Next(-3, 4);
					int num11 = j + WorldGen.genRand.Next(-3, 4);
					if (WorldGen.InWorld(num10, num11, 10) && Main.tile[num10, num11].HasTile)
					{
						if (WorldGen.nearbyChlorophyte(num10, num11))
						{
							WorldGen.ChlorophyteDefense(num10, num11);
						}
						else if (CountNearBlocksTypes(num10, num11, 2, 1, [
							TileID.Sunflower,
							ModContent.TileType<Sunflower>(),
						]) <= 0)
						{
							ushort num18 = Main.tile[num10, num11].TileType;
							bool convertTile2 = TileLoader.Convert(num10, num11, 1);
							if (num18 != Main.tile[num10, num11].TileType)
							{
								if (WorldGen.genRand.Next(2) == 0)
								{
									flag2 = true;
								}
							}
							else if (convertTile2)
							{
								if (Main.tile[num10, num11].TileType == 2)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag2 = true;
									}
									Main.tile[num10, num11].TileType = TileID.CorruptGrass;
									WorldGen.SquareTileFrame(num10, num11, true);
									NetMessage.SendTileSquare(-1, num10, num11, TileChangeType.None);
								}
								else if (Main.tile[num10, num11].TileType == 1 || Main.tileMoss[(int)(Main.tile[num10, num11].TileType)])
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag2 = true;
									}
									Main.tile[num10, num11].TileType = 25;
									WorldGen.SquareTileFrame(num10, num11, true);
									NetMessage.SendTileSquare(-1, num10, num11, TileChangeType.None);
								}
								else if (Main.tile[num10, num11].TileType == 53)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag2 = true;
									}
									Main.tile[num10, num11].TileType = 112;
									WorldGen.SquareTileFrame(num10, num11, true);
									NetMessage.SendTileSquare(-1, num10, num11, TileChangeType.None);
								}
								else if (Main.tile[num10, num11].TileType == 396)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag2 = true;
									}
									Main.tile[num10, num11].TileType = 400;
									WorldGen.SquareTileFrame(num10, num11, true);
									NetMessage.SendTileSquare(-1, num10, num11, TileChangeType.None);
								}
								else if (Main.tile[num10, num11].TileType == 397)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag2 = true;
									}
									Main.tile[num10, num11].TileType = 398;
									WorldGen.SquareTileFrame(num10, num11, true);
									NetMessage.SendTileSquare(-1, num10, num11, TileChangeType.None);
								}
								else if (Main.tile[num10, num11].TileType == 60)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag2 = true;
									}
									Main.tile[num10, num11].TileType = 661;
									WorldGen.SquareTileFrame(num10, num11, true);
									NetMessage.SendTileSquare(-1, num10, num11, TileChangeType.None);
								}
								else if (Main.tile[num10, num11].TileType == 69)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag2 = true;
									}
									Main.tile[num10, num11].TileType = 32;
									WorldGen.SquareTileFrame(num10, num11, true);
									NetMessage.SendTileSquare(-1, num10, num11, TileChangeType.None);
								}
								else if (Main.tile[num10, num11].TileType == 161)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag2 = true;
									}
									Main.tile[num10, num11].TileType = 163;
									WorldGen.SquareTileFrame(num10, num11, true);
									NetMessage.SendTileSquare(-1, num10, num11, TileChangeType.None);
								}
							}
						}
					}
				}
			}
			if (type == TileID.CrimsonGrass || type == 200 || type == 201 || type == TileID.Crimstone || type == 205 || type == 234 || type == 352 || type == 401 || type == 399 || type == 662)
			{
				bool flag3 = true;
				while (flag3)
				{
					flag3 = false;
					int num12 = i + WorldGen.genRand.Next(-3, 4);
					int num13 = j + WorldGen.genRand.Next(-3, 4);
					if (WorldGen.InWorld(num12, num13, 10) && Main.tile[num12, num13].HasTile)
					{
						if (WorldGen.nearbyChlorophyte(num12, num13))
						{
							WorldGen.ChlorophyteDefense(num12, num13);
						}
						else if (CountNearBlocksTypes(num12, num13, 2, 1, [
							TileID.Sunflower,
							ModContent.TileType<Sunflower>(),
						]) <= 0)
						{
							ushort num19 = Main.tile[num12, num13].TileType;
							bool convertTile3 = TileLoader.Convert(num12, num13, 4);
							if (num19 != Main.tile[num12, num13].TileType)
							{
								if (WorldGen.genRand.Next(2) == 0)
								{
									flag3 = true;
								}
							}
							else if (convertTile3)
							{
								if (Main.tile[num12, num13].TileType == 2)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag3 = true;
									}
									Main.tile[num12, num13].TileType = TileID.CrimsonGrass;
									WorldGen.SquareTileFrame(num12, num13, true);
									NetMessage.SendTileSquare(-1, num12, num13, TileChangeType.None);
								}
								else if (Main.tile[num12, num13].TileType == 1 || Main.tileMoss[(int)(Main.tile[num12, num13].TileType)])
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag3 = true;
									}
									Main.tile[num12, num13].TileType = TileID.Crimstone;
									WorldGen.SquareTileFrame(num12, num13, true);
									NetMessage.SendTileSquare(-1, num12, num13, TileChangeType.None);
								}
								else if (Main.tile[num12, num13].TileType == 53)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag3 = true;
									}
									Main.tile[num12, num13].TileType = 234;
									WorldGen.SquareTileFrame(num12, num13, true);
									NetMessage.SendTileSquare(-1, num12, num13, TileChangeType.None);
								}
								else if (Main.tile[num12, num13].TileType == 396)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag3 = true;
									}
									Main.tile[num12, num13].TileType = 401;
									WorldGen.SquareTileFrame(num12, num13, true);
									NetMessage.SendTileSquare(-1, num12, num13, TileChangeType.None);
								}
								else if (Main.tile[num12, num13].TileType == 397)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag3 = true;
									}
									Main.tile[num12, num13].TileType = 399;
									WorldGen.SquareTileFrame(num12, num13, true);
									NetMessage.SendTileSquare(-1, num12, num13, TileChangeType.None);
								}
								else if (Main.tile[num12, num13].TileType == 60)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag3 = true;
									}
									Main.tile[num12, num13].TileType = 662;
									WorldGen.SquareTileFrame(num12, num13, true);
									NetMessage.SendTileSquare(-1, num12, num13, TileChangeType.None);
								}
								else if (Main.tile[num12, num13].TileType == 69)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag3 = true;
									}
									Main.tile[num12, num13].TileType = 352;
									WorldGen.SquareTileFrame(num12, num13, true);
									NetMessage.SendTileSquare(-1, num12, num13, TileChangeType.None);
								}
								else if (Main.tile[num12, num13].TileType == 161)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag3 = true;
									}
									Main.tile[num12, num13].TileType = 200;
									WorldGen.SquareTileFrame(num12, num13, true);
									NetMessage.SendTileSquare(-1, num12, num13, TileChangeType.None);
								}
							}
						}
					}
				}
			}
			if (type != 109 && type != 110 && type != 113 && type != 115 && type != 116 && type != 117 && type != 164 && type != 402 && type != 403 && type != 492)
			{
				return;
			}
			bool flag4 = true;
			while (flag4)
			{
				flag4 = false;
				int num14 = i + WorldGen.genRand.Next(-3, 4);
				int num15 = j + WorldGen.genRand.Next(-3, 4);
				if (WorldGen.InWorld(num14, num15, 10) && CountNearBlocksTypes(num14, num15, 2, 1, [
					TileID.Sunflower,
					ModContent.TileType<Sunflower>(),
				]) <= 0 && Main.tile[num14, num15].HasTile)
				{
					ushort num20 = Main.tile[num14, num15].TileType;
					bool convertTile4 = TileLoader.Convert(num14, num15, 2);
					if (num20 != Main.tile[num14, num15].TileType)
					{
						if (WorldGen.genRand.Next(2) == 0)
						{
							flag4 = true;
						}
					}
					else if (convertTile4)
					{
						if (Main.tile[num14, num15].TileType == 2)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag4 = true;
							}
							Main.tile[num14, num15].TileType = 109;
							WorldGen.SquareTileFrame(num14, num15, true);
							NetMessage.SendTileSquare(-1, num14, num15, TileChangeType.None);
						}
						else if (Main.tile[num14, num15].TileType == 477)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag4 = true;
							}
							Main.tile[num14, num15].TileType = 492;
							WorldGen.SquareTileFrame(num14, num15, true);
							NetMessage.SendTileSquare(-1, num14, num15, TileChangeType.None);
						}
						else if (Main.tile[num14, num15].TileType == 1 || Main.tileMoss[(int)(Main.tile[num14, num15].TileType)])
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag4 = true;
							}
							Main.tile[num14, num15].TileType = 117;
							WorldGen.SquareTileFrame(num14, num15, true);
							NetMessage.SendTileSquare(-1, num14, num15, TileChangeType.None);
						}
						else if (Main.tile[num14, num15].TileType == 53)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag4 = true;
							}
							Main.tile[num14, num15].TileType = 116;
							WorldGen.SquareTileFrame(num14, num15, true);
							NetMessage.SendTileSquare(-1, num14, num15, TileChangeType.None);
						}
						else if (Main.tile[num14, num15].TileType == 396)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag4 = true;
							}
							Main.tile[num14, num15].TileType = 403;
							WorldGen.SquareTileFrame(num14, num15, true);
							NetMessage.SendTileSquare(-1, num14, num15, TileChangeType.None);
						}
						else if (Main.tile[num14, num15].TileType == 397)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag4 = true;
							}
							Main.tile[num14, num15].TileType = 402;
							WorldGen.SquareTileFrame(num14, num15, true);
							NetMessage.SendTileSquare(-1, num14, num15, TileChangeType.None);
						}
						else if (Main.tile[num14, num15].TileType == 161)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag4 = true;
							}
							Main.tile[num14, num15].TileType = 164;
							WorldGen.SquareTileFrame(num14, num15, true);
							NetMessage.SendTileSquare(-1, num14, num15, TileChangeType.None);
						}
					}
				}
			}
		}
	}
}

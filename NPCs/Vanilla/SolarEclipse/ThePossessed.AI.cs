using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Succubus;

namespace V2.NPCs.Vanilla.SolarEclipse
{
	public partial class ThePossessed : GlobalNPC
	{
		public static bool V2ThePossessedAI(NPC npc)
		{
			npc.damage = npc.defDamage;
			if (Main.player[npc.target].position.Y + (float)Main.player[npc.target].height == npc.position.Y + (float)npc.height)
				npc.directionY = -1;

			if (PredNPC.GetCurrentBellyWeight(npc) > 0)
			{
				npc.ai[2] = 0f;
				npc.velocity.X *= 0.9f;
				npc.damage = (int)Math.Round((float)npc.defDamage / 2f);
				return false;
			}

			bool flag = false;
			bool flag5 = false;
			bool flag6 = false;
			if (npc.velocity.X == 0f)
				flag6 = true;

			if (npc.justHit)
				flag6 = false;

			int num56 = 60;
			bool flag7 = false;
			bool flag8 = false;

			if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
				flag7 = true;

			if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num56 || flag7)
				npc.ai[3] += 1f;
			else if ((double)Math.Abs(npc.velocity.X) > 0.9 && npc.ai[3] > 0f)
				npc.ai[3] -= 1f;

			if (npc.ai[3] > (float)(num56 * 10))
				npc.ai[3] = 0f;

			if (npc.justHit)
				npc.ai[3] = 0f;

			if (npc.ai[3] == (float)num56)
				npc.netUpdate = true;

			if (Main.player[npc.target].Hitbox.Intersects(npc.Hitbox))
				npc.ai[3] = 0f;

			npc.knockBackResist = 0.45f * Main.GameModeInfo.KnockbackToEnemiesMultiplier;
			if (npc.ai[2] == 1f)
				npc.knockBackResist = 0f;

			bool flag11 = false;
			int num75 = (int)npc.Center.X / 16;
			int num76 = (int)npc.Center.Y / 16;
			for (int num77 = num75 - 1; num77 <= num75 + 1; num77++)
			{
				for (int num78 = num76 - 1; num78 <= num76 + 1; num78++)
				{
					if (Main.tile[num77, num78].WallType > 0)
					{
						flag11 = true;
						break;
					}
				}

				if (flag11)
					break;
			}

			if (npc.ai[2] == 0f && flag11)
			{
				if (npc.velocity.Y == 0f)
				{
					flag = true;
					npc.velocity.Y = -4.6f;
					npc.velocity.X *= 1.3f;
				}
				else if (npc.velocity.Y > 0f && !Main.player[npc.target].dead)
				{
					npc.ai[2] = 1f;
				}
			}

			if (flag11 && npc.ai[2] == 1f && !Main.player[npc.target].dead && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
			{
				Vector2 vector23 = Main.player[npc.target].Center - npc.Center;
				float num79 = vector23.Length();
				vector23.Normalize();
				vector23 *= 4.5f + num79 / 300f;
				npc.velocity = (npc.velocity * 29f + vector23) / 30f;
				npc.noGravity = true;
				npc.ai[2] = 1f;
				return false;
			}

			npc.noGravity = false;
			npc.ai[2] = 0f;

			if (npc.ai[3] < (float)num56 && NPC.DespawnEncouragement_AIStyle3_Fighters_NotDiscouraged(npc.type, npc.position, npc))
			{
				npc.TargetClosest();
				if (npc.directionY > 0 && Main.player[npc.target].Center.Y <= npc.Bottom.Y)
					npc.directionY = -1;
			}
			else if (!(npc.ai[2] > 0f) || !NPC.DespawnEncouragement_AIStyle3_Fighters_CanBeBusyWithAction(npc.type))
			{
				if (Main.IsItDay() && (double)(npc.position.Y / 16f) < Main.worldSurface)
					npc.EncourageDespawn(10);

				if (npc.velocity.X == 0f)
				{
					if (npc.velocity.Y == 0f)
					{
						npc.ai[0] += 1f;
						if (npc.ai[0] >= 2f)
						{
							npc.direction *= -1;
							npc.spriteDirection = npc.direction;
							npc.ai[0] = 0f;
						}
					}
				}
				else
				{
					npc.ai[0] = 0f;
				}

				if (npc.direction == 0)
					npc.direction = 1;
			}

			float maxSpeed = 3.25f;
			if (Main.GameModeInfo.IsMasterMode)
				maxSpeed *= 1.30f;
			else if (Main.GameModeInfo.IsExpertMode)
				maxSpeed *= 1.15f;

			float accel = 0.075f;
			if (Main.GameModeInfo.IsMasterMode)
				accel *= 1.50f;
			else if (Main.GameModeInfo.IsExpertMode)
				accel *= 1.25f;

			float decel = 0.8f;
			if (Main.GameModeInfo.IsMasterMode)
				decel = 0.75f;
			else if (Main.GameModeInfo.IsExpertMode)
				decel = 0.7f;

			if (npc.velocity.X < 0f - maxSpeed || npc.velocity.X > maxSpeed)
			{
				if (npc.velocity.Y == 0f)
					npc.velocity *= decel;
			}
			else if (npc.velocity.X < maxSpeed && npc.direction == 1)
			{
				npc.velocity.X += accel;
				if (npc.velocity.X > maxSpeed)
					npc.velocity.X = maxSpeed;
			}
			else if (npc.velocity.X > 0f - maxSpeed && npc.direction == -1)
			{
				npc.velocity.X -= accel;
				if (npc.velocity.X < 0f - maxSpeed)
					npc.velocity.X = 0f - maxSpeed;
			}

			if (npc.velocity.Y == 0f || flag)
			{
				int num181 = (int)(npc.position.Y + (float)npc.height + 7f) / 16;
				int num182 = (int)(npc.position.Y - 9f) / 16;
				int num183 = (int)npc.position.X / 16;
				int num184 = (int)(npc.position.X + (float)npc.width) / 16;
				int num185 = (int)(npc.position.X + 8f) / 16;
				int num186 = (int)(npc.position.X + (float)npc.width - 8f) / 16;
				bool flag22 = false;
				for (int num187 = num185; num187 <= num186; num187++)
				{
					if (num187 >= num183 && num187 <= num184 && Main.tile[num187, num181] == null)
					{
						flag22 = true;
						continue;
					}

					if (Main.tile[num187, num182] != null && Main.tile[num187, num182].HasUnactuatedTile && Main.tileSolid[Main.tile[num187, num182].TileType])
					{
						flag5 = false;
						break;
					}

					if (!flag22 && num187 >= num183 && num187 <= num184 && Main.tile[num187, num181].HasUnactuatedTile && Main.tileSolid[Main.tile[num187, num181].TileType])
						flag5 = true;
				}

				if (!flag5 && npc.velocity.Y < 0f)
					npc.velocity.Y = 0f;

				if (flag22)
					return false;
			}

			if (npc.velocity.Y >= 0f)
			{
				int num188 = 0;
				if (npc.velocity.X < 0f)
					num188 = -1;

				if (npc.velocity.X > 0f)
					num188 = 1;

				Vector2 vector39 = npc.position;
				vector39.X += npc.velocity.X;
				int num189 = (int)((vector39.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num188)) / 16f);
				int num190 = (int)((vector39.Y + (float)npc.height - 1f) / 16f);
				if (WorldGen.InWorld(num189, num190, 4))
				{
					if ((float)(num189 * 16) < vector39.X + (float)npc.width && (float)(num189 * 16 + 16) > vector39.X && ((Main.tile[num189, num190].HasUnactuatedTile && !Main.tile[num189, num190].TopSlope && !Main.tile[num189, num190 - 1].TopSlope && Main.tileSolid[Main.tile[num189, num190].TileType] && !Main.tileSolidTop[Main.tile[num189, num190].TileType]) || (Main.tile[num189, num190 - 1].IsHalfBlock && Main.tile[num189, num190 - 1].HasUnactuatedTile)) && (!Main.tile[num189, num190 - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[num189, num190 - 1].TileType] || Main.tileSolidTop[Main.tile[num189, num190 - 1].TileType] || (Main.tile[num189, num190 - 1].IsHalfBlock && (!Main.tile[num189, num190 - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[num189, num190 - 4].TileType] || Main.tileSolidTop[Main.tile[num189, num190 - 4].TileType]))) && (!Main.tile[num189, num190 - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[num189, num190 - 2].TileType] || Main.tileSolidTop[Main.tile[num189, num190 - 2].TileType]) && (!Main.tile[num189, num190 - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[num189, num190 - 3].TileType] || Main.tileSolidTop[Main.tile[num189, num190 - 3].TileType]) && (!Main.tile[num189 - num188, num190 - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[num189 - num188, num190 - 3].TileType]))
					{
						float num191 = num190 * 16;
						if (Main.tile[num189, num190].IsHalfBlock)
							num191 += 8f;

						if (Main.tile[num189, num190 - 1].IsHalfBlock)
							num191 -= 8f;

						if (num191 < vector39.Y + (float)npc.height)
						{
							float num192 = vector39.Y + (float)npc.height - num191;
							float num193 = 16.1f;

							if (num192 <= num193)
							{
								npc.gfxOffY += npc.position.Y + (float)npc.height - num191;
								npc.position.Y = num191 - (float)npc.height;
								if (num192 < 9f)
									npc.stepSpeed = 1f;
								else
									npc.stepSpeed = 2f;
							}
						}
					}
				}
			}

			if (flag5)
			{
				int num194 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
				int num195 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);

				Tile tile = Main.tile[num194, num195 + 1];
				tile.IsHalfBlock = false;

				int num197 = npc.spriteDirection;

				if ((npc.velocity.X < 0f && num197 == -1) || (npc.velocity.X > 0f && num197 == 1))
				{
					if (npc.height >= 32 && Main.tile[num194, num195 - 2].HasUnactuatedTile && Main.tileSolid[Main.tile[num194, num195 - 2].TileType])
					{
						if (Main.tile[num194, num195 - 3].HasUnactuatedTile && Main.tileSolid[Main.tile[num194, num195 - 3].TileType])
						{
							npc.velocity.Y = -8f;
							npc.netUpdate = true;
						}
						else
						{
							npc.velocity.Y = -7f;
							npc.netUpdate = true;
						}
					}
					else if (Main.tile[num194, num195 - 1].HasUnactuatedTile && Main.tileSolid[Main.tile[num194, num195 - 1].TileType])
					{
						npc.velocity.Y = -6f;
						npc.netUpdate = true;
					}
					else if (npc.position.Y + (float)npc.height - (float)(num195 * 16) > 20f && Main.tile[num194, num195].HasUnactuatedTile && !Main.tile[num194, num195].TopSlope && Main.tileSolid[Main.tile[num194, num195].TileType])
					{
						npc.velocity.Y = -5f;
						npc.netUpdate = true;
					}
					else if (npc.directionY < 0 && (!Main.tile[num194, num195 + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[num194, num195 + 1].TileType]) && (!Main.tile[num194 + npc.direction, num195 + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[num194 + npc.direction, num195 + 1].TileType]))
					{
						npc.velocity.Y = -8f;
						npc.velocity.X *= 1.5f;
						if (npc.velocity.X > maxSpeed)
							npc.velocity.X = maxSpeed;
						if (npc.velocity.X < -maxSpeed)
							npc.velocity.X = -maxSpeed;
						npc.netUpdate = true;
					}

					if (npc.velocity.Y == 0f && flag6 && npc.ai[3] == 1f)
						npc.velocity.Y = -5f;

					if (npc.velocity.Y == 0f && Main.expertMode && Main.player[npc.target].Bottom.Y < npc.Top.Y && Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < (float)(Main.player[npc.target].width * 3) && Collision.CanHit(npc, Main.player[npc.target]))
					{
						if (npc.velocity.Y == 0f)
						{
							int num200 = 6;
							if (Main.player[npc.target].Bottom.Y > npc.Top.Y - (float)(num200 * 16))
							{
								npc.velocity.Y = -7.9f;
							}
							else
							{
								int num201 = (int)(npc.Center.X / 16f);
								int num202 = (int)(npc.Bottom.Y / 16f) - 1;
								for (int num203 = num202; num203 > num202 - num200; num203--)
								{
									if (Main.tile[num201, num203].HasUnactuatedTile && TileID.Sets.Platforms[Main.tile[num201, num203].TileType])
									{
										npc.velocity.Y = -7.9f;
										break;
									}
								}
							}
						}
					}
				}
			}
			return false;
		}
	}
}

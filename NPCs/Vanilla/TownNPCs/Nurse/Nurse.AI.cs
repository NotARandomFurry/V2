using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using static V2.NPCs.Vanilla.TownNPCs.TownNPCAIReference;

namespace V2.NPCs.Vanilla.TownNPCs.Nurse
{
	public partial class Nurse : GlobalNPC
	{
		public static bool V2NurseAI(NPC npc)
		{
			NPC.ShimmeredTownNPCs[NPCID.Nurse] = npc.IsShimmerVariant;
			int num = 300;

			bool tryToStayInHouse = Main.raining;
			if (!Main.dayTime)
				tryToStayInHouse = true;

			if (Main.eclipse)
				tryToStayInHouse = true;

			if (Main.slimeRain)
				tryToStayInHouse = true;

			float damageMult = 1f;
			if (Main.masterMode)
				npc.defense = npc.dryadWard ? (npc.defDefense + 14) : npc.defDefense;
			else if (Main.expertMode)
				npc.defense = npc.dryadWard ? (npc.defDefense + 10) : npc.defDefense;
			else
				npc.defense = npc.dryadWard ? (npc.defDefense + 6) : npc.defDefense;

			if (npc.isLikeATownNPC)
			{
				if (NPC.combatBookWasUsed)
				{
					damageMult += 0.2f;
					npc.defense += 6;
				}

				if (NPC.combatBookVolumeTwoWasUsed)
				{
					damageMult += 0.2f;
					npc.defense += 6;
				}

				if (NPC.downedBoss1)
				{
					damageMult += 0.1f;
					npc.defense += 3;
				}

				if (NPC.downedBoss2)
				{
					damageMult += 0.1f;
					npc.defense += 3;
				}

				if (NPC.downedBoss3)
				{
					damageMult += 0.1f;
					npc.defense += 3;
				}

				if (NPC.downedQueenBee)
				{
					damageMult += 0.1f;
					npc.defense += 3;
				}

				if (Main.hardMode)
				{
					damageMult += 0.4f;
					npc.defense += 12;
				}

				if (NPC.downedQueenSlime)
				{
					damageMult += 0.15f;
					npc.defense += 6;
				}

				if (NPC.downedMechBoss1)
				{
					damageMult += 0.15f;
					npc.defense += 6;
				}

				if (NPC.downedMechBoss2)
				{
					damageMult += 0.15f;
					npc.defense += 6;
				}

				if (NPC.downedMechBoss3)
				{
					damageMult += 0.15f;
					npc.defense += 6;
				}

				if (NPC.downedPlantBoss)
				{
					damageMult += 0.15f;
					npc.defense += 8;
				}

				if (NPC.downedEmpressOfLight)
				{
					damageMult += 0.15f;
					npc.defense += 8;
				}

				if (NPC.downedGolemBoss)
				{
					damageMult += 0.15f;
					npc.defense += 8;
				}

				if (NPC.downedAncientCultist)
				{
					damageMult += 0.15f;
					npc.defense += 8;
				}

				NPCLoader.BuffTownNPC(ref damageMult, ref npc.defense);
			}

			npc.dontTakeDamage = false;
			if (npc.ai[0] == 25f)
			{
				npc.dontTakeDamage = true;
				if (npc.ai[1] == 0f)
					npc.velocity.X = 0f;

				npc.shimmerWet = false;
				npc.wet = false;
				npc.lavaWet = false;
				npc.honeyWet = false;
				if (npc.ai[1] == 0f && Main.netMode == NetmodeID.MultiplayerClient)
					return false;

				if (npc.ai[1] == 0f && npc.ai[2] < 1f)
					AI_007_TownEntities_Shimmer_TeleportToLandingSpot(npc);

				if (npc.ai[2] > 0f)
				{
					npc.ai[2] -= 1f;
					if (npc.ai[2] <= 0f)
						npc.ai[1] = 1f;

					return false;
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 30f)
				{
					if (!Collision.WetCollision(npc.position, npc.width, npc.height))
						npc.shimmerTransparency = MathHelper.Clamp(npc.shimmerTransparency - 1f / 60f, 0f, 1f);
					else
						npc.ai[1] = 30f;

					npc.velocity = new Vector2(0f, -4f * npc.shimmerTransparency);
				}

				Rectangle hitbox = npc.Hitbox;
				hitbox.Y += 20;
				hitbox.Height -= 20;
				float num5 = Main.rand.NextFloatDirection();
				Lighting.AddLight(npc.Center, Main.hslToRgb((float)Main.timeForVisualEffects / 360f % 1f, 0.6f, 0.65f).ToVector3() * Utils.Remap(npc.ai[1], 30f, 90f, 0f, 0.7f));
				if (Main.rand.NextFloat() > Utils.Remap(npc.ai[1], 30f, 60f, 1f, 0.5f))
					Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(hitbox) + Main.rand.NextVector2Circular(8f, 0f) + new Vector2(0f, 4f), 309, new Vector2(0f, -2f).RotatedBy(num5 * ((float)Math.PI * 2f) * 0.11f), 0, default(Color), 1.7f - Math.Abs(num5) * 1.3f);

				if (npc.ai[1] > 60f && Main.rand.NextBool(15))
				{
					for (int i = 0; i < 3; i++)
					{
						Vector2 vector = Main.rand.NextVector2FromRectangle(npc.Hitbox);
						ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ShimmerBlock, new ParticleOrchestraSettings
						{
							PositionInWorld = vector,
							MovementVector = npc.DirectionTo(vector).RotatedBy((float)Math.PI * 9f / 20f * (float)(Main.rand.Next(2) * 2 - 1)) * Main.rand.NextFloat()
						});
					}
				}

				npc.TargetClosest();
				NPCAimedTarget targetData = npc.GetTargetData();
				if (npc.ai[1] >= 75f && npc.shimmerTransparency <= 0f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					Math.Sign(targetData.Center.X - npc.Center.X);
					npc.velocity = new Vector2(0f, -4f);
					npc.localAI[0] = 0f;
					npc.localAI[1] = 0f;
					npc.localAI[2] = 0f;
					npc.localAI[3] = 0f;
					npc.netUpdate = true;
					npc.townNpcVariationIndex = ((npc.townNpcVariationIndex != 1) ? 1 : 0);
					NetMessage.SendData(MessageID.UniqueTownNPCInfoSyncRequest, -1, -1, null, npc.whoAmI);
					npc.Teleport(npc.position, 12);
					ParticleOrchestrator.BroadcastParticleSpawn(ParticleOrchestraType.ShimmerTownNPC, new ParticleOrchestraSettings
					{
						PositionInWorld = npc.Center
					});
				}

				return false;
			}

			if (npc.homeTileX == -1 && npc.homeTileY == -1 && npc.velocity.Y == 0f && !npc.shimmering)
				npc.UpdateHomeTileState(npc.homeless, (int)npc.Center.X / 16, (int)(npc.position.Y + (float)npc.height + 4f) / 16);

			bool flag3 = false;
			int num6 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
			int num7 = (int)(npc.position.Y + (float)npc.height + 1f) / 16;
			AI_007_FindGoodRestingSpot(npc, num6, num7, out var floorX, out var floorY);

			npc.directionY = -1;
			if (npc.direction == 0)
				npc.direction = 1;

			if (npc.ai[0] != 24f)
			{
				for (int j = 0; j < 255; j++)
				{
					if (Main.player[j].active && Main.player[j].talkNPC == npc.whoAmI)
					{
						flag3 = true;
						if (npc.ai[0] != 0f)
							npc.netUpdate = true;

						npc.ai[0] = 0f;
						npc.ai[1] = 300f;
						npc.localAI[3] = 100f;
						if (Main.player[j].position.X + (float)(Main.player[j].width / 2) < npc.position.X + (float)(npc.width / 2))
							npc.direction = -1;
						else
							npc.direction = 1;
					}
				}
			}

			if (npc.ai[3] == 1f)
			{
				npc.life = -1;
				npc.HitEffect();
				npc.active = false;
				npc.netUpdate = true;

				return false;
			}

			if (!WorldGen.InWorld(num6, num7) || Main.netMode == NetmodeID.MultiplayerClient && !Main.sectionManager.TileLoaded(num6, num7))
				return false;

			if (!npc.homeless && Main.netMode != NetmodeID.MultiplayerClient && npc.townNPC && tryToStayInHouse && !AI_007_TownEntities_IsInAGoodRestingSpot(npc, num6, num7, floorX, floorY))
			{
				bool flag4 = true;
				for (int k = 0; k < 2; k++)
				{
					if (!flag4)
						break;

					Rectangle rectangle = new Rectangle((int)(npc.position.X + (float)(npc.width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(npc.position.Y + (float)(npc.height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
					if (k == 1)
						rectangle = new Rectangle(floorX * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, floorY * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);

					for (int l = 0; l < 255; l++)
					{
						if (Main.player[l].active && new Rectangle((int)Main.player[l].position.X, (int)Main.player[l].position.Y, Main.player[l].width, Main.player[l].height).Intersects(rectangle))
						{
							flag4 = false;
							break;
						}
					}
				}

				if (flag4)
					AI_007_TownEntities_TeleportToHome(npc, floorX, floorY);
			}

			float num8 = 200f;
			if (NPCID.Sets.DangerDetectRange[NPCID.Nurse] != -1)
				num8 = NPCID.Sets.DangerDetectRange[NPCID.Nurse];

			bool flag13 = false;
			bool flag14 = false;
			float currentPrimaryTargetDistance = -1f;
			float currentSecondaryTargetDistance = -1f;
			int currentTargetDirection = 0;
			int currentPrimaryTarget = -1;
			int currentSecondaryTarget = -1;
			if (Main.netMode != NetmodeID.MultiplayerClient && !flag3)
			{
				for (int npcIterationIndex = 0; npcIterationIndex < 200; npcIterationIndex++)
				{
					if (!Main.npc[npcIterationIndex].active || Main.npc[npcIterationIndex].friendly || Main.npc[npcIterationIndex].damage <= 0 || !(Main.npc[npcIterationIndex].Distance(npc.Center) < num8) || (!Main.npc[npcIterationIndex].noTileCollide && !Collision.CanHit(npc.Center, 0, 0, Main.npc[npcIterationIndex].Center, 0, 0)))
						continue;

					if (!NPCLoader.CanHitNPC(Main.npc[npcIterationIndex], npc))
						continue;

					bool canBeChasedByNurse = Main.npc[npcIterationIndex].CanBeChasedBy(npc);
					flag13 = true;
					float potentialTargetDistance = Main.npc[npcIterationIndex].Center.X - npc.Center.X;

					if (potentialTargetDistance < 0f && (currentPrimaryTargetDistance == -1f || potentialTargetDistance > currentPrimaryTargetDistance))
					{
						currentPrimaryTargetDistance = potentialTargetDistance;
						if (canBeChasedByNurse)
							currentPrimaryTarget = npcIterationIndex;
					}

					if (potentialTargetDistance > 0f && (currentSecondaryTargetDistance == -1f || potentialTargetDistance < currentSecondaryTargetDistance))
					{
						currentSecondaryTargetDistance = potentialTargetDistance;
						if (canBeChasedByNurse)
							currentSecondaryTarget = npcIterationIndex;
					}
				}

				if (flag13)
				{
					currentTargetDirection = ((currentPrimaryTargetDistance == -1f) ? 1 : ((currentSecondaryTargetDistance != -1f) ? (currentSecondaryTargetDistance < 0f - currentPrimaryTargetDistance).ToDirectionInt() : (-1)));
					float currentFinalizedTargetDistance = 0f;
					if (currentPrimaryTargetDistance != -1f)
						currentFinalizedTargetDistance = 0f - currentPrimaryTargetDistance;

					if (currentFinalizedTargetDistance == 0f || (currentSecondaryTargetDistance < currentFinalizedTargetDistance && currentSecondaryTargetDistance > 0f))
						currentFinalizedTargetDistance = currentSecondaryTargetDistance;

					if (npc.ai[0] == 8f)
					{
						if (npc.direction == -currentTargetDirection)
						{
							npc.ai[0] = 1f;
							npc.ai[1] = 300 + Main.rand.Next(300);
							npc.ai[2] = 0f;
							npc.localAI[3] = 0f;
							npc.netUpdate = true;
						}
					}
					else if (npc.ai[0] != 10f && npc.ai[0] != 12f && npc.ai[0] != 13f && npc.ai[0] != 14f && npc.ai[0] != 15f)
					{
						if (NPCID.Sets.PrettySafe[NPCID.Nurse] != -1 && (float)NPCID.Sets.PrettySafe[NPCID.Nurse] < currentFinalizedTargetDistance)
						{
							flag13 = false;
							flag14 = true;
						}
						else if (npc.ai[0] != 1f)
						{
							int tileX = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
							int tileY = (int)((npc.position.Y + (float)npc.height - 16f) / 16f);
							bool currentlyDrowning = npc.wet;
							AI_007_TownEntities_GetWalkPrediction(npc, num6, floorX, false, currentlyDrowning, tileX, tileY, out var _, out var avoidFalling);
							if (!avoidFalling)
							{
								if (npc.ai[0] == 3f || npc.ai[0] == 4f || npc.ai[0] == 16f || npc.ai[0] == 17f)
								{
									NPC nPC = Main.npc[(int)npc.ai[2]];
									if (nPC.active)
									{
										nPC.ai[0] = 1f;
										nPC.ai[1] = 120 + Main.rand.Next(120);
										nPC.ai[2] = 0f;
										nPC.localAI[3] = 0f;
										nPC.direction = -currentTargetDirection;
										nPC.netUpdate = true;
									}
								}

								npc.ai[0] = 1f;
								npc.ai[1] = 120 + Main.rand.Next(120);
								npc.ai[2] = 0f;
								npc.localAI[3] = 0f;
								npc.direction = -currentTargetDirection;
								npc.netUpdate = true;
							}
						}
						else if (npc.ai[0] == 1f && npc.direction != -currentTargetDirection)
						{
							npc.direction = -currentTargetDirection;
							npc.netUpdate = true;
						}
					}
				}
			}

			if (npc.ai[0] == 0f)
			{
				if (npc.localAI[3] > 0f)
					npc.localAI[3] -= 1f;

				if (tryToStayInHouse && !flag3)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (num6 == floorX && num7 == floorY)
						{
							if (npc.velocity.X != 0f)
								npc.netUpdate = true;

							if (npc.velocity.X > 0.1f)
							{
								npc.velocity.X -= 0.1f;
							}
							else if (npc.velocity.X < -0.1f)
							{
								npc.velocity.X += 0.1f;
							}
							else
							{
								npc.velocity.X = 0f;
								AI_007_TryForcingSitting(npc, floorX, floorY);
							}
						}
						else
						{
							if (num6 > floorX)
								npc.direction = -1;
							else
								npc.direction = 1;

							npc.ai[0] = 1f;
							npc.ai[1] = 200 + Main.rand.Next(200);
							npc.ai[2] = 0f;
							npc.localAI[3] = 0f;
							npc.netUpdate = true;
						}
					}
				}
				else
				{
					if (npc.velocity.X > 0.1f)
						npc.velocity.X -= 0.1f;
					else if (npc.velocity.X < -0.1f)
						npc.velocity.X += 0.1f;
					else
						npc.velocity.X = 0f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (npc.ai[1] > 0f)
							npc.ai[1] -= 1f;

						bool flag16 = true;
						int tileX2 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
						int tileY2 = (int)((npc.position.Y + (float)npc.height - 16f) / 16f);
						bool currentlyDrowning2 = npc.wet;
						AI_007_TownEntities_GetWalkPrediction(npc, num6, floorX, false, currentlyDrowning2, tileX2, tileY2, out var _, out var avoidFalling2);
						if (npc.wet)
						{
							bool currentlyDrowning3 = Collision.DrownCollision(npc.position, npc.width, npc.height, 1f, includeSlopes: true);
							if (currentlyDrowning3)
							{
								npc.ai[0] = 1f;
								npc.ai[1] = 200 + Main.rand.Next(300);
								npc.ai[2] = 0f;

								npc.localAI[3] = 0f;
								npc.netUpdate = true;
							}
						}

						if (avoidFalling2)
							flag16 = false;

						if (npc.ai[1] <= 0f)
						{
							if (flag16 && !avoidFalling2)
							{
								npc.ai[0] = 1f;
								npc.ai[1] = 200 + Main.rand.Next(300);
								npc.ai[2] = 0f;

								npc.localAI[3] = 0f;
								npc.netUpdate = true;
							}
							else
							{
								npc.direction *= -1;
								npc.ai[1] = 60 + Main.rand.Next(120);
								npc.netUpdate = true;
							}
						}
					}
				}

				if (Main.netMode != NetmodeID.MultiplayerClient && (!tryToStayInHouse || AI_007_TownEntities_IsInAGoodRestingSpot(npc, num6, num7, floorX, floorY)))
				{
					if (num6 < floorX - 25 || num6 > floorX + 25)
					{
						if (npc.localAI[3] == 0f)
						{
							if (num6 < floorX - 50 && npc.direction == -1)
							{
								npc.direction = 1;
								npc.netUpdate = true;
							}
							else if (num6 > floorX + 50 && npc.direction == 1)
							{
								npc.direction = -1;
								npc.netUpdate = true;
							}
						}
					}
					else if (Main.rand.NextBool(80) && npc.localAI[3] == 0f)
					{
						npc.localAI[3] = 200f;
						npc.direction *= -1;
						npc.netUpdate = true;
					}
				}
			}
			else if (npc.ai[0] == 1f)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient && tryToStayInHouse && AI_007_TownEntities_IsInAGoodRestingSpot(npc, num6, num7, floorX, floorY))
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 200 + Main.rand.Next(200);
					npc.localAI[3] = 60f;
					npc.netUpdate = true;
				}
				else
				{
					bool flag17 = Collision.DrownCollision(npc.position, npc.width, npc.height, 1f, includeSlopes: true);
					if (!flag17)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient && !npc.homeless && !Main.tileDungeon[Main.tile[num6, num7].TileType] && (num6 < floorX - 35 || num6 > floorX + 35))
						{
							if (npc.position.X < (float)(floorX * 16) && npc.direction == -1)
								npc.ai[1] -= 5f;
							else if (npc.position.X > (float)(floorX * 16) && npc.direction == 1)
								npc.ai[1] -= 5f;
						}

						npc.ai[1] -= 1f;
					}

					if (npc.ai[1] <= 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 300 + Main.rand.Next(300);
						npc.ai[2] = 0f;
						npc.ai[1] += Main.rand.Next(900);

						npc.localAI[3] = 60f;
						npc.netUpdate = true;
					}

					if (npc.closeDoor && ((npc.position.X + (float)(npc.width / 2)) / 16f > (float)(npc.doorX + 2) || (npc.position.X + (float)(npc.width / 2)) / 16f < (float)(npc.doorX - 2)))
					{
						Tile tileSafely = Framing.GetTileSafely(npc.doorX, npc.doorY);

						if (TileLoader.CloseDoorID(tileSafely) >= 0)
						{
							if (WorldGen.CloseDoor(npc.doorX, npc.doorY))
							{
								npc.closeDoor = false;
								NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 1, npc.doorX, npc.doorY, npc.direction);
							}

							if ((npc.position.X + (float)(npc.width / 2)) / 16f > (float)(npc.doorX + 4) || (npc.position.X + (float)(npc.width / 2)) / 16f < (float)(npc.doorX - 4) || (npc.position.Y + (float)(npc.height / 2)) / 16f > (float)(npc.doorY + 4) || (npc.position.Y + (float)(npc.height / 2)) / 16f < (float)(npc.doorY - 4))
								npc.closeDoor = false;
						}
						else if (tileSafely.TileType == 389)
						{
							if (WorldGen.ShiftTallGate(npc.doorX, npc.doorY, closing: true))
							{
								npc.closeDoor = false;
								NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 5, npc.doorX, npc.doorY);
							}

							if ((npc.position.X + (float)(npc.width / 2)) / 16f > (float)(npc.doorX + 4) || (npc.position.X + (float)(npc.width / 2)) / 16f < (float)(npc.doorX - 4) || (npc.position.Y + (float)(npc.height / 2)) / 16f > (float)(npc.doorY + 4) || (npc.position.Y + (float)(npc.height / 2)) / 16f < (float)(npc.doorY - 4))
								npc.closeDoor = false;
						}
						else
						{
							npc.closeDoor = false;
						}
					}

					float num17 = 1f;
					float num18 = 0.07f;

					if (npc.friendly && (flag13 || flag17))
					{
						num17 = 1.5f;
						float num19 = 1f - (float)npc.life / (float)npc.lifeMax;
						num17 += num19 * 0.9f;
						num18 = 0.1f;
					}

					if (npc.velocity.X < 0f - num17 || npc.velocity.X > num17)
					{
						if (npc.velocity.Y == 0f)
							npc.velocity *= 0.8f;
					}
					else if (npc.velocity.X < num17 && npc.direction == 1)
					{
						npc.velocity.X += num18;
						if (npc.velocity.X > num17)
							npc.velocity.X = num17;
					}
					else if (npc.velocity.X > 0f - num17 && npc.direction == -1)
					{
						npc.velocity.X -= num18;
						if (npc.velocity.X > num17)
							npc.velocity.X = num17;
					}

					bool flag18 = true;
					if ((float)(npc.homeTileY * 16 - 32) > npc.position.Y)
						flag18 = false;

					if (!flag18 && npc.velocity.Y == 0f)
						Collision.StepDown(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);

					if (npc.velocity.Y >= 0f)
						Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY, 1, flag18, 1);

					if (npc.velocity.Y == 0f)
					{
						int num20 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
						int num21 = (int)((npc.position.Y + (float)npc.height - 16f) / 16f);
						int num22 = 180;
						AI_007_TownEntities_GetWalkPrediction(npc, num6, floorX, false, flag17, num20, num21, out var keepwalking3, out var avoidFalling3);
						bool flag19 = false;
						bool flag20 = false;
						if (npc.wet && npc.townNPC && (flag20 = flag17) && npc.localAI[3] <= 0f)
						{
							avoidFalling3 = true;
							npc.localAI[3] = num22;
							int num23 = 0;
							for (int n = 0; n <= 10 && Framing.GetTileSafely(num20 - npc.direction, num21 - n).LiquidAmount != 0; n++)
							{
								num23++;
							}

							float num24 = 0.3f;
							float num25 = (float)Math.Sqrt((float)(num23 * 16 + 16) * 2f * num24);
							if (num25 > 26f)
								num25 = 26f;

							npc.velocity.Y = 0f - num25;
							npc.localAI[3] = npc.position.X;
							flag19 = true;
						}

						if (avoidFalling3 && !flag19)
						{
							int num26 = (int)((npc.position.X + (float)(npc.width / 2)) / 16f);
							int num27 = 0;
							for (int num28 = -1; num28 <= 1; num28++)
							{
								Tile tileSafely2 = Framing.GetTileSafely(num26 + num28, num21 + 1);
								if (tileSafely2.HasUnactuatedTile && Main.tileSolid[tileSafely2.TileType])
									num27++;
							}

							if (num27 <= 2)
							{
								if (npc.velocity.X != 0f)
									npc.netUpdate = true;

								keepwalking3 = (avoidFalling3 = false);
								npc.ai[0] = 0f;
								npc.ai[1] = 50 + Main.rand.Next(50);
								npc.ai[2] = 0f;
								npc.localAI[3] = 40f;
							}
						}

						if (npc.position.X == npc.localAI[3] && !flag19)
						{
							npc.direction *= -1;
							npc.netUpdate = true;
							npc.localAI[3] = num22;
						}

						if (flag17 && !flag19)
						{
							if (npc.localAI[3] > (float)num22)
								npc.localAI[3] = num22;

							if (npc.localAI[3] > 0f)
								npc.localAI[3] -= 1f;
						}
						else
						{
							npc.localAI[3] = -1f;
						}

						Tile tileSafely3 = Framing.GetTileSafely(num20, num21);
						Tile tileSafely4 = Framing.GetTileSafely(num20, num21 - 1);
						Tile tileSafely5 = Framing.GetTileSafely(num20, num21 - 2);
						bool flag21 = npc.height / 16 < 3;

						if ((npc.townNPC || NPCID.Sets.AllowDoorInteraction[NPCID.Nurse]) && tileSafely5.HasUnactuatedTile && (TileLoader.IsClosedDoor(tileSafely5) || tileSafely5.TileType == 388) && (Main.rand.NextBool(10) || tryToStayInHouse))
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								if (WorldGen.OpenDoor(num20, num21 - 2, npc.direction))
								{
									npc.closeDoor = true;
									npc.doorX = num20;
									npc.doorY = num21 - 2;
									NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, num20, num21 - 2, npc.direction);
									npc.netUpdate = true;
									npc.ai[1] += 80f;
								}
								else if (WorldGen.OpenDoor(num20, num21 - 2, -npc.direction))
								{
									npc.closeDoor = true;
									npc.doorX = num20;
									npc.doorY = num21 - 2;
									NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, num20, num21 - 2, -npc.direction);
									npc.netUpdate = true;
									npc.ai[1] += 80f;
								}
								else if (WorldGen.ShiftTallGate(num20, num21 - 2, closing: false))
								{
									npc.closeDoor = true;
									npc.doorX = num20;
									npc.doorY = num21 - 2;
									NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, num20, num21 - 2);
									npc.netUpdate = true;
									npc.ai[1] += 80f;
								}
								else
								{
									npc.direction *= -1;
									npc.netUpdate = true;
								}
							}
						}
						else
						{
							if ((npc.velocity.X < 0f && npc.direction == -1) || (npc.velocity.X > 0f && npc.direction == 1))
							{
								bool flag22 = false;
								bool flag23 = false;
								if (tileSafely5.HasUnactuatedTile && Main.tileSolid[tileSafely5.TileType] && !Main.tileSolidTop[tileSafely5.TileType] && (!flag21 || (tileSafely4.HasUnactuatedTile && Main.tileSolid[tileSafely4.TileType] && !Main.tileSolidTop[tileSafely4.TileType])))
								{
									if (!Collision.SolidTilesVersatile(num20 - npc.direction * 2, num20 - npc.direction, num21 - 5, num21 - 1) && !Collision.SolidTiles(num20, num20, num21 - 5, num21 - 3))
									{
										npc.velocity.Y = -6f;
										npc.netUpdate = true;
									}
									else if (flag13)
									{
										flag23 = true;
										flag22 = true;
									}
									else if (!flag20)
									{
										flag22 = true;
									}
								}
								else if (tileSafely4.HasUnactuatedTile && Main.tileSolid[tileSafely4.TileType] && !Main.tileSolidTop[tileSafely4.TileType])
								{
									if (!Collision.SolidTilesVersatile(num20 - npc.direction * 2, num20 - npc.direction, num21 - 4, num21 - 1) && !Collision.SolidTiles(num20, num20, num21 - 4, num21 - 2))
									{
										npc.velocity.Y = -5f;
										npc.netUpdate = true;
									}
									else if (flag13)
									{
										flag23 = true;
										flag22 = true;
									}
									else
									{
										flag22 = true;
									}
								}
								else if (npc.position.Y + (float)npc.height - (float)(num21 * 16) > 20f && tileSafely3.HasUnactuatedTile && Main.tileSolid[tileSafely3.TileType] && !tileSafely3.TopSlope)
								{
									if (!Collision.SolidTilesVersatile(num20 - npc.direction * 2, num20, num21 - 3, num21 - 1))
									{
										npc.velocity.Y = -4.4f;
										npc.netUpdate = true;
									}
									else if (flag13)
									{
										flag23 = true;
										flag22 = true;
									}
									else
									{
										flag22 = true;
									}
								}
								else if (avoidFalling3)
								{
									if (!flag20)
										flag22 = true;

									if (flag13)
										flag23 = true;
								}

								if (flag23)
								{
									keepwalking3 = false;
									npc.velocity.X = 0f;
									npc.ai[0] = 8f;
									npc.ai[1] = 240f;
									npc.netUpdate = true;
								}

								if (flag22)
								{
									npc.direction *= -1;
									npc.velocity.X *= -1f;
									npc.netUpdate = true;
								}

								if (keepwalking3)
								{
									npc.ai[1] = 90f;
									npc.netUpdate = true;
								}

								if (npc.velocity.Y < 0f)
									npc.localAI[3] = npc.position.X;
							}

							if (npc.velocity.Y < 0f && npc.wet)
								npc.velocity.Y *= 1.2f;
						}
					}
				}
			}
			else if (npc.ai[0] == 2f || npc.ai[0] == 11f)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[3] -= 1f;
					if (Main.rand.NextBool(60) && npc.localAI[3] == 0f)
					{
						npc.localAI[3] = 60f;
						npc.direction *= -1;
						npc.netUpdate = true;
					}
				}

				npc.ai[1] -= 1f;
				npc.velocity.X *= 0.8f;
				if (npc.ai[1] <= 0f)
				{
					npc.localAI[3] = 40f;
					npc.ai[0] = 0f;
					npc.ai[1] = 60 + Main.rand.Next(60);
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 3f || npc.ai[0] == 4f || npc.ai[0] == 5f || npc.ai[0] == 8f || npc.ai[0] == 9f || npc.ai[0] == 16f || npc.ai[0] == 17f || npc.ai[0] == 20f || npc.ai[0] == 21f || npc.ai[0] == 22f || npc.ai[0] == 23f)
			{
				npc.velocity.X *= 0.8f;
				npc.ai[1] -= 1f;
				if (npc.ai[0] == 8f && npc.ai[1] < 60f && flag13)
				{
					npc.ai[1] = 180f;
					npc.netUpdate = true;
				}

				if (npc.ai[0] == 5f)
				{
					Point coords = (npc.Bottom + Vector2.UnitY * -2f).ToTileCoordinates();
					Tile tile = Main.tile[coords.X, coords.Y];

					if (!TileID.Sets.CanBeSatOnForNPCs[tile.TileType])
						npc.ai[1] = 0f;
					else
						Main.sittingManager.AddNPC(npc.whoAmI, coords);
				}

				if (npc.ai[1] <= 0f)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 60 + Main.rand.Next(60);
					npc.ai[2] = 0f;
					npc.localAI[3] = 30 + Main.rand.Next(60);
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 6f || npc.ai[0] == 7f || npc.ai[0] == 18f || npc.ai[0] == 19f)
			{
				if (npc.ai[0] == 18f && (npc.localAI[3] < 1f || npc.localAI[3] > 2f))
					npc.localAI[3] = 2f;

				npc.velocity.X *= 0.8f;
				npc.ai[1] -= 1f;
				int num34 = (int)npc.ai[2];
				if (num34 < 0 || num34 > 255 || !Main.player[num34].CanBeTalkedTo || Main.player[num34].Distance(npc.Center) > 200f || !Collision.CanHitLine(npc.Top, 0, 0, Main.player[num34].Top, 0, 0))
					npc.ai[1] = 0f;

				if (npc.ai[1] > 0f)
				{
					int num35 = ((npc.Center.X < Main.player[num34].Center.X) ? 1 : (-1));
					if (num35 != npc.direction)
						npc.netUpdate = true;

					npc.direction = num35;
				}
				else
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 60 + Main.rand.Next(60);
					npc.ai[2] = 0f;
					npc.localAI[3] = 30 + Main.rand.Next(60);
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 10f)
			{
				int attackProjectileType = 0;
				int attackBaseDamage = 0;
				float attackKnockback = 0f;
				float attackProjectileSpeedMult = 0f;
				int attackProjectileDelay = 0;
				int attackCooldown = 0;
				int attackRandomExtraCooldown = 0;
				float attackProjectileGravityCorrection = 0f;
				float num42 = NPCID.Sets.DangerDetectRange[NPCID.Nurse];
				float attackProjectileRandomOffset = 0f;
				if ((float)NPCID.Sets.AttackTime[NPCID.Nurse] == npc.ai[1])
				{
					npc.frameCounter = 0.0;
					npc.localAI[3] = 0f;
				}

					attackProjectileType = 583;
					attackProjectileSpeedMult = 8f;
					attackBaseDamage = 8;
					attackProjectileDelay = 1;
					attackCooldown = 15;
					attackRandomExtraCooldown = 10;
					attackKnockback = 2f;
					attackProjectileGravityCorrection = 10f;

				NPCLoader.TownNPCAttackStrength(npc, ref attackBaseDamage, ref attackKnockback);
				NPCLoader.TownNPCAttackCooldown(npc, ref attackCooldown, ref attackRandomExtraCooldown);
				NPCLoader.TownNPCAttackProj(npc, ref attackProjectileType, ref attackProjectileDelay);
				NPCLoader.TownNPCAttackProjSpeed(npc, ref attackProjectileSpeedMult, ref attackProjectileGravityCorrection, ref attackProjectileRandomOffset);

				if (Main.expertMode)
					attackBaseDamage = (int)((float)attackBaseDamage * Main.GameModeInfo.TownNPCDamageMultiplier);

				attackBaseDamage = (int)((float)attackBaseDamage * damageMult);
				npc.velocity.X *= 0.8f;
				npc.ai[1] -= 1f;
				npc.localAI[3] += 1f;
				if (npc.localAI[3] == (float)attackProjectileDelay && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 attackProjectileSpeed = -Vector2.UnitY;
					if (currentTargetDirection == 1 && npc.spriteDirection == 1 && currentSecondaryTarget != -1)
						attackProjectileSpeed = npc.DirectionTo(Main.npc[currentSecondaryTarget].Center + new Vector2(0f, (0f - attackProjectileGravityCorrection) * MathHelper.Clamp(npc.Distance(Main.npc[currentSecondaryTarget].Center) / num42, 0f, 1f)));

					if (currentTargetDirection == -1 && npc.spriteDirection == -1 && currentPrimaryTarget != -1)
						attackProjectileSpeed = npc.DirectionTo(Main.npc[currentPrimaryTarget].Center + new Vector2(0f, (0f - attackProjectileGravityCorrection) * MathHelper.Clamp(npc.Distance(Main.npc[currentPrimaryTarget].Center) / num42, 0f, 1f)));

					if (attackProjectileSpeed.HasNaNs() || Math.Sign(attackProjectileSpeed.X) != npc.spriteDirection)
						attackProjectileSpeed = new Vector2(npc.spriteDirection, -1f);

					attackProjectileSpeed *= attackProjectileSpeedMult;
					attackProjectileSpeed += Utils.RandomVector2(Main.rand, 0f - attackProjectileRandomOffset, attackProjectileRandomOffset);
					int num44 = 1000;
					num44 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, attackProjectileSpeed.X, attackProjectileSpeed.Y, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer);
					Main.projectile[num44].npcProj = true;
					Main.projectile[num44].noDropItem = true;
				}

				if (npc.ai[1] <= 0f && 0 == 0)
				{
					npc.ai[0] = ((npc.localAI[2] == 8f && flag13) ? 8 : 0);
					npc.ai[1] = attackCooldown + Main.rand.Next(attackRandomExtraCooldown);
					npc.ai[2] = 0f;
					npc.localAI[1] = (npc.localAI[3] = attackCooldown / 2 + Main.rand.Next(attackRandomExtraCooldown));
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 13f)
			{
				npc.velocity.X *= 0.8f;
				if ((float)NPCID.Sets.AttackTime[NPCID.Nurse] == npc.ai[1])
					npc.frameCounter = 0.0;

				npc.ai[1] -= 1f;
				npc.localAI[3] += 1f;
				if (npc.localAI[3] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 vec3 = npc.DirectionTo(Main.npc[(int)npc.ai[2]].Center + new Vector2(0f, -20f));
					if (vec3.HasNaNs() || Math.Sign(vec3.X) == -npc.spriteDirection)
						vec3 = new Vector2(npc.spriteDirection, -1f);

					vec3 *= 8f;
					int num54 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, vec3.X, vec3.Y, 584, 0, 0f, Main.myPlayer, npc.ai[2]);
					Main.projectile[num54].npcProj = true;
					Main.projectile[num54].noDropItem = true;
				}

				if (npc.ai[1] <= 0f)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 10 + Main.rand.Next(10);
					npc.ai[2] = 0f;
					npc.localAI[3] = 5 + Main.rand.Next(10);
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 24f)
			{
				npc.velocity.X *= 0.8f;
				npc.ai[1] -= 1f;
				npc.localAI[3] += 1f;
				npc.direction = 1;
				npc.spriteDirection = 1;
				Vector3 vector7 = npc.GetMagicAuraColor().ToVector3();
				Lighting.AddLight(npc.Center, vector7.X, vector7.Y, vector7.Z);
				if (npc.ai[1] <= 0f)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 480f;
					npc.ai[2] = 0f;
					npc.localAI[1] = 480f;
					npc.netUpdate = true;
				}
			}

			if (Main.netMode != NetmodeID.MultiplayerClient && npc.isLikeATownNPC && !flag3)
			{
				bool flag26 = npc.ai[0] < 2f && !flag13 && !npc.wet;
				bool flag27 = (npc.ai[0] < 2f || npc.ai[0] == 8f) && (flag13 || flag14);
				if (npc.localAI[1] > 0f)
					npc.localAI[1] -= 1f;

				if (npc.localAI[1] > 0f)
					flag27 = false;

				if (npc.CanTalk && flag26 && npc.ai[0] == 0f && npc.velocity.Y == 0f && Main.rand.NextBool(300))
				{
					int num90 = 420;
					num90 = (!Main.rand.NextBool(2)) ? (num90 * Main.rand.Next(1, 3)) : (num90 * Main.rand.Next(1, 4));
					int num91 = 100;
					int num92 = 20;
					for (int num93 = 0; num93 < 200; num93++)
					{
						NPC nPC4 = Main.npc[num93];
						bool flag28 = (nPC4.ai[0] == 1f && nPC4.closeDoor) || (nPC4.ai[0] == 1f && nPC4.ai[1] > 200f) || nPC4.ai[0] > 1f || nPC4.wet;
						if (nPC4 != npc && nPC4.active && nPC4.CanBeTalkedTo && !flag28 && nPC4.Distance(npc.Center) < (float)num91 && nPC4.Distance(npc.Center) > (float)num92 && Collision.CanHit(npc.Center, 0, 0, nPC4.Center, 0, 0))
						{
							int num94 = (npc.position.X < nPC4.position.X).ToDirectionInt();
							npc.ai[0] = 3f;
							npc.ai[1] = num90;
							npc.ai[2] = num93;
							npc.direction = num94;
							npc.netUpdate = true;
							nPC4.ai[0] = 4f;
							nPC4.ai[1] = num90;
							nPC4.ai[2] = npc.whoAmI;
							nPC4.direction = -num94;
							nPC4.netUpdate = true;
							break;
						}
					}
				}
				else if (npc.CanTalk && flag26 && npc.ai[0] == 0f && npc.velocity.Y == 0f && Main.rand.NextBool(1800))
				{
					int num95 = 420;
					num95 = ((!Main.rand.NextBool(2)) ? (num95 * Main.rand.Next(1, 3)) : (num95 * Main.rand.Next(1, 4)));
					int num96 = 100;
					int num97 = 20;
					for (int num98 = 0; num98 < 200; num98++)
					{
						NPC nPC5 = Main.npc[num98];
						bool flag29 = (nPC5.ai[0] == 1f && nPC5.closeDoor) || (nPC5.ai[0] == 1f && nPC5.ai[1] > 200f) || nPC5.ai[0] > 1f || nPC5.wet;
						if (nPC5 != npc && nPC5.active && nPC5.CanBeTalkedTo && !NPCID.Sets.IsTownPet[nPC5.type] && !flag29 && nPC5.Distance(npc.Center) < (float)num96 && nPC5.Distance(npc.Center) > (float)num97 && Collision.CanHit(npc.Center, 0, 0, nPC5.Center, 0, 0))
						{
							int num99 = (npc.position.X < nPC5.position.X).ToDirectionInt();
							npc.ai[0] = 16f;
							npc.ai[1] = num95;
							npc.ai[2] = num98;
							npc.localAI[2] = Main.rand.Next(4);
							npc.localAI[3] = Main.rand.Next(3 - (int)npc.localAI[2]);
							npc.direction = num99;
							npc.netUpdate = true;
							nPC5.ai[0] = 17f;
							nPC5.ai[1] = num95;
							nPC5.ai[2] = npc.whoAmI;
							nPC5.localAI[2] = 0f;
							nPC5.localAI[3] = 0f;
							nPC5.direction = -num99;
							nPC5.netUpdate = true;
							break;
						}
					}
				}
				else if (flag26 && npc.ai[0] == 0f && npc.velocity.Y == 0f && Main.rand.NextBool(1200) && (BirthdayParty.PartyIsUp && NPCID.Sets.AttackType[NPCID.Nurse] == NPCID.Sets.AttackType[208]))
				{
					int num100 = 300;
					int num101 = 150;
					for (int num102 = 0; num102 < 255; num102++)
					{
						Player player = Main.player[num102];
						if (player.active && !player.dead && player.Distance(npc.Center) < (float)num101 && Collision.CanHitLine(npc.Top, 0, 0, player.Top, 0, 0))
						{
							int num103 = (npc.position.X < player.position.X).ToDirectionInt();
							npc.ai[0] = 6f;
							npc.ai[1] = num100;
							npc.ai[2] = num102;
							npc.direction = num103;
							npc.netUpdate = true;
							break;
						}
					}
				}
				else if (flag26 && npc.ai[0] == 0f && npc.velocity.Y == 0f && Main.rand.NextBool(1800))
				{
					npc.ai[0] = 2f;
					npc.ai[1] = 45 * Main.rand.Next(1, 2);
					npc.netUpdate = true;
				}
				else if (flag26 && npc.ai[0] == 0f && npc.velocity.Y == 0f && Main.rand.NextBool(1200))
				{
					int num108 = 220;
					int num109 = 150;
					for (int num110 = 0; num110 < 255; num110++)
					{
						Player player3 = Main.player[num110];
						if (player3.CanBeTalkedTo && player3.Distance(npc.Center) < (float)num109 && Collision.CanHitLine(npc.Top, 0, 0, player3.Top, 0, 0))
						{
							int num111 = (npc.position.X < player3.position.X).ToDirectionInt();
							npc.ai[0] = 7f;
							npc.ai[1] = num108;
							npc.ai[2] = num110;
							npc.direction = num111;
							npc.netUpdate = true;
							break;
						}
					}
				}
				else if (flag26 && npc.ai[0] == 1f && npc.velocity.Y == 0f && num > 0 && Main.rand.NextBool(num))
				{
					Point point = (npc.Bottom + Vector2.UnitY * -2f).ToTileCoordinates();
					bool flag30 = WorldGen.InWorld(point.X, point.Y, 1);
					if (flag30)
					{
						for (int num112 = 0; num112 < 200; num112++)
						{
							if (Main.npc[num112].active && Main.npc[num112].aiStyle == 7 && Main.npc[num112].townNPC && Main.npc[num112].ai[0] == 5f && (Main.npc[num112].Bottom + Vector2.UnitY * -2f).ToTileCoordinates() == point)
							{
								flag30 = false;
								break;
							}
						}

						for (int num113 = 0; num113 < 255; num113++)
						{
							if (Main.player[num113].active && Main.player[num113].sitting.isSitting && Main.player[num113].Center.ToTileCoordinates() == point)
							{
								flag30 = false;
								break;
							}
						}
					}

					if (flag30)
					{
						Tile tile2 = Main.tile[point.X, point.Y];

						flag30 = TileID.Sets.CanBeSatOnForNPCs[tile2.TileType];

						if (flag30 && tile2.TileType == 15 && tile2.TileFrameY >= 1080 && tile2.TileFrameY <= 1098)
							flag30 = false;

						if (flag30)
						{
							npc.ai[0] = 5f;
							npc.ai[1] = 900 + Main.rand.Next(10800);

							npc.SitDown(point, out int targetDirection, out var bottom);
							npc.direction = targetDirection;
							npc.Bottom = bottom;

							npc.velocity = Vector2.Zero;
							npc.localAI[3] = 0f;
							npc.netUpdate = true;
						}
					}
				}
				else if (flag26 && npc.ai[0] == 1f && npc.velocity.Y == 0f && Main.rand.NextBool(600) && Utils.PlotTileLine(npc.Top, npc.Bottom, npc.width, DelegateMethods.SearchAvoidedByNPCs))
				{
					Point point2 = (npc.Center + new Vector2(npc.direction * 10, 0f)).ToTileCoordinates();
					bool flag31 = WorldGen.InWorld(point2.X, point2.Y, 1);
					if (flag31)
					{
						Tile tileSafely7 = Framing.GetTileSafely(point2.X, point2.Y);
						if (!tileSafely7.HasUnactuatedTile || !TileID.Sets.InteractibleByNPCs[tileSafely7.TileType])
							flag31 = false;
					}

					if (flag31)
					{
						npc.ai[0] = 9f;
						npc.ai[1] = 40 + Main.rand.Next(90);
						npc.velocity = Vector2.Zero;
						npc.localAI[3] = 0f;
						npc.netUpdate = true;
					}
				}

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[0] < 2f && npc.velocity.Y == 0f && npc.breath > 0)
				{
					int num114 = -1;
					for (int num115 = 0; num115 < 200; num115++)
					{
						NPC nPC6 = Main.npc[num115];
						if (nPC6.CurrentCaptor() is null && nPC6.active && nPC6.townNPC && nPC6.life != nPC6.lifeMax && (num114 == -1 || nPC6.lifeMax - nPC6.life > Main.npc[num114].lifeMax - Main.npc[num114].life) && Collision.CanHitLine(npc.position, npc.width, npc.height, nPC6.position, nPC6.width, nPC6.height) && npc.Distance(nPC6.Center) < 500f)
							num114 = num115;
					}

					if (num114 != -1)
					{
						npc.ai[0] = 13f;
						npc.ai[1] = 34f;
						npc.ai[2] = num114;
						npc.localAI[3] = 0f;
						npc.direction = ((npc.position.X < Main.npc[num114].position.X) ? 1 : (-1));
						npc.netUpdate = true;
					}
				}

				if (flag27 && npc.velocity.Y == 0f && Main.rand.NextBool(NPCID.Sets.AttackAverageChance[NPCID.Nurse] * 2))
				{
					int num116 = NPCID.Sets.AttackTime[NPCID.Nurse];
					int num117 = ((currentTargetDirection == 1) ? currentSecondaryTarget : currentPrimaryTarget);
					int num118 = ((currentTargetDirection == 1) ? currentPrimaryTarget : currentSecondaryTarget);
					if (num117 != -1 && !Collision.CanHit(npc.Center, 0, 0, Main.npc[num117].Center, 0, 0))
						num117 = ((num118 == -1 || !Collision.CanHit(npc.Center, 0, 0, Main.npc[num118].Center, 0, 0)) ? (-1) : num118);

					bool flag32 = num117 != -1;

					if (flag32)
					{
						npc.localAI[2] = npc.ai[0];
						npc.ai[0] = 10f;
						npc.ai[1] = num116;
						npc.ai[2] = 0f;
						npc.localAI[3] = 0f;
						npc.direction = ((npc.position.X < Main.npc[num117].position.X) ? 1 : (-1));
						npc.netUpdate = true;
					}
				}
			}
			return false;
		}
	}
}

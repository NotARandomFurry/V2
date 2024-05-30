using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;

namespace V2.NPCs.Vanilla.TownNPCs
{
	public static class TownNPCAIReference
	{
		public static void TownNPCVanillaAI(NPC npc)
		{
			NPC.ShimmeredTownNPCs[npc.type] = npc.IsShimmerVariant;
			if (npc.type == NPCID.TaxCollector && npc.GivenName == "Andrew")
				npc.defDefense = 200;

			int num = 300;
			if (npc.type == NPCID.TownDog || npc.type == NPCID.TownBunny || NPCID.Sets.IsTownSlime[npc.type])
				num = 0;

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

			if (npc.type == NPCID.SantaClaus && Main.netMode != NetmodeID.MultiplayerClient && !Main.xMas)
			{
				npc.SimpleStrikeNPC(9999, 0, noPlayerInteraction: true);
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, 9999f);
			}

			if ((npc.type == NPCID.Penguin || npc.type == NPCID.PenguinBlack) && npc.localAI[0] == 0f)
				npc.localAI[0] = Main.rand.Next(1, 5);

			if (npc.type == NPCID.Mechanic)
			{
				int num3 = NPC.lazyNPCOwnedProjectileSearchArray[npc.whoAmI];
				bool flag2 = false;
				if (Main.projectile.IndexInRange(num3))
				{
					Projectile projectile = Main.projectile[num3];
					if (projectile.active && projectile.type == 582 && projectile.ai[1] == (float)npc.whoAmI)
						flag2 = true;
				}

				npc.localAI[0] = flag2.ToInt();
			}

			if ((npc.type == NPCID.Duck || npc.type == NPCID.DuckWhite || npc.type == NPCID.Seagull || npc.type == NPCID.Grebe) && Main.netMode != NetmodeID.MultiplayerClient && (npc.velocity.Y > 4f || npc.velocity.Y < -4f || npc.wet))
			{
				int num4 = npc.direction;
				npc.Transform(npc.type + 1);
				npc.TargetClosest();
				npc.direction = num4;
				npc.netUpdate = true;
				return;
			}

			switch (npc.type)
			{
				case 588:
					NPC.savedGolfer = true;
					break;
				case 441:
					NPC.savedTaxCollector = true;
					break;
				case 107:
					NPC.savedGoblin = true;
					break;
				case 108:
					NPC.savedWizard = true;
					break;
				case 124:
					NPC.savedMech = true;
					break;
				case 353:
					NPC.savedStylist = true;
					break;
				case 369:
					NPC.savedAngler = true;
					break;
				case 550:
					NPC.savedBartender = true;
					break;
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
					return;

				if (npc.ai[1] == 0f && npc.ai[2] < 1f)
					AI_007_TownEntities_Shimmer_TeleportToLandingSpot(npc);

				if (npc.ai[2] > 0f)
				{
					npc.ai[2] -= 1f;
					if (npc.ai[2] <= 0f)
						npc.ai[1] = 1f;

					return;
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

				return;
			}

			if (npc.type >= NPCID.None && NPCID.Sets.TownCritter[npc.type] && npc.target == 255)
			{
				npc.TargetClosest();
				if (npc.position.X < Main.player[npc.target].position.X)
				{
					npc.direction = 1;
					npc.spriteDirection = npc.direction;
				}

				if (npc.position.X > Main.player[npc.target].position.X)
				{
					npc.direction = -1;
					npc.spriteDirection = npc.direction;
				}

				if (npc.homeTileX == -1)
					npc.UpdateHomeTileState(npc.homeless, (int)((npc.position.X + (float)(npc.width / 2)) / 16f), npc.homeTileY);
			}
			else if (npc.homeTileX == -1 && npc.homeTileY == -1 && npc.velocity.Y == 0f && !npc.shimmering)
			{
				npc.UpdateHomeTileState(npc.homeless, (int)npc.Center.X / 16, (int)(npc.position.Y + (float)npc.height + 4f) / 16);
			}

			bool flag3 = false;
			int num6 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
			int num7 = (int)(npc.position.Y + (float)npc.height + 1f) / 16;
			AI_007_FindGoodRestingSpot(npc, num6, num7, out var floorX, out var floorY);
			if (npc.type == NPCID.TaxCollector)
				NPC.taxCollector = true;

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
				if (npc.type == NPCID.OldMan)
					SoundEngine.PlaySound(SoundID.Roar, npc.position);

				return;
			}

			if (npc.type == NPCID.OldMan && Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.UpdateHomeTileState(false, Main.dungeonX, Main.dungeonY);
				if (NPC.downedBoss3)
				{
					npc.ai[3] = 1f;
					npc.netUpdate = true;
				}
			}

			if (npc.type == NPCID.TravellingMerchant)
			{
				npc.homeless = true;
				if (!Main.dayTime)
				{
					if (!npc.shimmering)
						npc.UpdateHomeTileState(npc.homeless, (int)(npc.Center.X / 16f), (int)(npc.position.Y + (float)npc.height + 2f) / 16);

					if (!flag3 && npc.ai[0] == 0f)
					{
						npc.ai[0] = 1f;
						npc.ai[1] = 200f;
					}

					tryToStayInHouse = false;
				}
			}

			if (npc.type == NPCID.Angler && npc.homeless && npc.wet)
			{
				if (npc.Center.X / 16f < 380f || npc.Center.X / 16f > (float)(Main.maxTilesX - 380))
				{
					npc.UpdateHomeTileState(npc.homeless, Main.spawnTileX, Main.spawnTileY);
					npc.ai[0] = 1f;
					npc.ai[1] = 200f;
				}

				if (npc.position.X / 16f < 300f)
					npc.direction = 1;
				else if (npc.position.X / 16f > (float)(Main.maxTilesX - 300))
					npc.direction = -1;
			}

			if (!WorldGen.InWorld(num6, num7) || Main.netMode == NetmodeID.MultiplayerClient && !Main.sectionManager.TileLoaded(num6, num7))
				return;

			if (!npc.homeless && Main.netMode != NetmodeID.MultiplayerClient && npc.townNPC && (tryToStayInHouse || (npc.type == NPCID.OldMan && Main.tileDungeon[Main.tile[num6, num7].TileType])) && !AI_007_TownEntities_IsInAGoodRestingSpot(npc, num6, num7, floorX, floorY))
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

			bool isRodent = npc.type == NPCID.Mouse || npc.type == NPCID.GoldMouse || npc.type == NPCID.Rat;
			bool canBreatheUnderwater = npc.type == NPCID.Turtle || npc.type == NPCID.TurtleJungle || npc.type == NPCID.SeaTurtle;
			bool flag7 = npc.type == NPCID.Frog || npc.type == NPCID.GoldFrog || npc.type == NPCID.BoundTownSlimeYellow;
			bool flag8 = NPCID.Sets.IsTownSlime[npc.type];
			_ = NPCID.Sets.IsTownPet[npc.type];
			bool flag9 = canBreatheUnderwater || flag7;
			bool flag10 = canBreatheUnderwater || flag7;
			bool flag11 = flag8;
			bool flag12 = flag8;
			float num8 = 200f;
			if (NPCID.Sets.DangerDetectRange[npc.type] != -1)
				num8 = NPCID.Sets.DangerDetectRange[npc.type];

			bool flag13 = false;
			bool flag14 = false;
			float num9 = -1f;
			float num10 = -1f;
			int num11 = 0;
			int num12 = -1;
			int num13 = -1;
			if (!canBreatheUnderwater && Main.netMode != NetmodeID.MultiplayerClient && !flag3)
			{
				for (int m = 0; m < 200; m++)
				{
					if (!Main.npc[m].active || Main.npc[m].friendly || Main.npc[m].damage <= 0 || !(Main.npc[m].Distance(npc.Center) < num8) || (npc.type == NPCID.SkeletonMerchant && NPCID.Sets.Skeletons[Main.npc[m].type]) || (!Main.npc[m].noTileCollide && !Collision.CanHit(npc.Center, 0, 0, Main.npc[m].Center, 0, 0)))
						continue;

					if (!NPCLoader.CanHitNPC(Main.npc[m], npc))
						continue;

					bool flag15 = Main.npc[m].CanBeChasedBy(npc);
					flag13 = true;
					float num14 = Main.npc[m].Center.X - npc.Center.X;
					if (npc.type == NPCID.ExplosiveBunny)
					{
						if (num14 < 0f && (num9 == -1f || num14 > num9))
						{
							num10 = num14;
							num13 = m;
						}

						if (num14 > 0f && (num10 == -1f || num14 < num10))
						{
							num9 = num14;
							num12 = m;
						}

						continue;
					}

					if (num14 < 0f && (num9 == -1f || num14 > num9))
					{
						num9 = num14;
						if (flag15)
							num12 = m;
					}

					if (num14 > 0f && (num10 == -1f || num14 < num10))
					{
						num10 = num14;
						if (flag15)
							num13 = m;
					}
				}

				if (flag13)
				{
					num11 = ((num9 == -1f) ? 1 : ((num10 != -1f) ? (num10 < 0f - num9).ToDirectionInt() : (-1)));
					float num15 = 0f;
					if (num9 != -1f)
						num15 = 0f - num9;

					if (num15 == 0f || (num10 < num15 && num10 > 0f))
						num15 = num10;

					if (npc.ai[0] == 8f)
					{
						if (npc.direction == -num11)
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
						if (NPCID.Sets.PrettySafe[npc.type] != -1 && (float)NPCID.Sets.PrettySafe[npc.type] < num15)
						{
							flag13 = false;
							flag14 = NPCID.Sets.AttackType[npc.type] > -1;
						}
						else if (npc.ai[0] != 1f)
						{
							int tileX = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
							int tileY = (int)((npc.position.Y + (float)npc.height - 16f) / 16f);
							bool currentlyDrowning = npc.wet && !flag9;
							AI_007_TownEntities_GetWalkPrediction(npc, num6, floorX, flag9, currentlyDrowning, tileX, tileY, out var _, out var avoidFalling);
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
										nPC.direction = -num11;
										nPC.netUpdate = true;
									}
								}

								npc.ai[0] = 1f;
								npc.ai[1] = 120 + Main.rand.Next(120);
								npc.ai[2] = 0f;
								npc.localAI[3] = 0f;
								npc.direction = -num11;
								npc.netUpdate = true;
							}
						}
						else if (npc.ai[0] == 1f && npc.direction != -num11)
						{
							npc.direction = -num11;
							npc.netUpdate = true;
						}
					}
				}
			}

			if (npc.ai[0] == 0f)
			{
				if (npc.localAI[3] > 0f)
					npc.localAI[3] -= 1f;

				int num16 = 120;
				if (npc.type == NPCID.TownDog)
					num16 = 60;

				if ((flag7 || flag8) && npc.wet)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 200 + Main.rand.Next(500, 700);
					npc.ai[2] = 0f;
					npc.localAI[3] = 0f;
					npc.netUpdate = true;
				}
				else if (tryToStayInHouse && !flag3 && !NPCID.Sets.TownCritter[npc.type])
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

							if (NPCID.Sets.IsTownPet[npc.type])
								AI_007_AttemptToPlayIdleAnimationsForPets(npc, num16 * 4);
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
					if (isRodent)
						npc.velocity.X *= 0.5f;

					if (npc.velocity.X > 0.1f)
						npc.velocity.X -= 0.1f;
					else if (npc.velocity.X < -0.1f)
						npc.velocity.X += 0.1f;
					else
						npc.velocity.X = 0f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (!flag3 && NPCID.Sets.IsTownPet[npc.type] && npc.ai[1] >= 100f && npc.ai[1] <= 150f)
							AI_007_AttemptToPlayIdleAnimationsForPets(npc, num16);

						if (npc.ai[1] > 0f)
							npc.ai[1] -= 1f;

						bool flag16 = true;
						int tileX2 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
						int tileY2 = (int)((npc.position.Y + (float)npc.height - 16f) / 16f);
						bool currentlyDrowning2 = npc.wet && !flag9;
						AI_007_TownEntities_GetWalkPrediction(npc, num6, floorX, flag9, currentlyDrowning2, tileX2, tileY2, out var _, out var avoidFalling2);
						if (npc.wet && !flag9)
						{
							bool currentlyDrowning3 = Collision.DrownCollision(npc.position, npc.width, npc.height, 1f, includeSlopes: true);
							if (currentlyDrowning3)
							{
								npc.ai[0] = 1f;
								npc.ai[1] = 200 + Main.rand.Next(300);
								npc.ai[2] = 0f;
								if (NPCID.Sets.TownCritter[npc.type])
									npc.ai[1] += Main.rand.Next(200, 400);

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
								if (NPCID.Sets.TownCritter[npc.type])
									npc.ai[1] += Main.rand.Next(200, 400);

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
				if (Main.netMode != NetmodeID.MultiplayerClient && tryToStayInHouse && AI_007_TownEntities_IsInAGoodRestingSpot(npc, num6, num7, floorX, floorY) && !NPCID.Sets.TownCritter[npc.type])
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 200 + Main.rand.Next(200);
					npc.localAI[3] = 60f;
					npc.netUpdate = true;
				}
				else
				{
					bool flag17 = !flag9 && Collision.DrownCollision(npc.position, npc.width, npc.height, 1f, includeSlopes: true);
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
						if (NPCID.Sets.TownCritter[npc.type])
							npc.ai[1] -= Main.rand.Next(100);
						else
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
					if (npc.type == NPCID.ExplosiveBunny && flag13)
					{
						num17 = 1.5f;
						num18 = 0.1f;
					}
					else if (npc.type == NPCID.Squirrel || npc.type == NPCID.SquirrelGold || npc.type == NPCID.SquirrelRed || (npc.type >= NPCID.GemSquirrelAmethyst && npc.type <= NPCID.GemSquirrelAmber))
					{
						num17 = 1.5f;
					}
					else if (canBreatheUnderwater)
					{
						if (npc.wet)
						{
							num18 = 1f;
							num17 = 2f;
						}
						else
						{
							num18 = 0.07f;
							num17 = 0.5f;
						}
					}

					if (npc.type == NPCID.SeaTurtle)
					{
						if (npc.wet)
						{
							num18 = 1f;
							num17 = 2.5f;
						}
						else
						{
							num18 = 0.07f;
							num17 = 0.2f;
						}
					}

					if (isRodent)
					{
						num17 = 2f;
						num18 = 1f;
					}

					if (npc.friendly && (flag13 || flag17))
					{
						num17 = 1.5f;
						float num19 = 1f - (float)npc.life / (float)npc.lifeMax;
						num17 += num19 * 0.9f;
						num18 = 0.1f;
					}

					if (flag11 && npc.wet)
					{
						num17 = 2f;
						num18 = 0.2f;
					}

					if (flag7 && npc.wet)
					{
						if (Math.Abs(npc.velocity.X) < 0.05f && Math.Abs(npc.velocity.Y) < 0.05f)
							npc.velocity.X += num17 * 10f * (float)npc.direction;
						else
							npc.velocity.X *= 0.9f;
					}
					else if (npc.velocity.X < 0f - num17 || npc.velocity.X > num17)
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
						AI_007_TownEntities_GetWalkPrediction(npc, num6, floorX, flag9, flag17, num20, num21, out var keepwalking3, out var avoidFalling3);
						bool flag19 = false;
						bool flag20 = false;
						if (npc.wet && !flag9 && npc.townNPC && (flag20 = flag17) && npc.localAI[3] <= 0f)
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

						if ((npc.townNPC || NPCID.Sets.AllowDoorInteraction[npc.type]) && tileSafely5.HasUnactuatedTile && (TileLoader.IsClosedDoor(tileSafely5) || tileSafely5.TileType == 388) && (Main.rand.NextBool(10)|| tryToStayInHouse))
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
									else if (isRodent)
									{
										if (WorldGen.SolidTile((int)(npc.Center.X / 16f) + npc.direction, (int)(npc.Center.Y / 16f)))
										{
											npc.direction *= -1;
											npc.velocity.X *= 0f;
											npc.netUpdate = true;
										}
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
								else if (flag12 && !Collision.SolidTilesVersatile(num20 - npc.direction * 2, num20 - npc.direction, num21 - 2, num21 - 1))
								{
									npc.velocity.Y = -5f;
									npc.netUpdate = true;
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

							if (npc.velocity.Y < 0f && NPCID.Sets.TownCritter[npc.type] && !isRodent)
								npc.velocity.Y *= 1.2f;
						}
					}
					else if (flag12 && !npc.wet)
					{
						int num29 = (int)(npc.Center.X / 16f);
						int num30 = (int)((npc.position.Y + (float)npc.height - 16f) / 16f);
						int num31 = 0;
						for (int num32 = -1; num32 <= 1; num32++)
						{
							for (int num33 = 1; num33 <= 6; num33++)
							{
								Tile tileSafely6 = Framing.GetTileSafely(num29 + num32, num30 + num33);
								if (tileSafely6.LiquidAmount > 0 || (tileSafely6.HasUnactuatedTile && Main.tileSolid[tileSafely6.TileType]))
									num31++;
							}
						}

						if (num31 <= 2)
						{
							if (npc.velocity.X != 0f)
								npc.netUpdate = true;

							npc.velocity.X *= 0.2f;
							npc.ai[0] = 0f;
							npc.ai[1] = 50 + Main.rand.Next(50);
							npc.ai[2] = 0f;
							npc.localAI[3] = 40f;
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
				float num42 = NPCID.Sets.DangerDetectRange[npc.type];
				float attackProjectileRandomOffset = 0f;
				if ((float)NPCID.Sets.AttackTime[npc.type] == npc.ai[1])
				{
					npc.frameCounter = 0.0;
					npc.localAI[3] = 0f;
				}

				if (npc.type == NPCID.Demolitionist)
				{
					attackProjectileType = 30;
					attackProjectileSpeedMult = 6f;
					attackBaseDamage = 20;
					attackProjectileDelay = 10;
					attackCooldown = 180;
					attackRandomExtraCooldown = 120;
					attackProjectileGravityCorrection = 16f;
					attackKnockback = 7f;
				}
				else if (npc.type == NPCID.BestiaryGirl)
				{
					attackProjectileType = 880;
					attackProjectileSpeedMult = 24f;
					attackBaseDamage = 15;
					attackProjectileDelay = 1;
					attackProjectileGravityCorrection = 0f;
					attackKnockback = 7f;
					attackCooldown = 15;
					attackRandomExtraCooldown = 10;
					if (npc.ShouldBestiaryGirlBeLycantrope())
					{
						attackProjectileType = 929;
						attackBaseDamage = (int)((float)attackBaseDamage * 1.5f);
					}
				}
				else if (npc.type == NPCID.DD2Bartender)
				{
					attackProjectileType = 669;
					attackProjectileSpeedMult = 6f;
					attackBaseDamage = 24;
					attackProjectileDelay = 10;
					attackCooldown = 120;
					attackRandomExtraCooldown = 60;
					attackProjectileGravityCorrection = 16f;
					attackKnockback = 9f;
				}
				else if (npc.type == NPCID.Golfer)
				{
					attackProjectileType = 721;
					attackProjectileSpeedMult = 8f;
					attackBaseDamage = 15;
					attackProjectileDelay = 5;
					attackCooldown = 20;
					attackRandomExtraCooldown = 10;
					attackProjectileGravityCorrection = 16f;
					attackKnockback = 9f;
				}
				else if (npc.type == NPCID.PartyGirl)
				{
					attackProjectileType = 588;
					attackProjectileSpeedMult = 6f;
					attackBaseDamage = 30;
					attackProjectileDelay = 10;
					attackCooldown = 60;
					attackRandomExtraCooldown = 120;
					attackProjectileGravityCorrection = 16f;
					attackKnockback = 6f;
				}
				else if (npc.type == NPCID.Merchant)
				{
					attackProjectileType = 48;
					attackProjectileSpeedMult = 9f;
					attackBaseDamage = 12;
					attackProjectileDelay = 10;
					attackCooldown = 60;
					attackRandomExtraCooldown = 60;
					attackProjectileGravityCorrection = 16f;
					attackKnockback = 1.5f;
				}
				else if (npc.type == NPCID.Angler)
				{
					attackProjectileType = 520;
					attackProjectileSpeedMult = 12f;
					attackBaseDamage = 10;
					attackProjectileDelay = 10;
					attackCooldown = 0;
					attackRandomExtraCooldown = 1;
					attackProjectileGravityCorrection = 16f;
					attackKnockback = 3f;
				}
				else if (npc.type == NPCID.SkeletonMerchant)
				{
					attackProjectileType = 21;
					attackProjectileSpeedMult = 14f;
					attackBaseDamage = 14;
					attackProjectileDelay = 10;
					attackCooldown = 0;
					attackRandomExtraCooldown = 1;
					attackProjectileGravityCorrection = 16f;
					attackKnockback = 3f;
				}
				else if (npc.type == NPCID.GoblinTinkerer)
				{
					attackProjectileType = 24;
					attackProjectileSpeedMult = 5f;
					attackBaseDamage = 15;
					attackProjectileDelay = 10;
					attackCooldown = 60;
					attackRandomExtraCooldown = 60;
					attackProjectileGravityCorrection = 16f;
					attackKnockback = 1f;
				}
				else if (npc.type == NPCID.Mechanic)
				{
					attackProjectileType = 582;
					attackProjectileSpeedMult = 10f;
					attackBaseDamage = 11;
					attackProjectileDelay = 1;
					attackCooldown = 30;
					attackRandomExtraCooldown = 30;
					attackKnockback = 3.5f;
				}
				else if (npc.type == NPCID.Nurse)
				{
					attackProjectileType = 583;
					attackProjectileSpeedMult = 8f;
					attackBaseDamage = 8;
					attackProjectileDelay = 1;
					attackCooldown = 15;
					attackRandomExtraCooldown = 10;
					attackKnockback = 2f;
					attackProjectileGravityCorrection = 10f;
				}
				else if (npc.type == NPCID.SantaClaus)
				{
					attackProjectileType = 589;
					attackProjectileSpeedMult = 7f;
					attackBaseDamage = 22;
					attackProjectileDelay = 1;
					attackCooldown = 10;
					attackRandomExtraCooldown = 1;
					attackKnockback = 2f;
					attackProjectileGravityCorrection = 10f;
				}

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
					Vector2 vec = -Vector2.UnitY;
					if (num11 == 1 && npc.spriteDirection == 1 && num13 != -1)
						vec = npc.DirectionTo(Main.npc[num13].Center + new Vector2(0f, (0f - attackProjectileGravityCorrection) * MathHelper.Clamp(npc.Distance(Main.npc[num13].Center) / num42, 0f, 1f)));

					if (num11 == -1 && npc.spriteDirection == -1 && num12 != -1)
						vec = npc.DirectionTo(Main.npc[num12].Center + new Vector2(0f, (0f - attackProjectileGravityCorrection) * MathHelper.Clamp(npc.Distance(Main.npc[num12].Center) / num42, 0f, 1f)));

					if (vec.HasNaNs() || Math.Sign(vec.X) != npc.spriteDirection)
						vec = new Vector2(npc.spriteDirection, -1f);

					vec *= attackProjectileSpeedMult;
					vec += Utils.RandomVector2(Main.rand, 0f - attackProjectileRandomOffset, attackProjectileRandomOffset);
					int num44 = 1000;
					num44 = npc.type switch
					{
						NPCID.Mechanic => Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, vec.X, vec.Y, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer, 0f, npc.whoAmI, npc.townNpcVariationIndex),
						NPCID.SantaClaus => Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, vec.X, vec.Y, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer, 0f, Main.rand.Next(5)),
						_ => Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, vec.X, vec.Y, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer),
					};
					Main.projectile[num44].npcProj = true;
					Main.projectile[num44].noDropItem = true;
					if (npc.type == NPCID.Golfer)
						Main.projectile[num44].timeLeft = 480;
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
			else if (npc.ai[0] == 12f)
			{
				int attackProjectileType = 0;
				int attackBaseDamage = 0;
				float attackProjectileSpeedMultiplier = 0f;
				int attackProjectileDelay = 0;
				int attackCooldown = 0;
				int attackRandomExtraCooldown = 0;
				float attackKnockback = 0f;

				float attackProjectileGravityCorrection = 0;

				bool attackIsInBetweenShots = false;
				float attackProjectileRandomOffset = 0f;
				if ((float)NPCID.Sets.AttackTime[npc.type] == npc.ai[1])
				{
					npc.frameCounter = 0.0;
					npc.localAI[3] = 0f;
				}

				int num52 = -1;
				if (num11 == 1 && npc.spriteDirection == 1)
					num52 = num13;

				if (num11 == -1 && npc.spriteDirection == -1)
					num52 = num12;

				if (npc.type == NPCID.ArmsDealer)
				{
					attackProjectileType = 14;
					attackProjectileSpeedMultiplier = 13f;
					attackBaseDamage = 24;
					attackCooldown = 14;
					attackRandomExtraCooldown = 4;
					attackKnockback = 3f;
					attackProjectileDelay = 1;
					attackProjectileRandomOffset = 0.5f;
					if ((float)NPCID.Sets.AttackTime[npc.type] == npc.ai[1])
					{
						npc.frameCounter = 0.0;
						npc.localAI[3] = 0f;
					}

					if (Main.hardMode)
					{
						attackBaseDamage = 15;
						if (npc.localAI[3] > (float)attackProjectileDelay)
						{
							attackProjectileDelay = 10;
							attackIsInBetweenShots = true;
						}

						if (npc.localAI[3] > (float)attackProjectileDelay)
						{
							attackProjectileDelay = 20;
							attackIsInBetweenShots = true;
						}

						if (npc.localAI[3] > (float)attackProjectileDelay)
						{
							attackProjectileDelay = 30;
							attackIsInBetweenShots = true;
						}
					}
				}
				else if (npc.type == NPCID.Painter)
				{
					attackProjectileType = 587;
					attackProjectileSpeedMultiplier = 10f;
					attackBaseDamage = 8;
					attackCooldown = 10;
					attackRandomExtraCooldown = 1;
					attackKnockback = 1.75f;
					attackProjectileDelay = 1;
					attackProjectileRandomOffset = 0.5f;
					if (npc.localAI[3] > (float)attackProjectileDelay)
					{
						attackProjectileDelay = 12;
						attackIsInBetweenShots = true;
					}

					if (npc.localAI[3] > (float)attackProjectileDelay)
					{
						attackProjectileDelay = 24;
						attackIsInBetweenShots = true;
					}

					if (Main.hardMode)
						attackBaseDamage += 2;
				}
				else if (npc.type == NPCID.TravellingMerchant)
				{
					attackProjectileType = 14;
					attackProjectileSpeedMultiplier = 13f;
					attackBaseDamage = 24;
					attackCooldown = 12;
					attackRandomExtraCooldown = 5;
					attackKnockback = 2f;
					attackProjectileDelay = 1;
					attackProjectileRandomOffset = 0.2f;
					if (Main.hardMode)
					{
						attackBaseDamage = 30;
						attackProjectileType = 357;
					}
				}
				else if (npc.type == NPCID.Guide)
				{
					attackProjectileSpeedMultiplier = 10f;
					attackBaseDamage = 8;
					attackProjectileDelay = 1;
					if (Main.hardMode)
					{
						attackProjectileType = 2;
						attackCooldown = 15;
						attackRandomExtraCooldown = 10;
						attackBaseDamage += 6;
					}
					else
					{
						attackProjectileType = 1;
						attackCooldown = 30;
						attackRandomExtraCooldown = 20;
					}

					attackKnockback = 2.75f;
					attackProjectileGravityCorrection = 4;
					attackProjectileRandomOffset = 0.7f;
				}
				else if (npc.type == NPCID.WitchDoctor)
				{
					attackProjectileType = 267;
					attackProjectileSpeedMultiplier = 14f;
					attackBaseDamage = 20;
					attackProjectileDelay = 1;
					attackCooldown = 10;
					attackRandomExtraCooldown = 1;
					attackKnockback = 3f;
					attackProjectileGravityCorrection = 6;
					attackProjectileRandomOffset = 0.4f;
				}
				else if (npc.type == NPCID.Steampunker)
				{
					attackProjectileType = 242;
					attackProjectileSpeedMultiplier = 13f;
					attackBaseDamage = ((!Main.hardMode) ? 11 : 15);
					attackCooldown = 10;
					attackRandomExtraCooldown = 1;
					attackKnockback = 2f;
					attackProjectileDelay = 1;
					if (npc.localAI[3] > (float)attackProjectileDelay)
					{
						attackProjectileDelay = 8;
						attackIsInBetweenShots = true;
					}

					if (npc.localAI[3] > (float)attackProjectileDelay)
					{
						attackProjectileDelay = 16;
						attackIsInBetweenShots = true;
					}

					attackProjectileRandomOffset = 0.3f;
				}
				else if (npc.type == NPCID.Pirate)
				{
					attackProjectileType = 14;
					attackProjectileSpeedMultiplier = 14f;
					attackBaseDamage = 24;
					attackCooldown = 10;
					attackRandomExtraCooldown = 1;
					attackKnockback = 2f;
					attackProjectileDelay = 1;
					attackProjectileRandomOffset = 0.7f;
					if (npc.localAI[3] > (float)attackProjectileDelay)
					{
						attackProjectileDelay = 16;
						attackIsInBetweenShots = true;
					}

					if (npc.localAI[3] > (float)attackProjectileDelay)
					{
						attackProjectileDelay = 24;
						attackIsInBetweenShots = true;
					}

					if (npc.localAI[3] > (float)attackProjectileDelay)
					{
						attackProjectileDelay = 32;
						attackIsInBetweenShots = true;
					}

					if (npc.localAI[3] > (float)attackProjectileDelay)
					{
						attackProjectileDelay = 40;
						attackIsInBetweenShots = true;
					}

					if (npc.localAI[3] > (float)attackProjectileDelay)
					{
						attackProjectileDelay = 48;
						attackIsInBetweenShots = true;
					}

					if (npc.localAI[3] == 0f && num52 != -1 && npc.Distance(Main.npc[num52].Center) < (float)NPCID.Sets.PrettySafe[npc.type])
					{
						attackProjectileRandomOffset = 0.1f;
						attackProjectileType = 162;
						attackBaseDamage = 50;
						attackKnockback = 10f;
						attackProjectileSpeedMultiplier = 24f;
					}
				}
				else if (npc.type == NPCID.Cyborg)
				{
					attackProjectileType = Utils.SelectRandom<int>(Main.rand, 134, 133, 135);
					attackProjectileDelay = 1;
					switch (attackProjectileType)
					{
						case 135:
							attackProjectileSpeedMultiplier = 12f;
							attackBaseDamage = 30;
							attackCooldown = 30;
							attackRandomExtraCooldown = 10;
							attackKnockback = 7f;
							attackProjectileRandomOffset = 0.2f;
							break;
						case 133:
							attackProjectileSpeedMultiplier = 10f;
							attackBaseDamage = 25;
							attackCooldown = 10;
							attackRandomExtraCooldown = 1;
							attackKnockback = 6f;
							attackProjectileRandomOffset = 0.2f;
							break;
						case 134:
							attackProjectileSpeedMultiplier = 13f;
							attackBaseDamage = 20;
							attackCooldown = 20;
							attackRandomExtraCooldown = 10;
							attackKnockback = 4f;
							attackProjectileRandomOffset = 0.1f;
							break;
					}
				}

				NPCLoader.TownNPCAttackStrength(npc, ref attackBaseDamage, ref attackKnockback);
				NPCLoader.TownNPCAttackCooldown(npc, ref attackCooldown, ref attackRandomExtraCooldown);
				NPCLoader.TownNPCAttackProj(npc, ref attackProjectileType, ref attackProjectileDelay);
				NPCLoader.TownNPCAttackProjSpeed(npc, ref attackProjectileSpeedMultiplier, ref attackProjectileGravityCorrection, ref attackProjectileRandomOffset);
				NPCLoader.TownNPCAttackShoot(npc, ref attackIsInBetweenShots);

				if (Main.expertMode)
					attackBaseDamage = (int)((float)attackBaseDamage * Main.GameModeInfo.TownNPCDamageMultiplier);

				attackBaseDamage = (int)((float)attackBaseDamage * damageMult);
				npc.velocity.X *= 0.8f;
				npc.ai[1] -= 1f;
				npc.localAI[3] += 1f;
				if (npc.localAI[3] == (float)attackProjectileDelay && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 attackProjectileSpeed = Vector2.Zero;
					if (num52 != -1)
						attackProjectileSpeed = npc.DirectionTo(Main.npc[num52].Center + new Vector2(0f, -attackProjectileGravityCorrection));

					if (attackProjectileSpeed.HasNaNs() || Math.Sign(attackProjectileSpeed.X) != npc.spriteDirection)
						attackProjectileSpeed = new Vector2(npc.spriteDirection, 0f);

					attackProjectileSpeed *= attackProjectileSpeedMultiplier;
					attackProjectileSpeed += Utils.RandomVector2(Main.rand, 0f - attackProjectileRandomOffset, attackProjectileRandomOffset);
					int num53 = 1000;
					num53 = npc.type switch
					{
						NPCID.Painter => Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, attackProjectileSpeed.X, attackProjectileSpeed.Y, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer, 0f, (float)Main.rand.Next(12) / 6f),
						_ => Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, attackProjectileSpeed.X, attackProjectileSpeed.Y, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer)
					};
					Main.projectile[num53].npcProj = true;
					Main.projectile[num53].noDropItem = true;
				}

				if (npc.localAI[3] == (float)attackProjectileDelay && attackIsInBetweenShots && num52 != -1)
				{
					Vector2 vector2 = npc.DirectionTo(Main.npc[num52].Center);
					if (vector2.Y <= 0.5f && vector2.Y >= -0.5f)
						npc.ai[2] = vector2.Y;
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
				if ((float)NPCID.Sets.AttackTime[npc.type] == npc.ai[1])
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
			else if (npc.ai[0] == 14f)
			{
				int attackProjectileType = 0;
				int attackBaseDamage = 0;
				float attackProjectileSpeedMultiplier = 0f;
				int attackProjectileDelay = 0;
				int attackBaseCooldown = 0;
				int attackRandomExtraCooldown = 0;
				float attackKnockback = 0f;
				float attackProjectileGravityCorrection = 0f;
				float num61 = NPCID.Sets.DangerDetectRange[npc.type];
				float attackMagicAuraLightMultiplier = 1f;
				float attackProjectileRandomOffset = 0f;
				if ((float)NPCID.Sets.AttackTime[npc.type] == npc.ai[1])
				{
					npc.frameCounter = 0.0;
					npc.localAI[3] = 0f;
				}

				int num64 = -1;
				if (num11 == 1 && npc.spriteDirection == 1)
					num64 = num13;

				if (num11 == -1 && npc.spriteDirection == -1)
					num64 = num12;

				if (npc.type == NPCID.Clothier)
				{
					attackProjectileType = 585;
					attackProjectileSpeedMultiplier = 10f;
					attackBaseDamage = 16;
					attackProjectileDelay = 30;
					attackBaseCooldown = 20;
					attackRandomExtraCooldown = 15;
					attackKnockback = 2f;
					attackProjectileRandomOffset = 1f;
				}
				else if (npc.type == NPCID.Wizard)
				{
					attackProjectileType = 15;
					attackProjectileSpeedMultiplier = 6f;
					attackBaseDamage = 18;
					attackProjectileDelay = 15;
					attackBaseCooldown = 15;
					attackRandomExtraCooldown = 5;
					attackKnockback = 3f;
					attackProjectileGravityCorrection = 20f;
				}
				else if (npc.type == NPCID.Truffle)
				{
					attackProjectileType = 590;
					attackBaseDamage = 40;
					attackProjectileDelay = 15;
					attackBaseCooldown = 10;
					attackRandomExtraCooldown = 1;
					attackKnockback = 3f;
					for (; npc.localAI[3] > (float)attackProjectileDelay; attackProjectileDelay += 15)
					{
					}
				}
				else if (npc.type == NPCID.Princess)
				{
					attackProjectileType = 950;
					attackBaseDamage = ((!Main.hardMode) ? 15 : 20);
					attackProjectileDelay = 15;
					attackBaseCooldown = 0;
					attackRandomExtraCooldown = 0;
					attackKnockback = 3f;
					for (; npc.localAI[3] > (float)attackProjectileDelay; attackProjectileDelay += 10)
					{
					}
				}
				else if (npc.type == NPCID.Dryad)
				{
					attackProjectileType = 586;
					attackProjectileDelay = 24;
					attackBaseCooldown = 10;
					attackRandomExtraCooldown = 1;
					attackKnockback = 3f;
				}

				NPCLoader.TownNPCAttackStrength(npc, ref attackBaseDamage, ref attackKnockback);
				NPCLoader.TownNPCAttackCooldown(npc, ref attackBaseCooldown, ref attackRandomExtraCooldown);
				NPCLoader.TownNPCAttackProj(npc, ref attackProjectileType, ref attackProjectileDelay);
				NPCLoader.TownNPCAttackProjSpeed(npc, ref attackProjectileSpeedMultiplier, ref attackProjectileGravityCorrection, ref attackProjectileRandomOffset);
				NPCLoader.TownNPCAttackMagic(npc, ref attackMagicAuraLightMultiplier);

				if (Main.expertMode)
					attackBaseDamage = (int)((float)attackBaseDamage * Main.GameModeInfo.TownNPCDamageMultiplier);

				attackBaseDamage = (int)((float)attackBaseDamage * damageMult);
				npc.velocity.X *= 0.8f;
				npc.ai[1] -= 1f;
				npc.localAI[3] += 1f;
				if (npc.localAI[3] == (float)attackProjectileDelay && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 attackProjectileSpeed = Vector2.Zero;
					if (num64 != -1)
						attackProjectileSpeed = npc.DirectionTo(Main.npc[num64].Center + new Vector2(0f, (0f - attackProjectileGravityCorrection) * MathHelper.Clamp(npc.Distance(Main.npc[num64].Center) / num61, 0f, 1f)));

					if (attackProjectileSpeed.HasNaNs() || Math.Sign(attackProjectileSpeed.X) != npc.spriteDirection)
						attackProjectileSpeed = new Vector2(npc.spriteDirection, 0f);

					attackProjectileSpeed *= attackProjectileSpeedMultiplier;
					attackProjectileSpeed += Utils.RandomVector2(Main.rand, 0f - attackProjectileRandomOffset, attackProjectileRandomOffset);
					if (npc.type == NPCID.Wizard)
					{
						int num65 = Utils.SelectRandom<int>(Main.rand, 1, 1, 1, 1, 2, 2, 3);
						for (int num66 = 0; num66 < num65; num66++)
						{
							Vector2 vector3 = Utils.RandomVector2(Main.rand, -3.4f, 3.4f);
							int num67 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, attackProjectileSpeed.X + vector3.X, attackProjectileSpeed.Y + vector3.Y, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer, 0f, 0f, npc.townNpcVariationIndex);
							Main.projectile[num67].npcProj = true;
							Main.projectile[num67].noDropItem = true;
						}
					}
					else if (npc.type == NPCID.Truffle)
					{
						if (num64 != -1)
						{
							Vector2 vector4 = Main.npc[num64].position - Main.npc[num64].Size * 2f + Main.npc[num64].Size * Utils.RandomVector2(Main.rand, 0f, 1f) * 5f;
							int num68 = 10;
							while (num68 > 0 && WorldGen.SolidTile(Framing.GetTileSafely((int)vector4.X / 16, (int)vector4.Y / 16)))
							{
								num68--;
								vector4 = Main.npc[num64].position - Main.npc[num64].Size * 2f + Main.npc[num64].Size * Utils.RandomVector2(Main.rand, 0f, 1f) * 5f;
							}

							int num69 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector4.X, vector4.Y, 0f, 0f, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer, 0f, 0f, npc.townNpcVariationIndex);
							Main.projectile[num69].npcProj = true;
							Main.projectile[num69].noDropItem = true;
						}
					}
					else if (npc.type == NPCID.Princess)
					{
						if (num64 != -1)
						{
							Vector2 vector5 = Main.npc[num64].position + Main.npc[num64].Size * Utils.RandomVector2(Main.rand, 0f, 1f) * 1f;
							int num70 = 5;
							while (num70 > 0 && WorldGen.SolidTile(Framing.GetTileSafely((int)vector5.X / 16, (int)vector5.Y / 16)))
							{
								num70--;
								vector5 = Main.npc[num64].position + Main.npc[num64].Size * Utils.RandomVector2(Main.rand, 0f, 1f) * 1f;
							}

							int num71 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector5.X, vector5.Y, 0f, 0f, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer, 0f, 0f, npc.townNpcVariationIndex);
							Main.projectile[num71].npcProj = true;
							Main.projectile[num71].noDropItem = true;
						}
					}
					else if (npc.type == NPCID.Dryad)
					{
						int num72 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, attackProjectileSpeed.X, attackProjectileSpeed.Y, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer, 0f, npc.whoAmI, npc.townNpcVariationIndex);
						Main.projectile[num72].npcProj = true;
						Main.projectile[num72].noDropItem = true;
					}
					else
					{
						int num73 = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * 16), npc.Center.Y - 2f, attackProjectileSpeed.X, attackProjectileSpeed.Y, attackProjectileType, attackBaseDamage, attackKnockback, Main.myPlayer);
						Main.projectile[num73].npcProj = true;
						Main.projectile[num73].noDropItem = true;
					}
				}

				if (attackMagicAuraLightMultiplier > 0f)
				{
					// Patch note: num62 - auraLightMultiplier.
					Vector3 vector6 = npc.GetMagicAuraColor().ToVector3() * attackMagicAuraLightMultiplier;
					Lighting.AddLight(npc.Center, vector6.X, vector6.Y, vector6.Z);
				}

				if (npc.ai[1] <= 0f && 0 == 0)
				{
					npc.ai[0] = ((npc.localAI[2] == 8f && flag13) ? 8 : 0);
					// Patch note: num59 - cooldown.
					// Patch note: maxValue3 - randExtraCooldown.
					npc.ai[1] = attackBaseCooldown + Main.rand.Next(attackRandomExtraCooldown);
					npc.ai[2] = 0f;
					npc.localAI[1] = (npc.localAI[3] = attackBaseCooldown / 2 + Main.rand.Next(attackRandomExtraCooldown));
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 15f)
			{
				int attackBaseCooldown = 0;
				int attackRandomExtraCooldown = 0;
				if ((float)NPCID.Sets.AttackTime[npc.type] == npc.ai[1])
				{
					npc.frameCounter = 0.0;
					npc.localAI[3] = 0f;
				}

				int attackBaseDamage = 0;
				float attackKnockback = 0f;
				int attackItemWidth = 0;
				int attackItemHeight = 0;
				if (num11 == 1)
				{
					_ = npc.spriteDirection;
					_ = 1;
				}

				if (num11 == -1)
				{
					_ = npc.spriteDirection;
					_ = -1;
				}

				if (npc.type == NPCID.DyeTrader)
				{
					attackBaseDamage = 11;
					attackItemWidth = (attackItemHeight = 32);
					attackBaseCooldown = 12;
					attackRandomExtraCooldown = 6;
					attackKnockback = 4.25f;
				}
				else if (npc.type == NPCID.TaxCollector)
				{
					attackBaseDamage = 9;
					attackItemWidth = (attackItemHeight = 28);
					attackBaseCooldown = 9;
					attackRandomExtraCooldown = 3;
					attackKnockback = 3.5f;
					if (npc.GivenName == "Andrew")
					{
						attackBaseDamage *= 2;
						attackKnockback *= 2f;
					}
				}
				else if (npc.type == NPCID.Stylist)
				{
					attackBaseDamage = 10;
					attackItemWidth = (attackItemHeight = 32);
					attackBaseCooldown = 15;
					attackRandomExtraCooldown = 8;
					attackKnockback = 5f;
				}
				else if (NPCID.Sets.IsTownPet[npc.type])
				{
					attackBaseDamage = 10;
					attackItemWidth = (attackItemHeight = 32);
					attackBaseCooldown = 15;
					attackRandomExtraCooldown = 8;
					attackKnockback = 3f;
				}

				NPCLoader.TownNPCAttackStrength(npc, ref attackBaseDamage, ref attackKnockback);
				NPCLoader.TownNPCAttackCooldown(npc, ref attackBaseCooldown, ref attackRandomExtraCooldown);
				NPCLoader.TownNPCAttackSwing(npc, ref attackItemWidth, ref attackItemHeight);

				if (Main.expertMode)
					attackBaseDamage = (int)((float)attackBaseDamage * Main.GameModeInfo.TownNPCDamageMultiplier);

				attackBaseDamage = (int)((float)attackBaseDamage * damageMult);
				npc.velocity.X *= 0.8f;
				npc.ai[1] -= 1f;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Tuple<Vector2, float> swingStats = npc.GetSwingStats(NPCID.Sets.AttackTime[npc.type] * 2, (int)npc.ai[1], npc.spriteDirection, attackItemWidth, attackItemHeight);
					Rectangle itemRectangle = new Rectangle((int)swingStats.Item1.X, (int)swingStats.Item1.Y, attackItemWidth, attackItemHeight);
					if (npc.spriteDirection == -1)
						itemRectangle.X -= attackItemWidth;

					itemRectangle.Y -= attackItemHeight;
					npc.TweakSwingStats(NPCID.Sets.AttackTime[npc.type] * 2, (int)npc.ai[1], npc.spriteDirection, ref itemRectangle);
					int myPlayer = Main.myPlayer;
					for (int num79 = 0; num79 < 200; num79++)
					{
						NPC nPC2 = Main.npc[num79];
						if (nPC2.active && nPC2.immune[myPlayer] == 0 && !nPC2.dontTakeDamage && !nPC2.friendly && nPC2.damage > 0 && itemRectangle.Intersects(nPC2.Hitbox) && (nPC2.noTileCollide || Collision.CanHit(npc.position, npc.width, npc.height, nPC2.position, nPC2.width, nPC2.height)))
						{
							nPC2.SimpleStrikeNPC(attackBaseDamage, npc.spriteDirection, knockBack: attackKnockback);
							if (Main.netMode != NetmodeID.SinglePlayer)
								NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, num79, attackBaseDamage, attackKnockback, npc.spriteDirection);

							nPC2.netUpdate = true;
							nPC2.immune[myPlayer] = (int)npc.ai[1] + 2;
						}
					}
				}

				if (npc.ai[1] <= 0f)
				{
					bool flag25 = false;
					if (flag13)
					{
						int num80 = -num11;
						if (!Collision.CanHit(npc.Center, 0, 0, npc.Center + Vector2.UnitX * num80 * 32f, 0, 0) || npc.localAI[2] == 8f)
							flag25 = true;

						if (flag25)
						{
							int num81 = NPCID.Sets.AttackTime[npc.type];
							int num82 = ((num11 == 1) ? num13 : num12);
							int num83 = ((num11 == 1) ? num12 : num13);
							if (num82 != -1 && !Collision.CanHit(npc.Center, 0, 0, Main.npc[num82].Center, 0, 0))
								num82 = ((num83 == -1 || !Collision.CanHit(npc.Center, 0, 0, Main.npc[num83].Center, 0, 0)) ? (-1) : num83);

							if (num82 != -1)
							{
								npc.ai[0] = 15f;
								npc.ai[1] = num81;
								npc.ai[2] = 0f;
								npc.localAI[3] = 0f;
								npc.direction = ((npc.position.X < Main.npc[num82].position.X) ? 1 : (-1));
								npc.netUpdate = true;
							}
							else
							{
								flag25 = false;
							}
						}
					}

					if (!flag25)
					{
						npc.ai[0] = ((npc.localAI[2] == 8f && flag13) ? 8 : 0);
						// Patch note: num74 - cooldown.
						// Patch note: maxValue4 - randExtraCooldown.
						npc.ai[1] = attackBaseCooldown + Main.rand.Next(attackRandomExtraCooldown);
						npc.ai[2] = 0f;
						npc.localAI[1] = (npc.localAI[3] = attackBaseCooldown / 2 + Main.rand.Next(attackRandomExtraCooldown));
						npc.netUpdate = true;
					}
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

			if (flag11 && npc.wet)
			{
				int num84 = (int)(npc.Center.X / 16f);
				int num85 = 5;
				if (npc.collideX || (num84 < num85 && npc.direction == -1) || (num84 > Main.maxTilesX - num85 && npc.direction == 1))
				{
					npc.direction *= -1;
					npc.velocity.X *= -0.25f;
					npc.netUpdate = true;
				}

				npc.velocity.Y *= 0.9f;
				npc.velocity.Y -= 0.5f;
				if (npc.velocity.Y < -15f)
					npc.velocity.Y = -15f;
			}

			if (flag10 && npc.wet)
			{
				if (flag7)
					npc.ai[1] = 50f;

				int num86 = (int)(npc.Center.X / 16f);
				int num87 = 5;
				if (npc.collideX || (num86 < num87 && npc.direction == -1) || (num86 > Main.maxTilesX - num87 && npc.direction == 1))
				{
					npc.direction *= -1;
					npc.velocity.X *= -0.25f;
					npc.netUpdate = true;
				}

				if (Collision.GetWaterLine(npc.Center.ToTileCoordinates(), out var waterLineHeight))
				{
					float num88 = npc.Center.Y + 1f;
					if (npc.Center.Y > waterLineHeight)
					{
						npc.velocity.Y -= 0.8f;
						if (npc.velocity.Y < -4f)
							npc.velocity.Y = -4f;

						if (num88 + npc.velocity.Y < waterLineHeight)
							npc.velocity.Y = waterLineHeight - num88;
					}
					else
					{
						npc.velocity.Y = MathHelper.Min(npc.velocity.Y, waterLineHeight - num88);
					}
				}
				else
				{
					npc.velocity.Y -= 0.2f;
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

				if (flag27 && npc.type == NPCID.Mechanic && npc.localAI[0] == 1f)
					flag27 = false;

				if (flag27 && npc.type == NPCID.Dryad)
				{
					flag27 = false;
					for (int num89 = 0; num89 < 200; num89++)
					{
						NPC nPC3 = Main.npc[num89];
						if (nPC3.active && nPC3.townNPC && !(npc.Distance(nPC3.Center) > 1200f) && nPC3.FindBuffIndex(165) == -1)
						{
							flag27 = true;
							break;
						}
					}
				}

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
				else if (!NPCID.Sets.IsTownPet[npc.type] && flag26 && npc.ai[0] == 0f && npc.velocity.Y == 0f && Main.rand.NextBool(1200) && (npc.type == NPCID.PartyGirl || (BirthdayParty.PartyIsUp && NPCID.Sets.AttackType[npc.type] == NPCID.Sets.AttackType[208])))
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
				else if (flag26 && npc.ai[0] == 0f && npc.velocity.Y == 0f && Main.rand.NextBool(600) && npc.type == NPCID.DD2Bartender)
				{
					int num104 = 300;
					int num105 = 150;
					for (int num106 = 0; num106 < 255; num106++)
					{
						Player player2 = Main.player[num106];
						if (player2.active && !player2.dead && player2.Distance(npc.Center) < (float)num105 && Collision.CanHitLine(npc.Top, 0, 0, player2.Top, 0, 0))
						{
							int num107 = (npc.position.X < player2.position.X).ToDirectionInt();
							npc.ai[0] = 18f;
							npc.ai[1] = num104;
							npc.ai[2] = num106;
							npc.direction = num107;
							npc.netUpdate = true;
							break;
						}
					}
				}
				else if (!NPCID.Sets.IsTownPet[npc.type] && flag26 && npc.ai[0] == 0f && npc.velocity.Y == 0f && Main.rand.NextBool(1800))
				{
					npc.ai[0] = 2f;
					npc.ai[1] = 45 * Main.rand.Next(1, 2);
					npc.netUpdate = true;
				}
				else if (flag26 && npc.ai[0] == 0f && npc.velocity.Y == 0f && Main.rand.NextBool(600) && npc.type == NPCID.Pirate && !flag14)
				{
					npc.ai[0] = 11f;
					npc.ai[1] = 30 * Main.rand.Next(1, 4);
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

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[0] < 2f && npc.velocity.Y == 0f && npc.type == NPCID.Nurse && npc.breath > 0)
				{
					int num114 = -1;
					for (int num115 = 0; num115 < 200; num115++)
					{
						NPC nPC6 = Main.npc[num115];
						if (nPC6.active && nPC6.townNPC && nPC6.life != nPC6.lifeMax && (num114 == -1 || nPC6.lifeMax - nPC6.life > Main.npc[num114].lifeMax - Main.npc[num114].life) && Collision.CanHitLine(npc.position, npc.width, npc.height, nPC6.position, nPC6.width, nPC6.height) && npc.Distance(nPC6.Center) < 500f)
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

				if (flag27 && npc.velocity.Y == 0f && NPCID.Sets.AttackType[npc.type] == 0 && NPCID.Sets.AttackAverageChance[npc.type] > 0 && Main.rand.NextBool(NPCID.Sets.AttackAverageChance[npc.type] * 2))
				{
					int num116 = NPCID.Sets.AttackTime[npc.type];
					int num117 = ((num11 == 1) ? num13 : num12);
					int num118 = ((num11 == 1) ? num12 : num13);
					if (num117 != -1 && !Collision.CanHit(npc.Center, 0, 0, Main.npc[num117].Center, 0, 0))
						num117 = ((num118 == -1 || !Collision.CanHit(npc.Center, 0, 0, Main.npc[num118].Center, 0, 0)) ? (-1) : num118);

					bool flag32 = num117 != -1;
					if (flag32 && npc.type == NPCID.BestiaryGirl)
						flag32 = Vector2.Distance(npc.Center, Main.npc[num117].Center) <= 50f;

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
				else if (flag27 && npc.velocity.Y == 0f && NPCID.Sets.AttackType[npc.type] == 1 && NPCID.Sets.AttackAverageChance[npc.type] > 0 && Main.rand.NextBool(NPCID.Sets.AttackAverageChance[npc.type] * 2))
				{
					int num119 = NPCID.Sets.AttackTime[npc.type];
					int num120 = ((num11 == 1) ? num13 : num12);
					int num121 = ((num11 == 1) ? num12 : num13);
					if (num120 != -1 && !Collision.CanHitLine(npc.Center, 0, 0, Main.npc[num120].Center, 0, 0))
						num120 = ((num121 == -1 || !Collision.CanHitLine(npc.Center, 0, 0, Main.npc[num121].Center, 0, 0)) ? (-1) : num121);

					if (num120 != -1)
					{
						Vector2 vector8 = npc.DirectionTo(Main.npc[num120].Center);
						if (vector8.Y <= 0.5f && vector8.Y >= -0.5f)
						{
							npc.localAI[2] = npc.ai[0];
							npc.ai[0] = 12f;
							npc.ai[1] = num119;
							npc.ai[2] = vector8.Y;
							npc.localAI[3] = 0f;
							npc.direction = ((npc.position.X < Main.npc[num120].position.X) ? 1 : (-1));
							npc.netUpdate = true;
						}
					}
				}

				if (flag27 && npc.velocity.Y == 0f && NPCID.Sets.AttackType[npc.type] == 2 && NPCID.Sets.AttackAverageChance[npc.type] > 0 && Main.rand.NextBool(NPCID.Sets.AttackAverageChance[npc.type] * 2))
				{
					int num122 = NPCID.Sets.AttackTime[npc.type];
					int num123 = ((num11 == 1) ? num13 : num12);
					int num124 = ((num11 == 1) ? num12 : num13);
					if (num123 != -1 && !Collision.CanHitLine(npc.Center, 0, 0, Main.npc[num123].Center, 0, 0))
						num123 = ((num124 == -1 || !Collision.CanHitLine(npc.Center, 0, 0, Main.npc[num124].Center, 0, 0)) ? (-1) : num124);

					if (num123 != -1)
					{
						npc.localAI[2] = npc.ai[0];
						npc.ai[0] = 14f;
						npc.ai[1] = num122;
						npc.ai[2] = 0f;
						npc.localAI[3] = 0f;
						npc.direction = ((npc.position.X < Main.npc[num123].position.X) ? 1 : (-1));
						npc.netUpdate = true;
					}
					else if (npc.type == NPCID.Dryad)
					{
						npc.localAI[2] = npc.ai[0];
						npc.ai[0] = 14f;
						npc.ai[1] = num122;
						npc.ai[2] = 0f;
						npc.localAI[3] = 0f;
						npc.netUpdate = true;
					}
				}

				if (flag27 && npc.velocity.Y == 0f && NPCID.Sets.AttackType[npc.type] == 3 && NPCID.Sets.AttackAverageChance[npc.type] > 0 && Main.rand.NextBool(NPCID.Sets.AttackAverageChance[npc.type] * 2))
				{
					int num125 = NPCID.Sets.AttackTime[npc.type];
					int num126 = ((num11 == 1) ? num13 : num12);
					int num127 = ((num11 == 1) ? num12 : num13);
					if (num126 != -1 && !Collision.CanHit(npc.Center, 0, 0, Main.npc[num126].Center, 0, 0))
						num126 = ((num127 == -1 || !Collision.CanHit(npc.Center, 0, 0, Main.npc[num127].Center, 0, 0)) ? (-1) : num127);

					if (num126 != -1)
					{
						npc.localAI[2] = npc.ai[0];
						npc.ai[0] = 15f;
						npc.ai[1] = num125;
						npc.ai[2] = 0f;
						npc.localAI[3] = 0f;
						npc.direction = ((npc.position.X < Main.npc[num126].position.X) ? 1 : (-1));
						npc.netUpdate = true;
					}
				}
			}

			if (npc.type == 681)
			{
				float R = 0f;
				float G = 0f;
				float B = 0f;
				TorchID.TorchColor(23, out R, out G, out B);
				float num128 = 0.35f;
				R *= num128;
				G *= num128;
				B *= num128;
				Lighting.AddLight(npc.Center, R, G, B);
			}

			if (npc.type == 683 || npc.type == 687)
			{
				float num129 = Utils.WrappedLerp(0.75f, 1f, (float)Main.timeForVisualEffects % 120f / 120f);
				Lighting.AddLight(npc.Center, 0.25f * num129, 0.25f * num129, 0.1f * num129);
			}
		}

		public static void AI_007_TownEntities_Shimmer_TeleportToLandingSpot(NPC npc)
		{
			Vector2? vector = AI_007_TownEntities_Shimmer_ScanForBestSpotToLandOn(npc);
			if (vector.HasValue)
			{
				Vector2 vector2 = npc.position;
				npc.position = vector.Value;
				Vector2 movementVector = npc.position - vector2;
				int num = 560;
				if (movementVector.Length() >= (float)num)
				{
					npc.ai[2] = 30f;
					ParticleOrchestrator.BroadcastParticleSpawn(ParticleOrchestraType.ShimmerTownNPCSend, new ParticleOrchestraSettings
					{
						PositionInWorld = vector2 + npc.Size / 2f,
						MovementVector = movementVector
					});
				}

				npc.netUpdate = true;
			}
		}

		public static Vector2? AI_007_TownEntities_Shimmer_ScanForBestSpotToLandOn(NPC npc)
		{
			Point point = npc.Top.ToTileCoordinates();
			int num = 30;
			Vector2? result = null;
			bool flag = npc.homeless && (npc.homeTileX == -1 || npc.homeTileY == -1);
			for (int i = 1; i < num; i += 2)
			{
				Vector2? vector = ShimmerHelper.FindSpotWithoutShimmer(npc, point.X, point.Y, i, flag);
				if (vector.HasValue)
				{
					result = vector.Value;
					break;
				}
			}

			if (!result.HasValue && npc.homeTileX != -1 && npc.homeTileY != -1)
			{
				for (int j = 1; j < num; j += 2)
				{
					Vector2? vector2 = ShimmerHelper.FindSpotWithoutShimmer(npc, npc.homeTileX, npc.homeTileY, j, flag);
					if (vector2.HasValue)
					{
						result = vector2.Value;
						break;
					}
				}
			}

			if (!result.HasValue)
			{
				int num2 = (flag ? 30 : 0);
				num = 60;
				flag = true;
				for (int k = num2; k < num; k += 2)
				{
					Vector2? vector3 = ShimmerHelper.FindSpotWithoutShimmer(npc, point.X, point.Y, k, flag);
					if (vector3.HasValue)
					{
						result = vector3.Value;
						break;
					}
				}
			}

			if (!result.HasValue && npc.homeTileX != -1 && npc.homeTileY != -1)
			{
				num = 60;
				flag = true;
				for (int l = 30; l < num; l += 2)
				{
					Vector2? vector4 = ShimmerHelper.FindSpotWithoutShimmer(npc, npc.homeTileX, npc.homeTileY, l, flag);
					if (vector4.HasValue)
					{
						result = vector4.Value;
						break;
					}
				}
			}

			return result;
		}

		public static void AI_007_TownEntities_TeleportToHome(NPC npc, int homeFloorX, int homeFloorY)
		{
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				int num;
				switch (i)
				{
					default:
						num = 1;
						break;
					case 1:
						num = -1;
						break;
					case 0:
						num = 0;
						break;
				}

				int num2 = homeFloorX + num;
				if (npc.type == NPCID.OldMan || !Collision.SolidTiles(num2 - 1, num2 + 1, homeFloorY - 3, homeFloorY - 1))
				{
					npc.velocity.X = 0f;
					npc.velocity.Y = 0f;
					npc.position.X = num2 * 16 + 8 - npc.width / 2;
					npc.position.Y = (float)(homeFloorY * 16 - npc.height) - 0.1f;
					npc.netUpdate = true;
					AI_007_TryForcingSitting(npc, homeFloorX, homeFloorY);
					flag = true;
					break;
				}
			}

			if (!flag)
			{
				npc.homeless = true;
				WorldGen.QuickFindHome(npc.whoAmI);
			}
		}

		public static void AI_007_TownEntities_GetWalkPrediction(NPC npc, int myTileX, int homeFloorX, bool canBreathUnderWater, bool currentlyDrowning, int tileX, int tileY, out bool keepwalking, out bool avoidFalling)
		{
			keepwalking = false;
			avoidFalling = true;
			bool flag = myTileX >= homeFloorX - 35 && myTileX <= homeFloorX + 35;
			if (npc.townNPC && npc.ai[1] < 30f)
			{
				keepwalking = !Utils.PlotTileLine(npc.Top, npc.Bottom, npc.width, DelegateMethods.SearchAvoidedByNPCs);
				if (!keepwalking)
				{
					Rectangle hitbox = npc.Hitbox;
					hitbox.X -= 20;
					hitbox.Width += 40;
					for (int i = 0; i < 200; i++)
					{
						if (Main.npc[i].active && Main.npc[i].friendly && i != npc.whoAmI && Main.npc[i].velocity.X == 0f && hitbox.Intersects(Main.npc[i].Hitbox))
						{
							keepwalking = true;
							break;
						}
					}
				}
			}

			if (!keepwalking && currentlyDrowning)
				keepwalking = true;

			if (avoidFalling && (NPCID.Sets.TownCritter[npc.type] || (!flag && npc.direction == Math.Sign(homeFloorX - myTileX))))
				avoidFalling = false;

			if (!avoidFalling)
				return;

			bool flag2 = false;
			Point p = default(Point);
			int num = 0;
			for (int j = -1; j <= 4; j++)
			{
				Tile tileSafely = Framing.GetTileSafely(tileX, tileY + j);
				if (tileSafely.LiquidAmount > 0)
				{
					num++;
					if (tileSafely.LiquidType == LiquidID.Lava)
					{
						flag2 = true;
						break;
					}
				}

				if (tileSafely.HasUnactuatedTile && Main.tileSolid[tileSafely.TileType])
				{
					if (num > 0)
					{
						p.X = tileX;
						p.Y = tileY + j;
					}

					avoidFalling = false;
					break;
				}
			}

			avoidFalling |= flag2;
			double num2 = Math.Ceiling((float)npc.height / 16f);
			if ((double)num >= num2)
				avoidFalling = true;

			if (!avoidFalling && p.X != 0 && p.Y != 0)
			{
				Vector2 vector = p.ToWorldCoordinates(8f, 0f) + new Vector2(-npc.width / 2, -npc.height);
				avoidFalling = Collision.DrownCollision(vector, npc.width, npc.height, 1f);
			}
		}

		public static void AI_007_AttemptToPlayIdleAnimationsForPets(NPC npc, int petIdleChance)
		{
			if (npc.velocity.X == 0f && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(petIdleChance))
			{
				int num = 3;
				if (npc.type == NPCID.TownDog)
					num = 2;

				if (NPCID.Sets.IsTownSlime[npc.type])
					num = 0;

				npc.ai[0] = ((num == 0) ? 20 : Main.rand.Next(20, 20 + num));
				npc.ai[1] = 200 + Main.rand.Next(300);
				if (npc.ai[0] == 20f && npc.type == NPCID.TownCat)
					npc.ai[1] = 500 + Main.rand.Next(200);

				if (npc.ai[0] == 21f && npc.type == NPCID.TownDog)
					npc.ai[1] = 100 + Main.rand.Next(100);

				if (npc.ai[0] == 22f && npc.type == NPCID.TownBunny)
					npc.ai[1] = 200 + Main.rand.Next(200);

				if (npc.ai[0] == 20f && NPCID.Sets.IsTownSlime[npc.type])
					npc.ai[1] = 180 + Main.rand.Next(240);

				npc.ai[2] = 0f;
				npc.localAI[3] = 0f;
				npc.netUpdate = true;
			}
		}

		public static bool AI_007_TownEntities_IsInAGoodRestingSpot(NPC npc, int tileX, int tileY, int idealRestX, int idealRestY)
		{
			if (!Main.dayTime && npc.ai[0] == 5f)
			{
				if (Math.Abs(tileX - idealRestX) <= 7)
					return Math.Abs(tileY - idealRestY) <= 7;

				return false;
			}

			if ((npc.type == NPCID.Frog || npc.type == NPCID.GoldFrog || npc.type == NPCID.BoundTownSlimeYellow) && npc.wet)
				return false;

			if (tileX == idealRestX)
				return tileY == idealRestY;

			return false;
		}

		public static void AI_007_FindGoodRestingSpot(NPC npc, int myTileX, int myTileY, out int floorX, out int floorY)
		{
			floorX = npc.homeTileX;
			floorY = npc.homeTileY;
			if (floorX == -1 || floorY == -1)
				return;

			while (!WorldGen.SolidOrSlopedTile(floorX, floorY) && floorY < Main.maxTilesY - 20)
			{
				floorY++;
			}

			if (Main.dayTime || (npc.ai[0] == 5f && Math.Abs(myTileX - floorX) < 7 && Math.Abs(myTileY - floorY) < 7))
				return;

			Point point = new Point(floorX, floorY);
			Point point2 = new Point(-1, -1);
			int num = -1;
			if (npc.type == NPCID.TownDog || npc.type == NPCID.TownBunny || NPCID.Sets.IsTownSlime[npc.type] || npc.ai[0] == 5f)
				return;

			int num2 = 7;
			int num3 = 6;
			int num4 = 1;
			int num5 = 1;
			int num6 = 1;
			for (int i = point.X - num2; i <= point.X + num2; i += num5)
			{
				for (int num7 = point.Y + num4; num7 >= point.Y - num3; num7 -= num6)
				{
					Tile tile = Main.tile[i, num7];

					if (tile != null && tile.HasTile && TileID.Sets.CanBeSatOnForNPCs[tile.TileType])
					{
						int num8 = Math.Abs(i - point.X) + Math.Abs(num7 - point.Y);
						if (num == -1 || num8 < num)
						{
							num = num8;
							point2.X = i;
							point2.Y = num7;
						}
					}
				}
			}

			if (num == -1)
				return;

			Tile tile2 = Main.tile[point2.X, point2.Y];
			if (tile2.TileType == 497 || tile2.TileType == 15)
			{
				// Extra patch context.
				if (tile2.TileFrameY % 40 != 0)
					point2.Y--;

				point2.Y += 2;
			}
			//TML: This check is necessary as in this case vanilla changes (to vanilla tiles that aren't sittable by default) by the hook should not take effect.
			else if (tile2.TileType >= TileID.Count)
			{
				var info = new TileRestingInfo(npc, point2, Vector2.Zero, npc.direction);
				TileLoader.ModifySittingTargetInfo(point2.X, point2.Y, tile2.TileType, ref info);

				point2 = info.AnchorTilePosition;
				point2.Y += 1; // Set to tile *below* chair
			}

			for (int j = 0; j < 200; j++)
			{
				if (Main.npc[j].active && Main.npc[j].aiStyle == 7 && Main.npc[j].townNPC && Main.npc[j].ai[0] == 5f && (Main.npc[j].Bottom + Vector2.UnitY * -2f).ToTileCoordinates() == point2)
					return;
			}

			floorX = point2.X;
			floorY = point2.Y;
		}

		public static void AI_007_TryForcingSitting(NPC npc, int homeFloorX, int homeFloorY)
		{
			Tile tile = Main.tile[homeFloorX, homeFloorY - 1];
			bool flag = !NPCID.Sets.CannotSitOnFurniture[npc.type] && !NPCID.Sets.IsTownSlime[npc.type] && npc.ai[0] != 5f;

			if (flag)
				flag &= tile != null && tile.HasTile && TileID.Sets.CanBeSatOnForNPCs[tile.TileType];

			if (flag)
				flag &= tile.TileType != 15 || tile.TileFrameY < 1080 || tile.TileFrameY > 1098;

			if (flag)
			{
				Point point = (npc.Bottom + Vector2.UnitY * -2f).ToTileCoordinates();
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && Main.npc[i].aiStyle == 7 && Main.npc[i].townNPC && Main.npc[i].ai[0] == 5f && (Main.npc[i].Bottom + Vector2.UnitY * -2f).ToTileCoordinates() == point)
					{
						flag = false;
						break;
					}
				}
			}

			if (flag)
			{
				npc.ai[0] = 5f;
				npc.ai[1] = 900 + Main.rand.Next(10800);

				npc.SitDown(new Point(homeFloorX, homeFloorY - 1), out int targetDirection, out var bottom);

				npc.direction = targetDirection;
				npc.Bottom = bottom;

				npc.velocity = Vector2.Zero;
				npc.localAI[3] = 0f;
				npc.netUpdate = true;
			}
		}
	}
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.NPCs.Vanilla.NPCAIReference
{
	public static class FighterAIReference
	{
		public static bool VanillaFighterAI(NPC npc)
		{
			if (Main.player[npc.target].position.Y + (float)Main.player[npc.target].height == npc.position.Y + (float)npc.height)
				npc.directionY = -1;

			bool flag = false;
			if (npc.type == NPCID.Gnome && npc.AI_003_Gnomes_ShouldTurnToStone())
			{
				int num = (int)(npc.Center.X / 16f);
				int num2 = (int)(npc.Bottom.Y / 16f);
				npc.position += npc.netOffset;
				int num3 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.TintableDustLighted, 0f, 0f, 254, Color.White, 0.5f);
				Main.dust[num3].velocity *= 0.2f;
				npc.position -= npc.netOffset;
				if (WorldGen.SolidTileAllowBottomSlope(num, num2))
				{
					for (int i = 0; i < 5; i++)
					{
						npc.position += npc.netOffset;
						int num4 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.TintableDustLighted, 0f, 0f, 254, Color.White, 0.5f);
						Main.dust[num4].velocity *= 0.2f;
						npc.position -= npc.netOffset;
					}

					if (Main.netMode != NetmodeID.MultiplayerClient && TileObject.CanPlace(num, num2 - 1, 567, 0, npc.direction, out var _, onlyCheck: true) && WorldGen.PlaceTile(num, num2 - 1, 567, mute: false, forced: false, -1, Main.rand.Next(5)))
					{
						if (Main.netMode == NetmodeID.Server)
							NetMessage.SendTileSquare(-1, num, num2 - 2, 1, 2);

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							if (npc.IsNPCValidForBestiaryKillCredit())
								Main.BestiaryTracker.Kills.RegisterKill(npc);

							MethodInfo endOngoingTorchGodEventMethod = npc.GetType().GetMethod("CountKillForBannersAndDropThem", BindingFlags.Instance | BindingFlags.NonPublic);
							endOngoingTorchGodEventMethod.Invoke(npc, null);
						}

						npc.life = 0;
						npc.active = false;
						AchievementsHelper.NotifyProgressionEvent(24);
						return false;
					}
				}
			}

			if (npc.type == 466)
			{
				int num5 = 200;
				if (npc.ai[2] == 0f)
				{
					npc.alpha = num5;
					npc.TargetClosest();
					if (!Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() < 170f)
						npc.ai[2] = -16f;

					if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 2f || npc.justHit)
						npc.ai[2] = -16f;

					return false;
				}

				if (npc.ai[2] < 0f)
				{
					if (npc.alpha > 0)
					{
						npc.alpha -= num5 / 16;
						if (npc.alpha < 0)
							npc.alpha = 0;
					}

					npc.ai[2] += 1f;
					if (npc.ai[2] == 0f)
					{
						npc.ai[2] = 1f;
						npc.velocity.X = npc.direction * 2;
					}

					return false;
				}

				npc.alpha = 0;
			}

			if (npc.type == 166)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(240))
				{
					npc.ai[2] = Main.rand.Next(-480, -60);
					npc.netUpdate = true;
				}

				if (npc.ai[2] < 0f)
				{
					npc.TargetClosest();
					if (npc.justHit)
						npc.ai[2] = 0f;

					if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
						npc.ai[2] = 0f;
				}

				if (npc.ai[2] < 0f)
				{
					npc.velocity.X *= 0.9f;
					if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
						npc.velocity.X = 0f;

					npc.ai[2] += 1f;
					if (npc.ai[2] == 0f)
						npc.velocity.X = (float)npc.direction * 0.1f;

					return false;
				}
			}

			if (npc.type == 461)
			{
				if (npc.wet)
				{
					npc.knockBackResist = 0f;
					npc.ai[3] = -0.10101f;
					npc.noGravity = true;
					Vector2 center = npc.Center;
					npc.width = 34;
					npc.height = 24;
					npc.position.X = center.X - (float)(npc.width / 2);
					npc.position.Y = center.Y - (float)(npc.height / 2);
					npc.TargetClosest();
					if (npc.collideX)
						npc.velocity.X = 0f - npc.oldVelocity.X;

					if (npc.velocity.X < 0f)
						npc.direction = -1;

					if (npc.velocity.X > 0f)
						npc.direction = 1;

					if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].Center, 1, 1))
					{
						Vector2 vector = Main.player[npc.target].Center - npc.Center;
						vector.Normalize();
						vector *= 5f;
						npc.velocity = (npc.velocity * 19f + vector) / 20f;
						return false;
					}

					float num6 = 5f;
					if (npc.velocity.Y > 0f)
						num6 = 3f;

					if (npc.velocity.Y < 0f)
						num6 = 8f;

					Vector2 vector2 = new Vector2(npc.direction, -1f);
					vector2.Normalize();
					vector2 *= num6;
					if (num6 < 5f)
						npc.velocity = (npc.velocity * 24f + vector2) / 25f;
					else
						npc.velocity = (npc.velocity * 9f + vector2) / 10f;

					return false;
				}

				npc.knockBackResist = 0.4f * Main.GameModeInfo.KnockbackToEnemiesMultiplier;
				npc.noGravity = false;
				Vector2 center2 = npc.Center;
				npc.width = 18;
				npc.height = 40;
				npc.position.X = center2.X - (float)(npc.width / 2);
				npc.position.Y = center2.Y - (float)(npc.height / 2);
				if (npc.ai[3] == -0.10101f)
				{
					npc.ai[3] = 0f;
					float num7 = npc.velocity.Length();
					num7 *= 2f;
					if (num7 > 10f)
						num7 = 10f;

					npc.velocity.Normalize();
					npc.velocity *= num7;
					if (npc.velocity.X < 0f)
						npc.direction = -1;

					if (npc.velocity.X > 0f)
						npc.direction = 1;

					npc.spriteDirection = npc.direction;
				}
			}

			if (npc.type == 586)
			{
				if (npc.alpha == 255)
				{
					npc.TargetClosest();
					npc.spriteDirection = npc.direction;
					npc.velocity.Y = -6f;
					npc.netUpdate = true;
					for (int j = 0; j < 35; j++)
					{
						Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
						dust.velocity *= 1f;
						dust.scale = 1f + Main.rand.NextFloat() * 0.5f;
						dust.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
						dust.velocity += npc.velocity * 0.5f;
					}
				}

				npc.alpha -= 15;
				if (npc.alpha < 0)
					npc.alpha = 0;

				npc.position += npc.netOffset;
				if (npc.alpha != 0)
				{
					for (int k = 0; k < 2; k++)
					{
						Dust dust2 = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
						dust2.velocity *= 1f;
						dust2.scale = 1f + Main.rand.NextFloat() * 0.5f;
						dust2.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
						dust2.velocity += npc.velocity * 0.3f;
					}
				}

				if (Main.rand.Next(3) == 0)
				{
					Dust dust3 = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
					dust3.velocity *= 0f;
					dust3.alpha = 120;
					dust3.scale = 0.7f + Main.rand.NextFloat() * 0.5f;
					dust3.velocity += npc.velocity * 0.3f;
				}

				npc.position -= npc.netOffset;
				if (npc.wet)
				{
					npc.knockBackResist = 0f;
					npc.ai[3] = -0.10101f;
					npc.noGravity = true;
					Vector2 center3 = npc.Center;
					npc.position.X = center3.X - (float)(npc.width / 2);
					npc.position.Y = center3.Y - (float)(npc.height / 2);
					npc.TargetClosest();
					if (npc.collideX)
						npc.velocity.X = 0f - npc.oldVelocity.X;

					if (npc.velocity.X < 0f)
						npc.direction = -1;

					if (npc.velocity.X > 0f)
						npc.direction = 1;

					if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].Center, 1, 1))
					{
						Vector2 vector3 = Main.player[npc.target].Center - npc.Center;
						vector3.Normalize();
						float num8 = 1f;
						num8 += Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) / 40f;
						num8 = MathHelper.Clamp(num8, 5f, 20f);
						vector3 *= num8;
						if (npc.velocity.Y > 0f)
							npc.velocity = (npc.velocity * 29f + vector3) / 30f;
						else
							npc.velocity = (npc.velocity * 4f + vector3) / 5f;

						return false;
					}

					float num9 = 5f;
					if (npc.velocity.Y > 0f)
						num9 = 3f;

					if (npc.velocity.Y < 0f)
						num9 = 8f;

					Vector2 vector4 = new Vector2(npc.direction, -1f);
					vector4.Normalize();
					vector4 *= num9;
					if (num9 < 5f)
						npc.velocity = (npc.velocity * 24f + vector4) / 25f;
					else
						npc.velocity = (npc.velocity * 9f + vector4) / 10f;

					return false;
				}

				npc.noGravity = false;
				Vector2 center4 = npc.Center;
				npc.position.X = center4.X - (float)(npc.width / 2);
				npc.position.Y = center4.Y - (float)(npc.height / 2);
				if (npc.ai[3] == -0.10101f)
				{
					npc.ai[3] = 0f;
					float num10 = npc.velocity.Length();
					num10 *= 2f;
					if (num10 > 15f)
						num10 = 15f;

					npc.velocity.Normalize();
					npc.velocity *= num10;
					if (npc.velocity.X < 0f)
						npc.direction = -1;

					if (npc.velocity.X > 0f)
						npc.direction = 1;

					npc.spriteDirection = npc.direction;
				}
			}

			if (npc.type == 379 || npc.type == 380)
			{
				if (npc.ai[3] < 0f)
				{
					npc.directionY = -1;
					flag = false;
					npc.damage = 0;
					npc.velocity.X *= 0.93f;
					if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
						npc.velocity.X = 0f;

					int num11 = (int)(0f - npc.ai[3] - 1f);
					int num12 = Math.Sign(Main.npc[num11].Center.X - npc.Center.X);
					if (num12 != npc.direction)
					{
						npc.velocity.X = 0f;
						npc.direction = num12;
						npc.netUpdate = true;
					}

					if (npc.justHit && Main.netMode != NetmodeID.MultiplayerClient && Main.npc[num11].localAI[0] == 0f)
						Main.npc[num11].localAI[0] = 1f;

					if (npc.ai[0] < 1000f)
						npc.ai[0] = 1000f;

					if ((npc.ai[0] += 1f) >= 1300f)
					{
						npc.ai[0] = 1000f;
						npc.netUpdate = true;
					}

					return false;
				}

				if (npc.ai[0] >= 1000f)
					npc.ai[0] = 0f;

				npc.damage = npc.defDamage;
			}

			if (npc.type == 383 && npc.ai[2] == 0f && npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
			{
				int num13 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, 384, npc.whoAmI);
				npc.ai[2] = num13 + 1;
				npc.localAI[0] = -1f;
				npc.netUpdate = true;
				Main.npc[num13].ai[0] = npc.whoAmI;
				Main.npc[num13].netUpdate = true;
			}

			if (npc.type == 383)
			{
				int num14 = (int)npc.ai[2] - 1;
				if (num14 != -1 && Main.npc[num14].active && Main.npc[num14].type == 384)
				{
					npc.dontTakeDamage = true;
				}
				else
				{
					npc.dontTakeDamage = false;
					npc.ai[2] = 0f;
					if (npc.localAI[0] == -1f)
						npc.localAI[0] = 180f;

					if (npc.localAI[0] > 0f)
						npc.localAI[0] -= 1f;
				}
			}

			if (npc.type == 482)
			{
				int num15 = 300;
				int num16 = 120;
				npc.dontTakeDamage = false;
				if (npc.ai[2] < 0f)
				{
					npc.dontTakeDamage = true;
					npc.ai[2] += 1f;
					npc.velocity.X *= 0.9f;
					if ((double)Math.Abs(npc.velocity.X) < 0.001)
						npc.velocity.X = 0.001f * (float)npc.direction;

					if (Math.Abs(npc.velocity.Y) > 1f)
						npc.ai[2] += 10f;

					if (npc.ai[2] >= 0f)
					{
						npc.netUpdate = true;
						npc.velocity.X += (float)npc.direction * 0.3f;
					}

					return false;
				}

				if (npc.ai[2] < (float)num15)
				{
					if (npc.justHit)
						npc.ai[2] += 15f;

					npc.ai[2] += 1f;
				}
				else if (npc.velocity.Y == 0f)
				{
					npc.ai[2] = -num16;
					npc.netUpdate = true;
				}
			}

			if (npc.type == 631)
			{
				if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
					npc.TargetClosest(npc.ai[2] > 0f);

				Player player = Main.player[npc.target];
				bool flag2 = !player.dead && player.active && npc.Center.Distance(player.Center) < 320f;
				int num17 = 100;
				int num18 = 32;
				if (npc.ai[2] == 0f)
				{
					npc.ai[3] = 65f;
					if (flag2 && Collision.CanHit(player, npc))
					{
						npc.ai[2] = num17;
						npc.ai[3] = 0f;
						npc.velocity.X = (float)npc.direction * 0.01f;
						npc.netUpdate = true;
					}
				}
				else
				{
					if (npc.ai[2] < (float)num17)
					{
						npc.ai[2] += 1f;
						npc.velocity.X *= 0.9f;
						if ((double)Math.Abs(npc.velocity.X) < 0.001)
							npc.velocity.X = 0f;

						if (Math.Abs(npc.velocity.Y) > 1f)
							npc.ai[2] = 0f;

						if (npc.ai[2] == (float)(num17 - num18 / 2) && Main.netMode != NetmodeID.MultiplayerClient && !player.Hitbox.Intersects(npc.Hitbox) && Collision.CanHit(player, npc))
						{
							float num19 = 8f;
							Vector2 center5 = npc.Center;
							Vector2 vector5 = npc.DirectionTo(Main.player[npc.target].Center) * num19;
							if (vector5.HasNaNs())
								vector5 = new Vector2((float)npc.direction * num19, 0f);

							int num20 = 20;
							Vector2 v = vector5 + Utils.RandomVector2(Main.rand, -0.8f, 0.8f);
							v = v.SafeNormalize(Vector2.Zero);
							v *= num19;
							Projectile.NewProjectile(npc.GetSource_FromAI(), center5.X, center5.Y, v.X, v.Y, 909, num20, 1f, Main.myPlayer);
						}

						if (npc.ai[2] >= (float)num17)
						{
							npc.ai[2] = num17;
							npc.ai[3] = 0f;
							npc.velocity.X = (float)npc.direction * 0.01f;
							npc.netUpdate = true;
						}

						return false;
					}

					if (npc.velocity.Y == 0f && flag2 && (player.Hitbox.Intersects(npc.Hitbox) || Collision.CanHit(player, npc)))
					{
						npc.ai[2] = num17 - num18;
						npc.netUpdate = true;
					}
				}
			}

			if (npc.type == 480)
			{
				int num21 = 180;
				int num22 = 300;
				int num23 = 180;
				int num24 = 60;
				int num25 = 20;
				if (npc.life < npc.lifeMax / 3)
				{
					num21 = 120;
					num22 = 240;
					num23 = 240;
					num24 = 90;
				}

				if (npc.ai[2] > 0f)
				{
					npc.ai[2] -= 1f;
				}
				else if (npc.ai[2] == 0f)
				{
					if (((Main.player[npc.target].Center.X < npc.Center.X && npc.direction < 0) || (Main.player[npc.target].Center.X > npc.Center.X && npc.direction > 0)) && npc.velocity.Y == 0f && npc.Distance(Main.player[npc.target].Center) < 900f && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
					{
						npc.ai[2] = -num23 - num25;
						npc.netUpdate = true;
					}
				}
				else
				{
					if (npc.ai[2] < 0f && npc.ai[2] < (float)(-num23))
					{
						npc.position += npc.netOffset;
						npc.velocity.X *= 0.9f;
						if (npc.velocity.Y < -2f || npc.velocity.Y > 4f || npc.justHit)
						{
							npc.ai[2] = num21;
						}
						else
						{
							npc.ai[2] += 1f;
							if (npc.ai[2] == 0f)
								npc.ai[2] = num22;
						}

						float num26 = npc.ai[2] + (float)num23 + (float)num25;
						if (num26 == 1f)
							SoundEngine.PlaySound(SoundID.NPCDeath17, npc.position);

						if (num26 < (float)num25)
						{
							Vector2 vector6 = npc.Top + new Vector2(npc.spriteDirection * 6, 6f);
							float num27 = MathHelper.Lerp(20f, 30f, (num26 * 3f + 50f) / 182f);
							Main.rand.NextFloat();
							for (float num28 = 0f; num28 < 2f; num28 += 1f)
							{
								Vector2 vector7 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * (Main.rand.NextFloat() * 0.5f + 0.5f);
								Dust obj = Main.dust[Dust.NewDust(vector6, 0, 0, 228)];
								obj.position = vector6 + vector7 * num27;
								obj.noGravity = true;
								obj.velocity = vector7 * 2f;
								obj.scale = 0.5f + Main.rand.NextFloat() * 0.5f;
							}
						}

						Lighting.AddLight(npc.Center, 0.9f, 0.75f, 0.1f);
						npc.position -= npc.netOffset;
						return false;
					}

					if (npc.ai[2] < 0f && npc.ai[2] >= (float)(-num23))
					{
						npc.position += npc.netOffset;
						Lighting.AddLight(npc.Center, 0.9f, 0.75f, 0.1f);
						npc.velocity.X *= 0.9f;
						if (npc.velocity.Y < -2f || npc.velocity.Y > 4f || npc.justHit)
						{
							npc.ai[2] = num21;
						}
						else
						{
							npc.ai[2] += 1f;
							if (npc.ai[2] == 0f)
								npc.ai[2] = num22;
						}

						float num29 = npc.ai[2] + (float)num23;
						if (num29 < 180f && (Main.rand.Next(3) == 0 || npc.ai[2] % 3f == 0f))
						{
							Vector2 vector8 = npc.Top + new Vector2(npc.spriteDirection * 10, 10f);
							float num30 = MathHelper.Lerp(20f, 30f, (num29 * 3f + 50f) / 182f);
							Main.rand.NextFloat();
							for (float num31 = 0f; num31 < 1f; num31 += 1f)
							{
								Vector2 vector9 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * (Main.rand.NextFloat() * 0.5f + 0.5f);
								Dust obj2 = Main.dust[Dust.NewDust(vector8, 0, 0, 228)];
								obj2.position = vector8 + vector9 * num30;
								obj2.noGravity = true;
								obj2.velocity = vector9 * 4f;
								obj2.scale = 0.5f + Main.rand.NextFloat();
							}
						}

						npc.position -= npc.netOffset;
						if (Main.netMode == NetmodeID.Server)
							return false;

						Player player2 = Main.player[Main.myPlayer];
						_ = Main.myPlayer;
						if (player2.dead || !player2.active || player2.FindBuffIndex(156) != -1)
							return false;

						Vector2 vector10 = player2.Center - npc.Center;
						if (!(vector10.Length() < 700f))
							return false;

						bool flag3 = vector10.Length() < 30f;
						if (!flag3)
						{
							float x = ((float)Math.PI / 4f).ToRotationVector2().X;
							Vector2 vector11 = Vector2.Normalize(vector10);
							if (vector11.X > x || vector11.X < 0f - x)
								flag3 = true;
						}

						if (((player2.Center.X < npc.Center.X && npc.direction < 0 && player2.direction > 0) || (player2.Center.X > npc.Center.X && npc.direction > 0 && player2.direction < 0)) && flag3 && (Collision.CanHitLine(npc.Center, 1, 1, player2.Center, 1, 1) || Collision.CanHitLine(npc.Center - Vector2.UnitY * 16f, 1, 1, player2.Center, 1, 1) || Collision.CanHitLine(npc.Center + Vector2.UnitY * 8f, 1, 1, player2.Center, 1, 1)) && !player2.creativeGodMode)
							player2.AddBuff(156, num24 + (int)npc.ai[2] * -1);

						return false;
					}
				}
			}

			if (npc.type == 471)
			{
				if (npc.ai[3] < 0f)
				{
					npc.knockBackResist = 0f;
					npc.defense = (int)((double)npc.defDefense * 1.1);
					npc.noGravity = true;
					npc.noTileCollide = true;
					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else if (npc.velocity.X > 0f)
						npc.direction = 1;

					npc.rotation = npc.velocity.X * 0.1f;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[3] += 1f;
						if (npc.localAI[3] > (float)Main.rand.Next(20, 180))
						{
							npc.localAI[3] = 0f;
							Vector2 center6 = npc.Center;
							center6 += npc.velocity;
							NPC.NewNPC(npc.GetSource_FromAI(), (int)center6.X, (int)center6.Y, 30);
						}
					}
				}
				else
				{
					npc.localAI[3] = 0f;
					npc.knockBackResist = 0.35f * Main.GameModeInfo.KnockbackToEnemiesMultiplier;
					npc.rotation *= 0.9f;
					npc.defense = npc.defDefense;
					npc.noGravity = false;
					npc.noTileCollide = false;
				}

				if (npc.ai[3] == 1f)
				{
					npc.knockBackResist = 0f;
					npc.defense += 10;
				}

				if (npc.ai[3] == -1f)
				{
					npc.TargetClosest();
					float num32 = 8f;
					float num33 = 40f;
					Vector2 vector12 = Main.player[npc.target].Center - npc.Center;
					float num34 = vector12.Length();
					num32 += num34 / 200f;
					vector12.Normalize();
					vector12 *= num32;
					npc.velocity = (npc.velocity * (num33 - 1f) + vector12) / num33;
					if (num34 < 500f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.ai[3] = 0f;
						npc.ai[2] = 0f;
					}

					return false;
				}

				if (npc.ai[3] == -2f)
				{
					npc.velocity.Y -= 0.2f;
					if (npc.velocity.Y < -10f)
						npc.velocity.Y = -10f;

					if (Main.player[npc.target].Center.Y - npc.Center.Y > 200f)
					{
						npc.TargetClosest();
						npc.ai[3] = -3f;
						if (Main.player[npc.target].Center.X > npc.Center.X)
							npc.ai[2] = 1f;
						else
							npc.ai[2] = -1f;
					}

					npc.velocity.X *= 0.99f;
					return false;
				}

				if (npc.ai[3] == -3f)
				{
					if (npc.direction == 0)
						npc.TargetClosest();

					if (npc.ai[2] == 0f)
						npc.ai[2] = npc.direction;

					npc.velocity.Y *= 0.9f;
					npc.velocity.X += npc.ai[2] * 0.3f;
					if (npc.velocity.X > 10f)
						npc.velocity.X = 10f;

					if (npc.velocity.X < -10f)
						npc.velocity.X = -10f;

					float num35 = Main.player[npc.target].Center.X - npc.Center.X;
					if ((npc.ai[2] < 0f && num35 > 300f) || (npc.ai[2] > 0f && num35 < -300f))
					{
						npc.ai[3] = -4f;
						npc.ai[2] = 0f;
					}
					else if (Math.Abs(num35) > 800f)
					{
						npc.ai[3] = -1f;
						npc.ai[2] = 0f;
					}

					return false;
				}

				if (npc.ai[3] == -4f)
				{
					npc.ai[2] += 1f;
					npc.velocity.Y += 0.1f;
					if (npc.velocity.Length() > 4f)
						npc.velocity *= 0.9f;

					int num36 = (int)npc.Center.X / 16;
					int num37 = (int)(npc.position.Y + (float)npc.height + 12f) / 16;
					bool flag4 = false;
					for (int l = num36 - 1; l <= num36 + 1; l++)
					{
						if (Main.tile[l, num37].HasTile && Main.tileSolid[Main.tile[l, num37].TileType])
							flag4 = true;
					}

					if (flag4 && !Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.ai[3] = 0f;
						npc.ai[2] = 0f;
					}
					else if (npc.ai[2] > 300f || npc.Center.Y > Main.player[npc.target].Center.Y + 200f)
					{
						npc.ai[3] = -1f;
						npc.ai[2] = 0f;
					}
				}
				else
				{
					if (npc.ai[3] == 1f)
					{
						Vector2 center7 = npc.Center;
						center7.Y -= 70f;
						npc.velocity.X *= 0.8f;
						npc.ai[2] += 1f;
						if (npc.ai[2] == 60f)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
								NPC.NewNPC(npc.GetSource_FromAI(), (int)center7.X, (int)center7.Y + 18, 472);
						}
						else if (npc.ai[2] >= 90f)
						{
							npc.ai[3] = -2f;
							npc.ai[2] = 0f;
						}

						for (int m = 0; m < 2; m++)
						{
							Vector2 vector13 = center7;
							Vector2 vector14 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
							vector14.Normalize();
							vector14 *= (float)Main.rand.Next(0, 100) * 0.1f;
							Vector2 vector15 = vector13 + vector14;
							vector14.Normalize();
							vector14 *= (float)Main.rand.Next(50, 90) * 0.1f;
							int num38 = Dust.NewDust(vector15, 1, 1, 27);
							Main.dust[num38].velocity = -vector14 * 0.3f;
							Main.dust[num38].alpha = 100;
							if (Main.rand.Next(2) == 0)
							{
								Main.dust[num38].noGravity = true;
								Main.dust[num38].scale += 0.3f;
							}
						}

						return false;
					}

					npc.ai[2] += 1f;
					int num39 = 10;
					if (npc.velocity.Y == 0f && NPC.CountNPCS(472) < num39)
					{
						if (npc.ai[2] >= 180f)
						{
							npc.ai[2] = 0f;
							npc.ai[3] = 1f;
						}
					}
					else
					{
						if (NPC.CountNPCS(472) >= num39)
							npc.ai[2] += 1f;

						if (npc.ai[2] >= 360f)
						{
							npc.ai[2] = 0f;
							npc.ai[3] = -2f;
							npc.velocity.Y -= 3f;
						}
					}

					if (npc.target >= 0 && !Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() > 800f)
					{
						npc.ai[3] = -1f;
						npc.ai[2] = 0f;
					}
				}

				if (Main.player[npc.target].dead)
				{
					npc.TargetClosest();
					if (Main.player[npc.target].dead)
						npc.EncourageDespawn(1);
				}
			}

			if (npc.type == 419)
			{
				npc.reflectsProjectiles = false;
				npc.takenDamageMultiplier = 1f;
				int num40 = 6;
				int num41 = 10;
				float num42 = 16f;
				if (npc.ai[2] > 0f)
					npc.ai[2] -= 1f;

				if (npc.ai[2] == 0f)
				{
					if (((Main.player[npc.target].Center.X < npc.Center.X && npc.direction < 0) || (Main.player[npc.target].Center.X > npc.Center.X && npc.direction > 0)) && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
					{
						npc.ai[2] = -1f;
						npc.netUpdate = true;
						npc.TargetClosest();
					}
				}
				else
				{
					if (npc.ai[2] < 0f && npc.ai[2] > (float)(-num40))
					{
						npc.ai[2] -= 1f;
						npc.velocity.X *= 0.9f;
						return false;
					}

					if (npc.ai[2] == (float)(-num40))
					{
						npc.ai[2] -= 1f;
						npc.TargetClosest();
						Vector2 vector16 = npc.DirectionTo(Main.player[npc.target].Top + new Vector2(0f, -30f));
						if (vector16.HasNaNs())
							vector16 = Vector2.Normalize(new Vector2(npc.spriteDirection, -1f));

						npc.velocity = vector16 * num42;
						npc.netUpdate = true;
						return false;
					}

					if (npc.ai[2] < (float)(-num40))
					{
						npc.ai[2] -= 1f;
						if (npc.velocity.Y == 0f)
						{
							npc.ai[2] = 60f;
						}
						else if (npc.ai[2] < (float)(-num40 - num41))
						{
							npc.velocity.Y += 0.15f;
							if (npc.velocity.Y > 24f)
								npc.velocity.Y = 24f;
						}

						npc.reflectsProjectiles = true;
						npc.takenDamageMultiplier = 3f;
						if (npc.justHit)
						{
							npc.ai[2] = 60f;
							npc.netUpdate = true;
						}

						return false;
					}
				}
			}

			if (npc.type == 415)
			{
				int num43 = 42;
				int num44 = 18;
				if (npc.justHit)
				{
					npc.ai[2] = 120f;
					npc.netUpdate = true;
				}

				if (npc.ai[2] > 0f)
					npc.ai[2] -= 1f;

				if (npc.ai[2] == 0f)
				{
					int num45 = 0;
					for (int n = 0; n < 200; n++)
					{
						if (Main.npc[n].active && Main.npc[n].type == 516)
							num45++;
					}

					if (num45 > 6)
					{
						npc.ai[2] = 90f;
					}
					else if (((Main.player[npc.target].Center.X < npc.Center.X && npc.direction < 0) || (Main.player[npc.target].Center.X > npc.Center.X && npc.direction > 0)) && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
					{
						npc.ai[2] = -1f;
						npc.netUpdate = true;
						npc.TargetClosest();
					}
				}
				else if (npc.ai[2] < 0f && npc.ai[2] > (float)(-num43))
				{
					npc.ai[2] -= 1f;
					if (npc.ai[2] == (float)(-num43))
						npc.ai[2] = 180 + 30 * Main.rand.Next(10);

					npc.velocity.X *= 0.8f;
					if (npc.ai[2] == (float)(-num44) || npc.ai[2] == (float)(-num44 - 8) || npc.ai[2] == (float)(-num44 - 16))
					{
						npc.position += npc.netOffset;
						for (int num46 = 0; num46 < 20; num46++)
						{
							Vector2 vector17 = npc.Center + Vector2.UnitX * npc.spriteDirection * 40f;
							Dust obj3 = Main.dust[Dust.NewDust(vector17, 0, 0, 259)];
							Vector2 vector18 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
							obj3.position = vector17 + vector18 * 4f;
							obj3.velocity = vector18 * 2f + Vector2.UnitX * Main.rand.NextFloat() * npc.spriteDirection * 3f;
							obj3.scale = 0.3f + vector18.X * (float)(-npc.spriteDirection);
							obj3.fadeIn = 0.7f;
							obj3.noGravity = true;
						}

						npc.position -= npc.netOffset;
						if (npc.velocity.X > -0.5f && npc.velocity.X < 0.5f)
							npc.velocity.X = 0f;

						if (Main.netMode != NetmodeID.MultiplayerClient)
							NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X + npc.spriteDirection * 45, (int)npc.Center.Y + 8, 516, 0, 0f, 0f, 0f, 0f, npc.target);
					}

					return false;
				}
			}

			if (npc.type == 428)
			{
				npc.localAI[0] += 1f;
				if (npc.localAI[0] >= 300f)
				{
					int num47 = (int)npc.Center.X / 16 - 1;
					int num48 = (int)npc.Center.Y / 16 - 1;
					if (!Collision.SolidTiles(num47, num47 + 2, num48, num48 + 1) && Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.Transform(427);
						npc.life = npc.lifeMax;
						npc.localAI[0] = 0f;
						return false;
					}
				}

				int num49 = 0;
				num49 = ((npc.localAI[0] < 60f) ? 16 : ((npc.localAI[0] < 120f) ? 8 : ((npc.localAI[0] < 180f) ? 4 : ((npc.localAI[0] < 240f) ? 2 : ((!(npc.localAI[0] < 300f)) ? 1 : 1)))));
				if (Main.rand.Next(num49) == 0)
				{
					npc.position += npc.netOffset;
					Dust dust4 = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 229)];
					dust4.noGravity = true;
					dust4.scale = 1f;
					dust4.noLight = true;
					dust4.velocity = npc.DirectionFrom(dust4.position) * dust4.velocity.Length();
					dust4.position -= dust4.velocity * 5f;
					dust4.position.X += npc.direction * 6;
					dust4.position.Y += 4f;
					npc.position -= npc.netOffset;
				}
			}

			if (npc.type == 427)
			{
				npc.localAI[0] += 1f;
				npc.localAI[0] += Math.Abs(npc.velocity.X) / 2f;
				if (npc.localAI[0] >= 1200f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					int num50 = (int)npc.Center.X / 16 - 2;
					int num51 = (int)npc.Center.Y / 16 - 3;
					if (!Collision.SolidTiles(num50, num50 + 4, num51, num51 + 4))
					{
						npc.Transform(426);
						npc.life = npc.lifeMax;
						npc.localAI[0] = 0f;
						return false;
					}
				}

				int num52 = 0;
				num52 = ((npc.localAI[0] < 360f) ? 32 : ((npc.localAI[0] < 720f) ? 16 : ((npc.localAI[0] < 1080f) ? 6 : ((npc.localAI[0] < 1440f) ? 2 : ((!(npc.localAI[0] < 1800f)) ? 1 : 1)))));
				if (Main.rand.Next(num52) == 0)
				{
					npc.position += npc.netOffset;
					Dust obj4 = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 229)];
					obj4.noGravity = true;
					obj4.scale = 1f;
					obj4.noLight = true;
					npc.position -= npc.netOffset;
				}
			}

			if (npc.type == 590)
			{
				npc.position += npc.netOffset;
				int num53 = (int)(npc.position.Y + 6f) / 16;
				if (npc.spriteDirection < 0)
				{
					int num54 = (int)(npc.Center.X - 22f) / 16;
					Tile tileSafely = Framing.GetTileSafely(num54, num53);
					Tile tileSafely2 = Framing.GetTileSafely(num54 + 1, num53);
					if (WorldGen.InWorld(num54, num53) && tileSafely2.LiquidAmount == 0 && tileSafely.LiquidAmount == 0)
					{
						Lighting.AddLight(num54, num53, 1f, 0.95f, 0.8f);
						if (Main.rand.Next(30) == 0)
							Dust.NewDust(new Vector2(npc.Center.X - 22f, npc.position.Y + 6f), 1, 1, 6);
					}
				}
				else
				{
					int num55 = (int)(npc.Center.X + 14f) / 16;
					Tile tileSafely3 = Framing.GetTileSafely(num55, num53);
					Tile tileSafely4 = Framing.GetTileSafely(num55 - 1, num53);
					if (WorldGen.InWorld(num55, num53) && tileSafely4.LiquidAmount == 0 && tileSafely3.LiquidAmount == 0)
					{
						Lighting.AddLight(num55, num53, 1f, 0.95f, 0.8f);
						if (Main.rand.Next(30) == 0)
							Dust.NewDust(new Vector2(npc.Center.X + 14f, npc.position.Y + 6f), 1, 1, 6);
					}
				}

				npc.position -= npc.netOffset;
			}
			else if (npc.type == 591)
			{
				npc.position += npc.netOffset;
				if (!npc.wet)
				{
					if (npc.spriteDirection < 0)
					{
						Lighting.AddLight(new Vector2(npc.Center.X - 36f, npc.position.Y + 24f), 1f, 0.95f, 0.8f);
						if (npc.ai[2] == 0f && Main.rand.Next(30) == 0)
							Dust.NewDust(new Vector2(npc.Center.X - 36f, npc.position.Y + 24f), 1, 1, 6);
					}
					else
					{
						Lighting.AddLight(new Vector2(npc.Center.X + 28f, npc.position.Y + 24f), 1f, 0.95f, 0.8f);
						if (npc.ai[2] == 0f && Main.rand.Next(30) == 0)
							Dust.NewDust(new Vector2(npc.Center.X + 28f, npc.position.Y + 24f), 1, 1, 6);
					}
				}

				npc.position -= npc.netOffset;
			}

			bool flag5 = false;
			bool flag6 = false;
			if (npc.velocity.X == 0f)
				flag6 = true;

			if (npc.justHit)
				flag6 = false;

			if (Main.netMode != NetmodeID.MultiplayerClient && npc.type == NPCID.Lihzahrd && (double)npc.life <= (double)npc.lifeMax * 0.55)
				npc.Transform(199);

			if (Main.netMode != NetmodeID.MultiplayerClient && npc.type == 348 && (double)npc.life <= (double)npc.lifeMax * 0.55)
				npc.Transform(349);

			int num56 = 60;
			if (npc.type == 120)
			{
				num56 = 180;
				if (npc.ai[3] == -120f)
				{
					npc.velocity *= 0f;
					npc.ai[3] = 0f;
					npc.position += npc.netOffset;
					SoundEngine.PlaySound(SoundID.Item8, npc.position);
					Vector2 vector19 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num57 = npc.oldPos[2].X + (float)npc.width * 0.5f - vector19.X;
					float num58 = npc.oldPos[2].Y + (float)npc.height * 0.5f - vector19.Y;
					float num59 = (float)Math.Sqrt(num57 * num57 + num58 * num58);
					num59 = 2f / num59;
					num57 *= num59;
					num58 *= num59;
					for (int num60 = 0; num60 < 20; num60++)
					{
						int num61 = Dust.NewDust(npc.position, npc.width, npc.height, 71, num57, num58, 200, default(Color), 2f);
						Main.dust[num61].noGravity = true;
						Main.dust[num61].velocity.X *= 2f;
					}

					for (int num62 = 0; num62 < 20; num62++)
					{
						int num63 = Dust.NewDust(npc.oldPos[2], npc.width, npc.height, 71, 0f - num57, 0f - num58, 200, default(Color), 2f);
						Main.dust[num63].noGravity = true;
						Main.dust[num63].velocity.X *= 2f;
					}

					npc.position -= npc.netOffset;
				}
			}

			bool flag7 = false;
			bool flag8 = true;
			if (npc.type == 343 || npc.type == 47 || npc.type == 67 || npc.type == 109 || npc.type == 110 || npc.type == 111 || npc.type == 120 || npc.type == 163 || npc.type == 164 || npc.type == 239 || npc.type == 168 || npc.type == 199 || npc.type == 206 || npc.type == 214 || npc.type == 215 || npc.type == 216 || npc.type == 217 || npc.type == 218 || npc.type == 219 || npc.type == 220 || npc.type == 226 || npc.type == 243 || npc.type == 251 || npc.type == 257 || npc.type == 258 || npc.type == 290 || npc.type == 291 || npc.type == 292 || npc.type == 293 || npc.type == 305 || npc.type == 306 || npc.type == 307 || npc.type == 308 || npc.type == 309 || npc.type == 348 || npc.type == 349 || npc.type == 350 || npc.type == 351 || npc.type == 379 || (npc.type >= 430 && npc.type <= 436) || npc.type == 591 || npc.type == 380 || npc.type == 381 || npc.type == 382 || npc.type == 383 || npc.type == 386 || npc.type == 391 || (npc.type >= 449 && npc.type <= 452) || npc.type == 466 || npc.type == 464 || npc.type == 166 || npc.type == 469 || npc.type == 468 || npc.type == 471 || npc.type == 470 || npc.type == 480 || npc.type == 481 || npc.type == 482 || npc.type == 411 || npc.type == 424 || npc.type == 409 || (npc.type >= 494 && npc.type <= 506) || npc.type == 425 || npc.type == 427 || npc.type == 426 || npc.type == 428 || npc.type == 580 || npc.type == 508 || npc.type == 415 || npc.type == 419 || npc.type == 520 || (npc.type >= 524 && npc.type <= 527) || npc.type == 528 || npc.type == 529 || npc.type == 530 || npc.type == 532 || npc.type == 582 || npc.type == 624 || npc.type == 631)
				flag8 = false;

			bool flag9 = false;
			int num64 = npc.type;
			if (num64 == 425 || num64 == 471)
				flag9 = true;

			bool flag10 = true;
			switch (npc.type)
			{
				case 110:
				case 111:
				case 206:
				case 214:
				case 215:
				case 216:
				case 291:
				case 292:
				case 293:
				case 350:
				case 379:
				case 380:
				case 381:
				case 382:
				case 409:
				case 411:
				case 424:
				case 426:
				case 466:
				case 498:
				case 499:
				case 500:
				case 501:
				case 502:
				case 503:
				case 504:
				case 505:
				case 506:
				case 520:
					if (npc.ai[2] > 0f)
						flag10 = false;
					break;
			}

			if (!flag9 && flag10)
			{
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
			}

			if (npc.type == 463 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (npc.localAI[3] > 0f)
					npc.localAI[3] -= 1f;

				if (npc.justHit && npc.localAI[3] <= 0f && Main.rand.Next(3) == 0)
				{
					npc.localAI[3] = 30f;
					int num65 = Main.rand.Next(3, 6);
					int[] array = new int[num65];
					int num66 = 0;
					for (int num67 = 0; num67 < 255; num67++)
					{
						if (Main.player[num67].active && !Main.player[num67].dead && Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[num67].position, Main.player[num67].width, Main.player[num67].height))
						{
							array[num66] = num67;
							num66++;
							if (num66 == num65)
								break;
						}
					}

					if (num66 > 1)
					{
						for (int num68 = 0; num68 < 100; num68++)
						{
							int num69 = Main.rand.Next(num66);
							int num70;
							for (num70 = num69; num70 == num69; num70 = Main.rand.Next(num66))
							{
							}

							int num71 = array[num69];
							array[num69] = array[num70];
							array[num70] = num71;
						}
					}

					Vector2 vector20 = new Vector2(-1f, -1f);
					for (int num72 = 0; num72 < num66; num72++)
					{
						Vector2 vector21 = Main.npc[array[num72]].Center - npc.Center;
						vector21.Normalize();
						vector20 += vector21;
					}

					vector20.Normalize();
					for (int num73 = 0; num73 < num65; num73++)
					{
						float num74 = Main.rand.Next(8, 13);
						Vector2 vector22 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
						vector22.Normalize();
						if (num66 > 0)
						{
							vector22 += vector20;
							vector22.Normalize();
						}

						vector22 *= num74;
						if (num66 > 0)
						{
							num66--;
							vector22 = Main.player[array[num66]].Center - npc.Center;
							vector22.Normalize();
							vector22 *= num74;
						}

						Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.position.Y + (float)(npc.width / 4), vector22.X, vector22.Y, 498, (int)((double)npc.damage * 0.15), 1f, Main.myPlayer);
					}
				}
			}

			if (npc.type == 460)
			{
				if (npc.velocity.Y < 0f - npc.gravity || npc.velocity.Y > npc.gravity)
					npc.knockBackResist = 0f;
				else
					npc.knockBackResist = 0.25f * Main.GameModeInfo.KnockbackToEnemiesMultiplier;
			}

			if (npc.type == 469)
			{
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
			}

			if (npc.type == 462 && npc.velocity.Y == 0f && (Main.player[npc.target].Center - npc.Center).Length() < 150f && Math.Abs(npc.velocity.X) > 3f && ((npc.velocity.X < 0f && npc.Center.X > Main.player[npc.target].Center.X) || (npc.velocity.X > 0f && npc.Center.X < Main.player[npc.target].Center.X)))
			{
				flag = true;
				npc.velocity.X *= 1.75f;
				npc.velocity.Y -= 4.5f;
				if (npc.Center.Y - Main.player[npc.target].Center.Y > 20f)
					npc.velocity.Y -= 0.5f;

				if (npc.Center.Y - Main.player[npc.target].Center.Y > 40f)
					npc.velocity.Y -= 1f;

				if (npc.Center.Y - Main.player[npc.target].Center.Y > 80f)
					npc.velocity.Y -= 1.5f;

				if (npc.Center.Y - Main.player[npc.target].Center.Y > 100f)
					npc.velocity.Y -= 1.5f;

				if (Math.Abs(npc.velocity.X) > 7f)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = -7f;
					else
						npc.velocity.X = 7f;
				}
			}

			if (npc.type == 624 && npc.target < 255)
			{
				if (!Main.remixWorld && !Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					npc.ai[3] = num56;
					npc.directionY = -1;
					if (npc.type == 624 && !npc.AI_003_Gnomes_ShouldTurnToStone() && (npc.Center - Main.player[npc.target].Center).Length() > 500f)
					{
						npc.velocity.X *= 0.95f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							npc.velocity.X = 0f;

						return false;
					}
				}
				else if (Main.player[npc.target].Center.Y > npc.Center.Y - 128f)
				{
					npc.ai[3] = 0f;
				}
			}

			if (npc.ai[3] < (float)num56 && NPC.DespawnEncouragement_AIStyle3_Fighters_NotDiscouraged(npc.type, npc.position, npc))
			{
				if (npc.shimmerTransparency < 1f)
				{
					if ((npc.type == 3 || npc.type == 591 || npc.type == 590 || npc.type == 331 || npc.type == 332 || npc.type == 21 || (npc.type >= 449 && npc.type <= 452) || npc.type == 31 || npc.type == 294 || npc.type == 295 || npc.type == 296 || npc.type == 77 || npc.type == 110 || npc.type == 132 || npc.type == 167 || npc.type == 161 || npc.type == 162 || npc.type == 186 || npc.type == 187 || npc.type == 188 || npc.type == 189 || npc.type == 197 || npc.type == 200 || npc.type == 201 || npc.type == 202 || npc.type == 203 || npc.type == 223 || npc.type == 291 || npc.type == 292 || npc.type == 293 || npc.type == 320 || npc.type == 321 || npc.type == 319 || npc.type == 481 || npc.type == 632 || npc.type == 635) && Main.rand.Next(1000) == 0)
						SoundEngine.PlaySound(SoundID.ZombieMoan, npc.position);

					if ((npc.type == 489 || npc.type == 586) && Main.rand.Next(800) == 0)
						SoundEngine.PlaySound(SoundID.BloodZombie, npc.position);

					if ((npc.type == 78 || npc.type == 79 || npc.type == 80 || npc.type == 630) && Main.rand.Next(500) == 0)
						SoundEngine.PlaySound(SoundID.Mummy, npc.position);

					if (npc.type == 159 && Main.rand.Next(500) == 0)
						SoundEngine.PlaySound(SoundID.Zombie7, npc.position);

					if (npc.type == 162 && Main.rand.Next(500) == 0)
						SoundEngine.PlaySound(SoundID.Zombie6, npc.position);

					if (npc.type == 181 && Main.rand.Next(500) == 0)
						SoundEngine.PlaySound(SoundID.Zombie8, npc.position);

					if (npc.type >= 269 && npc.type <= 280 && Main.rand.Next(1000) == 0)
						SoundEngine.PlaySound(SoundID.ZombieMoan, npc.position);
				}

				npc.TargetClosest();
				if (npc.directionY > 0 && Main.player[npc.target].Center.Y <= npc.Bottom.Y)
					npc.directionY = -1;
			}
			else if (!(npc.ai[2] > 0f) || !NPC.DespawnEncouragement_AIStyle3_Fighters_CanBeBusyWithAction(npc.type))
			{
				if (Main.IsItDay() && (double)(npc.position.Y / 16f) < Main.worldSurface && npc.type != 624 && npc.type != 631)
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

			if (npc.type == 159 || npc.type == 349)
			{
				if (npc.type == 159 && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
					npc.velocity.X *= 0.95f;

				if (npc.velocity.X < -6f || npc.velocity.X > 6f)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (npc.velocity.X < 6f && npc.direction == 1)
				{
					if (npc.velocity.Y == 0f && npc.velocity.X < 0f)
						npc.velocity.X *= 0.99f;

					npc.velocity.X += 0.07f;
					if (npc.velocity.X > 6f)
						npc.velocity.X = 6f;
				}
				else if (npc.velocity.X > -6f && npc.direction == -1)
				{
					if (npc.velocity.Y == 0f && npc.velocity.X > 0f)
						npc.velocity.X *= 0.99f;

					npc.velocity.X -= 0.07f;
					if (npc.velocity.X < -6f)
						npc.velocity.X = -6f;
				}
			}
			else if (npc.type == 199)
			{
				if (npc.velocity.X < -4f || npc.velocity.X > 4f)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (npc.velocity.X < 4f && npc.direction == 1)
				{
					if (npc.velocity.Y == 0f && npc.velocity.X < 0f)
						npc.velocity.X *= 0.8f;

					npc.velocity.X += 0.1f;
					if (npc.velocity.X > 4f)
						npc.velocity.X = 4f;
				}
				else if (npc.velocity.X > -4f && npc.direction == -1)
				{
					if (npc.velocity.Y == 0f && npc.velocity.X > 0f)
						npc.velocity.X *= 0.8f;

					npc.velocity.X -= 0.1f;
					if (npc.velocity.X < -4f)
						npc.velocity.X = -4f;
				}
			}
			else if (npc.type == 120 || npc.type == 166 || npc.type == 213 || npc.type == 258 || npc.type == 528 || npc.type == 529)
			{
				if (npc.velocity.X < -3f || npc.velocity.X > 3f)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (npc.velocity.X < 3f && npc.direction == 1)
				{
					if (npc.velocity.Y == 0f && npc.velocity.X < 0f)
						npc.velocity.X *= 0.99f;

					npc.velocity.X += 0.07f;
					if (npc.velocity.X > 3f)
						npc.velocity.X = 3f;
				}
				else if (npc.velocity.X > -3f && npc.direction == -1)
				{
					if (npc.velocity.Y == 0f && npc.velocity.X > 0f)
						npc.velocity.X *= 0.99f;

					npc.velocity.X -= 0.07f;
					if (npc.velocity.X < -3f)
						npc.velocity.X = -3f;
				}
			}
			else if (npc.type == 461 || npc.type == 27 || npc.type == 77 || npc.type == 104 || npc.type == 163 || npc.type == 162 || npc.type == 196 || npc.type == 197 || npc.type == 212 || npc.type == 257 || npc.type == 326 || npc.type == 343 || npc.type == 348 || npc.type == 351 || (npc.type >= 524 && npc.type <= 527) || npc.type == 530 || npc.type == 236)
			{
				if (npc.velocity.X < -2f || npc.velocity.X > 2f)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (npc.velocity.X < 2f && npc.direction == 1)
				{
					npc.velocity.X += 0.07f;
					if (npc.velocity.X > 2f)
						npc.velocity.X = 2f;
				}
				else if (npc.velocity.X > -2f && npc.direction == -1)
				{
					npc.velocity.X -= 0.07f;
					if (npc.velocity.X < -2f)
						npc.velocity.X = -2f;
				}
			}
			else if (npc.type == 109)
			{
				if (npc.velocity.X < -2f || npc.velocity.X > 2f)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (npc.velocity.X < 2f && npc.direction == 1)
				{
					npc.velocity.X += 0.04f;
					if (npc.velocity.X > 2f)
						npc.velocity.X = 2f;
				}
				else if (npc.velocity.X > -2f && npc.direction == -1)
				{
					npc.velocity.X -= 0.04f;
					if (npc.velocity.X < -2f)
						npc.velocity.X = -2f;
				}
			}
			else if (npc.type == 21 || npc.type == 26 || npc.type == 31 || npc.type == 294 || npc.type == 295 || npc.type == 296 || npc.type == 47 || npc.type == 73 || npc.type == 140 || npc.type == 164 || npc.type == 239 || npc.type == 167 || npc.type == 168 || npc.type == 185 || npc.type == 198 || npc.type == 201 || npc.type == 202 || npc.type == 203 || npc.type == 217 || npc.type == 218 || npc.type == 219 || npc.type == 226 || npc.type == 181 || npc.type == 254 || npc.type == 338 || npc.type == 339 || npc.type == 340 || npc.type == 342 || npc.type == 385 || npc.type == 389 || npc.type == 462 || npc.type == 463 || npc.type == 466 || npc.type == 464 || npc.type == 469 || npc.type == 470 || npc.type == 480 || npc.type == 482 || npc.type == 425 || npc.type == 429 || npc.type == 586 || npc.type == 631 || npc.type == 635)
			{
				float num80 = 1.5f;
				if (npc.type == 181 && Main.remixWorld)
					num80 = 3.75f;
				else if (npc.type == 294)
					num80 = 2f;
				else if (npc.type == 295)
					num80 = 1.75f;
				else if (npc.type == 296)
					num80 = 1.25f;
				else if (npc.type == 201)
					num80 = 1.1f;
				else if (npc.type == 202)
					num80 = 0.9f;
				else if (npc.type == 203)
					num80 = 1.2f;
				else if (npc.type == 338)
					num80 = 1.75f;
				else if (npc.type == 339)
					num80 = 1.25f;
				else if (npc.type == 340)
					num80 = 2f;
				else if (npc.type == 385)
					num80 = 1.8f;
				else if (npc.type == 389)
					num80 = 2.25f;
				else if (npc.type == 462)
					num80 = 4f;
				else if (npc.type == 463)
					num80 = 0.75f;
				else if (npc.type == 466)
					num80 = 3.75f;
				else if (npc.type == 469)
					num80 = 3.25f;
				else if (npc.type == 480)
					num80 = 1.5f + (1f - (float)npc.life / (float)npc.lifeMax) * 2f;
				else if (npc.type == 425)
					num80 = 6f;
				else if (npc.type == 429)
					num80 = 4f;
				else if (npc.type == 631)
					num80 = 0.9f;
				else if (npc.type == 586)
					num80 = 1.5f + (1f - (float)npc.life / (float)npc.lifeMax) * 3.5f;

				if (npc.type == 21 || npc.type == 201 || npc.type == 202 || npc.type == 203 || npc.type == 342 || npc.type == 635)
					num80 *= 1f + (1f - npc.scale);

				if (npc.velocity.X < 0f - num80 || npc.velocity.X > num80)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (npc.velocity.X < num80 && npc.direction == 1)
				{
					if (npc.type == 466 && npc.velocity.X < -2f)
						npc.velocity.X *= 0.9f;

					if (npc.type == 586 && npc.velocity.Y == 0f && npc.velocity.X < -1f)
						npc.velocity.X *= 0.9f;

					npc.velocity.X += 0.07f;
					if (npc.velocity.X > num80)
						npc.velocity.X = num80;
				}
				else if (npc.velocity.X > 0f - num80 && npc.direction == -1)
				{
					if (npc.type == 466 && npc.velocity.X > 2f)
						npc.velocity.X *= 0.9f;

					if (npc.type == 586 && npc.velocity.Y == 0f && npc.velocity.X > 1f)
						npc.velocity.X *= 0.9f;

					npc.velocity.X -= 0.07f;
					if (npc.velocity.X < 0f - num80)
						npc.velocity.X = 0f - num80;
				}

				if (npc.velocity.Y == 0f && npc.type == 462 && ((npc.direction > 0 && npc.velocity.X < 0f) || (npc.direction < 0 && npc.velocity.X > 0f)))
					npc.velocity.X *= 0.9f;
			}
			else if (npc.type >= 269 && npc.type <= 280)
			{
				float num81 = 1.5f;
				if (npc.type == 269)
					num81 = 2f;

				if (npc.type == 270)
					num81 = 1f;

				if (npc.type == 271)
					num81 = 1.5f;

				if (npc.type == 272)
					num81 = 3f;

				if (npc.type == 273)
					num81 = 1.25f;

				if (npc.type == 274)
					num81 = 3f;

				if (npc.type == 275)
					num81 = 3.25f;

				if (npc.type == 276)
					num81 = 2f;

				if (npc.type == 277)
					num81 = 2.75f;

				if (npc.type == 278)
					num81 = 1.8f;

				if (npc.type == 279)
					num81 = 1.3f;

				if (npc.type == 280)
					num81 = 2.5f;

				num81 *= 1f + (1f - npc.scale);
				if (npc.velocity.X < 0f - num81 || npc.velocity.X > num81)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (npc.velocity.X < num81 && npc.direction == 1)
				{
					npc.velocity.X += 0.07f;
					if (npc.velocity.X > num81)
						npc.velocity.X = num81;
				}
				else if (npc.velocity.X > 0f - num81 && npc.direction == -1)
				{
					npc.velocity.X -= 0.07f;
					if (npc.velocity.X < 0f - num81)
						npc.velocity.X = 0f - num81;
				}
			}
			else if (npc.type >= 305 && npc.type <= 314)
			{
				float num82 = 1.5f;
				if (npc.type == 305 || npc.type == 310)
					num82 = 2f;

				if (npc.type == 306 || npc.type == 311)
					num82 = 1.25f;

				if (npc.type == 307 || npc.type == 312)
					num82 = 2.25f;

				if (npc.type == 308 || npc.type == 313)
					num82 = 1.5f;

				if (npc.type == 309 || npc.type == 314)
					num82 = 1f;

				if (npc.type < 310)
				{
					if (npc.velocity.Y == 0f)
					{
						npc.velocity.X *= 0.85f;
						if ((double)npc.velocity.X > -0.3 && (double)npc.velocity.X < 0.3)
						{
							flag = true;
							npc.velocity.Y = -7f;
							npc.velocity.X = num82 * (float)npc.direction;
						}
					}
					else if (npc.spriteDirection == npc.direction)
					{
						npc.velocity.X = (npc.velocity.X * 10f + num82 * (float)npc.direction) / 11f;
					}
				}
				else if (npc.velocity.X < 0f - num82 || npc.velocity.X > num82)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (npc.velocity.X < num82 && npc.direction == 1)
				{
					npc.velocity.X += 0.07f;
					if (npc.velocity.X > num82)
						npc.velocity.X = num82;
				}
				else if (npc.velocity.X > 0f - num82 && npc.direction == -1)
				{
					npc.velocity.X -= 0.07f;
					if (npc.velocity.X < 0f - num82)
						npc.velocity.X = 0f - num82;
				}
			}
			else if (npc.type == 67 || npc.type == 220 || npc.type == 428)
			{
				if (npc.velocity.X < -0.5f || npc.velocity.X > 0.5f)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.7f;
				}
				else if (npc.velocity.X < 0.5f && npc.direction == 1)
				{
					npc.velocity.X += 0.03f;
					if (npc.velocity.X > 0.5f)
						npc.velocity.X = 0.5f;
				}
				else if (npc.velocity.X > -0.5f && npc.direction == -1)
				{
					npc.velocity.X -= 0.03f;
					if (npc.velocity.X < -0.5f)
						npc.velocity.X = -0.5f;
				}
			}
			else if (npc.type == 78 || npc.type == 79 || npc.type == 80 || npc.type == 630)
			{
				float num83 = 1f;
				float num84 = 0.05f;
				if (npc.life < npc.lifeMax / 2)
				{
					num83 = 2f;
					num84 = 0.1f;
				}

				if (npc.type == 79 || npc.type == 630)
					num83 *= 1.5f;

				if (npc.velocity.X < 0f - num83 || npc.velocity.X > num83)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.7f;
				}
				else if (npc.velocity.X < num83 && npc.direction == 1)
				{
					npc.velocity.X += num84;
					if (npc.velocity.X > num83)
						npc.velocity.X = num83;
				}
				else if (npc.velocity.X > 0f - num83 && npc.direction == -1)
				{
					npc.velocity.X -= num84;
					if (npc.velocity.X < 0f - num83)
						npc.velocity.X = 0f - num83;
				}
			}
			else if (npc.type == 287)
			{
				float num85 = 5f;
				float num86 = 0.2f;
				if (npc.velocity.X < 0f - num85 || npc.velocity.X > num85)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.7f;
				}
				else if (npc.velocity.X < num85 && npc.direction == 1)
				{
					npc.velocity.X += num86;
					if (npc.velocity.X > num85)
						npc.velocity.X = num85;
				}
				else if (npc.velocity.X > 0f - num85 && npc.direction == -1)
				{
					npc.velocity.X -= num86;
					if (npc.velocity.X < 0f - num85)
						npc.velocity.X = 0f - num85;
				}
			}
			else if (npc.type == 243)
			{
				float num87 = 1f;
				float num88 = 0.07f;
				num87 += (1f - (float)npc.life / (float)npc.lifeMax) * 1.5f;
				num88 += (1f - (float)npc.life / (float)npc.lifeMax) * 0.15f;
				if (npc.velocity.X < 0f - num87 || npc.velocity.X > num87)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.7f;
				}
				else if (npc.velocity.X < num87 && npc.direction == 1)
				{
					npc.velocity.X += num88;
					if (npc.velocity.X > num87)
						npc.velocity.X = num87;
				}
				else if (npc.velocity.X > 0f - num87 && npc.direction == -1)
				{
					npc.velocity.X -= num88;
					if (npc.velocity.X < 0f - num87)
						npc.velocity.X = 0f - num87;
				}
			}
			else if (npc.type == 251)
			{
				float num89 = 1f;
				float num90 = 0.08f;
				num89 += (1f - (float)npc.life / (float)npc.lifeMax) * 2f;
				num90 += (1f - (float)npc.life / (float)npc.lifeMax) * 0.2f;
				if (npc.velocity.X < 0f - num89 || npc.velocity.X > num89)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.7f;
				}
				else if (npc.velocity.X < num89 && npc.direction == 1)
				{
					npc.velocity.X += num90;
					if (npc.velocity.X > num89)
						npc.velocity.X = num89;
				}
				else if (npc.velocity.X > 0f - num89 && npc.direction == -1)
				{
					npc.velocity.X -= num90;
					if (npc.velocity.X < 0f - num89)
						npc.velocity.X = 0f - num89;
				}
			}
			else if (npc.type == 386)
			{
				if (npc.ai[2] > 0f)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity.X *= 0.8f;
				}
				else
				{
					float num91 = 0.15f;
					float num92 = 1.5f;
					if (npc.velocity.X < 0f - num92 || npc.velocity.X > num92)
					{
						if (npc.velocity.Y == 0f)
							npc.velocity *= 0.7f;
					}
					else if (npc.velocity.X < num92 && npc.direction == 1)
					{
						npc.velocity.X += num91;
						if (npc.velocity.X > num92)
							npc.velocity.X = num92;
					}
					else if (npc.velocity.X > 0f - num92 && npc.direction == -1)
					{
						npc.velocity.X -= num91;
						if (npc.velocity.X < 0f - num92)
							npc.velocity.X = 0f - num92;
					}
				}
			}
			else if (npc.type == 460)
			{
				float num93 = 3f;
				float num94 = 0.1f;
				if (Math.Abs(npc.velocity.X) > 2f)
					num94 *= 0.8f;

				if ((double)Math.Abs(npc.velocity.X) > 2.5)
					num94 *= 0.8f;

				if (Math.Abs(npc.velocity.X) > 3f)
					num94 *= 0.8f;

				if ((double)Math.Abs(npc.velocity.X) > 3.5)
					num94 *= 0.8f;

				if (Math.Abs(npc.velocity.X) > 4f)
					num94 *= 0.8f;

				if ((double)Math.Abs(npc.velocity.X) > 4.5)
					num94 *= 0.8f;

				if (Math.Abs(npc.velocity.X) > 5f)
					num94 *= 0.8f;

				if ((double)Math.Abs(npc.velocity.X) > 5.5)
					num94 *= 0.8f;

				num93 += (1f - (float)npc.life / (float)npc.lifeMax) * 3f;
				if (npc.velocity.X < 0f - num93 || npc.velocity.X > num93)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.7f;
				}
				else if (npc.velocity.X < num93 && npc.direction == 1)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X *= 0.93f;

					npc.velocity.X += num94;
					if (npc.velocity.X > num93)
						npc.velocity.X = num93;
				}
				else if (npc.velocity.X > 0f - num93 && npc.direction == -1)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X *= 0.93f;

					npc.velocity.X -= num94;
					if (npc.velocity.X < 0f - num93)
						npc.velocity.X = 0f - num93;
				}
			}
			else if (npc.type == 508 || npc.type == 580 || npc.type == 582)
			{
				float num95 = 2.5f;
				float num96 = 10f;
				float num97 = Math.Abs(npc.velocity.X);
				if (npc.type == 582)
				{
					num95 = 2.25f;
					num96 = 7f;
					if (num97 > 2.5f)
					{
						num95 = 3f;
						num96 += 75f;
					}
					else if (num97 > 2f)
					{
						num95 = 2.75f;
						num96 += 55f;
					}
				}
				else if (num97 > 2.75f)
				{
					num95 = 3.5f;
					num96 += 80f;
				}
				else if ((double)num97 > 2.25)
				{
					num95 = 3f;
					num96 += 60f;
				}

				if ((double)Math.Abs(npc.velocity.Y) < 0.5)
				{
					if (npc.velocity.X > 0f && npc.direction < 0)
						npc.velocity *= 0.95f;

					if (npc.velocity.X < 0f && npc.direction > 0)
						npc.velocity *= 0.95f;
				}

				if (Math.Abs(npc.velocity.Y) > npc.gravity)
				{
					float num98 = 3f;
					if (npc.type == 582)
						num98 = 2f;

					num96 *= num98;
				}

				if (npc.velocity.X <= 0f && npc.direction < 0)
				{
					npc.velocity.X = (npc.velocity.X * num96 - num95) / (num96 + 1f);
				}
				else if (npc.velocity.X >= 0f && npc.direction > 0)
				{
					npc.velocity.X = (npc.velocity.X * num96 + num95) / (num96 + 1f);
				}
				else if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 20f && Math.Abs(npc.velocity.Y) <= npc.gravity)
				{
					npc.velocity.X *= 0.99f;
					npc.velocity.X += (float)npc.direction * 0.025f;
				}
			}
			else if (npc.type == 391 || npc.type == 427 || npc.type == 415 || npc.type == 419 || npc.type == 518 || npc.type == 532)
			{
				float num99 = 5f;
				float num100 = 0.25f;
				float num101 = 0.7f;
				if (npc.type == 427)
				{
					num99 = 6f;
					num100 = 0.2f;
					num101 = 0.8f;
				}
				else if (npc.type == 415)
				{
					num99 = 4f;
					num100 = 0.1f;
					num101 = 0.95f;
				}
				else if (npc.type == 419)
				{
					num99 = 6f;
					num100 = 0.15f;
					num101 = 0.85f;
				}
				else if (npc.type == 518)
				{
					num99 = 5f;
					num100 = 0.1f;
					num101 = 0.95f;
				}
				else if (npc.type == 532)
				{
					num99 = 5f;
					num100 = 0.15f;
					num101 = 0.98f;
				}

				if (npc.velocity.X < 0f - num99 || npc.velocity.X > num99)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= num101;
				}
				else if (npc.velocity.X < num99 && npc.direction == 1)
				{
					npc.velocity.X += num100;
					if (npc.velocity.X > num99)
						npc.velocity.X = num99;
				}
				else if (npc.velocity.X > 0f - num99 && npc.direction == -1)
				{
					npc.velocity.X -= num100;
					if (npc.velocity.X < 0f - num99)
						npc.velocity.X = 0f - num99;
				}
			}
			else if ((npc.type >= 430 && npc.type <= 436) || npc.type == 494 || npc.type == 495 || npc.type == 591)
			{
				if (npc.ai[2] == 0f)
				{
					npc.damage = npc.defDamage;
					float num102 = 1f;
					num102 *= 1f + (1f - npc.scale);
					if (npc.velocity.X < 0f - num102 || npc.velocity.X > num102)
					{
						if (npc.velocity.Y == 0f)
							npc.velocity *= 0.8f;
					}
					else if (npc.velocity.X < num102 && npc.direction == 1)
					{
						npc.velocity.X += 0.07f;
						if (npc.velocity.X > num102)
							npc.velocity.X = num102;
					}
					else if (npc.velocity.X > 0f - num102 && npc.direction == -1)
					{
						npc.velocity.X -= 0.07f;
						if (npc.velocity.X < 0f - num102)
							npc.velocity.X = 0f - num102;
					}

					if (npc.velocity.Y == 0f && (!Main.IsItDay() || (double)npc.position.Y > Main.worldSurface * 16.0) && !Main.player[npc.target].dead)
					{
						Vector2 vector24 = npc.Center - Main.player[npc.target].Center;
						int num103 = 50;
						if (npc.type >= 494 && npc.type <= 495)
							num103 = 42;

						if (vector24.Length() < (float)num103 && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
						{
							npc.velocity.X *= 0.7f;
							npc.ai[2] = 1f;
						}
					}
				}
				else
				{
					npc.damage = (int)((double)npc.defDamage * 1.5);
					npc.ai[3] = 1f;
					npc.velocity.X *= 0.9f;
					if ((double)Math.Abs(npc.velocity.X) < 0.1)
						npc.velocity.X = 0f;

					npc.ai[2] += 1f;
					if (npc.ai[2] >= 20f || npc.velocity.Y != 0f || (Main.IsItDay() && (double)npc.position.Y < Main.worldSurface * 16.0))
						npc.ai[2] = 0f;
				}
			}
			else if (npc.type != 110 && npc.type != 111 && npc.type != 206 && npc.type != 214 && npc.type != 215 && npc.type != 216 && npc.type != 290 && npc.type != 291 && npc.type != 292 && npc.type != 293 && npc.type != 350 && npc.type != 379 && npc.type != 380 && npc.type != 381 && npc.type != 382 && (npc.type < 449 || npc.type > 452) && npc.type != 468 && npc.type != 481 && npc.type != 411 && npc.type != 409 && (npc.type < 498 || npc.type > 506) && npc.type != 424 && npc.type != 426 && npc.type != 520)
			{
				float num104 = 1f;
				if (npc.type == 624)
					num104 = 2.5f;

				if (npc.type == 186)
					num104 = 1.1f;

				if (npc.type == 187)
					num104 = 0.9f;

				if (npc.type == 188)
					num104 = 1.2f;

				if (npc.type == 189)
					num104 = 0.8f;

				if (npc.type == 132)
					num104 = 0.95f;

				if (npc.type == 200)
					num104 = 0.87f;

				if (npc.type == 223)
					num104 = 1.05f;

				if (npc.type == 632)
					num104 = 0.8f;

				if (npc.type == 489)
				{
					float num105 = (Main.player[npc.target].Center - npc.Center).Length();
					num105 *= 0.0025f;
					if ((double)num105 > 1.5)
						num105 = 1.5f;

					num104 = ((!Main.expertMode) ? (2.5f - num105) : (3f - num105));
					num104 *= 0.8f;
				}

				if (npc.type == 489 || npc.type == 3 || npc.type == 132 || npc.type == 186 || npc.type == 187 || npc.type == 188 || npc.type == 189 || npc.type == 200 || npc.type == 223 || npc.type == 331 || npc.type == 332)
					num104 *= 1f + (1f - npc.scale);

				if (npc.velocity.X < 0f - num104 || npc.velocity.X > num104)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (npc.velocity.X < num104 && npc.direction == 1)
				{
					npc.velocity.X += 0.07f;
					if (npc.velocity.X > num104)
						npc.velocity.X = num104;
				}
				else if (npc.velocity.X > 0f - num104 && npc.direction == -1)
				{
					npc.velocity.X -= 0.07f;
					if (npc.velocity.X < 0f - num104)
						npc.velocity.X = 0f - num104;
				}
			}

			if (npc.type >= 277 && npc.type <= 280)
			{
				Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.2f, 0.1f, 0f);
			}
			else if (npc.type == 520)
			{
				Lighting.AddLight(npc.Top + new Vector2(0f, 20f), 0.3f, 0.3f, 0.7f);
			}
			else if (npc.type == 525)
			{
				Vector3 rgb = new Vector3(0.7f, 1f, 0.2f) * 0.5f;
				Lighting.AddLight(npc.Top + new Vector2(0f, 15f), rgb);
			}
			else if (npc.type == 526)
			{
				Vector3 rgb2 = new Vector3(1f, 1f, 0.5f) * 0.4f;
				Lighting.AddLight(npc.Top + new Vector2(0f, 15f), rgb2);
			}
			else if (npc.type == 527)
			{
				Vector3 rgb3 = new Vector3(0.6f, 0.3f, 1f) * 0.4f;
				Lighting.AddLight(npc.Top + new Vector2(0f, 15f), rgb3);
			}
			else if (npc.type == 415)
			{
				npc.hide = false;
				for (int num106 = 0; num106 < 200; num106++)
				{
					if (Main.npc[num106].active && Main.npc[num106].type == 416 && Main.npc[num106].ai[0] == (float)npc.whoAmI)
					{
						npc.hide = true;
						break;
					}
				}
			}
			else if (npc.type == 258)
			{
				if (npc.velocity.Y != 0f)
				{
					npc.TargetClosest();
					npc.spriteDirection = npc.direction;
					if (Main.player[npc.target].Center.X < npc.position.X && npc.velocity.X > 0f)
						npc.velocity.X *= 0.95f;
					else if (Main.player[npc.target].Center.X > npc.position.X + (float)npc.width && npc.velocity.X < 0f)
						npc.velocity.X *= 0.95f;

					if (Main.player[npc.target].Center.X < npc.position.X && npc.velocity.X > -5f)
						npc.velocity.X -= 0.1f;
					else if (Main.player[npc.target].Center.X > npc.position.X + (float)npc.width && npc.velocity.X < 5f)
						npc.velocity.X += 0.1f;
				}
				else if (Main.player[npc.target].Center.Y + 50f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					flag = true;
					npc.velocity.Y = -7f;
				}
			}
			else if (npc.type == 425)
			{
				if (npc.localAI[3] == 0f)
				{
					npc.localAI[3] = 1f;
					npc.ai[3] = -120f;
				}

				if (npc.velocity.Y == 0f)
					npc.ai[2] = 0f;

				if (npc.velocity.Y != 0f && npc.ai[2] == 1f)
				{
					npc.TargetClosest();
					npc.spriteDirection = -npc.direction;
					if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
					{
						float num107 = 0.3f;
						float num108 = 8f;
						float num109 = 0.3f;
						float num110 = 7f;
						float num111 = Main.player[npc.target].Center.X - (float)(npc.direction * 300) - npc.Center.X;
						float num112 = Main.player[npc.target].Bottom.Y - npc.Bottom.Y;
						if (num111 < 0f && npc.velocity.X > 0f)
							npc.velocity.X *= 0.9f;
						else if (num111 > 0f && npc.velocity.X < 0f)
							npc.velocity.X *= 0.9f;

						if (num111 < 0f && npc.velocity.X > 0f - num110)
							npc.velocity.X -= num109;
						else if (num111 > 0f && npc.velocity.X < num110)
							npc.velocity.X += num109;

						if (npc.velocity.X > num110)
							npc.velocity.X = num110;

						if (npc.velocity.X < 0f - num110)
							npc.velocity.X = 0f - num110;

						if (num112 < -20f && npc.velocity.Y > 0f)
							npc.velocity.Y *= 0.8f;
						else if (num112 > 20f && npc.velocity.Y < 0f)
							npc.velocity.Y *= 0.8f;

						if (num112 < -20f && npc.velocity.Y > 0f - num108)
							npc.velocity.Y -= num107;
						else if (num112 > 20f && npc.velocity.Y < num108)
							npc.velocity.Y += num107;
					}

					if (Main.rand.Next(3) == 0)
					{
						npc.position += npc.netOffset;
						Vector2 vector25 = npc.Center + new Vector2(npc.direction * -14, -8f) - Vector2.One * 4f;
						Vector2 vector26 = new Vector2(npc.direction * -6, 12f) * 0.2f + Utils.RandomVector2(Main.rand, -1f, 1f) * 0.1f;
						Dust obj5 = Main.dust[Dust.NewDust(vector25, 8, 8, 229, vector26.X, vector26.Y, 100, Color.Transparent, 1f + Main.rand.NextFloat() * 0.5f)];
						obj5.noGravity = true;
						obj5.velocity = vector26;
						obj5.customData = npc;
						npc.position -= npc.netOffset;
					}

					for (int num113 = 0; num113 < 200; num113++)
					{
						if (num113 != npc.whoAmI && Main.npc[num113].active && Main.npc[num113].type == npc.type && Math.Abs(npc.position.X - Main.npc[num113].position.X) + Math.Abs(npc.position.Y - Main.npc[num113].position.Y) < (float)npc.width)
						{
							if (npc.position.X < Main.npc[num113].position.X)
								npc.velocity.X -= 0.15f;
							else
								npc.velocity.X += 0.15f;

							if (npc.position.Y < Main.npc[num113].position.Y)
								npc.velocity.Y -= 0.15f;
							else
								npc.velocity.Y += 0.15f;
						}
					}
				}
				else if (Main.player[npc.target].Center.Y + 100f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					flag = true;
					npc.velocity.Y = -5f;
					npc.ai[2] = 1f;
				}

				if (npc.ai[3] < 0f)
					npc.ai[3] += 1f;

				int num114 = 30;
				int num115 = 10;
				int num116 = 180;
				if (npc.ai[3] >= 0f && npc.ai[3] <= (float)num114)
				{
					Vector2 vector27 = npc.DirectionTo(Main.player[npc.target].Center);
					bool flag12 = Math.Abs(vector27.Y) <= Math.Abs(vector27.X);
					bool flag13 = npc.Distance(Main.player[npc.target].Center) < 800f && flag12 && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0);
					npc.ai[3] = MathHelper.Clamp(npc.ai[3] + (float)flag13.ToDirectionInt(), 0f, num114);
				}

				if (npc.ai[3] >= (float)(num114 + 1) && (npc.ai[3] += 1f) >= (float)(num114 + num115))
				{
					npc.ai[3] = num114 - num116;
					npc.netUpdate = true;
				}

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[3] == (float)num114)
				{
					npc.ai[3] += 1f;
					npc.netUpdate = true;
					int num117 = 20;
					Vector2 chaserPosition = npc.Center + new Vector2(npc.direction * 30, 2f);
					Vector2 vector28 = npc.DirectionTo(Main.player[npc.target].Center) * num117;
					if (vector28.HasNaNs())
						vector28 = new Vector2(npc.direction * num117, 0f);

					int num118 = 2;
					Utils.ChaseResults chaseResults = Utils.GetChaseResults(chaserPosition, num117, Main.player[npc.target].Center, Main.player[npc.target].velocity * 0.5f / num118);
					if (chaseResults.InterceptionHappens)
					{
						Vector2 vector29 = chaseResults.ChaserVelocity / num118;
						vector28.X = vector29.X;
						vector28.Y = vector29.Y;
					}

					int attackDamage_ForProjectiles = npc.GetAttackDamage_ForProjectiles(75f, 50f);
					for (int num119 = 0; num119 < 4; num119++)
					{
						Vector2 vector30 = vector28 + Utils.RandomVector2(Main.rand, -0.8f, 0.8f) * ((num119 != 0) ? 1 : 0);
						Projectile.NewProjectile(npc.GetSource_FromAI(), chaserPosition.X, chaserPosition.Y, vector30.X, vector30.Y, 577, attackDamage_ForProjectiles, 1f, Main.myPlayer);
					}
				}
			}
			else if (npc.type == 427)
			{
				if (npc.velocity.Y == 0f)
				{
					npc.ai[2] = 0f;
					npc.rotation = 0f;
				}
				else
				{
					npc.rotation = npc.velocity.X * 0.1f;
				}

				if (npc.velocity.Y != 0f && npc.ai[2] == 1f)
				{
					npc.TargetClosest();
					npc.spriteDirection = -npc.direction;
					if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
					{
						float num120 = Main.player[npc.target].Center.X - npc.Center.X;
						float num121 = Main.player[npc.target].Center.Y - npc.Center.Y;
						if (num120 < 0f && npc.velocity.X > 0f)
							npc.velocity.X *= 0.98f;
						else if (num120 > 0f && npc.velocity.X < 0f)
							npc.velocity.X *= 0.98f;

						if (num120 < -20f && npc.velocity.X > -6f)
							npc.velocity.X -= 0.015f;
						else if (num120 > 20f && npc.velocity.X < 6f)
							npc.velocity.X += 0.015f;

						if (npc.velocity.X > 6f)
							npc.velocity.X = 6f;

						if (npc.velocity.X < -6f)
							npc.velocity.X = -6f;

						if (num121 < -20f && npc.velocity.Y > 0f)
							npc.velocity.Y *= 0.98f;
						else if (num121 > 20f && npc.velocity.Y < 0f)
							npc.velocity.Y *= 0.98f;

						if (num121 < -20f && npc.velocity.Y > -6f)
							npc.velocity.Y -= 0.15f;
						else if (num121 > 20f && npc.velocity.Y < 6f)
							npc.velocity.Y += 0.15f;
					}

					for (int num122 = 0; num122 < 200; num122++)
					{
						if (num122 != npc.whoAmI && Main.npc[num122].active && Main.npc[num122].type == npc.type && Math.Abs(npc.position.X - Main.npc[num122].position.X) + Math.Abs(npc.position.Y - Main.npc[num122].position.Y) < (float)npc.width)
						{
							if (npc.position.X < Main.npc[num122].position.X)
								npc.velocity.X -= 0.05f;
							else
								npc.velocity.X += 0.05f;

							if (npc.position.Y < Main.npc[num122].position.Y)
								npc.velocity.Y -= 0.05f;
							else
								npc.velocity.Y += 0.05f;
						}
					}
				}
				else if (Main.player[npc.target].Center.Y + 100f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					flag = true;
					npc.velocity.Y = -5f;
					npc.ai[2] = 1f;
				}
			}
			else if (npc.type == 426)
			{
				float num123 = 6f;
				float num124 = 0.2f;
				float num125 = 6f;
				if (npc.ai[1] > 0f && npc.velocity.Y > 0f)
				{
					npc.velocity.Y *= 0.85f;
					if (npc.velocity.Y == 0f)
						npc.velocity.Y = -0.4f;
				}

				if (npc.velocity.Y != 0f)
				{
					npc.TargetClosest();
					npc.spriteDirection = npc.direction;
					if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
					{
						float num126 = Main.player[npc.target].Center.X - (float)(npc.direction * 300) - npc.Center.X;
						if (num126 < 40f && npc.velocity.X > 0f)
							npc.velocity.X *= 0.98f;
						else if (num126 > 40f && npc.velocity.X < 0f)
							npc.velocity.X *= 0.98f;

						if (num126 < 40f && npc.velocity.X > 0f - num123)
							npc.velocity.X -= num124;
						else if (num126 > 40f && npc.velocity.X < num123)
							npc.velocity.X += num124;

						if (npc.velocity.X > num123)
							npc.velocity.X = num123;

						if (npc.velocity.X < 0f - num123)
							npc.velocity.X = 0f - num123;
					}
				}
				else if (Main.player[npc.target].Center.Y + 100f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					flag = true;
					npc.velocity.Y = 0f - num125;
				}

				for (int num127 = 0; num127 < 200; num127++)
				{
					if (num127 != npc.whoAmI && Main.npc[num127].active && Main.npc[num127].type == npc.type && Math.Abs(npc.position.X - Main.npc[num127].position.X) + Math.Abs(npc.position.Y - Main.npc[num127].position.Y) < (float)npc.width)
					{
						if (npc.position.X < Main.npc[num127].position.X)
							npc.velocity.X -= 0.1f;
						else
							npc.velocity.X += 0.1f;

						if (npc.position.Y < Main.npc[num127].position.Y)
							npc.velocity.Y -= 0.1f;
						else
							npc.velocity.Y += 0.1f;
					}
				}

				if (Main.rand.Next(6) == 0 && npc.ai[1] <= 20f)
				{
					npc.position += npc.netOffset;
					Dust obj6 = Main.dust[Dust.NewDust(npc.Center + new Vector2((npc.spriteDirection == 1) ? 8 : (-20), -20f), 8, 8, 229, npc.velocity.X, npc.velocity.Y, 100)];
					obj6.velocity = obj6.velocity / 4f + npc.velocity / 2f;
					obj6.scale = 0.6f;
					obj6.noLight = true;
					npc.position -= npc.netOffset;
				}

				if (npc.ai[1] >= 57f)
				{
					npc.position += npc.netOffset;
					int num128 = Utils.SelectRandom<int>(Main.rand, 161, 229);
					Dust obj7 = Main.dust[Dust.NewDust(npc.Center + new Vector2((npc.spriteDirection == 1) ? 8 : (-20), -20f), 8, 8, num128, npc.velocity.X, npc.velocity.Y, 100)];
					obj7.velocity = obj7.velocity / 4f + npc.DirectionTo(Main.player[npc.target].Top);
					obj7.scale = 1.2f;
					obj7.noLight = true;
					npc.position -= npc.netOffset;
				}

				if (Main.rand.Next(6) == 0)
				{
					npc.position += npc.netOffset;
					Dust dust5 = Main.dust[Dust.NewDust(npc.Center, 2, 2, 229)];
					dust5.position = npc.Center + new Vector2((npc.spriteDirection == 1) ? 26 : (-26), 24f);
					dust5.velocity.X = 0f;
					if (dust5.velocity.Y < 0f)
						dust5.velocity.Y = 0f;

					dust5.noGravity = true;
					dust5.scale = 1f;
					dust5.noLight = true;
					npc.position -= npc.netOffset;
				}
			}
			else if (npc.type == 185)
			{
				if (npc.velocity.Y == 0f)
				{
					npc.rotation = 0f;
					npc.localAI[0] = 0f;
				}
				else if (npc.localAI[0] == 1f)
				{
					npc.rotation += npc.velocity.X * 0.05f;
				}
			}
			else if (npc.type == 428)
			{
				if (npc.velocity.Y == 0f)
					npc.rotation = 0f;
				else
					npc.rotation += npc.velocity.X * 0.08f;
			}

			if (npc.type == 159 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				Vector2 vector31 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num129 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector31.X;
				float num130 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector31.Y;
				if ((float)Math.Sqrt(num129 * num129 + num130 * num130) > 300f)
					npc.Transform(158);
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (Main.expertMode && npc.target >= 0 && (npc.type == 163 || npc.type == 238 || npc.type == 236 || npc.type == 237) && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
				{
					npc.localAI[0] += 1f;
					if (npc.justHit)
					{
						npc.localAI[0] -= Main.rand.Next(20, 60);
						if (npc.localAI[0] < 0f)
							npc.localAI[0] = 0f;
					}

					if (npc.localAI[0] > (float)Main.rand.Next(180, 900))
					{
						npc.localAI[0] = 0f;
						Vector2 vector32 = Main.player[npc.target].Center - npc.Center;
						vector32.Normalize();
						vector32 *= 8f;
						int attackDamage_ForProjectiles2 = npc.GetAttackDamage_ForProjectiles(18f, 18f);
						Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, vector32.X, vector32.Y, 472, attackDamage_ForProjectiles2, 0f, Main.myPlayer);
					}
				}

				if (npc.velocity.Y == 0f)
				{
					int num131 = -1;
					switch (npc.type)
					{
						case 164:
							num131 = 165;
							break;
						case 236:
							num131 = 237;
							break;
						case 163:
							num131 = 238;
							break;
						case 239:
							num131 = 240;
							break;
						case 530:
							num131 = 531;
							break;
					}

					if (num131 != -1 && npc.NPCCanStickToWalls())
						npc.Transform(num131);
				}
			}

			if (npc.type == 243)
			{
				if (npc.justHit && Main.rand.Next(3) == 0)
					npc.ai[2] -= Main.rand.Next(30);

				if (npc.ai[2] < 0f)
					npc.ai[2] = 0f;

				if (npc.confused)
					npc.ai[2] = 0f;

				npc.ai[2] += 1f;
				float num132 = Main.rand.Next(30, 900);
				num132 *= (float)npc.life / (float)npc.lifeMax;
				num132 += 30f;
				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] >= num132 && npc.velocity.Y == 0f && !Main.player[npc.target].dead && !Main.player[npc.target].frozen && ((npc.direction > 0 && npc.Center.X < Main.player[npc.target].Center.X) || (npc.direction < 0 && npc.Center.X > Main.player[npc.target].Center.X)) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Vector2 vector33 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
					vector33.X += 10 * npc.direction;
					float num133 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector33.X;
					float num134 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector33.Y;
					num133 += (float)Main.rand.Next(-40, 41);
					num134 += (float)Main.rand.Next(-40, 41);
					float num135 = (float)Math.Sqrt(num133 * num133 + num134 * num134);
					npc.netUpdate = true;
					num135 = 15f / num135;
					num133 *= num135;
					num134 *= num135;
					int num136 = 32;
					int num137 = 257;
					vector33.X += num133 * 3f;
					vector33.Y += num134 * 3f;
					Projectile.NewProjectile(npc.GetSource_FromAI(), vector33.X, vector33.Y, num133, num134, num137, num136, 0f, Main.myPlayer);
					npc.ai[2] = 0f;
				}
			}

			if (npc.type == 251)
			{
				if (npc.justHit)
					npc.ai[2] -= Main.rand.Next(30);

				if (npc.ai[2] < 0f)
					npc.ai[2] = 0f;

				if (npc.confused)
					npc.ai[2] = 0f;

				npc.ai[2] += 1f;
				float num138 = Main.rand.Next(60, 1800);
				num138 *= (float)npc.life / (float)npc.lifeMax;
				num138 += 15f;
				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] >= num138 && npc.velocity.Y == 0f && !Main.player[npc.target].dead && !Main.player[npc.target].frozen && ((npc.direction > 0 && npc.Center.X < Main.player[npc.target].Center.X) || (npc.direction < 0 && npc.Center.X > Main.player[npc.target].Center.X)) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 12f);
					vector34.X += 6 * npc.direction;
					float num139 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector34.X;
					float num140 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector34.Y;
					num139 += (float)Main.rand.Next(-40, 41);
					num140 += (float)Main.rand.Next(-30, 0);
					float num141 = (float)Math.Sqrt(num139 * num139 + num140 * num140);
					npc.netUpdate = true;
					num141 = 15f / num141;
					num139 *= num141;
					num140 *= num141;
					int num142 = 30;
					int num143 = 83;
					vector34.X += num139 * 3f;
					vector34.Y += num140 * 3f;
					Projectile.NewProjectile(npc.GetSource_FromAI(), vector34.X, vector34.Y, num139, num140, num143, num142, 0f, Main.myPlayer);
					npc.ai[2] = 0f;
				}
			}

			if (npc.type == 386)
			{
				if (npc.confused)
				{
					npc.ai[2] = -60f;
				}
				else
				{
					if (npc.ai[2] < 60f)
						npc.ai[2] += 1f;

					if (npc.ai[2] > 0f && NPC.CountNPCS(387) >= 4 * NPC.CountNPCS(386))
						npc.ai[2] = 0f;

					if (npc.justHit)
						npc.ai[2] = -30f;

					if (npc.ai[2] == 30f)
					{
						int num144 = (int)npc.position.X / 16;
						int num145 = (int)npc.position.Y / 16;
						int num146 = (int)npc.position.X / 16;
						int num147 = (int)npc.position.Y / 16;
						int num148 = 5;
						int num149 = 0;
						bool flag14 = false;
						int num150 = 2;
						int num151 = 0;
						while (!flag14 && num149 < 100)
						{
							num149++;
							int num152 = Main.rand.Next(num144 - num148, num144 + num148);
							for (int num153 = Main.rand.Next(num145 - num148, num145 + num148); num153 < num145 + num148; num153++)
							{
								if ((num153 < num145 - num150 || num153 > num145 + num150 || num152 < num144 - num150 || num152 > num144 + num150) && (num153 < num147 - num151 || num153 > num147 + num151 || num152 < num146 - num151 || num152 > num146 + num151) && Main.tile[num152, num153].HasUnactuatedTile)
								{
									bool flag15 = true;
									if (Main.tile[num152, num153 - 1].LiquidAmount > 0 && Main.tile[num152, num153 - 1].LiquidType == LiquidID.Lava)
										flag15 = false;

									if (flag15 && Main.tileSolid[Main.tile[num152, num153].TileType] && !Collision.SolidTiles(num152 - 1, num152 + 1, num153 - 4, num153 - 1))
									{
										int num154 = NPC.NewNPC(npc.GetSource_FromAI(), num152 * 16 - npc.width / 2, num153 * 16, 387);
										Main.npc[num154].position.Y = num153 * 16 - Main.npc[num154].height;
										flag14 = true;
										npc.netUpdate = true;
										break;
									}
								}
							}
						}
					}

					if (npc.ai[2] == 60f)
						npc.ai[2] = -120f;
				}
			}

			if (npc.type == 389)
			{
				if (npc.confused)
				{
					npc.ai[2] = -60f;
				}
				else
				{
					if (npc.ai[2] < 20f)
						npc.ai[2] += 1f;

					if (npc.justHit)
						npc.ai[2] = -30f;

					if (npc.ai[2] == 20f && Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.ai[2] = -10 + Main.rand.Next(3) * -10;
						Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y + 8f, npc.direction * 6, 0f, 437, 25, 1f, Main.myPlayer);
					}
				}
			}

			if (npc.type == 110 || npc.type == 111 || npc.type == 206 || npc.type == 214 || npc.type == 215 || npc.type == 216 || npc.type == 290 || npc.type == 291 || npc.type == 292 || npc.type == 293 || npc.type == 350 || npc.type == 379 || npc.type == 380 || npc.type == 381 || npc.type == 382 || (npc.type >= 449 && npc.type <= 452) || npc.type == 468 || npc.type == 481 || npc.type == 411 || npc.type == 409 || (npc.type >= 498 && npc.type <= 506) || npc.type == 424 || npc.type == 426 || npc.type == 520)
			{
				bool flag16 = npc.type == 381 || npc.type == 382 || npc.type == 520;
				bool flag17 = npc.type == 426;
				bool flag18 = true;
				int num155 = -1;
				int num156 = -1;
				if (npc.type == 411)
				{
					flag16 = true;
					num155 = 120;
					num156 = 120;
					if (npc.ai[1] <= 220f)
						flag18 = false;
				}

				if (npc.ai[1] > 0f)
					npc.ai[1] -= 1f;

				if (npc.justHit)
				{
					npc.ai[1] = 30f;
					npc.ai[2] = 0f;
				}

				int num157 = 70;
				if (npc.type == 379 || npc.type == 380)
					num157 = 80;

				if (npc.type == 381 || npc.type == 382)
					num157 = 80;

				if (npc.type == 520)
					num157 = 15;

				if (npc.type == 350)
					num157 = 110;

				if (npc.type == 291)
					num157 = 200;

				if (npc.type == 292)
					num157 = 120;

				if (npc.type == 293)
					num157 = 90;

				if (npc.type == 111)
					num157 = 180;

				if (npc.type == 206)
					num157 = 50;

				if (npc.type == 481)
					num157 = 100;

				if (npc.type == 214)
					num157 = 40;

				if (npc.type == 215)
					num157 = 80;

				if (npc.type == 290)
					num157 = 30;

				if (npc.type == 411)
					num157 = 330;

				if (npc.type == 409)
					num157 = 60;

				if (npc.type == 424)
					num157 = 180;

				if (npc.type == 426)
					num157 = 60;

				bool flag19 = false;
				if (npc.type == 216)
				{
					if (npc.localAI[2] >= 20f)
						flag19 = true;

					num157 = ((!flag19) ? 8 : 60);
				}

				int num158 = num157 / 2;
				if (npc.type == 424)
					num158 = num157 - 1;

				if (npc.type == 426)
					num158 = num157 - 1;

				if (npc.type == 411)
					num158 = 220;

				if (npc.confused)
					npc.ai[2] = 0f;

				if (npc.ai[2] > 0f)
				{
					if (flag18)
						npc.TargetClosest();

					if (npc.ai[1] == (float)num158)
					{
						if (npc.type == 216)
							npc.localAI[2] += 1f;

						float num159 = 11f;
						if (npc.type == 111)
							num159 = 9f;

						if (npc.type == 206)
							num159 = 7f;

						if (npc.type == 290)
							num159 = 9f;

						if (npc.type == 293)
							num159 = 4f;

						if (npc.type == 214)
							num159 = 14f;

						if (npc.type == 215)
							num159 = 16f;

						if (npc.type == 382)
							num159 = 7f;

						if (npc.type == 520)
							num159 = 8f;

						if (npc.type == 409)
							num159 = 4f;

						if (npc.type >= 449 && npc.type <= 452)
							num159 = 7f;

						if (npc.type == 481)
							num159 = 8f;

						if (npc.type == 468)
							num159 = 7.5f;

						if (npc.type == 411)
							num159 = 1f;

						if (npc.type >= 498 && npc.type <= 506)
							num159 = 7f;

						Vector2 chaserPosition2 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						if (npc.type == 481)
							chaserPosition2.Y -= 14f;

						if (npc.type == 206)
							chaserPosition2.Y -= 10f;

						if (npc.type == 290)
							chaserPosition2.Y -= 10f;

						if (npc.type == 381 || npc.type == 382)
							chaserPosition2.Y += 6f;

						if (npc.type == 520)
							chaserPosition2.Y = npc.position.Y + 20f;

						if (npc.type >= 498 && npc.type <= 506)
							chaserPosition2.Y -= 8f;

						if (npc.type == 426)
						{
							chaserPosition2 += new Vector2(npc.spriteDirection * 2, -12f);
							num159 = 7f;
						}

						float num160 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - chaserPosition2.X;
						float num161 = Math.Abs(num160) * 0.1f;
						if (npc.type == 291 || npc.type == 292)
							num161 = 0f;

						if (npc.type == 215)
							num161 = Math.Abs(num160) * 0.08f;

						if (npc.type == 214 || (npc.type == 216 && !flag19))
							num161 = 0f;

						if (npc.type == 381 || npc.type == 382 || npc.type == 520)
							num161 = 0f;

						if (npc.type >= 449 && npc.type <= 452)
							num161 = Math.Abs(num160) * (float)Main.rand.Next(10, 50) * 0.01f;

						if (npc.type == 468)
							num161 = Math.Abs(num160) * (float)Main.rand.Next(10, 50) * 0.01f;

						if (npc.type == 481)
							num161 = Math.Abs(num160) * (float)Main.rand.Next(-10, 11) * 0.0035f;

						if (npc.type >= 498 && npc.type <= 506)
							num161 = Math.Abs(num160) * (float)Main.rand.Next(1, 11) * 0.0025f;

						float num162 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - chaserPosition2.Y - num161;
						if (npc.type == 291)
						{
							num160 += (float)Main.rand.Next(-40, 41) * 0.2f;
							num162 += (float)Main.rand.Next(-40, 41) * 0.2f;
						}
						else if (npc.type == 381 || npc.type == 382 || npc.type == 520)
						{
							num160 += (float)Main.rand.Next(-100, 101) * 0.4f;
							num162 += (float)Main.rand.Next(-100, 101) * 0.4f;
							num160 *= (float)Main.rand.Next(85, 116) * 0.01f;
							num162 *= (float)Main.rand.Next(85, 116) * 0.01f;
							if (npc.type == 520)
							{
								num160 += (float)Main.rand.Next(-100, 101) * 0.6f;
								num162 += (float)Main.rand.Next(-100, 101) * 0.6f;
								num160 *= (float)Main.rand.Next(85, 116) * 0.015f;
								num162 *= (float)Main.rand.Next(85, 116) * 0.015f;
							}
						}
						else if (npc.type == 481)
						{
							num160 += (float)Main.rand.Next(-40, 41) * 0.4f;
							num162 += (float)Main.rand.Next(-40, 41) * 0.4f;
						}
						else if (npc.type >= 498 && npc.type <= 506)
						{
							num160 += (float)Main.rand.Next(-40, 41) * 0.3f;
							num162 += (float)Main.rand.Next(-40, 41) * 0.3f;
						}
						else if (npc.type == 426)
						{
							num160 += (float)Main.rand.Next(-30, 31) * 0.3f;
							num162 += (float)Main.rand.Next(-30, 31) * 0.3f;
						}
						else if (npc.type != 292)
						{
							num160 += (float)Main.rand.Next(-40, 41);
							num162 += (float)Main.rand.Next(-40, 41);
						}

						float num163 = (float)Math.Sqrt(num160 * num160 + num162 * num162);
						npc.netUpdate = true;
						num163 = num159 / num163;
						num160 *= num163;
						num162 *= num163;
						int num164 = 35;
						int num165 = 82;
						if (npc.type == 111)
							num164 = 11;

						if (npc.type == 206)
							num164 = 37;

						if (npc.type == 379 || npc.type == 380)
							num164 = 40;

						if (npc.type == 350)
							num164 = 45;

						if (npc.type == 468)
							num164 = 50;

						if (npc.type == 111)
							num165 = 81;

						if (npc.type == 379 || npc.type == 380)
							num165 = 81;

						if (npc.type == 381)
						{
							num165 = 436;
							num164 = 24;
						}

						if (npc.type == 382)
						{
							num165 = 438;
							num164 = 30;
						}

						if (npc.type == 520)
						{
							num165 = 592;
							num164 = 35;
						}

						if (npc.type >= 449 && npc.type <= 452)
						{
							num165 = 471;
							num164 = 15;
						}

						if (npc.type >= 498 && npc.type <= 506)
						{
							num165 = 572;
							num164 = 14;
						}

						if (npc.type == 481)
						{
							num165 = 508;
							num164 = 18;
						}

						if (npc.type == 206)
							num165 = 177;

						if (npc.type == 468)
							num165 = 501;

						if (npc.type == 411)
						{
							num165 = 537;
							num164 = npc.GetAttackDamage_ForProjectiles(60f, 45f);
						}

						if (npc.type == 424)
						{
							num165 = 573;
							num164 = npc.GetAttackDamage_ForProjectiles(60f, 45f);
						}

						if (npc.type == 426)
						{
							num165 = 581;
							num164 = npc.GetAttackDamage_ForProjectiles(60f, 45f);
						}

						if (npc.type == 291)
						{
							num165 = 302;
							num164 = 100;
						}

						if (npc.type == 290)
						{
							num165 = 300;
							num164 = 60;
						}

						if (npc.type == 293)
						{
							num165 = 303;
							num164 = 60;
						}

						if (npc.type == 214)
						{
							num165 = 180;
							num164 = 25;
						}

						if (npc.type == 215)
						{
							num165 = 82;
							num164 = 40;
						}

						if (npc.type == 292)
						{
							num164 = 50;
							num165 = 180;
						}

						if (npc.type == 216)
						{
							num165 = 180;
							num164 = 30;
							if (flag19)
							{
								num164 = 100;
								num165 = 240;
								npc.localAI[2] = 0f;
							}
						}

						Player player3 = Main.player[npc.target];
						Vector2? vector35 = null;
						if (npc.type == 426)
							vector35 = Main.rand.NextVector2FromRectangle(player3.Hitbox);

						if (vector35.HasValue)
						{
							Utils.ChaseResults chaseResults2 = Utils.GetChaseResults(chaserPosition2, num159, vector35.Value, player3.velocity);
							if (chaseResults2.InterceptionHappens)
							{
								Vector2 vector36 = Utils.FactorAcceleration(chaseResults2.ChaserVelocity, chaseResults2.InterceptionTime, new Vector2(0f, 0.1f), 15);
								num160 = vector36.X;
								num162 = vector36.Y;
							}
						}

						chaserPosition2.X += num160;
						chaserPosition2.Y += num162;
						if (npc.type == 290)
							num164 = npc.GetAttackDamage_ForProjectiles(num164, (float)num164 * 0.75f);

						if (npc.type >= 381 && npc.type <= 392)
							num164 = npc.GetAttackDamage_ForProjectiles(num164, (float)num164 * 0.8f);

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							if (npc.type == 292)
							{
								for (int num166 = 0; num166 < 4; num166++)
								{
									num160 = player3.position.X + (float)player3.width * 0.5f - chaserPosition2.X;
									num162 = player3.position.Y + (float)player3.height * 0.5f - chaserPosition2.Y;
									num163 = (float)Math.Sqrt(num160 * num160 + num162 * num162);
									num163 = 12f / num163;
									num160 = (num160 += (float)Main.rand.Next(-40, 41));
									num162 = (num162 += (float)Main.rand.Next(-40, 41));
									num160 *= num163;
									num162 *= num163;
									Projectile.NewProjectile(npc.GetSource_FromAI(), chaserPosition2.X, chaserPosition2.Y, num160, num162, num165, num164, 0f, Main.myPlayer);
								}
							}
							else if (npc.type == 411)
							{
								Projectile.NewProjectile(npc.GetSource_FromAI(), chaserPosition2.X, chaserPosition2.Y, num160, num162, num165, num164, 0f, Main.myPlayer, 0f, npc.whoAmI);
							}
							else if (npc.type == 424)
							{
								for (int num167 = 0; num167 < 4; num167++)
								{
									Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X - (float)(npc.spriteDirection * 4), npc.Center.Y + 6f, (float)(-3 + 2 * num167) * 0.15f, (float)(-Main.rand.Next(0, 3)) * 0.2f - 0.1f, num165, num164, 0f, Main.myPlayer, 0f, npc.whoAmI);
								}
							}
							else if (npc.type == 409)
							{
								int num168 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, 410, npc.whoAmI);
								Main.npc[num168].velocity = new Vector2(num160, -6f + num162);
							}
							else
							{
								Projectile.NewProjectile(npc.GetSource_FromAI(), chaserPosition2.X, chaserPosition2.Y, num160, num162, num165, num164, 0f, Main.myPlayer);
							}
						}

						if (Math.Abs(num162) > Math.Abs(num160) * 2f)
						{
							if (num162 > 0f)
								npc.ai[2] = 1f;
							else
								npc.ai[2] = 5f;
						}
						else if (Math.Abs(num160) > Math.Abs(num162) * 2f)
						{
							npc.ai[2] = 3f;
						}
						else if (num162 > 0f)
						{
							npc.ai[2] = 2f;
						}
						else
						{
							npc.ai[2] = 4f;
						}
					}

					if ((npc.velocity.Y != 0f && !flag17) || npc.ai[1] <= 0f)
					{
						npc.ai[2] = 0f;
						npc.ai[1] = 0f;
					}
					else if (!flag16 || (num155 != -1 && npc.ai[1] >= (float)num155 && npc.ai[1] < (float)(num155 + num156) && (!flag17 || npc.velocity.Y == 0f)))
					{
						npc.velocity.X *= 0.9f;
						npc.spriteDirection = npc.direction;
					}
				}

				if (npc.type == 468 && !Main.eclipse)
				{
					flag16 = true;
				}
				else if ((npc.ai[2] <= 0f || flag16) && (npc.velocity.Y == 0f || flag17) && npc.ai[1] <= 0f && !Main.player[npc.target].dead)
				{
					bool flag20 = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
					if (npc.type == 520)
						flag20 = Collision.CanHitLine(npc.Top + new Vector2(0f, 20f), 0, 0, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);

					if (Main.player[npc.target].stealth == 0f && Main.player[npc.target].itemAnimation == 0)
						flag20 = false;

					if (flag20)
					{
						float num169 = 10f;
						Vector2 vector37 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num170 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector37.X;
						float num171 = Math.Abs(num170) * 0.1f;
						float num172 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector37.Y - num171;
						num170 += (float)Main.rand.Next(-40, 41);
						num172 += (float)Main.rand.Next(-40, 41);
						float num173 = (float)Math.Sqrt(num170 * num170 + num172 * num172);
						float num174 = 700f;
						if (npc.type == 214)
							num174 = 550f;

						if (npc.type == 215)
							num174 = 800f;

						if (npc.type >= 498 && npc.type <= 506)
							num174 = 190f;

						if (npc.type >= 449 && npc.type <= 452)
							num174 = 200f;

						if (npc.type == 481)
							num174 = 400f;

						if (npc.type == 468)
							num174 = 400f;

						if (num173 < num174)
						{
							npc.netUpdate = true;
							npc.velocity.X *= 0.5f;
							num173 = num169 / num173;
							num170 *= num173;
							num172 *= num173;
							npc.ai[2] = 3f;
							npc.ai[1] = num157;
							if (Math.Abs(num172) > Math.Abs(num170) * 2f)
							{
								if (num172 > 0f)
									npc.ai[2] = 1f;
								else
									npc.ai[2] = 5f;
							}
							else if (Math.Abs(num170) > Math.Abs(num172) * 2f)
							{
								npc.ai[2] = 3f;
							}
							else if (num172 > 0f)
							{
								npc.ai[2] = 2f;
							}
							else
							{
								npc.ai[2] = 4f;
							}
						}
					}
				}

				if (npc.ai[2] <= 0f || (flag16 && (num155 == -1 || !(npc.ai[1] >= (float)num155) || !(npc.ai[1] < (float)(num155 + num156)))))
				{
					float num175 = 1f;
					float num176 = 0.07f;
					float num177 = 0.8f;
					if (npc.type == 214)
					{
						num175 = 2f;
						num176 = 0.09f;
					}
					else if (npc.type == 215)
					{
						num175 = 1.5f;
						num176 = 0.08f;
					}
					else if (npc.type == 381 || npc.type == 382)
					{
						num175 = 2f;
						num176 = 0.5f;
					}
					else if (npc.type == 520)
					{
						num175 = 4f;
						num176 = 1f;
						num177 = 0.7f;
					}
					else if (npc.type == 411)
					{
						num175 = 2f;
						num176 = 0.5f;
					}
					else if (npc.type == 409)
					{
						num175 = 2f;
						num176 = 0.5f;
					}
					else if (npc.type == 426)
					{
						num175 = 4f;
						num176 = 0.6f;
						num177 = 0.95f;
					}

					bool flag21 = false;
					if ((npc.type == 381 || npc.type == 382) && Vector2.Distance(npc.Center, Main.player[npc.target].Center) < 300f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
					{
						flag21 = true;
						npc.ai[3] = 0f;
					}

					if (npc.type == 520 && Vector2.Distance(npc.Center, Main.player[npc.target].Center) < 400f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
					{
						flag21 = true;
						npc.ai[3] = 0f;
					}

					if (npc.velocity.X < 0f - num175 || npc.velocity.X > num175 || flag21)
					{
						if (npc.velocity.Y == 0f)
							npc.velocity *= num177;
					}
					else if (npc.velocity.X < num175 && npc.direction == 1)
					{
						npc.velocity.X += num176;
						if (npc.velocity.X > num175)
							npc.velocity.X = num175;
					}
					else if (npc.velocity.X > 0f - num175 && npc.direction == -1)
					{
						npc.velocity.X -= num176;
						if (npc.velocity.X < 0f - num175)
							npc.velocity.X = 0f - num175;
					}
				}

				if (npc.type == 520)
				{
					npc.localAI[2] += 1f;
					if (npc.localAI[2] >= 6f)
					{
						npc.localAI[2] = 0f;
						npc.localAI[3] = Main.player[npc.target].DirectionFrom(npc.Top + new Vector2(0f, 20f)).ToRotation();
					}
				}
			}

			if (npc.type == 109 && Main.netMode != NetmodeID.MultiplayerClient && !Main.player[npc.target].dead)
			{
				if (npc.justHit)
					npc.ai[2] = 0f;

				npc.ai[2] += 1f;
				if (npc.ai[2] > 60f)
				{
					Vector2 vector38 = new Vector2(npc.position.X + (float)npc.width * 0.5f - (float)(npc.direction * 24), npc.position.Y + 4f);
					if (Main.rand.Next(5) != 0 || NPC.AnyNPCs(378))
					{
						int num178 = Main.rand.Next(3, 8) * npc.direction;
						int num179 = Main.rand.Next(-8, -5);
						int num180 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector38.X, vector38.Y, num178, num179, 75, 80, 0f, Main.myPlayer);
						Main.projectile[num180].timeLeft = 300;
						npc.ai[2] = 0f;
					}
					else
					{
						npc.ai[2] = -120f;
						int number = NPC.NewNPC(npc.GetSource_FromAI(), (int)vector38.X, (int)vector38.Y, 378);
						NetMessage.SendData(23, -1, -1, null, number);
					}
				}
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

			if (npc.type == 428)
				flag5 = false;

			if (npc.velocity.Y >= 0f && (npc.type != 580 || npc.directionY != 1))
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
							if (npc.type == 163 || npc.type == 164 || npc.type == 236 || npc.type == 239 || npc.type == 530)
								num193 += 8f;

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
				if (npc.type == 109 || npc.type == 163 || npc.type == 164 || npc.type == 199 || npc.type == 236 || npc.type == 239 || npc.type == 257 || npc.type == 258 || npc.type == 290 || npc.type == 391 || npc.type == 425 || npc.type == 427 || npc.type == 426 || npc.type == 580 || npc.type == 508 || npc.type == 415 || npc.type == 530 || npc.type == 532 || npc.type == 582)
					num194 = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 16) * npc.direction)) / 16f);

				Tile tile = Main.tile[num194, num195 + 1];
				tile.IsHalfBlock = false;

				/*
				if (Main.tile[num194, num195 - 1].HasUnactuatedTile && (Main.tile[num194, num195 - 1].type == 10 || Main.tile[num194, num195 - 1].type == 388) && flag8) {
				*/
				if (Main.tile[num194, num195 - 1].HasUnactuatedTile && (TileLoader.IsClosedDoor(Main.tile[num194, num195 - 1]) || Main.tile[num194, num195 - 1].TileType == 388) && flag8)
				{
					npc.ai[2] += 1f;
					npc.ai[3] = 0f;
					if (npc.ai[2] >= 60f)
					{
						bool flag23 = npc.type == 3 || npc.type == 430 || npc.type == 590 || npc.type == 331 || npc.type == 332 || npc.type == 132 || npc.type == 161 || npc.type == 186 || npc.type == 187 || npc.type == 188 || npc.type == 189 || npc.type == 200 || npc.type == 223 || npc.type == 320 || npc.type == 321 || npc.type == 319 || npc.type == 21 || npc.type == 324 || npc.type == 323 || npc.type == 322 || npc.type == 44 || npc.type == 196 || npc.type == 167 || npc.type == 77 || npc.type == 197 || npc.type == 202 || npc.type == 203 || npc.type == 449 || npc.type == 450 || npc.type == 451 || npc.type == 452 || npc.type == 481 || npc.type == 201 || npc.type == 635;
						bool flag24 = Main.player[npc.target].ZoneGraveyard && Main.rand.Next(60) == 0;
						if ((!Main.bloodMoon || Main.getGoodWorld) && !flag24 && flag23)
							npc.ai[1] = 0f;

						npc.velocity.X = 0.5f * (float)(-npc.direction);
						int num196 = 5;
						if (Main.tile[num194, num195 - 1].TileType == 388)
							num196 = 2;

						npc.ai[1] += num196;
						if (npc.type == 27)
							npc.ai[1] += 1f;

						if (npc.type == 31 || npc.type == 294 || npc.type == 295 || npc.type == 296)
							npc.ai[1] += 6f;

						npc.ai[2] = 0f;
						bool flag25 = false;
						if (npc.ai[1] >= 10f)
						{
							flag25 = true;
							npc.ai[1] = 10f;
						}

						if (npc.type == 460)
							flag25 = true;

						WorldGen.KillTile(num194, num195 - 1, fail: true);
						if ((Main.netMode != NetmodeID.MultiplayerClient || !flag25) && flag25 && Main.netMode != NetmodeID.MultiplayerClient)
						{
							if (npc.type == 26)
							{
								WorldGen.KillTile(num194, num195 - 1);
								if (Main.netMode == NetmodeID.Server)
									NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, num194, num195 - 1);
							}
							else
							{
								if (TileLoader.IsClosedDoor(Main.tile[num194, num195 - 1]))
								{
									bool flag26 = WorldGen.OpenDoor(num194, num195 - 1, npc.direction);
									if (!flag26)
									{
										npc.ai[3] = num56;
										npc.netUpdate = true;
									}

									if (Main.netMode == NetmodeID.Server && flag26)
										NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, num194, num195 - 1, npc.direction);
								}

								if (Main.tile[num194, num195 - 1].TileType == 388)
								{
									bool flag27 = WorldGen.ShiftTallGate(num194, num195 - 1, closing: false);
									if (!flag27)
									{
										npc.ai[3] = num56;
										npc.netUpdate = true;
									}

									if (Main.netMode == NetmodeID.Server && flag27)
										NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, num194, num195 - 1);
								}
							}
						}
					}
				}
				else
				{
					int num197 = npc.spriteDirection;
					if (npc.type == 425)
						num197 *= -1;

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
							if (npc.type == NPCID.Gnome)
							{
								npc.velocity.Y = -8f;
								int num198 = (int)(npc.position.Y + (float)npc.height) / 16;
								if (WorldGen.SolidTile((int)npc.Center.X / 16, num198 - 8))
								{
									npc.direction *= -1;
									npc.spriteDirection = npc.direction;
									npc.velocity.X = 3 * npc.direction;
								}
							}
							else
							{
								npc.velocity.Y = -6f;
							}

							npc.netUpdate = true;
						}
						else if (npc.position.Y + (float)npc.height - (float)(num195 * 16) > 20f && Main.tile[num194, num195].HasUnactuatedTile && !Main.tile[num194, num195].TopSlope && Main.tileSolid[Main.tile[num194, num195].TileType])
						{
							npc.velocity.Y = -5f;
							npc.netUpdate = true;
						}
						else if (npc.directionY < 0 && npc.type != NPCID.Crab && (!Main.tile[num194, num195 + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[num194, num195 + 1].TileType]) && (!Main.tile[num194 + npc.direction, num195 + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[num194 + npc.direction, num195 + 1].TileType]))
						{
							npc.velocity.Y = -8f;
							npc.velocity.X *= 1.5f;
							npc.netUpdate = true;
						}
						else if (flag8)
						{
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						if (npc.velocity.Y == 0f && flag6 && npc.ai[3] == 1f)
							npc.velocity.Y = -5f;

						if (npc.velocity.Y == 0f && (Main.expertMode || npc.type == NPCID.ZombieMerman) && Main.player[npc.target].Bottom.Y < npc.Top.Y && Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < (float)(Main.player[npc.target].width * 3) && Collision.CanHit(npc, Main.player[npc.target]))
						{
							if (npc.type == NPCID.ZombieMerman)
							{
								int num199 = (int)((npc.Bottom.Y - 16f - Main.player[npc.target].Bottom.Y) / 16f);
								if (num199 < 14 && Collision.CanHit(npc, Main.player[npc.target]))
								{
									if (num199 < 7)
										npc.velocity.Y = -8.8f;
									else if (num199 < 8)
										npc.velocity.Y = -9.2f;
									else if (num199 < 9)
										npc.velocity.Y = -9.7f;
									else if (num199 < 10)
										npc.velocity.Y = -10.3f;
									else if (num199 < 11)
										npc.velocity.Y = -10.6f;
									else
										npc.velocity.Y = -11f;
								}
							}

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

					if ((npc.type == 31 || npc.type == 294 || npc.type == 295 || npc.type == 296 || npc.type == 47 || npc.type == 77 || npc.type == 104 || npc.type == 168 || npc.type == 196 || npc.type == 385 || npc.type == 389 || npc.type == 464 || npc.type == 470 || (npc.type >= 524 && npc.type <= 527)) && npc.velocity.Y == 0f)
					{
						int num204 = 100;
						int num205 = 50;
						if (npc.type == 586)
						{
							num204 = 150;
							num205 = 150;
						}

						if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < (float)num204 && Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < (float)num205 && ((npc.direction > 0 && npc.velocity.X >= 1f) || (npc.direction < 0 && npc.velocity.X <= -1f)))
						{
							if (npc.type == 586)
							{
								npc.velocity.X += npc.direction;
								npc.velocity.X *= 2f;
								if (npc.velocity.X > 8f)
									npc.velocity.X = 8f;

								if (npc.velocity.X < -8f)
									npc.velocity.X = -8f;

								npc.velocity.Y = -4.5f;
								if (npc.position.Y > Main.player[npc.target].position.Y + 40f)
									npc.velocity.Y -= 2f;

								if (npc.position.Y > Main.player[npc.target].position.Y + 80f)
									npc.velocity.Y -= 2f;

								if (npc.position.Y > Main.player[npc.target].position.Y + 120f)
									npc.velocity.Y -= 2f;
							}
							else
							{
								npc.velocity.X *= 2f;
								if (npc.velocity.X > 3f)
									npc.velocity.X = 3f;

								if (npc.velocity.X < -3f)
									npc.velocity.X = -3f;

								npc.velocity.Y = -4f;
							}

							npc.netUpdate = true;
						}
					}

					if (npc.type == 120 && npc.velocity.Y < 0f)
						npc.velocity.Y *= 1.1f;

					if (npc.type == 287 && npc.velocity.Y == 0f && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 150f && Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 50f && ((npc.direction > 0 && npc.velocity.X >= 1f) || (npc.direction < 0 && npc.velocity.X <= -1f)))
					{
						npc.velocity.X = 8 * npc.direction;
						npc.velocity.Y = -4f;
						npc.netUpdate = true;
					}

					if (npc.type == 287 && npc.velocity.Y < 0f)
					{
						npc.velocity.X *= 1.2f;
						npc.velocity.Y *= 1.1f;
					}

					if (npc.type == 460 && npc.velocity.Y < 0f)
					{
						npc.velocity.X *= 1.3f;
						npc.velocity.Y *= 1.1f;
					}
				}
			}
			else if (flag8)
			{
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient && npc.type == 120 && npc.ai[3] >= (float)num56)
			{
				int targetTileX = (int)Main.player[npc.target].Center.X / 16;
				int targetTileY = (int)Main.player[npc.target].Center.Y / 16;
				Vector2 chosenTile = Vector2.Zero;
				if (npc.AI_AttemptToFindTeleportSpot(ref chosenTile, targetTileX, targetTileY, 20, 9))
				{
					npc.position.X = chosenTile.X * 16f - (float)(npc.width / 2);
					npc.position.Y = chosenTile.Y * 16f - (float)npc.height;
					npc.ai[3] = -120f;
					npc.netUpdate = true;
				}
			}
			return false;
		}

		private static bool AI_003_Gnomes_ShouldTurnToStone(this NPC npc)
		{
			if (Main.remixWorld)
				return npc.position.Y / 16f > (float)(Main.maxTilesY - 350);

			if (Main.dayTime)
				return WorldGen.InAPlaceWithWind(npc.position, npc.width, npc.height);

			return false;
		}
	}
}

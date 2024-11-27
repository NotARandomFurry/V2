using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.NPCs.Vanilla.NPCAIReference;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Sets
{
	public class SlimeNPC : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => V2Utils.NPCIDSets.Slimes.Contains(entity.type);

		internal int TimeSinceLastJump { get; set; }

		public Vector2 JumpSpeed { get; set; }
		public StatModifier JumpXSpeedModifier { get; set; }
		public StatModifier JumpYSpeedModifier { get; set; }
		public Vector2 FloatSpeed { get; set; }
		public int JumpDelayBase { get; set; }
		public (int Min, int Max) JumpDelayExtra { get; set; }
		public StatModifier JumpDelayModifier { get; set; }

		/// <summary>
		/// Allows this slime to occasionally perform a "high jump" that uses additional modifiers to gain higher velocity.
		/// </summary>
		public bool OccasionalHighJumps { get; set; }
		internal int JumpsUntilNextHighJump { get; set; }
		public int HighJumpFrequency { get; set; }
		public StatModifier HighJumpXModifier { get; set; }
		public StatModifier HighJumpYModifier { get; set; }

		public override void SetDefaults(NPC entity)
		{
			JumpSpeed = new Vector2(2f, 6f);
			FloatSpeed = new Vector2(2f, 6f);
			JumpDelayBase = V2Utils.SensibleTime(
				seconds: 4,
				frames: 40
			);
			JumpDelayExtra = (
				V2Utils.SensibleTime(
					seconds: 0,
					frames: 50
				),
				V2Utils.SensibleTime(
					seconds: 1,
					frames: 40
				)
			);
			OccasionalHighJumps = false;
			HighJumpFrequency = 3;
			JumpsUntilNextHighJump = HighJumpFrequency;
			HighJumpXModifier = StatModifier.Default;
			HighJumpYModifier = StatModifier.Default;
		}

		public override void ResetEffects(NPC npc)
		{
			JumpXSpeedModifier = StatModifier.Default;
			JumpYSpeedModifier = StatModifier.Default;
			JumpDelayModifier = StatModifier.Default;
		}

		public static void OnKilledByDigestion_GrantSlimeMultiPreyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				List<int> distinctSlimes = new List<int>(V2Utils.NPCIDSets.Slimes);
				int distinctSlimesInTummy = 0;
				foreach (PreyData prey in predPlayer.AsPred().StomachTracker.Prey)
				{
					if (prey.Type != PreyType.NPC)
						continue;

					int preyNPCID = prey.ExactType;
					if (distinctSlimes.Contains(preyNPCID))
					{
						distinctSlimesInTummy++;
						distinctSlimes.Remove(preyNPCID);
					}
				}
				if (distinctSlimesInTummy >= 3)
					ModContent.GetInstance<Eat3DifferentSlimes>().TrySetCompletion(predPlayer);
			}
		}

		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			binaryWriter.Write(TimeSinceLastJump);
			binaryWriter.Write(JumpsUntilNextHighJump);
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			TimeSinceLastJump = binaryReader.ReadInt32();
			JumpsUntilNextHighJump = binaryReader.ReadInt32();
		}
	}

	public static partial class GeneralizedAIOverrides
	{
		public static SlimeNPC AsSlime(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out SlimeNPC slime))
				throw new Exception("this slime, supposedly, doesn't exist");

			return slime;
		}
		private static ref float SlimeBonusDrop(this NPC npc) => ref npc.ai[1];
		public static void SimpleSlimeFirstFrameAI(NPC npc, int baseHeight)
		{
			npc.position.Y -= (float)baseHeight * 0.75f;
		}

		public static bool SimpleSlimeAI(NPC npc, float baseScale, int baseWidth, int baseHeight)
		{
			if (PredNPC.GetCurrentBellyWeight(npc) > 0 || npc.AsPred().ExtraWeight > 0)
			{
				npc.scale = baseScale * (1.0f + (0.30f * (float)Math.Sqrt(npc.AsPred().ExtraWeight / npc.AsFood().DefinedBaseSize)));
				float scaleBeforeFood = npc.scale;
				npc.AsPred().MaxStomachCapacity = npc.AsFood().DefinedBaseSize * 1.5 * (npc.scale / baseScale);
				npc.AsFood().DefinedEffectiveSize = npc.AsFood().DefinedBaseSize * (npc.scale / baseScale);
				npc.scale *= 1f + (float)(PredNPC.GetCurrentBellyWeight(npc) / npc.AsFood().DefinedEffectiveSize);
				npc.width = (int)((float)baseWidth * npc.scale);
				npc.height = (int)((float)baseHeight * npc.scale);
			}

			float movementModifier = 1f + (float)PredNPC.GetCurrentBellyWeight(npc);
			movementModifier /= npc.scale / baseScale;

			if (npc.type == NPCID.BlueSlime && (npc.SlimeBonusDrop() == 1f || npc.SlimeBonusDrop() == 2f || npc.SlimeBonusDrop() == 3f))
				npc.SlimeBonusDrop() = -1f;

			if (npc.type == NPCID.BlueSlime && npc.SlimeBonusDrop() == ItemID.FallenStar)
			{
				float num = 0.3f;
				Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 0.8f * num, 0.7f * num, 0.1f * num);
				if (Main.rand.NextBool(12))
				{
					Dust dust = Dust.NewDustPerfect(npc.Center + new Vector2(0f, (float)npc.height * 0.2f) + Main.rand.NextVector2CircularEdge(npc.width, (float)npc.height * 0.6f) * (0.3f + Main.rand.NextFloat() * 0.5f), 228, new Vector2(0f, (0f - Main.rand.NextFloat()) * 0.3f - 1.5f), 127);
					dust.scale = 0.5f;
					dust.fadeIn = 1.1f;
					dust.noGravity = true;
					dust.noLight = true;
				}
			}

			if (npc.type == NPCID.BlueSlime && npc.SlimeBonusDrop() == 0f && Main.netMode != NetmodeID.MultiplayerClient && npc.value > 0f)
			{
				npc.SlimeBonusDrop() = -1f;
				if (Main.remixWorld && npc.ai[0] != -999f && Main.rand.NextBool(3))
				{
					npc.SlimeBonusDrop() = ItemID.FallenStar;
					npc.netUpdate = true;
				}
				else if (Main.rand.NextBool(20))
				{
					int num2 = DetermineSlimeBonusItem(npc.netID, npc.ai[0] == -999f);
					npc.SlimeBonusDrop() = num2;
					npc.netUpdate = true;
				}
			}

			if (npc.type == NPCID.BlueSlime && npc.ai[0] == -999f)
			{
				npc.frame.Y = 0;
				npc.frameCounter = 0.0;
				npc.rotation = 0f;
				return false;
			}

			if (npc.type == NPCID.RainbowSlime)
			{
				float num3 = (float)Main.DiscoR / 255f;
				float num4 = (float)Main.DiscoG / 255f;
				float num5 = (float)Main.DiscoB / 255f;
				num3 *= 1f;
				num4 *= 1f;
				num5 *= 1f;
				Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), num3, num4, num5);
				npc.AI_001_SetRainbowSlimeColor();
			}

			bool flag = false;
			if (!Main.dayTime || npc.life != npc.lifeMax || (double)npc.position.Y > Main.worldSurface * 16.0 || Main.slimeRain)
				flag = true;

			if (Main.remixWorld && npc.type == NPCID.LavaSlime && npc.life == npc.lifeMax)
				flag = false;

			if (npc.type == NPCID.CorruptSlime)
			{
				flag = true;
				if (Main.rand.NextBool(30))
				{
					npc.position += npc.netOffset;
					int num6 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Demonite, 0f, 0f, npc.alpha, npc.color);
					Main.dust[num6].velocity *= 0.3f;
					npc.position -= npc.netOffset;
				}
			}

			if (npc.type == NPCID.Crimslime)
				flag = true;

			if (npc.type == NPCID.HoppinJack)
				flag = true;

			if (npc.type == NPCID.GoldenSlime)
				flag = true;

			if (npc.type == NPCID.RainbowSlime)
			{
				flag = true;
				npc.ai[0] += 2f;
			}

			if (npc.type == NPCID.IceSlime && Main.rand.NextBool(10))
			{
				npc.position += npc.netOffset;
				int num7 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Snow);
				Main.dust[num7].noGravity = true;
				Main.dust[num7].velocity *= 0.1f;
				npc.position -= npc.netOffset;
			}

			if (npc.type == NPCID.GoldenSlime)
			{
				Color color = new Color(204, 181, 72, 255);
				Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), (float)(int)color.R / 255f * 1.1f, (float)(int)color.G / 255f * 1.1f, (float)(int)color.B / 255f * 1.1f);
				if (npc.velocity.Length() > 1f || !Main.rand.NextBool(4))
				{
					int num8 = 8;
					Vector2 vector = npc.position + new Vector2(-num8, -num8);
					int num9 = npc.width + num8 * 2;
					int num10 = npc.height + num8 * 2;
					npc.position += npc.netOffset;
					int num11 = Dust.NewDust(vector, num9, num10, DustID.GoldCoin);
					Main.dust[num11].noGravity = true;
					Main.dust[num11].noLightEmittence = true;
					Main.dust[num11].velocity *= 0.2f;
					Main.dust[num11].scale = 1.5f;
					npc.position -= npc.netOffset;
				}
			}

			if (npc.type == NPCID.ShimmerSlime)
			{
				Lighting.AddLight(npc.Center, 23);
				if ((npc.velocity.Length() > 1f && Main.rand.NextBool(3)) || Main.rand.NextBool(5))
				{
					Dust dust2 = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(npc.Hitbox), 306);
					dust2.noGravity = true;
					dust2.noLightEmittence = true;
					dust2.alpha = 127;
					dust2.color = Main.hslToRgb(((float)Main.timeForVisualEffects / 300f + Main.rand.NextFloat() * 0.1f) % 1f, 1f, 0.65f);
					dust2.color.A = 0;
					dust2.velocity = dust2.position - npc.Center;
					dust2.velocity *= 0.1f;
					dust2.velocity.X *= 0.25f;
					if (dust2.velocity.Y > 0f)
						dust2.velocity.Y *= -1f;

					dust2.scale = Main.rand.NextFloat() * 0.3f + 0.5f;
					dust2.fadeIn = 0.9f;
					dust2.position += npc.netOffset;
				}
			}

			if (npc.type == NPCID.SpikedIceSlime)
			{
				if (Main.rand.NextBool(8))
				{
					npc.position += npc.netOffset;
					int num12 = Dust.NewDust(npc.position - npc.velocity, npc.width, npc.height, DustID.Snow);
					Main.dust[num12].noGravity = true;
					Main.dust[num12].velocity *= 0.15f;
					npc.position -= npc.netOffset;
				}

				flag = true;
				if (npc.localAI[0] > 0f)
					npc.localAI[0] -= 1f;

				if (!npc.wet && !Main.player[npc.target].npcTypeNoAggro[npc.type])
				{
					Vector2 vector2 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num13 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector2.X;
					float num14 = Main.player[npc.target].position.Y - vector2.Y;
					float num15 = (float)Math.Sqrt(num13 * num13 + num14 * num14);
					if (Main.expertMode && num15 < 120f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
					{
						npc.ai[0] = -40f;
						if (npc.velocity.Y == 0f)
							npc.velocity.X *= 0.9f;

						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
						{
							for (int i = 0; i < 5; i++)
							{
								Vector2 vector3 = new Vector2(i - 2, -4f);
								vector3.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
								vector3.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
								vector3.Normalize();
								vector3 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;
								int attackDamage_ForProjectiles = npc.GetAttackDamage_ForProjectiles(9f, 9f);
								Projectile.NewProjectile(npc.GetSource_FromAI(), vector2.X, vector2.Y, vector3.X, vector3.Y, 174, attackDamage_ForProjectiles, 0f, Main.myPlayer);
								npc.localAI[0] = 30f;
							}
						}
					}
					else if (num15 < 200f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
					{
						npc.ai[0] = -40f;
						if (npc.velocity.Y == 0f)
							npc.velocity.X *= 0.9f;

						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
						{
							num14 = Main.player[npc.target].position.Y - vector2.Y - (float)Main.rand.Next(0, 200);
							num15 = (float)Math.Sqrt(num13 * num13 + num14 * num14);
							num15 = 4.5f / num15;
							num13 *= num15;
							num14 *= num15;
							npc.localAI[0] = 50f;
							Projectile.NewProjectile(npc.GetSource_FromAI(), vector2.X, vector2.Y, num13, num14, 174, 9, 0f, Main.myPlayer);
						}
					}
				}
			}

			if (npc.type == NPCID.SlimeSpiked)
			{
				flag = true;
				if (npc.localAI[0] > 0f)
					npc.localAI[0] -= 1f;

				if (!npc.wet && !Main.player[npc.target].npcTypeNoAggro[npc.type])
				{
					Vector2 vector4 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num16 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector4.X;
					float num17 = Main.player[npc.target].position.Y - vector4.Y;
					float num18 = (float)Math.Sqrt(num16 * num16 + num17 * num17);
					if (Main.expertMode && num18 < 120f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
					{
						npc.ai[0] = -40f;
						if (npc.velocity.Y == 0f)
							npc.velocity.X *= 0.9f;

						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
						{
							for (int j = 0; j < 5; j++)
							{
								Vector2 vector5 = new Vector2(j - 2, -4f);
								vector5.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
								vector5.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
								vector5.Normalize();
								vector5 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;
								int attackDamage_ForProjectiles2 = npc.GetAttackDamage_ForProjectiles(9f, 9f);
								Projectile.NewProjectile(npc.GetSource_FromAI(), vector4.X, vector4.Y, vector5.X, vector5.Y, 605, attackDamage_ForProjectiles2, 0f, Main.myPlayer);
								npc.localAI[0] = 30f;
							}
						}
					}
					else if (num18 < 200f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
					{
						npc.ai[0] = -40f;
						if (npc.velocity.Y == 0f)
							npc.velocity.X *= 0.9f;

						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
						{
							num17 = Main.player[npc.target].position.Y - vector4.Y - (float)Main.rand.Next(0, 200);
							num18 = (float)Math.Sqrt(num16 * num16 + num17 * num17);
							num18 = 4.5f / num18;
							num16 *= num18;
							num17 *= num18;
							npc.localAI[0] = 50f;
							Projectile.NewProjectile(npc.GetSource_FromAI(), vector4.X, vector4.Y, num16, num17, 605, 9, 0f, Main.myPlayer);
						}
					}
				}
			}

			if (npc.type == NPCID.QueenSlimeMinionBlue)
			{
				flag = true;
				if (npc.localAI[0] > 0f)
					npc.localAI[0] -= 1f;

				if (!npc.wet && Main.player[npc.target].active && !Main.player[npc.target].dead && !Main.player[npc.target].npcTypeNoAggro[npc.type])
				{
					Player obj = Main.player[npc.target];
					Vector2 center = npc.Center;
					float num19 = obj.Center.X - center.X;
					float num20 = obj.Center.Y - center.Y;
					float num21 = (float)Math.Sqrt(num19 * num19 + num20 * num20);
					int num22 = NPC.CountNPCS(NPCID.QueenSlimeMinionBlue);
					if (Main.expertMode && num22 < 5 && Math.Abs(num19) < 500f && Math.Abs(num20) < 550f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
					{
						npc.ai[0] = -40f;
						if (npc.velocity.Y == 0f)
							npc.velocity.X *= 0.9f;

						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
						{
							for (int k = 0; k < 3; k++)
							{
								Vector2 vector6 = new Vector2(k - 1, -4f);
								vector6.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
								vector6.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
								vector6.Normalize();
								vector6 *= 6f + (float)Main.rand.Next(-50, 51) * 0.01f;
								if (num21 > 350f)
									vector6 *= 2f;
								else if (num21 > 250f)
									vector6 *= 1.5f;

								int attackDamage_ForProjectiles_MultiLerp = npc.GetAttackDamage_ForProjectiles_MultiLerp(15f, 17f, 20f);
								Projectile.NewProjectile(npc.GetSource_FromAI(), center.X, center.Y, vector6.X, vector6.Y, 920, attackDamage_ForProjectiles_MultiLerp, 0f, Main.myPlayer);
								npc.localAI[0] = 25f;
								if (num22 > 4)
									break;
							}
						}
					}
					else if (Math.Abs(num19) < 500f && Math.Abs(num20) < 550f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
					{
						float num23 = num21;
						npc.ai[0] = -40f;
						if (npc.velocity.Y == 0f)
							npc.velocity.X *= 0.9f;

						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
						{
							num20 = Main.player[npc.target].position.Y - center.Y - (float)Main.rand.Next(0, 200);
							num21 = (float)Math.Sqrt(num19 * num19 + num20 * num20);
							num21 = 4.5f / num21;
							num21 *= 2f;
							if (num23 > 350f)
								num21 *= 2f;
							else if (num23 > 250f)
								num21 *= 1.5f;

							num19 *= num21;
							num20 *= num21;
							npc.localAI[0] = 50f;
							int attackDamage_ForProjectiles_MultiLerp2 = npc.GetAttackDamage_ForProjectiles_MultiLerp(15f, 17f, 20f);
							Projectile.NewProjectile(npc.GetSource_FromAI(), center.X, center.Y, num19, num20, 920, attackDamage_ForProjectiles_MultiLerp2, 0f, Main.myPlayer);
						}
					}
				}
			}

			if (npc.type == NPCID.QueenSlimeMinionPink)
			{
				flag = true;
				if (npc.localAI[0] > 0f)
					npc.localAI[0] -= 1f;

				if (!npc.wet && Main.player[npc.target].active && !Main.player[npc.target].dead && !Main.player[npc.target].npcTypeNoAggro[npc.type])
				{
					Player obj2 = Main.player[npc.target];
					Vector2 center2 = npc.Center;
					float num24 = obj2.Center.X - center2.X;
					float num25 = obj2.Center.Y - center2.Y;
					float num26 = (float)Math.Sqrt(num24 * num24 + num25 * num25);
					float num27 = num26;
					if (Math.Abs(num24) < 500f && Math.Abs(num25) < 550f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
					{
						npc.ai[0] = -40f;
						if (npc.velocity.Y == 0f)
							npc.velocity.X *= 0.9f;

						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
						{
							num25 = Main.player[npc.target].position.Y - center2.Y - (float)Main.rand.Next(0, 200);
							num26 = (float)Math.Sqrt(num24 * num24 + num25 * num25);
							num26 = 4.5f / num26;
							num26 *= 2f;
							if (num27 > 350f)
								num26 *= 1.75f;
							else if (num27 > 250f)
								num26 *= 1.25f;

							num24 *= num26;
							num25 *= num26;
							npc.localAI[0] = 40f;
							if (Main.expertMode)
								npc.localAI[0] = 30f;

							int attackDamage_ForProjectiles_MultiLerp3 = npc.GetAttackDamage_ForProjectiles_MultiLerp(15f, 17f, 20f);
							Projectile.NewProjectile(npc.GetSource_FromAI(), center2.X, center2.Y, num24, num25, 921, attackDamage_ForProjectiles_MultiLerp3, 0f, Main.myPlayer);
						}
					}
				}
			}

			if (npc.type == NPCID.SpikedJungleSlime)
			{
				flag = true;
				if (npc.localAI[0] > 0f)
					npc.localAI[0] -= 1f;

				if (!npc.wet && !Main.player[npc.target].npcTypeNoAggro[npc.type])
				{
					Vector2 vector7 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num28 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector7.X;
					float num29 = Main.player[npc.target].position.Y - vector7.Y;
					float num30 = (float)Math.Sqrt(num28 * num28 + num29 * num29);
					if (Main.expertMode && num30 < 200f && Collision.CanHit(new Vector2(npc.position.X, npc.position.Y - 20f), npc.width, npc.height + 20, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
					{
						npc.ai[0] = -40f;
						if (npc.velocity.Y == 0f)
							npc.velocity.X *= 0.9f;

						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
						{
							for (int l = 0; l < 5; l++)
							{
								Vector2 vector8 = new Vector2(l - 2, -2f);
								vector8.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.02f;
								vector8.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.02f;
								vector8.Normalize();
								vector8 *= 3f + (float)Main.rand.Next(-50, 51) * 0.01f;
								int attackDamage_ForProjectiles3 = npc.GetAttackDamage_ForProjectiles(13f, 13f);
								Projectile.NewProjectile(npc.GetSource_FromAI(), vector7.X, vector7.Y, vector8.X, vector8.Y, 176, attackDamage_ForProjectiles3, 0f, Main.myPlayer);
								npc.localAI[0] = 80f;
							}
						}
					}

					if (num30 < 400f && Collision.CanHit(new Vector2(npc.position.X, npc.position.Y - 20f), npc.width, npc.height + 20, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
					{
						npc.ai[0] = -80f;
						if (npc.velocity.Y == 0f)
							npc.velocity.X *= 0.9f;

						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
						{
							num29 = Main.player[npc.target].position.Y - vector7.Y - (float)Main.rand.Next(-30, 20);
							num29 -= num30 * 0.05f;
							num28 = Main.player[npc.target].position.X - vector7.X - (float)Main.rand.Next(-20, 20);
							num30 = (float)Math.Sqrt(num28 * num28 + num29 * num29);
							num30 = 7f / num30;
							num28 *= num30;
							num29 *= num30;
							npc.localAI[0] = 65f;
							Projectile.NewProjectile(npc.GetSource_FromAI(), vector7.X, vector7.Y, num28, num29, 176, 13, 0f, Main.myPlayer);
						}
					}
				}
			}

			if (npc.type == NPCID.LavaSlime)
			{
				npc.position += npc.netOffset;
				Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 1f, 0.3f, 0.1f);
				int num31 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Torch, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default, 1.7f);
				Main.dust[num31].noGravity = true;
				npc.position -= npc.netOffset;
			}

			if (npc.ai[2] > 1f)
				npc.ai[2] -= 1f;

			if (npc.wet)
			{
				if (npc.collideY)
					npc.velocity.Y = -2f;

				if (npc.velocity.Y < 0f && npc.ai[3] == npc.position.X)
				{
					npc.direction *= -1;
					npc.ai[2] = 200f;
				}

				if (npc.velocity.Y > 0f)
					npc.ai[3] = npc.position.X;

				if (npc.type == NPCID.LavaSlime && !Main.remixWorld)
				{
					if (npc.velocity.Y > 2f)
						npc.velocity.Y *= 0.9f;
					else if (npc.directionY < 0)
						npc.velocity.Y -= 0.8f;

					npc.velocity.Y -= 0.5f;
					if (npc.velocity.Y < -10f)
						npc.velocity.Y = -10f;
				}
				else
				{
					if (npc.velocity.Y > 2f)
						npc.velocity.Y *= 0.9f;

					npc.velocity.Y -= 0.5f;
					if (npc.velocity.Y < -4f)
						npc.velocity.Y = -4f;
				}

				if (npc.ai[2] == 1f && flag)
					npc.TargetClosest();
			}

			npc.aiAction = 0;
			if (npc.ai[2] == 0f)
			{
				npc.ai[0] = -100f;
				npc.ai[2] = 1f;
				npc.TargetClosest();
			}

			if (npc.velocity.Y == 0f)
			{
				if (npc.collideY && npc.oldVelocity.Y != 0f && Collision.SolidCollision(npc.position, npc.width, npc.height))
					npc.position.X -= npc.velocity.X + (float)npc.direction;

				if (npc.ai[3] == npc.position.X)
				{
					npc.direction *= -1;
					npc.ai[2] = 200f;
				}

				npc.ai[3] = 0f;
				npc.velocity.X *= 0.8f;
				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					npc.velocity.X = 0f;

				if (flag)
					npc.ai[0] += 1f;

				npc.ai[0] += 1f;
				if (npc.type == NPCID.LavaSlime && !Main.remixWorld)
					npc.ai[0] += 2f;

				if (npc.type == NPCID.DungeonSlime)
					npc.ai[0] += 3f;

				if (npc.type == NPCID.GoldenSlime)
					npc.ai[0] += 3f;

				if (npc.type == NPCID.IlluminantSlime)
					npc.ai[0] += 2f;

				if (npc.type == NPCID.Crimslime)
					npc.ai[0] += 1f;

				if (npc.type == NPCID.QueenSlimeMinionBlue)
					npc.ai[0] += 5f;

				if (npc.type == NPCID.QueenSlimeMinionPink)
					npc.ai[0] += 3f;

				if (npc.type == NPCID.HoppinJack)
				{
					float num32 = (1 - npc.life / npc.lifeMax) * 10;
					npc.ai[0] += num32;
				}

				if (npc.type == NPCID.CorruptSlime)
				{
					if (npc.scale >= 0f)
						npc.ai[0] += 4f;
					else
						npc.ai[0] += 1f;
				}

				npc.AsSlime().TimeSinceLastJump++;
				if (npc.AsSlime().TimeSinceLastJump >= npc.AsSlime().JumpDelayBase + Main.rand.Next(npc.AsSlime().JumpDelayExtra.Min, npc.AsSlime().JumpDelayExtra.Max + 1))
				{
					npc.netUpdate = true;
					if (flag && npc.ai[2] == 1f)
						npc.TargetClosest();

					npc.AsSlime().JumpSpeed = new Vector2(
						npc.AsSlime().JumpXSpeedModifier.ApplyTo(npc.AsSlime().JumpSpeed.X),
						npc.AsSlime().JumpYSpeedModifier.ApplyTo(npc.AsSlime().JumpSpeed.Y)
					);
					if (npc.AsSlime().OccasionalHighJumps)
					{
						if (npc.AsSlime().JumpsUntilNextHighJump <= 0)
						{
							npc.velocity.X += npc.AsSlime().HighJumpXModifier.ApplyTo(npc.AsSlime().JumpSpeed.X) * npc.direction / movementModifier;
							npc.velocity.Y = -npc.AsSlime().HighJumpYModifier.ApplyTo(npc.AsSlime().JumpSpeed.Y) / movementModifier;

							npc.AsSlime().JumpsUntilNextHighJump = npc.AsSlime().HighJumpFrequency;
							npc.AsSlime().TimeSinceLastJump = 0;
							npc.ai[3] = npc.position.X;
						}
						else
						{
							npc.velocity.X += npc.AsSlime().JumpSpeed.X * npc.direction / movementModifier;
							npc.velocity.Y = -npc.AsSlime().JumpSpeed.Y / movementModifier;

							npc.AsSlime().JumpsUntilNextHighJump -= 1;
							npc.AsSlime().TimeSinceLastJump = 0;
							npc.ai[3] = npc.position.X;
						}
					}
					else
					{
						npc.velocity.X += npc.AsSlime().JumpSpeed.X * npc.direction / movementModifier;
						npc.velocity.Y = -npc.AsSlime().JumpSpeed.Y / movementModifier;

						npc.AsSlime().TimeSinceLastJump = 0;
					}
				}
				else if (npc.AsSlime().TimeSinceLastJump >= npc.AsSlime().JumpDelayBase - 20)
					npc.aiAction = 1;
			}
			else if (npc.target < 255 && ((npc.direction == 1 && npc.velocity.X < 3f) || (npc.direction == -1 && npc.velocity.X > -3f)))
			{
				if (npc.collideX && Math.Abs(npc.velocity.X) == 0.2f)
					npc.position.X -= 1.4f * (float)npc.direction;

				if (npc.collideY && npc.oldVelocity.Y != 0f && Collision.SolidCollision(npc.position, npc.width, npc.height))
					npc.position.X -= npc.velocity.X + (float)npc.direction;

				if ((npc.direction == -1 && (double)npc.velocity.X < 0.01) || (npc.direction == 1 && (double)npc.velocity.X > -0.01))
					npc.velocity.X += 0.2f * (float)npc.direction;
				else
					npc.velocity.X *= 0.93f;
			}
			return false;
		}

		public static int DetermineSlimeBonusItem(int slimeType, bool isBallooned)
		{
			int num = Main.rand.Next(4);
			if (isBallooned)
			{
				switch (Main.rand.Next(13))
				{
					default:
						return ItemID.KiteBlue;
					case 1:
						return ItemID.KiteBlueAndYellow;
					case 2:
						return ItemID.KiteRed;
					case 3:
						return ItemID.KiteRedAndYellow;
					case 4:
						return ItemID.KiteYellow;
					case 5:
						return ItemID.KiteBunny;
					case 6:
						return ItemID.KiteGoldfish;
					case 7:
					case 8:
					case 9:
						return ItemID.PaperAirplaneA;
					case 10:
					case 11:
					case 12:
						return ItemID.PaperAirplaneB;
				}
			}

			switch (num)
			{
				case 0:
					switch (Main.rand.Next(7))
					{
						case 0:
							return ItemID.SwiftnessPotion;
						case 1:
							return ItemID.IronskinPotion;
						case 2:
							return ItemID.SpelunkerPotion;
						case 3:
							return ItemID.MiningPotion;
						default:
							if (Main.netMode != NetmodeID.SinglePlayer && Main.rand.NextBool(2))
								return ItemID.WormholePotion;
							return ItemID.RecallPotion;
					}
				case 1:
					return Main.rand.Next(4) switch
					{
						0 => ItemID.Torch,
						1 => ItemID.Bomb,
						2 => ItemID.Rope,
						_ => ItemID.Heart,
					};
				case 2:
					return Main.rand.Next(ItemID.Sets.OreDropsFromSlime.Keys.ToList()); // TML
				/*
				if (Main.rand.Next(2) == 0)
					return Main.rand.Next(11, 15);
				return Main.rand.Next(699, 703);
				*/
				default:
					switch (Main.rand.Next(3))
					{
						case 0:
							return ItemID.CopperCoin;
						case 1:
							return ItemID.SilverCoin;
						default:
							return ItemID.GoldCoin;
					}
			}
		}
	}
}

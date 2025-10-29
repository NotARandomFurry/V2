using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using V2.Core;
using V2.Items.Voraria;
using V2.Items.Voraria.Consumables.Catchables;
using V2.NPCs.Vanilla.Cavern;
using V2.NPCs.Vanilla.Forest;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;
using V2.Sounds.Vore;
using static V2.Projectiles.Vanilla.Summons.Pets.FairyPrincessStuff.Animations.BaseWeight.OVHerOwnFuckingMother;

namespace V2.NPCs.Sets
{
	public static class ButterflyGroupStuff
	{
		public static Butterfly AsAButterfly(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Butterfly tastyLittleTreatThatFluttersInsideYou))
				throw new Exception("this instance of a gem critter, supposedly, doesn't exist");

			return tastyLittleTreatThatFluttersInsideYou;
		}
	}

	public class Butterfly : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => V2Utils.NPCIDSets.Butterflies.Contains(entity.type);

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 0.035;

			npc.AsFood().OnSwallowedBy += OnSwallowedBy_GrantButterflyGroupMultiPreyGoal;
		}

		public static void OnSwallowedBy_GrantButterflyGroupMultiPreyGoal(NPC npc, Entity pred)
		{
			if (pred is not Player predPlayer)
				return;
			if (predPlayer.AsPred().StomachTracker is null)
				return;

			List<int> butterflies = [.. V2Utils.NPCIDSets.Butterflies];
			int butterfliesInTummy = 0;
			if (predPlayer.AsPred().StomachTracker.PreyQueue?.Count <= 0)
				goto checkMainPreyList;

			foreach (PreyData prey in predPlayer.AsPred().StomachTracker.PreyQueue)
			{
				if (prey.Type != PreyType.NPC)
					continue;

				int preyNPCID = prey.ExactType;
				if (butterflies.Contains(preyNPCID))
					butterfliesInTummy++;
			}

			checkMainPreyList:
			if (predPlayer.AsPred().StomachTracker.Prey?.Count <= 0)
				goto validateGoal;

			foreach (PreyData prey in predPlayer.AsPred().StomachTracker.Prey)
			{
				if (prey.Type != PreyType.NPC)
					continue;

				int preyNPCID = prey.ExactType;
				if (butterflies.Contains(preyNPCID))
					butterfliesInTummy++;
			}

			validateGoal:
			if (butterfliesInTummy >= 3)
				ModContent.GetInstance<StomachButterflies>().TrySetCompletion(predPlayer);
		}
	}

	public static partial class GeneralizedAIOverrides
	{
		public static bool SimpleButterflyAI(NPC npc)
		{
			float num = npc.ai[0];
			float num2 = npc.ai[1];
			if (npc.type == NPCID.EmpressButterfly)
			{
				Vector3 rgb = Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.33f % 1f, 1f, 0.5f).ToVector3() * 0.3f;
				rgb += Vector3.One * 0.1f;
				Lighting.AddLight(npc.Center, rgb);
				int num3 = 60;
				bool flag = false;
				int num4 = 50;
				NPCAimedTarget targetData = npc.GetTargetData();
				if (targetData.Invalid || targetData.Center.Distance(npc.Center) >= 300f)
					flag = true;

				if (!Main.remixWorld && !targetData.Invalid && targetData.Type == NPCTargetType.Player && !Main.player[npc.target].ZoneHallow)
				{
					num4 = num3;
					flag = true;
				}

				npc.ai[2] = MathHelper.Clamp(npc.ai[2] + (float)flag.ToDirectionInt(), 0f, num4);
				if (npc.ai[2] >= (float)num3)
				{
					npc.active = false;
					if (Main.netMode != NetmodeID.MultiplayerClient)
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);

					return false;
				}

				npc.Opacity = Utils.GetLerpValue(num3, (float)num4 / 2f, npc.ai[2], clamped: true);
				if (npc.ai[2] > 0f && Main.rand.NextBool(5))
				{
					float num6 = MathHelper.Lerp(0.9f, 0.6f, npc.Opacity);
					Color newColor = Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.3f % 1f, 1f, 0.5f) * 0.5f;
					int num7 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.RainbowMk2, 0f, 0f, 0, newColor);
					Dust num7d = Main.dust[num7];
					num7d.position = npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height);
					num7d.velocity *= Main.rand.NextFloat() * 0.8f;
					num7d.velocity += npc.velocity * 0.6f;
					num7d.noGravity = true;
					num7d.fadeIn = 0.6f + Main.rand.NextFloat() * 0.7f * num6;
					num7d.scale = 0.35f;
					if (num7 != 6000)
					{
						Dust dust = Dust.CloneDust(num7);
						dust.scale /= 2f;
						dust.fadeIn *= 0.85f;
						dust.color = new Color(255, 255, 255, 255) * 0.5f;
					}
				}

				npc.dontTakeDamage = npc.ai[2] >= (float)(num4 / 2);
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (npc.type == NPCID.Butterfly)
				{
					if (npc.ai[2] == 0f)
					{
						WeightedRandom<int> possibleSpecies = new WeightedRandom<int>([
							new((int)NormalButterflyStuff.VanillaButterflySpecies.Monarch, 25),
							new((int)NormalButterflyStuff.VanillaButterflySpecies.PurpleEmperor, 2),
							new((int)NormalButterflyStuff.VanillaButterflySpecies.RedAdmiral, 6),
							new((int)NormalButterflyStuff.VanillaButterflySpecies.Ulysses, 15),
							new((int)NormalButterflyStuff.VanillaButterflySpecies.Sulphur, 22),
							new((int)NormalButterflyStuff.VanillaButterflySpecies.TreeNymph, 1),
							new((int)NormalButterflyStuff.VanillaButterflySpecies.ZebraSwallowtail, 19),
							new((int)NormalButterflyStuff.VanillaButterflySpecies.Julia, 10),
						]);
						npc.ai[2] = 1 + possibleSpecies;
					}
					else if (npc.ai[2] > 8f)
					{
						int weightGainTarget = (int)Math.Floor(npc.ai[2] / 8.0);
						NormalButterfly.SetVisualWeightStage(npc, weightGainTarget);
						npc.ai[2] -= 8f * weightGainTarget;
					}
				}

				if (npc.ai[3] == 0f)
					npc.ai[3] = (float)Main.rand.Next(75, 111) * 0.01f;

				npc.localAI[0] -= 1f;
				if (npc.localAI[0] <= 0f)
				{
					npc.localAI[0] = Main.rand.Next(90, 240);
					npc.TargetClosest();
					float num17 = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);
					if (num17 > 700f && npc.localAI[3] == 0f)
					{
						float num18 = (float)Main.rand.Next(50, 151) * 0.01f;
						if (num17 > 1000f)
							num18 = (float)Main.rand.Next(150, 201) * 0.01f;
						else if (num17 > 850f)
							num18 = (float)Main.rand.Next(100, 151) * 0.01f;

						int num19 = npc.direction * Main.rand.Next(100, 251);
						int num20 = Main.rand.Next(-50, 51);
						if (npc.position.Y > Main.player[npc.target].position.Y - 100f)
							num20 -= Main.rand.Next(100, 251);

						float num21 = num18 / (float)Math.Sqrt(num19 * num19 + num20 * num20);
						num = (float)num19 * num21;
						num2 = (float)num20 * num21;
					}
					else
					{
						npc.localAI[3] = 1f;
						float num22 = (float)Main.rand.Next(26, 301) * 0.01f;
						int num23 = Main.rand.Next(-100, 101);
						int num24 = Main.rand.Next(-100, 101);
						float num25 = num22 / (float)Math.Sqrt(num23 * num23 + num24 * num24);
						num = (float)num23 * num25;
						num2 = (float)num24 * num25;
					}

					npc.netUpdate = true;
				}
			}

			npc.scale = npc.ai[3];
			int num26 = 60;
			npc.velocity.X = (npc.velocity.X * (float)(num26 - 1) + num) / (float)num26;
			npc.velocity.Y = (npc.velocity.Y * (float)(num26 - 1) + num2) / (float)num26;
			if (npc.velocity.Y > 0f)
			{
				int num27 = 3;
				int num28 = (int)npc.Center.X / 16;
				int num29 = (int)npc.Center.Y / 16;
				for (int j = num29; j < num29 + num27; j++)
				{
					if (Main.tile[num28, j] != null && ((Main.tile[num28, j].HasUnactuatedTile && Main.tileSolid[Main.tile[num28, j].TileType]) || Main.tile[num28, j].LiquidAmount > 0))
					{
						num2 *= -1f;
						if (npc.velocity.Y > 0f)
							npc.velocity.Y *= 0.9f;
					}
				}
			}

			if (npc.velocity.Y < 0f)
			{
				int num30 = 30;
				bool flag2 = false;
				int num31 = (int)npc.Center.X / 16;
				int num32 = (int)npc.Center.Y / 16;
				for (int k = num32; k < num32 + num30; k++)
				{
					if (Main.tile[num31, k] != null && Main.tile[num31, k].HasUnactuatedTile && Main.tileSolid[Main.tile[num31, k].TileType])
						flag2 = true;
				}

				if (!flag2)
				{
					num2 *= -1f;
					if (npc.velocity.Y < 0f)
						npc.velocity.Y *= 0.9f;
				}
			}

			if (npc.localAI[1] > 0f)
			{
				npc.localAI[1] -= 1f;
			}
			else
			{
				npc.localAI[1] = 15f;
				if (npc.type == NPCID.EmpressButterfly)
					npc.localAI[1] = 10f;

				float num33 = 0f;
				Vector2 zero = Vector2.Zero;
				for (int l = 0; l < 200; l++)
				{
					NPC nPC = Main.npc[l];
					if (nPC.active && nPC.damage > 0 && !nPC.friendly && nPC.Hitbox.Distance(npc.Center) <= 100f && !nPC.IsFoodFor(npc))
					{
						num33 += 1f;
						zero += npc.DirectionFrom(nPC.Center);
					}
				}

				if (num33 > 0f)
				{
					zero /= num33;
					zero *= 2f;
					npc.velocity += zero;
					if (npc.velocity.Length() > 16f)
						npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * 16f;
				}
			}

			if (npc.collideX)
			{
				num = ((!(npc.velocity.X < 0f)) ? (0f - Math.Abs(num)) : Math.Abs(num));
				npc.velocity.X *= -0.2f;
			}

			if (npc.velocity.X < 0f)
				npc.direction = -1;

			if (npc.velocity.X > 0f)
				npc.direction = 1;

			npc.ai[0] = num;
			npc.ai[1] = num2;
			if (npc.type == NPCID.Butterfly)
			{
				switch (npc.AsPred().GetVisualWeightStage.Invoke(npc))
				{
					case 0:
					default:
						npc.catchItem = (short)(1994f + npc.ai[2] - 1f);
						npc.AsPred().MaxStomachCapacity = 1.10;
						npc.AsFood().WellFedPower = 0.05;
						npc.AsFood().CalorieMultiplier = 0.50;
						break;
					case 1:
						npc.catchItem = (npc.ai[2] - 1) switch
						{
							(int)NormalButterflyStuff.VanillaButterflySpecies.Monarch => ModContent.ItemType<CaughtButterflyMonarchWG1>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.PurpleEmperor => ModContent.ItemType<CaughtButterflyPurpleEmperorWG1>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.RedAdmiral => ModContent.ItemType<CaughtButterflyRedAdmiralWG1>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.Ulysses => ModContent.ItemType<CaughtButterflyUlyssesWG1>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.Sulphur => ModContent.ItemType<CaughtButterflySulphurWG1>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.TreeNymph => ModContent.ItemType<CaughtButterflyTreeNymphWG1>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.ZebraSwallowtail => ModContent.ItemType<CaughtButterflyZebraSwallowtailWG1>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.Julia => ModContent.ItemType<CaughtButterflyJuliaWG1>(),
							_ => ModContent.ItemType<CaughtButterflyMonarchWG1>(),
						};
						npc.AsPred().MaxStomachCapacity = 1.65;
						npc.AsFood().WellFedPower = 0.075;
						npc.AsFood().CalorieMultiplier = 0.75;
						break;
					case 2:
						npc.catchItem = (npc.ai[2] - 1) switch
						{
							(int)NormalButterflyStuff.VanillaButterflySpecies.Monarch => ModContent.ItemType<CaughtButterflyMonarchWG2>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.PurpleEmperor => ModContent.ItemType<CaughtButterflyPurpleEmperorWG2>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.RedAdmiral => ModContent.ItemType<CaughtButterflyRedAdmiralWG2>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.Ulysses => ModContent.ItemType<CaughtButterflyUlyssesWG2>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.Sulphur => ModContent.ItemType<CaughtButterflySulphurWG2>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.TreeNymph => ModContent.ItemType<CaughtButterflyTreeNymphWG2>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.ZebraSwallowtail => ModContent.ItemType<CaughtButterflyZebraSwallowtailWG2>(),
							(int)NormalButterflyStuff.VanillaButterflySpecies.Julia => ModContent.ItemType<CaughtButterflyJuliaWG2>(),
							_ => ModContent.ItemType<CaughtButterflyMonarchWG1>(),
						};
						npc.AsPred().MaxStomachCapacity = 2.40;
						npc.AsFood().WellFedPower = 0.011;
						npc.AsFood().CalorieMultiplier = 1.10;
						break;
				}
			}
			if (npc.type == NPCID.HellButterfly)
			{
				npc.position += npc.netOffset;
				Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.6f, 0.3f, 0.1f);
				if (Main.rand.NextBool(60))
				{
					int num34 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, 0f, 0f, 254);
					Main.dust[num34].velocity *= 0f;
				}

				npc.position -= npc.netOffset;
			}
			return false;
		}
	}
}

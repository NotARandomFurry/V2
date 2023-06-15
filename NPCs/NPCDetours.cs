using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using System.Reflection;
using Terraria.Chat;
using Terraria.GameContent.Achievements;
using ReLogic.Utilities;
using V2.PlayerHandling;

namespace V2.NPCs
{
	public static class NPCDetours
	{
		public static void checkDead(NPC npc)
		{
			if (!npc.active || (npc.realLife >= 0 && npc.realLife != npc.whoAmI) || npc.life > 0)
				return;

			if (npc.type == 604 || npc.type == 605)
				NPC.LadyBugKilled(npc.Center, npc.type == 605);

			if (npc.type == 397 || npc.type == 396)
			{
				if (npc.ai[0] != -2f)
				{
					npc.ai[0] = -2f;
					npc.life = npc.lifeMax;
					npc.netUpdate = true;
					npc.dontTakeDamage = true;
					if (Main.netMode != 1)
					{
						int num = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, 400);
						Main.npc[num].ai[3] = npc.ai[3];
						Main.npc[num].netUpdate = true;
					}
				}

				return;
			}

			if (npc.type == 398 && npc.ai[0] != 2f)
			{
				npc.ai[0] = 2f;
				npc.life = npc.lifeMax;
				npc.netUpdate = true;
				npc.dontTakeDamage = true;
				return;
			}

			if ((npc.type == 517 || npc.type == 422 || npc.type == 507 || npc.type == 493) && npc.ai[2] != 1f)
			{
				npc.ai[2] = 1f;
				npc.ai[1] = 0f;
				npc.life = npc.lifeMax;
				npc.dontTakeDamage = true;
				npc.netUpdate = true;
				return;
			}

			if (npc.type == 548 && npc.ai[1] != 1f)
			{
				npc.ai[1] = 1f;
				npc.ai[0] = 0f;
				npc.life = npc.lifeMax;
				npc.dontTakeDamageFromHostiles = true;
				npc.netUpdate = true;
				return;
			}

			if (!NPCLoader.CheckDead(npc))
				return;

			if (npc.AsFood().IsCurrentlyEaten)
				npc.AsFood().Digested = true;

			FieldInfo noSpawnCycleInfo = typeof(NPC).GetField("noSpawnCycle", BindingFlags.NonPublic | BindingFlags.Static);
			noSpawnCycleInfo.SetValue(null, true);
			if (npc.townNPC && npc.type != NPCID.OldMan && npc.type != NPCID.SkeletonMerchant)
			{
				if (Main.netMode != NetmodeID.Server)
				{
					int myPlayer = Main.myPlayer;
					if (npc.type == NPCID.Guide)
					{
						if (Main.player[myPlayer].ladyBugLuckTimeLeft >= 0 && Main.player[myPlayer].active && !Main.player[myPlayer].dead)
						{
							int goodLuckTime = NPC.ladyBugGoodLuckTime / 3;
							if (goodLuckTime > Main.player[myPlayer].ladyBugLuckTimeLeft)
							{
								Main.player[myPlayer].ladyBugLuckTimeLeft = goodLuckTime;
								Main.player[myPlayer].luckNeedsSync = true;
							}
						}
					}
					else if (npc.type != NPCID.Clothier && Main.player[myPlayer].active && !Main.player[myPlayer].dead)
					{
						int badLuckTime = NPC.ladyBugBadLuckTime / 3;
						if (badLuckTime < Main.player[myPlayer].ladyBugLuckTimeLeft)
						{
							Main.player[myPlayer].ladyBugLuckTimeLeft = badLuckTime;
							Main.player[myPlayer].luckNeedsSync = true;
						}
					}
				}

				bool shouldDropTombstone = !npc.AsFood().Digested;
				NetworkText fullNetName = npc.GetFullNetName();
				int num2 = 19;
				if (npc.type == NPCID.Angler || npc.type == NPCID.Princess || NPCID.Sets.IsTownPet[npc.type])
				{
					num2 = 36;
					shouldDropTombstone = false;
				}

				NetworkText networkText = NetworkText.FromKey(Lang.misc[num2].Key, fullNetName);
				if (npc.AsFood().Digested)
					networkText = NetworkText.FromKey("Mods.V2.Death.DigestedTownNPC", fullNetName);

				if (shouldDropTombstone)
				{
					for (int i = 0; i < 255; i++)
					{
						Player player = Main.player[i];
						if (player != null && player.active && player.difficulty != 2)
						{
							shouldDropTombstone = false;
							break;
						}
					}
				}

				if (shouldDropTombstone)
					npc.DropTombstoneTownNPC(networkText);

				if (Main.netMode == NetmodeID.SinglePlayer)
					Main.NewText(networkText.ToString(), byte.MaxValue, 25, 25);
				else if (Main.netMode == NetmodeID.Server)
					ChatHelper.BroadcastChatMessage(networkText, new Color(255, 25, 25));
			}

			if (Main.netMode != NetmodeID.MultiplayerClient && !Main.dayTime && npc.type == NPCID.Clothier && !NPC.AnyNPCs(35))
			{
				for (int j = 0; j < 255; j++)
				{
					if (Main.player[j].active && !Main.player[j].dead && Main.player[j].killClothier)
					{
						NPC.SpawnSkeletron(j);
						break;
					}
				}
			}

			if (npc.townNPC && Main.netMode != NetmodeID.MultiplayerClient && npc.homeless && WorldGen.prioritizedTownNPCType == npc.type)
				WorldGen.prioritizedTownNPCType = 0;


			if (npc.AsFood().Digested)
			{
				if (npc.AsFood().DigestedDeathSound != null)
					SoundEngine.PlaySound(npc.AsFood().DigestedDeathSound, npc.position);

				if (npc.AsFood().CurrentCaptor.Value.Predator is Player hungryPlayer)
					PredPlayer.CountDigestionKillForBannersAndDropThem(hungryPlayer, npc);
				npc.NPCLoot();
			}
			else
			{
				if (npc.DeathSound != null)
					SoundEngine.PlaySound(npc.DeathSound, npc.position);

				if (!NPCLoader.SpecialOnKill(npc))
				{
					if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
					{
						DropEoWLoot();
					}
					else if (npc.type == NPCID.TheDestroyer)
					{
						Vector2 position = npc.position;
						Vector2 center = Main.player[npc.target].Center;
						float num3 = 100000000f;
						Vector2 position2 = npc.position;
						for (int k = 0; k < 200; k++)
						{
							if (Main.npc[k].active && (Main.npc[k].type == NPCID.TheDestroyer || Main.npc[k].type == NPCID.TheDestroyerBody || Main.npc[k].type == NPCID.TheDestroyerTail))
							{
								float num4 = Math.Abs(Main.npc[k].Center.X - center.X) + Math.Abs(Main.npc[k].Center.Y - center.Y);
								if (num4 < num3)
								{
									num3 = num4;
									position2 = Main.npc[k].position;
								}
							}
						}

						npc.position = position2;
						npc.NPCLoot();
						npc.position = position;
					}
					else
					{
						npc.NPCLoot();
					}
				}
			}

			npc.active = false;
			DD2Event.CheckProgress(npc.type);
			CheckProgressFrostMoon();
			CheckProgressPumpkinMoon();
			int nPCInvasionGroup = NPC.GetNPCInvasionGroup(npc.type);
			if (nPCInvasionGroup <= 0 || nPCInvasionGroup != Main.invasionType)
				return;

			int num5 = 1;
			switch (npc.type)
			{
				case 216:
					num5 = 5;
					break;
				case 395:
					num5 = 10;
					break;
				case 491:
					num5 = 10;
					break;
				case 471:
					num5 = 10;
					break;
				case 472:
					num5 = 0;
					break;
				case 387:
					num5 = 0;
					break;
			}

			if (num5 > 0)
			{
				Main.invasionSize -= num5;
				if (Main.invasionSize < 0)
					Main.invasionSize = 0;

				if (Main.netMode != NetmodeID.MultiplayerClient)
					Main.ReportInvasionProgress(Main.invasionSizeStart - Main.invasionSize, Main.invasionSizeStart, nPCInvasionGroup + 3, 0);

				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, null, Main.invasionProgress, Main.invasionProgressMax, Main.invasionProgressIcon);
			}

			void DropEoWLoot()
			{
				bool lastSegment = true;
				for (int i = 0; i < 200; i++)
				{
					if (i != npc.whoAmI && Main.npc[i].active && (Main.npc[i].type == 13 || Main.npc[i].type == 14 || Main.npc[i].type == 15))
					{
						lastSegment = false;
						break;
					}
				}

				if (lastSegment)
				{
					npc.boss = true;
					npc.NPCLoot();
				}
				else
				{
					npc.NPCLoot();
				}
			}

			void CheckProgressFrostMoon()
			{
				if (!Main.snowMoon)
					return;

				int num = 0;
				NetworkText networkText = NetworkText.Empty;
				int[] array = new int[21] {
				0,
				25,
				15,
				10,
				30,
				100,
				160,
				180,
				200,
				250,
				300,
				375,
				450,
				525,
				675,
				850,
				1025,
				1325,
				1550,
				2000,
				0
			};

				num = array[NPC.waveNumber];
				switch (NPC.waveNumber)
				{
					case 1:
						networkText = Lang.GetInvasionWaveText(2, 338, 350);
						break;
					case 2:
						networkText = Lang.GetInvasionWaveText(3, 338, 350, 342, 348);
						break;
					case 3:
						networkText = Lang.GetInvasionWaveText(4, 344, 338, 350, 342);
						break;
					case 4:
						networkText = Lang.GetInvasionWaveText(5, 344, 338, 350, 348);
						break;
					case 5:
						networkText = Lang.GetInvasionWaveText(6, 344, 350, 348, 347);
						break;
					case 6:
						networkText = Lang.GetInvasionWaveText(7, 346, 342, 350, 338);
						break;
					case 7:
						networkText = Lang.GetInvasionWaveText(8, 346, 347, 350, 348, 351);
						break;
					case 8:
						networkText = Lang.GetInvasionWaveText(9, 346, 344, 348, 347, 342);
						break;
					case 9:
						networkText = Lang.GetInvasionWaveText(10, 346, 344, 351, 338, 347);
						break;
					case 10:
						networkText = Lang.GetInvasionWaveText(11, 345, 352, 338, 342);
						break;
					case 11:
						networkText = Lang.GetInvasionWaveText(12, 345, 344, 342, 343, 338);
						break;
					case 12:
						networkText = Lang.GetInvasionWaveText(13, 345, 346, 342, 352, 343, 347);
						break;
					case 13:
						networkText = Lang.GetInvasionWaveText(14, 345, 346, 344, 343, 351);
						break;
					case 14:
						networkText = Lang.GetInvasionWaveText(15, 345, 346, 344, 343, 347);
						break;
					case 15:
						networkText = Lang.GetInvasionWaveText(16, 345, 346, 344, 343, 352);
						break;
					case 16:
						networkText = Lang.GetInvasionWaveText(17, 345, 346, 344, 343, 351, 347);
						break;
					case 17:
						networkText = Lang.GetInvasionWaveText(18, 345, 346, 344, 343, 348, 351);
						break;
					case 18:
						networkText = Lang.GetInvasionWaveText(19, 345, 346, 344, 343);
						break;
					case 19:
						networkText = Lang.GetInvasionWaveText(-1, 345, 346, 344);
						break;
				}

				float num2 = 0f;
				switch (npc.type)
				{
					case 338:
					case 339:
					case 340:
						num2 = 1f;
						break;
					case 341:
						num2 = 20f;
						break;
					case 342:
						num2 = 2f;
						break;
					case 343:
						num2 = 18f;
						break;
					case 344:
						num2 = 50f;
						break;
					case 345:
						num2 = 150f;
						break;
					case 346:
						num2 = 100f;
						break;
					case 347:
						num2 = 8f;
						break;
					case 348:
					case 349:
						num2 = 4f;
						break;
					case 350:
						num2 = 3f;
						break;
				}

				if (Main.expertMode)
					num2 *= 2f;

				float num3 = NPC.waveKills;
				NPC.waveKills += num2;
				if (NPC.waveKills >= (float)num && num != 0)
				{
					NPC.waveKills = 0f;
					NPC.waveNumber++;
					num = array[NPC.waveNumber];
					if (networkText != NetworkText.Empty)
					{
						if (Main.netMode == 0)
							Main.NewText(networkText.ToString(), 175, 75);
						else if (Main.netMode == 2)
							ChatHelper.BroadcastChatMessage(networkText, new Color(175, 75, 255));

						if (NPC.waveNumber == 15)
							AchievementsHelper.NotifyProgressionEvent(14);
					}
				}

				if (NPC.waveKills != num3 && num2 != 0f)
				{
					if (Main.netMode != 1)
						Main.ReportInvasionProgress((int)NPC.waveKills, num, 1, NPC.waveNumber);

					if (Main.netMode == 2)
						NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, null, Main.invasionProgress, Main.invasionProgressMax, 1f, NPC.waveNumber);
				}
			}

			void CheckProgressPumpkinMoon()
			{
				if (!Main.pumpkinMoon)
					return;

				int num = 0;
				NetworkText networkText = NetworkText.Empty;
				int[] array = new int[16] {
				0,
				25,
				40,
				50,
				80,
				100,
				160,
				180,
				200,
				250,
				300,
				375,
				450,
				525,
				675,
				0
			};

				num = array[NPC.waveNumber];
				switch (NPC.waveNumber)
				{
					case 1:
						networkText = Lang.GetInvasionWaveText(2, 305, 326);
						break;
					case 2:
						networkText = Lang.GetInvasionWaveText(3, 305, 326, 329);
						break;
					case 3:
						networkText = Lang.GetInvasionWaveText(4, 305, 326, 329, 325);
						break;
					case 4:
						networkText = Lang.GetInvasionWaveText(5, 305, 326, 329, 330, 325);
						break;
					case 5:
						networkText = Lang.GetInvasionWaveText(6, 326, 329, 330, 325);
						break;
					case 6:
						networkText = Lang.GetInvasionWaveText(7, 305, 329, 330, 327);
						break;
					case 7:
						networkText = Lang.GetInvasionWaveText(8, 326, 329, 330, 327);
						break;
					case 8:
						networkText = Lang.GetInvasionWaveText(9, 305, 315, 325, 327);
						break;
					case 9:
						networkText = Lang.GetInvasionWaveText(10, 326, 329, 330, 315, 325, 327);
						break;
					case 10:
						networkText = Lang.GetInvasionWaveText(11, 305, 326, 329, 330, 315, 325, 327);
						break;
					case 11:
						networkText = Lang.GetInvasionWaveText(12, 326, 329, 330, 315, 325, 327);
						break;
					case 12:
						networkText = Lang.GetInvasionWaveText(13, 329, 330, 315, 325, 327);
						break;
					case 13:
						networkText = Lang.GetInvasionWaveText(14, 315, 325, 327);
						break;
					case 14:
						networkText = Lang.GetInvasionWaveText(-1, 325, 327);
						break;
				}

				float num2 = 0f;
				switch (npc.type)
				{
					case 305:
					case 306:
					case 307:
					case 308:
					case 309:
					case 310:
					case 311:
					case 312:
					case 313:
					case 314:
						num2 = 1f;
						break;
					case 315:
						num2 = 25f;
						break;
					case 325:
						num2 = 75f;
						break;
					case 326:
						num2 = 2f;
						break;
					case 327:
						num2 = 150f;
						break;
					case 329:
						num2 = 4f;
						break;
					case 330:
						num2 = 8f;
						break;
				}

				if (Main.expertMode)
					num2 *= 2f;

				float num3 = NPC.waveKills;
				NPC.waveKills += num2;
				if (NPC.waveKills >= (float)num && num != 0)
				{
					NPC.waveKills = 0f;
					NPC.waveNumber++;
					num = array[NPC.waveNumber];
					if (networkText != NetworkText.Empty)
					{
						if (Main.netMode == 0)
							Main.NewText(networkText.ToString(), 175, 75);
						else if (Main.netMode == NetmodeID.Server)
							ChatHelper.BroadcastChatMessage(networkText, new Color(175, 75, 255));

						if (NPC.waveNumber == 15)
							AchievementsHelper.NotifyProgressionEvent(15);
					}
				}

				if (NPC.waveKills != num3 && num2 != 0f)
				{
					if (Main.netMode != 1)
						Main.ReportInvasionProgress((int)NPC.waveKills, num, 2, NPC.waveNumber);

					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, null, Main.invasionProgress, Main.invasionProgressMax, 2f, NPC.waveNumber);
				}
			}
		}

		public static void DoDeathEvents_CelebrateBossDeath(NPC npc, string typeName)
		{
			if (npc.AsFood().Digested)
			{
				string localName = "";
				Entity pred = npc.AsFood().CurrentCaptor.Value.Predator;
				if (pred is NPC predNPC)
				{
					localName = predNPC.FullName;
				}
				else if (pred is Player predPlayer)
				{
					localName = predPlayer.name;
				}
				NetworkText networkName = NetworkText.FromLiteral(localName);
				string gurgledBossKey = npc.type switch
				{
					NPCID.KingSlime => "KS",
					NPCID.EyeofCthulhu => "EoC",
					NPCID.EaterofWorldsHead or NPCID.EaterofWorldsBody or NPCID.EaterofWorldsTail => "EoW",
					NPCID.BrainofCthulhu => "BoC",
					NPCID.QueenBee => "QB",
					NPCID.SkeletronHead => "Skele",
					NPCID.Deerclops => "Deerclops",
					NPCID.WallofFlesh or NPCID.WallofFleshEye => "WoF",
					NPCID.QueenSlimeBoss => "QS",
					NPCID.Retinazer or NPCID.Spazmatism => "Twins",
					NPCID.TheDestroyer or NPCID.TheDestroyerBody or NPCID.TheDestroyerTail => "Destro",
					NPCID.SkeletronPrime => "SkelePrime",
					NPCID.Plantera => "Plantera",
					NPCID.Golem => "Golem",
					NPCID.HallowBoss => "LightSnack",
					NPCID.DukeFishron => "Fishron",
					NPCID.CultistBoss => "Cultist",
					NPCID.LunarTowerSolar => "SolarPillar",
					NPCID.LunarTowerVortex => "VortexPillar",
					NPCID.LunarTowerNebula => "NebulaPillar",
					NPCID.LunarTowerStardust => "StardustPillar",
					NPCID.MoonLordCore => "MoonLord",
					_ => "Default"
				};
				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					Main.NewText(Language.GetTextValueWith("Mods.V2.Death.DigestedBoss." + gurgledBossKey, new { Pred = localName }), 175, 75);
				}
				else if (Main.netMode == NetmodeID.Server)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.V2.Death.DigestedBoss." + gurgledBossKey, new { Pred = networkName }), new Color(175, 75, 255));
				}
			}
			else
			{
				if (npc.type == 125 || npc.type == 126)
				{
					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue("Announcement.HasBeenDefeated_Plural", Language.GetTextValue("Enemies.TheTwins")), 175, 75);
					else if (Main.netMode == 2)
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasBeenDefeated_Plural", NetworkText.FromKey("Enemies.TheTwins")), new Color(175, 75, 255));
				}
				else if (npc.type == 398)
				{
					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue("Announcement.HasBeenDefeated_Single", Language.GetTextValue("Enemies.MoonLord")), 175, 75);
					else if (Main.netMode == 2)
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasBeenDefeated_Single", NetworkText.FromKey("Enemies.MoonLord")), new Color(175, 75, 255));
				}
				else if (Main.netMode == 0)
				{
					Main.NewText(Language.GetTextValue("Announcement.HasBeenDefeated_Single", typeName), 175, 75);
				}
				else if (Main.netMode == 2)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasBeenDefeated_Single", npc.GetTypeNetName()), new Color(175, 75, 255));
				}
			}
		}
	}
}

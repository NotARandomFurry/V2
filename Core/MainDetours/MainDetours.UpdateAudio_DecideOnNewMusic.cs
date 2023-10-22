using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using V2.NPCs;

namespace V2.Core.MainDetours
{
	public static partial class MainDetours
	{
		public static void UpdateAudio_DecideOnNewMusic()
		{
			// I don't like havin' to do this but the relevant fields are private and I can't really weasel a PR to tML in that makes them not so here we go
			FieldInfo lastMusicPlayedInfo = typeof(Main).GetField("lastMusicPlayed", BindingFlags.NonPublic | BindingFlags.Instance);
			int GetLastMusicPlayed() => (int)lastMusicPlayedInfo.GetValue(Main.instance);
			void SetLastMusicPlayed(int value) => lastMusicPlayedInfo.SetValue(Main.instance, value);

			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			bool flag10 = false;
			bool flag11 = false;
			bool flag12 = false;
			bool flag13 = false;
			bool flag14 = false;
			bool flag15 = false;
			bool flag16 = false;
			bool flag17 = Main.LocalPlayer.townNPCs > 2f;
			bool flag18 = Main.slimeRain;
			float num = 0f;
			for (int i = 0; i < 91; i++)
			{
				if (Main.musicFade[i] > num)
				{
					num = Main.musicFade[i];
					if (num == 1f)
						SetLastMusicPlayed(i);
				}
			}

			if (GetLastMusicPlayed() == 50)
				Main.musicNoCrossFade[51] = true;

			int modMusic = -1;
			SceneEffectPriority modPriority = SceneEffectPriority.None;

			if (!Main.gameMenu)
			{ // showSplash -> Main.gameMenu. Fix crash iterating active NPCs while unloading mods
				Rectangle rectangle = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
				int num2 = 5000;
				for (int j = 0; j < 200; j++)
				{
					NPC npc = Main.npc[j];
					if (!npc.active)
						continue;

					if (npc.CurrentCaptor() is not null)
						continue;

					num2 = 5000;
					int num3 = 0;
					switch (npc.type)
					{
						case 13:
						case 14:
						case 15:
							num3 = 1;
							break;
						case 26:
						case 27:
						case 28:
						case 29:
						case 111:
						case 471:
							num3 = 11;
							break;
						case 113:
						case 114:
						case 125:
						case 126:
							num3 = 2;
							break;
						case 134:
						case 143:
						case 144:
						case 145:
						case 266:
							num3 = 3;
							break;
						case 212:
						case 213:
						case 214:
						case 215:
						case 216:
						case 491:
							num3 = 8;
							break;
						case 245:
							num3 = 4;
							break;
						case 222:
							num3 = 5;
							break;
						case 262:
						case 263:
						case 264:
							num3 = 6;
							break;
						case 381:
						case 382:
						case 383:
						case 385:
						case 386:
						case 388:
						case 389:
						case 390:
						case 391:
						case 395:
						case 520:
							num3 = 9;
							break;
						case 398:
							num3 = 7;
							break;
						case 422:
						case 493:
						case 507:
						case 517:
							num3 = 10;
							break;
						case 439:
							num3 = 4;
							break;
						case 438:
							if (npc.ai[1] == 1f)
							{
								num2 = 1600;
								num3 = 4;
							}
							break;
						case 657:
							num3 = 13;
							break;
						case 636:
							num3 = 14;
							break;
						case 370:
							num3 = 15;
							break;
						case 668:
							num3 = 16;
							break;
					}

					if (NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type])
						num3 = 12;

					if (num3 == 0 && npc.boss)
						num3 = 1;

					if (num3 == 0)
						continue;

					Rectangle value = new Rectangle(
						(int)(npc.position.X + (float)(npc.width / 2)) - num2,
						(int)(npc.position.Y + (float)(npc.height / 2)) - num2,
						num2 * 2,
						num2 * 2
					);
					if (rectangle.Intersects(value))
					{
						//TODO: Should this be wrapped inside the new SceneEffect stuff? Not at this time, left for legacy.
						if (npc.ModNPC != null && npc.ModNPC.Music >= 0 && (modMusic < 0 || npc.ModNPC.SceneEffectPriority > modPriority))
						{
							modMusic = npc.ModNPC.Music;
							modPriority = npc.ModNPC.SceneEffectPriority;
						}
						switch (num3)
						{
							case 1:
								flag = true;
								break;
							case 2:
								flag3 = true;
								break;
							case 3:
								flag4 = true;
								break;
							case 4:
								flag5 = true;
								break;
							case 5:
								flag6 = true;
								break;
							case 6:
								flag7 = true;
								break;
							case 7:
								flag8 = true;
								break;
							case 8:
								flag9 = true;
								break;
							case 9:
								flag10 = true;
								break;
							case 10:
								flag11 = true;
								break;
							case 11:
								flag12 = true;
								break;
							case 12:
								flag13 = true;
								break;
							case 13:
								flag14 = true;
								break;
							case 14:
								flag15 = true;
								break;
							case 15:
								flag16 = true;
								break;
							case 16:
								flag2 = true;
								break;
						}

						break;
					}
				}

				LoaderManager.Get<SceneEffectLoader>().UpdateMusic(ref modMusic, ref modPriority);
			}

			_ = (Main.screenPosition.X + (float)(Main.screenWidth / 2)) / 16f;
			if (Main.musicVolume == 0f)
			{
				Main.newMusic = 0;
				return;
			}

			if (Main.gameMenu)
			{
				if (Main.netMode != NetmodeID.Server)
				{
					if (WorldGen.drunkWorldGen)
					{
						Main.newMusic = 60;
					}
					else if (Main.menuMode == 3000)
					{
						Main.newMusic = 89;
					}
					else
					{
						Main.newMusic = MenuLoader.CurrentMenu.Music;
					}

					FieldInfo _isAsyncLoadCompleteInfo = typeof(Main).GetField("_isAsyncLoadComplete", BindingFlags.NonPublic | BindingFlags.Static);
					bool _isAsyncLoadComplete = (bool)_isAsyncLoadCompleteInfo.GetValue(null);
					if (_isAsyncLoadComplete && Main.newMusic == 50 && !Main.audioSystem.IsTrackPlaying(50))
					{
						Main.newMusic = 51;
						if (Main.musicNoCrossFade[51])
							Main.musicFade[51] = 1f;
					}
				}
				else
				{
					Main.newMusic = 0;
				}

				return;
			}

			float num4 = Main.maxTilesX / 4200;
			num4 *= num4;
			float num5 = (float)((double)((Main.screenPosition.Y + (float)(Main.screenHeight / 2)) / 16f - (65f + 10f * num4)) / (Main.worldSurface / 5.0));
			if (CreditsRollEvent.IsEventOngoing)
			{
				Main.newMusic = 89;
			}
			else if (modPriority >= SceneEffectPriority.BossHigh)
			{ //Should this be before credits or nah?
				Main.newMusic = modMusic;
			}
			else if (Main.player[Main.myPlayer].happyFunTorchTime)
			{
				Main.newMusic = 13;
			}
			else if (flag8)
			{
				Main.newMusic = 38;
			}
			else if (modPriority >= SceneEffectPriority.BossMedium)
			{
				Main.newMusic = modMusic;
			}
			else if (flag10)
			{
				Main.newMusic = 37;
			}
			else if (flag11)
			{
				Main.newMusic = 34;
			}
			else if (flag7)
			{
				Main.newMusic = 24;
			}
			else if (modPriority >= SceneEffectPriority.BossLow)
			{
				Main.newMusic = modMusic;
			}
			else if (flag15)
			{
				Main.newMusic = 57;
			}
			else if (flag16)
			{
				Main.newMusic = 58;
			}
			else if (flag3)
			{
				Main.newMusic = 12;
			}
			else if (flag)
			{
				Main.newMusic = 5;
			}
			else if (flag4)
			{
				Main.newMusic = 13;
			}
			else if (flag5)
			{
				Main.newMusic = 17;
			}
			else if (flag6)
			{
				Main.newMusic = 25;
			}
			else if (modPriority >= SceneEffectPriority.Event)
			{
				Main.newMusic = modMusic;
			}
			else if (flag14)
			{
				Main.newMusic = 56;
			}
			else if (flag2)
			{
				Main.newMusic = 90;
			}
			else if (flag9)
			{
				Main.newMusic = 35;
			}
			else if (flag12)
			{
				Main.newMusic = 39;
			}
			else if (flag13)
			{
				Main.newMusic = 41;
			}
			else if (modPriority >= SceneEffectPriority.Environment)
			{
				Main.newMusic = modMusic;
			}
			else if (Main.eclipse && (double)Main.player[Main.myPlayer].position.Y < Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
			{
				Main.newMusic = 27;
			}
			else if (flag18 && !Main.player[Main.myPlayer].ZoneGraveyard && (!Main.bloodMoon || Main.dayTime) && (double)Main.player[Main.myPlayer].position.Y < Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
			{
				Main.newMusic = 48;
			}
			else if (flag17 && Main.dayTime && ((Main.cloudAlpha == 0f && !Main._shouldUseWindyDayMusic) || (double)Main.player[Main.myPlayer].position.Y >= Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2)) && !Main.player[Main.myPlayer].ZoneGraveyard)
			{
				Main.newMusic = 46;
			}
			else if (flag17 && !Main.dayTime && ((!Main.bloodMoon && Main.cloudAlpha == 0f) || (double)Main.player[Main.myPlayer].position.Y >= Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2)) && !Main.player[Main.myPlayer].ZoneGraveyard)
			{
				Main.newMusic = 47;
			}
			else if (Main.player[Main.myPlayer].ZoneSandstorm)
			{
				Main.newMusic = 40;
			}
			else if (Main.player[Main.myPlayer].position.Y > (float)(Main.UnderworldLayer * 16))
			{
				Main.newMusic = 36;
			}
			else if (num5 < 1f)
			{
				Main.newMusic = (Main.dayTime ? 42 : 15);
			}
			else if (modPriority >= SceneEffectPriority.BiomeHigh)
			{
				Main.newMusic = modMusic;
			}
			else if (Main.tile[(int)(Main.player[Main.myPlayer].Center.X / 16f), (int)(Main.player[Main.myPlayer].Center.Y / 16f)].WallType == 87)
			{
				Main.newMusic = 26;
			}
			else if (Main.player[Main.myPlayer].ZoneDungeon)
			{
				Main.newMusic = 23;
			}
			else if ((Main.bgStyle == 9 && (double)Main.player[Main.myPlayer].position.Y < Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2)) || Main.undergroundBackground == 2)
			{
				Main.newMusic = 29;
			}
			else if (Main.player[Main.myPlayer].ZoneCorrupt)
			{
				if (Main.player[Main.myPlayer].ZoneCrimson && Main.SceneMetrics.BloodTileCount > Main.SceneMetrics.EvilTileCount)
				{
					if ((double)Main.player[Main.myPlayer].position.Y > Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
						Main.newMusic = 33;
					else
						Main.newMusic = 16;
				}
				else if ((double)Main.player[Main.myPlayer].position.Y > Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
				{
					Main.newMusic = 10;
				}
				else
				{
					Main.newMusic = 8;
				}
			}
			else if (Main.player[Main.myPlayer].ZoneCrimson)
			{
				if ((double)Main.player[Main.myPlayer].position.Y > Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
					Main.newMusic = 33;
				else
					Main.newMusic = 16;
			}
			else if (modPriority >= SceneEffectPriority.BiomeMedium)
			{
				Main.newMusic = modMusic;
			}
			else if (Main.player[Main.myPlayer].ZoneMeteor)
			{
				Main.newMusic = 2;
			}
			else if (Main.player[Main.myPlayer].ZoneGraveyard)
			{
				Main.newMusic = 53;
			}
			else if (Main.player[Main.myPlayer].ZoneJungle)
			{
				if ((double)Main.player[Main.myPlayer].position.Y > Main.rockLayer * 16.0 + (double)(Main.screenHeight / 2))
					Main.newMusic = 54;
				else if (Main.newMusic == 54 && (double)Main.player[Main.myPlayer].position.Y > (Main.rockLayer - 50.0) * 16.0 + (double)(Main.screenHeight / 2))
					Main.newMusic = 54;
				else if (Main._shouldUseStormMusic && (double)Main.player[Main.myPlayer].position.Y < Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
					Main.newMusic = 52;
				else if (Main.dayTime)
					Main.newMusic = 7;
				else
					Main.newMusic = 55;
			}
			else if (Main.player[Main.myPlayer].ZoneSnow)
			{
				if ((double)Main.player[Main.myPlayer].position.Y > Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
					Main.newMusic = 20;
				else
					Main.newMusic = 14;
			}
			else if (modPriority >= SceneEffectPriority.BiomeLow)
			{
				Main.newMusic = modMusic;
			}
			else if ((double)Main.player[Main.myPlayer].position.Y >= Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2) && !WorldGen.oceanDepths((int)(Main.screenPosition.X + (float)(Main.screenWidth / 2)) / 16, (int)(Main.screenPosition.Y + (float)(Main.screenHeight / 2)) / 16))
			{
				if (Main.player[Main.myPlayer].ZoneHallow)
				{
					Main.newMusic = 11;
				}
				else if (Main.player[Main.myPlayer].ZoneUndergroundDesert)
				{
					if ((double)Main.player[Main.myPlayer].position.Y >= Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
						Main.newMusic = 61;
					else
						Main.newMusic = 21;
				}
				else
				{
					if (Main.ugMusic == 0)
						Main.ugMusic = 4;

					if (!Main.audioSystem.IsTrackPlaying(4) && !Main.audioSystem.IsTrackPlaying(31))
					{
						if (Main.musicFade[4] == 1f)
							Main.musicFade[31] = 1f;

						if (Main.musicFade[31] == 1f)
							Main.musicFade[4] = 1f;

						switch (Main.rand.Next(2))
						{
							case 0:
								Main.ugMusic = 4;
								Main.musicFade[31] = 0f;
								break;
							case 1:
								Main.ugMusic = 31;
								Main.musicFade[4] = 0f;
								break;
						}
					}

					Main.newMusic = Main.ugMusic;
				}
			}
			else if (Main.dayTime && Main.player[Main.myPlayer].ZoneHallow)
			{
				if (Main._shouldUseStormMusic)
					Main.newMusic = 52;
				else if (Main.cloudAlpha > 0f && !Main.gameMenu)
					Main.newMusic = 19;
				else if (Main._shouldUseWindyDayMusic)
					Main.newMusic = 44;
				else
					Main.newMusic = 9;
			}
			else if (Main._shouldUseStormMusic)
			{
				if (Main.bloodMoon)
					Main.newMusic = 2;
				else
					Main.newMusic = 52;
			}
			else if (WorldGen.oceanDepths((int)(Main.screenPosition.X + (float)(Main.screenWidth / 2)) / 16, (int)(Main.screenPosition.Y + (float)(Main.screenHeight / 2)) / 16))
			{
				if (Main.bloodMoon)
				{
					Main.newMusic = 2;
				}
				else if (flag17)
				{
					if (Main.dayTime)
						Main.newMusic = 46;
					else
						Main.newMusic = 47;
				}
				else
				{
					Main.newMusic = (Main.dayTime ? 22 : 43);
				}
			}
			else if (Main.player[Main.myPlayer].ZoneDesert)
			{
				if ((double)Main.player[Main.myPlayer].position.Y >= Main.worldSurface * 16.0)
				{
					int num6 = (int)(Main.player[Main.myPlayer].Center.X / 16f);
					int num7 = (int)(Main.player[Main.myPlayer].Center.Y / 16f);
					if (WorldGen.InWorld(num6, num7) && (WallID.Sets.Conversion.Sandstone[Main.tile[num6, num7].WallType] || WallID.Sets.Conversion.HardenedSand[Main.tile[num6, num7].WallType]))
						Main.newMusic = 61;
					else
						Main.newMusic = 21;
				}
				else
				{
					Main.newMusic = 21;
				}
			}
			else if (Main.dayTime)
			{
				if (Main.cloudAlpha > 0f && !Main.gameMenu)
				{
					if (Main.time < 10800.0)
						Main.newMusic = 59;
					else
						Main.newMusic = 19;
				}
				else
				{
					if (Main.dayMusic == 0)
						Main.dayMusic = 1;

					if (!Main.audioSystem.IsTrackPlaying(1) && !Main.audioSystem.IsTrackPlaying(18))
						Main.dayMusic = Main.rand.NextBool() ? 1 : 18;

					Main.newMusic = Main.dayMusic;
					if (Main._shouldUseWindyDayMusic)
						Main.newMusic = 44;
				}
			}
			else if (!Main.dayTime)
			{
				if (Main.bloodMoon)
					Main.newMusic = 2;
				else if (Main.cloudAlpha > 0f && !Main.gameMenu)
					Main.newMusic = 19;
				else
					Main.newMusic = 3;
			}

			if ((double)(Main.screenPosition.Y / 16f) < Main.worldSurface + 10.0 && Main.pumpkinMoon)
				Main.newMusic = 30;

			if ((double)(Main.screenPosition.Y / 16f) < Main.worldSurface + 10.0 && Main.snowMoon)
				Main.newMusic = 32;
		}
	}
}

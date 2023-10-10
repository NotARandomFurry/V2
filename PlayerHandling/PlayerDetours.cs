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
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Graphics.Shaders;
using System.Reflection;
using V2.Items;

namespace V2.PlayerHandling
{
	public static class PlayerDetours
	{
		public static void ItemCheck_ReleaseCritter(Player player, Item sItem)
		{
			if (sItem.makeNPC == NPCID.ExplosiveBunny)
			{
				player.ApplyItemTime(sItem);
				int releasedCritterIndex = NPC.ReleaseNPC((int)player.Center.X, (int)player.Bottom.Y, sItem.makeNPC, sItem.placeStyle, player.whoAmI);
				NPC releasedCritter = Main.npc[releasedCritterIndex];
				if (sItem.AsFood().MaxHealth != 0 && sItem.AsFood().MaxHealth == releasedCritter.lifeMax)
					releasedCritter.life = sItem.AsFood().Health;
				if (Main.myPlayer == player.whoAmI && V2.SwallowHotkey.Current && PredPlayer.CanSwallow(player, releasedCritter))
					PredPlayer.Swallow(player, releasedCritter);
			}
			else if (player.position.X / 16f - (float)Player.tileRangeX - (float)sItem.tileBoost <= (float)Player.tileTargetX
				 && (player.position.X + (float)player.width) / 16f + (float)Player.tileRangeX + (float)sItem.tileBoost - 1f >= (float)Player.tileTargetX
				 && player.position.Y / 16f - (float)Player.tileRangeY - (float)sItem.tileBoost <= (float)Player.tileTargetY
				 && (player.position.Y + (float)player.height) / 16f + (float)Player.tileRangeY + (float)sItem.tileBoost - 2f >= (float)Player.tileTargetY)
			{
				int num = Main.mouseX + (int)Main.screenPosition.X;
				int num2 = Main.mouseY + (int)Main.screenPosition.Y;
				int i = num / 16;
				int j = num2 / 16;
				if (!WorldGen.SolidTile(i, j))
				{
					player.ApplyItemTime(sItem);
					int releasedCritterIndex = NPC.ReleaseNPC(num, num2, sItem.makeNPC, sItem.placeStyle, player.whoAmI);
					NPC releasedCritter = Main.npc[releasedCritterIndex];
					if (sItem.AsFood().MaxHealth != 0 && sItem.AsFood().MaxHealth == releasedCritter.lifeMax)
						releasedCritter.life = sItem.AsFood().Health;
					if (Main.myPlayer == player.whoAmI && V2.SwallowHotkey.Current && PredPlayer.CanSwallow(player, releasedCritter))
						PredPlayer.Swallow(player, releasedCritter);
				}
			}
		}

		public static void KillMe(Player player, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp = false)
		{
			if (player.creativeGodMode || player.dead)
				return;

			player.StopVanityActions();
			bool playSound = true;
			bool genGore = true;
			if (!PlayerLoader.PreKill(player, dmg, hitDirection, pvp, ref playSound, ref genGore, ref damageSource))
				return;

			if (pvp)
				player.pvpDeath = true;

			if (player.trapDebuffSource)
				AchievementsHelper.HandleSpecialEvent(player, 4);

			if (Main.myPlayer == player.whoAmI)
			{
				if (player._framesLeftEligibleForDeadmansChestDeathAchievement > 0)
					AchievementsHelper.HandleSpecialEvent(player, 23);

				Main.NotifyOfEvent(GameNotificationType.SpawnOrDeath);
			}
			player.lastDeathPostion = player.Center;
			player.lastDeathTime = DateTime.Now;
			player.showLastDeath = true;
			bool overFlowing;
			int coinsOwned = (int)Utils.CoinsCount(out overFlowing, player.inventory);
			if (Main.myPlayer == player.whoAmI)
			{
				player.lostCoins = coinsOwned;
				player.lostCoinString = Main.ValueToCoins(player.lostCoins);

				MethodInfo endOngoingTorchGodEventMethod = player.GetType().GetMethod("EndOngoingTorchGodEvent", BindingFlags.Instance | BindingFlags.NonPublic);
				endOngoingTorchGodEventMethod.Invoke(player, null);

				Main.mapFullscreen = false;

				player.trashItem.SetDefaults();
				if (player.difficulty == PlayerDifficultyID.SoftCore || player.difficulty == PlayerDifficultyID.Creative)
				{
					for (int i = 0; i < 59; i++)
					{
						if (player.inventory[i].stack > 0 && ((player.inventory[i].type >= ItemID.LargeAmethyst && player.inventory[i].type <= ItemID.LargeDiamond) || player.inventory[i].type == ItemID.LargeAmber))
						{
							int num = Item.NewItem(player.GetSource_Death(), (int)player.position.X, (int)player.position.Y, player.width, player.height, player.inventory[i].type);
							Main.item[num].netDefaults(player.inventory[i].netID);
							Main.item[num].Prefix(player.inventory[i].prefix);
							Main.item[num].stack = player.inventory[i].stack;
							Main.item[num].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
							Main.item[num].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
							Main.item[num].noGrabDelay = 100;
							Main.item[num].favorited = false;
							Main.item[num].newAndShiny = false;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num);

							player.inventory[i].SetDefaults();
						}
					}
				}
				else if (player.difficulty == 1)
				{
					player.DropItems();
				}
				else if (player.difficulty == 2)
				{
					player.DropItems();
					player.KillMeForGood();
				}
			}

			if (playSound)
			{
				SoundEngine.PlaySound(
					SoundID.PlayerKilled,
					player.Center
				);
			}

			player.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			player.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			player.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			player.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			player.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			player.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			if (player.stoned || !genGore || player.AsFood().Digested)
			{
				player.headPosition = Vector2.Zero;
				player.bodyPosition = Vector2.Zero;
				player.legPosition = Vector2.Zero;
			}

			if (genGore && !player.AsFood().Digested)
			{
				for (int j = 0; j < 100; j++)
				{
					if (player.stoned)
					{
						Dust.NewDust(player.position, player.width, player.height, DustID.Stone, 2 * hitDirection, -2f);
					}
					else if (player.frostArmor)
					{
						int num2 = Dust.NewDust(player.position, player.width, player.height, DustID.IceTorch, 2 * hitDirection, -2f);
						Main.dust[num2].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
					}
					else if (player.boneArmor)
					{
						int num3 = Dust.NewDust(player.position, player.width, player.height, DustID.Bone, 2 * hitDirection, -2f);
						Main.dust[num3].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
					}
					else
					{
						Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 2 * hitDirection, -2f);
					}
				}
			}

			player.mount.Dismount(player);
			player.dead = true;
			player.respawnTimer = 600;
			bool flag = false;
			if (Main.netMode != NetmodeID.SinglePlayer && !pvp)
			{
				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && (Main.npc[k].boss || Main.npc[k].type == NPCID.EaterofWorldsHead || Main.npc[k].type == NPCID.EaterofWorldsBody || Main.npc[k].type == NPCID.EaterofWorldsTail) && Math.Abs(player.Center.X - Main.npc[k].Center.X) + Math.Abs(player.Center.Y - Main.npc[k].Center.Y) < 4000f)
					{
						flag = true;
						break;
					}
				}
			}

			if (flag)
				player.respawnTimer += 600;

			if (Main.expertMode)
				player.respawnTimer = (int)((double)player.respawnTimer * 1.5);

			PlayerLoader.Kill(player, dmg, hitDirection, pvp, damageSource);
			player.immuneAlpha = 0;
			if (!ChildSafety.Disabled)
				player.immuneAlpha = 255;

			player.palladiumRegen = false;
			player.iceBarrier = false;
			player.crystalLeaf = false;
			NetworkText deathText = damageSource.GetDeathText(player.name);
			if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(deathText, new Color(225, 25, 25));
			else if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(deathText.ToString(), 225, 25, 25);

			if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
				NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)dmg, hitDirection, pvp);

			if (player.whoAmI == Main.myPlayer && (player.difficulty == 0 || player.difficulty == 3))
			{
				if (!pvp)
				{
					player.DropCoins();
				}
				else
				{
					player.lostCoins = 0;
					player.lostCoinString = Main.ValueToCoins(player.lostCoins);
				}
			}

			if (!player.AsFood().Digested)
				player.DropTombstone(coinsOwned, deathText, hitDirection);

			if (player.whoAmI == Main.myPlayer)
			{
				try
				{
					WorldGen.saveToonWhilePlaying();
				}
				catch
				{
				}
			}
		}
	}
}

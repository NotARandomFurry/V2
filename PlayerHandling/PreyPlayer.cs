using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using V2.Core;
using V2.NPCs;
using V2.StatusEffects.Debuffs;

namespace V2.PlayerHandling
{
	public static class PreyPlayerDigestionSounds
	{
		public static readonly SoundStyle PlayerDigestingMale = new SoundStyle(
			"V2/PlayerHandling/MaleHit_FromDigestTick",
			0,
			3,
			SoundType.Sound
		)
		{
			Volume = 1f,
			PitchVariance = 0f
		};
		public static readonly SoundStyle PlayerDigestingFemale = new SoundStyle(
			"V2/PlayerHandling/FemaleHit_FromDigestTick",
			0,
			3,
			SoundType.Sound
		)
		{
			Volume = 1f,
			PitchVariance = 0f
		};
	}

	public partial class PreyPlayer : ModPlayer
	{
		// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
		// public int[] HasBeenDigestedByNPC { get; set; }
		// public int[] HasBeenDigestedByNPCTotal { get; set; }

		public bool Digested { get; set; }

		public (int _swallowCount, int _gurgleCount) _timesEaten;
		public int TotalTimesSwallowed
		{
			get => _timesEaten._swallowCount;
			set => _timesEaten._swallowCount = value;
		}
		public int TotalTimesDigested
		{
			get => _timesEaten._gurgleCount;
			set => _timesEaten._gurgleCount = value;
		}

		public StatModifier TakenDigestionDamageModifier { get; set; }

		public double SoftenedDigestionDamageTaken { get; set; }
		public StatModifier SoftenedDigestionDamageModifier { get; set; }
		public int SoftenedWearoffDelay { get; set; }
		public static int SoftenedWearoffMaxDelay => V2Utils.SensibleTime(seconds: 2, frames: 30);
		public StatModifier SoftenedWearoffRateModifier { get; set; }
		public int SoftenedStacks => Math.Min(Softened.MaxStacks, (int)Math.Floor((double)Player.AsFood().SoftenedDigestionDamageTaken / (Player.statLifeMax * Softened.MaxHealthDigestedForOneStack)));

		public override void Initialize()
		{
			Player.AsFood().STR = new PredStat();

			Player.AsFood().Digested = false;
			// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
			// Player.AsPrey().HasBeenDigestedByNPC = new int[NPCLoader.NPCCount];
			// Player.AsPrey().HasBeenDigestedByNPCTotal = new int[NPCLoader.NPCCount];

			Player.AsFood().SoftenedDigestionDamageTaken = 0;
			Player.AsFood().SoftenedWearoffDelay = 0;
		}

		public override void OnEnterWorld()
		{
			Player.AsFood().Digested = false;

			Player.AsFood().SoftenedDigestionDamageTaken = 0;
			Player.AsFood().SoftenedWearoffDelay = 0;
		}

		public override void ResetEffects()
		{
			Player.AsFood().Digested = false;

			Player.AsFood().StruggleStrengthModifier = StatModifier.Default;

			Player.AsFood().TakenDigestionDamageModifier = StatModifier.Default;

			if (!Player.HasBuff(ModContent.BuffType<Softened>()))
				Player.AddBuff(ModContent.BuffType<Softened>(), 3);
			Player.AsFood().SoftenedDigestionDamageModifier = StatModifier.Default;
			Player.AsFood().SoftenedWearoffRateModifier = StatModifier.Default;
			if (Player.AsFood().SoftenedWearoffDelay > 0)
				Player.AsFood().SoftenedWearoffDelay--;
		}

		public override void UpdateDead()
		{
			Player.AsFood().SoftenedDigestionDamageTaken = 0;
			Player.AsFood().SoftenedWearoffDelay = 0;
		}

		public override void PreUpdateMovement()
		{
			if (Player.wet)
				Player.AsFood().SoftenedWearoffRateModifier *= 2.0f;

			if (Player.AsFood().SoftenedWearoffDelay <= 0 && Player.AsFood().SoftenedDigestionDamageTaken > 0)
				Player.AsFood().SoftenedDigestionDamageTaken -= Player.AsFood().SoftenedWearoffRateModifier.ApplyTo((float)(25.0 / 60.0));
		}

		public override void PostItemCheck()
		{
			if (V2.FeedHotkey.JustPressed && Player.whoAmI == Main.myPlayer)
			{
				if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
					Main.NewText("Attempting to force-feed " + Player.name + " to nearby predators...");
				if (Player.CurrentCaptor() is not null)
				{
					if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
						Main.NewText("Force-feed attempt failed; " + Player.name + " is already busy being food.");
					return;
				}
				string predType = "none";
				int predIndex = -1;
				Vector2 playerLocation = Player.MountedCenter;
				Vector2 cursorLocation = Main.MouseWorld;
				double maxDistanceFromPlayer = V2Utils.TileCountAsPixelCount(4.25);
				double maxDistanceFromCursor = 2000;
				for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
				{
					NPC potentialPred = Main.npc[npcIndex];
					if (!potentialPred.active)
						continue;

					if (potentialPred.CurrentCaptor() is not null)
						continue;

					switch (ModContent.GetInstance<V2ServerConfig>().GenderBlacklist)
					{
						default:
							// do absolutely fucking nothing lmao
							break;
						case "No Male":
							if (potentialPred.AsV2NPC().Gender == EntityGender.Male)
								continue;
							break;
						case "No Female":
							if (potentialPred.AsV2NPC().Gender == EntityGender.Female)
								continue;
							break;
						case "No M or F...but why?":
							if (potentialPred.AsV2NPC().Gender != EntityGender.Other)
								continue;
							break;
					}

					if (!potentialPred.AsPred().CanBeForceFed.Invoke(potentialPred))
						continue;

					if (potentialPred.Distance(playerLocation) >= maxDistanceFromPlayer)
						continue;

					if (potentialPred.Distance(cursorLocation) < maxDistanceFromCursor)
					{
						predIndex = npcIndex;
						predType = "NPC";
						maxDistanceFromCursor = potentialPred.Distance(cursorLocation);
					}
				}
				for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
				{
					Player potentialPred = Main.player[playerIndex];
					if (!potentialPred.active || potentialPred.dead || potentialPred.whoAmI == Player.whoAmI)
						continue;

					if (potentialPred.CurrentCaptor() is not null)
						continue;

					switch (ModContent.GetInstance<V2ServerConfig>().GenderBlacklist)
					{
						default:
							// do absolutely fucking nothing lmao
							break;
						case "No Male":
							if (potentialPred.Male)
								continue;
							break;
						case "No Female":
							if (!potentialPred.Male)
								continue;
							break;
						case "No M or F...but why?":
							continue;
					}

					if (potentialPred.Distance(playerLocation) >= maxDistanceFromPlayer)
						continue;

					if (potentialPred.Distance(cursorLocation) < maxDistanceFromCursor)
					{
						predIndex = playerIndex;
						predType = "player";
						maxDistanceFromCursor = potentialPred.Distance(cursorLocation);
					}
				}

				if (predType != "none" && predIndex != -1)
				{
					if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
						Main.NewText("Pred found! Pred type: " + predType + ". Pred index: " + predIndex + ".\n"
								   + "Cramming " + Player.name + " into the chosen stomach...");
					string foodFor = "";
					switch (predType)
					{
						case "NPC":
							NPC predNPC = Main.npc[predIndex];
							if (!PredNPC.CanSwallow(predNPC, Player))
								return;

							PredNPC.Swallow(predNPC, Player);
							predNPC.AsPred().OnForceFed.Invoke(predNPC, Player);
							foodFor = predNPC.FullName;
							break;
						case "player":
							Player predPlayer = Main.player[predIndex];
							if (!PredPlayer.CanSwallow(predPlayer, Player))
								return;

							PredPlayer.Swallow(predPlayer, Player);
							foodFor = predPlayer.name;
							break;
					}
					if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
					{
						string debugText = "Force-feed action successful; " + Player.name + " is now food for " + foodFor + ".";
						if (Main.netMode == NetmodeID.SinglePlayer)
							Main.NewText(debugText, Color.PaleVioletRed);
						else if (Main.netMode == NetmodeID.Server)
							ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(debugText), Color.PaleVioletRed);
					}
				}
				else
				{
					if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
					{
						string debugText = "Force-feed action failed; there are no suitable preds nearby to turn " + Player.name + " into a snack for.";
						if (Main.netMode == NetmodeID.SinglePlayer)
							Main.NewText(debugText, Color.PaleVioletRed);
						else if (Main.netMode == NetmodeID.Server)
							ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(debugText), Color.PaleVioletRed);
					}
					return;
				}
			}
		}

		public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
		{
			if (npc.CurrentCaptor() is not null || Player.CurrentCaptor() is not null)
				return false;

			return true;
		}

		public override bool CanBeHitByProjectile(Projectile proj)
		{
			if (Player.CurrentCaptor() is not null)
				return false;

			return true;
		}

		public override void NaturalLifeRegen(ref float regen)
		{
			if (Player.CurrentCaptor() is not null)
			{
				Player.lifeRegen = 0;
				Player.lifeRegenTime = 0;
				Player.lifeRegenCount = 0;
				regen = 0;
			}
		}

		/// <summary>
		/// Deals the given amount of digestion damage to the player, respecting damage variation and luck.
		/// </summary>
		/// <param name="pred">The pred currently digesting this player.</param>
		/// <param name="digestionDamage">The total amount of digestion damage to be dealt, before damage variation calculations.</param>
		/// <returns>Whether or not the resulting digestion tick kills the player.</returns>
		public bool TakeDigestionDamage(Entity pred, double digestionDamage)
		{
			int trueDigestionDamage = Main.DamageVar((float)digestionDamage, Player.luck);
			if (ModContent.GetInstance<V2ServerConfig>().DefenseInDigestionCalcs)
			{
				trueDigestionDamage -= Player.statDefense / 2;
				if (trueDigestionDamage < 0)
					trueDigestionDamage = 0;
			}
			trueDigestionDamage = (int)Math.Floor(Player.AsFood().TakenDigestionDamageModifier.ApplyTo(trueDigestionDamage));
			Player.AsFood().SoftenedDigestionDamageTaken += Player.AsFood().SoftenedDigestionDamageModifier.ApplyTo(trueDigestionDamage);
			Player.AsFood().SoftenedWearoffDelay = SoftenedWearoffMaxDelay;
			Player.statLife -= trueDigestionDamage;
			if (Player.statLife <= 0)
			{
				Player.AsFood().Digested = true;
				Player.AsFood().TotalTimesDigested += 1;
				if (pred is NPC predNPC)
				{
					// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
					// Player.AsPrey().HasBeenDigestedByNPC[predNPC.type] += 1;
					// Player.AsPrey().HasBeenDigestedByNPCTotal[predNPC.type] += 1;
					Player.KillMe(
						PlayerDeathReason.ByCustomReason(PredNPC.GetDigestedPlayerDeathReason(predNPC, Player)),
						trueDigestionDamage,
						0
					);
				}
				else if (pred is Player predPlayer)
				{
					Player.KillMe(
						PlayerDeathReason.ByCustomReason(PredPlayer.GetDigestedPlayerDeathReason(predPlayer, Player)),
						trueDigestionDamage,
						0
					);
				}
				else
				{
					Player.KillMe(
						PlayerDeathReason.ByCustomReason(Player.name + " was digested."),
						trueDigestionDamage,
						0
					);
				}
				return true;
			}
			else
			{
				CombatText digestionText = Main.combatText[CombatText.NewText(
					Player.Hitbox,
					Color.DarkGreen,
					trueDigestionDamage,
					false,
					true
				)];
				digestionText.position.X = pred.Center.X;
				digestionText.position.X += pred.direction * 14;
				if (pred.direction == -1)
					digestionText.position.X -= ChatManager.GetStringSize(FontAssets.CombatText[0].Value, digestionText.text, new Vector2(digestionText.scale)).X;
				digestionText.position.Y = Player.Center.Y;
				digestionText.position.Y += Player.height / 5f;
				digestionText.velocity.X = pred.direction * 2.5f;
				digestionText.velocity.Y = -4f;
				SoundEngine.PlaySound(Player.Male ? PreyPlayerDigestionSounds.PlayerDigestingMale : PreyPlayerDigestionSounds.PlayerDigestingFemale, pred.position);
				return false;
			}
		}

		public override bool PreKill(
			double damage,
			int hitDirection,
			bool pvp,
			ref bool playSound,
			ref bool genGore,
			ref PlayerDeathReason damageSource
		)
		{
			if (Player.AsFood().Digested)
			{
				playSound = false;
				genGore = false;
			}
			if (damageSource.SourceOtherIndex == 1)
			{
				if (Player.AsFood().TotalTimesDigested >= 20)
				{
					damageSource.SourceCustomReason = Language.GetTextValueWith(
						Main.rand.NextFromList(
							"Mods.V2.Death.DrownedPlayer.GutSlut.1",
							"Mods.V2.Death.DrownedPlayer.GutSlut.2",
							"Mods.V2.Death.DrownedPlayer.GutSlut.3"
						),
						new
						{
							Player = Player.name
						}
					);
				}
			}
			return true;
		}

		public override void HideDrawLayers(PlayerDrawSet drawInfo)
		{
			foreach (PlayerDrawLayer drawLayer in PlayerDrawLayerLoader.Layers)
			{
				if (!Main.gameMenu && (Player.CurrentCaptor() is not null || Player.AsFood().Digested))
					drawLayer.Hide();
			}
		}

		public override void SaveData(TagCompound tag)
		{
			// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
			// tag["hasBeenEatenBy"] = Player.AsPrey().HasBeenDigestedByNPC.ToList();
			// tag["hasBeenEatenByTotal"] = Player.AsPrey().HasBeenDigestedByNPCTotal.ToList();
		}

		public override void LoadData(TagCompound tag)
		{
			// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
			// Player.AsPrey().HasBeenDigestedByNPC = tag.GetList<int>("hasBeenEatenBy").ToArray();
			// Player.AsPrey().HasBeenDigestedByNPCTotal = tag.GetList<int>("hasBeenEatenByTotal").ToArray();
		}
	}
}
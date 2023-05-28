using Humanizer;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
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
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.NPCs.Vanilla.TownNPCs.Nurse;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs
{
	public static class PredNPCStuff
	{
		public static PredNPC AsPred(this NPC npc, bool risky = false)
		{
			if (!npc.TryGetGlobalNPC(out PredNPC predNPC))
			{
				if (risky)
					return null;

				throw new Exception("this NPC can't be a pred at all, and thus, doesn't have a PredNPC global attached to them. look for your favorite gut to sleep in elsewhere");
			}
			return predNPC;
		}
	}

	public class PredNPC : GlobalNPC
	{
		public EntityGender Gender;
		public List<Prey> stomachContents;
		public List<Prey> stomachContentsQueue;
		public double maxStomachCapacity;
		public float swallowRange;

		public SoundStyle SmallBurps { get; set; }
		public SoundStyle StandardBurps { get; set; }
		public SoundStyle BigBurps { get; set; }

		public List<SoundStyle> SmallGulps { get; set; }
		public List<SoundStyle> BigGulps { get; set; }

		public delegate void DelegateResetPredSpecificVariables(NPC npc);
		public DelegateResetPredSpecificVariables ResetPredSpecificVariablesMethod { get; set; }

		public delegate double DelegateGetDigestionTickRate(NPC npc, Prey prey);
		public DelegateGetDigestionTickRate GetDigestionTickRateMethod { get; set; }

		public delegate double DelegateGetDigestionTickDamage(NPC npc, Prey prey);
		public DelegateGetDigestionTickDamage GetDigestionTickDamageMethod { get; set; }

		public delegate void DelegateOnDigestionKill(NPC npc, Prey digestedPrey);
		public DelegateOnDigestionKill OnDigestionKillMethod { get; set; }

		public delegate double DelegateGetPreyAbsorptionRate(NPC npc);
		public DelegateGetPreyAbsorptionRate GetPreyAbsorptionRateMethod { get; set; }

		public delegate List<string> DelegateGetTownNPCChat(NPC npc, Player player);
		public DelegateGetTownNPCChat GetChatMethod { get; set; }

		public delegate void DelegateGetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathMessageKeyList);
		public DelegateGetDigestedPlayerAdditionalDeathMessages GetDigestedPlayerAdditionalDeathMessagesMethod { get; set; }

		public delegate int DelegateGetVisualBellySize(NPC npc);
		public DelegateGetVisualBellySize GetVisualBellySizeMethod { get; set; }

		public delegate bool DelegateSpecialPredAI(NPC npc);
		public DelegateSpecialPredAI SpecialPredAIMethod { get; set; }

		public delegate bool DelegateCanBeForceFed(NPC npc);
		public DelegateCanBeForceFed CanBeForceFedMethod { get; set; }

		public delegate void DelegateOnForceFed(NPC npc, Player player);
		public DelegateOnForceFed OnForceFedMethod { get; set; }

		public delegate void DelegateModifyChatButtons(NPC npc, Player player, ref string button, ref string button2);
		public DelegateModifyChatButtons ModifyChatButtonsMethod { get; set; }

		public SlotId ActiveStomachNoises { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;

		public PredNPC()
		{
			stomachContents = new List<Prey>();
			stomachContentsQueue = new List<Prey>();
			maxStomachCapacity = 1.0;
			swallowRange = 36f;

			// This is where all the defaults methods get set.
			ResetPredSpecificVariablesMethod = null;
			GetDigestionTickRateMethod = null;
			GetDigestionTickDamageMethod = null;
			GetPreyAbsorptionRateMethod = null;
			GetChatMethod = null;
			GetVisualBellySizeMethod = null;
			SpecialPredAIMethod = null;

			OnDigestionKillMethod = null;

			CanBeForceFedMethod = (NPC npc) => false;
			OnForceFedMethod = null;

			SmallBurps = Burps.Humanoid.Small;
			StandardBurps = Burps.Humanoid.Standard;

			SmallGulps = new List<SoundStyle>
			{
				Gulps.Short1,
				Gulps.Short2,
				Gulps.Short3,
				Gulps.Short4,
			};
			BigGulps = new List<SoundStyle>
			{
				Gulps.Standard1,
				Gulps.Standard2,
				Gulps.Standard3,
				Gulps.Standard4,
				Gulps.Standard5,
				Gulps.Standard6,
				Gulps.Standard7,
				Gulps.Standard8,
				Gulps.Standard9,
				Gulps.Standard10,
			};
		}

		public override void ResetEffects(NPC npc)
		{
			while (npc.AsPred().stomachContentsQueue.Count > 0)
			{
				npc.AsPred().stomachContents.Add(npc.AsPred().stomachContentsQueue.First());
				npc.AsPred().stomachContentsQueue.Remove(npc.AsPred().stomachContentsQueue.First());
			}

			if (npc.AsPred().ResetPredSpecificVariablesMethod is not null)
				npc.AsPred().ResetPredSpecificVariablesMethod.Invoke(npc);
		}

		public static bool CanSwallow(NPC pred, Entity prey)
		{
			if (V2.VoreNPCBlacklist.Contains(pred.type))
				return false;

			if (GetCurrentBellyWeight(pred) >= pred.AsPred().maxStomachCapacity)
				return false;

			switch (ModContent.GetInstance<V2ServerSideConfigs>().GenderBlacklist)
			{
				default:
					// do absolutely fucking nothing lmao
					break;
				case "No Male":
					if (pred.AsV2NPC().Gender == EntityGender.Male)
						return false;
					break;
				case "No Female":
					if (pred.AsV2NPC().Gender == EntityGender.Female)
						return false;
					break;
				case "No M or F...but why?":
					if (pred.AsV2NPC().Gender != EntityGender.Other)
						return false;
					break;
			}

			if (prey is Player preyPlayer)
			{
				if (Prey.GetInitialPreyWeight(preyPlayer) >= pred.AsPred().maxStomachCapacity - GetCurrentBellyWeight(pred))
					return false;

				return !preyPlayer.AsPrey().IsCurrentlyEaten;
			}
			else if (prey is NPC preyNPC)
			{
				if (V2.VoreNPCBlacklist.Contains(preyNPC.type))
					return false;

				bool tastesLikeSkittles = preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerSideConfigs>().EasilyEdibleEmpress;
				if (tastesLikeSkittles)
					return true;

				bool isThisAFuckingBoss = preyNPC.boss || (preyNPC.type >= NPCID.EaterofWorldsHead && preyNPC.type <= NPCID.EaterofWorldsTail); // I hate EoW
				if (isThisAFuckingBoss)
					return false;

				if (Prey.GetInitialPreyWeight(preyNPC) >= pred.AsPred().maxStomachCapacity - GetCurrentBellyWeight(pred))
					return false;

				return !preyNPC.AsPrey().IsCurrentlyEaten;
			}

			return true;
		}

		/// <summary>
		/// Causes the given predator NPC to swallow the given prey entity, if the given prey entity can be swallowed.
		/// </summary>
		/// <param name="pred">The predator which will attempt to swallow the given prey.</param>
		/// <param name="prey">The prey which will be attempt to be swallowed by the given predator.</param>
		public static void Swallow(NPC pred, Entity prey)
		{
			if (CanSwallow(pred, prey))
			{
				if (pred.AsPred().stomachContents is null || pred.AsPred().stomachContents.Count <= 0)
					pred.AsPred().stomachContents = new List<Prey>();

				Prey food = new Prey(prey);
				pred.AsPred().stomachContents.Add(food);
				SoundEngine.PlaySound(
					Main.rand.NextFromCollection(
						food.WeightLeftToDigest <= 0.2
						? pred.AsPred().SmallGulps
						: pred.AsPred().BigGulps
					),
					pred.Center
				);
				switch (food.Type)
				{
					case PreyType.Player:
						Player player = prey as Player;
						player.AsPrey().TotalTimesSwallowed += 1;
						player.AsPrey().IsCurrentlyEaten = true;
						break;
					case PreyType.NPC:
						NPC npc = prey as NPC;
						PreyNPC.UpdateNPCEatenStatus(npc);
						break;
				}
			}
		}

		public static void SwallowWithTextIfApplicable(NPC pred, Player prey, string chatboxText)
		{
			if (!CanSwallow(pred, prey))
				return;

			Swallow(pred, prey);
			SetChatboxText(pred, prey, chatboxText);
		}

		public static void SetChatboxText(NPC pred, Player prey, string chatText)
		{
			Main.CancelHairWindow();
			Main.SetNPCShopIndex(0);
			Main.InGuideCraftMenu = false;
			prey.dropItemCheck();
			Main.npcChatCornerItem = 0;
			prey.sign = -1;
			Main.editSign = false;
			prey.SetTalkNPC(pred.whoAmI);
			Main.playerInventory = false;
			prey.chest = -1;
			Recipe.FindRecipes();
			Main.npcChatText = chatText;
		}

		/// <summary>
		/// Runs update ticks on all food in the given predator's stomach.
		/// </summary>
		/// <param name="npc">The NPC to update all food in the stomach of.</param>
		public static void UpdatePrey(NPC npc)
		{
			if (npc.AsPred().stomachContents is null)
				return;
			npc.AsPred().stomachContents.RemoveAll(x => x.Dead && x.WeightLeftToDigest == 0);
			if (npc.AsPred().stomachContents.Count <= 0)
				return;

			foreach (Prey prey in npc.AsPred().stomachContents)
			{
				prey.timeSpentInStomach++;
				if (!prey.Dead)
				{
					if (prey.Type == PreyType.Player
					 && npc.type == NPCID.Nurse
					 && npc.AsNurse().healPlayerIndex != -1
					 && npc.AsNurse().healPlayerIndex == prey.Index
					 && !npc.AsNurse().digestScamPatient)
					{
						Player healingPreyPlayer = Main.player[prey.Index];
						if (healingPreyPlayer.statLife >= healingPreyPlayer.statLifeMax2)
							npc.AsNurse().healOvertime += 1;
					}

					if (npc.AsPred().GetDigestionTickRateMethod is null || npc.AsPred().GetDigestionTickDamageMethod is null)
					{
						if (ModContent.GetInstance<V2ServerSideConfigs>().DebugChatMessages)
							Main.NewText(npc.FullName + " has invalid digestion damage/tick rate methods!");
						return;
					}
					double digestionDamage = npc.AsPred().GetDigestionTickDamageMethod.Invoke(npc, prey);
					double digestionTickRate = npc.AsPred().GetDigestionTickRateMethod.Invoke(npc, prey);
					int digestionTickFrameRate = (int)Math.Round(60.0 / digestionTickRate);
					if (prey.timeSpentInStomach % (int)digestionTickFrameRate == 0)
					{
						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPlayer = Main.player[prey.Index];
								bool shouldDigestPlayer = true;
								bool shouldHealPlayer = npc.type == NPCID.Nurse && npc.AsNurse().healPlayerIndex != -1 && npc.AsNurse().healPlayerIndex == preyPlayer.whoAmI && !npc.AsNurse().digestScamPatient;
								if (shouldHealPlayer)
								{
									bool shouldFurtherHealPlayer = preyPlayer.statLife < preyPlayer.statLifeMax2;
									if (shouldFurtherHealPlayer)
									{
										prey.Dead = false;
										preyPlayer.statLife += (int)Math.Round(digestionDamage);
										if (preyPlayer.statLife > preyPlayer.statLifeMax2)
											preyPlayer.statLife = preyPlayer.statLifeMax2;
										CombatText digestionText = Main.combatText[CombatText.NewText(
											preyPlayer.Hitbox,
											Color.LimeGreen,
											(int)Math.Round(digestionDamage),
											false,
											true
										)];
										digestionText.position.X = npc.Center.X;
										digestionText.position.X += npc.direction * 14;
										digestionText.position.Y = preyPlayer.Center.Y;
										digestionText.position.Y += preyPlayer.height / 5f;
										digestionText.velocity.X = npc.direction * 2.5f;
										digestionText.velocity.Y = -4f;
									}
								}
								else if (shouldDigestPlayer)
								{
									prey.Dead = preyPlayer.AsPrey().TakeDigestionDamage(npc, digestionDamage);
									if (ModContent.GetInstance<V2ServerSideConfigs>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyPlayer.name);
									if (prey.Dead && npc.AsPred().OnDigestionKillMethod is not null)
										npc.AsPred().OnDigestionKillMethod.Invoke(npc, prey);
								}
								else if (ModContent.GetInstance<V2ServerSideConfigs>().DebugChatMessages)
									Main.NewText("Failed to deal digestion damage to prey: " + preyPlayer.name);
								break;
							case PreyType.NPC:
								NPC preyNPC = Main.npc[prey.Index];
								bool shouldDigestNPC = true;
								if (shouldDigestNPC)
								{
									if (preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerSideConfigs>().EasilyEdibleEmpress)
										digestionDamage *= 50.0;
									prey.Dead = preyNPC.AsPrey().TakeDigestionDamage(preyNPC, npc, digestionDamage);
									if (ModContent.GetInstance<V2ServerSideConfigs>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyNPC.GivenOrTypeName);
									else if (ModContent.GetInstance<V2ServerSideConfigs>().DebugChatMessages)
										Main.NewText("Failed to deal digestion damage to prey: " + preyNPC.GivenOrTypeName);
									if (prey.Dead && npc.AsPred().OnDigestionKillMethod is not null)
										npc.AsPred().OnDigestionKillMethod.Invoke(npc, prey);
								}
								break;
						}
					}
				}
				else
				{
					if (npc.AsPred().GetPreyAbsorptionRateMethod is null)
						continue;

					prey.WeightLeftToDigest -= npc.AsPred().GetPreyAbsorptionRateMethod.Invoke(npc) / (double)npc.AsPred().stomachContents.Count;
					if (prey.WeightLeftToDigest < 0)
						prey.WeightLeftToDigest = 0;
				}
			}

			if (npc.AsPrey(risky: true) is null)
				return;

			if (!npc.AsPrey().IsCurrentlyEaten && npc.AsPred().GetVisualBellySizeMethod is not null)
			{
				bool stomachNoisesPlaying = SoundEngine.TryGetActiveSound(npc.AsPred().ActiveStomachNoises, out ActiveSound stomachNoises);
				if (!stomachNoisesPlaying)
				{
					npc.AsPred().ActiveStomachNoises = SoundEngine.PlaySound(
						StomachNoises.Muffled with { Volume = 0.2f + (0.1f * npc.AsPred().GetVisualBellySizeMethod.Invoke(npc)) },
						npc.TrueCenter()
					);
					SoundEngine.TryGetActiveSound(npc.AsPred().ActiveStomachNoises, out stomachNoises);
				}

				if (stomachNoises is null)
					return;

				stomachNoises.Position = npc.TrueCenter();
				stomachNoises.Volume = 0.2f;
				stomachNoises.Volume += 0.1f * npc.AsPred().GetVisualBellySizeMethod.Invoke(npc);
				if (stomachNoises.Volume > 0.75f)
					stomachNoises.Volume = 0.75f;
			}
		}

		public override void PostAI(NPC npc)
		{
			UpdatePrey(npc);
		}

		public static string GetDigestedPlayerDeathReason(NPC npc, Player player)
		{
			List<string> deathMessageKeyList = new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.Universal.1",
				"Mods.V2.Death.DigestedPlayer.Universal.2",
				"Mods.V2.Death.DigestedPlayer.Universal.3",
				"Mods.V2.Death.DigestedPlayer.Universal.4",
				"Mods.V2.Death.DigestedPlayer.Universal.5",
				"Mods.V2.Death.DigestedPlayer.Universal.6",
				"Mods.V2.Death.DigestedPlayer.Universal.7",
				"Mods.V2.Death.DigestedPlayer.Universal.8",
				"Mods.V2.Death.DigestedPlayer.Universal.9",
				"Mods.V2.Death.DigestedPlayer.Universal.10",
				"Mods.V2.Death.DigestedPlayer.Universal.11",
				"Mods.V2.Death.DigestedPlayer.Universal.12",
				"Mods.V2.Death.DigestedPlayer.Universal.13",
				"Mods.V2.Death.DigestedPlayer.Universal.14",
				"Mods.V2.Death.DigestedPlayer.Universal.15",
				"Mods.V2.Death.DigestedPlayer.Universal.16",
				"Mods.V2.Death.DigestedPlayer.Universal.17",
				"Mods.V2.Death.DigestedPlayer.Universal.18",
				"Mods.V2.Death.DigestedPlayer.Universal.19",
				"Mods.V2.Death.DigestedPlayer.Universal.20",
				"Mods.V2.Death.DigestedPlayer.Universal.21",
				"Mods.V2.Death.DigestedPlayer.Universal.22",
				"Mods.V2.Death.DigestedPlayer.Universal.23",
				"Mods.V2.Death.DigestedPlayer.Universal.24",
				"Mods.V2.Death.DigestedPlayer.Universal.25",
				"Mods.V2.Death.DigestedPlayer.Universal.26",
				"Mods.V2.Death.DigestedPlayer.Universal.27",
				"Mods.V2.Death.DigestedPlayer.Universal.28",
				"Mods.V2.Death.DigestedPlayer.Universal.29",
			};
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathMessageKeyList.AddRange(new List<string>
				{
					"Mods.V2.Death.DigestedPlayer.Hardcore.1",
					"Mods.V2.Death.DigestedPlayer.Hardcore.2",
					"Mods.V2.Death.DigestedPlayer.Hardcore.3",
					"Mods.V2.Death.DigestedPlayer.Hardcore.4",
					"Mods.V2.Death.DigestedPlayer.Hardcore.5",
				});
			}
			if (npc.AsPred().GetDigestedPlayerAdditionalDeathMessagesMethod is not null)
				npc.AsPred().GetDigestedPlayerAdditionalDeathMessagesMethod.Invoke(npc, player, deathMessageKeyList);
			string finalDeathReasonKey = Main.rand.NextFromCollection(deathMessageKeyList);
			
			return Language.GetTextValueWith(
				finalDeathReasonKey,
				new
				{
					Player = player.name,
					Pred = npc.GivenOrTypeName
				}
			);
		}

		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			binaryWriter.Write(npc.AsPred().stomachContents.Count);
			if (npc.AsPred().stomachContents.Count > 0)
			{
				foreach (Prey prey in npc.AsPred().stomachContents)
				{
					binaryWriter.Write(prey.Type switch
					{
						PreyType.Player => 0,
						PreyType.NPC => 1,
						PreyType.Projectile => 2,
						PreyType.Item => 3,
						_ => throw new NotImplementedException(),
					});
					// because EntityID is set automagically on initialization of a Prey instance, this isn't actually needed
					// I'm keepin' it commented out for now just in case it does end up needed
					// binaryWriter.Write(prey.EntityID);
					binaryWriter.Write(prey.Index);
					binaryWriter.Write(prey.Dead);
					binaryWriter.Write(prey.WeightLeftToDigest);
				}
			}
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			npc.AsPred().stomachContents = new List<Prey>();

			// read how many snacks are supposed to be in this NPC's gut, and prepare accordingly
			int gutCount = binaryReader.ReadInt32();
			if (gutCount > 0)
			{
				for (int i = 0; i < gutCount; i++)
				{
					int preyType = binaryReader.ReadInt32();
					// see previous note on EntityID
					// int preyID = binaryReader.ReadInt32();
					int preyIndex = binaryReader.ReadInt32();
					bool preyDead = binaryReader.ReadBoolean();
					double preyWeightLeft = binaryReader.ReadDouble();
					Prey prey = new Prey(preyType switch
					{
						0 => Main.player[preyIndex],
						1 => Main.npc[preyIndex],
						2 => Main.projectile[preyIndex],
						3 => Main.item[preyIndex],
						_ => throw new NotImplementedException(),
					});
					if (preyDead)
					{
						prey.Dead = true;
						prey.WeightLeftToDigest = preyWeightLeft;
					}
					npc.AsPred().stomachContents.Add(prey);
				}
			}
		}

		public override void OnKill(NPC npc)
		{
			if (npc.AsPrey().IsCurrentlyEaten)
			{
				foreach (Prey prey in npc.AsPred().stomachContents)
				{
					Entity betterPred = npc.AsPrey().CurrentCaptor.Value.Predator;
					if (betterPred is NPC npcPred)
					{
						npcPred.AsPred().stomachContentsQueue.Add(prey);
					}
					else if (betterPred is Player playerPred)
					{
						playerPred.AsPred().stomachContentsQueue.Add(prey);
					}
				}
			}

			npc.AsPred().stomachContents.Clear();
		}

		/// <summary>
		/// Calculates the current weight of the given predator's stomach, based on all the prey inside of it.<br/>
		/// Used primarily in conjunction with <see cref="maxStomachCapacity"/> to safeguard against overeating.<br/>
		/// </summary>
		/// <param name="pred">The predator whose stomach is to be weighed.</param>
		/// <returns>The current total weight of the given predator's stomach.</returns>
		public static double GetCurrentBellyWeight(NPC pred)
		{
			double totalBellyWeight = 0.0;
			if (pred.AsPred().stomachContents is not null && pred.AsPred().stomachContents.Count > 0)
			{
				foreach (Prey prey in pred.AsPred().stomachContents)
				{
					totalBellyWeight += prey.WeightLeftToDigest;
					if (prey.Dead)
						continue;

					switch (prey.Type)
					{
						case PreyType.Player:
							Player preyPredPlayer = Main.player[prey.Index];
							totalBellyWeight += PredPlayer.GetCurrentBellyWeight(preyPredPlayer);
							break;
						case PreyType.NPC:
							NPC preyPredNPC = Main.npc[prey.Index];
							totalBellyWeight += GetCurrentBellyWeight(preyPredNPC);
							break;
					}
				}
			}
			return totalBellyWeight;
		}

		public static bool AnyPreyStillAlive(NPC pred)
		{
			if (pred.AsPred().stomachContents is not null && pred.AsPred().stomachContents.Count > 0)
			{
				foreach (Prey prey in pred.AsPred().stomachContents)
				{
					if (!prey.Dead)
						return true;
				}
			}
			return false;
		}
	}
}

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
using V2.Items;
using V2.NPCs.Vanilla.TownNPCs.Nurse;
using V2.PlayerHandling;
using V2.Sounds.Vore;
using V2.StatusEffects.Debuffs;

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
		public EntityGender Gender { get; set; }
		public List<Prey> stomachContents;
		public List<Prey> stomachContentsQueue;
		public EntityDigestionType DigestionType { get; set; }
		public double MaxStomachCapacity { get; set; }
		public float MaxSwallowRange { get; set; }
		public double ExtraWeight { get; set; }

		public SoundStyle? SmallBurps { get; set; }
		public SoundStyle? StandardBurps { get; set; }
		public SoundStyle? BigBurps { get; set; }

		public SoundStyle SmallGulps { get; set; }
		public double SmallGulpThreshold { get; set; }
		public SoundStyle BigGulps { get; set; }

		public delegate void DelegateResetPredSpecificVariables(NPC npc);
		public DelegateResetPredSpecificVariables ResetPredSpecificVariables { get; set; }

		public delegate List<string> DelegateGetTownNPCChat(NPC npc, Player player);
		public DelegateGetTownNPCChat GetChat { get; set; }

		public bool NonPreferenceBypass { get; set; }
		public delegate bool DelegateCanBeForceFed(NPC npc);
		public DelegateCanBeForceFed CanBeForceFed { get; set; }

		public delegate void DelegateOnForceFed(NPC npc, Player player);
		public DelegateOnForceFed OnForceFed { get; set; }


		public delegate bool DelegateSpecialPredAI(NPC npc);
		public DelegateSpecialPredAI SpecialPredAI { get; set; }

		public delegate double DelegateGetDigestionTickRate(NPC npc, Prey prey);
		public DelegateGetDigestionTickRate GetDigestionTickRate { get; set; }

		public delegate double DelegateGetDigestionTickDamage(NPC npc, Prey prey);
		public DelegateGetDigestionTickDamage GetDigestionTickDamage { get; set; }

		public delegate void DelegateOnDigestionKill(NPC npc, Prey digestedPrey);
		public DelegateOnDigestionKill OnDigestionKill { get; set; }

		public delegate void DelegateGetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathMessageKeyList);
		public DelegateGetDigestedPlayerAdditionalDeathMessages GetAdditionalDigestedPlayerMessages { get; set; }

		public delegate double DelegateGetPreyAbsorptionRate(NPC npc);
		public DelegateGetPreyAbsorptionRate GetPreyAbsorptionRate { get; set; }

		public delegate int DelegateGetVisualBellySize(NPC npc);
		public DelegateGetVisualBellySize GetVisualBellySize { get; set; }

		public delegate int DelegateGetVisualWeightStage(NPC npc);
		public DelegateGetVisualWeightStage GetVisualWeightStage { get; set; }

		public SlotId ActiveStomachNoises { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;

		public PredNPC()
		{
			stomachContents = new List<Prey>();
			stomachContentsQueue = new List<Prey>();
			MaxStomachCapacity = 1.0;
			MaxSwallowRange = 36f;
			ExtraWeight = 0.0;
			
			// This is where all the defaults methods get set.
			ResetPredSpecificVariables = null;
			GetDigestionTickRate = null;
			GetDigestionTickDamage = null;
			GetPreyAbsorptionRate = null;
			GetChat = null;
			SpecialPredAI = null;

			NonPreferenceBypass = false;
			CanBeForceFed = (NPC npc) => false;
			OnForceFed = null;

			OnDigestionKill = null;

			GetVisualBellySize = null;
			GetVisualWeightStage = null;

			SmallBurps = null;
			StandardBurps = null;
			BigBurps = null;

			SmallGulps = Gulps.Short;
			SmallGulpThreshold = 0.2;
			BigGulps = Gulps.Standard;
		}

		public override void ResetEffects(NPC npc)
		{
			while (npc.AsPred().stomachContentsQueue.Count > 0)
			{
				npc.AsPred().stomachContents.Add(npc.AsPred().stomachContentsQueue.First());
				npc.AsPred().stomachContentsQueue.Remove(npc.AsPred().stomachContentsQueue.First());
			}

			if (npc.AsPred().ResetPredSpecificVariables is not null)
				npc.AsPred().ResetPredSpecificVariables.Invoke(npc);
		}

		public static bool CanSwallow(NPC pred, Entity prey)
		{
			if (V2.VoreNPCBlacklist.Contains(pred.type))
				return false;

			if (GetCurrentBellyWeight(pred) >= pred.AsPred().MaxStomachCapacity)
				return false;

			switch (ModContent.GetInstance<V2ServerConfig>().GenderBlacklist)
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
				if (Prey.GetInitialPreySize(preyPlayer) >= pred.AsPred().MaxStomachCapacity - GetCurrentBellyWeight(pred))
					return false;

				if (preyPlayer.AsFood().IsCurrentlyEaten)
					return false;
			}
			else if (prey is NPC preyNPC)
			{
				if (V2.VoreNPCBlacklist.Contains(preyNPC.type))
					return false;

				bool tastesLikeSkittles = preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress;
				if (tastesLikeSkittles)
					return !preyNPC.AsFood().IsCurrentlyEaten;

				bool isThisAFuckingBoss = preyNPC.boss || (preyNPC.type >= NPCID.EaterofWorldsHead && preyNPC.type <= NPCID.EaterofWorldsTail); // I hate EoW
				if (isThisAFuckingBoss && !pred.boss)
					return false;

				if (Prey.GetInitialPreySize(preyNPC) >= pred.AsPred().MaxStomachCapacity - GetCurrentBellyWeight(pred))
					return false;

				if (preyNPC.AsFood().IsCurrentlyEaten)
					return false;
			}
			else if (prey is Item preyItem)
			{
				if (preyItem.AsFood().MaxHealth == -1)
					return false;

				if (preyItem.favorited)
					return false;

				if (preyItem.AsFood().IsCurrentlyEaten)
					return false;
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
					food.WeightLeftToDigest <= pred.AsPred().SmallGulpThreshold
						? pred.AsPred().SmallGulps
						: pred.AsPred().BigGulps,
					pred.Center
				);
				switch (food.Type)
				{
					case PreyType.Player:
						Player player = prey as Player;
						player.AsFood().TotalTimesSwallowed += 1;
						player.AsFood().IsCurrentlyEaten = true;
						break;
					case PreyType.NPC:
						NPC npc = prey as NPC;
						PreyNPC.UpdateNPCEatenStatus(npc);
						break;
					case PreyType.Item:
						Item item = prey as Item;
						item.AsFood().OnSwallow?.Invoke(item, pred);
						if (item.AsFood().OnSwallowDamage > 0)
						{
							pred.StrikeNPC(
								new NPC.HitInfo
								{
									SourceDamage = item.AsFood().OnSwallowDamage,
									DamageType = DamageClass.Default,
									Crit = false,
									HideCombatText = true
								}
							);
						}
						if (item.AsFood().OnSwallowSoreThroatTime > 0)
							pred.AddBuff(ModContent.BuffType<SoreThroat>(), item.AsFood().OnSwallowSoreThroatTime);
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
			npc.AsPred().stomachContents.RemoveAll(x => x.NoHealth && x.WeightLeftToDigest == 0);
			if (npc.AsPred().stomachContents.Count <= 0)
				return;

			foreach (Prey prey in npc.AsPred().stomachContents)
			{
				prey.timeSpentInStomach++;

				switch (prey.Type)
				{
					case PreyType.Item:
						Item preyItem = prey.Instance as Item;
						preyItem.AsFood().UpdateInStomach?.Invoke(preyItem, npc, prey.NoHealth);
						break;
				}

				if (!prey.NoHealth)
				{
					if (prey.Type == PreyType.Player
					 && npc.type == NPCID.Nurse
					 && npc.AsNurse().healPlayerIndex != -1
					 && npc.AsNurse().healPlayerIndex == (prey.Instance as Player).whoAmI
					 && !npc.AsNurse().digestScamPatient)
					{
						Player healingPreyPlayer = prey.Instance as Player;
						if (healingPreyPlayer.statLife >= healingPreyPlayer.statLifeMax2)
							npc.AsNurse().healOvertime += 1;
					}

					if (npc.AsPred().GetDigestionTickRate is null || npc.AsPred().GetDigestionTickDamage is null)
					{
						if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
							Main.NewText(npc.FullName + " has invalid digestion damage/tick rate methods!");
						return;
					}
					double digestionDamage = npc.AsPred().GetDigestionTickDamage.Invoke(npc, prey);
					double digestionTickRate = npc.AsPred().GetDigestionTickRate.Invoke(npc, prey);
					int digestionTickFrameRate = (int)Math.Round(60.0 / digestionTickRate);
					if (prey.timeSpentInStomach % (int)digestionTickFrameRate == 0)
					{
						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPlayer = prey.Instance as Player;
								bool shouldDigestPlayer = true;
								bool shouldHealPlayer = npc.type == NPCID.Nurse && npc.AsNurse().healPlayerIndex != -1 && npc.AsNurse().healPlayerIndex == preyPlayer.whoAmI && !npc.AsNurse().digestScamPatient;
								if (shouldHealPlayer)
								{
									bool shouldFurtherHealPlayer = preyPlayer.statLife < preyPlayer.statLifeMax2;
									if (shouldFurtherHealPlayer)
									{
										prey.NoHealth = false;
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
									prey.NoHealth = preyPlayer.AsFood().TakeDigestionDamage(npc, digestionDamage);
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyPlayer.name);
									if (prey.NoHealth && npc.AsPred().OnDigestionKill is not null)
										npc.AsPred().OnDigestionKill.Invoke(npc, prey);
								}
								else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
									Main.NewText("Failed to deal digestion damage to prey: " + preyPlayer.name);
								break;
							case PreyType.NPC:
								NPC preyNPC = prey.Instance as NPC;
								bool shouldDigestNPC = true;
								if (shouldDigestNPC)
								{
									if (preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress)
										digestionDamage *= 50.0;
									prey.NoHealth = PreyNPC.TakeDigestionDamage(preyNPC, npc, digestionDamage);
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyNPC.GivenOrTypeName);
									else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Failed to deal digestion damage to prey: " + preyNPC.GivenOrTypeName);
									if (prey.NoHealth && npc.AsPred().OnDigestionKill is not null)
										npc.AsPred().OnDigestionKill.Invoke(npc, prey);
								}
								break;
						}
					}
				}
				else
				{
					if (npc.AsPred().GetPreyAbsorptionRate is null)
						continue;

					double digestedWeightPerTick = npc.AsPred().GetPreyAbsorptionRate.Invoke(npc) / (double)npc.AsPred().stomachContents.Count;
					if (prey.WeightLeftToDigest <= digestedWeightPerTick)
					{
						npc.AsPred().ExtraWeight += prey.WeightLeftToDigest * 0.4;
						prey.WeightLeftToDigest = 0;
					}
					else
					{
						npc.AsPred().ExtraWeight += digestedWeightPerTick * 0.4;
						prey.WeightLeftToDigest -= digestedWeightPerTick;
					}
					switch (prey.Type)
					{
						case PreyType.Item:
							Item item = prey.Instance as Item;
							item.AsFood().FullyDigested = true;
							break;
					}
				}
			}

			if (npc.AsFood(risky: true) is null)
				return;

			if (!npc.AsFood().IsCurrentlyEaten && npc.AsPred().GetVisualBellySize is not null)
			{
				bool stomachNoisesPlaying = SoundEngine.TryGetActiveSound(npc.AsPred().ActiveStomachNoises, out ActiveSound stomachNoises);
				if (!stomachNoisesPlaying)
				{
					npc.AsPred().ActiveStomachNoises = SoundEngine.PlaySound(
						StomachNoises.Muffled with { Volume = 0.2f + (0.1f * npc.AsPred().GetVisualBellySize.Invoke(npc)) },
						npc.TrueCenter()
					);
					SoundEngine.TryGetActiveSound(npc.AsPred().ActiveStomachNoises, out stomachNoises);
				}

				if (stomachNoises is null)
					return;

				stomachNoises.Position = npc.TrueCenter();
				stomachNoises.Volume = 0.2f;
				stomachNoises.Volume += 0.1f * npc.AsPred().GetVisualBellySize.Invoke(npc);
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
			};
			switch (npc.AsPred().DigestionType)
			{
				case EntityDigestionType.Acidic:
					deathMessageKeyList.AddRange(new List<string>
					{
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.1",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.2",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.3",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.4",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.5",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.6",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.7",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.8",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.9",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.10",
					});
					break;
				case EntityDigestionType.Thermal:
					deathMessageKeyList.AddRange(new List<string>
					{
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.1",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.2",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.3",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.4",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.5",
					});
					break;
				case EntityDigestionType.Other:
					break;
			}

			if (npc.AsPred().GetAdditionalDigestedPlayerMessages is not null)
				npc.AsPred().GetAdditionalDigestedPlayerMessages.Invoke(npc, player, deathMessageKeyList);
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
					// binaryWriter.Write((prey.Instance as NPC).type);
					binaryWriter.Write(prey.Instance.whoAmI);
					binaryWriter.Write(prey.NoHealth);
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
						prey.NoHealth = true;
						prey.WeightLeftToDigest = preyWeightLeft;
					}
					npc.AsPred().stomachContents.Add(prey);
				}
			}
		}

		public override void OnKill(NPC npc)
		{
			if (npc.AsFood().IsCurrentlyEaten)
			{
				foreach (Prey prey in npc.AsPred().stomachContents)
				{
					Entity betterPred = npc.AsFood().CurrentCaptor.Value.Predator;
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
		/// Used primarily in conjunction with <see cref="MaxStomachCapacity"/> to safeguard against overeating.<br/>
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
					if (prey.NoHealth)
						continue;

					switch (prey.Type)
					{
						case PreyType.Player:
							Player preyPredPlayer = prey.Instance as Player;
							totalBellyWeight += preyPredPlayer.AsPred().StomachWeight;
							break;
						case PreyType.NPC:
							NPC preyPredNPC = prey.Instance as NPC;
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
					if (!prey.NoHealth)
						return true;
				}
			}
			return false;
		}
	}
}

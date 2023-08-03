using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.Items;
using V2.NPCs;
using V2.Sounds.Vore;
using V2.StatusEffects.Debuffs;

namespace V2.PlayerHandling
{
	public class PredStat
	{
		public int Base { get; set; }
		public int Extra { get; set; }
		public int Total => Base + Extra;

		public PredStat()
		{
			Base = 0;
			Extra = 0;
		}

		public void Reset()
		{
			Base = 0;
			Extra = 0;
		}
	}
	public class PredPlayer : ModPlayer
	{
		public List<Prey> stomachContents;
		public List<Prey> stomachContentsQueue;

		public double Stomachache;

		public int predLevel;

		public PredStat GLP { get; set; }
		public StatModifier SwallowSizeModifier;
		public double SwallowSize
		{
			get
			{
				double baseSwallowSize = 0.6;
				baseSwallowSize += 0.05 * GLP.Total;
				return SwallowSizeModifier.ApplyTo((float)baseSwallowSize);
			}
		}
		public StatModifier StomachacheGraceTimeModifier;
		public double StomachacheGraceTime
		{
			get
			{
				double baseGracePeriod = 0.1;
				baseGracePeriod += 0.1 * (GLP.Total / 5);
				return StomachacheGraceTimeModifier.ApplyTo((float)baseGracePeriod);
			}
		}
		public PredStat ACI { get; set; }
		public StatModifier DigestionTickDamageModifier;
		public double DigestionTickDamage
		{
			get
			{
				double baseDigestionDamage = 6.0;
				baseDigestionDamage += 0.75 * ACI.Total;
				return DigestionTickDamageModifier.ApplyTo((float)baseDigestionDamage);
			}
		}
		public StatModifier DigestionTickRateModifier;
		public double DigestionTickRate
		{
			get
			{
				double baseDigestionRate = 1.0;
				baseDigestionRate += 0.1 * (ACI.Total / 5);
				return DigestionTickRateModifier.ApplyTo((float)baseDigestionRate);
			}
		}
		public PredStat TUM { get; set; }
		public StatModifier StomachCapacityModifier;
		public double StomachCapacity
		{
			get
			{
				double baseStomachCapacity = 3.0;
				baseStomachCapacity += 0.04 * TUM.Total;
				return StomachCapacityModifier.ApplyTo((float)baseStomachCapacity);
			}
		}
		public StatModifier StomachacheMeterCapacityModifier;
		public double StomachacheMeterCapacity
		{
			get
			{
				double baseStomachacheMeterCapacity = 100.0;
				baseStomachacheMeterCapacity += 10.0 * (TUM.Total / 5);
				return StomachacheMeterCapacityModifier.ApplyTo((float)baseStomachacheMeterCapacity);
			}
		}
		public PredStat ABS { get; set; }
		public StatModifier PreyAbsorptionRateModifier;
		public double PreyAbsorptionRate
		{
			get
			{
				double basePreyAbsorptionRate = 0.2;
				basePreyAbsorptionRate += 0.005 * ABS.Total;
				basePreyAbsorptionRate *= 4.0;
				return PreyAbsorptionRateModifier.ApplyTo((float)(basePreyAbsorptionRate / (60.0 * 60.0)));
			}
		}
		public StatModifier BuffExtensionTimeModifier;
		public double BuffExtensionTime
		{
			get
			{
				double baseBuffExtensionTime = 0.0;
				baseBuffExtensionTime += 0.01 * (ABS.Total / 5);
				return BuffExtensionTimeModifier.ApplyTo((float)baseBuffExtensionTime);
			}
		}

		public SoundStyle SmallBurps { get; set; }
		public SoundStyle StandardBurps { get; set; }
		public SoundStyle BigBurps { get; set; }

		public List<SoundStyle> SmallGulps { get; set; }
		public List<SoundStyle> BigGulps { get; set; }

		public bool charmBracelet;
		public int CharmBraceletSlots
		{
			get
			{
				if (predLevel >= 10)
					return 2;

				return 1;
			}
		}

		public bool charmNoDigest;
		public bool charmNoAirDrain;

		public bool endoToggleUnlocked;
		public bool endoToggle;
		public bool SafeStomach => (charmNoDigest && charmNoAirDrain) || endoToggle;

		public string lastEntitySwallowed;
		public string lastEntitySwallowedMod;
		public Dictionary<string, int> mealCount;
		public int TotalMeals
		{
			get
			{
				if (mealCount.Count <= 0)
					return 0;

				int meals = 0;
				foreach (KeyValuePair<string, int> keyValuePair in mealCount)
				{
					meals += keyValuePair.Value;
				}
				return meals;
			}
		}

		public double specialHealthRegenCount;
		public double specialManaRegenCount;

		public bool BlockSwallowAttempts {
			get {
				if (Player.AsFood().IsCurrentlyEaten)
					return true;

				if (Player.HasBuff(ModContent.BuffType<SoreThroat>()))
					return true;

				return false;
			}
		}
		public SlotId ActiveStomachNoises { get; set; }

		public StatModifier StomachSizeModifier;
		public double StomachFullness
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (stomachContents is not null && stomachContents.Count > 0)
				{
					foreach (Prey prey in stomachContents)
					{
						totalBellyWeight += prey.WeightLeftToDigest;
						if (prey.Dead)
							continue;

						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachWeight;
								break;
							case PreyType.NPC:
								NPC preyPredNPC = prey.Instance as NPC;
								totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
								break;
						}
					}
				}
				return totalBellyWeight;
			}
		}

		public double KickyStomachFullness
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (stomachContents is not null && stomachContents.Count > 0)
				{
					foreach (Prey prey in stomachContents)
					{
						if (prey.Dead)
							continue;

						totalBellyWeight += prey.WeightLeftToDigest;
						if (prey.Dead)
							continue;

						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachWeight;
								break;
							case PreyType.NPC:
								NPC preyPredNPC = prey.Instance as NPC;
								totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
								break;
						}
					}
				}
				return totalBellyWeight;
			}
		}

		public StatModifier StomachWeightModifier;
		public double StomachWeight
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (stomachContents is not null && stomachContents.Count > 0)
				{
					foreach (Prey prey in stomachContents)
					{
						totalBellyWeight += prey.WeightLeftToDigest;
						if (prey.Dead)
							continue;

						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachWeight;
								break;
							case PreyType.NPC:
								NPC preyPredNPC = prey.Instance as NPC;
								totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
								break;
						}
					}
				}
				return (double)StomachWeightModifier.ApplyTo((float)totalBellyWeight);
			}
		}

		public double KickyStomachWeight
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (stomachContents is not null && stomachContents.Count > 0)
				{
					foreach (Prey prey in stomachContents)
					{
						if (prey.Dead)
							continue;

						totalBellyWeight += prey.WeightLeftToDigest;
						if (prey.Dead)
							continue;

						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachWeight;
								break;
							case PreyType.NPC:
								NPC preyPredNPC = prey.Instance as NPC;
								totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
								break;
						}
					}
				}
				return (double)StomachWeightModifier.ApplyTo((float)totalBellyWeight);
			}
		}

		public override void Initialize()
		{
			stomachContents = new List<Prey>();
			stomachContentsQueue = new List<Prey>();

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

			GLP = new PredStat();
			ACI = new PredStat();
			TUM = new PredStat();
			ABS = new PredStat();

			endoToggleUnlocked = false;
			endoToggle = false;

			lastEntitySwallowed = null;
			lastEntitySwallowedMod = null;
			mealCount = new Dictionary<string, int>();
		}

		public override void ResetEffects()
		{
			while (Player.AsPred().stomachContentsQueue is not null && Player.AsPred().stomachContentsQueue.Count > 0)
			{
				Player.AsPred().stomachContents.Add(Player.AsPred().stomachContentsQueue.First());
				Player.AsPred().stomachContentsQueue.Remove(Player.AsPred().stomachContentsQueue.First());
			}

			GLP.Extra = 0;
			SwallowSizeModifier = StatModifier.Default;
			StomachacheGraceTimeModifier = StatModifier.Default;
			ACI.Extra = 0;
			DigestionTickDamageModifier = StatModifier.Default;
			DigestionTickRateModifier = StatModifier.Default;
			TUM.Extra = 0;
			StomachCapacityModifier = StatModifier.Default;
			StomachacheMeterCapacityModifier = StatModifier.Default;
			ABS.Extra = 0;
			PreyAbsorptionRateModifier = StatModifier.Default;
			BuffExtensionTimeModifier = StatModifier.Default;

			StomachWeightModifier = StatModifier.Default;
		}

		public override bool HoverSlot(Item[] inventory, int context, int slot)
		{
			if (inventory.Length == 59)
			{
				if (V2.ItemGulpHotkey.Current && V2.SwallowHotkey.JustPressed)
				{
					if (CanSwallow(Player, inventory[slot]))
					{
						Player.ForceDropItem(Player.Center, ref inventory[slot], out Item droppedItem);
						Swallow(Player, droppedItem);
					}
				}
			}
			return false;
		}

		public override void PostItemCheck()
		{
			if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer)
			{
				if (V2.SwallowHotkey.JustPressed && !V2.ItemGulpHotkey.Current)
				{
					if (Player.AsPred().BlockSwallowAttempts)
						goto UpdatePrey;

					string mealType = "none";
					int mealIndex = -1;
					Vector2 playerLocation = Player.MountedCenter;
					Vector2 cursorLocation = Main.MouseWorld;
					double maxDistanceFromPlayer = V2Utils.TileCountAsPixelCount(4.25);
					double maxDistanceFromCursor = 2000;
					for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
					{
						NPC potentialMeal = Main.npc[npcIndex];
						if (!potentialMeal.active)
							continue;

						if (potentialMeal.realLife != -1 && potentialMeal.realLife != potentialMeal.whoAmI)
							continue;

						if (potentialMeal.AsFood().IsCurrentlyEaten)
							continue;

						if (!Collision.CanHit(Player.TrueCenter(), 1, 1, potentialMeal.TrueCenter(), 1, 1))
							continue;

						if (potentialMeal.Distance(playerLocation) >= maxDistanceFromPlayer)
							continue;

						if (potentialMeal.Distance(cursorLocation) < maxDistanceFromCursor)
						{
							mealIndex = npcIndex;
							mealType = "NPC";
							maxDistanceFromCursor = potentialMeal.Distance(cursorLocation);
						}
					}
					for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
					{
						Player potentialMeal = Main.player[playerIndex];
						if (!potentialMeal.active || potentialMeal.whoAmI == Player.whoAmI)
							continue;

						if (potentialMeal.AsFood().IsCurrentlyEaten)
							continue;

						if (!Collision.CanHit(Player.TrueCenter(), 1, 1, potentialMeal.TrueCenter(), 1, 1))
							continue;

						if (potentialMeal.Distance(playerLocation) >= maxDistanceFromPlayer)
							continue;

						if (potentialMeal.Distance(cursorLocation) < maxDistanceFromCursor)
						{
							mealIndex = playerIndex;
							mealType = "player";
							maxDistanceFromCursor = potentialMeal.Distance(cursorLocation);
						}
					}

					if (mealType != "none" && mealIndex != -1)
					{
						switch (mealType)
						{
							case "NPC":
								Swallow(Player, Main.npc[mealIndex]);
								Player.lastCreatureHit = Item.NPCtoBanner(Main.npc[mealIndex].BannerID());
								break;
							case "player":
								Swallow(Player, Main.player[mealIndex]);
								break;
						}
					}
				}

				if (V2.RegurgitateHotkey.JustPressed)
				{
					if (Player.AsPred().BlockSwallowAttempts)
						goto UpdatePrey;

					if (Player.AsPred().stomachContents is null || Player.AsPred().stomachContents.Count <= 0)
						goto UpdatePrey;

					Prey prey = Player.AsPred().stomachContents.FindLast(x => !x.Dead);
					if (prey is not null)
					{
						Entity realPrey = prey.Type switch
						{
							PreyType.Player => prey.Instance as Player,
							PreyType.NPC => prey.Instance as NPC,
							PreyType.Projectile => prey.Instance as Projectile,
							PreyType.Item => prey.Instance as Item,
							PreyType.Custom => null,
							_ => throw new NotImplementedException(),
						};
						realPrey.position = Player.TrueCenter() + new Vector2(Player.direction * 8f, -14f);
						realPrey.velocity = new Vector2(Player.direction * 12.5f, -2.5f);
						if (realPrey is NPC realPreyNPC)
						{
							realPreyNPC.AsFood().IsCurrentlyEaten = false;
							realPreyNPC.AsFood().CurrentCaptor = null;
							realPreyNPC.AsFood().EatenSafetyFrames = 20;
						}
						else if (realPrey is Player realPreyPlayer)
						{
							realPreyPlayer.AsFood().IsCurrentlyEaten = false;
							realPreyPlayer.AsFood().CurrentCaptor = null;
						}
						else if (realPrey is Item realPreyItem)
						{
							realPreyItem.AsFood().IsCurrentlyEaten = false;
							realPreyItem.AsFood().CurrentCaptor = null;
							realPreyItem.noGrabDelay = 60;
						}
						Player.AsPred().stomachContents.Remove(prey);
						SoundEngine.PlaySound(
							prey.WeightLeftToDigest <= 0.3 ? Player.AsPred().SmallBurps : Player.AsPred().StandardBurps,
							Player.TrueCenter() + new Vector2(Player.direction * 8f, -14f)
						);
					}
				}
			}

			UpdatePrey:
			UpdatePrey(Player);
		}
		
		public static bool CanSwallow(Player pred, Entity prey)
		{
			if (pred.AsPred().BlockSwallowAttempts)
				return false;

			switch (ModContent.GetInstance<V2ServerConfig>().GenderBlacklist)
			{
				default:
					// do absolutely fucking nothing lmao
					break;
				case "No Male":
					if (pred.Male)
						return false;
					break;
				case "No Female":
					if (!pred.Male)
						return false;
					break;
				case "No M or F...but why?":
					return false;
			}

			if (prey is Player preyPlayer)
			{
				if (preyPlayer.AsFood().IsCurrentlyEaten)
					return false;
			}
			else if (prey is NPC preyNPC)
			{
				if (V2.VoreNPCBlacklist.Contains(preyNPC.type))
					return false;

				bool tastesLikeSkittles = preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress;
				if (tastesLikeSkittles)
					return true;

				bool isThisAFuckingBoss = preyNPC.boss || (preyNPC.type >= NPCID.EaterofWorldsHead && preyNPC.type <= NPCID.EaterofWorldsTail); // I hate EoW
				if (isThisAFuckingBoss)
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

			if (Prey.GetInitialPreySize(prey) > pred.AsPred().StomachCapacity - GetCurrentBellyWeight(pred))
				return false;

			return true;
		}

		/// <summary>
		/// Causes the given predator player to swallow the given prey entity, if the given prey entity can be swallowed.
		/// </summary>
		/// <param name="pred">The predator which will attempt to swallow the given prey.</param>
		/// <param name="prey">The prey which will be attempt to be swallowed by the given predator.</param>
		public static void Swallow(Player pred, Entity prey)
		{
			if (CanSwallow(pred, prey))
			{
				if (pred.AsPred().stomachContents is null || pred.AsPred().stomachContents.Count <= 0)
					pred.AsPred().stomachContents = new List<Prey>();

				Prey food = new Prey(prey);
				pred.AsPred().stomachContents.Add(food);
				if (prey is not NPC preyNPC || preyNPC.realLife == -1)
				{
					SoundEngine.PlaySound(
						Main.rand.NextFromCollection(
							food.WeightLeftToDigest <= 0.3
							? pred.AsPred().SmallGulps
							: pred.AsPred().BigGulps
						),
						pred.Center
					);
				}
				switch (food.Type)
				{
					case PreyType.NPC:
						NPC npc = prey as NPC;
						npc.AsFood().IsCurrentlyEaten = true;
						npc.AsFood().CurrentCaptor = new PredEntityReference()
						{
							Predator = pred,
							PreyInstance = food
						};
						if (npc.realLife != 1 && npc.realLife == npc.whoAmI)
						{
							for (int i = 0; i < Main.maxNPCs; i++)
							{
								if (Main.npc[i].whoAmI != npc.whoAmI && Main.npc[i].realLife == npc.whoAmI)
								{
									Swallow(pred, Main.npc[i]);
								}
							}
						}

						pred.AsPred().lastEntitySwallowed = npc.TypeName;
						pred.AsPred().lastEntitySwallowedMod = npc.ModNPC != null ? npc.ModNPC.Mod.DisplayName : "Terraria";
						break;
					case PreyType.Player:
						Player player = prey as Player;
						player.AsFood().IsCurrentlyEaten = true;
						player.AsFood().CurrentCaptor = new PredEntityReference()
						{
							Predator = pred,
							PreyInstance = food
						};
						player.AsFood().TotalTimesSwallowed += 1;
						pred.AsPred().lastEntitySwallowed = "Player";
						pred.AsPred().lastEntitySwallowedMod = "Terraria";
						break;
					case PreyType.Item:
						Item item = prey as Item;
						item.AsFood().IsCurrentlyEaten = true;
						item.AsFood().CurrentCaptor = new PredEntityReference()
						{
							Predator = pred,
							PreyInstance = food
						};
						pred.AsPred().lastEntitySwallowed = item.Name;
						pred.AsPred().lastEntitySwallowedMod = item.ModItem != null ? item.ModItem.Mod.DisplayName : "Terraria";

						item.AsFood().OnSwallow?.Invoke(item, pred);
						if (item.AsFood().OnSwallowDamage > 0 && item.AsFood().OnSwallowDeathReason is not null)
						{
							pred.Hurt(
								damageSource: PlayerDeathReason.ByCustomReason(item.AsFood().OnSwallowDeathReason),
								Damage: item.AsFood().OnSwallowDamage,
								hitDirection: 0,
								dodgeable: false,
								scalingArmorPenetration: 1f
							);
						}
						if (item.AsFood().OnSwallowSoreThroatTime > 0)
							pred.AddBuff(ModContent.BuffType<SoreThroat>(), item.AsFood().OnSwallowSoreThroatTime);
						break;
				}
			}
		}

		/// <summary>
		/// Runs update ticks on all food in this predatory player's stomach.
		/// </summary>
		public static void UpdatePrey(Player player)
		{
			if (player.AsPred().stomachContents is null)
				return;
			player.AsPred().stomachContents.RemoveAll(x => x.Dead && x.WeightLeftToDigest <= 0);
			if (player.AsPred().stomachContents.Count <= 0)
				return;

			foreach (Prey prey in player.AsPred().stomachContents)
			{
				prey.timeSpentInStomach++;

				switch (prey.Type)
				{
					case PreyType.Item:
						Item preyItem = prey.Instance as Item;
						if (!preyItem.IsAir)
							preyItem.AsFood().UpdateInStomach?.Invoke(preyItem, player, prey.Dead);
						break;
				}

				if (!prey.Dead)
				{
					double digestionDamage = player.AsPred().DigestionTickDamage;
					double digestionRate = player.AsPred().DigestionTickRate;
					int digestionFrameRate = (int)Math.Round(60.0 / digestionRate);
					if (prey.timeSpentInStomach % digestionFrameRate == 0)
					{
						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPlayer = prey.Instance as Player;
								bool shouldDigestPlayer = !player.AsPred().SafeStomach;
								if (shouldDigestPlayer)
								{
									prey.Dead = preyPlayer.AsFood().TakeDigestionDamage(player, digestionDamage);
									if (prey.Dead)
									{
										if (!player.AsPred().mealCount.ContainsKey("Terraria: Player"))
											player.AsPred().mealCount.Add("Terraria: Player", 0);
										player.AsPred().mealCount["Terraria: Player"] += 1;
										SoundEngine.PlaySound(
											player.AsPred().StandardBurps,
											player.TrueCenter() + new Vector2(player.direction * 8f, -14f)
										);
									}
								}
								break;
							case PreyType.NPC:
								NPC preyNPC = prey.Instance as NPC;
								bool shouldDigestNPC = !player.AsPred().SafeStomach;
								if (shouldDigestNPC)
								{
									if (preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress)
										digestionDamage *= 40.0;
									prey.Dead = PreyNPC.TakeDigestionDamage(preyNPC, player, digestionDamage);
									if (prey.Dead)
									{
										string preyNPCMod = preyNPC.ModNPC != null ? preyNPC.ModNPC.Mod.DisplayName : "Terraria";
										if (!player.AsPred().mealCount.ContainsKey(preyNPCMod + ": " + preyNPC.TypeName))
											player.AsPred().mealCount.Add(preyNPCMod + ": " + preyNPC.TypeName, 0);
										player.AsPred().mealCount[preyNPCMod + ": " + preyNPC.TypeName] += 1;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? player.AsPred().SmallBurps : player.AsPred().StandardBurps,
											player.TrueCenter() + new Vector2(player.direction * 8f, -14f)
										);
									}
								}
								break;
							case PreyType.Item:
								Item preyItem = prey.Instance as Item;
								if (preyItem.IsAir)
									break;

								bool shouldDigestItem = !player.AsPred().SafeStomach;
								if (shouldDigestItem)
								{
									prey.Dead = preyItem.TakeDigestionDamage(player, digestionDamage);
									if (prey.Dead)
									{
										string preyItemMod = preyItem.ModItem != null ? preyItem.ModItem.Mod.DisplayName : "Terraria";
										if (!player.AsPred().mealCount.ContainsKey(preyItemMod + ": " + preyItem.Name))
											player.AsPred().mealCount.Add(preyItemMod + ": " + preyItem.Name, 0);
										player.AsPred().mealCount[preyItemMod + ": " + preyItem.Name] += preyItem.stack;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? player.AsPred().SmallBurps : player.AsPred().StandardBurps,
											player.TrueCenter() + new Vector2(player.direction * 8f, -14f)
										);
									}
								}
								break;
						}
					}
				}
				else
				{
					prey.WeightLeftToDigest -= player.AsPred().PreyAbsorptionRate / (double)player.AsPred().stomachContents.Count;
					if (prey.WeightLeftToDigest < 0)
						prey.WeightLeftToDigest = 0;

					switch (prey.Type)
					{
						case PreyType.Item:
							Item preyItem = prey.Instance as Item;
							if (!preyItem.IsAir && prey.WeightLeftToDigest == 0)
								preyItem.AsFood().FullyDigested = true;
							break;
					}
				}
			}

			if (!player.AsFood().IsCurrentlyEaten)
			{
				bool stomachNoisesPlaying = SoundEngine.TryGetActiveSound(player.AsPred().ActiveStomachNoises, out ActiveSound stomachNoises);
				if (!stomachNoisesPlaying)
				{
					player.AsPred().ActiveStomachNoises = SoundEngine.PlaySound(
						StomachNoises.Muffled with { Volume = 0.25f + (0.15f * GetVisualBellySize(player)) },
						player.TrueCenter()
					);
					SoundEngine.TryGetActiveSound(player.AsPred().ActiveStomachNoises, out stomachNoises);
				}

				if (stomachNoises is null)
					return;

				stomachNoises.Position = player.TrueCenter();
				stomachNoises.Volume = 0.25f;
				stomachNoises.Volume += 0.15f * GetVisualBellySize(player);
			}
		}

		public override void PostUpdateMiscEffects()
		{
			while (specialHealthRegenCount >= 60.0)
			{
				specialHealthRegenCount -= 60.0;
				Player.statLife += 1;
				if (Player.statLife > Player.statLifeMax2)
					Player.statLife = Player.statLifeMax2;
			}
			while (specialManaRegenCount >= 60.0)
			{
				specialManaRegenCount -= 60.0;
				Player.statMana += 1;
				if (Player.statMana > Player.statManaMax2)
					Player.statMana = Player.statManaMax2;
			}
		}

		public override void PostUpdateRunSpeeds()
		{
			if (!Player.mount.Active)
			{
				float weightMovementMult = (float)Math.Min(1.0, 1.0 / (Player.AsPred().StomachWeight + 1.0));
				Player.maxRunSpeed *= weightMovementMult;
				Player.accRunSpeed *= weightMovementMult;
				Player.runAcceleration *= weightMovementMult;
				Player.jumpSpeed *= weightMovementMult;
				Player.jumpHeight = (int)Math.Round((float)Player.jumpHeight * weightMovementMult);
				Player.gravity /= (2f + weightMovementMult) / 3f;
				Player.maxFallSpeed /= weightMovementMult;
			}
		}

		/// <summary>
		/// Calculates the current weight of the given predator's stomach, based on all the prey inside of it.<br/>
		/// Used primarily in conjunction with <see cref="StomachCapacity"/> to safeguard against overeating.<br/>
		/// </summary>
		/// <param name="pred">
		/// The predatory player whose stomach is to be weighed.<br/>
		/// </param>
		/// <param name="onlyKicky">
		/// If set to <see langword="true"/>, only counts out the weight of prey that is still alive (not in second digestion phase).<br/>
		/// Defaults to false.<br/>
		/// </param>
		/// <returns>
		/// The current total weight of the given predator player's stomach.<br/>
		/// </returns>
		public static double GetCurrentBellyWeight(Player pred, bool onlyKicky = false)
		{
			double totalBellyWeight = 0.0;
			if (pred.AsPred().stomachContents is not null && pred.AsPred().stomachContents.Count > 0)
			{
				foreach (Prey prey in pred.AsPred().stomachContents)
				{
					if (prey.Dead && onlyKicky)
						continue;

					totalBellyWeight += prey.WeightLeftToDigest;
					if (prey.Dead)
						continue;

					switch (prey.Type)
					{
						case PreyType.Player:
							Player preyPredPlayer = prey.Instance as Player;
							totalBellyWeight += GetCurrentBellyWeight(preyPredPlayer);
							break;
						case PreyType.NPC:
							NPC preyPredNPC = prey.Instance as NPC;
							totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
							break;
					}
				}
			}
			return totalBellyWeight;
		}

		public static bool AnyPreyStillAlive(Player pred)
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

		public static string GetDigestedPlayerDeathReason(Player player, Player prey)
		{
			if (player.whoAmI == prey.whoAmI)
			{
				return Language.GetTextValueWith(
					"Mods.V2.Death.DigestedPlayer.Paradox",
					new { Player = prey.name }
				);
			}
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
			};
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathMessageKeyList.AddRange(new List<string>
				{
					"Mods.V2.Death.DigestedPlayer.Hardcore.1",
					"Mods.V2.Death.DigestedPlayer.Hardcore.2",
					"Mods.V2.Death.DigestedPlayer.Hardcore.3",
					"Mods.V2.Death.DigestedPlayer.Hardcore.4",
				});
			}
			string finalDeathReasonKey = Main.rand.NextFromCollection(deathMessageKeyList);

			return Language.GetTextValueWith(
				finalDeathReasonKey,
				new
				{
					Player = prey.name,
					Pred = player.name
				}
			);
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			if (Player.AsFood().IsCurrentlyEaten)
			{
				foreach (Prey prey in Player.AsPred().stomachContents)
				{
					Entity betterPred = Player.AsFood().CurrentCaptor.Value.Predator;
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

			Player.AsPred().stomachContents.Clear();
		}

		public override void UpdateDead()
		{
			Player.AsPred().stomachContents.Clear();
		}

		public override void OnRespawn()
		{
			if (Player.SpawnX != -1 && Main.rand.NextBool(7, 1000000))
			{
				Swallow(Player, Player);
			}
		}

		public static void CountDigestionKillForBannersAndDropThem(Player player, NPC npc)
		{
			int num = Item.NPCtoBanner(npc.BannerID());
			if (num <= 0 || npc.ExcludedFromDeathTally())
				return;

			NPC.killCount[num]++;
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.NPCKillCountDeathTally, -1, -1, null, num);

			int num2 = ItemID.Sets.KillsToBanner[Item.BannerToItem(num)];
			if (NPC.killCount[num] % num2 == 0 && num > 0)
			{
				int npcID = Item.BannerToNPC(num);
				int num4 = npc.lastInteraction;
				if (!Main.player[num4].active || Main.player[num4].dead)
					num4 = npc.FindClosestPlayer();

				NetworkText networkText = NetworkText.FromLiteral(Language.GetTextValueWith("Mods.V2.Death.DigestedEnemiesAnnouncement", new
				{
					Pred = player.name,
					Number = NPC.killCount[num],
					Prey = NetworkText.FromKey(Lang.GetNPCName(npcID).Key)
				}));

				if (Main.netMode == NetmodeID.SinglePlayer)
					Main.NewText(networkText.ToString(), 250, 250, 0);
				else if (Main.netMode == NetmodeID.Server)
					ChatHelper.BroadcastChatMessage(networkText, new Color(250, 250, 0));

				int num5 = Item.BannerToItem(num);
				Vector2 vector = npc.position;
				if (num4 >= 0 && num4 < 255)
					vector = Main.player[num4].position;

				Item.NewItem(npc.GetSource_Loot(), (int)vector.X, (int)vector.Y, npc.width, npc.height, num5);
			}
		}

		public override void SaveData(TagCompound tag)
		{
			foreach (KeyValuePair<string, int> keyValuePair in Player.AsPred().mealCount)
			{
				tag.Add("[DIGESTED] " + keyValuePair.Key, keyValuePair.Value);
			}
		}

		public override void LoadData(TagCompound tag)
		{
			mealCount = new Dictionary<string, int>();
			foreach (KeyValuePair<string, object> keyValuePair in tag)
			{
				if (!keyValuePair.Key.StartsWith("[DIGESTED] "))
					continue;

				string realKey = keyValuePair.Key.Remove(0, 11);
				mealCount.Add(realKey, (int)keyValuePair.Value);
			}
		}


		public override void CopyClientState(ModPlayer clientClone)/* tModPorter Suggestion: Replace Item.Clone usages with Item.CopyNetStateTo */
		{
			PredPlayer predClientClone = clientClone as PredPlayer;
			predClientClone.stomachContents = Player.AsPred().stomachContents;
			predClientClone.GLP.Base = Player.AsPred().GLP.Base;
			predClientClone.GLP.Extra = Player.AsPred().GLP.Extra;
			predClientClone.ACI.Base = Player.AsPred().ACI.Base;
			predClientClone.ACI.Extra = Player.AsPred().ACI.Extra;
			predClientClone.TUM.Base = Player.AsPred().TUM.Base;
			predClientClone.TUM.Extra = Player.AsPred().TUM.Extra;
			predClientClone.ABS.Base = Player.AsPred().ABS.Base;
			predClientClone.ABS.Extra = Player.AsPred().ABS.Extra;
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			PredPlayer predClientClone = clientPlayer as PredPlayer;

			foreach (Prey prey in Player.AsPred().stomachContents)
			{
				if (predClientClone.stomachContents.IndexOf(prey) != Player.AsPred().stomachContents.IndexOf(prey))
					SyncPlayer(-1, Main.myPlayer, false);
			}

			if (predClientClone.GLP.Base != Player.AsPred().GLP.Base || predClientClone.GLP.Extra != Player.AsPred().GLP.Extra)
				SyncPlayer(-1, Main.myPlayer, false);
			if (predClientClone.ACI.Base != Player.AsPred().ACI.Base || predClientClone.ACI.Extra != Player.AsPred().ACI.Extra)
				SyncPlayer(-1, Main.myPlayer, false);
			if (predClientClone.TUM.Base != Player.AsPred().TUM.Base || predClientClone.TUM.Extra != Player.AsPred().TUM.Extra)
				SyncPlayer(-1, Main.myPlayer, false);
			if (predClientClone.ABS.Base != Player.AsPred().ABS.Base || predClientClone.ABS.Extra != Player.AsPred().ABS.Extra)
				SyncPlayer(-1, Main.myPlayer, false);
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			ModPacket tumPacket = Mod.GetPacket();
			tumPacket.Write((byte)V2.MessageType.PredPlayerSync);
			tumPacket.Write((byte)Player.whoAmI);
			tumPacket.Write(Player.AsPred().stomachContents.Count);
			foreach (Prey prey in Player.AsPred().stomachContents)
			{
				tumPacket.Write(prey.Type switch
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
				tumPacket.Write(prey.Instance.whoAmI);
				tumPacket.Write(prey.Dead);
				tumPacket.Write(prey.WeightLeftToDigest);
			}
			tumPacket.Write(Player.AsPred().GLP.Base);
			tumPacket.Write(Player.AsPred().GLP.Extra);
			tumPacket.Write(Player.AsPred().ACI.Base);
			tumPacket.Write(Player.AsPred().ACI.Extra);
			tumPacket.Write(Player.AsPred().TUM.Base);
			tumPacket.Write(Player.AsPred().TUM.Extra);
			tumPacket.Write(Player.AsPred().ABS.Base);
			tumPacket.Write(Player.AsPred().ABS.Extra);
			tumPacket.Send(toWho, fromWho);
		}

		public void ReceivePlayerSync(BinaryReader binaryReader)
		{
			Player.AsPred().stomachContents = new List<Prey>();

			int gutCount = binaryReader.ReadInt32();
			if (gutCount <= 0)
				return;

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
				Player.AsPred().stomachContents.Add(prey);
			}
			Player.AsPred().GLP.Base = binaryReader.ReadInt32();
			Player.AsPred().GLP.Extra = binaryReader.ReadInt32();
			Player.AsPred().ACI.Base = binaryReader.ReadInt32();
			Player.AsPred().ACI.Extra = binaryReader.ReadInt32();
			Player.AsPred().TUM.Base = binaryReader.ReadInt32();
			Player.AsPred().TUM.Extra = binaryReader.ReadInt32();
			Player.AsPred().ABS.Base = binaryReader.ReadInt32();
			Player.AsPred().ABS.Extra = binaryReader.ReadInt32();
		}

		public static int GetVisualBellySize(Player player)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(GetCurrentBellyWeight(player))),
				7
			);
		}
	}

	public class VoreTum : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Torso);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player player = drawInfo.drawPlayer;
			int tumSize = PredPlayer.GetVisualBellySize(player);

			void DrawTmuuy(ref PlayerDrawSet drawInfo, int size, int offsetX = 0, int offsetY = 0)
			{
				Texture2D tum = ModContent.Request<Texture2D>("V2/PlayerHandling/TumSprites/Bare_" + size, AssetRequestMode.ImmediateLoad).Value;
				if (player.IsAirborne())
					tum = ModContent.Request<Texture2D>("V2/PlayerHandling/TumSprites/Bare_" + size + "_Airborne", AssetRequestMode.ImmediateLoad).Value;
				Vector2 tumLocation =
					new Vector2(
					(int)(
						drawInfo.Position.X
					  - Main.screenPosition.X
					  - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2)
					  + (float)(drawInfo.drawPlayer.width / 2)
					),
						(int)(
							drawInfo.Position.Y
						  - Main.screenPosition.Y
						  + (float)drawInfo.drawPlayer.height
						  - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f
						)
					)
				  + drawInfo.drawPlayer.bodyPosition
				  + new Vector2(
					drawInfo.drawPlayer.bodyFrame.Width / 2,
					drawInfo.drawPlayer.bodyFrame.Height / 2
				);
				tumLocation.Y += drawInfo.torsoOffset;
				tumLocation.X += offsetX;
				tumLocation.Y += offsetY;
				if (player.direction == -1)
					tumLocation.X -= (float)tum.Width + (offsetX * 2);
				DrawData tumDraw = new DrawData(
					tum,
					tumLocation,
					tum.Bounds,
					drawInfo.colorBodySkin,
					player.bodyRotation,
					Vector2.Zero,
					1f,
					drawInfo.playerEffect,
					0
				);
				tumDraw.shader = 0;
				drawInfo.DrawDataCache.Add(tumDraw);
			}

			switch (tumSize)
			{
				case 0:
				default:
					//default:
					// do absolutely nothing lol
					break;
				case 1:
					DrawTmuuy(ref drawInfo, 1, 0, 6);
					break;
				case 2:
					DrawTmuuy(ref drawInfo, 2, -2, 6);
					break;
				case 3:
					DrawTmuuy(ref drawInfo, 3, player.IsAirborne() ? -4 : -2, 6);
					break;
				case 4:
					DrawTmuuy(ref drawInfo, 4, -4, 2);
					break;
				case 5:
					DrawTmuuy(ref drawInfo, 5, -4, 0);
					break;
				case 6:
					DrawTmuuy(ref drawInfo, 6, -4, -2);
					break;
				case 7:
					DrawTmuuy(ref drawInfo, 7, -6, -4);
					break;
			}
		}
	}
}

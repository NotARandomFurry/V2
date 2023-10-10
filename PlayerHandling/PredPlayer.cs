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
using V2.PlayerHandling.PredPlayerGoals;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Starter;
using V2.Sounds.Vore;
using V2.StatusEffects.Debuffs;

namespace V2.PlayerHandling
{
	public class PredStat
	{
		public int Spent { get; set; }
		public int Base { get; set; }
		public int Extra { get; set; }
		public int Total => Spent + Base + Extra;

		public PredStat()
		{
			Spent = 0;
			Base = 0;
			Extra = 0;
		}

		public void Reset()
		{
			Spent = 0;
			Base = 0;
			Extra = 0;
		}
	}
	public class PredPlayer : ModPlayer
	{
		public bool ForceV2DataSync { get; set; }
		public List<VoreTracker> stomachContents;
		public List<VoreTracker> stomachContentsQueue;

		public double Stomachache;

		public int predLevel;

		public bool InPredStatsMenu { get; set; }
		public Dictionary<string, bool> GoalsCompleted { get; set; }
		public int TotalStatPoints { get; set; }
		public int AllocatedStatPoints => GLP.Spent + TUM.Spent + ACI.Spent + ABS.Spent;
		public int AvailableStatPoints => TotalStatPoints - AllocatedStatPoints;
		public PredStat GLP { get; set; }
		public StatModifier SwallowSizeModifier;
		public static double BaseSwallowSize => 0.4;
		public static double SwallowSizePerLevel => 0.08;
		public double SwallowSize
		{
			get
			{
				double baseSwallowSize = BaseSwallowSize;
				baseSwallowSize += SwallowSizePerLevel * GLP.Total;
				return SwallowSizeModifier.ApplyTo((float)baseSwallowSize);
			}
		}
		public static int BaseLiquidSwallowSize => 5;
		public static int LiquidSwallowSizePer5Levels => 1;
		public StatModifier LiquidSwallowSizeModifier;
		public int LiquidSwallowSize
		{
			get
			{
				int baseLiquidSwallowSize = BaseLiquidSwallowSize;
				baseLiquidSwallowSize += LiquidSwallowSizePer5Levels * (int)Math.Floor(GLP.Total / 5.0);
				return (int)Math.Round(LiquidSwallowSizeModifier.ApplyTo((float)baseLiquidSwallowSize));
			}
		}
		public double EffectiveLiquidSwallowSize(int liquidType)
		{
			double effectiveBaseLiquidSwallowSize = (double)LiquidSwallowSize / 255.0;
			return liquidType switch
			{
				LiquidID.Lava => effectiveBaseLiquidSwallowSize * 4.0,
				LiquidID.Honey => effectiveBaseLiquidSwallowSize * 1.5,
				LiquidID.Shimmer => effectiveBaseLiquidSwallowSize * 0.75,
				_ => effectiveBaseLiquidSwallowSize,
			};
		}
		public StatModifier StruggleGraceTimeModifier;
		public static double BaseStruggleGraceTime => 0.8;
		public static double StruggleGraceTimePer5Levels => 0.1;
		public double StruggleGraceTime
		{
			get
			{
				double baseGracePeriod = BaseStruggleGraceTime;
				baseGracePeriod += StruggleGraceTimePer5Levels * Math.Floor(GLP.Total / 5.0);
				return StruggleGraceTimeModifier.ApplyTo((float)baseGracePeriod);
			}
		}
		public string StruggleGraceTimeReadable
		{
			get
			{
				double seconds = StruggleGraceTime.CastToDecimalPlaces(2);
				int hours = 0;
				int minutes = 0;
				while (seconds > 3600.0)
				{
					hours += 1;
					seconds -= 60.0;
				}
				while (seconds > 60.0)
				{
					minutes += 1;
					seconds -= 60.0;
				}

				string readableTime = seconds + "sec";
				if (minutes > 0)
					readableTime = minutes + "min" + readableTime;
				if (hours > 0)
					readableTime = hours + "hr" + readableTime;

				return readableTime;
			}
		}
		public PredStat TUM { get; set; }
		public StatModifier StomachCapacityModifier;
		public static double BaseStomachCapacity => 0.80;
		public static double StomachCapacityPerLevel => 0.04;
		public double StomachCapacity
		{
			get
			{
				double baseStomachCapacity = BaseStomachCapacity;
				baseStomachCapacity += StomachCapacityPerLevel * TUM.Total;
				return StomachCapacityModifier.ApplyTo((float)baseStomachCapacity);
			}
		}
		public StatModifier StomachacheMeterCapacityModifier;
		public static double BaseStomachacheMeterCapacity => 0.80;
		public static double StomachacheMeterCapacityPer5Levels => 10.0;
		public double StomachacheMeterCapacity
		{
			get
			{
				double baseStomachacheMeterCapacity = BaseStomachacheMeterCapacity;
				baseStomachacheMeterCapacity += StomachacheMeterCapacityPer5Levels * Math.Floor(TUM.Total / 5.0);
				return StomachacheMeterCapacityModifier.ApplyTo((float)baseStomachacheMeterCapacity);
			}
		}
		public PredStat ACI { get; set; }
		public StatModifier DigestionTickDamageModifier;
		public static double BaseDigestionTickDamage => 6.0;
		public static double DigestionTickDamagePerLevel => 0.75;
		public double DigestionTickDamage
		{
			get
			{
				double baseDigestionDamage = BaseDigestionTickDamage;
				baseDigestionDamage += DigestionTickDamagePerLevel * ACI.Total;
				return DigestionTickDamageModifier.ApplyTo((float)baseDigestionDamage);
			}
		}
		public StatModifier DigestionTickRateModifier;
		public static double BaseDigestionTickRate => 1.0;
		public static double DigestionTickRatePer5Levels => 0.1;
		public double DigestionTickRate
		{
			get
			{
				double baseDigestionRate = BaseDigestionTickRate;
				baseDigestionRate += DigestionTickRatePer5Levels * Math.Floor(ACI.Total / 5.0);
				return DigestionTickRateModifier.ApplyTo((float)baseDigestionRate);
			}
		}
		public PredStat ABS { get; set; }
		public StatModifier PreyAbsorptionRateModifier;
		public static double BasePreyAbsorptionRate => 0.2;
		public static double PreyAbsorptionRatePerLevel => 0.02;
		public double PreyAbsorptionRate
		{
			get
			{
				double basePreyAbsorptionRate = BasePreyAbsorptionRate;
				basePreyAbsorptionRate += PreyAbsorptionRatePerLevel * ABS.Total;
				return PreyAbsorptionRateModifier.ApplyTo((float)basePreyAbsorptionRate);
			}
		}
		public double PreyAbsorptionRatePerTick => PreyAbsorptionRate / (double)V2Utils.SensibleTime(minutes: 1);
		public StatModifier BuffExtensionTimeModifier;
		public static double BuffExtensionTimePer5Levels => 0.04;
		public double BuffExtensionTime
		{
			get
			{
				double baseBuffExtensionTime = BuffExtensionTimePer5Levels * Math.Floor(ABS.Total / 5.0);
				return BuffExtensionTimeModifier.ApplyTo((float)baseBuffExtensionTime);
			}
		}
		public StatModifier DebuffDisextensionTimeModifier;
		public static double DebuffDisextensionTimePer5Levels => 0.02;
		public double DebuffDisextensionTime
		{
			get
			{
				double baseDebuffDisextensionTime = DebuffDisextensionTimePer5Levels * Math.Floor(ABS.Total / 5.0);
				return DebuffDisextensionTimeModifier.ApplyTo((float)baseDebuffDisextensionTime);
			}
		}

		public SoundStyle SmallBurps { get; set; }
		public SoundStyle StandardBurps { get; set; }
		public SoundStyle BigBurps { get; set; }

		public SoundStyle SmallGulps { get; set; }
		public SoundStyle BigGulps { get; set; }

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
		public bool lastSwallowWasDrinking;
		public string lastLiquidDrank;
		public string lastLiquidDrankMod;
		public Dictionary<string, int> drinkCount;
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

		public bool CanDrinkLavaSafe
		{
			get
			{
				if (Player.lavaImmune)
					return true;

				return false;
			}
		}
		public bool MoltenTummy => Player.HasBuff(ModContent.BuffType<MoltenStomach>());

		public bool CanDrinkShimmerSafe
		{
			get
			{
				for (int i = 3; i < 10; i++)
				{
					if (!Player.armor[i].IsAir && Player.armor[i].type == ItemID.ShimmerCloak)
						return true;
				}

				return false;
			}
		}
		public bool PrimedForShimmerStomachDeath { get; set; }
		public bool ShimmeringTummy
		{
			get => Player.HasBuff(ModContent.BuffType<ShimmeringStomach>());
			set
			{
				if (value)
				{
					if (!Player.HasBuff(ModContent.BuffType<ShimmeringStomach>()))
						Player.AddBuff(ModContent.BuffType<ShimmeringStomach>(), V2Utils.SensibleTime(seconds: 5));
				}
				else
				{
					if (!Player.HasBuff(ModContent.BuffType<ShimmeringStomach>()))
						Player.ClearBuff(ModContent.BuffType<ShimmeringStomach>());
				}
			}
		}

		public double StomachWeightAtSleepStart;
		public int OverfullTime;

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
					foreach (VoreTracker prey in stomachContents)
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
								totalBellyWeight += preyPredNPC.AsPred().ExtraWeight;
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
					foreach (VoreTracker prey in stomachContents)
					{
						if (prey.NoHealth)
							continue;

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
					foreach (VoreTracker prey in stomachContents)
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
					foreach (VoreTracker prey in stomachContents)
					{
						if (prey.NoHealth)
							continue;

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
			stomachContents = new List<VoreTracker>();
			stomachContentsQueue = new List<VoreTracker>();

			SmallBurps = Burps.Humanoid.Small;
			StandardBurps = Burps.Humanoid.Standard;

			SmallGulps = Gulps.Short;
			BigGulps = Gulps.Standard;

			GLP = new PredStat();
			ACI = new PredStat();
			TUM = new PredStat();
			ABS = new PredStat();

			endoToggleUnlocked = false;
			endoToggle = false;

			lastEntitySwallowed = null;
			lastEntitySwallowedMod = null;
			mealCount = new Dictionary<string, int>();
			lastSwallowWasDrinking = false;
			lastLiquidDrank = null;
			lastLiquidDrankMod = null;
			drinkCount = new Dictionary<string, int>();

			PrimedForShimmerStomachDeath = false;

			GoalsCompleted = new Dictionary<string, bool>();
			foreach (PredPlayerGoal goal in PredPlayerGoalLoader.PredPlayerGoals)
			{
				GoalsCompleted.Add(goal.InternalName, false);
			}

			InPredStatsMenu = false;

			StomachWeightAtSleepStart = 0.0;
			OverfullTime = 0;
		}

		public override void ResetEffects()
		{
			while (Player.AsPred().stomachContentsQueue is not null && Player.AsPred().stomachContentsQueue.Count > 0)
			{
				Player.AsPred().stomachContents.Add(Player.AsPred().stomachContentsQueue.First());
				Player.AsPred().stomachContentsQueue.Remove(Player.AsPred().stomachContentsQueue.First());
			}

			ForceV2DataSync = false;

			GLP.Base = 0;
			GLP.Extra = 0;
			SwallowSizeModifier = StatModifier.Default;
			LiquidSwallowSizeModifier = StatModifier.Default;
			StruggleGraceTimeModifier = StatModifier.Default;
			TUM.Base = 0;
			TUM.Extra = 0;
			StomachCapacityModifier = StatModifier.Default;
			StomachacheMeterCapacityModifier = StatModifier.Default;
			ACI.Base = 0;
			ACI.Extra = 0;
			DigestionTickDamageModifier = StatModifier.Default;
			DigestionTickRateModifier = StatModifier.Default;
			ABS.Base = 0;
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
						ModContent.GetInstance<FirstItemEaten>().TrySetCompletion(Player);
					}
				}
			}
			return false;
		}

		public override void UpdateBadLifeRegen()
		{
			if (Player.AsPred().MoltenTummy)
			{
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				Player.lifeRegen -= 75;
				Player.lifeRegenTime = 0;
			}
		}

		public override void PostUpdateMiscEffects()
		{
			if (Player.sleeping.FullyFallenAsleep)
			{
				Player.AsPred().DigestionTickRateModifier += 0.25f;
				Player.AsPred().PreyAbsorptionRateModifier += 0.25f;
				bool isEveryoneAsleep = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
				if (isEveryoneAsleep)
				{
					Player.AsPred().DigestionTickRateModifier *= (float)Main.dayRate;
					Player.AsPred().PreyAbsorptionRateModifier *= (float)Main.dayRate;
				}
			}
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

		public override void PostItemCheck()
		{
			if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer && !Player.AsPred().BlockSwallowAttempts)
			{
				#region Swallowing nearby prey
				if (V2.SwallowHotkey.JustPressed && !(Main.playerInventory && V2.ItemGulpHotkey.Current))
				{
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
				#endregion
				#region Drinking liquids
				bool inAnyLiquid = Player.wet || Player.lavaWet || Player.honeyWet || Player.shimmerWet;
				if (V2.SwallowHotkey.Current && inAnyLiquid && Main.GameUpdateCount % 4 == 0)
				{
					Point playerTileLocation = (Player.Center + new Vector2(0, -10)).ToTileCoordinates();
					Tile tile = Main.tile[playerTileLocation];
					if (tile.LiquidAmount > 0 && Player.AsPred().StomachCapacity - GetCurrentBellyWeight(Player) >= Player.AsPred().EffectiveLiquidSwallowSize(tile.LiquidType))
					{
						int liquidToDrink = (tile.LiquidAmount > Player.AsPred().LiquidSwallowSize) ? Player.AsPred().LiquidSwallowSize : tile.LiquidAmount;
						Player.AsPred().stomachContents.Add(new VoreTracker(
							tile.LiquidType,
							liquidToDrink
						));

						Player.AsPred().lastLiquidDrank = tile.LiquidType switch
						{
							0 => "Water",
							1 => "Lava",
							2 => "Honey",
							3 => "Shimmer",
							_ => "Some other liquid",
						};

						void AddVanillaDrinkCount()
						{
							Player.AsPred().lastLiquidDrankMod = "Terraria";
							if (!Player.AsPred().drinkCount.ContainsKey(Player.AsPred().lastLiquidDrankMod + ": " + Player.AsPred().lastLiquidDrank))
								Player.AsPred().drinkCount.Add(Player.AsPred().lastLiquidDrankMod + ": " + Player.AsPred().lastLiquidDrank, 0);
							Player.AsPred().drinkCount[Player.AsPred().lastLiquidDrankMod + ": " + Player.AsPred().lastLiquidDrank] += liquidToDrink;
							Player.AsPred().lastSwallowWasDrinking = true;
						}
						bool VanillaDrinkCountHas(int req) => Player.AsPred().drinkCount[Player.AsPred().lastLiquidDrankMod + ": " + Player.AsPred().lastLiquidDrank] >= req;
						switch (tile.LiquidType)
						{
							case LiquidID.Water:
								AddVanillaDrinkCount();
								if (VanillaDrinkCountHas(255))
									ModContent.GetInstance<FirstDrink>().TrySetCompletion(Player);
								break;
							case LiquidID.Lava:
								if (Player.AsPred().CanDrinkLavaSafe)
								{
									AddVanillaDrinkCount();
								//	if (VanillaDrinkCountHas(255))
								//		ModContent.GetInstance<FirstDrink>().TrySetCompletion(Player);
								}
								break;
							case LiquidID.Honey:
								AddVanillaDrinkCount();
								if (VanillaDrinkCountHas(255))
									ModContent.GetInstance<DrinkHoney>().TrySetCompletion(Player);
								break;
							case LiquidID.Shimmer:
								if (!Player.AsPred().CanDrinkShimmerSafe && !Player.AsPred().PrimedForShimmerStomachDeath)
								{
									Player.AddBuff(ModContent.BuffType<ShimmeringStomach>(), 300);
									Player.AsPred().PrimedForShimmerStomachDeath = true;
								}
								else if (Player.AsPred().CanDrinkShimmerSafe)
								{
									AddVanillaDrinkCount();
								//	if (VanillaDrinkCountHas(255))
								//		ModContent.GetInstance<FirstDrink>().TrySetCompletion(Player);
								}
								break;
						}
						if (tile.LiquidAmount <= (byte)Player.AsPred().LiquidSwallowSize)
						{
							tile.LiquidAmount = 0;
							tile.LiquidType = 0;
						}
						else
							tile.LiquidAmount -= (byte)Player.AsPred().LiquidSwallowSize;

						WorldGen.SquareTileFrame(playerTileLocation.X, playerTileLocation.Y);

						SoundEngine.PlaySound(
							Player.AsPred().SmallGulps with { Volume = 0.45f, Pitch = 0.25f },
							Player.position + new Vector2(0f, -10f)
						);
						Player.AsPred().ForceV2DataSync = true;
					}
				}
				#endregion
				#region Regurgitating swallowed prey
				if (V2.RegurgitateHotkey.JustPressed && Player.AsPred().stomachContents.Count > 0)
				{
					VoreTracker prey = Player.AsPred().stomachContents.FindLast(x => !x.NoHealth && x.Type != PreyType.Liquid);
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
						Player.AsPred().ForceV2DataSync = true;
						SoundEngine.PlaySound(
							prey.WeightLeftToDigest <= 0.3 ? Player.AsPred().SmallBurps : Player.AsPred().StandardBurps,
							Player.TrueCenter() + new Vector2(Player.direction * 8f, -14f)
						);
					}
				}
				#endregion
			}

			UpdatePrey(Player);
			UpdateGeneralPredGoalsLogic(Player);
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

			if (VoreTracker.GetInitialPreySize(prey) > pred.AsPred().StomachCapacity - GetCurrentBellyWeight(pred))
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
					pred.AsPred().stomachContents = new List<VoreTracker>();

				VoreTracker food = new VoreTracker(prey);
				pred.AsPred().stomachContents.Add(food);
				if (prey is not NPC preyNPC || preyNPC.realLife == -1)
				{
					SoundEngine.PlaySound(
						food.WeightLeftToDigest <= 0.3
						? pred.AsPred().SmallGulps
						: pred.AsPred().BigGulps,
						pred.Center
					);
				}
				pred.AsPred().lastSwallowWasDrinking = false;
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

			int contentsCountPrePruning = player.AsPred().stomachContents.Count;
			player.AsPred().stomachContents.RemoveAll(x => x.NoHealth && x.WeightLeftToDigest <= 0);
			if (contentsCountPrePruning != player.AsPred().stomachContents.Count)
				player.AsPred().ForceV2DataSync = true;
			if (player.AsPred().stomachContents.Count <= 0)
				return;

			foreach (VoreTracker prey in player.AsPred().stomachContents)
			{
				prey.timeSpentInStomach++;

				switch (prey.Type)
				{
					case PreyType.Item:
						Item preyItem = prey.Instance as Item;
						if (!preyItem.IsAir)
							preyItem.AsFood().UpdateInStomach?.Invoke(preyItem, player, prey.NoHealth);
						break;
				}

				if (!prey.NoHealth)
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
									prey.NoHealth = preyPlayer.AsFood().TakeDigestionDamage(player, digestionDamage);
									if (prey.NoHealth)
									{
										if (!player.AsPred().mealCount.ContainsKey("Terraria: Player"))
											player.AsPred().mealCount.Add("Terraria: Player", 0);
										player.AsPred().mealCount["Terraria: Player"] += 1;
										SoundEngine.PlaySound(
											player.AsPred().StandardBurps,
											player.TrueCenter() + new Vector2(player.direction * 8f, -14f)
										);
										player.AsPred().ForceV2DataSync = true;
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
									prey.NoHealth = PreyNPC.TakeDigestionDamage(preyNPC, player, digestionDamage);
									if (prey.NoHealth)
									{
										prey.Instance = null;
										string preyNPCMod = preyNPC.ModNPC != null ? preyNPC.ModNPC.Mod.DisplayName : "Terraria";
										if (!player.AsPred().mealCount.ContainsKey(preyNPCMod + ": " + preyNPC.TypeName))
											player.AsPred().mealCount.Add(preyNPCMod + ": " + preyNPC.TypeName, 0);
										player.AsPred().mealCount[preyNPCMod + ": " + preyNPC.TypeName] += 1;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? player.AsPred().SmallBurps : player.AsPred().StandardBurps,
											player.TrueCenter() + new Vector2(player.direction * 8f, -14f)
										);
										player.AsPred().ForceV2DataSync = true;
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
									prey.NoHealth = preyItem.TakeDigestionDamage(player, digestionDamage);
									if (prey.NoHealth)
									{
										string preyItemMod = preyItem.ModItem != null ? preyItem.ModItem.Mod.DisplayName : "Terraria";
										if (!player.AsPred().mealCount.ContainsKey(preyItemMod + ": " + preyItem.Name))
											player.AsPred().mealCount.Add(preyItemMod + ": " + preyItem.Name, 0);
										player.AsPred().mealCount[preyItemMod + ": " + preyItem.Name] += preyItem.stack;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? player.AsPred().SmallBurps : player.AsPred().StandardBurps,
											player.TrueCenter() + new Vector2(player.direction * 8f, -14f)
										);
										player.AsPred().ForceV2DataSync = true;
									}
								}
								break;
						}
					}
				}
				else
				{
					double absorptionRate = player.AsPred().PreyAbsorptionRatePerTick / (double)player.AsPred().stomachContents.Count;
					if (player.sleeping.FullyFallenAsleep)
					{
						absorptionRate *= 1.1;
						bool everybodyIsSleepingOffAMeal = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
						if (everybodyIsSleepingOffAMeal)
							absorptionRate *= Main.dayRate;
					}
					prey.WeightLeftToDigest -= absorptionRate;
					if (prey.WeightLeftToDigest < 0)
						prey.WeightLeftToDigest = 0;

					switch (prey.Type)
					{
						case PreyType.Item:
							Item preyItem = prey.Instance as Item;
							if (!preyItem.IsAir && prey.WeightLeftToDigest == 0)
								preyItem.AsFood().FullyDigested = true;
							break;
						case PreyType.Liquid:
							switch (prey.ExactType)
							{
								case "Water":
									break;
								case "Lava":
									if (!player.AsPred().CanDrinkLavaSafe)
									{
										player.AddBuff(ModContent.BuffType<MoltenStomach>(), 3);
									}
									break;
								case "Honey":
									break;
								case "Shimmer":
									if (!player.AsPred().CanDrinkShimmerSafe)
									{
										if (!player.AsPred().PrimedForShimmerStomachDeath)
										{
											player.AsPred().PrimedForShimmerStomachDeath = true;
											player.AddBuff(ModContent.BuffType<ShimmeringStomach>(), 300);
										}
										else if (!player.AsPred().ShimmeringTummy)
										{
											player.AsPred().PrimedForShimmerStomachDeath = false;
											player.KillMe(
												PlayerDeathReason.ByCustomReason(
													Language.GetTextValueWith(
														Main.rand.NextFromCollection(new List<string>
														{
															"Mods.V2.Death.OverlyHungryPlayer.UnsafeShimmerDrink.1",
															"Mods.V2.Death.OverlyHungryPlayer.UnsafeShimmerDrink.2",
															"Mods.V2.Death.OverlyHungryPlayer.UnsafeShimmerDrink.3",
														}),
														new
														{
															Player = player.name
														}
													)
												),
												9999,
												0
											);
										}
									}
									break;
							}
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

		public static void UpdateGeneralPredGoalsLogic(Player pred)
		{
			if (pred.sleeping.FullyFallenAsleep)
			{
				if (pred.AsPred().StomachWeight == 0.0 && pred.AsPred().StomachWeightAtSleepStart > 0.0 && pred.AsPred().StomachWeightAtSleepStart >= SleepSpeedsDigestion.FlatFullnessThreshold)
					ModContent.GetInstance<SleepSpeedsDigestion>().TrySetCompletion(pred);
			}
			else
				pred.AsPred().StomachWeightAtSleepStart = 0.0;

			if (pred.AsPred().StomachFullness / pred.AsPred().StomachCapacity > TooFull.TimeThreshold)
			{
				pred.AsPred().OverfullTime += 1;
				if (pred.AsPred().OverfullTime >= TooFull.TimeThreshold)
					ModContent.GetInstance<TooFull>().TrySetCompletion(pred);
			}
			else
				pred.AsPred().OverfullTime = 0;
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
				foreach (VoreTracker prey in pred.AsPred().stomachContents)
				{
					if (prey.NoHealth && onlyKicky)
						continue;

					totalBellyWeight += prey.WeightLeftToDigest;
					if (prey.NoHealth)
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
				foreach (VoreTracker prey in pred.AsPred().stomachContents)
				{
					if (!prey.NoHealth)
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

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (Player.AsFood().IsCurrentlyEaten)
			{

			}
			else
			{
				if (Player.AsPred().MoltenTummy)
				{
					damageSource = PlayerDeathReason.ByCustomReason(
						Language.GetTextValueWith(
							Main.rand.NextFromCollection(new List<string>
							{
								"Mods.V2.Death.OverlyHungryPlayer.UnsafeLavaDrink.1",
								"Mods.V2.Death.OverlyHungryPlayer.UnsafeLavaDrink.2",
								"Mods.V2.Death.OverlyHungryPlayer.UnsafeLavaDrink.3",
							}),
							new
							{
								Player = Player.name
							}
						)
					);
				}
			}
			return true;
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			if (Player.AsFood().IsCurrentlyEaten)
			{
				foreach (VoreTracker prey in Player.AsPred().stomachContents)
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
			Player.AsPred().ForceV2DataSync = true;
		}

		public override void UpdateDead()
		{
			Player.AsPred().stomachContents.Clear();
			Player.AsPred().PrimedForShimmerStomachDeath = false;
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
			foreach (KeyValuePair<string, int> keyValuePair in Player.AsPred().drinkCount)
			{
				tag.Add("[DRANK] " + keyValuePair.Key, keyValuePair.Value);
			}
			foreach (KeyValuePair<string, bool> keyValuePair in Player.AsPred().GoalsCompleted)
			{
				tag.Add("[GOAL] " + keyValuePair.Key, keyValuePair.Value);
			}
		}

		public override void LoadData(TagCompound tag)
		{
			mealCount = new Dictionary<string, int>();
			GoalsCompleted = new Dictionary<string, bool>();
			foreach (KeyValuePair<string, object> keyValuePair in tag)
			{
				if (keyValuePair.Key.StartsWith("[DIGESTED] "))
				{
					string realKey = keyValuePair.Key.Remove(0, 11);
					int specificMealCount = tag.GetInt(keyValuePair.Key);
					mealCount.Add(realKey, specificMealCount);
					continue;
				}
				if (keyValuePair.Key.StartsWith("[DRANK] "))
				{
					string realKey = keyValuePair.Key.Remove(0, 8);
					int specificDrinkCount = tag.GetInt(keyValuePair.Key);
					drinkCount.Add(realKey, specificDrinkCount);
					continue;
				}
				if (keyValuePair.Key.StartsWith("[GOAL] "))
				{
					string realKey = keyValuePair.Key.Remove(0, 7);
					bool completeState = tag.GetBool(keyValuePair.Key);
					GoalsCompleted.Add(realKey, completeState);
					continue;
				}
			}
		}


		public override void CopyClientState(ModPlayer clientClone)
		{
			PredPlayer predClientClone = clientClone as PredPlayer;
			predClientClone.ForceV2DataSync = ForceV2DataSync;
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			PredPlayer predClientClone = clientPlayer as PredPlayer;
			if (predClientClone.ForceV2DataSync)
				SyncPlayer(-1, Main.myPlayer, false);
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			ModPacket tumPacket = Mod.GetPacket();
			tumPacket.Write((byte)V2.MessageType.SyncPlayerPredData);
			tumPacket.Write((byte)Player.whoAmI);
			tumPacket.Write(Player.AsPred().stomachContents.Count);
			foreach (VoreTracker prey in Player.AsPred().stomachContents)
			{
				tumPacket.Write(prey.Type switch
				{
					PreyType.Player => 0,
					PreyType.NPC => 1,
					PreyType.Projectile => 2,
					PreyType.Item => 3,
					_ => throw new NotImplementedException(),
				});
				tumPacket.Write(prey.NoHealth);
				if (prey.Instance != null)
				{
					tumPacket.Write(prey.Instance.whoAmI);
				}
				else
				{
					tumPacket.Write(prey.ExactType);
					tumPacket.Write(prey.WeightLeftToDigest);
				}
			}
			tumPacket.Write(Player.AsPred().GLP.Spent);
			tumPacket.Write(Player.AsPred().GLP.Base);
			tumPacket.Write(Player.AsPred().GLP.Extra);
			tumPacket.Write(Player.AsPred().ACI.Spent);
			tumPacket.Write(Player.AsPred().ACI.Base);
			tumPacket.Write(Player.AsPred().ACI.Extra);
			tumPacket.Write(Player.AsPred().TUM.Spent);
			tumPacket.Write(Player.AsPred().TUM.Base);
			tumPacket.Write(Player.AsPred().TUM.Extra);
			tumPacket.Write(Player.AsPred().ABS.Spent);
			tumPacket.Write(Player.AsPred().ABS.Base);
			tumPacket.Write(Player.AsPred().ABS.Extra);
			tumPacket.Send(toWho, fromWho);
		}

		public void ReceivePlayerSync(BinaryReader binaryReader)
		{
			Player.AsPred().stomachContents = new List<VoreTracker>();

			int gutCount = binaryReader.ReadInt32();
			if (gutCount <= 0)
				return;

			for (int i = 0; i < gutCount; i++)
			{
				int preyType = binaryReader.ReadInt32();
				// see previous note on EntityID
				// int preyID = binaryReader.ReadInt32();
				bool preyDead = binaryReader.ReadBoolean();
				if (preyDead)
				{
					string preyExactType = binaryReader.ReadString();
					double preyWeightLeft = binaryReader.ReadDouble();
					VoreTracker deadPrey = new VoreTracker(preyType, preyExactType, preyWeightLeft);
					deadPrey.NoHealth = true;
					Player.AsPred().stomachContents.Add(deadPrey);
				}
				else
				{
					int preyIndex = binaryReader.ReadInt32();
					VoreTracker prey = new VoreTracker(preyType switch
					{
						0 => Main.player[preyIndex],
						1 => Main.npc[preyIndex],
						2 => Main.projectile[preyIndex],
						3 => Main.item[preyIndex],
						_ => throw new NotImplementedException(),
					});
					Player.AsPred().stomachContents.Add(prey);
				}
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

			void DrawHungryPlayerTummy(ref PlayerDrawSet drawInfo, int size, int offsetX = 0, int offsetY = 0)
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
					DrawHungryPlayerTummy(ref drawInfo, 1, 0, 6);
					break;
				case 2:
					DrawHungryPlayerTummy(ref drawInfo, 2, -2, 6);
					break;
				case 3:
					DrawHungryPlayerTummy(ref drawInfo, 3, player.IsAirborne() ? -4 : -2, 6);
					break;
				case 4:
					DrawHungryPlayerTummy(ref drawInfo, 4, -4, 2);
					break;
				case 5:
					DrawHungryPlayerTummy(ref drawInfo, 5, -4, 0);
					break;
				case 6:
					DrawHungryPlayerTummy(ref drawInfo, 6, -4, -2);
					break;
				case 7:
					DrawHungryPlayerTummy(ref drawInfo, 7, -6, -4);
					break;
			}
		}
	}
}

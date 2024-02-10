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
using V2.Core.StruggleSystem;
using V2.Items;
using V2.Items.Voraria.Consumables.PermanentUpgrades;
using V2.NPCs;
using V2.PlayerHandling.PredPlayerGoals;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Starter;
using V2.Sounds.Vore;
using V2.StatusEffects.Debuffs;
using V2.UI.PredStatsMenu;

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
	public partial class PredPlayer : ModPlayer
	{
		public bool SyncRequired_PredPoints { get; set; }
		public VoreTracker StomachTracker
		{
			get
			{
				if (Main.gameMenu)
					return null;

				return ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(x => x.Predator is Player predPlayer && predPlayer.whoAmI == Player.whoAmI);
			}
		}

		private double _stomachache;
		public double Stomachache
		{
			get => _stomachache;
			set => _stomachache = Math.Min(Math.Max(0, value), StomachacheMeterCapacity);
		}

		public int predLevel;

		public bool InPredStatsMenu { get; set; }
		public Dictionary<string, bool> GoalsCompleted { get; set; }
		public int TotalStatPoints
		{
			get
			{
				int points = 0;
				foreach (PredPlayerGoal goal in PredPlayerGoalLoader.PredPlayerGoals)
				{
					if (!GoalsCompleted.ContainsKey(goal.InternalName))
						GoalsCompleted.Add(goal.InternalName, false);

					if (GoalsCompleted[goal.InternalName])
						points += goal.StatPointsFromCompletion;
				}
				return points;
			}
		}
		public int AllocatedStatPoints => GLP.Spent + TUM.Spent + ACI.Spent + ABS.Spent;
		public int AvailableStatPoints => TotalStatPoints - AllocatedStatPoints;
		public PredStat GLP { get; set; }
		public StatModifier SwallowSizeModifier;
		public static double BaseSwallowSize => 0.80;
		public static double SwallowSizePerLevel => 0.05;
		public double SwallowCapacity
		{
			get
			{
				double baseSwallowSize = BaseSwallowSize;
				baseSwallowSize += SwallowSizePerLevel * GLP.Total;
				if (ModContent.GetInstance<V2ServerConfig>().Glutton)
					baseSwallowSize *= 120.0;
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
		public static double StomachCapacityPerLevel => 0.05;
		public double StomachCapacity
		{
			get
			{
				double baseStomachCapacity = BaseStomachCapacity;
				baseStomachCapacity += StomachCapacityPerLevel * TUM.Total;
				if (ModContent.GetInstance<V2ServerConfig>().Glutton)
					baseStomachCapacity *= 120.0;
				return StomachCapacityModifier.ApplyTo((float)baseStomachCapacity);
			}
		}
		public StatModifier StomachacheMeterCapacityModifier;
		public static double BaseStomachacheMeterCapacity => 100.0;
		public static double StomachacheMeterCapacityPer5Levels => 10.0;
		public double StomachacheMeterCapacity
		{
			get
			{
				double baseStomachacheMeterCapacity = BaseStomachacheMeterCapacity;
				baseStomachacheMeterCapacity += StomachacheMeterCapacityPer5Levels * Math.Floor(TUM.Total / 5.0);
				if (ModContent.GetInstance<V2ServerConfig>().Glutton)
					baseStomachacheMeterCapacity *= 80.0;
				return StomachacheMeterCapacityModifier.ApplyTo((float)baseStomachacheMeterCapacity);
			}
		}
		public PredStat ACI { get; set; }
		/// <summary>
		/// Denotes the tier of stomach acids this player currently has.<br/>
		/// Defaults to 0.<br/>
		/// <br/>
		/// 0 - Normal<br/>
		/// 1 - Enchanted<br/>
		/// 2 - Royal<br/>
		/// 99 - Divine<br/>
		/// 100 - Chronological<br/>
		/// </summary>
		public int AcidTier
		{
			get {
				if (PermanentUpgradesGained.ContainsKey("AcidTier2") && PermanentUpgradesGained["AcidTier2"])
					return 2;

				if (PermanentUpgradesGained.ContainsKey("AcidTier1") && PermanentUpgradesGained["AcidTier1"])
					return 1;

				return 0;
			}
		}
		public StatModifier DigestionTickDamageModifier;
		public static double BaseDigestionTickDamage => 12.0;
		public static double DigestionTickDamagePerLevel => 1.5;
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
				if (ModContent.GetInstance<V2ServerConfig>().Glutton)
					basePreyAbsorptionRate *= 60.0;
				return PreyAbsorptionRateModifier.ApplyTo((float)basePreyAbsorptionRate);
			}
		}
		public double PreyAbsorptionRatePerTick => PreyAbsorptionRate / (double)V2Utils.SensibleTime(minutes: 1);
		public StatModifier BuffExtensionTimeModifier;
		public static double BuffExtensionTimePer5Levels => 0.04;
		public double BuffExtensionFactor
		{
			get
			{
				double baseBuffExtensionTime = BuffExtensionTimePer5Levels * Math.Floor(ABS.Total / 5.0);
				return 1.0 + BuffExtensionTimeModifier.ApplyTo((float)baseBuffExtensionTime);
			}
		}
		public StatModifier DebuffDisextensionTimeModifier;
		public static double DebuffDisextensionTimePer5Levels => 0.04;
		public double DebuffDisextensionFactor
		{
			get
			{
				double baseDebuffDisextensionTime = DebuffDisextensionTimePer5Levels * Math.Floor(ABS.Total / 5.0);
				return 1.0 + DebuffDisextensionTimeModifier.ApplyTo((float)baseDebuffDisextensionTime);
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

		// Charms.
		/// <summary>
		/// Denotes whether or not this player has the Indigestion Charm equipped.<br/>
		/// Defaults to <see langword="false"/> at the start of each tick. Set to <see langword="true"/> if the player has the Indigestion Charm equipped.<br/>
		/// </summary>
		public bool charmNoDigest;
		public bool charmNoAirDrain;
		public bool charmStealPreyLoot;

		public Dictionary<string, bool> PermanentUpgradesGained { get; set; }
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

		public bool BlockSwallowAttempts
		{
			get
			{
				if (Player.CurrentCaptor() is not null)
					return true;

				if (Player.HasBuff(ModContent.BuffType<SoreThroat>()))
					return true;

				return false;
			}
		}
		public SlotId ActiveStomachNoises { get; set; }
		public double StomachFullness
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (StomachTracker is not null)
				{
					foreach (PreyData prey in StomachTracker.Prey)
					{
						totalBellyWeight += prey.WeightLeftToDigest;
						if (prey.NoHealth)
							continue;

						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachFullness;
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
				if (StomachTracker is not null)
				{
					foreach (PreyData prey in StomachTracker.Prey)
					{
						if (prey.NoHealth)
							continue;

						totalBellyWeight += prey.WeightLeftToDigest;
						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachFullness;
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

		public StatModifier StomachWeightModifier;

		public double StomachWeight
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (StomachTracker is not null)
				{
					foreach (PreyData prey in StomachTracker.Prey)
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
				return (double)StomachWeightModifier.ApplyTo((float)totalBellyWeight);
			}
		}

		public double KickyStomachWeight
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (StomachTracker is not null)
				{
					foreach (PreyData prey in StomachTracker.Prey)
					{
						if (prey.NoHealth)
							continue;

						totalBellyWeight += prey.WeightLeftToDigest;
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
				return (double)StomachWeightModifier.ApplyTo((float)totalBellyWeight);
			}
		}

		public double PercentBellySizeModifier { get; set; }
		public int FlatBellySizeModifier { get; set; }
		public int StomachSize
		{
			get
			{
				int tummySize = (int)Math.Floor(5.0 * Math.Sqrt(StomachFullness));
				tummySize = (int)Math.Round((double)tummySize * PercentBellySizeModifier);
				tummySize += FlatBellySizeModifier;
				return Math.Min(tummySize, 7);
			}
		}

		public bool SizeScanner { get; set; }

		public override void Initialize()
		{
			SmallBurps = Burps.Humanoid.Small;
			StandardBurps = Burps.Humanoid.Standard;

			SmallGulps = Gulps.Short;
			BigGulps = Gulps.Standard;

			GLP = new PredStat();
			ACI = new PredStat();
			TUM = new PredStat();
			ABS = new PredStat();

			Stomachache = 0;

			charmNoDigest = false;
			charmNoAirDrain = false;
			charmStealPreyLoot = false;

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

			PercentBellySizeModifier = 1.0;
			FlatBellySizeModifier = 0;

			PermanentUpgradesGained = new Dictionary<string, bool>();
			PermanentUpgradesGained.Add("PureSwallow1", false);
			PermanentUpgradesGained.Add("AcidTier1", false);
			PermanentUpgradesGained.Add("AcidTier2", false);

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
			SyncRequired_PredPoints = false;

			charmNoDigest = false;
			charmNoAirDrain = false;
			charmStealPreyLoot = false;

			GLP.Base = 0;
			GLP.Extra = 0;
			SwallowSizeModifier = StatModifier.Default;
			LiquidSwallowSizeModifier = StatModifier.Default;
			StruggleGraceTimeModifier = StatModifier.Default;
			TUM.Base = 0;
			TUM.Extra = 0;
			if (StomachTracker is null || KickyStomachFullness == 0.0)
				Stomachache -= 0.08;
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
			DebuffDisextensionTimeModifier = StatModifier.Default;

			StomachWeightModifier = StatModifier.Default;

			PercentBellySizeModifier = 1.0;
			FlatBellySizeModifier = 0;

			SizeScanner = false;

			UpdatePredStatPointsFromPermUpgrades();
		}

		public void UpdatePredStatPointsFromPermUpgrades()
		{
			if (PermanentUpgradesGained.ContainsKey("PureSwallow1") && PermanentUpgradesGained["PureSwallow1"])
				GLP.Base += PureSwallowBoost1.GLPBonus;
		}

		public override bool HoverSlot(Item[] inventory, int context, int slot)
		{
			if (inventory.Length == 59)
			{
				if (V2.ItemGulpHotkey.JustPressed && Player.whoAmI == Main.myPlayer)
				{
					int origStack = inventory[slot].stack;
					inventory[slot].stack = 1;
					if (CanSwallow(Player, inventory[slot]))
					{
						if (origStack > 1)
						{
							Item eatenItem = new Item();
							eatenItem.SetDefaults(inventory[slot].type);
							eatenItem.stack = 1;
							Player.ForceDropItem(Player.Center, ref eatenItem, out Item itemDrop);
							Swallow(Player, itemDrop);
							inventory[slot].stack = origStack - 1;
						}
						else
						{
							Player.ForceDropItem(Player.Center, ref inventory[slot], out Item itemDrop);
							Swallow(Player, itemDrop);
						}
						ModContent.GetInstance<FirstItemEaten>().TrySetCompletion(Player);
					}
					else
						inventory[slot] .stack = origStack;
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
				if (V2.SwallowHotkey.JustPressed)
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

						if (potentialMeal.CurrentCaptor() is not null)
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
						if (!potentialMeal.active || potentialMeal.dead || potentialMeal.whoAmI == Player.whoAmI)
							continue;

						if (potentialMeal.CurrentCaptor() is not null)
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
					if (tile.LiquidAmount > 0 && Player.AsPred().StomachCapacity - Player.AsPred().StomachFullness >= Player.AsPred().EffectiveLiquidSwallowSize(tile.LiquidType))
					{
						int liquidToDrink = (tile.LiquidAmount > Player.AsPred().LiquidSwallowSize) ? Player.AsPred().LiquidSwallowSize : tile.LiquidAmount;

						Player.AsPred().lastLiquidDrank = tile.LiquidType switch
						{
							0 => "Water",
							1 => "Lava",
							2 => "Honey",
							3 => "Shimmer",
							_ => "Some other liquid",
						};

						PreyData newDrink = new PreyData(tile.LiquidType, liquidToDrink);
						if (Player.AsPred().StomachTracker is not null && Player.AsPred().StomachTracker.Prey.FirstOrDefault(x => x.Type == PreyType.Liquid && x.ExactType == tile.LiquidType) is PreyData existingDrink)
							existingDrink.WeightLeftToDigest += newDrink.WeightLeftToDigest;
						else
							AddNewPrey(Player, newDrink);

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

						if (Main.GameUpdateCount % 60 == 0)
						{
							SoundEngine.PlaySound(
								Player.AsPred().SmallGulps with { Volume = 0.45f, Pitch = 0.25f },
								Player.position + new Vector2(0f, -10f)
							);
						}
					}
				}
				#endregion
				#region Regurgitating swallowed prey
				if (V2.RegurgitateHotkey.JustPressed && Player.AsPred().StomachTracker?.Prey.Count > 0)
				{
					PreyData prey = Player.AsPred().StomachTracker?.Prey.FindLast(x => !x.NoHealth && x.Type != PreyType.Liquid);
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
						realPrey.velocity = new Vector2(Player.direction * 10f, -2.5f);
						if (realPrey is NPC realPreyNPC)
						{
							realPreyNPC.AsFood().EatenSafetyFrames = 20;
						}
						else if (realPrey is Player realPreyPlayer)
						{

						}
						else if (realPrey is Item realPreyItem)
						{
							realPreyItem.noGrabDelay = 60;
						}
						Player.AsPred().StomachTracker?.Prey.Remove(prey);
						SoundEngine.PlaySound(
							prey.WeightLeftToDigest <= 0.3 ? Player.AsPred().SmallBurps : Player.AsPred().StandardBurps,
							Player.TrueCenter() + new Vector2(Player.direction * 8f, -14f)
						);
					}
				}
				#endregion
			}

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
				if (preyPlayer.CurrentCaptor() is not null)
					return false;
			}
			else if (prey is NPC preyNPC)
			{
				if (V2.VoreNPCBlacklist.Contains(preyNPC.type))
					return false;

				bool tastesLikeSkittles = preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress;
				if (tastesLikeSkittles)
					return preyNPC.CurrentCaptor() is null;

				bool isThisAFuckingBoss = preyNPC.boss || (preyNPC.type >= NPCID.EaterofWorldsHead && preyNPC.type <= NPCID.EaterofWorldsTail); // I hate EoW
				if (isThisAFuckingBoss && !ModContent.GetInstance<V2ServerConfig>().Glutton)
					return false;

				if (preyNPC.CurrentCaptor() is not null)
					return false;
			}
			else if (prey is Item preyItem)
			{
				if (preyItem.AsFood().MaxHealth == -1)
					return false;

				if (preyItem.favorited)
					return false;

				if (preyItem.CurrentCaptor() is not null)
					return false;
			}

			if (PreyData.GetPreySize(prey) > pred.AsPred().SwallowCapacity)
				return false;

			if (PreyData.GetPreySize(prey) > pred.AsPred().StomachCapacity - pred.AsPred().StomachFullness)
				return false;

			return true;
		}

		/// <summary>
		/// Causes the given predator player to swallow the given prey entity, if the given prey entity can be swallowed.
		/// </summary>
		/// <param name="pred">The predator which will attempt to swallow the given prey.</param>
		/// <param name="prey">The prey which will be attempt to be swallowed by the given predator.</param>
		/// <param name="MPstate">
		/// </param>
		/// <param name="MPwhoAmI">
		/// The <see cref="Player.whoAmI"/> of the client that sent a request for this swallow.<br/>
		/// Unused in singleplayer, but used in multiplayer to subsequently send and correctly receive a second message.<br/>
		/// </param>
		public static void Swallow(Player pred, Entity prey, int MPstate = 0, int MPwhoAmI = -1)
		{
			if (!CanSwallow(pred, prey))
				return;

			if (MPstate == 0 && Main.netMode == NetmodeID.MultiplayerClient)
			{
				MPstate = 1;
				MPwhoAmI = Main.myPlayer;
			}

			PreyData food = PreyData.NewData(prey);
			AddNewPrey(pred, food);
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
				case PreyType.Player:
					Player player = prey as Player;
					player.AsFood().TotalTimesSwallowed += 1;
					pred.AsPred().lastEntitySwallowed = "Player";
					pred.AsPred().lastEntitySwallowedMod = "Terraria";
					break;
				case PreyType.NPC:
					NPC npc = prey as NPC;
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
				case PreyType.Item:
					Item item = prey as Item;
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

			if (MPstate == 1)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.RequestSwallowPrey);
				packet.Write((byte)0);
				packet.Write(pred.whoAmI);
				packet.Write((byte)food.Type);
				packet.Write(prey.whoAmI);
				packet.Write(MPwhoAmI);
				packet.Send();
			}
			else if (MPstate == 2)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.SyncSwallowPrey);
				packet.Write((byte)0);
				packet.Write(pred.whoAmI);
				packet.Write((byte)food.Type);
				packet.Write(prey.whoAmI);
				packet.Write(MPwhoAmI);
				packet.Send(-1, ignoreClient: MPwhoAmI);
			}
		}

		public static void AddNewPrey(Player pred, PreyData prey)
		{
			if (pred.AsPred().StomachTracker is null)
				VoreTracker.NewTracker(pred, new List<PreyData>() { prey });
			else
				pred.AsPred().StomachTracker.QueueNewPrey(prey);
		}

		/// <summary>
		/// Runs update ticks on all food in this predatory player's stomach.
		/// </summary>
		public static void UpdatePrey(Player pred)
		{
			if (pred.AsPred().Stomachache == pred.AsPred().StomachacheMeterCapacity && pred.AsPred().StomachTracker is not null && pred.AsPred().StomachTracker.Prey.Count > 0)
			{
				foreach (PreyData prey in pred.AsPred().StomachTracker.Prey)
				{
					Entity realPrey = prey.Type switch
					{
						PreyType.Player => prey.Instance as Player,
						PreyType.NPC => prey.Instance as NPC,
						PreyType.Projectile => prey.Instance as Projectile,
						PreyType.Item => prey.Instance as Item,
						PreyType.Liquid => null,
						PreyType.Custom => null,
						_ => throw new NotImplementedException(),
					};

					if (realPrey is null)
						continue;

					realPrey.position = pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f);
					realPrey.velocity = new Vector2(pred.direction * 12.5f, -2.5f);
					if (realPrey is NPC realPreyNPC)
					{
						realPreyNPC.AsFood().EatenSafetyFrames = 20;
					}
					else if (realPrey is Player realPreyPlayer)
					{

					}
					else if (realPrey is Item realPreyItem)
					{
						realPreyItem.noGrabDelay = 60;
					}
				}
				SoundEngine.PlaySound(
					pred.AsPred().StandardBurps,
					pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
				);
				pred.AsPred().StomachTracker.Prey.Clear();
				pred.AsPred().StomachTracker.RefreshStruggleChartList();
				return;
			}

			foreach (PreyData prey in pred.AsPred().StomachTracker.Prey)
			{
				if (!prey.NoHealth)
				{
					switch (prey.Type)
					{
						case PreyType.Player:
							Player preyPlayer = prey.Instance as Player;
							preyPlayer.velocity = Vector2.Zero;
							preyPlayer.position = pred.position;
							break;
						case PreyType.NPC:
							NPC preyNPC = prey.Instance as NPC;
							preyNPC.velocity = Vector2.Zero;
							preyNPC.position = pred.position;
							break;
						case PreyType.Projectile:
							Projectile preyProjectile = prey.Instance as Projectile;
							if (preyProjectile.active)
								preyProjectile.velocity = Vector2.Zero;
							preyProjectile.position = pred.position;
							break;
						case PreyType.Item:
							Item preyItem = prey.Instance as Item;
							preyItem.AsFood().UpdateInStomach?.Invoke(preyItem, pred, prey.NoHealth);
							break;
					}
					double digestionDamage = pred.AsPred().DigestionTickDamage;
					double digestionRate = pred.AsPred().DigestionTickRate;
					if (digestionRate <= 0.0)
						digestionRate = 1.0;

					int digestionFrameRate = (int)Math.Round(60.0 / digestionRate);
					if (prey.timeSpentInStomach % digestionFrameRate == 0)
					{
						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPlayer = prey.Instance as Player;
								bool shouldDigestPlayer = !pred.AsPred().SafeStomach;
								if (shouldDigestPlayer)
								{
									prey.NoHealth = preyPlayer.AsFood().TakeDigestionDamage(pred, digestionDamage);
									if (prey.NoHealth)
									{
										if (!pred.AsPred().mealCount.ContainsKey("Terraria: Player"))
											pred.AsPred().mealCount.Add("Terraria: Player", 0);
										pred.AsPred().mealCount["Terraria: Player"] += 1;
										SoundEngine.PlaySound(
											pred.AsPred().StandardBurps,
											pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
										);
									}
								}
								break;
							case PreyType.NPC:
								NPC preyNPC = prey.Instance as NPC;
								bool shouldDigestNPC = !pred.AsPred().SafeStomach;
								if (shouldDigestNPC)
								{
									if (preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress)
										digestionDamage *= 40.0;
									prey.NoHealth = PreyNPC.TakeDigestionDamage(preyNPC, pred, digestionDamage);
									if (prey.NoHealth)
									{
										prey.Instance = null;
										string preyNPCMod = preyNPC.ModNPC != null ? preyNPC.ModNPC.Mod.DisplayName : "Terraria";
										if (!pred.AsPred().mealCount.ContainsKey(preyNPCMod + ": " + preyNPC.TypeName))
											pred.AsPred().mealCount.Add(preyNPCMod + ": " + preyNPC.TypeName, 0);
										pred.AsPred().mealCount[preyNPCMod + ": " + preyNPC.TypeName] += 1;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? pred.AsPred().SmallBurps : pred.AsPred().StandardBurps,
											pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
										);
									}
								}
								break;
							case PreyType.Item:
								Item preyItem = prey.Instance as Item;
								if (preyItem.IsAir)
									break;

								bool shouldDigestItem = !pred.AsPred().SafeStomach;
								shouldDigestItem &= pred.AsPred().AcidTier >= preyItem.AsFood().AcidResistTier;
								if (shouldDigestItem)
								{
									prey.NoHealth = preyItem.TakeDigestionDamage(pred, digestionDamage);
									if (prey.NoHealth)
									{
										string preyItemMod = preyItem.ModItem != null ? preyItem.ModItem.Mod.DisplayName : "Terraria";
										if (!pred.AsPred().mealCount.ContainsKey(preyItemMod + ": " + preyItem.Name))
											pred.AsPred().mealCount.Add(preyItemMod + ": " + preyItem.Name, 0);
										pred.AsPred().mealCount[preyItemMod + ": " + preyItem.Name] += preyItem.stack;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? pred.AsPred().SmallBurps : pred.AsPred().StandardBurps,
											pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
										);
									}
								}
								break;
						}
					}
				}
				else
				{
					double absorptionRate = pred.AsPred().PreyAbsorptionRatePerTick / (double)pred.AsPred().StomachTracker?.Prey.Count;
					prey.WeightLeftToDigest -= absorptionRate;
					if (prey.WeightLeftToDigest < 0)
						prey.WeightLeftToDigest = 0;

					switch (prey.Type)
					{
						case PreyType.Liquid:
							switch (prey.ExactType)
							{
								case LiquidID.Water:
									break;
								case LiquidID.Lava:
									if (!pred.AsPred().CanDrinkLavaSafe)
									{
										pred.AddBuff(ModContent.BuffType<MoltenStomach>(), 3);
									}
									break;
								case LiquidID.Honey:
									break;
								case LiquidID.Shimmer:
									if (!pred.AsPred().CanDrinkShimmerSafe)
									{
										if (!pred.AsPred().PrimedForShimmerStomachDeath)
										{
											pred.AsPred().PrimedForShimmerStomachDeath = true;
											pred.AddBuff(ModContent.BuffType<ShimmeringStomach>(), 300);
										}
										else if (!pred.AsPred().ShimmeringTummy)
										{
											pred.AsPred().PrimedForShimmerStomachDeath = false;
											pred.KillMe(
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
															Player = pred.name
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

			if (pred.CurrentCaptor() is null)
			{
				bool stomachNoisesPlaying = SoundEngine.TryGetActiveSound(pred.AsPred().ActiveStomachNoises, out ActiveSound stomachNoises);
				if (!stomachNoisesPlaying)
				{
					pred.AsPred().ActiveStomachNoises = SoundEngine.PlaySound(
						StomachNoises.Muffled with { Volume = 0.25f + (0.15f * pred.AsPred().StomachSize) },
						pred.TrueCenter()
					);
					SoundEngine.TryGetActiveSound(pred.AsPred().ActiveStomachNoises, out stomachNoises);
				}

				if (stomachNoises is null)
					return;

				stomachNoises.Position = pred.TrueCenter();
				stomachNoises.Volume = 0.25f;
				stomachNoises.Volume += 0.15f * pred.AsPred().StomachSize;
			}
		}

		public static void UpdateGeneralPredGoalsLogic(Player pred)
		{
			if (pred.sleeping.FullyFallenAsleep)
			{
				if (pred.AsPred().StomachWeightAtSleepStart == -1.0)
					pred.AsPred().StomachWeightAtSleepStart = pred.AsPred().StomachWeight;

				if (pred.AsPred().StomachWeight == 0.0 && pred.AsPred().StomachWeightAtSleepStart > 0.0 && pred.AsPred().StomachWeightAtSleepStart >= SleepSpeedsDigestion.FlatFullnessThreshold)
					ModContent.GetInstance<SleepSpeedsDigestion>().TrySetCompletion(pred);
			}
			else
				pred.AsPred().StomachWeightAtSleepStart = -1.0;

			if (pred.AsPred().StomachFullness / pred.AsPred().StomachCapacity > TooFull.FullnessThreshold)
			{
				pred.AsPred().OverfullTime += 1;
				if (pred.AsPred().OverfullTime >= TooFull.TimeThreshold)
					ModContent.GetInstance<TooFull>().TrySetCompletion(pred);
			}
			else
				pred.AsPred().OverfullTime = 0;
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
			if (Player.CurrentCaptor() is not null)
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
			if (Player.CurrentCaptor() is not null && Player.AsPred().StomachTracker is not null)
			{
				foreach (PreyData prey in Player.AsPred().StomachTracker.Prey)
				{
					Player.CurrentCaptor().QueueNewPrey(prey);
				}
			}

			Player.AsPred().InPredStatsMenu = false;
		}

		public override void UpdateDead()
		{
			Player.AsPred().PrimedForShimmerStomachDeath = false;
			Player.AsPred().Stomachache = 0;
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
			tag.Add("GLPSpent", GLP.Spent);
			tag.Add("TUMSpent", TUM.Spent);
			tag.Add("ACISpent", ACI.Spent);
			tag.Add("ABSSpent", ABS.Spent);
			foreach (KeyValuePair<string, bool> keyValuePair in Player.AsPred().PermanentUpgradesGained)
			{
				tag.Add("[PERM UPGRADES] " + keyValuePair.Key, keyValuePair.Value);
			}
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
			GLP.Spent = tag.GetInt("GLPSpent");
			TUM.Spent = tag.GetInt("TUMSpent");
			ACI.Spent = tag.GetInt("ACISpent");
			ABS.Spent = tag.GetInt("ABSSpent");
			PermanentUpgradesGained = new Dictionary<string, bool>();
			mealCount = new Dictionary<string, int>();
			drinkCount = new Dictionary<string, int>();
			GoalsCompleted = new Dictionary<string, bool>();
			foreach (KeyValuePair<string, object> keyValuePair in tag)
			{
				if (keyValuePair.Key.StartsWith("[PERM UPGRADES] "))
				{
					string realKey = keyValuePair.Key.Remove(0, 16);
					bool permUpgradeUsed = tag.GetBool(keyValuePair.Key);
					PermanentUpgradesGained.Add(realKey, permUpgradeUsed);
					continue;
				}
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
	}

	public class VoreTum : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Torso);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player player = drawInfo.drawPlayer;
			int tumSize = player.AsPred().StomachSize;

			void DrawHungryPlayerTummy(ref PlayerDrawSet drawInfo, int size, int offsetX = 0, int offsetY = 0)
			{
				Texture2D tum = ModContent.Request<Texture2D>("V2/PlayerHandling/TumSprites/Bare_" + size, AssetRequestMode.ImmediateLoad).Value;
				if (player.IsAirborne() || player.sleeping.isSleeping)
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
				default:
					// do absolutely nothing lol
					break;
				case 1:
					DrawHungryPlayerTummy(
						ref drawInfo,
						1,
						player.IsAirborne() ? 0 : 0,
						player.IsAirborne() ? 6 : 6
					);
					break;
				case 2:
					DrawHungryPlayerTummy(
						ref drawInfo,
						2,
						player.IsAirborne() ? -2 : -2,
						player.IsAirborne() ? 6 : 6
					);
					break;
				case 3:
					DrawHungryPlayerTummy(
						ref drawInfo,
						3,
						player.IsAirborne() ? -4 : -2,
						player.IsAirborne() ? 6 : 6
					);
					break;
				case 4:
					DrawHungryPlayerTummy(
						ref drawInfo,
						4,
						player.IsAirborne() ? -4 : -4,
						player.IsAirborne() ? 4 : 4
					);
					break;
				case 5:
					DrawHungryPlayerTummy(
						ref drawInfo,
						5,
						player.IsAirborne() ? -4 : -4,
						player.IsAirborne() ? 4 : 2
					);
					break;
				case 6:
					DrawHungryPlayerTummy(
						ref drawInfo,
						6,
						player.IsAirborne() ? -2 : -2,
						player.IsAirborne() ? 4 : -2);
					break;
				case 7:
					DrawHungryPlayerTummy(
						ref drawInfo,
						7,
						player.IsAirborne() ? -4 : -2,
						player.IsAirborne() ? 0 : -4);
					break;
			}
		}
	}
}

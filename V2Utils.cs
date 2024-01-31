using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2
{
	public enum MealTime
	{
		Breakfast,
		BetweenBreakfastAndLunch,
		Lunch,
		BetweenLunchAndDinner,
		Dinner,
		LateNightSnacking
	}

	public static class V2Utils
	{
		/// <summary>
		/// Takes the given amount of time for an effect to last and converts it to a concrete frame count.<br/>
		/// Measurements are all based on real-life time, and assume a constant FPS (frames per second rate) of 60.
		/// </summary>
		/// <param name="hours">
		/// The number of hours an effect should last for.<br/>
		/// Defaults to 0, because no sane person ever makes an effect last for an hour or more.<br/>
		/// </param>
		/// <param name="minutes">
		/// The number of minutes an effect should last for.<br/>
		/// Defaults to 0.
		/// </param>
		/// <param name="seconds">
		/// The number of hours an effect should last for.<br/>
		/// Defaults to 0.
		/// </param>
		/// <param name="frames">
		/// The number of individual frames an effect should last for.<br/>
		/// Used only for very specific adjustments.<br/>
		/// Defaults to 0. To cover many common use cases for this:<br/>
		/// - 15 is a quarter of a second.<br/>
		/// - 20 is a third of a second.<br/>
		/// - 30 is a half of a second.
		/// </param>
		/// <returns>The total number of frames that the effect should last for, for the purpose of setting time-related fields.</returns>
		public static int SensibleTime(int hours = 0, int minutes = 0, int seconds = 0, int frames = 0)
		{
			int totalFrameCount = hours * 60 * 60 * 60;
			totalFrameCount += minutes * 60 * 60;
			totalFrameCount += seconds * 60;
			totalFrameCount += frames;
			return totalFrameCount;
		}

		/// <summary>
		/// Determines what time it is given the current state of the save, and outputs that time in measures comprehensible to the human mind.<br/>
		/// Assumes 12-hour time.<br/>
		/// </summary>
		/// <param name="pastMorning">
		/// Whether noon has passed for the day or not.<br/>
		/// </param>
		/// <param name="hour">
		/// The current hour.<br/>
		/// Ranges from 1-12.<br/>
		/// </param>
		/// <param name="minute">
		/// The current minute of the current hour.<br/>
		/// Ranges from 0-59.<br/>
		/// </param>
		/// <param name="second">
		/// The current second of the current minute of the current hour.<br/>
		/// Ranges from 0-59.<br/>
		/// </param>
		/// <param name="mealTime">
		/// The time of day that it's currently considered for the purpose of when you got ate, you dirty gut slut.<br/>
		/// If between 5:00 AM and 8:00 AM: <see cref="MealTime.Breakfast"/>.<br/>
		/// If between 8:00 AM and 11:00 AM: <see cref="MealTime.BetweenBreakfastAndLunch"/>.<br/>
		/// If between 11:00 AM and 2:00 PM: <see cref="MealTime.Lunch"/>.<br/>
		/// If between 2:00 PM and 5:00 PM: <see cref="MealTime.BetweenLunchAndDinner"/>.<br/>
		/// If between 5:00 PM and 9:00 PM: <see cref="MealTime.Dinner"/>.<br/>
		/// If between 9:00 PM and 5:00 AM: <see cref="MealTime.LateNightSnacking"/>.<br/>
		/// </param>
		public static void FigureOutWhatTimeItIs(out bool pastMorning, out int hour, out int minute, out int second, out MealTime mealTime)
		{
			pastMorning = false;
			double hours = Main.time;
			if (!Main.dayTime)
				hours += 54000.0;

			hours = hours / 86400.0 * 24.0;
			double mainTimeOffset = 7.5;
			hours = hours - mainTimeOffset - 12.0;
			if (hours < 0.0)
				hours += 24.0;

			if (hours >= 12.0)
				pastMorning = true;

			hour = (int)hours;
			double minutes = hours - (double)hour;
			minute = (int)(minutes * 60.0);
			double seconds = minutes - (double)minute;
			second = (int)(seconds * 60.0);

			if (hour > 12)
				hour -= 12;

			if (hour == 0)
				hour = 12;

			if (!pastMorning)
			{
				switch (hour)
				{
					case 12:
					case 1:
					case 2:
					case 3:
					case 4:
						mealTime = MealTime.LateNightSnacking;
						break;
					case 5:
					case 6:
					case 7:
						mealTime = MealTime.Breakfast;
						break;
					case 8:
					case 9:
					case 10:
						mealTime = MealTime.BetweenBreakfastAndLunch;
						break;
					case 11:
					default:
						mealTime = MealTime.Lunch;
						break;
				}
			}
			else
			{
				switch (hour)
				{
					case 12:
					case 1:
					default:
						mealTime = MealTime.Lunch;
						break;
					case 2:
					case 3:
					case 4:
						mealTime = MealTime.BetweenLunchAndDinner;
						break;
					case 5:
					case 6:
					case 7:
					case 8:
						mealTime = MealTime.Dinner;
						break;
					case 9:
					case 10:
					case 11:
						mealTime = MealTime.LateNightSnacking;
						break;
				}
			}
		}

		public static void AddVorariaDynamicTooltip(this List<TooltipLine> tooltips, string itemTooltipKey, object tooltipVariables)
		{
			TooltipLine dynamicTooltip = new TooltipLine(
				V2.Instance,
				"VorariaDynamicTooltip",
				(Main.keyState.IsKeyDown(Keys.LeftShift) && Main.keyState.IsKeyDown(Keys.LeftControl))
				? Language.GetTextValue(
					"Mods.V2.ItemTooltip." + itemTooltipKey + ".Flavor"
				) : (Main.keyState.IsKeyDown(Keys.LeftShift)
				? Language.GetTextValueWith(
					"Mods.V2.ItemTooltip." + itemTooltipKey + ".Long",
					tooltipVariables
				) : Language.GetTextValueWith(
					"Mods.V2.ItemTooltip." + itemTooltipKey + ".Short",
					tooltipVariables
				))
			);
			if (Main.keyState.IsKeyDown(Keys.LeftShift) && Main.keyState.IsKeyDown(Keys.LeftControl))
			{
				string tooltipFlavorText = "";
				string[] tooltipFlavorTextLines = Utils.WordwrapString(dynamicTooltip.Text, FontAssets.MouseText.Value, 900, 25, out int lineAmount);
				for (int i = 0; i < tooltipFlavorTextLines.Length; i++)
				{
					string line = tooltipFlavorTextLines[i];
					if (line is not null && line != "")
					{
						tooltipFlavorText += line;
						if (!line.Contains("\n") && i < lineAmount)
							tooltipFlavorText += "\n";
					}
				}
				dynamicTooltip.Text = tooltipFlavorText;
				dynamicTooltip.OverrideColor = Color.Gray;
			}
			
			if (tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name.Contains("Tooltip")) is TooltipLine tooltipLine)
			{
				tooltips.Insert(
					tooltips.IndexOf(tooltipLine),
					dynamicTooltip
				);
				tooltips.RemoveAll(x => x.Mod == "Terraria" && x.Name.Contains("Tooltip"));
			}
			else
			{
				tooltips.Add(dynamicTooltip);
			}
		}

		public static int TileCountAsPixelCount(double tileCount) => (int)Math.Round(tileCount * 16.0);


		// TO-DO: this shit is dumb. refactor tooltips once tooltip rework happens...assumin' it'll ever happen, that is. why do people love talkin' a big game and playin' none of it?
		// for the moment, what this does is search for each potential tooltip line before Tooltip0 in reverse order and return the first one that isn't null
		public static bool FindLastTooltipLineBeforeFlavorText(List<TooltipLine> tooltips, out TooltipLine line)
		{
			line = tooltips.FirstOrDefault(x => x.Name == "V2EdibleByNormalUse")
				?? tooltips.FirstOrDefault(x => x.Name == "V2AcidResist")
				?? tooltips.FirstOrDefault(x => x.Name == "V2SizeAsFood")
				?? tooltips.FirstOrDefault(x => x.Name == "V2Durability")
				?? tooltips.FirstOrDefault(x => x.Name == "Material")
				?? tooltips.FirstOrDefault(x => x.Name == "Consumable")
				?? tooltips.FirstOrDefault(x => x.Name == "Ammo")
				?? tooltips.FirstOrDefault(x => x.Name == "Placeable")
				?? tooltips.FirstOrDefault(x => x.Name == "UseManaPerSecond")
				?? tooltips.FirstOrDefault(x => x.Name == "UseMana")
				?? tooltips.FirstOrDefault(x => x.Name == "HealMana")
				?? tooltips.FirstOrDefault(x => x.Name == "HealLife")
				?? tooltips.FirstOrDefault(x => x.Name == "TileBoost")
				?? tooltips.FirstOrDefault(x => x.Name == "HammerPower")
				?? tooltips.FirstOrDefault(x => x.Name == "AxePower")
				?? tooltips.FirstOrDefault(x => x.Name == "PickPower")
				?? tooltips.FirstOrDefault(x => x.Name == "Defense")
				?? tooltips.FirstOrDefault(x => x.Name == "VanityLegal")
				?? tooltips.FirstOrDefault(x => x.Name == "Vanity")
				?? tooltips.FirstOrDefault(x => x.Name == "Quest")
				?? tooltips.FirstOrDefault(x => x.Name == "WandConsumes")
				?? tooltips.FirstOrDefault(x => x.Name == "Equipable")
				?? tooltips.FirstOrDefault(x => x.Name == "BaitPower")
				?? tooltips.FirstOrDefault(x => x.Name == "NeedsBait")
				?? tooltips.FirstOrDefault(x => x.Name == "FishingPower")
				?? tooltips.FirstOrDefault(x => x.Name == "Knockback")
				?? tooltips.FirstOrDefault(x => x.Name == "Speed")
				?? tooltips.FirstOrDefault(x => x.Name == "CritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "Damage")
				?? tooltips.FirstOrDefault(x => x.Name == "SocialDesc")
				?? tooltips.FirstOrDefault(x => x.Name == "Social")
				?? tooltips.FirstOrDefault(x => x.Name == "FavoriteNoNoms")
				?? tooltips.FirstOrDefault(x => x.Name == "FavoriteDesc")
				?? tooltips.FirstOrDefault(x => x.Name == "Favorite")
				?? tooltips.FirstOrDefault(x => x.Name == "ItemName");
			return line != null;
		}

		public static bool FindFirstTooltipLineAfterFlavorText(List<TooltipLine> tooltips, out TooltipLine line)
		{
			line = tooltips.FirstOrDefault(x => x.Name == "EtherianManaWarning")
				?? tooltips.FirstOrDefault(x => x.Name == "WellFedExpert")
				?? tooltips.FirstOrDefault(x => x.Name == "BuffTime")
				?? tooltips.FirstOrDefault(x => x.Name == "OneDropLogo")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixDamage")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixSpeed")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixCritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixUseMana")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixSize")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixShootSpeed")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixKnockback")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccDefense")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccMaxMana")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccCritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccDamage")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccMoveSpeed")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccMeleeSpeed")
				?? tooltips.FirstOrDefault(x => x.Name == "SetBonus")
				?? tooltips.FirstOrDefault(x => x.Name == "Expert")
				?? tooltips.FirstOrDefault(x => x.Name == "Master")
				?? tooltips.FirstOrDefault(x => x.Name == "JourneyResearch")
				?? tooltips.FirstOrDefault(x => x.Name == "BestiaryNotes")
				?? tooltips.FirstOrDefault(x => x.Name == "SpecialPrice")
				?? tooltips.FirstOrDefault(x => x.Name == "Price");
			return line != null;
		}

		public static bool FindLastDamageRelatedTooltipLine(List<TooltipLine> tooltips, out TooltipLine line)
		{
			line = tooltips.FirstOrDefault(x => x.Name == "Knockback")
				?? tooltips.FirstOrDefault(x => x.Name == "Speed")
				?? tooltips.FirstOrDefault(x => x.Name == "CritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "Damage");
			return line != null;
		}

		public static bool FindLastTooltipLineBeforeManaCost(List<TooltipLine> tooltips, out TooltipLine line)
		{
			line = tooltips.FirstOrDefault(x => x.Name == "HealMana")
				?? tooltips.FirstOrDefault(x => x.Name == "HealLife")
				?? tooltips.FirstOrDefault(x => x.Name == "TileBoost")
				?? tooltips.FirstOrDefault(x => x.Name == "HammerPower")
				?? tooltips.FirstOrDefault(x => x.Name == "AxePower")
				?? tooltips.FirstOrDefault(x => x.Name == "PickPower")
				?? tooltips.FirstOrDefault(x => x.Name == "Defense")
				?? tooltips.FirstOrDefault(x => x.Name == "VanityLegal")
				?? tooltips.FirstOrDefault(x => x.Name == "Vanity")
				?? tooltips.FirstOrDefault(x => x.Name == "Quest")
				?? tooltips.FirstOrDefault(x => x.Name == "WandConsumes")
				?? tooltips.FirstOrDefault(x => x.Name == "Equipable")
				?? tooltips.FirstOrDefault(x => x.Name == "BaitPower")
				?? tooltips.FirstOrDefault(x => x.Name == "NeedsBait")
				?? tooltips.FirstOrDefault(x => x.Name == "FishingPower")
				?? tooltips.FirstOrDefault(x => x.Name == "Knockback")
				?? tooltips.FirstOrDefault(x => x.Name == "Speed")
				?? tooltips.FirstOrDefault(x => x.Name == "CritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "Damage")
				?? tooltips.FirstOrDefault(x => x.Name == "SocialDesc")
				?? tooltips.FirstOrDefault(x => x.Name == "Social")
				?? tooltips.FirstOrDefault(x => x.Name == "FavoriteDesc")
				?? tooltips.FirstOrDefault(x => x.Name == "Favorite")
				?? tooltips.FirstOrDefault(x => x.Name == "ItemName");
			return line != null;
		}

		public static void InsertNewTooltipLine(ref List<TooltipLine> tooltips, TooltipLine baseLine, int lineOffset, string lineName, string lineContents)
		{
			TooltipLine newLine = new TooltipLine(V2.Instance, lineName, lineContents);
			InsertNewTooltipLine(ref tooltips, baseLine, lineOffset, newLine);
		}
		public static void InsertNewTooltipLine(ref List<TooltipLine> tooltips, TooltipLine baseLine, int lineOffset, TooltipLine newLine)
		{
			if (tooltips.IndexOf(baseLine) + lineOffset > tooltips.Count - 1)
				tooltips.Add(newLine);
			else
				tooltips.Insert(tooltips.IndexOf(baseLine) + lineOffset, newLine);
		}
	}
}

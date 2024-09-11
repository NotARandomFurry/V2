using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using V2.PlayerHandling;

namespace V2.Core
{
	public static class ArmorSetHandler
	{
		public static List<ArmorSetDefinition> ArmorSets { get; set; } = [];

		public static void RegisterArmorSet(ArmorSetDefinition armorSet) => ArmorSets.Add(armorSet);

		/// <summary>
		/// Checks against every VSC-defined armor set, and applies the first set bonus found.
		/// </summary>
		/// <param name="player">The player to check against and apply defined armor sets for.</param>
		/// <returns>Whether or not a VSC-defined armor set was found and applied.</returns>
		public static bool CheckDefinedArmorSets(Player player)
		{
			foreach (ArmorSetDefinition set in ArmorSets)
			{
				if (set.Active(player))
				{
					player.setBonus = Main.keyState.IsKeyDown(Keys.LeftShift)
						? ("SET BONUS:\n" + Language.GetTextValueWith(
							"Mods.V2.ItemTooltip." + set.SetBonusDescriptionKey + ".Long",
							set.SetBonusDescriptionVariables
						)) : Language.GetTextValueWith(
							"Mods.V2.ItemTooltip." + set.SetBonusDescriptionKey + ".Short",
							set.SetBonusDescriptionVariables
						);
					player.AsV2Player().setBonusActive = true;
					if (!(Main.keyState.IsKeyDown(Keys.LeftShift) && Main.keyState.IsKeyDown(Keys.LeftControl)))
						player.AsV2Player().setBonusShouldBeDisplayed = true;
					set.ApplySetBonus(player);
					return true;
				}
			}

			return false;
		}
	}
}

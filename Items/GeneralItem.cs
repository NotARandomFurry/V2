using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.UI;

namespace V2.Items
{
	public enum ItemLocation
	{
		InWorld,
		InPlayerInventory,
		BeingFood,
	}
	public class GeneralItem : GlobalItem
	{
		public DelegateHeldItemDrawingUI heldItemUIDrawMethod;

		public delegate void DelegateArmorEffectCode(Item item, Player player);
		public DelegateArmorEffectCode ArmorEffectCode { get; internal set; }

		public delegate void DelegateAccessoryEffectCode(Item item, Player player, bool hideVisual);
		public DelegateAccessoryEffectCode AccessoryEffectCode { get; internal set; }

		public delegate void DelegateAccessoryVanityEffectCode(Item item, Player player);
		public DelegateAccessoryVanityEffectCode AccessoryVanityEffectCode { get; internal set; }

		public int ReleasedNPCNetID;

		public float StruggleDamageBaseMod { get; set; }

		public bool PlaceableCanBeHungry { get; set; }
		public bool PlaceableHungryByDefault { get; set; }

		public override bool InstancePerEntity => true;

		public GeneralItem()
		{
			heldItemUIDrawMethod = null;

			ReleasedNPCNetID = 0;

			StruggleDamageBaseMod = 0f;

			PlaceableCanBeHungry = false;
			PlaceableHungryByDefault = false;
		}

		public override void HoldItem(Item item, Player player)
		{
			player.AsFood().StruggleDamageModifier.Base += StruggleDamageBaseMod;
		}

		public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
		{
			float weightMovementMult = (float)Math.Min(1.0, 1.0 / (player.AsPred().StomachWeight + 1.0));
			acceleration *= weightMovementMult;
		}

		public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			float weightMovementMult = (float)Math.Min(1.0, 1.0 / (player.AsPred().StomachWeight + 1.0));
			ascentWhenFalling *= weightMovementMult;
			ascentWhenRising *= weightMovementMult;
			maxCanAscendMultiplier *= weightMovementMult;
			maxAscentMultiplier *= weightMovementMult;
			constantAscend *= weightMovementMult;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;

			TooltipLine damageLine = tooltips.FirstOrDefault(x => x.Name == "Damage");
			if (damageLine is not null && StruggleDamageBaseMod != 0)
			{
				tooltips.Insert(
					tooltips.IndexOf(damageLine) + 1,
					new TooltipLine(
						V2.Instance,
						"V2StruggleDamage",
						Language.GetTextValueWith("Mods.V2.ItemTooltip.Generic.StruggleDamageBoost", new { StrDmgUp = StruggleDamageBaseMod })
					)
				);
			}

			V2Utils.FindLastTooltipLineBeforeFlavorText(tooltips, out TooltipLine finalLine);
			int flavorTextOffset = 1;
			if (PlaceableCanBeHungry)
			{
				V2Utils.InsertNewTooltipLine(
					ref tooltips,
					finalLine,
					flavorTextOffset,
					new TooltipLine(
						V2.Instance,
						"V2HungryObjects",
						PlaceableHungryByDefault ? Language.GetTextValue("Mods.V2.ItemTooltip.Generic.HungryPlaceable.DefaultHungry") : Language.GetTextValue("Mods.V2.ItemTooltip.Generic.HungryPlaceable.DefaultNormal")
					)
				);
				flavorTextOffset++;
			}

			/*if (item.AsAnItem().reloadTime > 0)
			{
				double reloadTimeInSeconds = (double)item.AsAnItem().reloadTime / 60.0;
				List<string> reloadKeybind = DissonantDuality.ReloadKeybind.GetAssignedKeys();
				bool reloadKeyBound = reloadKeybind != null && reloadKeybind.Count > 0;
				string reloadTimeText = Language.GetTextValueWith(
					"Mods.DissonantDuality.ItemTooltip.Generic.ReloadTime",
					new
					{
						ReloadKey = reloadKeyBound ? reloadKeybind[0] : "HOTKEY NOT BOUND",
						ReloadTime = reloadTimeInSeconds.CastToDecimalPlaces(1)
					}
				);
				if (V2Utils.FindLastDamageRelatedTooltipLine(tooltips, out TooltipLine lastDamageRelatedLine) && lastDamageRelatedLine != null)
				{
					V2Utils.InsertNewTooltipLine(
						ref tooltips,
						lastDamageRelatedLine,
						1,
						new TooltipLine(Mod, "ReloadTime", reloadTimeText)
					);
				}
				else
					tooltips.Add(new TooltipLine(Mod, "ReloadTime", reloadTimeText));
			}*/

			if (item.wornArmor && player.AsV2Player().setBonusActive)
			{
				TooltipLine setBonusLine = tooltips.FirstOrDefault(x => x.Name == "SetBonus");
				setBonusLine.Hide();
				if (player.AsV2Player().setBonusShouldBeDisplayed && V2Utils.FindFirstTooltipLineThatIsOrComesAfterFlavorText(tooltips, out TooltipLine newSetBonusLineDestination))
				{
					tooltips.Insert(
						tooltips.IndexOf(newSetBonusLineDestination) + 1,
						new TooltipLine(
							Mod,
							"V2SetBonus",
							player.setBonus
						)
						{
							OverrideColor = Color.Gold
						}
					);
				}
			}
		}
	}
}

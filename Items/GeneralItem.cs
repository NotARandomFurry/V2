using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.Items.Voraria.TransformationItems.Baelz;
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

		public bool ShouldSaveSummonWeights { get; set; }
		public IList<double> SavedSummonWeights = new List<double>();
		public IList<bool> InUseSummonWeights = new List<bool>();

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

		public override bool WingUpdate(int wings, Player player, bool inUse) //For transformation item visuals
		{
			if (player.AsV2Player().HasTransformation)
				if (player.AsV2Player().BaeTransformation)
				{
					if (inUse)
					{
						Vector2 velocity = Vector2.Zero;
						Vector2 AngleLeft = Vector2.Zero;
						Vector2 AngleRight = Vector2.Zero;
						float boostX = (Main.GlobalTimeWrappedHourly * 40) % 20;
						if (boostX > 10)
							boostX = 20 - boostX;
						bool DecideIfDust = Main.rand.NextBool(8);
						if (player.direction == 1)
						{
							if (DecideIfDust)
							{
								velocity = new(Main.rand.Next(-125, -49) / 33f, Main.rand.Next(-100, -10) / 33f);
								Dust.NewDustPerfect(player.BottomLeft + new Vector2(0, -3), ModContent.DustType<BaelzDust>(), velocity);
							}
							velocity = new(-boostX, 6);
							AngleLeft = player.BottomLeft.DirectionTo(player.BottomLeft + velocity);
							AngleRight = player.BottomRight.DirectionTo(player.BottomRight + velocity);
							Dust.NewDustPerfect(player.BottomRight + new Vector2(0, -3), ModContent.DustType<BaelzSparkleDustBlack>(), AngleRight * 0.8f);
							Dust.NewDustPerfect(player.BottomLeft + new Vector2(0, -3), ModContent.DustType<BaelzSparkleDustCyan>(), AngleLeft * 0.8f);
							velocity = new(boostX, 6);
							AngleLeft = player.BottomLeft.DirectionTo(player.BottomLeft + velocity);
							AngleRight = player.BottomRight.DirectionTo(player.BottomRight + velocity);
							Dust.NewDustPerfect(player.BottomRight + new Vector2(0, -3), ModContent.DustType<BaelzSparkleDustYellow>(), AngleRight * 0.8f);
							Dust.NewDustPerfect(player.BottomLeft + new Vector2(0, -3), ModContent.DustType<BaelzSparkleDustRed>(), AngleLeft * 0.8f);
						}
						else
						{
							if (DecideIfDust)
							{
								velocity = new(Main.rand.Next(50, 126) / 33f, Main.rand.Next(-100, -10) / 33f);
								Dust.NewDustPerfect(player.BottomRight + new Vector2(0, -3), ModContent.DustType<BaelzDust>(), velocity);
							}
							velocity = new(-boostX, 6);
							AngleLeft = player.BottomLeft.DirectionTo(player.BottomLeft + velocity);
							AngleRight = player.BottomRight.DirectionTo(player.BottomRight + velocity);
							Dust.NewDustPerfect(player.BottomLeft + new Vector2(0, -3), ModContent.DustType<BaelzSparkleDustBlack>(), AngleLeft * 0.8f);
							Dust.NewDustPerfect(player.BottomRight + new Vector2(0, -3), ModContent.DustType<BaelzSparkleDustCyan>(), AngleRight * 0.8f);
							velocity = new(boostX, 6);
							AngleLeft = player.BottomLeft.DirectionTo(player.BottomLeft + velocity);
							AngleRight = player.BottomRight.DirectionTo(player.BottomRight + velocity);
							Dust.NewDustPerfect(player.BottomLeft + new Vector2(0, -3), ModContent.DustType<BaelzSparkleDustYellow>(), AngleLeft * 0.8f);
							Dust.NewDustPerfect(player.BottomRight + new Vector2(0, -3), ModContent.DustType<BaelzSparkleDustRed>(), AngleRight * 0.8f);
						}
					}
					return true;
				} 
			return base.WingUpdate(wings, player, inUse);
		}

		public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
		{
			float weightMovementMult = PredPlayer.WeightMovementMultiplier(player); //(float)Math.Min(1.0, 1.0 / (player.AsPred().StomachWeight + 1.0));
			acceleration *= Math.Min(1.0f, weightMovementMult * 3f);
		}

		public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			float weightMovementMult = PredPlayer.WeightMovementMultiplier(player); //(float)Math.Min(1.0, 1.0 / (player.AsPred().StomachWeight + 1.0));
			ascentWhenFalling *= weightMovementMult / 2f;
			ascentWhenRising *= Math.Min(1.0f, weightMovementMult * 3f);
			maxCanAscendMultiplier *= Math.Min(1.0f, weightMovementMult * 7.5f);
			maxAscentMultiplier *= Math.Min(1.0f, weightMovementMult * 7.5f);
			constantAscend *= Math.Min(1.0f, weightMovementMult * 1.75f);
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

		public override void SaveData(Item item, TagCompound tag)
		{
			if (ShouldSaveSummonWeights)
			{
				tag["SavedWeights"] = SavedSummonWeights;
			}
		}
		public override void LoadData(Item item, TagCompound tag)
		{
			if (ShouldSaveSummonWeights)
			{
				SavedSummonWeights = tag.GetList<double>("SavedWeights");
				foreach (double value in SavedSummonWeights)
				{
					InUseSummonWeights.Add(false);
				}
			}
		}
	}
}

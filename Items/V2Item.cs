using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
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
	public class V2Item : GlobalItem
	{
		public DelegateHeldItemDrawingUI heldItemUIDrawMethod;

		public delegate void DelegateArmorEffectCode(Item item, Player player);
		public DelegateArmorEffectCode ArmorEffectCode { get; internal set; }

		public delegate void DelegateAccessoryEffectCode(Item item, Player player, bool hideVisual);
		public DelegateAccessoryEffectCode AccessoryEffectCode { get; internal set; }

		public delegate void DelegateAccessoryVanityEffectCode(Item item, Player player);
		public DelegateAccessoryVanityEffectCode AccessoryVanityEffectCode { get; internal set; }

		public int ReleasedNPCNetID;

		public override bool InstancePerEntity => true;

		public V2Item()
		{
			heldItemUIDrawMethod = null;

			ReleasedNPCNetID = 0;
		}

		public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
		{
			float weightMovementMult = (float)Math.Min(1.0, 1.0 / (player.AsPred().StomachWeight + 1.0));
			speed *= weightMovementMult;
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

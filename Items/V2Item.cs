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
		public ItemLocation State { get; set; }
		public DelegateHeldItemDrawingUI heldItemUIDrawMethod;

		public int ReleasedNPCNetID;

		public override bool InstancePerEntity => true;

		public V2Item()
		{
			heldItemUIDrawMethod = null;

			ReleasedNPCNetID = 0;
		}

		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			item.AsV2Item().State = ItemLocation.InWorld;
		}

		public override void UpdateInventory(Item item, Player player)
		{
			item.AsV2Item().State = ItemLocation.InPlayerInventory;
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
	}
}

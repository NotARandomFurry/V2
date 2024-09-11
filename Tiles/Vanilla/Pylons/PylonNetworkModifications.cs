using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Map;
using Terraria.ModLoader;
using V2.Core;

namespace V2.Tiles.Vanilla.Pylons
{
	public class PylonNetworkModifications : GlobalPylon
	{
		public override bool PreDrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, ref TeleportPylonInfo pylonInfo, ref bool isNearPylon, ref Color drawColor, ref float deselectedScale, ref float selectedScale)
		{
			if (Main.CurrentPlayer.CurrentCaptor() is not null)
				isNearPylon = false;

			return true;
		}

		public override bool? ValidTeleportCheck_PreAnyDanger(TeleportPylonInfo pylonInfo)
		{
			if (Main.CurrentPlayer.CurrentCaptor() is not null)
				return false;

			return true;
		}
	}
}

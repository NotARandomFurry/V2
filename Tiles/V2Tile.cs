using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Tiles
{
	public class V2Tile : GlobalTile
	{
		public override void FloorVisuals(int type, Player player)
		{
			if (!player.GetModPlayer<V2Player>().StandingTiles.Contains(type))
				player.GetModPlayer<V2Player>().StandingTiles.Add(type);
		}
	}
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items;
using V2.NPCs;
using V2.UI;

namespace V2.PlayerHandling
{
	public partial class V2Player : ModPlayer
	{
		public List<int> StandingTiles { get; set; }
		public bool InTheCold
		{
			get
			{
				if (Player.ZoneSnow)
				{
					return StandingTiles.FindAll(x => x is TileID.Hellstone or TileID.HellstoneBrick or TileID.Meteorite).Count <= 0;
				}
				else
				{
					return StandingTiles.FindAll(x => x is TileID.IceBlock or TileID.SnowBlock).Count > 0;
				}
			}
		}

		public void ResetEnvironmentEffects()
		{
			StandingTiles = [];
		}
	}
}

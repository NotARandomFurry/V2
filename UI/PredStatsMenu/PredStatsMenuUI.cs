using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;

namespace V2.UI.PredStatsMenu
{
	public class PredStatsMenuUI : UIState
	{
		public static bool Visible { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (PredPlayer.GetCurrentBellyWeight(player) > 0 && !player.AsFood().IsCurrentlyEaten)
				Visible = true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;
		}
	}
}
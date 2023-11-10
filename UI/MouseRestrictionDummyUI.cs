using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;
using V2.UI.PredStatsMenu;

namespace V2.UI
{
	public class MouseRestrictionDummyUI : UIState
	{
		private static Asset<Texture2D> _predStatsMenuBackground = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Background", AssetRequestMode.ImmediateLoad);
		public override void Draw(SpriteBatch spriteBatch)
		{
			Player player = Main.LocalPlayer;
			if (player.AsPred().InPredStatsMenu)
			{
				Vector2 backdropPos = new Vector2(
					(Main.screenWidth - _predStatsMenuBackground.Value.Width) / 2,
					(Main.screenHeight - _predStatsMenuBackground.Value.Height) / 2
				);
				Rectangle backdropRect = _predStatsMenuBackground.Value.Bounds;
				backdropRect.X = (int)backdropPos.X;
				backdropRect.Y = (int)backdropPos.Y;
				backdropRect.X += 10;
				backdropRect.Y += 10;
				backdropRect.Width -= 20;
				backdropRect.Height -= 20;

				if (Main.hasFocus)
				{
					MouseState state = Mouse.GetState();
					int mouseX = state.X;
					int mouseY = state.Y;
					if (Main.MouseScreen.X <= backdropRect.Left)
						mouseX = backdropRect.Left;
					if (Main.MouseScreen.X >= backdropRect.Right)
						mouseX = backdropRect.Right;
					if (Main.MouseScreen.Y <= backdropRect.Top)
						mouseY = backdropRect.Top;
					if (Main.MouseScreen.Y >= backdropRect.Bottom)
						mouseY = backdropRect.Bottom;
					Mouse.SetPosition(mouseX, mouseY);
				}
			}
		}
	}
}
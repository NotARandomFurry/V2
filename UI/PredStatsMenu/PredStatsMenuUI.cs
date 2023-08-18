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
using Terraria.UI.Chat;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;

namespace V2.UI.PredStatsMenu
{
	public class PredStatsMenuUI : UIState
	{
		public static bool Visible { get; set; }

		public static bool GoalsMenuOpen { get; set; }

		private static Asset<Texture2D> _predStatsMenuBackground = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Background", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_GLP = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_GLP", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_TUM = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_TUM", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_ACI = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_ACI", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_ABS = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_ABS", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsOverviewPanel = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_StatOverviewPanel", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsExitButton = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Exit", AssetRequestMode.ImmediateLoad);

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (player.AsPred().InPredStatsMenu)
				Visible = true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			Main.LocalPlayer.mouseInterface = true;
			Vector2 backdropPos = new Vector2(
				(Main.screenWidth - _predStatsMenuBackground.Value.Width) / 2,
				(Main.screenHeight - _predStatsMenuBackground.Value.Height) / 2
			);
			spriteBatch.Draw(
				_predStatsMenuBackground.Value,
				backdropPos,
				_predStatsMenuBackground.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);

			Rectangle backdropRect = _predStatsMenuBackground.Value.Bounds;
			backdropRect.X = (int)backdropPos.X;
			backdropRect.Y = (int)backdropPos.Y;

			string hoveredStatSlice = "none";
			#region GLP
			spriteBatch.Draw(
				_predStatsPizzaSlice_GLP.Value,
				backdropPos + new Vector2(50f, 50f),
				_predStatsPizzaSlice_GLP.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);
			Rectangle sliceRectGLP = new Rectangle(
				backdropRect.X + 50,
				backdropRect.Y + 50,
				_predStatsPizzaSlice_GLP.Value.Width,
				_predStatsPizzaSlice_GLP.Value.Height
			);
			if (sliceRectGLP.Contains(Main.MouseScreen.ToPoint()))
				hoveredStatSlice = "GLP";
			#endregion
			#region TUM
			spriteBatch.Draw(
				_predStatsPizzaSlice_TUM.Value,
				backdropPos + new Vector2(86f, 50f),
				_predStatsPizzaSlice_TUM.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);
			Rectangle sliceRectTUM = new Rectangle(
				backdropRect.X + 86,
				backdropRect.Y + 50,
				_predStatsPizzaSlice_TUM.Value.Width,
				_predStatsPizzaSlice_TUM.Value.Height
			);
			if (sliceRectTUM.Contains(Main.MouseScreen.ToPoint()))
				hoveredStatSlice = "TUM";
			#endregion
			#region ACI
			spriteBatch.Draw(
				_predStatsPizzaSlice_ACI.Value,
				backdropPos + new Vector2(50f, 86f),
				_predStatsPizzaSlice_ACI.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);
			Rectangle sliceRectACI = new Rectangle(
				backdropRect.X + 50,
				backdropRect.Y + 86,
				_predStatsPizzaSlice_ACI.Value.Width,
				_predStatsPizzaSlice_ACI.Value.Height
			);
			if (sliceRectACI.Contains(Main.MouseScreen.ToPoint()))
				hoveredStatSlice = "ACI";
			#endregion
			#region ABS
			spriteBatch.Draw(
				_predStatsPizzaSlice_ABS.Value,
				backdropPos + new Vector2(86f, 86f),
				_predStatsPizzaSlice_ABS.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);
			Rectangle sliceRectABS = new Rectangle(
				backdropRect.X + 86,
				backdropRect.Y + 86,
				_predStatsPizzaSlice_ABS.Value.Width,
				_predStatsPizzaSlice_ABS.Value.Height
			);
			if (sliceRectABS.Contains(Main.MouseScreen.ToPoint()))
				hoveredStatSlice = "ABS";
			#endregion

			spriteBatch.Draw(
				_predStatsOverviewPanel.Value,
				backdropPos + new Vector2(20f, 170f),
				_predStatsOverviewPanel.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);
			switch (hoveredStatSlice)
			{
				case "GLP":
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						"Swallow Strength (GLP): [c/FFFF00:" + Main.LocalPlayer.AsPred().GLP.Total + "]",
						backdropPos + new Vector2(20f, 170f) + new Vector2(4f, 4f),
						Color.White,
						0f,
						Vector2.Zero,
						Vector2.One
					);
					break;
				case "TUM":
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						"Stomach Strength (TUM): [c/FFFF00:" + Main.LocalPlayer.AsPred().TUM.Total + "]",
						backdropPos + new Vector2(20f, 170f) + new Vector2(4f, 4f),
						Color.White,
						0f,
						Vector2.Zero,
						Vector2.One
					);
					break;
				case "ACI":
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						"Acid Strength (ACI): [c/FFFF00:" + Main.LocalPlayer.AsPred().ACI.Total + "]",
						backdropPos + new Vector2(20f, 170f) + new Vector2(4f, 4f),
						Color.White,
						0f,
						Vector2.Zero,
						Vector2.One
					);
					break;
				case "ABS":
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						"Absorption Strength (ABS): [c/FFFF00:" + Main.LocalPlayer.AsPred().ABS.Total + "]",
						backdropPos + new Vector2(20f, 170f) + new Vector2(4f, 4f),
						Color.White,
						0f,
						Vector2.Zero,
						Vector2.One
					);
					break;
				default:
					break;
			}

			spriteBatch.Draw(
				_predStatsExitButton.Value,
				backdropPos + new Vector2(300f, 10f),
				_predStatsExitButton.Value.Bounds,
				Color.White,
				0f,
				new Vector2(29, 0),
				1f,
				SpriteEffects.None,
				0f
			);
			Rectangle exitGulletRect = new Rectangle(
				(int)backdropPos.X + 293,
				(int)backdropPos.Y + 20,
				14,
				14
			);
			if (exitGulletRect.Contains(Main.MouseScreen.ToPoint()))
			{
				Main.instance.MouseText(
					"Close the pred stats menu\n"
				  + "(Are you sure your cursor can't stay a little longer?)"
				);
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					PredStatsMenuMouthUI.MouthState = PredStatsMenuMouthState.RegurgitatingCursor;
				}
			}
		}
	}
}
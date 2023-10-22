using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using V2.Core;
using V2.Core.StruggleSystem;
using V2.Items;
using V2.PlayerHandling;

namespace V2.UI.StruggleSystem
{
	public class PlayerPredStruggleUI : UIState
	{
		public static bool Visible { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (PredPlayer.GetCurrentBellyWeight(player, onlyKicky: true) > 0)
				Visible = true;
		}

		private Asset<Texture2D> _struggleSystemBackdrop = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop", AssetRequestMode.ImmediateLoad);

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			VoreTracker tracker = Main.LocalPlayer.AsPred().StomachTracker;
			if (tracker.PredatorChart is null)
				return;

			Vector2 bottomCenter = new Vector2(
				Main.screenWidth / 2,
				Main.screenHeight / 2
			);
			bottomCenter.Y += 52;
			bottomCenter += Main.LocalPlayer.Center - (Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2));

			spriteBatch.Draw(
				_struggleSystemBackdrop.Value,
				bottomCenter,
				_struggleSystemBackdrop.Value.Bounds,
				Color.White,
				0f,
				new Vector2(
					_struggleSystemBackdrop.Value.Bounds.Bottom,
					_struggleSystemBackdrop.Value.Width / 2
				),
				1f,
				SpriteEffects.None,
				0f
			);

			foreach (StruggleChartNote[] noteSpan in tracker.PredatorChart.Notes)
			{
				spriteBatch.Draw(
					_struggleSystemBackdrop.Value,
					bottomCenter,
					_struggleSystemBackdrop.Value.Bounds,
					Color.White,
					0f,
					_struggleSystemBackdrop.Value.Size() / 2f,
					1f,
					SpriteEffects.None,
					0f
				);
			}
		}
	}
}
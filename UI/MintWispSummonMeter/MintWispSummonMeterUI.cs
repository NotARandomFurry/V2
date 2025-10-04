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
using V2.NPCs;
using V2.PlayerHandling;

namespace V2.UI.MintWispSummonMeter
{
	public struct MintSummonMeterSnapshot
	{
		public double Stored;
		public double CapacityMax;

		private int numCapacitySegments;
		private static readonly int minCapacitySegments = 1;
		private static readonly int maxCapacitySegments = 25;
		public int AmountOfCapacitySegments
		{
			get
			{
				if (numCapacitySegments < minCapacitySegments)
					numCapacitySegments = minCapacitySegments;
				if (numCapacitySegments > maxCapacitySegments)
					numCapacitySegments = maxCapacitySegments;
				return numCapacitySegments;
			}
			set => numCapacitySegments = value;
		}

		public MintSummonMeterSnapshot(Player player)
		{
			Stored = player.AsV2Player().MintWispSummonMeter;
			CapacityMax = player.AsV2Player().MintWispSummonMeterMax;
			numCapacitySegments = (int)player.AsV2Player().MintWispSummonMeterMax;
        }
	}

	public class MintWispSummonMeterUI : UIState
	{
		public static bool Visible { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (player.AsV2Player().MintTransformation && player.AsV2Player().MintWispSummonMeter > 0)
				Visible = true;
		}

		private int _wispSegmentsCount;
		private float _capacityPercent;
		private int _wispFilledSegmentsCount;
		private bool _wispCapacityHovered;
		private Asset<Texture2D> _wispCapacityFill = ModContent.Request<Texture2D>("V2/UI/MintWispSummonMeter/MintSummonMeter_FillingPart", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _wispCapacityFilledFill = ModContent.Request<Texture2D>("V2/UI/MintWispSummonMeter/MintSummonMeter_FullPart", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _wispCapacityPanelLeft = ModContent.Request<Texture2D>("V2/UI/MintWispSummonMeter/MintSummonMeter_LeftPanel", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _wispCapacityPanelMiddle = ModContent.Request<Texture2D>("V2/UI/MintWispSummonMeter/MintSummonMeter_MidPanel", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _wispCapacityPanelRight = ModContent.Request<Texture2D>("V2/UI/MintWispSummonMeter/MintSummonMeter_RightPanel", AssetRequestMode.ImmediateLoad);

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			PrepareFields(Main.LocalPlayer);

			Vector2 topLeftCorner = Main.LocalPlayer.Center - Main.screenPosition;
            topLeftCorner.X -= (20 + _wispSegmentsCount * (_wispCapacityPanelMiddle.Value.Width / 2)) * Main.UIScale;
            topLeftCorner.Y += 62 * Main.UIScale * Main.GameZoomTarget;

			topLeftCorner /= Main.UIScale;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            for (int i = 0; i < _wispSegmentsCount; i++)
			{
				spriteBatch.Draw(
                    _wispCapacityPanelMiddle.Value,
					topLeftCorner + new Vector2(28 + (i * _wispCapacityPanelMiddle.Value.Width), 0),
					_wispCapacityPanelMiddle.Value.Bounds,
					Color.White,
					0f,
					default,
					1,
					SpriteEffects.None,
					0f
				);
			}

			for (int i = 0; i < _wispSegmentsCount; i++)
			{
				if (i / (double)_wispSegmentsCount >= _capacityPercent)
					continue;

				Texture2D fillTexture = _wispCapacityFill.Value;
				if (i < _wispFilledSegmentsCount)
					fillTexture = _wispCapacityFilledFill.Value;
				Rectangle fullDrawRect = fillTexture.Bounds;
				if ((i + 1.0) / (double)_wispSegmentsCount > _capacityPercent)
				{
					double fullRatio = (double)i / (double)_wispSegmentsCount;
					fullRatio = _capacityPercent - fullRatio;
					fullRatio *= (double)_wispSegmentsCount;
					fullDrawRect.Width = (int)Math.Ceiling((double)fullDrawRect.Width * fullRatio);
				}
				spriteBatch.Draw(
					fillTexture,
					topLeftCorner + new Vector2(28 + (i * _wispCapacityPanelMiddle.Value.Width), 10),
					fullDrawRect,
					Color.White,
					0f,
					default,
					1,
					SpriteEffects.None,
					0f
				);
			}

			spriteBatch.Draw(
				_wispCapacityPanelLeft.Value,
				topLeftCorner + new Vector2(-4, 0),
				_wispCapacityPanelLeft.Value.Bounds,
				Color.White,
				0f,
				default,
				1,
				SpriteEffects.None,
				0f
			);
			spriteBatch.Draw(
				_wispCapacityPanelRight.Value,
				topLeftCorner + new Vector2(28 + (_wispSegmentsCount * _wispCapacityPanelMiddle.Value.Width), 4),
				_wispCapacityPanelRight.Value.Bounds,
				Color.White,
				0f,
				default,
				1,
				SpriteEffects.None,
				0f
			);

			Rectangle hoverRect = new Rectangle(
				(int)topLeftCorner.X - 8,
				(int)topLeftCorner.Y + 4,
				20 + (_wispSegmentsCount * (_wispCapacityPanelMiddle.Value.Width + 1)) + _wispCapacityPanelRight.Value.Width + 8,
                _wispCapacityPanelMiddle.Value.Height
			);
			_wispCapacityHovered = hoverRect.Contains(Main.MouseScreen.ToPoint());
			if (_wispCapacityHovered && !Main.mouseText && !Main.LocalPlayer.AsPred().InPredStatsMenu)
			{
				Player localPlayer = Main.LocalPlayer;
				localPlayer.cursorItemIconEnabled = false;
				string normalText =
						"Wisp Summon Meter: "
					  + localPlayer.AsV2Player().MintWispSummonMeter.CastToDecimalPlaces(2)
					  + "/"
					  + localPlayer.AsV2Player().MintWispSummonMeterMax.CastToDecimalPlaces(2);
                Main.instance.MouseTextHackZoom(normalText);
                Main.mouseText = true;
			}

            spriteBatch.End();
            spriteBatch.Begin();
        }

		private void PrepareFields(Player player)
		{
            MintSummonMeterSnapshot WispMeterSnapshot = new MintSummonMeterSnapshot(player);

            _wispSegmentsCount = WispMeterSnapshot.AmountOfCapacitySegments;
			_wispFilledSegmentsCount = (int)Math.Floor(player.AsV2Player().MintWispSummonMeter);
            _capacityPercent = (float)(player.AsV2Player().MintWispSummonMeter / player.AsV2Player().MintWispSummonMeterMax);
        }
	}
}
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
using V2.Items;
using V2.PlayerHandling;

namespace V2.UI
{
	public struct PlayerPredStatsSnapshot
	{
		public double BellyWeight;
		public double CapacityMax;
		public double KickyPreyPercentage;

		public double CapacityPerSegment => CapacityMax / (double)AmountOfCapacitySegments;

		private int numCapacitySegments;
		private static readonly int minCapacitySegments = 4;
		private static readonly int maxCapacitySegments = 20;
		/// <summary>
		/// How many segments should be drawn for the stomach capacity bar.<br/>
		/// Has a maximum of 20, similar to health and mana bars.<br/>
		/// </summary>
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

		public PlayerPredStatsSnapshot(Player player)
		{
			BellyWeight = PredPlayer.GetCurrentBellyWeight(player);
			CapacityMax = player.AsPred().StomachCapacity;
			KickyPreyPercentage = PredPlayer.GetCurrentBellyWeight(player, onlyKicky: true) / CapacityMax;

			numCapacitySegments = (int)(CapacityMax / 0.2);
		}
	}

	public class StomachCapacityBarUI : UIState
	{
		public static bool Visible { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (PredPlayer.GetCurrentBellyWeight(player) > 0 && !player.AsFood().IsCurrentlyEaten)
				Visible = true;
		}

		private int _capacitySegmentsCount;
		private double _kickyPreyPercent;
		private float _capacityPercent;
		private bool _stomachCapacityHovered;
		private Asset<Texture2D> _stomachCapacityFill = ModContent.Request<Texture2D>("V2/UI/StomachCapacityBar/StomachCapacityBar_Fill", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachCapacityFillKicky = ModContent.Request<Texture2D>("V2/UI/StomachCapacityBar/StomachCapacityBar_Fill_Kicky", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachCapacityPanelLeft = ModContent.Request<Texture2D>("V2/UI/StomachCapacityBar/StomachCapacityBar_Panel_Left", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachCapacityPanelMiddle = ModContent.Request<Texture2D>("V2/UI/StomachCapacityBar/StomachCapacityBar_Panel_Middle", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachCapacityPanelRight = ModContent.Request<Texture2D>("V2/UI/StomachCapacityBar/StomachCapacityBar_Panel_Right", AssetRequestMode.ImmediateLoad);

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			PrepareFields(Main.LocalPlayer);

			Vector2 topLeftCorner = new Vector2(
				Main.screenWidth / 2,
				Main.screenHeight / 2
			);
			topLeftCorner.X -= 20 + (_capacitySegmentsCount * (_stomachCapacityPanelMiddle.Value.Width / 2));
			topLeftCorner.Y += 32;
			topLeftCorner += Main.LocalPlayer.Center - (Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2));

			for (int i = 0; i < _capacitySegmentsCount; i++)
			{
				spriteBatch.Draw(
					_stomachCapacityPanelMiddle.Value,
					topLeftCorner + new Vector2(20 + (i * _stomachCapacityPanelMiddle.Value.Width), 4),
					_stomachCapacityPanelMiddle.Value.Bounds,
					Color.White
				);
			}

			for (int i = 0; i < _capacitySegmentsCount; i++)
			{
				if ((double)i / (double)_capacitySegmentsCount >= _capacityPercent)
					continue;

				Texture2D fillTexture = _stomachCapacityFill.Value;
				Rectangle fullDrawRect = fillTexture.Bounds;
				if (((double)i + 1.0) / (double)_capacitySegmentsCount > _capacityPercent)
				{
					double fullRatio = (double)i / (double)_capacitySegmentsCount;
					fullRatio = _capacityPercent - fullRatio;
					fullRatio *= (double)_capacitySegmentsCount;
					fullDrawRect.Width = (int)Math.Ceiling((double)fullDrawRect.Width * fullRatio);
				}
				spriteBatch.Draw(
					fillTexture,
					topLeftCorner + new Vector2(20 + (i * _stomachCapacityPanelMiddle.Value.Width), 10),
					fullDrawRect,
					Color.White
				);

				if (_kickyPreyPercent <= 0 || (double)i / (double)_capacitySegmentsCount >= _kickyPreyPercent)
					continue;

				Texture2D kickyFillTexture = _stomachCapacityFillKicky.Value;
				Rectangle kickyDrawRect = kickyFillTexture.Bounds;
				if (((double)i + 1.0) / (double)_capacitySegmentsCount > _kickyPreyPercent)
				{
					double kickyRatio = (double)i / (double)_capacitySegmentsCount;
					kickyRatio = _kickyPreyPercent - kickyRatio;
					kickyRatio *= (double)_capacitySegmentsCount;
					kickyDrawRect.Width = (int)Math.Ceiling((double)kickyDrawRect.Width * kickyRatio);
				}
				spriteBatch.Draw(
					kickyFillTexture,
					topLeftCorner + new Vector2(20 + (i * _stomachCapacityPanelMiddle.Value.Width), 10),
					kickyDrawRect,
					Color.White
				);
			}

			spriteBatch.Draw(
				_stomachCapacityPanelLeft.Value,
				topLeftCorner,
				_stomachCapacityPanelLeft.Value.Bounds,
				Color.White
			);
			spriteBatch.Draw(
				_stomachCapacityPanelRight.Value,
				topLeftCorner + new Vector2(20 + (_capacitySegmentsCount * _stomachCapacityPanelMiddle.Value.Width), 4),
				_stomachCapacityPanelRight.Value.Bounds,
				Color.White
			);
		}

		// Added by TML.
		private PlayerPredStatsSnapshot preparedSnapshot;

		private void PrepareFields(Player player)
		{
			PlayerPredStatsSnapshot PlayerPredStatsSnapshot = new PlayerPredStatsSnapshot(player);

			_capacitySegmentsCount = PlayerPredStatsSnapshot.AmountOfCapacitySegments;
			_kickyPreyPercent = PlayerPredStatsSnapshot.KickyPreyPercentage;
			_capacityPercent = (float)PlayerPredStatsSnapshot.BellyWeight / (float)PlayerPredStatsSnapshot.CapacityMax;

			preparedSnapshot = PlayerPredStatsSnapshot;
		}

		private void CapacityFillingDrawer(int elementIndex, int firstElementIndex, int lastElementIndex, out Asset<Texture2D> sprite, out Vector2 offset, out float drawScale, out Rectangle? sourceRect)
		{
			sprite = _stomachCapacityFill;

			// Make the filling draw from right to left (#HealthManaAPI)
			/*
			if (elementIndex >= _hpSegmentsCount - _hpFruitCount)
			*/
			if ((double)elementIndex / (double)_capacitySegmentsCount < _kickyPreyPercent)
				sprite = _stomachCapacityFillKicky;

			FillBarByValues(elementIndex, sprite, _capacitySegmentsCount, _capacityPercent, out offset, out drawScale, out sourceRect);

			// Make the bar fillings draw from right to left (#HealthManaAPI)
			int opposite = lastElementIndex - (elementIndex - firstElementIndex);
			int drawIndexOffset = opposite - elementIndex;
			offset.X += drawIndexOffset * sprite.Width();
		}

		public static void FillBarByValues(int elementIndex, Asset<Texture2D> sprite, int segmentsCount, float fillPercent, out Vector2 offset, out float drawScale, out Rectangle? sourceRect)
		{
			sourceRect = null;
			offset = Vector2.Zero;
			float num = 1f;
			float num2 = 1f / (float)segmentsCount;

			/*
			float t = 1f - fillPercent;
			*/
			float t = fillPercent;
			float lerpValue = Utils.GetLerpValue(num2 * (float)elementIndex, num2 * (float)(elementIndex + 1), t, clamped: true);

			/*
			num = 1f - lerpValue;
			*/
			num = lerpValue;
			drawScale = 1f;
			Rectangle value = sprite.Frame();
			int num3 = (int)((float)value.Width * (1f - num));
			offset.X += num3;
			value.X += num3;
			value.Width -= num3;
			sourceRect = value;
		}

		public void TryToHover()
		{
			if (_stomachCapacityHovered && !Main.mouseText)
			{
				Player localPlayer = Main.LocalPlayer;
				localPlayer.cursorItemIconEnabled = false;
				string text = localPlayer.statLife + "/" + localPlayer.statLifeMax2;
				Main.instance.MouseTextHackZoom(text);
				Main.mouseText = true;
			}
		}
	}
}
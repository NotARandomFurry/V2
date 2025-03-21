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

namespace V2.UI.StomachCapacityMeter
{
	public struct PlayerPredStomachCapacitySnapshot
	{
		public double Fullness;
		public double CapacityMax;
		public double KickyPreyPercentage;

		private int numCapacitySegments;
		private static readonly int minCapacitySegments = 4;
		private static readonly int maxCapacitySegments = 15;
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

		public PlayerPredStomachCapacitySnapshot(Player player)
		{
			Fullness = player.AsPred().StomachFullness;
			CapacityMax = player.AsPred().StomachCapacity;

			if (CapacityMax == -1)
			{
				KickyPreyPercentage = player.AsPred().KickyStomachFullness / player.AsPred().StomachFullness;

				numCapacitySegments = 10;
			}
			else
			{
				KickyPreyPercentage = player.AsPred().KickyStomachFullness / CapacityMax;

				numCapacitySegments = (int)(CapacityMax / 0.2);
			}
		}
	}

	public class StomachCapacityMeterUI : UIState
	{
		public static bool Visible { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (player.AsPred().StomachFullness > 0.0 && player.CurrentCaptor() is null)
				Visible = true;
		}

		private int _capacitySegmentsCount;
		private double _kickyPreyPercent;
		private float _capacityPercent;
		private bool _stomachCapacityHovered;
		private Asset<Texture2D> _stomachCapacityFill = ModContent.Request<Texture2D>("V2/UI/StomachCapacityMeter/StomachCapacityMeter_Fill", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachCapacityFillKicky = ModContent.Request<Texture2D>("V2/UI/StomachCapacityMeter/StomachCapacityMeter_Fill_Kicky", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachCapacityPanelLeft = ModContent.Request<Texture2D>("V2/UI/StomachCapacityMeter/StomachCapacityMeter_Panel_Left", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachCapacityPanelMiddle = ModContent.Request<Texture2D>("V2/UI/StomachCapacityMeter/StomachCapacityMeter_Panel_Middle", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachCapacityPanelRight = ModContent.Request<Texture2D>("V2/UI/StomachCapacityMeter/StomachCapacityMeter_Panel_Right", AssetRequestMode.ImmediateLoad);

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
			topLeftCorner.Y += 32 * Main.GameZoomTarget;
			topLeftCorner += Main.LocalPlayer.Center - (Main.screenPosition + new Vector2(Main.screenWidth / 2 * Main.UIScale, Main.screenHeight / 2));
			
			topLeftCorner.Y /= Main.UIScale;
			
			for (int i = 0; i < _capacitySegmentsCount; i++)
			{
				spriteBatch.Draw(
					_stomachCapacityPanelMiddle.Value,
					topLeftCorner + new Vector2(20 + (i * _stomachCapacityPanelMiddle.Value.Width), 4),
					_stomachCapacityPanelMiddle.Value.Bounds,
					Color.White,
					0f,
					default,
					Main.UIScale,
					SpriteEffects.None,
					0f
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
					Color.White,
					0f,
					default,
					Main.UIScale,
					SpriteEffects.None,
					0f
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
					Color.White,
					0f,
					default,
					Main.UIScale,
					SpriteEffects.None,
					0f
				);
			}

			spriteBatch.Draw(
				_stomachCapacityPanelLeft.Value,
				topLeftCorner,
				_stomachCapacityPanelLeft.Value.Bounds,
				Color.White,
				0f,
				default,
				Main.UIScale,
				SpriteEffects.None,
				0f
			);
			spriteBatch.Draw(
				_stomachCapacityPanelRight.Value,
				topLeftCorner + new Vector2(20 + (_capacitySegmentsCount * _stomachCapacityPanelMiddle.Value.Width), 4),
				_stomachCapacityPanelRight.Value.Bounds,
				Color.White,
				0f,
				default,
				Main.UIScale,
				SpriteEffects.None,
				0f
			);

			Rectangle hoverRect = new Rectangle(
				(int)topLeftCorner.X,
				(int)topLeftCorner.Y + 4,
				20 + (_capacitySegmentsCount * _stomachCapacityPanelMiddle.Value.Width) + _stomachCapacityPanelRight.Value.Width,
				_stomachCapacityPanelMiddle.Value.Height
			);
			_stomachCapacityHovered = hoverRect.Contains(Main.MouseScreen.ToPoint());
			if (_stomachCapacityHovered && !Main.mouseText && !Main.LocalPlayer.AsPred().InPredStatsMenu)
			{
				Player localPlayer = Main.LocalPlayer;
				localPlayer.cursorItemIconEnabled = false;
				if (localPlayer.AsPred().StomachCapacity == -1)
				{
					string bottomlessText =
						"Stomach Fullness: "
					  + localPlayer.AsPred().StomachFullness.CastToDecimalPlaces(2)
					  + " (Infinite Capacity)"
					  + "\n"
					  + "Stomach Size: "
					  + localPlayer.AsPred().StomachSize;
					Main.instance.MouseTextHackZoom(bottomlessText);
				}
				else
				{
					string normalText =
						"Stomach Fullness: "
					  + localPlayer.AsPred().StomachFullness.CastToDecimalPlaces(2)
					  + "/"
					  + localPlayer.AsPred().StomachCapacity.CastToDecimalPlaces(2)
					  + " ("
					  + (localPlayer.AsPred().StomachFullness / localPlayer.AsPred().StomachCapacity).ToPercentage(2)
					  + ")"
					  + "\n"
					  + "Stomach Size: "
					  + localPlayer.AsPred().StomachSize;
					Main.instance.MouseTextHackZoom(normalText);
				}
				Main.mouseText = true;
			}
		}

		private void PrepareFields(Player player)
		{
			PlayerPredStomachCapacitySnapshot PlayerPredStatsSnapshot = new PlayerPredStomachCapacitySnapshot(player);

			_capacitySegmentsCount = PlayerPredStatsSnapshot.AmountOfCapacitySegments;
			_kickyPreyPercent = PlayerPredStatsSnapshot.KickyPreyPercentage;
			if (PlayerPredStatsSnapshot.CapacityMax == -1)
				_capacityPercent = 1f;
			else
				_capacityPercent = (float)PlayerPredStatsSnapshot.Fullness / (float)PlayerPredStatsSnapshot.CapacityMax;
		}
	}
}
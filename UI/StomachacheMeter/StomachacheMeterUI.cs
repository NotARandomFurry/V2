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

namespace V2.UI.StomachacheMeter
{
	public struct PlayerPredStomachacheSnapshot
	{
		public double Stomachache;
		public double StomachacheMax;

		public double StomachacheValuePerSegment => StomachacheMax / (double)AmountOfStomachacheMeterSegments;

		private int numCapacitySegments;
		private static readonly int minCapacitySegments = 4;
		private static readonly int maxCapacitySegments = 20;
		/// <summary>
		/// How many segments should be drawn for the stomach capacity bar.<br/>
		/// Has a maximum of 20, similar to health and mana bars.<br/>
		/// </summary>
		public int AmountOfStomachacheMeterSegments
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

		public PlayerPredStomachacheSnapshot(Player player)
		{
			Stomachache = player.AsPred().Stomachache;
			StomachacheMax = player.AsPred().StomachacheMeterCapacity;

			numCapacitySegments = (int)(StomachacheMax / 20.0);
		}
	}
	public class StomachacheMeterUI : UIState
	{
		public static bool Visible { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (PredPlayer.GetCurrentBellyWeight(player) > 0 && player.CurrentCaptor() is null)
				Visible = true;
		}

		private int _stomachacheSegments;
		private float _stomachachePercent;
		private bool _stomachacheHovered;
		private Asset<Texture2D> _stomachacheFill = ModContent.Request<Texture2D>("V2/UI/StomachacheMeter/StomachacheMeter_Fill", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachachePanelLeft = ModContent.Request<Texture2D>("V2/UI/StomachacheMeter/StomachacheMeter_Panel_Left", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachachePanelMiddle = ModContent.Request<Texture2D>("V2/UI/StomachacheMeter/StomachacheMeter_Panel_Middle", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachachePanelRight = ModContent.Request<Texture2D>("V2/UI/StomachacheMeter/StomachacheMeter_Panel_Right", AssetRequestMode.ImmediateLoad);

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			PrepareFields(Main.LocalPlayer);

			Vector2 topLeftCorner = new Vector2(
				Main.screenWidth / 2,
				Main.screenHeight / 2
			);
			topLeftCorner.X -= 14 + (_stomachacheSegments * (_stomachachePanelMiddle.Value.Width / 2));
			topLeftCorner.Y -= 40;
			topLeftCorner += Main.LocalPlayer.Center - (Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2));

			for (int i = 0; i < _stomachacheSegments; i++)
			{
				spriteBatch.Draw(
					_stomachachePanelMiddle.Value,
					topLeftCorner + new Vector2(14 + (i * _stomachachePanelMiddle.Value.Width), 6),
					_stomachachePanelMiddle.Value.Bounds,
					Color.White
				);
			}

			for (int i = 0; i < _stomachacheSegments; i++)
			{
				if ((double)i / (double)_stomachacheSegments >= _stomachachePercent)
					continue;

				Texture2D fillTexture = _stomachacheFill.Value;
				Rectangle fullDrawRect = fillTexture.Bounds;
				if (((double)i + 1.0) / (double)_stomachacheSegments > _stomachachePercent)
				{
					double fullRatio = (double)i / (double)_stomachacheSegments;
					fullRatio = _stomachachePercent - fullRatio;
					fullRatio *= (double)_stomachacheSegments;
					fullDrawRect.Width = (int)Math.Ceiling((double)fullDrawRect.Width * fullRatio);
				}
				spriteBatch.Draw(
					fillTexture,
					topLeftCorner + new Vector2(14 + (i * _stomachachePanelMiddle.Value.Width), 6),
					fullDrawRect,
					Color.White
				);
			}

			spriteBatch.Draw(
				_stomachachePanelLeft.Value,
				topLeftCorner,
				_stomachachePanelLeft.Value.Bounds,
				Color.White
			);
			spriteBatch.Draw(
				_stomachachePanelRight.Value,
				topLeftCorner + new Vector2(10 + (_stomachacheSegments * _stomachachePanelMiddle.Value.Width), 0),
				_stomachachePanelRight.Value.Bounds,
				Color.White
			);

			Rectangle hoverRect = new Rectangle(
				(int)topLeftCorner.X,
				(int)topLeftCorner.Y + 4,
				20 + (_stomachacheSegments * _stomachachePanelMiddle.Value.Width) + _stomachachePanelRight.Value.Width,
				_stomachachePanelMiddle.Value.Height
			);
			_stomachacheHovered = hoverRect.Contains(Main.MouseScreen.ToPoint());
			if (_stomachacheHovered && !Main.mouseText)
			{
				Player localPlayer = Main.LocalPlayer;
				localPlayer.cursorItemIconEnabled = false;
				string text =
					"Stomach Weight: "
				  + localPlayer.AsPred().Stomachache.CastToDecimalPlaces(2)
				  + "/"
				  + localPlayer.AsPred().StomachacheMeterCapacity.CastToDecimalPlaces(2)
				  + " ("
				  + (localPlayer.AsPred().Stomachache / localPlayer.AsPred().StomachacheMeterCapacity).ToPercentage(2)
				  + ")";
				Main.instance.MouseTextHackZoom(text);
				Main.mouseText = true;
			}
		}

		private void PrepareFields(Player player)
		{
			PlayerPredStomachacheSnapshot PlayerPredStatsSnapshot = new PlayerPredStomachacheSnapshot(player);

			_stomachacheSegments = PlayerPredStatsSnapshot.AmountOfStomachacheMeterSegments;
			_stomachachePercent = (float)PlayerPredStatsSnapshot.Stomachache / (float)PlayerPredStatsSnapshot.StomachacheMax;
		}
	}
}
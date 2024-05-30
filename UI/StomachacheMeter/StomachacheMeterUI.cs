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
using V2.Projectiles;

namespace V2.UI.StomachacheMeter
{
	public struct PlayerPredStomachacheSnapshot
	{
		public double Stomachache;
		public double StomachacheMax;

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

			if (StomachacheMax == -1)
				numCapacitySegments = 5;
			else
				numCapacitySegments = (int)(StomachacheMax / 20.0);
		}
	}
	public struct NPCPredStomachacheSnapshot
	{
		public double Stomachache;
		public double StomachacheMax;

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

		public NPCPredStomachacheSnapshot(NPC npc)
		{
			Stomachache = npc.AsPred().Stomachache;
			StomachacheMax = npc.AsPred().StomachacheMeterCapacity;

			if (StomachacheMax == -1)
				numCapacitySegments = 5;
			else
				numCapacitySegments = (int)(StomachacheMax / 20.0);
		}
	}
	public struct ProjectilePredStomachacheSnapshot
	{
		public double Stomachache;
		public double StomachacheMax;

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

		public ProjectilePredStomachacheSnapshot(Projectile projectile)
		{
			Stomachache = projectile.AsPred().Stomachache;
			StomachacheMax = projectile.AsPred().StomachacheMeterCapacity;

			if (StomachacheMax == -1)
				numCapacitySegments = 5;
			else
				numCapacitySegments = (int)(StomachacheMax / 20.0);
		}
	}
	public class StomachacheMeterUI : UIState
	{
		public static bool Visible { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = true;
		}

		private int _stomachacheSegments;
		private float _stomachachePercent;
		private bool _stomachacheHovered;
		private double _stomachacheExactCurrent;
		private double _stomachacheExactMax;
		private Asset<Texture2D> _stomachacheFill = ModContent.Request<Texture2D>("V2/UI/StomachacheMeter/StomachacheMeter_Fill", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachachePanelLeft = ModContent.Request<Texture2D>("V2/UI/StomachacheMeter/StomachacheMeter_Panel_Left", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachachePanelMiddle = ModContent.Request<Texture2D>("V2/UI/StomachacheMeter/StomachacheMeter_Panel_Middle", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _stomachachePanelRight = ModContent.Request<Texture2D>("V2/UI/StomachacheMeter/StomachacheMeter_Panel_Right", AssetRequestMode.ImmediateLoad);

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			void Draw(Entity pred)
			{
				if (pred is not Player && _stomachacheExactMax == -1)
					return;

				if (_stomachacheExactCurrent <= 0)
					return;

				Vector2 topLeftCorner = new Vector2(
					Main.screenWidth / 2,
					Main.screenHeight / 2
				);
				topLeftCorner.X -= 14 + (_stomachacheSegments * (_stomachachePanelMiddle.Value.Width / 2));
				topLeftCorner.Y -= 40 * Main.GameZoomTarget;
				topLeftCorner += pred.Center - (Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2));

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
						topLeftCorner + new Vector2(14 + (i * _stomachachePanelMiddle.Value.Width), 8),
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
				if (_stomachacheHovered && !Main.mouseText && !Main.LocalPlayer.AsPred().InPredStatsMenu)
				{
					Main.LocalPlayer.cursorItemIconEnabled = false;
					if (_stomachacheExactMax == -1)
					{
						string bottomlessText = "Stomach Unease: 0 (and it will stay that way)";
						Main.instance.MouseTextHackZoom(bottomlessText);
					}
					else
					{
						string normalText =
							"Stomach Unease: "
						  + _stomachacheExactCurrent.CastToDecimalPlaces(2)
						  + "/"
						  + _stomachacheExactMax.CastToDecimalPlaces(2)
						  + " ("
						  + (_stomachacheExactCurrent / _stomachacheExactMax).ToPercentage(2)
						  + ")";
						Main.instance.MouseTextHackZoom(normalText);
					}
					Main.mouseText = true;
				}
			}

			foreach (Player player in Main.ActivePlayers)
			{
				PrepareFields(player);
				Draw(player);
			}

			foreach (NPC npc in Main.ActiveNPCs)
			{
				PrepareFields(npc);
				Draw(npc);
			}

			foreach (Projectile projectile in Main.ActiveProjectiles)
			{
				PrepareFields(projectile);
				Draw(projectile);
			}
		}

		private void PrepareFields(Player player)
		{
			PlayerPredStomachacheSnapshot PlayerPredStatsSnapshot = new PlayerPredStomachacheSnapshot(player);

			_stomachacheSegments = PlayerPredStatsSnapshot.AmountOfStomachacheMeterSegments;
			if (PlayerPredStatsSnapshot.StomachacheMax == -1)
				_stomachachePercent = 0f;
			else
				_stomachachePercent = (float)PlayerPredStatsSnapshot.Stomachache / (float)PlayerPredStatsSnapshot.StomachacheMax;

			_stomachacheExactCurrent = PlayerPredStatsSnapshot.Stomachache;
			_stomachacheExactMax = PlayerPredStatsSnapshot.StomachacheMax;
		}

		private void PrepareFields(NPC npc)
		{
			NPCPredStomachacheSnapshot NPCPredStatsSnapshot = new NPCPredStomachacheSnapshot(npc);

			_stomachacheSegments = NPCPredStatsSnapshot.AmountOfStomachacheMeterSegments;
			if (NPCPredStatsSnapshot.StomachacheMax == -1)
				_stomachachePercent = 0f;
			else
				_stomachachePercent = (float)NPCPredStatsSnapshot.Stomachache / (float)NPCPredStatsSnapshot.StomachacheMax;

			_stomachacheExactCurrent = NPCPredStatsSnapshot.Stomachache;
			_stomachacheExactMax = NPCPredStatsSnapshot.StomachacheMax;
		}

		private void PrepareFields(Projectile projectile)
		{
			ProjectilePredStomachacheSnapshot NPCPredStatsSnapshot = new ProjectilePredStomachacheSnapshot(projectile);

			_stomachacheSegments = NPCPredStatsSnapshot.AmountOfStomachacheMeterSegments;
			if (NPCPredStatsSnapshot.StomachacheMax == -1)
				_stomachachePercent = 0f;
			else
				_stomachachePercent = (float)NPCPredStatsSnapshot.Stomachache / (float)NPCPredStatsSnapshot.StomachacheMax;

			_stomachacheExactCurrent = NPCPredStatsSnapshot.Stomachache;
			_stomachacheExactMax = NPCPredStatsSnapshot.StomachacheMax;
		}
	}
}
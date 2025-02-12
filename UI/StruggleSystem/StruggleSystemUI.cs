using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using V2.Core;
using V2.Core.StruggleSystem;
using V2.PlayerHandling;

namespace V2.UI.StruggleSystem
{
	public class StruggleSystemUI : UIState
	{
		public static bool Visible { get; set; }
		private static int _opacity;
		public static int Opacity {
			get => _opacity;
			set => _opacity = Math.Max(Math.Min(255, value), 0);
		}
		public static int ActiveTimer { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			if (Main.netMode != NetmodeID.SinglePlayer)
				return;

			Player player = Main.LocalPlayer;
			if (player.AsPred().KickyStomachFullness > 0.0 && player.AsPred().StomachTracker.PredatorStruggleChart is not null)
				Visible = true;
		}

		private Asset<Texture2D> _struggleSystemBackdropHoriz = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_Horizontal", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleSystemBackdropVerti = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_Vertical", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteUp = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_UpNote", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteLeft = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_LeftNote", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteSpecial = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_UpNote", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteRight = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_RightNote", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteDown = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_DownNote", AssetRequestMode.ImmediateLoad);

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				Opacity += 15;
				ActiveTimer++;
			}
			else
			{
				Opacity -= 15;
				if (Opacity <= 0)
					ActiveTimer = 0;
			}
			Vector2 bottomCenter = new Vector2(
				Main.screenWidth / 2,
				Main.screenHeight / 2
			);
			bottomCenter.Y -= 55 * Main.GameZoomTarget;
			bottomCenter += Main.LocalPlayer.Center - (Main.screenPosition + new Vector2(Main.screenWidth / 2 * Main.UIScale, Main.screenHeight / 2));

			bottomCenter.Y /= Main.UIScale;
			
			spriteBatch.Draw(
				_struggleSystemBackdropHoriz.Value,
				bottomCenter,
				_struggleSystemBackdropHoriz.Value.Bounds,
				Color.White,
				0f,
				new Vector2(
					_struggleSystemBackdropHoriz.Value.Bounds.Bottom,
					_struggleSystemBackdropHoriz.Value.Width / 2
				),
				Main.UIScale,
				SpriteEffects.None,
				0f
			);

			if (ActiveTimer >= 150)
				goto SkipLaneIdentifiers;

			SkipLaneIdentifiers:
			// this is the cutoff point for today's struggle system work
			return;
			VoreTracker tracker = Main.LocalPlayer.AsPred().StomachTracker;
			foreach ((StruggleChartNote note, double proximity) noteData in tracker.CheckCloseNotes(-1, true))
			{
				if (noteData.note.CorrectlyPressed && noteData.note.PressAnimTimer >= 28)
					continue;
				float alpha = 1f;
				if (!noteData.note.CorrectlyPressed)
				{
					if (noteData.proximity >= 0)
					{
						double realProximity = 2.5 - noteData.proximity;
						if (realProximity < 0.0)
							realProximity = 0.0;
						alpha = (float)Math.Min(Math.Max(realProximity / 2.5, 0.0), 1.0);
					}
					else if (noteData.proximity < 0)
					{
						double realProximity = 0.5 + noteData.proximity;
						if (realProximity < 0.0)
							realProximity = 0.0;
						alpha = (float)Math.Min(Math.Max(realProximity / 0.5, 0.0), 1.0);
					}
				}
				Vector2 notePosition = bottomCenter;
				notePosition.X -= 16;
				notePosition.X += noteData.note.Direction switch
				{
					NoteDirection.Up => -48,
					NoteDirection.Left => -24,
					NoteDirection.Special => 0,
					NoteDirection.Right => 24,
					NoteDirection.Down => 48,
					_ => 0,
				};
				notePosition.Y -= (float)((noteData.note.CorrectlyPressed ? noteData.note.PressedPosition : noteData.proximity) * 26.0) * 1.5f;

				int frame = 0;
				if (noteData.note.PressAnimTimer > 7)
					frame = 1;
				if (noteData.note.PressAnimTimer > 14)
					frame = 2;
				if (noteData.note.PressAnimTimer > 21)
					frame = 3;
				Rectangle noteFrame = new Rectangle(
					frame * 28,
					0,
					26,
					26
				);

				Texture2D noteTexture = noteData.note.Direction switch
				{
					NoteDirection.Up => _struggleNoteUp.Value,
					NoteDirection.Left => _struggleNoteLeft.Value,
					NoteDirection.Special => _struggleNoteSpecial.Value,
					NoteDirection.Right => _struggleNoteRight.Value,
					NoteDirection.Down => _struggleNoteDown.Value,
					_ => null,
				};
				spriteBatch.Draw(
					noteTexture,
					notePosition,
					noteFrame,
					Color.White * alpha,
					0f,
					noteFrame.Size() / 2f,
					1f,
					SpriteEffects.None,
					0f
				);
			}
		}
	}
}
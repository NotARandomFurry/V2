using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using V2.Core;
using V2.Core.StruggleSystem;
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
			if (player.AsPred().KickyStomachFullness > 0.0 && player.AsPred().StomachTracker.PredatorStruggleChart is not null)
				Visible = true;
		}

		private Asset<Texture2D> _struggleSystemBackdrop = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteUp = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_UpNote", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteLeft = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_LeftNote", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteSpecial = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_UpNote", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteRight = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_RightNote", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteDown = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_DownNote", AssetRequestMode.ImmediateLoad);

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			Vector2 bottomCenter = new Vector2(
				Main.screenWidth / 2,
				Main.screenHeight / 2
			);
			bottomCenter.X += 16;
			bottomCenter.X += 60;
			bottomCenter.Y -= 52;
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
				notePosition.X += noteData.note.Lane switch
				{
					NoteLane.Up => -48,
					NoteLane.Left => -24,
					NoteLane.Special => 0,
					NoteLane.Right => 24,
					NoteLane.Down => 48,
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

				Texture2D noteTexture = noteData.note.Lane switch
				{
					NoteLane.Up => _struggleNoteUp.Value,
					NoteLane.Left => _struggleNoteLeft.Value,
					NoteLane.Special => _struggleNoteSpecial.Value,
					NoteLane.Right => _struggleNoteRight.Value,
					NoteLane.Down => _struggleNoteDown.Value,
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
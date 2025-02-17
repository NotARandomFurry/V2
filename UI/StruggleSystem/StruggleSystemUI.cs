using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stubble.Core.Imported;
using System;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using V2.Core;
using V2.Core.StruggleSystem;
using V2.PlayerHandling;

namespace V2.UI.StruggleSystem
{
	public class StruggleSystemUI : UIState
	{
		public static bool Visible { get; set; }
		public static bool PlayerIsPred { get; set; }
		public static bool PlayerIsPudge { get; set; }
		private static int _opacity;
		public static int Opacity {
			get => _opacity;
			set => _opacity = Math.Max(Math.Min(255, value), 0);
		}
		public static int ActiveTimer { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			PlayerIsPred = false;
			PlayerIsPudge = false;
			/*
			just because I'm overhaulin' this system
			to be more streamlined and actually work correctly
			does NOT mean I have to make it work in MP yet
			*/
			if (Main.netMode != NetmodeID.SinglePlayer)
				return;

			Player player = Main.LocalPlayer;
			if (player.CurrentCaptor() is not null)// && player.CurrentCaptor().PredatorStruggleChart is not null)
			{
				Visible = true;
				PlayerIsPudge = true;
			}
			else if (player.AsPred().KickyStomachFullness > 0.0)// && player.AsPred().StomachTracker.PredatorStruggleChart is not null)
			{
				Visible = true;
				PlayerIsPred = true;
			}
		}

		private Asset<Texture2D> _struggleSystemBackdropHoriz = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_Horizontal", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleSystemBackdropVerti = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_Vertical", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteUp = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteLeft = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteSpecial = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteRight = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);
		private Asset<Texture2D> _struggleNoteDown = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);

		public enum StruggleUIOrientation
		{
			Horizontal,
			Vertical,
		};

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				Opacity += 8;
				ActiveTimer++;
			}
			else
			{
				Opacity -= 8;
				if (Opacity <= 0)
					ActiveTimer = 0;
			}

			if (Opacity <= 0)
				return;

			Vector2 bottomCenter = Main.LocalPlayer.position - Main.screenPosition;
			bottomCenter.Y -= 55 * Main.GameZoomTarget;

			bottomCenter.Y /= Main.UIScale;

			switch (ModContent.GetInstance<V2ClientConfig>().StruggleSystemBackdropOrientation)
			{
				case StruggleUIOrientation.Horizontal:
					spriteBatch.Draw(
						_struggleSystemBackdropHoriz.Value,
						bottomCenter,
						_struggleSystemBackdropHoriz.Value.Bounds,
						Color.White * ((float)Opacity / 255f),
						0f,
						new Vector2(
							_struggleSystemBackdropHoriz.Value.Bounds.Bottom,
							_struggleSystemBackdropHoriz.Value.Bounds.Width / 2
						),
						Main.UIScale,
						SpriteEffects.None,
						0f
					);
					break;
				case StruggleUIOrientation.Vertical:
					spriteBatch.Draw(
						_struggleSystemBackdropVerti.Value,
						bottomCenter,
						_struggleSystemBackdropVerti.Value.Bounds,
						Color.White * ((float)Opacity / 255f),
						0f,
						new Vector2(
							_struggleSystemBackdropVerti.Value.Bounds.Bottom,
							_struggleSystemBackdropVerti.Value.Bounds.Width / 2
						),
						Main.UIScale,
						SpriteEffects.None,
						0f
					);
					break;
			}

			if (ActiveTimer >= 150)
				goto SkipLaneIdentifiers;

			double laneIdentifierDisplayOffset = ActiveTimer;
			laneIdentifierDisplayOffset -= 75.0;
			laneIdentifierDisplayOffset /= 150.0;
			laneIdentifierDisplayOffset = Math.Pow(laneIdentifierDisplayOffset, 2.0);
			laneIdentifierDisplayOffset *= 250.0;

			float laneIdentifierTextScale = 1.2f;
			Vector2 predLaneIdentifierDisplayLocation = bottomCenter;
			predLaneIdentifierDisplayLocation.X -= (float)laneIdentifierDisplayOffset;
			Vector2 preyLaneIdentifierDisplayLocation = bottomCenter;
			preyLaneIdentifierDisplayLocation.X += (float)laneIdentifierDisplayOffset;
			Vector2 predLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.ThisIsThePredLane"), new Vector2(laneIdentifierTextScale));
			Vector2 preyLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.ThisIsThePreyLane"), new Vector2(laneIdentifierTextScale));
			float laneIdentifierAlpha = (ActiveTimer < 75 ? ActiveTimer : 150 - ActiveTimer) / 75f;

			switch (ModContent.GetInstance<V2ClientConfig>().StruggleSystemBackdropOrientation)
			{
				case StruggleUIOrientation.Horizontal:
					predLaneIdentifierDisplayLocation.X -= 80f;
					predLaneIdentifierDisplayLocation.X += 30f;
					predLaneIdentifierDisplayLocation.Y -= 80f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.ThisIsThePredLane"),
						predLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						predLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					preyLaneIdentifierDisplayLocation.X += 80f;
					preyLaneIdentifierDisplayLocation.X += 30f;
					preyLaneIdentifierDisplayLocation.Y -= 40f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.ThisIsThePreyLane"),
						preyLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						preyLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					break;
				case StruggleUIOrientation.Vertical:
					predLaneIdentifierDisplayLocation.X -= 105f;
					predLaneIdentifierDisplayLocation.Y -= 40f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.ThisIsThePredLane"),
						predLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						predLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					preyLaneIdentifierDisplayLocation.X += 105f;
					preyLaneIdentifierDisplayLocation.Y -= 40f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.ThisIsThePreyLane"),
						preyLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						preyLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					break;
			}

			SkipLaneIdentifiers:
			// this is the cutoff point for current struggle system work
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
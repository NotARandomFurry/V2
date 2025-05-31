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
		/// <summary>
		/// Whether or not the struggle system's UI is currently visible at all.
		/// </summary>
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

		private static readonly Asset<Texture2D> _struggleSystemBackdropHoriz = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_Horizontal", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleSystemBackdropHorizFlip = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_HorizontalFlipped", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleSystemBackdropUpscroll = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_FNF", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleSystemBackdropDownscroll = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_GuitarHero", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleNoteUp = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleNoteLeft = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleNoteSpecial = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleNoteRight = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleNoteDown = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);

		public enum StruggleUIOrientation
		{
			Horizontal,
			HorizontalFlipped,
			FNF,
			GuitarHero,
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

			Vector2 topLeftCorner = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
			topLeftCorner += Main.LocalPlayer.Center - (Main.screenPosition + new Vector2(Main.screenWidth / 2 * Main.UIScale, Main.screenHeight / 2));
			topLeftCorner.Y -= 50 * Main.GameZoomTarget;

			topLeftCorner.Y /= Main.UIScale;

			switch (ModContent.GetInstance<V2ClientConfig>().StruggleSystemBackdropOrientation)
			{
				case StruggleUIOrientation.Horizontal:
					topLeftCorner.Y -= _struggleSystemBackdropHoriz.Height();
					spriteBatch.Draw(
						_struggleSystemBackdropHoriz.Value,
						topLeftCorner - new Vector2(_struggleSystemBackdropHoriz.Width() / 2f, 0f),
						_struggleSystemBackdropHoriz.Value.Bounds,
						Color.White * ((float)Opacity / 255f),
						0f,
						default,
						Main.UIScale,
						SpriteEffects.None,
						0f
					);
					break;
				case StruggleUIOrientation.HorizontalFlipped:
					topLeftCorner.Y -= _struggleSystemBackdropHorizFlip.Height();
					spriteBatch.Draw(
						_struggleSystemBackdropHorizFlip.Value,
						topLeftCorner - new Vector2(_struggleSystemBackdropHorizFlip.Width() / 2f, 0f),
						_struggleSystemBackdropHorizFlip.Value.Bounds,
						Color.White * ((float)Opacity / 255f),
						0f,
						default,
						Main.UIScale,
						SpriteEffects.None,
						0f
					);
					break;
				case StruggleUIOrientation.FNF:
					topLeftCorner.Y -= _struggleSystemBackdropUpscroll.Height();
					spriteBatch.Draw(
						_struggleSystemBackdropUpscroll.Value,
						topLeftCorner - new Vector2(_struggleSystemBackdropUpscroll.Width() / 2f, 0f),
						_struggleSystemBackdropUpscroll.Value.Bounds,
						Color.White * ((float)Opacity / 255f),
						0f,
						default,
						Main.UIScale,
						SpriteEffects.None,
						0f
					);
					break;
				case StruggleUIOrientation.GuitarHero:
					topLeftCorner.Y -= _struggleSystemBackdropDownscroll.Height();
					spriteBatch.Draw(
						_struggleSystemBackdropDownscroll.Value,
						topLeftCorner - new Vector2(_struggleSystemBackdropDownscroll.Width() / 2f, 0f),
						_struggleSystemBackdropDownscroll.Value.Bounds,
						Color.White * ((float)Opacity / 255f),
						0f,
						default,
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
			Vector2 predLaneIdentifierDisplayLocation = topLeftCorner;
			predLaneIdentifierDisplayLocation.X -= (float)laneIdentifierDisplayOffset;
			Vector2 preyLaneIdentifierDisplayLocation = topLeftCorner;
			preyLaneIdentifierDisplayLocation.X += (float)laneIdentifierDisplayOffset;
			Vector2 predLaneStringSize;
			Vector2 preyLaneStringSize;
			float laneIdentifierAlpha = (ActiveTimer < 75 ? ActiveTimer : 150 - ActiveTimer) / 75f;

			switch (ModContent.GetInstance<V2ClientConfig>().StruggleSystemBackdropOrientation)
			{
				case StruggleUIOrientation.Horizontal:
					predLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.Horiz.ThisIsThePredLane"), new Vector2(laneIdentifierTextScale));
					predLaneIdentifierDisplayLocation.X -= 80f;
					predLaneIdentifierDisplayLocation.Y -= 30f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.Horiz.ThisIsThePredLane"),
						predLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						predLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					preyLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.Horiz.ThisIsThePreyLane"), new Vector2(laneIdentifierTextScale));
					preyLaneIdentifierDisplayLocation.X += 80f;
					preyLaneIdentifierDisplayLocation.Y -= 30f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.Horiz.ThisIsThePreyLane"),
						preyLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						preyLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					break;
				case StruggleUIOrientation.HorizontalFlipped:
					predLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.HorizFlip.ThisIsThePredLane"), new Vector2(laneIdentifierTextScale));
					predLaneIdentifierDisplayLocation.X -= 80f;
					predLaneIdentifierDisplayLocation.Y -= 30f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.HorizFlip.ThisIsThePredLane"),
						predLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						predLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					preyLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.HorizFlip.ThisIsThePreyLane"), new Vector2(laneIdentifierTextScale));
					preyLaneIdentifierDisplayLocation.X += 80f;
					preyLaneIdentifierDisplayLocation.Y -= 30f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.HorizFlip.ThisIsThePreyLane"),
						preyLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						preyLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					break;
				case StruggleUIOrientation.FNF:
					predLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.FNF.ThisIsThePredLane"), new Vector2(laneIdentifierTextScale));
					predLaneIdentifierDisplayLocation.X -= 80f;
					predLaneIdentifierDisplayLocation.Y -= 30f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.FNF.ThisIsThePredLane"),
						predLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						predLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					preyLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.FNF.ThisIsThePreyLane"), new Vector2(laneIdentifierTextScale));
					preyLaneIdentifierDisplayLocation.X += 80f;
					preyLaneIdentifierDisplayLocation.Y -= 30f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.FNF.ThisIsThePreyLane"),
						preyLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						preyLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					break;
				case StruggleUIOrientation.GuitarHero:
					predLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.GuitarHero.ThisIsThePredLane"), new Vector2(laneIdentifierTextScale));
					predLaneIdentifierDisplayLocation.X -= 80f;
					predLaneIdentifierDisplayLocation.Y -= 30f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.GuitarHero.ThisIsThePredLane"),
						predLaneIdentifierDisplayLocation,
						Color.Yellow * laneIdentifierAlpha,
						0f,
						predLaneStringSize / 2f,
						new Vector2(laneIdentifierTextScale)
					);
					preyLaneStringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.StruggleSystem.GuitarHero.ThisIsThePreyLane"), new Vector2(laneIdentifierTextScale));
					preyLaneIdentifierDisplayLocation.X += 80f;
					preyLaneIdentifierDisplayLocation.Y -= 30f;
					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValue("Mods.V2.StruggleSystem.GuitarHero.ThisIsThePreyLane"),
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
				Vector2 notePosition = topLeftCorner;
				notePosition.X -= 16;
				notePosition.X += noteData.note.Direction switch
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

				Texture2D noteTexture = noteData.note.Direction switch
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
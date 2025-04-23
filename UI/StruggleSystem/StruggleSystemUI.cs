using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stubble.Core.Imported;
using System;
using System.Security.Cryptography;
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
		/// Whether or not the struggle system's UI is currently visible at all.<br/>
		/// </summary>
		public static bool Visible { get; set; }
		/// <summary>
		/// Whether or not the local player is currently the pred in this scenario.<br/>
		///	Influences which side of the struggle UI belongs to the player.<br/>
		/// </summary>
		public static bool PlayerIsPred { get; set; }
		private static int _opacity;
		public static int Opacity
		{
			get => _opacity;
			set => _opacity = Math.Max(Math.Min(255, value), 0);
		}
		public static int ActiveTimer { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			PlayerIsPred = false;
			/*
			just because I'm overhaulin' this system
			to be more streamlined and actually work correctly
			does NOT mean I have to make it work in MP yet
			*/
			if (Main.netMode != NetmodeID.SinglePlayer)
				return;

			Player player = Main.LocalPlayer;
			if (player.CurrentCaptor() is not null && player.CurrentCaptor().PredatorStruggleChart is not null)
			{
				Visible = true;
				PlayerIsPred = false;
			}
			else if (player.AsPred().KickyStomachFullness > 0.0 && player.AsPred().StomachTracker.PredatorStruggleChart is not null)
			{
				Visible = true;
				PlayerIsPred = true;
			}
		}

		private static readonly Asset<Texture2D> _struggleSystemBackdropHoriz = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_Horizontal", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleSystemBackdropHorizFlip = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_HorizontalFlipped", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleSystemBackdropUpscroll = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_FNF", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleSystemBackdropDownscroll = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_NoteBackdrop_GuitarHero", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _struggleNotesSheet = ModContent.Request<Texture2D>("V2/UI/StruggleSystem/StruggleSystem_Main_Notes", AssetRequestMode.ImmediateLoad);

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

			topLeftCorner.X = (int)Math.Round(topLeftCorner.X);
			topLeftCorner.Y = (int)Math.Round(topLeftCorner.Y);

			switch (ModContent.GetInstance<V2ClientConfig>().StruggleSystemBackdropOrientation)
			{
				case StruggleUIOrientation.Horizontal:
					topLeftCorner.Y -= _struggleSystemBackdropHoriz.Height();
					spriteBatch.Draw(
						_struggleSystemBackdropHoriz.Value,
						topLeftCorner - new Vector2((int)Math.Round(_struggleSystemBackdropHoriz.Width() / 2f), 0f),
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
						topLeftCorner - new Vector2((int)Math.Round(_struggleSystemBackdropHorizFlip.Width() / 2f), 0f),
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
					topLeftCorner.Y -= 20;
					spriteBatch.Draw(
						_struggleSystemBackdropUpscroll.Value,
						topLeftCorner - new Vector2((int)Math.Round(_struggleSystemBackdropUpscroll.Width() / 2f), 0f),
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
					topLeftCorner.Y -= 20;
					spriteBatch.Draw(
						_struggleSystemBackdropDownscroll.Value,
						topLeftCorner - new Vector2((int)Math.Round(_struggleSystemBackdropDownscroll.Width() / 2f), 0f),
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
			if (!Visible)
				return;

			VoreTracker tracker = PlayerIsPred ? Main.LocalPlayer.AsPred().StomachTracker : Main.LocalPlayer.CurrentCaptor();
			if (tracker.PredatorStruggleChart is null)
				return;

			if (PlayerIsPred)
			{
				DrawArrowsForSubject(-1);
				DrawArrowsForSubject(0);
			}
			else
			{
				DrawArrowsForSubject(-1);
				DrawArrowsForSubject(tracker.Prey.FindIndex(x => x.Instance == Main.LocalPlayer));
			}

			void DrawArrowsForSubject(int index)
			{
				foreach ((StruggleChartNote note, double proximity) noteData in tracker.CheckCloseNotes(index, true))
				{
					if (noteData.note.CorrectlyPressed && noteData.note.PressAnimTimer >= 28)
						continue;
					float alpha = 1f;
					if (!noteData.note.CorrectlyPressed)
					{
						if (noteData.proximity >= 0)
						{
							double visProximity = 2.5 - Math.Min(noteData.proximity - 1.0, 2.5);
							if (visProximity < 0.0)
								visProximity = 0.0;
							alpha = (float)Math.Min(Math.Max(visProximity / 2.5, 0.0), 1.0);
						}
						else if (noteData.proximity < 0)
						{
							double visProximity = 0.5 + noteData.proximity;
							if (visProximity < 0.0)
								visProximity = 0.0;
							alpha = (float)Math.Min(Math.Max(visProximity / 0.5, 0.0), 1.0);
						}
					}
					Vector2 notePosition = topLeftCorner + new Vector2(0, 13);
					double noteSpacingFactor = index == -1 ? tracker.PredatorStruggleChart.NoteSpacingFactor : tracker.Prey[index].AssignedStruggleChart.NoteSpacingFactor;
					float noteOffsetByProximity = 20 + (float)Math.Round((noteData.note.CorrectlyPressed ? noteData.note.PressedPosition : noteData.proximity) * 26.0 * noteSpacingFactor);
					switch (ModContent.GetInstance<V2ClientConfig>().StruggleSystemBackdropOrientation)
					{
						case StruggleUIOrientation.Horizontal:
							notePosition.Y += 6;
							notePosition.Y += noteData.note.Direction switch
							{
								NoteDirection.Up => 0,
								NoteDirection.Right => 36,
								NoteDirection.Special => 72,
								NoteDirection.Left => 108,
								NoteDirection.Down => 144,
								_ => 0,
							};
							notePosition.X += (index == -1 ? 1f : -1f) * noteOffsetByProximity;
							break;
						case StruggleUIOrientation.HorizontalFlipped:
							notePosition.Y += 6;
							notePosition.Y += noteData.note.Direction switch
							{
								NoteDirection.Up => 0,
								NoteDirection.Right => 36,
								NoteDirection.Special => 72,
								NoteDirection.Left => 108,
								NoteDirection.Down => 144,
								_ => 0,
							};
							notePosition.X -= (index == -1 ? 1f : -1f) * noteOffsetByProximity;
							break;
						case StruggleUIOrientation.FNF:
							notePosition.X += (index == -1 ? 1f : -1f) * 20;
							notePosition.Y -= 13;
							notePosition.X += noteData.note.Direction switch
							{
								NoteDirection.Left => index == -1 ? 0 : -144,
								NoteDirection.Down => index == -1 ? 36 : -108,
								NoteDirection.Special => index == -1 ? 72 : -72,
								NoteDirection.Up => index == -1 ? 108 : -36,
								NoteDirection.Right => index == -1 ? 144 : 0,
								_ => 0,
							};
							notePosition.Y += noteOffsetByProximity;
							break;
						case StruggleUIOrientation.GuitarHero:
							notePosition.X += (index == -1 ? 1f : -1f) * 20;
							notePosition.Y += _struggleSystemBackdropDownscroll.Height();
							notePosition.Y -= 13;
							notePosition.X += noteData.note.Direction switch
							{
								NoteDirection.Left => index == -1 ? 0 : -144,
								NoteDirection.Up => index == -1 ? 36 : -108,
								NoteDirection.Special => index == -1 ? 72 : -72,
								NoteDirection.Down => index == -1 ? 108 : -36,
								NoteDirection.Right => index == -1 ? 144 : 0,
								_ => 0,
							};
							notePosition.Y -= noteOffsetByProximity;
							break;
					}

					Texture2D notesTexture = _struggleNotesSheet.Value;
					Rectangle noteRect = noteData.note.Direction switch
					{
						NoteDirection.Up => new(26, 0, 26, 26),
						NoteDirection.Left => new(0, 26, 26, 26),
						NoteDirection.Special => new(26, 26, 26, 26),
						NoteDirection.Right => new(52, 26, 26, 26),
						NoteDirection.Down => new(26, 52, 26, 26),
						_ => new(26, 26, 26, 26),
					};
					Color colorToUse = Color.White;
					if (Math.Abs(noteData.proximity) <= (index == -1 ? tracker.PredatorStruggleChart.ProgressRate : tracker.Prey[index].AssignedStruggleChart.ProgressRate) / 10f)
						colorToUse = Color.Gray;
					spriteBatch.Draw(
						notesTexture,
						notePosition,
						noteRect,
						colorToUse * alpha,
						0f,
						noteRect.Size() / 2f,
						Main.UIScale,
						SpriteEffects.None,
						0f
					);
				}
			}
		}
	}
}
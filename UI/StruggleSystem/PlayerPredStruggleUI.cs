using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using V2.Core;
using V2.Core.StruggleSystem;
using V2.Items;
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
			if (PredPlayer.GetCurrentBellyWeight(player, onlyKicky: true) > 0)
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

			VoreTracker tracker = Main.LocalPlayer.AsPred().StomachTracker;
			if (tracker.PredatorStruggleChart is null)
				return;

			Vector2 bottomCenter = new Vector2(
				Main.screenWidth / 2,
				Main.screenHeight / 2
			);
			bottomCenter.Y += 52;
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

			foreach ((StruggleChartNote note, double proximity) noteData in tracker.CheckCloseNotes(-1, true))
			{
				float alpha = 1f;
				if (noteData.proximity >= 0)
					alpha = (float)Math.Max(noteData.proximity - 1.5f, 0f);
				else if (noteData.proximity < 0)
					alpha = (float)Math.Max((float)1.0 - Math.Abs((noteData.proximity - 0.2) * 10.0), 0f);

				Vector2 notePosition = bottomCenter;
				notePosition.X += noteData.note.Lane switch
				{
					NoteLane.Up => -32,
					NoteLane.Left => -16,
					NoteLane.Special => 0,
					NoteLane.Right => 16,
					NoteLane.Down => 32
				};
				notePosition.Y -= (float)((noteData.note.CorrectlyPressed ? noteData.note.PressedPosition : noteData.proximity) * 18.0);
				notePosition.Y -= _struggleNoteSpecial.Height();

				int frame = 0;
				if (noteData.note.PressAnimTimer > 7)
					frame = 1;
				if (noteData.note.PressAnimTimer > 14)
					frame = 2;
				if (noteData.note.PressAnimTimer > 21)
					frame = 3;
				Rectangle noteFrame = new Rectangle(
					frame * 20,
					0,
					18,
					18
				);

				Texture2D noteTexture = noteData.note.Lane switch
				{
					NoteLane.Up => _struggleNoteUp.Value,
					NoteLane.Left => _struggleNoteLeft.Value,
					NoteLane.Special => _struggleNoteSpecial.Value,
					NoteLane.Right => _struggleNoteRight.Value,
					NoteLane.Down => _struggleNoteDown.Value
				};
				spriteBatch.Draw(
					_struggleSystemBackdrop.Value,
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
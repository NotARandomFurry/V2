using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.UI.PredStatsMenu
{
	public static class PredStatsMenuMouthState
	{
		public const int NotHovered = 0;
		public const int Hovered = 1;
		public const int EatingCursor = 2;
		public const int YourCursorGotFuckingGulpedIdiot = 3;
		public const int RegurgitatingCursor = 4;
	}

	public class PredStatsMenuMouthUI : UIState
	{
		public static bool Visible { get; set; }

		public static Vector2 MouthPosition
		{
			get
			{
				Vector2 defPos = new Vector2(
					AccessorySlotLoader.DefenseIconPosition.X - 10 - 47 - 47 - 14,
					AccessorySlotLoader.DefenseIconPosition.Y + (float)TextureAssets.InventoryBack.Height() * 0.5f
				);
				Vector2 mouthPosition = new Vector2(
					(int)(defPos.X - TextureAssets.Extra[ExtrasID.DefenseShield].Value.Width / 2f),
					(int)(defPos.Y - TextureAssets.Extra[ExtrasID.DefenseShield].Value.Height / 2f)
				);
				mouthPosition.X -= 30f;
				mouthPosition.Y += 16f;
				return mouthPosition;
			}
		}

		public static int MouthState { get; set; }

		private int _mawHoverTime { get; set; }
		private int _mawSwallowTime { get; set; }
		private static Asset<Texture2D> _predStatsMenuEntryMaw = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenuMouth_Panel", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsMenuBackground = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Background", AssetRequestMode.ImmediateLoad);

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			if (Main.playerInventory && Main.EquipPage == 0)
				Visible = true;
			else
			{
				_mawHoverTime = 0;
				MouthState = PredStatsMenuMouthState.NotHovered;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			(int x, int y) = (1, 1);
			Rectangle hoverBox = new Rectangle(
				(int)MouthPosition.X - 17,
				(int)MouthPosition.Y - 17,
				34,
				34
			);

			void DecideNormalFrame()
			{
				switch (_mawHoverTime)
				{
					case int i when i < 25:
						x = 1;
						break;
					case int i when i >= 25 && i < 60:
						x = 2;
						break;
					case int i when i >= 60 && i < 105:
						x = 3;
						break;
					case int i when i >= 105 && i < 160:
						x = 4;
						break;
					case int i when i >= 160:
						x = 5;
						break;
				}
			}
			void DecideCursorGettingGulpedFrame(bool midSwallow)
			{
				y = 3;
				if (midSwallow)
				{
					switch (_mawSwallowTime)
					{
						case int i when i < 80:
							x = 1;
							break;
						case int i when i >= 80 && i < 110:
							x = 2;
							break;
						case int i when i >= 110 && i < 140:
							x = 3;
							break;
						case int i when i >= 140 && i < 170:
							x = 4;
							break;
						case int i when i >= 170 && i < 200:
							x = 5;
							break;
						case int i when i >= 200 && i < 230:
							x = 6;
							break;
						case int i when i >= 230 && i < 260:
							x = 7;
							break;
						case int i when i >= 260 && i < 280:
							x = 8;
							break;
						case int i when i >= 280 && i < 300:
							x = 9;
							break;
					}
				}
				else
					x = 10;
			}
			switch (MouthState)
			{
				case PredStatsMenuMouthState.NotHovered:
					if (hoverBox.Contains(Main.MouseScreen.ToPoint()))
					{
						MouthState = PredStatsMenuMouthState.Hovered;
						goto case PredStatsMenuMouthState.Hovered;
					}
					_mawHoverTime -= 1;
					if (_mawHoverTime < 0)
						_mawHoverTime = 0;
					DecideNormalFrame();
					y = 1;
					break;
				case PredStatsMenuMouthState.Hovered:
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						Main.LocalPlayer.AsPred().InPredStatsMenu = true;
						MouthState = PredStatsMenuMouthState.EatingCursor;
						goto case PredStatsMenuMouthState.EatingCursor;
					}
					if (!hoverBox.Contains(Main.MouseScreen.ToPoint()))
					{
						MouthState = PredStatsMenuMouthState.NotHovered;
						goto case PredStatsMenuMouthState.NotHovered;
					}

					Main.LocalPlayer.mouseInterface = true;
					_mawHoverTime += 1;
					if (_mawHoverTime > 225)
						_mawHoverTime = 225;
					Main.instance.MouseTextNoOverride(
						"Open the pred stats menu\n"
					  + "(WARNING: Your cursor may or\n"
					  + "may not get eaten by doing this)"
					);
					DecideNormalFrame();
					y = 2;
					break;
				case PredStatsMenuMouthState.EatingCursor:
					Main.LocalPlayer.mouseInterface = true;
					if (_mawSwallowTime >= 300)
					{
						Vector2 backdropPos = new Vector2(
							(Main.screenWidth - _predStatsMenuBackground.Value.Width) / 2,
							(Main.screenHeight - _predStatsMenuBackground.Value.Height) / 2
						);
						Mouse.SetPosition((int)backdropPos.X + 350, (int)backdropPos.Y + 40);
						MouthState = PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot;
						goto case PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot;
					}
					else
					{
						switch (_mawSwallowTime)
						{
							case 0:
								SoundEngine.PlaySound(
									Gulps.Short with { Volume = 1f },
									Main.screenPosition + MouthPosition
								);
								break;
							case 80:
								SoundEngine.PlaySound(
									Gulps.Standard with { Volume = 1f },
									Main.screenPosition + MouthPosition
								);
								break;
							case 200:
								SoundEngine.PlaySound(
									Gulps.Standard with { Volume = 1f, Pitch = -0.35f },
									Main.screenPosition + MouthPosition
								);
								break;
						}
						_mawSwallowTime += 1;
						Mouse.SetPosition((int)MouthPosition.X - 50, (int)MouthPosition.Y - 50);
					}
					DecideCursorGettingGulpedFrame(true);
					break;
				case PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot:
					DecideCursorGettingGulpedFrame(false);
					break;
				case PredStatsMenuMouthState.RegurgitatingCursor:
					Main.LocalPlayer.mouseInterface = true;
					if (_mawSwallowTime <= 0)
					{
						SoundEngine.PlaySound(
							Burps.Humanoid.Small with { Volume = 0.9f },
							Main.screenPosition + MouthPosition
						);
						Main.LocalPlayer.AsPred().InPredStatsMenu = false;
						_mawHoverTime = 145;
						Mouse.SetPosition((int)MouthPosition.X, (int)MouthPosition.Y);
						MouthState = PredStatsMenuMouthState.Hovered;
						goto case PredStatsMenuMouthState.Hovered;
					}
					else
					{
						_mawSwallowTime -= 1;
						Mouse.SetPosition((int)MouthPosition.X - 50, (int)MouthPosition.Y - 50);
					}
					DecideCursorGettingGulpedFrame(true);
					break;
			}

			spriteBatch.Draw(
				_predStatsMenuEntryMaw.Value,
				MouthPosition,
				new Rectangle(
					36 * (x - 1),
					36 * (y - 1),
					34,
					34
				),
				Color.White,
				0f,
				hoverBox.Size() / 2f,
				1f,
				SpriteEffects.None,
				0f
			);
		}
	}
}
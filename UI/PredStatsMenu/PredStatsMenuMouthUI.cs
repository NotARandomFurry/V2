using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;

namespace V2.UI.PredStatsMenu
{
	public static class PredStatsMenuMouthState
	{
		public static int NotHovered => 0;
		public static int Hovered => 1;
		public static int EatingCursor => 2;
		public static int YourCursorGotFuckingGulpedIdiot => 3;
		public static int RegurgitatingCursor => 4;
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

		public static int State { get; private set; }

		private int _mawHoverTime { get; set; }
		private int _mawSwallowTime { get; set; }
		private int _mawRegurgitateTime { get; set; }
		private static Asset<Texture2D> _predStatsMenuEntryMaw = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenuMouth_Panel", AssetRequestMode.ImmediateLoad);

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			if (Main.playerInventory && Main.EquipPage == 0)
				Visible = true;
			else
				_mawHoverTime = 0;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			int frame = 1;
			bool highlight = false;
			Rectangle hoverBox = new Rectangle(
				(int)MouthPosition.X - 17,
				(int)MouthPosition.Y - 17,
				34,
				34
			);
			if (hoverBox.Contains(Main.MouseScreen.ToPoint()))
			{
				highlight = true;
				_mawHoverTime += 1;
				if (_mawHoverTime > 225)
					_mawHoverTime = 225;
				Main.instance.MouseTextNoOverride(
					"Open the pred stats menu\n"
				  + "(WARNING: Your cursor may or\n"
				  + "may not get eaten by doing this)"
				);
			}
			else
			{
				_mawHoverTime -= 1;
				if (_mawHoverTime < 0)
					_mawHoverTime = 0;
			}
			switch (_mawHoverTime)
			{
				case int i when i < 25:
					frame = 1;
					break;
				case int i when i >= 25 && i < 60:
					frame = 2;
					break;
				case int i when i >= 60 && i < 105:
					frame = 3;
					break;
				case int i when i >= 105 && i < 160:
					frame = 4;
					break;
				case int i when i >= 160:
					frame = 5;
					break;
			}

			spriteBatch.Draw(
				_predStatsMenuEntryMaw.Value,
				MouthPosition,
				new Rectangle(
					36 * (frame - 1),
					highlight ? 36 : 0,
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
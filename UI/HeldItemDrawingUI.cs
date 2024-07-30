using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using V2.Items;
using V2.PlayerHandling;

namespace V2.UI
{
	public delegate void DelegateHeldItemDrawingUI(Item item, Player player, SpriteBatch spriteBatch);
	public delegate void DelegateGeneralItemDrawingUI(Player player, SpriteBatch spriteBatch);

	public class HeldItemDrawingUI : UIState
	{
		public static bool Visible = false;

		public override void Update(GameTime gameTime)
		{
			Player player = Main.LocalPlayer;
			Visible = false;
			if (!player.HeldItem.IsAir && player.HeldItem.AsAnItem().heldItemUIDrawMethod != null)
				Visible = true;
			if (player.AsV2Player().generalItemUIDrawMethods is not null && player.AsV2Player().generalItemUIDrawMethods.Count > 0)
				Visible = true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			Player player = Main.LocalPlayer;
			if (!player.HeldItem.IsAir && player.HeldItem.AsAnItem().heldItemUIDrawMethod != null)
				player.HeldItem.AsAnItem().heldItemUIDrawMethod.Invoke(player.HeldItem, player, spriteBatch);

			if (player.AsV2Player().generalItemUIDrawMethods is not null && player.AsV2Player().generalItemUIDrawMethods.Count > 0)
			{
				for (int i = 0; i < player.AsV2Player().generalItemUIDrawMethods.Count; i++)
				{
					DelegateGeneralItemDrawingUI UIDelegate = player.AsV2Player().generalItemUIDrawMethods[i];
					UIDelegate.Invoke(player, spriteBatch);
				}
			}
		}
	}
}
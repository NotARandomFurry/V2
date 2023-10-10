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

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			StruggleTracker tracker = ModContent.GetInstance<V2MasterSystem>().StruggleTrackers.FirstOrDefault(x => x.Predator is Player predPlayer && predPlayer.whoAmI == Main.myPlayer);
			if (tracker is null)
				return;

			
		}
	}
}
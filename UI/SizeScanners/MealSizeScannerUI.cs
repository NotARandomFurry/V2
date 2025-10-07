using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using V2.Core;
using V2.Items;
using V2.NPCs;
using V2.PlayerHandling;

namespace V2.UI.SizeScanners
{
	public class MealSizeScannerUI : UIState
	{
		public static bool Visible { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (player.AsPred().SizeScanner)
				Visible = true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			double maxEntityDistanceForDrawing = V2Utils.TileCountAsPixelCount(100.0);
			Player player = Main.LocalPlayer;
			double playerGutCapacity = player.AsPred().StomachCapacity;
			double playerGutFullness = player.AsPred().StomachFullness;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC futureFood = Main.npc[i];
				if (!futureFood.active || futureFood.CurrentCaptor() is not null)
					continue;

				if (futureFood.AsFood().CannotBeEatenDueToShenanigans)
					continue;

				if (futureFood.Distance(player.TrueCenter()) >= maxEntityDistanceForDrawing)
					continue;

				string size = "[c/";

				double npcSize = PreyData.GetPreySize(futureFood).CastToDecimalPlaces(3);
				if (player.AsPred().Rose)
					size += "00FFFF";
				else if (player.AsPred().SwallowCapacity < npcSize)
					size += "FF0000";
				else if (player.AsPred().StomachCapacity < npcSize)
					size += "FF0000";
				else
				{
					double playerGutFreeRoom = playerGutCapacity - playerGutFullness;
					double playerGutTickDamage = Math.Max(player.AsPred().DigestionTickDamage - futureFood.defense, 0);
					double playerGutDPS = playerGutTickDamage * player.AsPred().DigestionTickRate;
					if (playerGutFreeRoom < npcSize)
						size += "FFFF00";
					else if (playerGutTickDamage <= 0)
						size += "FFFF00";
					else if (futureFood.life > playerGutDPS * 60.0)
						size += "FFFF00";
					else
						size += "00FF00";
				}

				size += ":" + npcSize + "]";

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
				ChatManager.DrawColorCodedStringWithShadow(
					spriteBatch,
					FontAssets.MouseText.Value,
					size,
					(futureFood.Center + new Vector2(
						0,
						(futureFood.height / 2) + 16
					) - Main.screenPosition),
					Color.White,
					0f,
					ChatManager.GetStringSize(FontAssets.MouseText.Value, size, Vector2.One) * 0.5f,
					new Vector2(1.25f, 1.25f) / Main.GameZoomTarget
				);
				spriteBatch.End();
				spriteBatch.Begin();
			}

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player futureFood = Main.player[i];
				if (!futureFood.active || futureFood.dead || futureFood.whoAmI == Main.myPlayer || futureFood.CurrentCaptor() is not null)
					continue;

				if (futureFood.Distance(player.TrueCenter()) >= maxEntityDistanceForDrawing)
					continue;

				string size = "[c/";

				double playerSize = PreyData.GetPreySize(futureFood).CastToDecimalPlaces(3);
				if (player.AsPred().SwallowCapacity < playerSize)
					size += "FF00";
				else if (player.AsPred().StomachCapacity < playerSize)
					size += "FF00";
				else
				{
					double playerGutFreeRoom = playerGutCapacity - playerGutFullness;
					double playerGutTickDamage = Math.Max(player.AsPred().DigestionTickDamage - futureFood.statDefense, 0);
					double playerGutDPS = playerGutTickDamage * player.AsPred().DigestionTickRate;
					if (playerGutFreeRoom < playerSize)
						size += "FFFF";
					else if (playerGutTickDamage <= 0)
						size += "FFFF";
					else if (futureFood.statLife > playerGutDPS * 60.0)
						size += "FFFF";
					else
						size += "00FF";
				}

				size += "00:" + playerSize + "]";

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
				ChatManager.DrawColorCodedStringWithShadow(
					spriteBatch,
					FontAssets.MouseText.Value,
					size,
					(futureFood.Center + new Vector2(
						0,
						(futureFood.height / 2) + 20
					) - Main.screenPosition),
					Color.White,
					0f,
					ChatManager.GetStringSize(FontAssets.MouseText.Value, size, Vector2.One) * 0.5f,
					new Vector2(1.25f, 1.25f) / Main.GameZoomTarget
				);
				spriteBatch.End();
				spriteBatch.Begin();
			}
		}
	}
}
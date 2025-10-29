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
	public class PredCapacityScannerUI : UIState
	{
		public static bool Visible { get; set; }

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (player.AsFood().PredScanner)
				Visible = true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			double maxEntityDistanceForDrawing = V2Utils.TileCountAsPixelCount(100.0);
			Player player = Main.LocalPlayer;
			double playerSize = PreyData.GetPreySize(player);
			PreyData playerAsFood = PreyData.NewData(player);
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC futurePred = Main.npc[i];
				if (!futurePred.active || futurePred.CurrentCaptor() is not null)
					continue;

				if (futurePred.Distance(player.TrueCenter()) >= maxEntityDistanceForDrawing)
					continue;

				double futurePredGutCapacity = futurePred.AsPred().MaxStomachCapacity;
				double futurePredGutFullness = PredNPC.GetCurrentBellyWeight(futurePred);

				string size = "[c/";

				if (futurePred.AsPred().GetDigestionTickDamage is null || futurePred.AsPred().GetDigestionTickRate is null)
					size = "[c/FF0000:N/A]";
				else if (futurePred.AsPred().MaxStomachCapacity >= 9999999.0)
					size = "[c/00FFFF:∞]";
				else
				{
					if (player.AsFood().PerfectMeal)
						size += "00FFFF";
					else if (futurePredGutCapacity < playerSize)
						size += "FF0000";
					else
					{
						double futurePredGutFreeRoom = futurePredGutCapacity - futurePredGutFullness;
						double futurePredGutTickDamage = Math.Max(futurePred.AsPred().GetDigestionTickDamage.Invoke(futurePred, playerAsFood) - player.statDefense, 0);
						double futurePredGutDPS = futurePredGutTickDamage * futurePred.AsPred().GetDigestionTickRate.Invoke(futurePred, playerAsFood);
						if (futurePredGutFreeRoom < playerSize)
							size += "FFFF00";
						else if (futurePredGutTickDamage <= 0)
							size += "FFFF00";
						else if (player.statLife > futurePredGutDPS * 60.0)
							size += "FFFF00";
						else
							size += "00FF00";
					}

					size += ":" + futurePredGutCapacity + "]";
				}

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
				ChatManager.DrawColorCodedStringWithShadow(
					spriteBatch,
					FontAssets.MouseText.Value,
					size,
					(futurePred.Center + new Vector2(
						0,
						-((futurePred.height / 2) + 16)
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
				Player futurePred = Main.player[i];
				if (!futurePred.active || futurePred.dead || futurePred.whoAmI == Main.myPlayer || futurePred.CurrentCaptor() is not null)
					continue;

				if (futurePred.Distance(player.TrueCenter()) >= maxEntityDistanceForDrawing)
					continue;

				double futurePredGutCapacity = futurePred.AsPred().StomachCapacity;
				double futurePredGutFullness = futurePred.AsPred().StomachFullness;

				string size = "[c/";
				if (futurePredGutCapacity < playerSize)
					size += "FF00";
				else
				{
					double futurePredGutFreeRoom = futurePredGutCapacity - futurePredGutFullness;
					double futurePredGutTickDamage = Math.Max(futurePred.AsPred().DigestionTickDamage - player.statDefense, 0);
					double futurePredGutDPS = futurePredGutTickDamage * futurePred.AsPred().DigestionTickRate;
					if (futurePredGutFreeRoom < playerSize)
						size += "FFFF";
					else if (futurePredGutTickDamage <= 0)
						size += "FFFF";
					else if (player.statLife > futurePredGutDPS * 60.0)
						size += "FFFF";
					else
						size += "00FF";
				}

				size += "00:" + futurePredGutCapacity + "]";

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
				ChatManager.DrawColorCodedStringWithShadow(
					spriteBatch,
					FontAssets.MouseText.Value,
					size,
					(futurePred.Center + new Vector2(
						0,
						-((futurePred.height / 2) + 16)
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
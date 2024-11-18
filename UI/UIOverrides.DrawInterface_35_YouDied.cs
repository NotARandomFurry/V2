using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using V2.NPCs;
using V2.NPCs.Vanilla.TownNPCs.Stylist;
using V2.PlayerHandling;

namespace V2.UI
{
	public static partial class UIOverrides
	{
		public static void DrawInterface_35_YouDied()
		{
			Player player = Main.player[Main.myPlayer];
			if (!player.dead)
				return;

			float yOffsetFromScreenCenter = -60f;
			string value = Lang.inter[38].Value;
			if (player.AsFood().Digested)
			{
				if (ModContent.GetInstance<V2ClientConfig>().TheGutSlutVisionOMatic)
					yOffsetFromScreenCenter -= Main.screenHeight * 0.30f;
				value = Language.GetTextValue("Mods.V2.Death.DigestedPlayer.YouWereEaten");
			}
			Main.spriteBatch.DrawString(
				FontAssets.DeathText.Value,
				value,
				new Vector2(
					(float)(Main.screenWidth / 2) - FontAssets.DeathText.Value.MeasureString(value).X / 2f,
					(float)(Main.screenHeight / 2) + yOffsetFromScreenCenter
				),
				player.GetDeathAlpha(Color.Transparent),
				0f,
				default,
				1f,
				SpriteEffects.None,
				0f
			);

			if (player.lostCoins > 0)
			{
				yOffsetFromScreenCenter += 50f;
				string textValue = Language.GetTextValue("Game.DroppedCoins", player.lostCoinString);
				Main.spriteBatch.DrawString(
					FontAssets.MouseText.Value,
					textValue,
					new Vector2(
						(float)(Main.screenWidth / 2) - FontAssets.MouseText.Value.MeasureString(textValue).X / 2f,
						(float)(Main.screenHeight / 2) + yOffsetFromScreenCenter
					),
					player.GetDeathAlpha(Color.Transparent),
					0f,
					default,
					1f,
					SpriteEffects.None,
					0f
				);
			}

			yOffsetFromScreenCenter += (float)((player.lostCoins > 0) ? 24 : 50);
			yOffsetFromScreenCenter += 20f;
			float respawnCountdownScale = 0.7f;
			string textValue2 = Language.GetTextValue("Game.RespawnInSuffix", ((float)(int)(1f + (float)player.respawnTimer / 60f)).ToString());
			if (player.AsFood().Digested && ModContent.GetInstance<V2ClientConfig>().TheGutSlutVisionOMatic)
			{
				yOffsetFromScreenCenter += Main.screenHeight * 0.6f;
				respawnCountdownScale = 0.5f;
				textValue2 = Language.GetTextValue("Mods.V2.Death.DigestedPlayer.ManualRespawn");
				Main.spriteBatch.DrawString(
					FontAssets.DeathText.Value,
					textValue2,
					new Vector2(
						Main.screenWidth / 2,
						(Main.screenHeight / 2) + yOffsetFromScreenCenter
					),
					player.GetDeathAlpha(Color.Transparent),
					0f,
					ChatManager.GetStringSize(FontAssets.MouseText.Value, textValue2, new Vector2(respawnCountdownScale)) / 2f,
					respawnCountdownScale,
					SpriteEffects.None,
					0f
				);
			}
			else
			{
				Main.spriteBatch.DrawString(
					FontAssets.DeathText.Value,
					textValue2,
					new Vector2(
						(float)(Main.screenWidth / 2) - FontAssets.MouseText.Value.MeasureString(textValue2).X * respawnCountdownScale / 2f,
						(float)(Main.screenHeight / 2) + yOffsetFromScreenCenter
					),
					player.GetDeathAlpha(Color.Transparent),
					0f,
					default,
					respawnCountdownScale,
					SpriteEffects.None,
					0f
				);
			}
		}
	}
}
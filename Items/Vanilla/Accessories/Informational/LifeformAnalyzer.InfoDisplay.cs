using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Vanilla.Accessories.Informational
{
	public class LifeformAnalyzerDisplay : GlobalInfoDisplay
	{
		public override void ModifyDisplayParameters(InfoDisplay currentDisplay, ref string displayValue, ref string displayName, ref Color displayColor, ref Color displayShadowColor)
		{
			if (currentDisplay != InfoDisplay.LifeformAnalyzer)
				return;

			Player player = Main.player[Main.myPlayer];
			int num11 = 1300;
			int num12 = 0;
			int num13 = -1;
			if (player.accCritterGuideCounter <= 0)
			{
				player.accCritterGuideCounter = 15;
				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && Main.npc[k].rarity > num12 && (Main.npc[k].Center - player.Center).Length() < (float)num11)
					{
						num13 = k;
						num12 = Main.npc[k].rarity;
					}
				}

				player.accCritterGuideNumber = (byte)num13;
			}
			else
			{
				player.accCritterGuideCounter--;
				num13 = player.accCritterGuideNumber;
			}

			if (num13 >= 0 && num13 < 200 && Main.npc[num13].active && Main.npc[num13].rarity > 0)
			{
				displayValue = Main.npc[num13].GivenOrTypeName;
				DrawInfoAccs_AdjustInfoTextColorsForNPC(Main.npc[num13], ref displayColor, ref displayShadowColor);
			}
			else
			{
				displayValue = Language.GetTextValue("GameUI.NoRareCreatures");
			}
		}

		private static void DrawInfoAccs_AdjustInfoTextColorsForNPC(NPC npc, ref Color infoTextColor, ref Color infoTextShadowColor)
		{
			if (npc.CurrentCaptor() is not null)
			{
				infoTextColor = new Color(0, 191, 0);
				infoTextShadowColor = infoTextColor * 0.1f;
				byte a = infoTextShadowColor.A = Main.mouseTextColor;
				infoTextColor.A = a;
				return;
			}

			for (int i = 0; i < NPCID.Sets.GoldCrittersCollection.Count; i++)
			{
				int num = NPCID.Sets.GoldCrittersCollection[i];
				if (npc.type == num)
				{
					infoTextColor = Main.OurFavoriteColor;
					infoTextShadowColor = infoTextColor * 0.1f;
					byte a = infoTextShadowColor.A = Main.mouseTextColor;
					infoTextColor.A = a;
					break;
				}
			}
		}
	}
}

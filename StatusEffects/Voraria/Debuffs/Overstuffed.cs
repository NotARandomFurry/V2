using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using V2.Core;
using V2.Items.Vanilla.Accessories;
using V2.Items.Voraria.Accessories;
using V2.Items.Voraria.Accessories.Transformations;
using V2.Items.Voraria.Accessories.Transformations.Baelz;
using V2.Items.Voraria.Consumables.Potions;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Debuffs
{
	public class Overstuffed : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Debuffs.Overstuffed.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Debuffs.Overstuffed.Description");

		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			Player player = Main.LocalPlayer;
			double Overstuff = player.AsPred().StomachFullness / player.AsPred().StomachCapacity;

			int textRarity = ItemRarityID.LightRed;


			rare = textRarity;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			
		}

		public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
		{
			Player player = Main.LocalPlayer;
			double Overstuff = player.AsPred().Overstuff;
			int chosenImage = 0;
			int XOffset = 0;
			int XOffset2 = 0;

			if (Overstuff >= 3)
			{
				chosenImage = 5;
				XOffset = 24;
				XOffset2 = 16;
			}
			else if (Overstuff >= 2)
			{
				chosenImage = 4;
				XOffset = 12;
				XOffset2 = 4;
			}
			else if (Overstuff >= 1.6)
			{
				chosenImage = 3;
				XOffset = 4;
			}
			else if (Overstuff >= 1.3)
			{
				chosenImage = 2;
			}
			else if (Overstuff >= 1.1)
			{
				chosenImage = 1;
			}

			Texture2D buffTextureSheet = ModContent.Request<Texture2D>("V2/StatusEffects/Voraria/Debuffs/OverstuffedSheet").Value;

			spriteBatch.Draw(
				buffTextureSheet,
				drawParams.Position - new Vector2(XOffset / 2, 0),
				new Rectangle((32 * chosenImage) + XOffset2, 0, 32 + XOffset, 54),
				drawParams.DrawColor,
				0f,
				Vector2.Zero,
				1.0f,
				SpriteEffects.None,
				0f
			);
			return false;
		}
	}
}

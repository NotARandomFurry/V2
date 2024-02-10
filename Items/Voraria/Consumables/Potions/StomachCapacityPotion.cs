using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Voraria.Consumables.Potions
{
	public class StomachCapacityPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
				new Color(121, 255, 76),
				new Color(121, 255, 76),
				new Color(50, 191, 38),
			};
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.maxStack = Item.CommonMaxStack;
			Item.UseSound = SoundID.Item3;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTurn = true;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.consumable = true;

			Item.value = Item.buyPrice(0, 1, 25, 0);
			Item.rare = ItemRarityID.Green;

			Item.AsFood().EdibleOnUse = true;
			Item.AsFood().AlwaysEatenByUse = true;
		}
	}
}
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Consumables.PermanentUpgrades
{
	public class PureSwallowBoost1 : ModItem
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

			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Green;

			Item.AsFood().Size = 0.05;
			Item.AsFood().MaxHealth = 80;

			Item.AsFood().EdibleOnUse = true;

			Item.AsFood().OnSwallowDamage = 15;
			Item.AsFood().OnSwallowDeathReason = "{0} thought they were taking a shot, not getting one.";

			Item.AsFood().OnBreak = OnBreak;
		}

		public override bool? UseItem(Player player)
		{
			if (!player.AsPred().PermanentUpgradesUsed["PureSwallow1"])
				player.AsPred().PermanentUpgradesUsed["PureSwallow1"] = true;
			return true;
		}

		public static void OnBreak(Item item, Entity pred)
		{
			if (pred is Player playerPred)
			{
				if (!playerPred.AsPred().PermanentUpgradesUsed["PureSwallow1"])
					playerPred.AsPred().PermanentUpgradesUsed["PureSwallow1"] = true;
			}
		}
	}
}
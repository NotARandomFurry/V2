using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Accessories.Informational;
using V2.PlayerHandling;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Voraria.Consumables.PermanentUpgrades.Jujus
{
	public class BiomeJujuForest : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuForest");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuForest");
        public override string Texture => "V2/Items/UnspritedItem";

        public static int TUMBonus => 7;
        public static int ABSBonus => 7;
        public static int PermTUMBonus => 1;
        public static int PermABSBonus => 1;

        public override void SetStaticDefaults()
        {
            DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
            Main.RegisterItemAnimation(Type, anim);
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

		public override void SetDefaults()
		{
            Item.accessory = true;

            Item.width = 20;
			Item.height = 26;
			Item.maxStack = 1;

			Item.value = Item.sellPrice(0, 3);
			Item.rare = ItemRarityID.Lime;

            Item.AsFood().Size = 0.5;
            Item.AsFood().MaxHealth = 150;

            Item.AsFood().EdibleOnUse = true;

            Item.AsFood().OnBreak += OnBreak;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AsPred().TUM.Extra += TUMBonus;
            player.AsPred().ABS.Extra += ABSBonus;
        }

        public static bool OnBreak(Item item, Entity pred, bool direct)
        {
            SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
            if (pred is Player playerPred)
            {
                if (!playerPred.AsPred().PermanentUpgradesGained.ContainsKey("BiomeJujuForest"))
                    playerPred.AsPred().PermanentUpgradesGained.Add("BiomeJujuForest", false);

                if (!playerPred.AsPred().PermanentUpgradesGained["BiomeJujuForest"])
                    playerPred.AsPred().PermanentUpgradesGained["BiomeJujuForest"] = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuForest",
				new
				{
                    TUM = TUMBonus,
                    ABS = ABSBonus,
                    PermTUM = PermTUMBonus,
                    PermABS = PermABSBonus,
                }
			);
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlankJuju>()
                .AddIngredient(ItemID.Wood, 25)
                .AddIngredient(ItemID.Sunflower, 3)
                .AddIngredient(ItemID.Mushroom, 5)
                .AddIngredient(ItemID.Gel, 100)
                .Register();
        }
    }
}
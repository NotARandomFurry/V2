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
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.Items.Voraria.Consumables.PermanentUpgrades.Jujus
{
	public class BiomeJujuHallow : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => false;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuHallow");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuHallow.Short");

        public static int TUMBonus => 7;
        public static int ABSBonus => 7;
        public static int PermTUMBonus => 1;
        public static int PermABSBonus => 1;

        public override void SetStaticDefaults()
        {
            DrawAnimationVertical anim = new DrawAnimationVertical(8, 2);
            Main.RegisterItemAnimation(Type, anim);
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

		public override void SetDefaults()
		{
            Item.accessory = true;

            Item.width = 32;
			Item.height = 32;
			Item.maxStack = 1;

			Item.value = Item.sellPrice(0, 3);
			Item.rare = ItemRarityID.Lime;

            Item.AsFood().Size = 0.5;
            Item.AsFood().MaxHealth = 150;

            Item.AsFood().EdibleOnUse = true;

            Item.AsFood().OnBreak += OnBreak;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, new Vector3(80, 255, 80) * 0.005f);
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
                if (!playerPred.AsPred().PermanentUpgradesGained.ContainsKey("BiomeJujuHallow"))
                    playerPred.AsPred().PermanentUpgradesGained.Add("BiomeJujuHallow", false);

                if (!playerPred.AsPred().PermanentUpgradesGained["BiomeJujuHallow"])
                    playerPred.AsPred().PermanentUpgradesGained["BiomeJujuHallow"] = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuHallow",
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
                .AddIngredient(ItemID.Pearlwood, 33)
                .AddIngredient(ItemID.UnicornHorn, 3)
                .AddIngredient(ItemID.QueenSlimeCrystal, 3)
                .AddIngredient(ItemID.PixieDust, 33)
                .Register();
        }
    }
}
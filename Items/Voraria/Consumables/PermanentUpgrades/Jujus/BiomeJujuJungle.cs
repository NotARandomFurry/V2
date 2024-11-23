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
	public class BiomeJujuJungle : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuJungle");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuJungle");

        public static int TUMBonus => 17;
        public static float SpeedDecrease => 0.1f;
        public static int PermTUMBonus => 3;

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
            Lighting.AddLight(Item.Center, new Vector3(135, 255, 80) * 0.005f);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AsPred().TUM.Extra += TUMBonus;
            player.moveSpeed -= SpeedDecrease;
        }

        public static bool OnBreak(Item item, Entity pred, bool direct)
        {
            SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
            if (pred is Player playerPred)
            {
                if (!playerPred.AsPred().PermanentUpgradesGained.ContainsKey("BiomeJujuJungle"))
                    playerPred.AsPred().PermanentUpgradesGained.Add("BiomeJujuJungle", false);

                if (!playerPred.AsPred().PermanentUpgradesGained["BiomeJujuJungle"])
                    playerPred.AsPred().PermanentUpgradesGained["BiomeJujuJungle"] = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuJungle",
				new
				{
                    TUM = TUMBonus,
                    SPD = 10,
                    PermTUM = PermTUMBonus,
                }
			);
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlankJuju>()
                .AddIngredient(ItemID.RichMahogany, 25)
                .AddIngredient(ItemID.HoneyBlock, 15)
                .AddIngredient(ItemID.Vine, 3)
                .AddIngredient(ItemID.BambooBlock, 25)
                .AddIngredient(ItemID.JungleSpores, 20)
                .Register();
        }
    }
}
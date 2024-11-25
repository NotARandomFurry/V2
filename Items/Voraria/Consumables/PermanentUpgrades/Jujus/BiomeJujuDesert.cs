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
	public class BiomeJujuDesert : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuDesert");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuDesert.Short");

        public static int ACIBonus => 8;
        public static float StruggleBonus => 1f;
        public static int PermACIBonus => 2;
        public static float PermStruggleBonus => 25f;

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
            Lighting.AddLight(Item.Center, new Vector3(255, 255, 80) * 0.005f);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AsPred().ACI.Extra += ACIBonus;
            player.AsFood().StruggleStrengthModifier += StruggleBonus;
        }

        public static bool OnBreak(Item item, Entity pred, bool direct)
        {
            SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
            if (pred is Player playerPred)
            {
                if (!playerPred.AsPred().PermanentUpgradesGained.ContainsKey("BiomeJujuDesert"))
                    playerPred.AsPred().PermanentUpgradesGained.Add("BiomeJujuDesert", false);

                if (!playerPred.AsPred().PermanentUpgradesGained["BiomeJujuDesert"])
                    playerPred.AsPred().PermanentUpgradesGained["BiomeJujuDesert"] = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuDesert",
				new
				{
                    ACI = ACIBonus,
                    STR = 100,
                    PermACI = PermACIBonus,
                    PermSTR = 25,
                }
			);
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlankJuju>()
                .AddIngredient(ItemID.Cactus, 25)
                .AddIngredient(ItemID.AntlionMandible, 6)
                .AddIngredient(ItemID.FossilOre, 15)
                .AddIngredient(ItemID.AncientCloth, 4)
                .Register();
        }
    }
}
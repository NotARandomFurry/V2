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
	public class BiomeJujuSky : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuSky");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuSky.Short");

        public static float MoveSpeedBonus => 0.20f;
        public static float JumpSpeedBonus => 2.505f;
        public static float StomachWeight => 0.35f;
        public static float PermStomachWeight => 0.1f;

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;

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
            player.AsPred().StomachWeightModifier *= 1f - StomachWeight;
            player.moveSpeed += MoveSpeedBonus;
            player.jumpSpeedBoost += JumpSpeedBonus;
        }

        public static bool OnBreak(Item item, Entity pred, bool direct)
        {
            SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
            if (pred is Player playerPred)
            {
                if (!playerPred.AsPred().PermanentUpgradesGained.ContainsKey("BiomeJujuSky"))
                    playerPred.AsPred().PermanentUpgradesGained.Add("BiomeJujuSky", false);

                if (!playerPred.AsPred().PermanentUpgradesGained["BiomeJujuSky"])
                    playerPred.AsPred().PermanentUpgradesGained["BiomeJujuSky"] = true;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.Consumables.PermanentUpgrades.Jujus.BiomeJujuSky",
				new
				{
                    SPD = 20,
                    Jump = 100,
                    WeightReduce = 35,
                    PermWeightReduce = 10,
                }
			);
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlankJuju>()
                .AddIngredient(ItemID.Cloud, 25)
                .AddIngredient(ItemID.GiantHarpyFeather, 1)
                .AddIngredient(ItemID.SoulofFlight, 25)
                .AddIngredient<ObserverPupil>(3)
                .Register();
        }
    }
}
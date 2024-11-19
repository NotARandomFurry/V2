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
	public class BlankJuju : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.PermanentUpgrades.Jujus.BlankJuju");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.PermanentUpgrades.Jujus.BlankJuju");
        public override string Texture => "V2/Items/UnspritedItem";

        public override void SetStaticDefaults()
        {
            DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
            Main.RegisterItemAnimation(Type, anim);
            ItemID.Sets.AnimatesAsSoul[Type] = true;

            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ShimmerJuju>();
        }

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.maxStack = Item.CommonMaxStack;

			Item.value = Item.buyPrice(0, 2, 50);
			Item.rare = ItemRarityID.Orange;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.Consumables.PermanentUpgrades.Jujus.BlankJuju",
				new
				{

				}
			);
		}
	}
}
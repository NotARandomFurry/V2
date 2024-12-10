using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Items.Voraria.Weapons.Summon;

namespace V2.Items.Voraria
{
	public class MushroomToken : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.MushroomToken");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.MushroomToken.Short");
        public override string Texture => "V2/Items/UnspritedItem";

        public override void SetStaticDefaults()
        {
            DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
            Main.RegisterItemAnimation(Type, anim);
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
		{
			Item.maxStack = Item.CommonMaxStack;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(
                gold: 5
            );
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Voraria.MushroomToken",
				new
				{
					
				}
			);
		}
	}
}

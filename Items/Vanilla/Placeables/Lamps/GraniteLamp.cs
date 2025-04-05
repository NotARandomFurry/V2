using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Lamps
{
    internal class GraniteLamp : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.GraniteLamp;
        public override void SetDefaults(Item item)
        {
            item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Vanilla.Furniture.GraniteSet.GraniteLamp>());

			item.AsAnItem().PlaceableCanBeHungry = true;
			item.AsAnItem().PlaceableHungryByDefault = false;
		}

    }
}

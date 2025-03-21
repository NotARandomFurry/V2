using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Chairs
{
    internal class GraniteChair : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GraniteChair;
        public override void SetDefaults(Item entity)
        {
            entity.DefaultToPlaceableTile(ModContent.TileType<Tiles.Vanilla.Furniture.GraniteSet.GraniteChair>());
        }

    }
}

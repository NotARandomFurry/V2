using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Relics
{
    internal class EmpressOfLightRelic : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.FairyQueenMasterTrophy;
        public override void SetDefaults(Item item)
        {
            item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Vanilla.Relics.EmpressOfLightRelic>());

			item.AsAnItem().PlaceableCanBeHungry = true;
			item.AsAnItem().PlaceableHungryByDefault = true;
		}
    }
}

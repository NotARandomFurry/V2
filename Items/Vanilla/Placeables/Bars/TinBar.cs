using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Bars
{
    public class TinBar : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TinBar;
        public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 30;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
    }
}

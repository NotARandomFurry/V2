using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Vanilla
{
    public class VanillaGel : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Gel;

<<<<<<< Updated upstream
        public override void SetDefaults(Item entity)
        {
            entity.AsFood().MaxHealth = 6;
            entity.AsFood().Size = 0.006d;
            entity.AsFood().WellFedPower = 0.1;
        }
=======
		public override void SetDefaults(Item entity)
		{
			entity.AsFood().MaxHealth = 6;
			entity.AsFood().Size = 0.006;
			entity.AsFood().WellFedPower = 0.15;
		}
>>>>>>> Stashed changes

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.AddVorariaDynamicItemTooltip("Vanilla.Gel", new { });
        }
    }
    public class PinkGel : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.PinkGel;

<<<<<<< Updated upstream
        public override void SetDefaults(Item entity)
        {
            entity.AsFood().MaxHealth = 24;
            entity.AsFood().Size = 0.006d;
            entity.AsFood().WellFedPower = 0.5;
        }
    }
=======
		public override void SetDefaults(Item entity)
		{
			entity.AsFood().MaxHealth = 24;
			entity.AsFood().Size = 0.006;
			entity.AsFood().WellFedPower = 0.75;
            entity.AsFood().CalorieMultiplier = 1.5;
        }
	}
>>>>>>> Stashed changes
}

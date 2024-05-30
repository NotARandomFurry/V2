using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Gems
{
	public class Emerald : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Emerald;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 662;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Placeables.Gems.Emerald",
				new
				{

				}
			);
		}
	}
}

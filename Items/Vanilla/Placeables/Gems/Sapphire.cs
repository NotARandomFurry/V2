using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Gems
{
	public class Sapphire : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Sapphire;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 676;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Placeables.Gems.Sapphire",
				new
				{

				}
			);
		}
	}
}

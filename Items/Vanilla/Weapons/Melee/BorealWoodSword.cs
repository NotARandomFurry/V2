using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class BorealWoodSword : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.BorealWoodSword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 110;
			item.AsFood().Size = 0.40;

			item.AsTaggable().Broadsword = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.BorealWoodSword",
				new
				{
					
				}
			);
		}
	}
}

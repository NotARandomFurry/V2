using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class OakWoodSword : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodenSword;

		public override void SetDefaults(Item item)
		{
			item.SetNameOverride(Language.GetTextValue("Mods.V2.ItemName.Vanilla.Weapons.Melee.OakWoodSword"));

			item.AsFood().MaxHealth = 110;
			item.AsFood().Size = 0.40;

			item.AsTaggable().Broadsword = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.OakWoodSword",
				new
				{
					
				}
			);
		}
	}
}

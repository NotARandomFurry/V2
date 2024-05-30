using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Ranged
{
	public class OakWoodBow : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodenBow;

		public override void SetDefaults(Item item)
		{
			item.SetNameOverride(Language.GetTextValue("Mods.V2.ItemName.Vanilla.Weapons.Ranged.OakWoodBow"));

			item.AsFood().MaxHealth = 65;
			item.AsFood().Size = 0.25;

			item.AsTaggable().Broadsword = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Ranged.OakWoodBow",
				new
				{
					
				}
			);
		}
	}
}

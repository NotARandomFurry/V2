using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Projectiles.Vanilla.Summons.Pets;

namespace V2.Items.Vanilla.Armor
{
	public class OakWoodBreastplate : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodBreastplate;

		public override void SetDefaults(Item item)
		{
			item.SetNameOverride(Language.GetTextValue("Mods.V2.ItemName.Vanilla.Armor.OakWood.Chest"));

			item.AsFood().MaxHealth = 200;
			item.AsFood().Size = 0.50;

			item.defense = 1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Armor.OakWood.Chest",
				new
				{
					
				}
			);
		}
	}
}

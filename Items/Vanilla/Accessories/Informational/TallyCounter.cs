using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Accessories.Informational
{
	public class TallyCounter : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.TallyCounter;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.RemoveAll(x => x.Name.Contains("Tooltip"));
			if (V2Utils.FindLastTooltipLineBeforeFlavorText(tooltips, out TooltipLine line))
			{
				V2Utils.InsertNewTooltipLine(
					ref tooltips,
					line,
					1,
					"Tooltip",
					Language.GetTextValue(
						"Mods.V2.ItemTooltip.Vanilla.Accessories.Informational.TallyCounter"
					)
				);
			}
		}
	}
}

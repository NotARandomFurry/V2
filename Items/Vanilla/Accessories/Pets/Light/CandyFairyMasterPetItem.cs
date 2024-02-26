using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Accessories.Pets.Light
{
	public class CandyFairyMasterPetItem : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.FairyQueenPetItem;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.RemoveAll(x => x.Name.Contains("Tooltip"));
			V2Utils.AddVorariaDynamicItemTooltip(
				tooltips,
				"Vanilla.Accessories.Pets.Light.CandyFairyMasterPetItem",
				new
				{

				}
			);
		}
	}
}

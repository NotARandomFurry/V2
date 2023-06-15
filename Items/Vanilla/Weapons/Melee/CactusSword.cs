using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class CactusSword : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.CactusSword;

		public override void SetDefaults(Item entity)
		{
			entity.AsFood().MaxHealth = 51;
			entity.AsFood().Size = 0.52;

			entity.AsFood().OnSwallow += OnSwallow;
			entity.AsFood().UpdateInStomach += UpdateInStomach;
		}

		public static void OnSwallow(Item item, Entity pred)
		{
			if (pred is Player playerPred)
				playerPred.statLife -= 6;
			else if (pred is NPC NPCPred)
				NPCPred.life -= 6;
		}

		public static void UpdateInStomach(Item item, Entity pred, bool dead)
		{
			if (!dead)
				return;

			if (pred is Player playerPred)
				playerPred.AddBuff(BuffID.Thorns, V2Utils.SensibleTime(seconds: 10));
			else if (pred is NPC NPCPred)
				NPCPred.AddBuff(BuffID.Thorns, V2Utils.SensibleTime(seconds: 10));
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			V2Utils.FindLastTooltipLineBeforeFlavorText(tooltips, out TooltipLine finalLine);
			tooltips.Insert(
				tooltips.IndexOf(finalLine) + 1,
				new TooltipLine(
					V2.Instance,
					"FlavorText",
					"Grants the Thorns buff while digesting and briefly after being fully digested"
				)
			);
		}
	}
}

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
	public class BorealWoodHelmet : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodHelmet;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 120;
			item.AsFood().Size = 0.30;

			item.AsFood().OnBreak += OnBreak;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;
	}
}

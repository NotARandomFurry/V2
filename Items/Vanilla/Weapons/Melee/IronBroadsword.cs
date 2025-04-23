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
	public class IronBroadsword : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.IronBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 495;
			item.AsFood().Size = 0.54;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
}

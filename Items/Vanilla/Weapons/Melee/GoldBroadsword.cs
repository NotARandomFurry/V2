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
	public class GoldBroadsword : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GoldBroadsword;

		public override void SetDefaults(Item entity)
		{
			entity.AsFood().MaxHealth = 312;
			entity.AsFood().Size = 0.54;
			entity.AsFood().AcidResistTier = 2;

			entity.AsTaggable().Broadsword = true;
		}
	}
}

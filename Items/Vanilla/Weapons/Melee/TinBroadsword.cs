using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class TinBroadsword : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TinBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 180;
			item.AsFood().Size = 0.44;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Broadsword = true;
		}
	}
}

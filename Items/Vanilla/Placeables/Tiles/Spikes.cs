using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Tiles
{
	public class Spike : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Spike;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 333;
			item.AsFood().Size = 0.07;
			item.AsFood().AcidResistTier = 1;

			item.AsFood().OnSwallowDamage = 15;
			item.AsFood().OnSwallowDeathReason = "{0} ate spikes. What'd you think was going to happen?";
			item.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(seconds: 2);
		}
	}
	public class WoodenSpike : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodenSpike;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 90;
			item.AsFood().Size = 0.07;

			item.AsFood().OnSwallowDamage = 30;
			item.AsFood().OnSwallowDeathReason = "{0} ate spikes. What'd you think was going to happen?";
			item.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(seconds: 6);
		}
	}
}

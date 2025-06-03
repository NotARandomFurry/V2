using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class GoldHammer : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GoldHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 600;
			item.AsFood().Size = 0.40;
			item.AsFood().AcidResistTier = 2;
		}
	}
}

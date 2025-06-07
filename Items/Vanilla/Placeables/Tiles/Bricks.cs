using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Tile
{
	public class LihzahrdBrick : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.LihzahrdBrick;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 50000;
			item.AsFood().Size = 0.125;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class DungeonBrick : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => 
			entity.type is ItemID.PinkBrick or ItemID.BlueBrick or ItemID.GreenBrick;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 2000;
			item.AsFood().Size = 0.125;
			item.AsFood().AcidResistTier = 2;
		}
	}
}

using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Tiles
{
	public class Dirt : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.DirtBlock || entity.type == ItemID.MudBlock || entity.type == ItemID.AshBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 90;
			item.AsFood().Size = 0.07;
		}
	}
	public class Seeds : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GrassSeeds || entity.type == ItemID.CorruptSeeds || entity.type == ItemID.CrimsonSeeds;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.03;
		}
	}
	public class Stone : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.StoneBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 300;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().Size = 0.1;
		}
	}
	public class EvilStone : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.EbonstoneBlock or ItemID.CrimstoneBlock or ItemID.PearlstoneBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 1000;
			item.AsFood().AcidResistTier = 2;
			item.AsFood().Size = 0.1;
		}
	}
}

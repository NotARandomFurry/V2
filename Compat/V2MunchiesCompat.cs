using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Items.Voraria.Consumables.PermanentUpgrades;
using V2.Items.Voraria.Accessories.Thingymajigs;
using V2.PlayerHandling;

namespace V2.Compat
{
	[JITWhenModsEnabled("munchies")]
	public class V2MunchiesCompat : V2CompatModule
	{
		private void AddSingleConsumablePlayer<TItem>(string upgradeName = null) where TItem : class
		{
			compatMod.Call("AddSingleConsumable", V2.Instance, "1.4.2", ModContent.GetInstance<TItem>(), "player", hasMyPlayerPermanentUpgradeFunc(upgradeName ?? typeof(TItem).Name), null, null);
		}
		public V2MunchiesCompat(Mod compatMod) : base(compatMod)
		{
		}
		public override void ApplyCompatibility()
		{
			AddSingleConsumablePlayer<PureSwallowBoost1>("PureSwallow1");
			AddSingleConsumablePlayer<BiomeCorruptionThingy>("Thingy_BiomeCorruption");
			AddSingleConsumablePlayer<BiomeCrimsonThingy>("Thingy_BiomeCrimson");
			AddSingleConsumablePlayer<BiomeDesertThingy>("Thingy_BiomeDesert");
			AddSingleConsumablePlayer<BiomeDungeonThingy>("Thingy_BiomeDungeon");
			AddSingleConsumablePlayer<BiomeForestThingy>("Thingy_BiomeForest");
			AddSingleConsumablePlayer<BiomeHallowThingy>("Thingy_BiomeHallow");
			AddSingleConsumablePlayer<BiomeJungleThingy>("Thingy_BiomeJungle");
			AddSingleConsumablePlayer<BiomeMushroomThingy>("Thingy_BiomeMushroom");
			AddSingleConsumablePlayer<BiomeOceanThingy>("Thingy_BiomeOcean");
			AddSingleConsumablePlayer<BiomeShimmerThingy>("Thingy_BiomeShimmer");
			AddSingleConsumablePlayer<BiomeSkyThingy>("Thingy_BiomeSky");
			AddSingleConsumablePlayer<BiomeSnowThingy>("Thingy_BiomeSnow");
			AddSingleConsumablePlayer<BiomeUnderworldThingy>("Thingy_BiomeUnderworld");
		}
		public override void UnapplyCompatibility()
		{

		}

		private static Func<bool> hasMyPlayerPermanentUpgradeFunc(string upgrade)
		{
			return () =>
			{
				if (Main.player[Main.myPlayer].AsPred().PermanentUpgradesGained.TryGetValue(upgrade, out bool hasUpgrade))
					return hasUpgrade;
				else
					return false;
			};
		}

	}
}

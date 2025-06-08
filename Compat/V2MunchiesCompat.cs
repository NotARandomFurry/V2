using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Items.Voraria.Consumables.PermanentUpgrades;
using V2.Items.Voraria.Consumables.PermanentUpgrades.Jujus;
using V2.PlayerHandling;

namespace V2.Compat
{
	[JITWhenModsEnabled("munchies")]
	public class V2MunchiesCompat : V2CompatModule
	{
		private void AddSingleConsumablePlayer<TItem>(string upgradeName = null) where TItem : class
		{
			compatMod.Call("AddSingleConsumable", V2.Instance, "1.4.2", ModContent.GetInstance<TItem>(), "player", HasMyPlayerPermanentUpgradeFunc(upgradeName ?? typeof(TItem).Name), null, null);
		}
		public V2MunchiesCompat(Mod compatMod) : base(compatMod)
		{
		}
		public override void ApplyCompatibility()
		{
			AddSingleConsumablePlayer<PureSwallowBoost1>("PureSwallow1");
			AddSingleConsumablePlayer<BiomeJujuForest>();
			AddSingleConsumablePlayer<BiomeJujuDesert>();
			AddSingleConsumablePlayer<BiomeJujuSnow>();
			AddSingleConsumablePlayer<BiomeJujuJungle>();
			AddSingleConsumablePlayer<BiomeJujuSky>();
			AddSingleConsumablePlayer<ShimmerJuju>();
		}
		public override void UnapplyCompatibility()
		{

		}

		private static Func<bool> HasMyPlayerPermanentUpgradeFunc(string upgrade)
		{
			return () => Main.player[Main.myPlayer].AsPred().PermanentUpgradesGained.TryGetValue(upgrade, out bool hasUpgrade) && hasUpgrade;
		}

	}
}

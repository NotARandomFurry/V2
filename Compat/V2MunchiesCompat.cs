using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.Items.Voraria.Consumables.PermanentUpgrades;
using V2.PlayerHandling;

namespace V2.Compat
{
    [JITWhenModsEnabled("munchies")]
    public class V2MunchiesCompat : V2CompatModule
    {
        public V2MunchiesCompat(Mod compatMod) : base(compatMod)
        { 
        }
        public void DoCompatibility()
        {
            compatMod.Call("AddSingleConsumable", V2.Instance, "1.4.2", ModContent.GetInstance<PureSwallowBoost1>(), "player", hasMyPlayerPermanentUpgradeFunc("PureSwallow1"), null, null);
        }



        private static Func<bool> hasMyPlayerPermanentUpgradeFunc(string upgrade)
        {
            return new Func<bool>(() =>
            {
                if (Main.player[Main.myPlayer].AsPred().PermanentUpgradesGained.TryGetValue(upgrade, out bool hasUpgrade))
                {
                    return hasUpgrade;
                }
                else
                {
                    return false;
                }
            });
        }
    }
}

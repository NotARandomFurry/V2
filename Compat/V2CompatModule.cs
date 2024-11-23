using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace V2.Compat
{
    public class V2CompatModule
    {
        public Mod compatMod;
        public V2CompatModule(Mod mod)
        {
            V2.Instance.Logger.Info($"Loading compat: {mod.Name} - {mod.Version}");
            compatMod = mod;
        }
    }
}

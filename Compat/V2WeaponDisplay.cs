using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;

namespace V2.Compat
{
    // Before you murder me sign
    // Harmony is a great library and at least give it a try. 
    [JITWhenModsEnabled("WeaponDisplay")]
    internal class V2WeaponDisplay : V2CompatModule
    {
        Harmony h;

        MethodInfo drawPatch;
        public V2WeaponDisplay(Mod mod) : base(mod)
        {
            h = new Harmony("V2");
#if DEBUG
            Harmony.DEBUG = true;
#endif
        }

        public override void ApplyCompatibility()
        {
            V2.Instance.Logger.Info("Applying patch: WeaponDisplay.ItemInWorld.ItemLight::PreDrawInWorld");
            MethodInfo m = typeof(WeaponDisplay.ItemInWorld.ItemLight).GetMethod("PreDrawInWorld");
            drawPatch = h.Patch(m, prefix: new HarmonyMethod(Prefix));
        }

        public override void UnapplyCompatibility() 
        {
            h.UnpatchAll();
        }

        public static bool Prefix(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return !SkipDrawIfItemInTum(item);
        }

        public static bool SkipDrawIfItemInTum(Item i)
        {
            return i.CurrentCaptor() != null;
        }

    }
}
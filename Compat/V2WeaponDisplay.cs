using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
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
	// -VenomiZeD

	// no, if I wanna learn Harmony I'll go work on Lunch Break of Ruina
	// -Thomas
	[JITWhenModsEnabled("WeaponDisplay")]
	public class V2WeaponDisplay : V2CompatModule
	{
		private delegate bool orig_PreDrawInWorld(WeaponDisplay.ItemInWorld.ItemLight self, Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI);
		internal static Hook WeaponDisplay_ItemInWorld_ItemLightHook;
		private static readonly MethodInfo WeaponDisplay_ItemInWorld_ItemLight_MethodInfo =
			typeof(WeaponDisplay.ItemInWorld.ItemLight).GetMethod("PreDrawInWorld");
		public V2WeaponDisplay(Mod mod) : base(mod)
		{

		}

		public override void ApplyCompatibility()
		{
			V2.Instance.Logger.Info("Applying patch: WeaponDisplay.ItemInWorld.ItemLight::PreDrawInWorld");
			WeaponDisplay_ItemInWorld_ItemLightHook = new Hook(WeaponDisplay_ItemInWorld_ItemLight_MethodInfo, (orig_PreDrawInWorld orig, WeaponDisplay.ItemInWorld.ItemLight self, Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) =>
			{
				if (item.CurrentCaptor() is null)
					return orig(self, item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);

				return false;
			});
			WeaponDisplay_ItemInWorld_ItemLightHook.Apply();
		}

		public override void UnapplyCompatibility()
		{
			if (WeaponDisplay_ItemInWorld_ItemLightHook is null) return;
			WeaponDisplay_ItemInWorld_ItemLightHook.Undo();
			WeaponDisplay_ItemInWorld_ItemLightHook = null;
		}
	}
}
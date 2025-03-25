using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using V2.Core;
using V2.Core.StruggleSystem;
using V2.NPCs;

namespace V2.PlayerHandling
{
	public partial class PreyPlayer : ModPlayer
	{
		/// <summary>
		/// Affects the amount of struggle damage that the player deals to their predator with struggles.>br/>
		/// </summary>
		public StatModifier StruggleStrengthModifier { get; set; }
		public double StruggleDamage {
			get {
				double baseStruggleStrength = 6.0;
				return StruggleStrengthModifier.ApplyTo((float)baseStruggleStrength);
			}
		}
	}
}

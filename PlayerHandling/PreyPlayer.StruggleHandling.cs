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
		public PredStat STR { get; set; }
		public StatModifier StruggleStrengthModifier { get; set; }
		public double StruggleStrength {
			get {
				double baseStruggleStrength = 1.5;
				baseStruggleStrength += 0.3 * STR.Total;
				return StruggleStrengthModifier.ApplyTo((float)baseStruggleStrength);
			}
		}
	}
}

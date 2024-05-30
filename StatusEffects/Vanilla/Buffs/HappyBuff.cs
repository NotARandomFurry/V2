using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.StatusEffects.Vanilla.Buffs
{
	public class HappyBuff : GlobalBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[BuffID.Sunflower] = false;
		}
	}
}

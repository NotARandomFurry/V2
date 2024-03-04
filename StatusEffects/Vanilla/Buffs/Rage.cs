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
	public class Rage : GlobalBuff
	{
		public static double CritChanceBonus => 0.10;
		public static int GLPBonus => 5;
		public static int ABSBonus => 5;
		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.Rage)
				return;

			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Vanilla.Buffs.Rage.Description",
				new
				{
					RageCritChanceBonus = CritChanceBonus.ToPercentage(2),
					RageGLPBonus = GLPBonus,
					RageABSBonus = ABSBonus,
				}
			);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Rage)
				return;

			player.AsPred().GLP.Extra += GLPBonus;
			player.AsPred().ABS.Extra += ABSBonus;
		}
	}
}

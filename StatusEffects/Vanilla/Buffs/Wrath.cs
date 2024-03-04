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
	public class Wrath : GlobalBuff
	{
		public static double DamageBonus => 0.10;
		public static int TUMBonus => 5;
		public static int ACIBonus => 5;
		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.Wrath)
				return;

			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Vanilla.Buffs.Wrath.Description",
				new
				{
					WrathDamageBonus = DamageBonus.ToPercentage(2),
					WrathTUMBonus = TUMBonus,
					WrathACIBonus = ACIBonus,
				}
			);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Wrath)
				return;

			player.AsPred().TUM.Extra += TUMBonus;
			player.AsPred().ACI.Extra += ACIBonus;
		}
	}
}

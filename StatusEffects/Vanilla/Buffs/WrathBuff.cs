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
	public class WrathBuff : GlobalBuff
	{
		public static float DamageBonus => 0.10f;
		public static int TUMBonus => 5;
		public static int ACIBonus => 5;
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Wrath, this);
		}

		public override bool RightClick(int type, int buffIndex) => type != BuffID.Wrath;

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Wrath)
				return;

			player.GetDamage(DamageClass.Generic) += DamageBonus;
			player.AsPred().TUM.Extra += TUMBonus;
			player.AsPred().ACI.Extra += ACIBonus;
		}

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.Wrath)
				return;

			rare = ItemRarityID.Red;
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
	}
}

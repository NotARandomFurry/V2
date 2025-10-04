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
	public class RageBuff : GlobalBuff
	{
		public static float CritChanceBonus => 0.10f;
		public static int GLPBonus => 5;
		public static int ABSBonus => 5;
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Rage, this);
		}

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Rage)
				return;

			player.GetCritChance(DamageClass.Generic) += CritChanceBonus;
			player.AsPred().GLP.Extra += GLPBonus;
			player.AsPred().ABS.Extra += ABSBonus;
		}

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.Rage)
				return;

			rare = ItemRarityID.Red;
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
	}
}

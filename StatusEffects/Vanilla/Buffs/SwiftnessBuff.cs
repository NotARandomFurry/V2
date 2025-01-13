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
	public class SwiftnessBuff : GlobalBuff
	{
		public static float MoveSpeedBonus => 0.25f;
		public static float StomachWeightReduction => 0.05f;
		public override void SetStaticDefaults()
		{
			V2.ModifiedStatusEffects.Add(BuffID.Swiftness, this);
		}

		public override bool RightClick(int type, int buffIndex) => type != BuffID.Swiftness;

		public override void Update(int type, Player player, ref int buffIndex)
		{
			if (type != BuffID.Wrath)
				return;

			player.moveSpeed += MoveSpeedBonus;
			player.AsPred().StomachWeightModifier *= 1f - StomachWeightReduction;
		}

		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (type != BuffID.Wrath)
				return;

			rare = ItemRarityID.Red;
			tip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Vanilla.Buffs.Swiftness.Description",
				new
				{
					SwiftnessBuffMoveSpeedBonus = MoveSpeedBonus.ToPercentage(2),
					SwiftnessBuffStomachWeightReduction = StomachWeightReduction.ToPercentage(2),
				}
			);
		}
	}
}

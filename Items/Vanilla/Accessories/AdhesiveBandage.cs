using Microsoft.Xna.Framework;
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
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Items.Vanilla.Accessories
{
	public class AdhesiveBandage : GlobalItem
	{
		public static float SoftenedBuildupReduction => 0.075f;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.AdhesiveBandage;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 90;
			item.AsFood().Size = 0.07;
			item.AsFood().AcidResistTier = 0;

			item.AsAnItem().AccessoryEffectCode += UpdateAdhesiveBandage;
		}

		public static void UpdateAdhesiveBandage(Item item, Player player, bool hideVisual)
		{
			player.buffImmune[BuffID.Bleeding] = true;
			player.AsFood().SoftenedDigestionDamageModifier *= 1f - SoftenedBuildupReduction;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.player[Main.myPlayer];
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Accessories.AdhesiveBandage",
				new
				{
					AdhesiveBandageSoftenedBuildupReduction = SoftenedBuildupReduction.ToPercentage(2),
				}
			);
		}
	}
}

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

namespace V2.Items.Vanilla.Accessories
{
	public class HealthRegenAndPotionCooldownBand : GlobalItem
	{
		public static double WornHealthRegenFlat => 1.0;
		public static double DigestingHealthRegenFlat => 2.0;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.CharmofMyths;

		public override void SetDefaults(Item item)
		{
			item.AsV2Item().AccessoryEffectCode = UpdateHealthRegenAndPotionCooldownBand;

			item.lifeRegen = 0;

			item.AsFood().MaxHealth = 1500;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().UpdateInStomach += UpdateInStomach;
		}

		public static void UpdateHealthRegenAndPotionCooldownBand(Item item, Player player, bool hideVisual)
		{
			player.AddHealthRegenEffect(
				healthPerSecond: WornHealthRegenFlat
			);

			player.pStone = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead && pred is Player player)
			{
				player.AddHealthRegenEffect(
					healthPerSecond: DigestingHealthRegenFlat
				);
			}
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.social)
				return;

			Player player = Main.player[Main.myPlayer];
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Accessories.HealthRegenAndPotionCooldownBand",
				new
				{
					HealthRegenBandIIWornHealthRegen = WornHealthRegenFlat,
				}
			);
		}
	}
}

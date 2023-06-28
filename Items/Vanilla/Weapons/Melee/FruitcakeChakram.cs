using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.StatusEffects.Debuffs;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class FruitcakeChakram : GlobalItem
	{
		public static int SwallowDamageToPred => 40;
		public static int PoisonTime => V2Utils.SensibleTime(seconds: 5);
		public static int WrathTime => V2Utils.SensibleTime(seconds: 5);
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.FruitcakeChakram;

		public override void SetDefaults(Item entity)
		{
			entity.AsFood().MaxHealth = 250;
			entity.AsFood().Size = 0.82;
			entity.AsFood().MealSizeTextOverride = "Despite its size, it makes for a terrible meal";

			entity.AsFood().OnSwallowDamage = 40;
			entity.AsFood().OnSwallowDeathReason = "{0} thought fruitcake was a good idea to eat. Ever.";
			entity.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(seconds: 10, frames: 0);

			entity.AsFood().UpdateInStomach += UpdateInStomach;
		}

		public static void UpdateInStomach(Item item, Entity pred, bool dead)
		{
			pred.AddStatus(BuffID.Poisoned, PoisonTime);
			pred.AddStatus(BuffID.Wrath, WrathTime);
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicTooltip(
				"Vanilla.Weapons.Melee.FruitcakeChakram",
				new
				{
					FruitcakeChakramSwallowDamage = item.AsFood().OnSwallowDamage,
					FruitcakeChakramSoreThroatTime = ((double)item.AsFood().OnSwallowSoreThroatTime / 60.0).CastToDecimalPlaces(2),
					FruitcakeChakramPoisonTime = ((double)PoisonTime / 60.0).CastToDecimalPlaces(2),
					FruitcakeChakramWrathTime = ((double)WrathTime / 60.0).CastToDecimalPlaces(2)
				}
			);
		}
	}
}

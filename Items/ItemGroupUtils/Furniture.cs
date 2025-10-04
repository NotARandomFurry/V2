using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Skilled;

namespace V2.Items.ItemGroupUtils
{
	public partial class Furniture : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => FurnitureDefinitionMappings.ContainsKey(entity.type);

		public override void SetDefaults(Item item)
		{
			item.AsFood().Size = FurnitureTypeMappings[FurnitureDefinitionMappings[item.type].Type].Size;
			item.AsFood().MaxHealth = (int)Math.Round((double)FurnitureTypeMappings[FurnitureDefinitionMappings[item.type].Type].BaseHealth * FurnitureMaterialMappings[FurnitureDefinitionMappings[item.type].Material].HealthMult);
			item.AsFood().AcidResistTier = FurnitureMaterialMappings[FurnitureDefinitionMappings[item.type].Material].AcidResist;

            item.AsFood().CalorieMultiplier = FurnitureMaterialMappings[FurnitureDefinitionMappings[item.type].Material].CalorieMult;
            item.AsFood().WellFedPower = FurnitureMaterialMappings[FurnitureDefinitionMappings[item.type].Material].WellFedPower;

			if (FurnitureDefinitionMappings[item.type].Material == FurnitureMaterial.Cactus)
            {
                item.AsFood().OnSwallowDamage = (int)Math.Ceiling(2.5 * item.AsFood().Size);
                item.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.CactusFurniture";
                item.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(seconds: 1, frames: item.AsFood().OnSwallowDamage * 20);
            }
            else if (FurnitureDefinitionMappings[item.type].Material == FurnitureMaterial.Crystal)
            {
                item.AsFood().OnSwallowDamage = (int)Math.Ceiling(1.5 * item.AsFood().Size);
                item.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.CrystalFurniture";
            }

            item.AsFood().OnSwallow += OnSwallow_GrantFurnitureGoals;
		}

		public static void OnSwallow_GrantFurnitureGoals(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
                ModContent.GetInstance<SwallowAnyFurniture>().TrySetCompletion(predPlayer);
		}
	}
}
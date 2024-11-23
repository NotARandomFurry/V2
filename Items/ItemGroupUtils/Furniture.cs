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

			item.AsFood().OnSwallow += OnSwallow_GrantFurnitureGoals;
		}

		public static void OnSwallow_GrantFurnitureGoals(Item item, Entity pred)
		{
			if (pred is not Player predPlayer)
				return;

			// furniture goal handlin' here
		}
	}
}
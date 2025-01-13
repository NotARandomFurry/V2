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
	public partial class Furniture
	{
		public enum FurnitureMaterial
		{
			OakWood,
			BorealWood,
			PalmWood,
			Ebonwood,
			Shadewood,
			JungleWood,
			Pearlwood,
			AshWood,
			SpookyWood,
			Bamboo,
			Cactus,
			Pumpkin,
			Mushroom,
			Marble,
			Granite,
			Sandstone,
			Ice,
			Coral,
			Golden,
			Crystal,
		}
		public static Dictionary<FurnitureMaterial, (double HealthMult, int AcidResist)> FurnitureMaterialMappings => new Dictionary<FurnitureMaterial, (double HealthMult, int AcidResist)>
		{
			{ FurnitureMaterial.OakWood,             ( 1.000, 0) },
			{ FurnitureMaterial.BorealWood,          ( 1.015, 0) },
			{ FurnitureMaterial.PalmWood,            ( 1.025, 0) },
			{ FurnitureMaterial.Ebonwood,            ( 1.050, 0) },
			{ FurnitureMaterial.Shadewood,           ( 1.050, 0) },
			{ FurnitureMaterial.JungleWood,          ( 1.080, 0) },
			{ FurnitureMaterial.Pearlwood,           ( 1.080, 0) },
			{ FurnitureMaterial.AshWood,             ( 0.925, 0) },
			{ FurnitureMaterial.SpookyWood,          ( 1.850, 0) },
			{ FurnitureMaterial.Bamboo,              ( 1.085, 0) },
			{ FurnitureMaterial.Cactus,              ( 1.100, 0) },
			{ FurnitureMaterial.Pumpkin,             ( 0.985, 0) },
			{ FurnitureMaterial.Mushroom,            ( 1.035, 0) },
			{ FurnitureMaterial.Marble,              ( 2.125, 1) },
			{ FurnitureMaterial.Granite,             ( 2.215, 1) },
			{ FurnitureMaterial.Sandstone,           ( 2.150, 0) },
			{ FurnitureMaterial.Ice,              ( 0.950, 0) },
			{ FurnitureMaterial.Coral,                ( 1.250, 0) },
			{ FurnitureMaterial.Golden,              ( 3.750, 2) },
			{ FurnitureMaterial.Crystal,             ( 1.400, 1) },
		};
	}
}
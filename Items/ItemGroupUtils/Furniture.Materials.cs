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
			Crystal,
		}
		public static Dictionary<FurnitureMaterial, (double HealthMult, int AcidResist)> FurnitureMaterialMappings => new Dictionary<FurnitureMaterial, (double HealthMult, int AcidResist)>
		{
			{ FurnitureMaterial.OakWood,             ( 1.000, 0) },
			{ FurnitureMaterial.BorealWood,          ( 1.015, 0) },
			{ FurnitureMaterial.PalmWood,            ( 1.025, 0) },
			{ FurnitureMaterial.Ebonwood,         ( 1.050, 0) },
			{ FurnitureMaterial.Shadewood,         ( 1.050, 0) },
			{ FurnitureMaterial.JungleWood,          ( 1.080, 0) },
			{ FurnitureMaterial.Crystal,             ( 1.400, 1) },
		};
	}
}
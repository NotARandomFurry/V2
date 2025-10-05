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
			DynastyWood,
			LivingWood,
			Bamboo,
			Cactus,
			Pumpkin,
			Mushroom,
			Marble,
			Granite,
			Sandstone,
			Ice,
			Coral,
			Skyware,
			Meteorite,
			Obsidian,
			Dungeon,
			Lesion,
			Flesh,
			Bone,
			Golden,
			Crystal,
			Glass,
			Balloon,
			Slime,
			Honey,
			Spider,
			Steampunk,
			Lihzahrd,
			Martian,
			Solar,
			Vortex,
			Nebula,
			Stardust,
			Stone,
			Metal
		}
		public static Dictionary<FurnitureMaterial, (double HealthMult, int AcidResist, double CalorieMult, double WellFedPower)> FurnitureMaterialMappings => new Dictionary<FurnitureMaterial, (double HealthMult, int AcidResist, double CalorieMult, double WellFedPower)>
		{
			{ FurnitureMaterial.OakWood,		( 1.000, 0, 1, 0) },
			{ FurnitureMaterial.BorealWood,		( 1.015, 0, 1, 0) },
			{ FurnitureMaterial.PalmWood,		( 1.025, 0, 1, 0) },
			{ FurnitureMaterial.Ebonwood,		( 1.050, 0, 1, 0) },
			{ FurnitureMaterial.Shadewood,		( 1.050, 0, 1, 0) },
			{ FurnitureMaterial.JungleWood,		( 1.080, 0, 1, 0) },
			{ FurnitureMaterial.Pearlwood,		( 1.080, 0, 1, 0) },
			{ FurnitureMaterial.AshWood,		( 0.925, 0, 1, 0) },
			{ FurnitureMaterial.SpookyWood,		( 1.850, 0, 1, 0) },
            { FurnitureMaterial.DynastyWood,	( 0.970, 0, 1, 0) },
            { FurnitureMaterial.LivingWood,		( 0.980, 0, 1, 0) },
            { FurnitureMaterial.Bamboo,			( 1.085, 0, 1, 0) },
			{ FurnitureMaterial.Cactus,			( 1.100, 0, 1, 0) },
			{ FurnitureMaterial.Pumpkin,		( 0.985, 0, 1, 0) },
			{ FurnitureMaterial.Mushroom,		( 1.035, 0, 1.5, 0.15) },
			{ FurnitureMaterial.Marble,			( 2.125, 1, 1, 0) },
			{ FurnitureMaterial.Granite,		( 2.215, 1, 1, 0) },
			{ FurnitureMaterial.Sandstone,		( 2.150, 0, 1, 0) },
			{ FurnitureMaterial.Ice,			( 0.950, 0, 1, 0) },
			{ FurnitureMaterial.Coral,			( 1.250, 0, 1, 0) },
            { FurnitureMaterial.Skyware,		( 1.575, 0, 1, 0) },
            { FurnitureMaterial.Meteorite,		( 2.750, 0, 1, 0) },
            { FurnitureMaterial.Obsidian,		( 5.250, 0, 1, 0) },
            { FurnitureMaterial.Dungeon,		( 6.250, 0, 1, 0) },
            { FurnitureMaterial.Lesion,			( 1.100, 0, 1, -1) },
            { FurnitureMaterial.Flesh,			( 1.075, 0, 1, -0.2) },
            { FurnitureMaterial.Bone,			( 1.9, 0, 0.85, -0.66) },
            { FurnitureMaterial.Golden,			( 3.750, 2, 1, 0.15) },
			{ FurnitureMaterial.Crystal,		( 1.400, 1, 1.2, 0.2) },
            { FurnitureMaterial.Glass,			( 1.350, 0, 1, 0) },
            { FurnitureMaterial.Balloon,		( 0.85, 0, 0.33, -0.1) },
            { FurnitureMaterial.Slime,			( 0.875, 0, 1, 0.1) },
            { FurnitureMaterial.Honey,			( 0.890, 0, 1.5, 0.75) },
            { FurnitureMaterial.Spider,			( 0.900, 0, 1, -0.33) },
            { FurnitureMaterial.Steampunk,		( 2.950, 0, 1, 0) },
            { FurnitureMaterial.Lihzahrd,		( 85.000, 2, 1, 0) },
            { FurnitureMaterial.Martian,		( 11.275, 0, 1, 0) },
            { FurnitureMaterial.Solar,			( 18.770, 0, 3, 0.5) },
            { FurnitureMaterial.Vortex,			( 18.730, 0, 1.95, 0.25) },
            { FurnitureMaterial.Nebula,			( 18.690, 0, 2.65, 0.4) },
            { FurnitureMaterial.Stardust,		( 18.650, 0, 3, 0.5) },
            { FurnitureMaterial.Stone,			( 2.2, 1, 1, 0) },
            { FurnitureMaterial.Metal,			( 2.75, 2, 1, 0) },
        };
	}
}
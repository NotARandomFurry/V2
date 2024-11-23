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
		public enum FurnitureType
		{
			Bathtub,
			Bed,
			Bookcase,
			Candelabra,
			Candle,
			Chair,
			Chandelier,
			Chest,
			ChestTrapped,
			Clock,
			Door,
			DoorTall,
			Dresser,
			Fence,
			Lamp,
			Lantern,
			Piano,
			Platform,
			Sink,
			Sofa,
			Table,
			Toilet,
			Wall,
			WorkBench,
		}
		public static Dictionary<FurnitureType, (int BaseHealth, double Size)> FurnitureTypeMappings => new Dictionary<FurnitureType, (int BaseHealth, double Size)>
		{
			{ FurnitureType.Bathtub,         ( 600, 3.200) },
			{ FurnitureType.Bed,             ( 535, 2.700) },
			{ FurnitureType.Bookcase,        ( 725, 3.800) },
			{ FurnitureType.Candelabra,      ( 340, 0.725) },
			{ FurnitureType.Candle,          (  85, 0.280) },
			{ FurnitureType.Chair,           ( 185, 1.075) },
			{ FurnitureType.Chandelier,      ( 485, 1.150) },
			{ FurnitureType.Chest,           ( 270, 1.120) },
			{ FurnitureType.ChestTrapped,    ( 270, 1.120) },
			{ FurnitureType.Clock,           ( 320, 3.200) },
			{ FurnitureType.Door,            ( 250, 1.000) },
			{ FurnitureType.DoorTall,        ( 500, 1.700) },
			{ FurnitureType.Dresser,         ( 680, 3.850) },
			{ FurnitureType.Fence,           ( 335, 1.350) },
			{ FurnitureType.Lamp,            ( 210, 0.650) },
			{ FurnitureType.Lantern,         ( 220, 0.675) },
			{ FurnitureType.Piano,           (1000, 3.650) },
			{ FurnitureType.Platform,        (  40, 0.050) },
			{ FurnitureType.Sink,            ( 325, 1.850) },
			{ FurnitureType.Sofa,            ( 880, 4.000) },
			{ FurnitureType.Table,           ( 365, 2.150) },
			{ FurnitureType.Toilet,          ( 350, 1.440) },
			{ FurnitureType.Wall,            ( 500, 2.000) },
			{ FurnitureType.WorkBench,       ( 140, 0.550) },
		};
	}
}

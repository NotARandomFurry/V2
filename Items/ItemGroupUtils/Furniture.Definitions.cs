using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Skilled;

namespace V2.Items.ItemGroupUtils
{
	public partial class Furniture
	{
		public static Dictionary<int, (FurnitureType Type, FurnitureMaterial Material)> FurnitureDefinitionMappings => new Dictionary<int, (FurnitureType, FurnitureMaterial)>
		{
			// Oak Wood furniture
			{ ItemID.Bed,                            (FurnitureType.Bed,          FurnitureMaterial.OakWood) },
			{ ItemID.Bookcase,                       (FurnitureType.Bookcase,     FurnitureMaterial.OakWood) },
			{ ItemID.WoodenChair,                    (FurnitureType.Chair,        FurnitureMaterial.OakWood) },
			{ ItemID.Chest,                          (FurnitureType.Chest,        FurnitureMaterial.OakWood) },
			{ ItemID.Fake_Chest,                     (FurnitureType.ChestTrapped, FurnitureMaterial.OakWood) },
			{ ItemID.GrandfatherClock,               (FurnitureType.Clock,        FurnitureMaterial.OakWood) },
			{ ItemID.WoodenDoor,                     (FurnitureType.Door,         FurnitureMaterial.OakWood) },
			{ ItemID.TallGate,                       (FurnitureType.DoorTall,     FurnitureMaterial.OakWood) },
			{ ItemID.Dresser,                        (FurnitureType.Dresser,      FurnitureMaterial.OakWood) },
			{ ItemID.WoodenFence,                    (FurnitureType.Fence,        FurnitureMaterial.OakWood) },
			{ ItemID.Piano,                          (FurnitureType.Piano,        FurnitureMaterial.OakWood) },
			{ ItemID.WoodPlatform,                   (FurnitureType.Platform,     FurnitureMaterial.OakWood) },
			{ ItemID.WoodenSink,                     (FurnitureType.Sink,         FurnitureMaterial.OakWood) },
			{ ItemID.Sofa,                           (FurnitureType.Sofa,         FurnitureMaterial.OakWood) },
			{ ItemID.WoodenTable,                    (FurnitureType.Table,        FurnitureMaterial.OakWood) },
			{ ItemID.WoodWall,                       (FurnitureType.Wall,         FurnitureMaterial.OakWood) },
			{ ItemID.WorkBench,                      (FurnitureType.WorkBench,    FurnitureMaterial.OakWood) },
			
			// Boreal Wood furniture
			{ ItemID.BorealWoodBathtub,              (FurnitureType.Bathtub,      FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodBed,                  (FurnitureType.Bed,          FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodBookcase,             (FurnitureType.Bookcase,     FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodCandelabra,           (FurnitureType.Candelabra,   FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodCandle,               (FurnitureType.Candle,       FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodChair,                (FurnitureType.Chair,        FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodChest,                (FurnitureType.Chest,        FurnitureMaterial.BorealWood) },
			{ ItemID.Fake_BorealWoodChest,           (FurnitureType.ChestTrapped, FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodClock,                (FurnitureType.Clock,        FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodDoor,                 (FurnitureType.Door,         FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodDresser,              (FurnitureType.Dresser,      FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodFence,                (FurnitureType.Fence,        FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodLamp,                 (FurnitureType.Lamp,         FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodLantern,              (FurnitureType.Lantern,      FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodPiano,                (FurnitureType.Piano,        FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodPlatform,             (FurnitureType.Platform,     FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodSink,                 (FurnitureType.Sink,         FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodSofa,                 (FurnitureType.Sofa,         FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodTable,                (FurnitureType.Table,        FurnitureMaterial.BorealWood) },
			{ ItemID.ToiletBoreal,                   (FurnitureType.Toilet,       FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodWall,                 (FurnitureType.Wall,         FurnitureMaterial.BorealWood) },
			{ ItemID.BorealWoodWorkBench,            (FurnitureType.WorkBench,    FurnitureMaterial.BorealWood) },
			
			// Ebonwood furniture
			{ ItemID.EbonwoodBathtub,                (FurnitureType.Bathtub,      FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodBed,                    (FurnitureType.Bed,          FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodBookcase,               (FurnitureType.Bookcase,     FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodCandelabra,             (FurnitureType.Candelabra,   FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodCandle,                 (FurnitureType.Candle,       FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodChair,                  (FurnitureType.Chair,        FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodChest,                  (FurnitureType.Chest,        FurnitureMaterial.Ebonwood) },
			{ ItemID.Fake_EbonwoodChest,             (FurnitureType.ChestTrapped, FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodClock,                  (FurnitureType.Clock,        FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodDoor,                   (FurnitureType.Door,         FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodDresser,                (FurnitureType.Dresser,      FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodFence,                  (FurnitureType.Fence,        FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodLamp,                   (FurnitureType.Lamp,         FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodLantern,                (FurnitureType.Lantern,      FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodPiano,                  (FurnitureType.Piano,        FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodPlatform,               (FurnitureType.Platform,     FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodSink,                   (FurnitureType.Sink,         FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodSofa,                   (FurnitureType.Sofa,         FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodTable,                  (FurnitureType.Table,        FurnitureMaterial.Ebonwood) },
			{ ItemID.ToiletEbonyWood,                (FurnitureType.Toilet,       FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodWall,                   (FurnitureType.Wall,         FurnitureMaterial.Ebonwood) },
			{ ItemID.EbonwoodWorkBench,              (FurnitureType.WorkBench,    FurnitureMaterial.Ebonwood) },
			
			// Shadewood furniture
			{ ItemID.ShadewoodBathtub,               (FurnitureType.Bathtub,      FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodBed,                   (FurnitureType.Bed,          FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodBookcase,              (FurnitureType.Bookcase,     FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodCandelabra,            (FurnitureType.Candelabra,   FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodCandle,                (FurnitureType.Candle,       FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodChair,                 (FurnitureType.Chair,        FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodChest,                 (FurnitureType.Chest,        FurnitureMaterial.Shadewood) },
			{ ItemID.Fake_ShadewoodChest,            (FurnitureType.ChestTrapped, FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodClock,                 (FurnitureType.Clock,        FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodDoor,                  (FurnitureType.Door,         FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodDresser,               (FurnitureType.Dresser,      FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodFence,                 (FurnitureType.Fence,        FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodLamp,                  (FurnitureType.Lamp,         FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodLantern,               (FurnitureType.Lantern,      FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodPiano,                 (FurnitureType.Piano,        FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodPlatform,              (FurnitureType.Platform,     FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodSink,                  (FurnitureType.Sink,         FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodSofa,                  (FurnitureType.Sofa,         FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodTable,                 (FurnitureType.Table,        FurnitureMaterial.Shadewood) },
			{ ItemID.ToiletShadewood,                (FurnitureType.Toilet,       FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodWall,                  (FurnitureType.Wall,         FurnitureMaterial.Shadewood) },
			{ ItemID.ShadewoodWorkBench,             (FurnitureType.WorkBench,    FurnitureMaterial.Shadewood) },

			// Crystal furniture (how does that even work?)
			{ ItemID.CrystalBathtub,                 (FurnitureType.Bathtub,      FurnitureMaterial.Crystal) },
			{ ItemID.CrystalBed,                     (FurnitureType.Bed,          FurnitureMaterial.Crystal) },
			{ ItemID.CrystalBookCase,                (FurnitureType.Bookcase,     FurnitureMaterial.Crystal) },
			{ ItemID.CrystalCandelabra,              (FurnitureType.Candelabra,   FurnitureMaterial.Crystal) },
			{ ItemID.CrystalCandle,                  (FurnitureType.Candle,       FurnitureMaterial.Crystal) },
			{ ItemID.CrystalChair,                   (FurnitureType.Chair,        FurnitureMaterial.Crystal) },
			{ ItemID.CrystalChandelier,              (FurnitureType.Chandelier,   FurnitureMaterial.Crystal) },
			{ ItemID.CrystalChest,                   (FurnitureType.Chest,        FurnitureMaterial.Crystal) },
			{ ItemID.Fake_CrystalChest,              (FurnitureType.ChestTrapped, FurnitureMaterial.Crystal) },
			{ ItemID.CrystalClock,                   (FurnitureType.Clock,        FurnitureMaterial.Crystal) },
			{ ItemID.CrystalDoor,                    (FurnitureType.Door,         FurnitureMaterial.Crystal) },
			{ ItemID.CrystalDresser,                 (FurnitureType.Dresser,      FurnitureMaterial.Crystal) },
			{ ItemID.CrystalLamp,                    (FurnitureType.Lamp,         FurnitureMaterial.Crystal) },
			{ ItemID.CrystalLantern,                 (FurnitureType.Lantern,      FurnitureMaterial.Crystal) },
			{ ItemID.CrystalPiano,                   (FurnitureType.Piano,        FurnitureMaterial.Crystal) },
			{ ItemID.CrystalPlatform,                (FurnitureType.Platform,     FurnitureMaterial.Crystal) },
			{ ItemID.CrystalSink,                    (FurnitureType.Sink,         FurnitureMaterial.Crystal) },
			{ ItemID.CrystalSofaHowDoesThatEvenWork, (FurnitureType.Sofa,         FurnitureMaterial.Crystal) },
			{ ItemID.CrystalTable,                   (FurnitureType.Table,        FurnitureMaterial.Crystal) },
			{ ItemID.ToiletCrystal,                  (FurnitureType.Toilet,       FurnitureMaterial.Crystal) },
			{ ItemID.CrystalBlockWall,               (FurnitureType.Wall,         FurnitureMaterial.Crystal) },
			{ ItemID.CrystalWorkbench,               (FurnitureType.WorkBench,    FurnitureMaterial.Crystal) },
		};
	}
}

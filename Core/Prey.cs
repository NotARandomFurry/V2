using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.NPCs;
using V2.PlayerHandling;
using static V2.Core.FoodTypeTags;

namespace V2.Core
{
	/// <summary>
	/// Used to define what type of prey this is.
	/// </summary>
	public enum PreyType
	{
		Player,
		NPC,
		Projectile,
		Item,
		Custom,
		Undefined
	};

	public abstract class FoodTypeTag
	{
		public double TotalWeight
		{
			get
			{
				double fullWeight = 0.0;
				foreach ((string subtype, double weight) validSubtype in FoodSubtypeTags)
				{
					fullWeight += validSubtype.weight;
				}
				return fullWeight;
			}
		}

		public abstract string Name { get; }

		public abstract List<string> ValidFoodSubtypes { get; }

		public abstract List<(string subtype, double weight)> FoodSubtypeTags { get; set; }
	}

	/// <summary>
	/// Used to define various ease-of-indentification values to be assigned to each new food item that enters a predator's stomach.
	/// </summary>
	public static class FoodTypeTags
	{
		public class PlantTag : FoodTypeTag
		{
			public override string Name => "Plant";

			public override List<string> ValidFoodSubtypes => new List<string>
			{
				"Fruit",
				"Vegetable",
				"Bark",
				"Green",
			};
			public override List<(string subtype, double weight)> FoodSubtypeTags { get; set; }
		}
		public class MeatTag : FoodTypeTag
		{
			public override string Name => "Meat";

			public override List<string> ValidFoodSubtypes => new List<string>
			{
				"Steak",
				"Poultry",
				"Pork",
				"Human",
				"Lizard",
				"Insect",
				"Arachnid",
			};
			public override List<(string subtype, double weight)> FoodSubtypeTags { get; set; }
		}
		public class MetalTag : FoodTypeTag
		{
			public override string Name => "Metal";

			public override List<string> ValidFoodSubtypes => new List<string>
			{
				"Tin",
				"Copper",
				"Iron",
				"Lead",
				"Silver",
				"Tungsten",
				"Gold",
				"Platinum",
				"Cobalt",
				"Palladium",
				"Orichalcum",
				"Mythril",
				"Adamantite",
				"Titanium",
			};
			public override List<(string subtype, double weight)> FoodSubtypeTags { get; set; }
		}
		public class UndeadTag : FoodTypeTag
		{
			public override string Name => "Undead";

			public override List<string> ValidFoodSubtypes => new List<string>
			{
				"Zombie",
				"Skeleton",
			};
			public override List<(string subtype, double weight)> FoodSubtypeTags { get; set; }
		}
	}

	public static class FoodFlavorTags
	{

	}

	/// <summary>
	/// Used to store a reference to whatever's eaten a given prey entity.
	/// </summary>
	public struct PredEntityReference
	{
		public Entity Predator { get; set; }
		public Prey PreyInstance { get; set; }
	}

	public class Prey
	{
		public PreyType Type { get; set; }
		public int Index { get; set; }
		public int EntityID { get; set; }
		public List<FoodTypeTag> TypeTags { get; set; }
		public bool Dead { get; set; }
		public double WeightLeftToDigest { get; set; }

		public int timeSpentInStomach;

		public Prey(Entity preyEntity)
		{
			if (preyEntity is Player player)
			{
				Type = PreyType.Player;
				Index = player.whoAmI;
				TypeTags = new List<FoodTypeTag>
				{
					new MeatTag()
					{
						FoodSubtypeTags = new List<(string subtype, double weight)>
						{
							("Human", 1.0),
						}
					}
				};
			}
			else if (preyEntity is NPC npc)
			{
				Type = PreyType.NPC;
				Index = npc.whoAmI;
				EntityID = npc.type;
				TypeTags = npc.AsPrey().FoodTypeTags ?? null;
			}

			Dead = false;
			WeightLeftToDigest = GetInitialPreyWeight();
			timeSpentInStomach = 0;
		}

		public double GetInitialPreyWeight()
		{
			switch (Type)
			{
				case PreyType.Player:
					return 1.0;
				case PreyType.NPC:
					NPC actualPrey = Main.npc[Index];
					if (actualPrey.AsPrey().PreyBaseSizeOverrideMethod is not null)
						return actualPrey.AsPrey().PreyBaseSizeOverrideMethod.Invoke(actualPrey);
					else if (TypeTags is not null)
					{
						double preyWeight = 0.0;
						foreach (FoodTypeTag foodTypeTag in TypeTags)
						{
							foreach ((string subtype, double weight) subtypeTag in foodTypeTag.FoodSubtypeTags)
							{
								preyWeight += subtypeTag.weight;
							}
						}
						return preyWeight;
					}
					else
					{
						double refPlayerWidth = 20.0;
						double refPlayerHeight = 42.0;
						double playerToNPCWidthRatio = (double)actualPrey.width / refPlayerWidth;
						double playerToNPCHeightRatio = (double)actualPrey.height / refPlayerHeight;
						return playerToNPCWidthRatio * playerToNPCHeightRatio;
					}
				case PreyType.Projectile:
					return 1.0;
				case PreyType.Item:
					return 1.0;
				default:
					V2.Instance.Logger.Error("the type of the currently-weighed prey isn't recognized. I'll return its weight as 1.0 for now, but please be more careful");
					return 1.0;
			}
		}

		public double GetPreyWeight() => WeightLeftToDigest;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Items;
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
		Liquid,
		Custom,
		Undefined
	};

	public abstract class FoodTypeTag
	{
		public double TotalSize
		{
			get
			{
				double fullWeight = 0.0;
				foreach ((string subtype, double weight) in FoodSubtypeTags)
				{
					fullWeight += weight;
				}
				return fullWeight;
			}
		}

		public abstract string Name { get; }

		public abstract List<string> ValidFoodSubtypes { get; }

		public List<(string subtype, double weight)> FoodSubtypeTags { get; set; }
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
		}
		public class UndeadTag : FoodTypeTag
		{
			public override string Name => "Undead";

			public override List<string> ValidFoodSubtypes => new List<string>
			{
				"Zombie",
				"Skeleton",
			};
		}
		public class LiquidTag : FoodTypeTag
		{
			public override string Name => "Liquid";

			public override List<string> ValidFoodSubtypes => new List<string>
			{
				"Water",
				"Lava",
				"Honey",
				"Shimmer",
			};
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
		public Entity Instance { get; set; }
		public List<FoodTypeTag> TypeTags { get; set; }
		public bool NoHealth { get; set; }
		public bool InventoryItem { get; set; }
		public double InitialWeight { get; set; }
		public double InitialSize { get; set; }
		public double WeightLeftToDigest { get; set; }
		public double SizeLeftToDigest => WeightLeftToDigest / InitialWeight * InitialSize;

		public int timeSpentInStomach;

		public Prey(Entity preyEntity)
		{
			if (preyEntity is Player player)
			{
				Type = PreyType.Player;
				Instance = player;
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
				Instance = npc;
				TypeTags = npc.AsFood().FoodTypeTags ?? null;
			}
			else if (preyEntity is Item item)
			{
				Type = PreyType.Item;
				Instance = item;
				TypeTags = item.AsFood().FoodTypeTags ?? null;
			}

			NoHealth = false;
			InitialWeight = InitialSize = WeightLeftToDigest = GetInitialPreySize(this);
			timeSpentInStomach = 0;
		}

		public Prey(int liquidType, int liquidAmount)
		{
			Type = PreyType.Liquid;
			Instance = null;
			NoHealth = true;
			TypeTags = new List<FoodTypeTag>
			{
				new LiquidTag() {
					FoodSubtypeTags = new List<(string subtype, double weight)>
					{
						(
							liquidType switch
							{
								LiquidID.Water => "Water",
								LiquidID.Lava => "Lava",
								LiquidID.Honey => "Honey",
								LiquidID.Shimmer => "Shimmer",
								_ => throw new NotImplementedException(),
							},
							liquidAmount
						)
					}
				}
			};
			InitialWeight = liquidAmount;
		}

		public static double GetInitialPreySize(Entity entity) => GetInitialPreySize(new Prey(entity));

		public static double GetInitialPreySize(Prey prey)
		{
			switch (prey.Type)
			{
				case PreyType.Player:
					return 1.0;
				case PreyType.NPC:
					NPC actualPreyNPC = prey.Instance as NPC;
					double actualPreyWeight = 0;
					if (actualPreyNPC.AsFood().PreyBaseSizeOverrideMethod is not null)
						actualPreyWeight = actualPreyNPC.AsFood().PreyBaseSizeOverrideMethod.Invoke(actualPreyNPC);
					else if (prey.TypeTags is not null)
					{
						double preyWeight = 0.0;
						foreach (FoodTypeTag foodTypeTag in prey.TypeTags)
						{
							foreach ((string subtype, double weight) in foodTypeTag.FoodSubtypeTags)
							{
								preyWeight += weight;
							}
						}
						actualPreyWeight = preyWeight;
					}
					else
					{
						double refPlayerWidth = 20.0;
						double refPlayerHeight = 42.0;
						double playerToNPCWidthRatio = (double)actualPreyNPC.width / refPlayerWidth;
						double playerToNPCHeightRatio = (double)actualPreyNPC.height / refPlayerHeight;
						actualPreyWeight = playerToNPCWidthRatio * playerToNPCHeightRatio;
					}
					return actualPreyWeight;
				case PreyType.Projectile:
					return 1.0;
				case PreyType.Item:
					Item actualPreyItem = prey.Instance as Item;
					return actualPreyItem.CalculateSnackSize();
				case PreyType.Custom:
					return 1.0;
				default:
					V2.Instance.Logger.Error("the type of the currently-weighed prey isn't recognized. I'll return its weight as 1.0 for now, but please be more careful");
					return 1.0;
			}
		}
	}
}

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
		public string ExactType { get; set; }
		public string Name { get; set; }
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
				ExactType = "Player";
				Name = player.name;
			}
			else if (preyEntity is NPC npc)
			{
				Type = PreyType.NPC;
				Instance = npc;
				ExactType = npc.TypeName;
				Name = npc.GivenName;
			}
			else if (preyEntity is Item item)
			{
				Type = PreyType.Item;
				Instance = item;
				ExactType = item.AffixName();
			}

			NoHealth = false;
			InitialWeight = InitialSize = WeightLeftToDigest = GetInitialPreySize(this);
			timeSpentInStomach = 0;
		}

		public Prey(int liquidType, int liquidAmount)
		{
			double liquidAmountReal = liquidAmount / 256.0 * (liquidType switch
			{
				LiquidID.Lava => 4.0,
				LiquidID.Honey => 1.5,
				LiquidID.Shimmer => 0.75,
				_ => 1.0,
			});
			Type = PreyType.Liquid;
			Instance = null;
			NoHealth = true;
			ExactType = liquidType switch
			{
				LiquidID.Water => "Water",
				LiquidID.Lava => "Lava",
				LiquidID.Honey => "Honey",
				LiquidID.Shimmer => "Shimmer",
				_ => "Some Other Liquid",
			};
			InitialWeight = InitialSize = WeightLeftToDigest = liquidAmountReal;
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
					if (actualPreyNPC.AsFood().Size != 0)
						actualPreyWeight = actualPreyNPC.AsFood().Size;
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

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
using V2.PlayerHandling.PredPlayerGoals.Intermediate;

namespace V2.Items.ItemGroupUtils
{
	public class RegularBanners : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => V2Utils.ItemIDSets.RegularBanners.Contains(entity.type);

		public override void SetDefaults(Item item)
		{
			item.AsFood().Size = 0.95;
			item.AsFood().MaxHealth = 175;
		}
	}
	public class EnemyBanners : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => V2Utils.ItemIDSets.EnemyBanners.Contains(entity.type);

		public override void SetDefaults(Item item)
		{
			item.AsFood().Size = 0.95;
			item.AsFood().MaxHealth = 175;
			//item.AsFood().OnBreak += OnBreak;
		}
		/*public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			if (pred is Player predPlayer && (item.type == ItemID.RainbowDye || item.type == ItemID.IntenseRainbowDye))
			{
				ModContent.GetInstance<EatRainbowDye>().TrySetCompletion(predPlayer);
			}
			return true;
		}*/
	}
}

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
	public class LargeGem : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => V2Utils.ItemIDSets.LargeGems.Contains(entity.type);

		public override void SetDefaults(Item item)
		{
			item.AsFood().Size = 0.875;
			item.AsFood().MaxHealth = 5000;
			item.AsFood().AcidResistTier = 1;

			item.AsFood().OnBreak += OnBreak_GrantLargeGemMultiPreyGoal;
		}

		public static bool OnBreak_GrantLargeGemMultiPreyGoal(Item item, Entity pred, bool direct)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<EatLargeGem>().TrySetCompletion(predPlayer);
				List<int> distinctLargeGems = [.. V2Utils.ItemIDSets.LargeGems];
				int distinctLargeGemsInTummy = 1;
				distinctLargeGems.Remove(item.type);
				foreach (PreyData prey in predPlayer.AsPred().StomachTracker.Prey)
				{
					if (prey.Type != PreyType.Item)
						continue;

					if (!prey.NoHealth)
						continue;

					int preyItemID = prey.ExactType;
					if (distinctLargeGems.Contains(preyItemID))
					{
						distinctLargeGemsInTummy++;
						distinctLargeGems.Remove(preyItemID);
					}
				}
				if (distinctLargeGemsInTummy >= 7)
					ModContent.GetInstance<HoardLargeGems>().TrySetCompletion(predPlayer);
			}
			return true;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Amateur
{
	public class DrinkLava : PredPlayerGoal
	{
		public override string InternalName => "DrinkLava";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.DrinkLava.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.DrinkLava.Description";
		public override bool Available(Player pred) =>
			pred.AsV2Player().HasVisitedLocation("hell")
		 || pred.HasItemInInventoryOrOpenVoidBag(ItemID.LavaBucket)
		 || pred.HasItemInInventoryOrOpenVoidBag(ItemID.LavaCharm)
		 || Complete(pred);

		public override int StatPointsFromCompletion => 9;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

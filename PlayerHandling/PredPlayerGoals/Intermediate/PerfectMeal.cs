using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Intermediate
{
	public class PerfectMeal : PredPlayerGoal
	{
		public override string InternalName => "PerfectMeal";

		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.PerfectMeal.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.PerfectMeal.Description";

		public override int StatPointsFromCompletion => 16;

		public override ProgressionStage Stage => ModContent.GetInstance<IntermediateStage>();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Beginner
{
	public class EatHelpfulFairy : PredPlayerGoal
	{
		public override string InternalName => "EatHelpfulFairy";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatHelpfulFairy.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatHelpfulFairy.Description";

		public override int StatPointsFromCompletion => 7;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

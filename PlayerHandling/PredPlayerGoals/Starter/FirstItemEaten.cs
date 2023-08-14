using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace V2.PlayerHandling.PredPlayerGoals.Starter
{
	public class FirstItemEaten : PredPlayerGoal
	{
		public override string DisplayName(Player pred) => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.FirstItemEaten.Name");
		public override string Description(Player pred) => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.FirstItemEaten.Description");

		public override int StatPointsFromCompletion => 1;

		public override ProgressionStage Stage => ProgressionStage.Starter;
	}
}

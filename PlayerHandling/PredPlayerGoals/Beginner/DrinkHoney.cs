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
	public class DrinkHoney : PredPlayerGoal
	{
		public override string InternalName => "DrinkHoney";
		public override string DisplayName(Player pred) => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.DrinkHoney.Name");
		public override string Description(Player pred) => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.DrinkHoney.Description");

		public override int StatPointsFromCompletion => 2;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

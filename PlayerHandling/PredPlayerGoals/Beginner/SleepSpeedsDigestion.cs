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
	public class SleepSpeedsDigestion : PredPlayerGoal
	{
		public static double FlatFullnessThreshold => 0.45;
		public override string InternalName => "SleepSpeedsDigestion";
		public override string DisplayName(Player pred) => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.SleepSpeedsDigestion.Name");
		public override string Description(Player pred) => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.SleepSpeedsDigestion.Description");

		public override int StatPointsFromCompletion => 1;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

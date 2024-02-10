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
	public class TooFull : PredPlayerGoal
	{
		public static double FullnessThreshold => 0.90;
		public static int TimeThreshold => V2Utils.SensibleTime(minutes: 1);
		public override string InternalName => "TooFull";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.TooFull.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.TooFull.Description";
		public override bool HasClearDescription(Player pred) => true;

		public override int StatPointsFromCompletion => 1;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals
{
	public static class PredPlayerGoalLoader
	{
		internal static List<PredPlayerGoal> PredPlayerGoals = new List<PredPlayerGoal>();
		internal static List<ProgressionStage> ProgressionStages = new List<ProgressionStage>();

		internal static void Load()
		{
			PredPlayerGoals = new List<PredPlayerGoal>()
			{
				PredPlayerGoal.FirstLivePrey,
			};
		}
	}
}

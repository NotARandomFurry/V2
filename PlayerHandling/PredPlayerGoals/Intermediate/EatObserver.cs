using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Amateur
{
	public class EatObserver : PredPlayerGoal
	{
		public override string InternalName => "EatObserver";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.EatObserver.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.EatObserver.Description";
		public override bool HasClearDescription(Player pred) => true;

		public override int StatPointsFromCompletion => 15;

		public override ProgressionStage Stage => ModContent.GetInstance<IntermediateStage>();
	}
}

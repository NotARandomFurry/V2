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
	public class EatShark : PredPlayerGoal
	{
		public override string InternalName => "EatShark";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.EatShark.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.EatShark.Description";
		public override bool Available(Player pred) => pred.AsV2Player().HasVisitedLocation("beach") || Complete(pred);

		public override int StatPointsFromCompletion => 7;

		public override ProgressionStage Stage => ModContent.GetInstance<IntermediateStage>();
	}
}

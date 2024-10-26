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
	public class EatSandElemental : PredPlayerGoal
	{
		public override string InternalName => "EatSandElemental";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.EatSandElemental.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.EatSandElemental.Description";
		public override bool HasClearDescription(Player pred) => true;
		public override bool Available(Player pred) => (Main.hardMode && pred.AsV2Player().HasVisitedLocation("sandstorm")) || Complete(pred);

		public override int StatPointsFromCompletion => 20;

		public override ProgressionStage Stage => ModContent.GetInstance<IntermediateStage>();
	}
}

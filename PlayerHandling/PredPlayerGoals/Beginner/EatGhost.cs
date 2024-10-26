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
	public class EatGhost : PredPlayerGoal
	{
		public override string InternalName => "EatGhost";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatGhost.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatGhost.Description";
		public override bool HasClearDescription(Player pred) => true;
		public override bool Available(Player pred) => pred.AsV2Player().HasVisitedLocation("graveyard") || Complete(pred);
		public override int StatPointsFromCompletion => 2;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

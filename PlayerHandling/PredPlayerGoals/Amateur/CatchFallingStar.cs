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
	public class CatchFallingStar : PredPlayerGoal
	{
		public override string InternalName => "CatchFallingStar";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.CatchFallingStar.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.CatchFallingStar.Description";
		public override bool Available(Player pred) => pred.AsV2Player().HasVisitedLocation("nighttime") || Complete(pred);

		public override int StatPointsFromCompletion => 5;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

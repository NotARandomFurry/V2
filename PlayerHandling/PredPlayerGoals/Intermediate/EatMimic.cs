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
	public class EatMimic : PredPlayerGoal
	{
		public override string InternalName => "EatMimic";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.EatMimic.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.EatMimic.Description";

		public override int StatPointsFromCompletion => 12;

		public override ProgressionStage Stage => ModContent.GetInstance<IntermediateStage>();
	}
}

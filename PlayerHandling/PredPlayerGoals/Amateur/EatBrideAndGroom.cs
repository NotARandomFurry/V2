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
	public class EatBrideAndGroom : PredPlayerGoal
	{
		public override string InternalName => "EatBrideAndGroom";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatBrideAndGroom.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatBrideAndGroom.Description";
		public override bool HasClearDescription(Player pred) => true;
		public override bool Available(Player pred) => Main.bloodMoon || Complete(pred);

		public override int StatPointsFromCompletion => 10;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

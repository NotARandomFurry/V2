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
	public class LongGolf : PredPlayerGoal
	{
		public override string InternalName => "LongGolf";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.LongGolf.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.LongGolf.Description";

		public override int StatPointsFromCompletion => 5;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

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
	public class EatLargeGem : PredPlayerGoal
	{
		public override string InternalName => "EatLargeGem";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatLargeGem.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatLargeGem.Description";

		public override int StatPointsFromCompletion => 7;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

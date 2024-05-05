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
	public class EatSnowFlinx : PredPlayerGoal
	{
		public override string InternalName => "EatSnowFlinx";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatSnowFlinx.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatSnowFlinx.Description";
		public override bool HasClearDescription(Player pred) => true;

		public override int StatPointsFromCompletion => 2;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

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
	public class EatPinky : PredPlayerGoal
	{
		public override string InternalName => "EatPinky";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatPinky.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatPinky.Description";

		public override int StatPointsFromCompletion => 2;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Beginner
{
	internal class SwallowAnyFurniture : PredPlayerGoal
	{
		public override string InternalName => "SwallowAnyFurniture";

		public override int StatPointsFromCompletion => 3;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();

		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.SwallowAnyFurniture.Description";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.SwallowAnyFurniture.Name";
	}
}

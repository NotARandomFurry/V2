using Terraria;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Beginner
{
	public class EatManEater : PredPlayerGoal
	{
		public override string InternalName => "EatManEater";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatManEater.Name";

		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatManEater.Description";

		public override int StatPointsFromCompletion => 2;
		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}
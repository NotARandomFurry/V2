using Terraria;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Beginner
{
	public class EatTheManEater : PredPlayerGoal
	{
		public override string InternalName => "EatTheManEater";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatTheManEater.Name";

		public override string Description(Player pred) =>
			"Mods.V2.PredPlayerGoals.Beginner.EatTheManEater.Description";

		public override int StatPointsFromCompletion => 2;
		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Starter
{
	public class FirstLivePrey : PredPlayerGoal
	{
		public override string InternalName => "FirstLivePrey";
		public override string DisplayName(Player pred) => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.FirstLivePrey.Name");
		public override string Description(Player pred) => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.FirstLivePrey.Description");
		public override bool HasClearDescription(Player pred) => true;

		public override int StatPointsFromCompletion => 1;

		public override ProgressionStage Stage => ModContent.GetInstance<StarterStage>();
	}
}

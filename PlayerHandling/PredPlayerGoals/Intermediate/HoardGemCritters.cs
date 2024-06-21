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
	public class HoardGemCritters : PredPlayerGoal
	{
		public override string InternalName => "HoardGemCritters";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.HoardGemCritters.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.HoardGemCritters.Description";
		public override bool HasClearDescription(Player pred) => true;

		public override int StatPointsFromCompletion => 11;

		public override ProgressionStage Stage => ModContent.GetInstance<IntermediateStage>();
	}
}

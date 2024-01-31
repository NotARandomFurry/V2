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
	public class EatNymph : PredPlayerGoal
	{
		public override string InternalName => "EatNymph";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatNymph.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatNymph.Description";

		public override int StatPointsFromCompletion => 3;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

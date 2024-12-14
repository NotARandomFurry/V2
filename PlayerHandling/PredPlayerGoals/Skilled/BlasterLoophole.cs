using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Skilled
{
	public class BlasterLoophole : PredPlayerGoal
	{
		public override string InternalName => "BlasterLoophole";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.BlasterLoophole.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.BlasterLoophole.Description";

		public override int StatPointsFromCompletion => 3;

		public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
	}
}

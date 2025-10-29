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
	public class Recycler : PredPlayerGoal
	{
		public override string InternalName => "Recycler";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.Recycler.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.Recycler.Description";

		public override int StatPointsFromCompletion => 22;

		public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
	}
}

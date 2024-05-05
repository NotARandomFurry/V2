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
	public class EatFirstLifeCrystal : PredPlayerGoal
	{
		public override string InternalName => "EatFirstLifeCrystal";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatFirstLifeCrystal.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatFirstLifeCrystal.Description";
		public override bool HasClearDescription(Player pred) => true;

		public override int StatPointsFromCompletion => 1;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

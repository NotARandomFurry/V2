using Microsoft.Xna.Framework.Graphics;
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
	public class EatAngyFlower : PredPlayerGoal
	{
		public override string InternalName => "EatAngyFlower";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatAngyFlower.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatAngyFlower.Description";
		public override bool Available(Player pred) => pred.AsV2Player().HasVisitedLocation("windy_day") || Complete(pred);

		public override int StatPointsFromCompletion => 3;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

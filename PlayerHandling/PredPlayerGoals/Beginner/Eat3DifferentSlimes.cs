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
	public class Eat3DifferentSlimes : PredPlayerGoal
	{
		public override string InternalName => "Eat3DifferentSlimes";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.Eat3DifferentSlimes.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.Eat3DifferentSlimes.Description";

		public override int StatPointsFromCompletion => 5;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

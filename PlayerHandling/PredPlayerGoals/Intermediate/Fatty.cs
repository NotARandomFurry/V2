using Microsoft.Xna.Framework.Graphics;
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
	public class Fatty : PredPlayerGoal
	{
		public override string InternalName => "Fatty";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.Fatty.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Intermediate.Fatty.Description";
        public override bool Available(Player pred) => HasCompleted(pred, "Chunky") || Complete(pred);

        public override int StatPointsFromCompletion => 16;

		public override ProgressionStage Stage => ModContent.GetInstance<IntermediateStage>();
	}
}

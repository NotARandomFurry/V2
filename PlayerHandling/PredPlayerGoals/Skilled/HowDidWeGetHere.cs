using Microsoft.Xna.Framework.Graphics;
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
	public class HowDidWeGetHere : PredPlayerGoal
	{
		public override string InternalName => "HowDidWeGetHere";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.HowDidWeGetHere.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.HowDidWeGetHere.Description";
        public override bool Available(Player pred) => HasCompleted(pred, "HaveWeGoneTooFar") || Complete(pred);

        public override int StatPointsFromCompletion => 50;

		public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
	}
}

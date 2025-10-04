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
	public class HaveWeGoneTooFar : PredPlayerGoal
	{
		public override string InternalName => "HaveWeGoneTooFar";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.HaveWeGoneTooFar.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.HaveWeGoneTooFar.Description";
        public override bool Available(Player pred) => HasCompleted(pred, "MajorConsequences") || Complete(pred);

        public override int StatPointsFromCompletion => 36;

		public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
	}
}

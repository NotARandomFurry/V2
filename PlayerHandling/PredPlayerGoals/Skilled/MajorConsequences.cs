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
	public class MajorConsequences : PredPlayerGoal
	{
		public override string InternalName => "MajorConsequences";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.MajorConsequences.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.MajorConsequences.Description";
		public override bool Available(Player pred) => HasCompleted(pred, "Fatty") || Complete(pred);

		public override int StatPointsFromCompletion => 25;

		public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
	}
}

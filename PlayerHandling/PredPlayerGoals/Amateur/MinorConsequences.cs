using Microsoft.Xna.Framework.Graphics;
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
	public class MinorConsequences : PredPlayerGoal
	{
		public override string InternalName => "MinorConsequences";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.MinorConsequences.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.MinorConsequences.Description";
		public override bool Available(Player pred) => HasCompleted(pred, "BecomeSomeoneElse") || Complete(pred);

		public override int StatPointsFromCompletion => 2;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

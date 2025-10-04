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
	public class Chunky : PredPlayerGoal
	{
		public override string InternalName => "Chunky";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.Chunky.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.Chunky.Description";
        public override bool Available(Player pred) => HasCompleted(pred, "MinorConsequences") || Complete(pred);

        public override int StatPointsFromCompletion => 5;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

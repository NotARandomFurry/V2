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
	public class BecomeTheRatGirl : PredPlayerGoal
	{
		public override string InternalName => "BecomeTheRatGirl";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.BecomeTheRatGirl.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.BecomeTheRatGirl.Description";

		public override int StatPointsFromCompletion => 3;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

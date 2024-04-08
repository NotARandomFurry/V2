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
	public class EatGoldCritter : PredPlayerGoal
	{
		public override string InternalName => "EatGoldCritter";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatGoldCritter.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatGoldCritter.Description";
		public override bool HasClearDescription(Player pred) => true;
		public override int StatPointsFromCompletion => 3;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

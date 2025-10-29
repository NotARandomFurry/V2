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
	public class BecomeSomeoneElse : PredPlayerGoal
	{
		public override string InternalName => "BecomeSomeoneElse";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.BecomeSomeoneElse.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.BecomeSomeoneElse.Description";

		public override int StatPointsFromCompletion => 1;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

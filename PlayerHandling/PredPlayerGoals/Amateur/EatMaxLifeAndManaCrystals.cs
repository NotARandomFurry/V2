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
	public class EatMaxLifeAndManaCrystals : PredPlayerGoal
	{
		public override string InternalName => "EatMaxLifeAndManaCrystals";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatMaxLifeAndManaCrystals.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatMaxLifeAndManaCrystals.Description";
		public override bool HasClearDescription(Player pred) => true;

		public override int StatPointsFromCompletion => 3;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

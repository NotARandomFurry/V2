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
	public class DigestWithVoodoo : PredPlayerGoal
	{
		public override string InternalName => "DigestWithVoodoo";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.DigestWithVoodoo.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.DigestWithVoodoo.Description";

		public override int StatPointsFromCompletion => 10;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

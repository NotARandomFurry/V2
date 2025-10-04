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
	public class DigestTempleBrick : PredPlayerGoal
	{
		public override string InternalName => "DigestTempleBrick";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.DigestTempleBrick.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.DigestTempleBrick.Description";
		public override bool Available(Player pred) => (Main.hardMode && pred.AsV2Player().HasVisitedLocation("temple")) || Complete(pred);

		public override int StatPointsFromCompletion => 13;

		public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
	}
}

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
	public class EatReaper : PredPlayerGoal
	{
		public override string InternalName => "EatReaper";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.EatReaper.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.EatReaper.Description";
		public override bool Available(Player pred) => (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && pred.AsV2Player().HasVisitedLocation("eclipse")) || Complete(pred);

		public override int StatPointsFromCompletion => 14;

		public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
	}
}

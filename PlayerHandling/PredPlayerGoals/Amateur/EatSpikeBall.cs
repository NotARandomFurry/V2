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
	public class EatSpikeBall : PredPlayerGoal
	{
		public override string InternalName => "EatSpikeBall";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatSpikeBall.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatSpikeBall.Description";
		public override bool Available(Player pred) => (NPC.downedBoss3 && pred.AsV2Player().HasVisitedLocation("dungeon")) || Complete(pred);

		public override int StatPointsFromCompletion => 6;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

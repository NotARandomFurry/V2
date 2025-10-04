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
	public class EatMothronEgg : PredPlayerGoal
	{
		public override string InternalName => "EatMothronEgg";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.EatMothronEgg.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.EatMothronEgg.Description";
		public override bool Available(Player pred) => (NPC.downedPlantBoss && pred.AsV2Player().HasVisitedLocation("eclipse")) || Complete(pred);

		public override int StatPointsFromCompletion => 12;

		public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
	}
}

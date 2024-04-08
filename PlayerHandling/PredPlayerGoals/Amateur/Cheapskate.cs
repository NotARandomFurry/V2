using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Amateur
{
	public class Cheapskate : PredPlayerGoal
	{
		public override string InternalName => "Cheapskate";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.Cheapskate.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.Cheapskate.Description";
		public override bool HasClearDescription(Player pred) => true;
		public override bool Available(Player pred) => NPC.AnyNPCs(NPCID.Nurse) || Complete(pred);

		public override int StatPointsFromCompletion => 3;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

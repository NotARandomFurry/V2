using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Items.Voraria.Weapons.Ranged;

namespace V2.PlayerHandling.PredPlayerGoals.Amateur
{
	public class LongGolf : PredPlayerGoal
	{
		public override string InternalName => "LongGolf";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.LongGolf.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.LongGolf.Description";
        public override bool Available(Player pred) => NPC.AnyNPCs(NPCID.Golfer) || Complete(pred);

        public override int StatPointsFromCompletion => 5;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

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
    public class EatSquid : PredPlayerGoal
    {
        public override string InternalName => "EatSquid";
        public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatSquid.Name";
        public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatSquid.Description";
        public override bool HasClearDescription(Player pred) => true;
        public override bool Available(Player pred) => pred.AsV2Player().HasVisitedLocation("beach") || Complete(pred);
        public override int StatPointsFromCompletion => 4;

        public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
    }
}

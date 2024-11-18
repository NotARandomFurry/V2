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
    public class HoardLargeGems : PredPlayerGoal
    {
        public override string InternalName => "HoardLargeGems";
        public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.HoardLargeGems.Name";
        public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.HoardLargeGems.Description";

        public override int StatPointsFromCompletion => 24;

        public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
    }
}

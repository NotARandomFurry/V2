using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Items.Voraria.Weapons.Ranged;

namespace V2.PlayerHandling.PredPlayerGoals.Amateur
{
    public class BlasterLoophole : PredPlayerGoal
    {
        public override string InternalName => "BlasterLoophole";
        public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.BlasterLoophole.Name";
        public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.BlasterLoophole.Description";
        public override bool Available(Player pred) => pred.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<DinnerBlaster>());
        public override int StatPointsFromCompletion => 3;

        public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
    }
}

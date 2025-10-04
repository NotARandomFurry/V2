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
<<<<<<< Updated upstream:PlayerHandling/PredPlayerGoals/Skilled/TrulyStuffed.cs
    public class TrulyStuffed : PredPlayerGoal
    {
        public override string InternalName => "TrulyStuffed";
        public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.TrulyStuffed.Name";
        public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.TrulyStuffed.Description";

        public override int StatPointsFromCompletion => 15;
=======
	public class Recycler : PredPlayerGoal
	{
		public override string InternalName => "Recycler";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.Recycler.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Skilled.Recycler.Description";

		public override int StatPointsFromCompletion => 22;
>>>>>>> Stashed changes:PlayerHandling/PredPlayerGoals/Skilled/Recycler.cs

        public override ProgressionStage Stage => ModContent.GetInstance<SkilledStage>();
    }
}

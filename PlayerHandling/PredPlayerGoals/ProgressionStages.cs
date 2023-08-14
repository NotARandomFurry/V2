using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;

namespace V2.PlayerHandling.PredPlayerGoals
{
	public class StarterStage : ProgressionStage
	{
		public override string DisplayName => Language.GetTextValue("Mods.V2.PredPlayerGoals.NewPred.Name");
		public override string DisplaySubtitle => Language.GetTextValue("Mods.V2.PredPlayerGoals.NewPred.Subtitle");
		public override string Description => Language.GetTextValue("Mods.V2.PredPlayerGoals.NewPred.Description");
	}
}

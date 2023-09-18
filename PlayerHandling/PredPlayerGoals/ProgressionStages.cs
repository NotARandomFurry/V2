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
		public override string DisplayName => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.Name");
		public override string DisplaySubtitle => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.Subtitle");
		public override string Description => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.Description");
		public override double Order => 0.0;
	}
	public class BeginnerStage : ProgressionStage
	{
		public override string DisplayName => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.Name");
		public override string DisplaySubtitle => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.Subtitle");
		public override string Description => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.Description");
		public override double Order => 1.0;
	}
	public class AmateurStage : ProgressionStage
	{
		public override string DisplayName => Language.GetTextValue("Mods.V2.PredPlayerGoals.Amateur.Name");
		public override string DisplaySubtitle => Language.GetTextValue("Mods.V2.PredPlayerGoals.Amateur.Subtitle");
		public override string Description => Language.GetTextValue("Mods.V2.PredPlayerGoals.Amateur.Description");
		public override double Order => 2.0;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling.PredPlayerGoals.Starter;

namespace V2.PlayerHandling.PredPlayerGoals
{
	public class StarterStage : ProgressionStage
	{
		public override string DisplayName => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.Name");
		public override string DisplaySubtitle => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.Subtitle");
		public override string Description => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.Description");
		public override string FooterAdvice => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.FooterAdvice");
		public override bool Available(Player pred) => true;
		public override string UnlockCondition => Language.GetTextValue("Mods.V2.PredPlayerGoals.Starter.UnlockCondition");
		public override double Order => 0.0;
	}
	public class BeginnerStage : ProgressionStage
	{
		public override string DisplayName => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.Name");
		public override string DisplaySubtitle => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.Subtitle");
		public override string Description => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.Description");
		public override string FooterAdvice => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.FooterAdvice");
		public override bool Available(Player pred)
			=> ModContent.GetInstance<FirstLivePrey>().Complete(pred)
			&& ModContent.GetInstance<FirstItemEaten>().Complete(pred)
			&& ModContent.GetInstance<FirstDrink>().Complete(pred);
		public override string UnlockCondition => Language.GetTextValue("Mods.V2.PredPlayerGoals.Beginner.UnlockCondition");
		public override double Order => 1.0;
	}
	public class AmateurStage : ProgressionStage
	{
		public override string DisplayName => Language.GetTextValue("Mods.V2.PredPlayerGoals.Amateur.Name");
		public override string DisplaySubtitle => Language.GetTextValue("Mods.V2.PredPlayerGoals.Amateur.Subtitle");
		public override string Description => Language.GetTextValue("Mods.V2.PredPlayerGoals.Amateur.Description");
		public override string FooterAdvice => Language.GetTextValue("Mods.V2.PredPlayerGoals.Amateur.FooterAdvice");
		public override bool Available(Player pred) => NPC.downedSlimeKing || NPC.downedBoss1;
		public override string UnlockCondition => Language.GetTextValue("Mods.V2.PredPlayerGoals.Amateur.UnlockCondition");
		public override double Order => 2.0;
	}
	public class IntermediateStage : ProgressionStage
	{
		public override string DisplayName => Language.GetTextValue("Mods.V2.PredPlayerGoals.Intermediate.Name");
		public override string DisplaySubtitle => Language.GetTextValue("Mods.V2.PredPlayerGoals.Intermediate.Subtitle");
		public override string Description => Language.GetTextValue("Mods.V2.PredPlayerGoals.Intermediate.Description");
		public override string FooterAdvice => Language.GetTextValue("Mods.V2.PredPlayerGoals.Intermediate.FooterAdvice");
		public override bool Available(Player pred) => NPC.downedBoss3;
		public override string UnlockCondition => Language.GetTextValue("Mods.V2.PredPlayerGoals.Intermediate.UnlockCondition");
		public override double Order => 3.0;
	}
	public class SkilledStage : ProgressionStage
	{
		public override string DisplayName => Language.GetTextValue("Mods.V2.PredPlayerGoals.Skilled.Name");
		public override string DisplaySubtitle => Language.GetTextValue("Mods.V2.PredPlayerGoals.Skilled.Subtitle");
		public override string Description => Language.GetTextValue("Mods.V2.PredPlayerGoals.Skilled.Description");
		public override string FooterAdvice => Language.GetTextValue("Mods.V2.PredPlayerGoals.Skilled.FooterAdvice");
		public override bool Available(Player pred) => NPC.downedMechBossAny;
		public override string UnlockCondition => Language.GetTextValue("Mods.V2.PredPlayerGoals.Skilled.UnlockCondition");
		public override double Order => 4.0;
	}
}

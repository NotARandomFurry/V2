using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Beginner
{
	public class EatPinky : PredPlayerGoal
	{
		public override string InternalName => "EatPinky";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatPinky.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.EatPinky.Description";
		public override Texture2D IncompleteTexture => ModContent.Request<Texture2D>("V2/PlayerHandling/PredPlayerGoals/Beginner/EatPinky_Incomplete").Value;
		public override Texture2D CompleteTexture => ModContent.Request<Texture2D>("V2/PlayerHandling/PredPlayerGoals/Beginner/EatPinky_Complete").Value;

		public override int StatPointsFromCompletion => 2;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

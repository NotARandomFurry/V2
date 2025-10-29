using Microsoft.Xna.Framework;
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
	public class BLUH : PredPlayerGoal
	{
		public override string InternalName => "BLUH";
		public override Color DisplayNameColor(Player pred)
		{
			return Color.Red;
		}
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.BLUH.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Beginner.BLUH.Description";

		public override int StatPointsFromCompletion => 1;

		public override ProgressionStage Stage => ModContent.GetInstance<BeginnerStage>();
	}
}

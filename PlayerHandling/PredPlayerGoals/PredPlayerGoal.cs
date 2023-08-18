using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.PlayerHandling.PredPlayerGoals.Starter;

namespace V2.PlayerHandling.PredPlayerGoals
{
	public abstract class PredPlayerGoal : ModType
	{
		public static FirstLivePrey FirstLivePrey { get; set; } = new FirstLivePrey();
		public static FirstItemEaten FirstItemEaten { get; set; } = new FirstItemEaten();

		protected sealed override void Register()
		{
			ModTypeLookup<PredPlayerGoal>.Register(this);

			PredPlayerGoalLoader.PredPlayerGoals.Add(this);
		}

		public abstract string DisplayName(Player pred);

		public abstract string Description(Player pred);

		public abstract int StatPointsFromCompletion { get; }

		public abstract ProgressionStage Stage { get; }

		public virtual bool Available(Player pred) => true;
	}
}

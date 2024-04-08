using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals
{
	public abstract class ProgressionStage : ModType
	{
		protected sealed override void Register()
		{
			ModTypeLookup<ProgressionStage>.Register(this);

			PredPlayerGoalLoader.ProgressionStages.Add(this);
		}

		public abstract string DisplayName { get; }
		public abstract string DisplaySubtitle { get; }
		public abstract string Description { get; }
		public abstract string FooterAdvice { get; }
		public abstract bool Available(Player pred);
		public abstract double Order { get; }
	}
}

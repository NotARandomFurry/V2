using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace V2.Core.StruggleSystem
{
	public abstract class StruggleChart
	{
		public static ProceduralStruggleChart Default => new ProceduralStruggleChart();

		public VoreTracker ConnectedTracker { get; set; }
		public bool ForPredator { get; set; }

		public double DifficultyCoeff
		{
			get
			{
				double predTUM = ConnectedTracker.Predator.GetPredStat("TUM");
				double preyCombinedSTR = ConnectedTracker.TotalPreySTR;
				if (ForPredator)
				{
					double predDiff = preyCombinedSTR / Math.Max(1.0, (double)predTUM);
					if (predDiff < 0.6)
						predDiff = 0.6;
					if (predDiff > 2.0)
						predDiff = 2.0;

					return predDiff;
				}
				else
				{

					double preyDiff = predTUM / Math.Max(1.0, (double)preyCombinedSTR);
					if (preyDiff < 0.6)
						preyDiff = 0.6;
					if (preyDiff > 2.0)
						preyDiff = 2.0;

					return preyDiff;
				}
			}
		}

		public abstract List<StruggleChartNote[]> Notes { get; }

		public virtual void OnStartup() { }

		public void RefreshPressedNotes()
		{
			if (Notes is null)
				return;

			foreach (StruggleChartNote[] noteSet in Notes)
			{
				if (noteSet is null)
					continue;

				if (noteSet.FirstOrDefault(x => x is not null) is null)
					continue;

				foreach (StruggleChartNote note in noteSet)
				{
					if (note is null)
						continue;

					if (!note.CorrectlyPressed)
						continue;

					note.PressAnimTimer++;
					if (note.PressAnimTimer > 180)
					{
						note.CorrectlyPressed = false;
						note.PressedPosition = 0.0;
						note.PressAnimTimer = 0;
					}
				}
			}
		}
	}
}

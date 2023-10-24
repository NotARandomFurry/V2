using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace V2.Core.StruggleSystem
{
	public class WeightableStruggleChart : StruggleChart
	{
		public override List<StruggleChartNote[]> Notes => RandomNoteSpan;
		private List<StruggleChartNote[]> RandomNoteSpan { get; set; }
		private static int MaxRandomNoteSpanLength => 128;

		public override void OnStartup()
		{
			RandomNoteSpan = new List<StruggleChartNote[]>();
			if (ConnectedTracker is null)
				return;

			bool isForPrey = ConnectedTracker.PredatorStruggleChart != this;
			double predTUM = ConnectedTracker.Predator.GetPredStat("TUM");
			double preyCombinedSTR = ConnectedTracker.TotalPreySTR;
			for (int i = 0; i < MaxRandomNoteSpanLength; i++)
			{
				double randomDifficultyFactor = 0.5f + Main.rand.NextDouble();
				if (isForPrey)
					randomDifficultyFactor *= predTUM / preyCombinedSTR;
				else
					randomDifficultyFactor /= predTUM / preyCombinedSTR;

				StruggleChartNote[] noteSet = new StruggleChartNote[5] { null, null, null, null, null };
				double noteAmount = Main.rand.NextDouble() * randomDifficultyFactor;
				if (noteAmount >= 1.0f)
				{
					List<NoteLane> lanes = new List<NoteLane> { NoteLane.Up, NoteLane.Left, NoteLane.Right, NoteLane.Down };
					NoteLane noteLaneToFill = Main.rand.NextFromCollection(lanes);
					RandomNoteSpan[i][(int)noteLaneToFill] = new StruggleChartNote(noteLaneToFill);
					lanes.Remove(noteLaneToFill);
					if (noteAmount >= 1.0f)
					{
						noteLaneToFill = Main.rand.NextFromCollection(lanes);
						RandomNoteSpan[i][(int)noteLaneToFill] = new StruggleChartNote(noteLaneToFill);
						lanes.Remove(noteLaneToFill);
					}
				}

				RandomNoteSpan.Add(noteSet);
			}
		}
	}
}

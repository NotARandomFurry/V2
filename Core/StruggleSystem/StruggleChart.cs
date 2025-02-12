using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace V2.Core.StruggleSystem
{
	public abstract class StruggleChart : ModType
	{
		public VoreTracker ConnectedTracker { get; set; }
		public bool ForPredator { get; set; }

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
					if (note.PressAnimTimer > 70)
					{
						note.CorrectlyPressed = false;
						note.PressedPosition = 0.0;
						note.PressAnimTimer = 0;
					}
				}
			}
		}

		protected sealed override void Register()
		{
			ModTypeLookup<StruggleChart>.Register(this);

			StruggleChartLoader.RegisterChart(this);
		}
	}
}

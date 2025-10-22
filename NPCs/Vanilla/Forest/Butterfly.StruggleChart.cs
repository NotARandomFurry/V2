using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V2.Core.StruggleSystem;

namespace V2.NPCs.Vanilla.Forest
{
	public static partial class NormalButterflyStuff
	{
		public class NormalButterflyStruggleChart : StruggleChart
		{
			public override double ProgressRate => 4.0f;
			public override double NoteSpacingFactor => 1.0f;
			public override List<StruggleChartNote[]> Notes =>
			[
				[ new StruggleChartNote(NoteDirection.Up) ],
				[ null ],
				[ null ],
				[ null ],
			];
		}
	}
}

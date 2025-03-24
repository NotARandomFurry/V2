using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2.Core.StruggleSystem
{
	public class EmptyStruggleChart : StruggleChart
	{
		public override List<StruggleChartNote[]> Notes =>
		[
			[ null ],
			[ null ],
			[ null ],
			[ null ],
			[ null ],
			[ null ],
			[ null ],
			[ null ],
		];
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace V2.Core.StruggleSystem
{
	public class StruggleChart : ModType
	{
		public List<StruggleChartNote[]> Notes { get; private set; }

		/// <summary>
		/// The beats per minute that this struggle chart follows.
		/// </summary>
		public double BPM { get; private set; }

		protected override void Register()
		{
			
		}
	}
}

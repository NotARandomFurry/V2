using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace V2.Core.StruggleSystem
{
	public static class StruggleChartLoader
	{
		internal static List<StruggleChart> StruggleCharts { get; set; }

		public static void Load()
		{
			StruggleCharts = new List<StruggleChart>();
		}

		public static void Unload()
		{
			StruggleCharts = null;
		}
	}
}

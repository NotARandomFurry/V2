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
		public VoreTracker ConnectedTracker => ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(x => x.PredatorChart == this || x.PreyCharts.Contains(this));
		public abstract List<StruggleChartNote[]> Notes { get; }

		protected override void Register()
		{
			ModTypeLookup<StruggleChart>.Register(this);
			StruggleChartLoader.StruggleCharts.Add(this);
		}

		public virtual void OnStartup() { }
	}
}

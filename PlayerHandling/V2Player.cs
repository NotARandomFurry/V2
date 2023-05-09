using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using V2.UI;

namespace V2.PlayerHandling
{
	public class V2Player : ModPlayer
	{
		public List<DelegateGeneralItemDrawingUI> generalItemUIDrawMethods;

		public override void ResetEffects()
		{
			generalItemUIDrawMethods = new List<DelegateGeneralItemDrawingUI>();
		}
	}
}

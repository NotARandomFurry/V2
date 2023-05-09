using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using V2.UI;

namespace V2.Items
{
	public class V2Item : GlobalItem
	{
		public DelegateHeldItemDrawingUI heldItemUIDrawMethod;

		public override bool InstancePerEntity => true;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.PlayerHandling;
using V2.UI;

namespace V2.Items
{
	public class GeneralTaggableItem : GlobalItem
	{
		public override bool InstancePerEntity => true;

		// BY TYPE
		public bool Sword => Broadsword || Shortsword;
		public bool Broadsword { get; set; }
		public bool Shortsword { get; set; }
		public bool Bow { get; set; }
		public bool Gun { get; set; }
		public bool NormalFood { get; set; }
		public bool NormalDrink { get; set; }

		public GeneralTaggableItem()
		{
			Broadsword = false;
			Shortsword = false;
			Bow = false;
			Gun = false;
			NormalFood = false;
			NormalDrink = false;
		}
	}
}

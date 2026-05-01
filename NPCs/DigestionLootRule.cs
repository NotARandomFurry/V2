using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace V2.NPCs
{
	/// <summary>
	/// Used to define rules for stealing items from digested NPCs.<br/>
	/// </summary>
	public struct DigestionLootRule
	{
		public delegate int GetItemType(NPC npc, Entity pred);
		public GetItemType ItemType { get; set; }
		public delegate int GetItemAmount(NPC npc, Entity pred);
		public GetItemAmount ItemAmount { get; set; }
		public delegate double GetItemChance(NPC npc, Entity pred);
		public GetItemChance ItemChance { get; set; }

		public DigestionLootRule(GetItemType type, GetItemAmount amount, GetItemChance chance)
		{
			ItemType = type;
			ItemAmount = amount;
			ItemChance = chance;
		}
	}
}

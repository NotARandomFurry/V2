using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using V2.PlayerHandling;

namespace V2.NPCs.Vanilla.TownNPCs
{
	public static class V2ShopConditions
	{
		public static readonly Condition ShopOwnerHasEatenWellRecently =	new Condition("Mods.V2.Conditions.FullNPC",				() => PredNPC.GetCurrentBellyWeight(Main.LocalPlayer.TalkNPC) > 0.0);
		public static readonly Condition BeginnerStatPoints =				new Condition("Mods.V2.Conditions.BeginnerStatPoints",	() => Main.LocalPlayer.AsPred().TotalStatPoints >= 10);
	}
}

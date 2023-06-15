using BetterDialogue.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.NPCs.Vanilla.TownNPCs.Mechanic.ChatButtons
{
	public class MechanicShopButtonModification : GlobalChatButton
	{
		public override bool PreClick(ChatButton chatButton, NPC npc, Player player)
		{
			if (chatButton != ChatButton.Shop || npc.type != NPCID.Mechanic)
				return true;

			if (player.IsFoodFor(npc, out bool pastTense) && !pastTense)
			{
				Main.npcChatText = Main.bloodMoon
					? "Cessate your pointless, mind-numbing questions. Why should, or WOULD, I engage in commerce with any battery of mine?"
					: "Unfortunately, any attempt to retrieve money from my digestive system would result in failure, as it's efficient enough to melt down metal alongside meat like yourself.";
				return false;
			}
			return true;
		}
	}
}

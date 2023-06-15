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

namespace V2.NPCs.Vanilla.TownNPCs.Stylist.ChatButtons
{
	public class StylistShopButtonModification : GlobalChatButton
	{
		public override bool PreClick(ChatButton chatButton, NPC npc, Player player)
		{
			if (chatButton != ChatButton.Shop || npc.type != NPCID.Stylist)
				return true;

			if (player.IsFoodFor(npc, out bool pastTense) && !pastTense)
			{
				Main.npcChatText = Main.bloodMoon
					? "The hell do you think you're gonna be able to buy in there? You're a snack, not a client, and it's not like you'll be needing anything I could sell you where you're going..."
					: "Sorry, can't really sell you anything while I'm giving you a gut cut! Maybe later, after your cut's done, I'll getcha some of my deliciously dazzling hair dyes to spruce up your scalp!";
				return false;
			}
			return true;
		}
	}
}

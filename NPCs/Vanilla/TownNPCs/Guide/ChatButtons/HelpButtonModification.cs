using BetterDialogue.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.NPCs.Vanilla.TownNPCs.Guide.ChatButtons
{
	public class HelpButtonModification : GlobalChatButton
	{
		public override bool PreClick(ChatButton chatButton, NPC npc, Player player)
		{
			if (Main.hardMode)
			{
				Main.chatText = "Nope. Not anymore. You're on your own.";
				return false;
			}

			return true;
		}
	}
}

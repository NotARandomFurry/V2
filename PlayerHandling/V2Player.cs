using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.NPCs;
using V2.UI;

namespace V2.PlayerHandling
{
	public class V2Player : ModPlayer
	{
		public List<DelegateGeneralItemDrawingUI> generalItemUIDrawMethods;

		public override void ResetEffects()
		{
			generalItemUIDrawMethods = new List<DelegateGeneralItemDrawingUI>();

			if (Player.whoAmI != Main.myPlayer)
				return;

			if (Player.talkNPC != -1)
			{
				NPC npc = Player.TalkNPC;
				if (npc.AsFood().IsCurrentlyEaten)
					Main.CloseNPCChatOrSign();
			}
		}
	}
}

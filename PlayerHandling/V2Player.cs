using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.UI;

namespace V2.PlayerHandling
{
	public partial class V2Player : ModPlayer
	{
		public List<DelegateGeneralItemDrawingUI> generalItemUIDrawMethods;

		public int GuideHelpText = 0;

		public override void Initialize()
		{
			ResetHealthRegenTime();
			ResetHealthRegenEffectList();
		}

		public override void ResetEffects()
		{
			generalItemUIDrawMethods = new List<DelegateGeneralItemDrawingUI>();
			setBonusActive = false;
			setBonusShouldBeDisplayed = false;

			if (Player.whoAmI != Main.myPlayer)
				return;

			if (Player.talkNPC != -1)
			{
				NPC npc = Player.TalkNPC;
				if (npc.CurrentCaptor() is not null)
					Main.CloseNPCChatOrSign();
			}

			ResetHealthRegenEffectList();
		}

		public override void UpdateDead()
		{
			ResetHealthRegenTime();
			ResetHealthRegenEffectList();
		}

		public override void PostUpdateMiscEffects()
		{
			HandleSittingAndSleepingHealthRegenEffect();
		}
	}
}

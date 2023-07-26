using BetterDialogue.UI;
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

namespace V2.NPCs.Vanilla.TownNPCs.Nurse.ChatButtons
{
	public class TummyHealButton : ChatButton
	{
		public override string Text(NPC npc, Player player) => "In-Stomach";

		public override double Priority => NurseHeal.Priority + 0.01;

		public override bool IsActive(NPC npc, Player player) => npc.type == NPCID.Nurse && npc.AsNurse().healTypeChoice;

		public override void OnClick(NPC npc, Player player)
		{
			if (npc.AsPred().stomachContents.FirstOrDefault(x => x.Type == PreyType.NPC && (x.Instance as NPC).type == NPCID.ArmsDealer) is not null)
				Main.npcChatText = "My stomach's currently helping a very specific patient; one that needs to be properly quarantined for a little while. Wait until he's feeling better, and I'm sure I'll be able to squeeze you in there.";
			else if (npc.AsNurse().healPlayerIndex != -1 && !npc.AsNurse().digestScamPatient)
				Main.npcChatText = "I've currently got someone else in there getting healed. You'll have to wait your turn. Maybe make sure you've actually got the money while you're waiting...";
			else if (PredNPC.CanSwallow(npc, Main.LocalPlayer))
			{
				List<string> possibleGutHealLines = new List<string>
				{
					"...alright, then. Note that you'll be staying in there until the treatment's done; overtime costs extra, too.",
					"You...SHOULD have enough to pay me. Fine, you can stay in me for a little while.",
					"...if you make me and my gut work too much overtime, you'll be on a one-way trip to bulking up my butt...hopefully. Got it?",
				};
				PredNPC.SwallowWithTextIfApplicable(
					npc,
					Main.LocalPlayer,
					Main.rand.NextFromCollection(possibleGutHealLines) + "\n"
				  + "[c/7F7F7F:<" + npc.GivenName + "'s stomach rumbles and grumbles in anticipation as she nonchalantly pulls you headfirst into her mouth, soon depositing you into her waiting stomach. There don't seem to be any acids for the minute, and it's actually quite pleasant in here, though you realize she didn't ask for upfront payment...>]"
				);
				npc.AsNurse().healPlayerIndex = Main.myPlayer;
			}
			else
				Main.npcChatText = "...I'm afraid I'm a bit too full of food to keep you inside me at the moment. Give me a little bit to digest, and I'm sure I'll be able to squeeze you in.";

			npc.AsNurse().healTypeChoice = false;
		}
	}
}

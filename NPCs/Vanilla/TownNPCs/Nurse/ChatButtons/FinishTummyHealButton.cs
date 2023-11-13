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

namespace V2.NPCs.Vanilla.TownNPCs.Nurse.ChatButtons
{
	public class FinishTummyHealButton : ChatButton
	{
		public override string Text(NPC npc, Player player)
		{
			string buttonText = "Finish Healing";
			if (npc.AsNurse().healOvertime > 0)
			{
				int originalPrice = (int)(npc.AsNurse().originalHealPrice * 0.80) + npc.AsNurse().healOvertime;
				PlayerLoader.ModifyNursePrice(Main.LocalPlayer, npc, 0, false, ref originalPrice);
				int platOvertimeFee = 0;
				int goldOvertimeFee = 0;
				int silverOvertimeFee = 0;
				int copperOvertimeFee = 0;
				if (originalPrice >= 1000000)
				{
					platOvertimeFee = originalPrice / 1000000;
					originalPrice -= platOvertimeFee * 1000000;
				}

				if (originalPrice >= 10000)
				{
					goldOvertimeFee = originalPrice / 10000;
					originalPrice -= goldOvertimeFee * 10000;
				}

				if (originalPrice >= 100)
				{
					silverOvertimeFee = originalPrice / 100;
					originalPrice -= silverOvertimeFee * 100;
				}

				if (originalPrice >= 1)
					copperOvertimeFee = originalPrice;

				if (originalPrice > 0)
				{
					buttonText += " (";
					if (platOvertimeFee > 0)
						buttonText += platOvertimeFee + " " + Lang.inter[15].Value + " ";

					if (goldOvertimeFee > 0)
						buttonText += goldOvertimeFee + " " + Lang.inter[16].Value + " ";

					if (silverOvertimeFee > 0)
						buttonText += silverOvertimeFee + " " + Lang.inter[17].Value + " ";

					if (copperOvertimeFee > 0)
						buttonText += copperOvertimeFee + " " + Lang.inter[18].Value + " ";
					buttonText += ")";
				}
			}
			return buttonText;
		}

		public override double Priority => NurseHeal.Priority;

		public override Color? OverrideColor(NPC npc, Player player)
		{
			if (npc.AsNurse().healOvertime > 0)
			{
				int originalPrice = (int)(npc.AsNurse().originalHealPrice * 0.80) + npc.AsNurse().healOvertime;
				int platOvertimeFee = 0;
				int goldOvertimeFee = 0;
				int silverOvertimeFee = 0;
				int copperOvertimeFee = 0;
				if (originalPrice >= 1000000)
				{
					platOvertimeFee = originalPrice / 1000000;
					originalPrice -= platOvertimeFee * 1000000;
				}

				if (originalPrice >= 10000)
				{
					goldOvertimeFee = originalPrice / 10000;
					originalPrice -= goldOvertimeFee * 10000;
				}

				if (originalPrice >= 100)
				{
					silverOvertimeFee = originalPrice / 100;
					originalPrice -= silverOvertimeFee * 100;
				}

				if (originalPrice >= 1)
					copperOvertimeFee = originalPrice;

				float num11 = (float)(int)Main.mouseTextColor / 255f;
				if (platOvertimeFee > 0)
					return new Color((byte)(220f * num11), (byte)(220f * num11), (byte)(198f * num11), Main.mouseTextColor);
				else if (goldOvertimeFee > 0)
					return new Color((byte)(224f * num11), (byte)(201f * num11), (byte)(92f * num11), Main.mouseTextColor);
				else if (silverOvertimeFee > 0)
					return new Color((byte)(181f * num11), (byte)(192f * num11), (byte)(193f * num11), Main.mouseTextColor);
				else if (copperOvertimeFee > 0)
					return new Color((byte)(246f * num11), (byte)(138f * num11), (byte)(96f * num11), Main.mouseTextColor);
			}

			return Color.Gray;
		}

		public override bool IsActive(NPC npc, Player player) => npc.type == NPCID.Nurse && player.IsFoodFor(npc, out bool pastTense) && !pastTense && npc.AsNurse().healPlayerIndex == player.whoAmI;

		public override void OnClick(NPC npc, Player player)
		{
			if (npc.AsNurse().healOvertime > 0)
			{
				if (Main.LocalPlayer.BuyItem((int)(npc.AsNurse().originalHealPrice * 0.80) + npc.AsNurse().healOvertime))
				{
					PredNPC.GetStomachTracker(npc).Prey.RemoveAll(x => x.Type == PreyType.Player && !x.NoHealth && (x.Instance as Player).whoAmI == Main.CurrentPlayer.whoAmI);
					npc.AsNurse().healPlayerIndex = -1;
					npc.AsNurse().originalHealPrice = 0;
					npc.AsNurse().healOvertime = 0;
					npc.AsNurse().digestScamPatient = false;
					Main.npcChatText = "Ready to get out, then? Feels like you've got enough to pay in there, too. Alright, give me a moment...\n"
									 + "[c/7F7F7F:<" + npc.GivenName + "'s stomach begins convulsing rhythmically as you begin to be forced back up her throat and out of her mouth. Once you're safely out of her stomach, she takes her payment before you have the chance to give it to her yourself.>]\n"
									 + "There you go. Good as new, and you didn't even need to get melted into butt fat. Be grateful I gave you that much, and don't ask for a lollipop.";
				}
				else
				{
					npc.AsNurse().originalHealPrice = 0;
					npc.AsNurse().healOvertime = 0;
					npc.AsNurse().digestScamPatient = true;
					Main.npcChatText = "Really? Trying to undercut ME on healing? Why, you little prick...\n"
									 + "[c/7F7F7F:<As if on cue, " + npc.GivenName + "'s stomach roars to life, acids already starting to flood in to melt you down just as readily as she'd healed you up only moments prior.>]\n"
									 + "Hope you enjoyed the \"free\" treatment while it lasted, " + Main.LocalPlayer.name + ". You're about to find out firsthand why healthcare isn't free, cheapskate!";
				}
			}
			else
			{
				Main.npcChatText = "Your healing's not even done yet. Sit still in there until me and my gut are done fixing you up.";
			}
		}
	}
}

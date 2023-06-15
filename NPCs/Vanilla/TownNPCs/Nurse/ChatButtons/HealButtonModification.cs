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

namespace V2.NPCs.Vanilla.TownNPCs.Nurse.ChatButtons
{
	public class HealButtonModification : GlobalChatButton
	{
		public override bool? IsActive(ChatButton chatButton, NPC npc, Player player)
		{
			if (chatButton != ChatButton.NurseHeal)
				return null;

			if (npc.type != NPCID.Nurse)
				return false;

			if (npc.AsNurse().healTypeChoice)
				return false;

			return null;
		}
		public override void ModifyText(ChatButton chatButton, NPC npc, Player player, ref string buttonText)
		{
			if (chatButton != ChatButton.NurseHeal)
				return;

			if (player.IsFoodFor(npc, out bool pastTense) && !pastTense)
			{
				buttonText = Lang.inter[54].Value;
			}
		}

		public override void ModifyColor(ChatButton chatButton, NPC npc, Player player, ref Color buttonTextColor)
		{
			if (chatButton != ChatButton.NurseHeal)
				return;

			if (player.IsFoodFor(npc, out bool pastTense) && !pastTense)
			{
				buttonTextColor = Color.Gray;
			}
		}

		public override bool PreClick(ChatButton chatButton, NPC npc, Player player)
		{
			if (chatButton != ChatButton.NurseHeal)
				return true;

			if (player.IsFoodFor(npc, out bool pastTense) && !pastTense)
			{
				Main.npcChatText = npc.AsNurse().digestScamPatient
					? "Oh, what's that? You wanted to be healed, not hurt? Well, you shouldn't have tried to undercut me, then. Enjoy being a nutrient soup...and, hopefully, my soon-to-be ass fat."
					: "Aren't you in there to be a meal for me? I don't think I need to worry about healing you at the moment.";
			}
			else
			{

				int num4 = Main.player[Main.myPlayer].statLifeMax2 - Main.player[Main.myPlayer].statLife;
				for (int j = 0; j < Player.MaxBuffs; j++)
				{
					int num5 = Main.player[Main.myPlayer].buffType[j];
					if (Main.debuff[num5] && Main.player[Main.myPlayer].buffTime[j] > 60 && (num5 < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[num5]))
						num4 += 100;
				}

				int health = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife;
				bool removeDebuffs = true;
				if (NPC.downedGolemBoss)
					num4 *= 200;
				else if (NPC.downedPlantBoss)
					num4 *= 150;
				else if (NPC.downedMechBossAny)
					num4 *= 100;
				else if (Main.hardMode)
					num4 *= 60;
				else if (NPC.downedBoss3 || NPC.downedQueenBee)
					num4 *= 25;
				else if (NPC.downedBoss2)
					num4 *= 10;
				else if (NPC.downedBoss1)
					num4 *= 3;

				if (Main.expertMode)
					num4 *= 2;

				int copperCoins = (int)((double)num4 * Main.player[Main.myPlayer].currentShoppingSettings.PriceAdjustment);
				if (copperCoins > 0 && copperCoins < 1)
					copperCoins = 1;
				int originalHealPrice = copperCoins;
				string reason = Language.GetTextValue("tModLoader.DefaultNurseCantHealChat");
				bool canHeal = PlayerLoader.ModifyNurseHeal(Main.player[Main.myPlayer], npc, ref health, ref removeDebuffs, ref reason);

				if (originalHealPrice < 0)
					originalHealPrice = 0;

				if (canHeal)
				{
					npc.AsNurse().originalHealPrice = originalHealPrice;
					npc.AsNurse().healOvertime = 0;
					npc.AsNurse().healTypeChoice = true;
					Main.npcChatText = "Alright, so you're looking to get patched up. Great, just great...you want the quick way, or the easy way?";
				}
				else
				{
					Main.npcChatText = reason;
				}
			}
			return false;
		}
	}
}

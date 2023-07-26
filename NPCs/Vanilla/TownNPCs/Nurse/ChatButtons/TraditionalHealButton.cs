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

namespace V2.NPCs.Vanilla.TownNPCs.Nurse.ChatButtons
{
	public class TraditionalHealButton : ChatButton
	{
		public override string Text(NPC npc, Player player) => "Traditional";

		public override double Priority => NurseHeal.Priority;

		public override bool IsActive(NPC npc, Player player) => npc.type == NPCID.Nurse && npc.AsNurse().healTypeChoice;

		public override void OnClick(NPC npc, Player player)
		{
			int health = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife;
			bool removeDebuffs = true;
			PlayerLoader.ModifyNursePrice(Main.LocalPlayer, npc, health, removeDebuffs, ref npc.AsNurse().originalHealPrice);

			if (npc.AsNurse().originalHealPrice > 0)
			{
				if (Main.LocalPlayer.BuyItem(npc.AsNurse().originalHealPrice))
				{
					AchievementsHelper.HandleNurseService(npc.AsNurse().originalHealPrice);
					SoundEngine.PlaySound(SoundID.Item4);
					SoundEngine.PlaySound(SoundID.Coins);
					Main.LocalPlayer.HealEffect(health, true);
					if ((double)Main.LocalPlayer.statLife < (double)Main.LocalPlayer.statLifeMax2 * 0.25)
						Main.npcChatText = Lang.dialog(227);
					else if ((double)Main.LocalPlayer.statLife < (double)Main.LocalPlayer.statLifeMax2 * 0.5)
						Main.npcChatText = Lang.dialog(228);
					else if ((double)Main.LocalPlayer.statLife < (double)Main.LocalPlayer.statLifeMax2 * 0.75)
						Main.npcChatText = Lang.dialog(229);
					else
						Main.npcChatText = Lang.dialog(230);

					Main.LocalPlayer.statLife += health;

					if (!removeDebuffs)
						goto SkipDebuffRemoval;

					for (int l = 0; l < Player.MaxBuffs; l++)
					{
						int num24 = Main.LocalPlayer.buffType[l];
						if (Main.debuff[num24] && Main.LocalPlayer.buffTime[l] > 0 && (num24 < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[num24]))
						{
							Main.player[Main.myPlayer].DelBuff(l);
							l = -1;
						}
					}

					SkipDebuffRemoval:
					PlayerLoader.PostNurseHeal(Main.LocalPlayer, npc, health, removeDebuffs, npc.AsNurse().originalHealPrice);
				}
				else
				{
					Main.npcChatText = Main.rand.NextFromCollection(new List<string> {
						"You can't afford me. How unfortunate. Guess you'll have to stop wasting my time.",
						"I'll never be able to go for lunch if you keep calling me for check-ups you can't afford. And trust me, that won't stop me from eating.",
						"I don't work for free, and neither does my gut. Either cough up the cash for a traditional fix-up or get out.",
					});
				}
			}
			else
			{
				Main.npcChatText = Main.rand.NextFromCollection(new List<string> {
					"I don't give happy endings, unless you consider the chance to fatten up my glutes to be one.",
					"I'll never be able to go for lunch if you keep calling me for nothing. And trust me, that won't stop me from eating.",
					"You keep wasting my time, I'll see if I can somehow churn you into more space in the hospital I run around back.",
				});
			}
			npc.AsNurse().healTypeChoice = false;
			npc.AsNurse().originalHealPrice = 0;
		}
	}
}

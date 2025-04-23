using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.PlayerHandling;

namespace V2.NPCs
{
	public static class NPCHealthHandlingExtensions
	{
		public static void AddHealthRegenEffect(
			this NPC npc,
			DelegateHealthRegenPerSecond healthPerSecond,
			bool natural = false,
			DelegateHealthRegenModifyHealthRegenTime modifyHealthRegenTimeMethod = null,
			DelegateHealthRegenModifyTotalHealthRegen modifyTotalHealthRegenMethod = null,
			DelegateHealthRegenOnHealthAdjustment onHealthAdjustmentMethod = null
		) => npc.AsV2NPC().HealthRegenEffects.Add(new HealthRegenEffect(
			healthPerSecond,
			natural,
			modifyHealthRegenTimeMethod,
			modifyTotalHealthRegenMethod,
			onHealthAdjustmentMethod
		));

		public static void AddHealthRegenEffect(
			this NPC npc,
			double healthPerSecond,
			bool natural = false,
			DelegateHealthRegenModifyHealthRegenTime modifyHealthRegenTimeMethod = null,
			DelegateHealthRegenModifyTotalHealthRegen modifyTotalHealthRegenMethod = null,
			DelegateHealthRegenOnHealthAdjustment onHealthAdjustmentMethod = null
		) => npc.AsV2NPC().HealthRegenEffects.Add(new HealthRegenEffect(
			healthPerSecond,
			natural,
			modifyHealthRegenTimeMethod,
			modifyTotalHealthRegenMethod,
			onHealthAdjustmentMethod
		));
	}

	public partial class GeneralNPC : GlobalNPC
	{
		public List<HealthRegenEffect> HealthRegenEffects { get; set; }
		public (
			double baseRegen,
			double additiveRegenModifier,
			double multiplicativeRegenModifier,
			double flatRegenBonus
		) HealthRegenNatural;
		public (
			double baseRegen,
			double additiveRegenModifier,
			double multiplicativeRegenModifier,
			double flatRegenBonus
		) HealthRegenArtificial;
		public double healthRegenTime;
		public double healthRegenCount;

		public void ResetHealthRegenTime()
		{
			healthRegenTime = 0.0;
			healthRegenCount = 0.0;
		}

		public void ResetHealthRegenEffectList()
		{
			HealthRegenEffects = [];
		}

		public static void Detour_UpdateNPC_BuffApplyDOTs(NPC npc)
		{
			if (npc.dontTakeDamage)
				return;

			int num = npc.lifeRegenExpectedLossPerSecond;
			if (npc.poisoned)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 12;
			}

			if (npc.onFire)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 8;
			}

			if (npc.onFire3)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 30;
				if (num < 5)
					num = 5;
			}

			if (npc.onFrostBurn)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 16;
				if (num < 2)
					num = 2;
			}

			if (npc.onFrostBurn2)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 50;
				if (num < 10)
					num = 10;
			}

			if (npc.onFire2)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 48;
				if (num < 10)
					num = 10;
			}

			if (npc.venom)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 60;
				if (num < 15)
					num = 15;
			}

			if (npc.shadowFlame)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 30;
				if (num < 5)
					num = 5;
			}

			if (npc.oiled && (npc.onFire || npc.onFire2 || npc.onFire3 || npc.onFrostBurn || npc.onFrostBurn2 || npc.shadowFlame))
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 50;
				if (num < 10)
					num = 10;
			}

			if (npc.javelined)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				int num2 = 0;
				int num3 = 1;
				for (int i = 0; i < 1000; i++)
				{
					if (Main.projectile[i].active && Main.projectile[i].type == 598 && Main.projectile[i].ai[0] == 1f && Main.projectile[i].ai[1] == (float)npc.whoAmI)
						num2++;
				}

				npc.lifeRegen -= num2 * 2 * 3;
				if (num < num2 * 3 / num3)
					num = num2 * 3 / num3;
			}

			if (npc.tentacleSpiked)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				int num4 = 0;
				int num5 = 1;
				for (int j = 0; j < 1000; j++)
				{
					if (Main.projectile[j].active && Main.projectile[j].type == 971 && Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == (float)npc.whoAmI)
						num4++;
				}

				npc.lifeRegen -= num4 * 2 * 3;
				if (num < num4 * 3 / num5)
					num = num4 * 3 / num5;
			}

			if (npc.bloodButchered)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				int num6 = 0;
				int num7 = 1;
				for (int k = 0; k < 1000; k++)
				{
					if (Main.projectile[k].active && Main.projectile[k].type == 975 && Main.projectile[k].ai[0] == 1f && Main.projectile[k].ai[1] == (float)npc.whoAmI)
						num6++;
				}

				npc.lifeRegen -= num6 * 2 * 4;
				if (num < num6 * 4 / num7)
					num = num6 * 4 / num7;
			}

			if (npc.daybreak)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				int num8 = 0;
				int num9 = 4;
				for (int l = 0; l < 1000; l++)
				{
					if (Main.projectile[l].active && Main.projectile[l].type == 636 && Main.projectile[l].ai[0] == 1f && Main.projectile[l].ai[1] == (float)npc.whoAmI)
						num8++;
				}

				if (num8 == 0)
					num8 = 1;

				npc.lifeRegen -= num8 * 2 * 100;
				if (num < num8 * 100 / num9)
					num = num8 * 100 / num9;
			}

			if (npc.celled)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				int num10 = 0;
				for (int m = 0; m < 1000; m++)
				{
					if (Main.projectile[m].active && Main.projectile[m].type == 614 && Main.projectile[m].ai[0] == 1f && Main.projectile[m].ai[1] == (float)npc.whoAmI)
						num10++;
				}

				npc.lifeRegen -= num10 * 2 * 20;
				if (num < num10 * 20)
					num = num10 * 20 / 2;
			}

			if (npc.dryadBane)
			{
				int num11 = 4;
				float num12 = 1f;
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				if (NPC.downedBoss1)
					num12 += 0.1f;

				if (NPC.downedBoss2)
					num12 += 0.1f;

				if (NPC.downedBoss3)
					num12 += 0.1f;

				if (NPC.downedQueenBee)
					num12 += 0.1f;

				if (Main.hardMode)
					num12 += 0.4f;

				if (NPC.downedMechBoss1)
					num12 += 0.15f;

				if (NPC.downedMechBoss2)
					num12 += 0.15f;

				if (NPC.downedMechBoss3)
					num12 += 0.15f;

				if (NPC.downedPlantBoss)
					num12 += 0.15f;

				if (NPC.downedGolemBoss)
					num12 += 0.15f;

				if (NPC.downedAncientCultist)
					num12 += 0.15f;

				if (Main.expertMode)
					num12 *= Main.GameModeInfo.TownNPCDamageMultiplier;

				num11 = (int)((float)num11 * num12);
				npc.lifeRegen -= 2 * num11;
				if (num < num11)
					num = num11 / 3;
			}

			if (npc.soulDrain && npc.realLife == -1)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 50;
				if (num < 5)
					num = 5;
			}

			NPCLoader.UpdateLifeRegen(npc, ref num);

			if (npc.lifeRegen <= -240 && num < 2)
				num = 2;

			// Extra patch context.
			npc.lifeRegenCount += npc.lifeRegen;
			while (npc.lifeRegenCount >= 120)
			{
				npc.lifeRegenCount -= 120;
				if (!npc.immortal)
				{
					if (npc.life < npc.lifeMax)
						npc.life++;

					if (npc.life > npc.lifeMax)
						npc.life = npc.lifeMax;
				}
			}

			if (num > 0)
			{
				while (npc.lifeRegenCount <= -120 * num)
				{
					npc.lifeRegenCount += 120 * num;
					int whoAmI = npc.whoAmI;
					if (npc.realLife >= 0)
						whoAmI = npc.realLife;

					if (!Main.npc[whoAmI].immortal)
						Main.npc[whoAmI].life -= num;

					CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), CombatText.LifeRegenNegative, num, dramatic: false, dot: true);
					if (Main.npc[whoAmI].life > 0 || Main.npc[whoAmI].immortal)
						continue;

					Main.npc[whoAmI].life = 1;
					if (Main.netMode != 1)
					{
						Main.npc[whoAmI].SimpleStrikeNPC(9999, 0, noPlayerInteraction: true);
						if (Main.netMode == 2)
							NetMessage.SendData(28, -1, -1, null, whoAmI, 9999f);
					}
				}

				return;
			}

			while (npc.lifeRegenCount <= -120)
			{
				npc.lifeRegenCount += 120;
				int whoAmI = npc.whoAmI;
				if (npc.realLife >= 0)
					whoAmI = npc.realLife;

				if (!Main.npc[whoAmI].immortal)
					Main.npc[whoAmI].life--;

				CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), CombatText.LifeRegenNegative, 1, dramatic: false, dot: true);
				if (Main.npc[whoAmI].life > 0 || Main.npc[whoAmI].immortal)
					continue;

				Main.npc[whoAmI].life = 1;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Main.npc[whoAmI].SimpleStrikeNPC(9999, 0, noPlayerInteraction: true);
					if (Main.netMode == 2)
						NetMessage.SendData(28, -1, -1, null, whoAmI, 9999f);
				}
			}
		}
	}
}

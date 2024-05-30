using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.UI;

namespace V2.PlayerHandling
{
	public static class PlayerHealthHandlingExtensions
	{
		public static void AddHealthRegenEffect(
			this Player player,
			DelegateHealthRegenPerSecond healthPerSecond,
			bool natural = false,
			DelegateHealthRegenModifyHealthRegenTime modifyHealthRegenTimeMethod = null,
			DelegateHealthRegenModifyTotalHealthRegen modifyTotalHealthRegenMethod = null,
			DelegateHealthRegenOnHealthAdjustment onHealthAdjustmentMethod = null
		) => player.AsV2Player().HealthRegenEffects.Add(new HealthRegenEffect(
			healthPerSecond,
			natural,
			modifyHealthRegenTimeMethod,
			modifyTotalHealthRegenMethod,
			onHealthAdjustmentMethod
		));

		public static void AddHealthRegenEffect(
			this Player player,
			double healthPerSecond,
			bool natural = false,
			DelegateHealthRegenModifyHealthRegenTime modifyHealthRegenTimeMethod = null,
			DelegateHealthRegenModifyTotalHealthRegen modifyTotalHealthRegenMethod = null,
			DelegateHealthRegenOnHealthAdjustment onHealthAdjustmentMethod = null
		) => player.AsV2Player().HealthRegenEffects.Add(new HealthRegenEffect(
			healthPerSecond,
			natural,
			modifyHealthRegenTimeMethod,
			modifyTotalHealthRegenMethod,
			onHealthAdjustmentMethod
		));
	}

	public partial class V2Player : ModPlayer
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
			HealthRegenEffects = new List<HealthRegenEffect>();
			HealthRegenEffects.Add(new HealthRegenEffect(
				healthPerSecond: NaturalHealthRegen,
				natural: true
			));
		}

		public static double NaturalHealthRegen(Player player)
		{
			double oneMinuteFrameCount = (double)V2Utils.SensibleTime(
				minutes: 1
			);
			double healEffectivenessPercentage = Math.Max(player.AsV2Player().healthRegenTime, 0.0) / oneMinuteFrameCount;
			double healthToRegenAtMaxNaturalEffectiveness = (double)player.statLifeMax2 / 100.0;
			return healEffectivenessPercentage * healthToRegenAtMaxNaturalEffectiveness;
		}

		public void HandleSittingAndSleepingHealthRegenEffect()
		{
			if (Player.sitting.isSitting || Player.sleeping.isSleeping)
			{
				Player.AddHealthRegenEffect(
					healthPerSecond: 0.8,
					natural: true,
					modifyHealthRegenTimeMethod: RelaxationModifyHealthRegenTime,
					modifyTotalHealthRegenMethod: RelaxationModifyTotalHealthRegen
				);
			}
		}

		public static void RelaxationModifyHealthRegenTime(Player player, ref double healthRegenTime)
		{
			healthRegenTime += 0.4;
		}

		public static void RelaxationModifyTotalHealthRegen(Player player, ref double naturalRegenAdditive, ref double naturalRegenMultiplicative, ref double artificialRegenAdditive, ref double artificialRegenMultiplicative)
		{
			naturalRegenAdditive += 0.25;
		}

		public static void Detour_UpdateLifeRegen(Player player)
		{
			bool shinyStoneShouldEverFuckingWork = false;
			if (player.shinyStone && player.velocity.Length() < 0.05f && player.itemAnimation == 0)
				shinyStoneShouldEverFuckingWork = true;

			player.AsV2Player().healthRegenTime += 1.0;
			foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
			{
				healthRegenEffect.modifyHealthRegenTimeMethod?.Invoke(
					player,
					ref player.AsV2Player().healthRegenTime
				);
			}
			double oneMinuteFrameCount = (double)V2Utils.SensibleTime(
				minutes: 1
			);
			if (player.AsV2Player().healthRegenTime >= oneMinuteFrameCount)
				player.AsV2Player().healthRegenTime = oneMinuteFrameCount;

			player.AsV2Player().HealthRegenNatural.baseRegen = 0.0;
			player.AsV2Player().HealthRegenNatural.additiveRegenModifier = 1.0;
			player.AsV2Player().HealthRegenNatural.flatRegenBonus = 0.0;
			player.AsV2Player().HealthRegenNatural.multiplicativeRegenModifier = 1.0;
			player.AsV2Player().HealthRegenArtificial.baseRegen = 0.0;
			player.AsV2Player().HealthRegenArtificial.additiveRegenModifier = 1.0;
			player.AsV2Player().HealthRegenArtificial.flatRegenBonus = 0.0;
			player.AsV2Player().HealthRegenArtificial.multiplicativeRegenModifier = 1.0;
			foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
			{
				if (healthRegenEffect.natural)
					player.AsV2Player().HealthRegenNatural.baseRegen += (float)healthRegenEffect.healthPerSecond.Invoke(player);
				else
					player.AsV2Player().HealthRegenArtificial.baseRegen += (float)healthRegenEffect.healthPerSecond.Invoke(player);
			}

			foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
			{
				healthRegenEffect.modifyTotalHealthRegenMethod?.Invoke(
					player,
					ref player.AsV2Player().HealthRegenNatural.additiveRegenModifier,
					ref player.AsV2Player().HealthRegenNatural.multiplicativeRegenModifier,
					ref player.AsV2Player().HealthRegenArtificial.additiveRegenModifier,
					ref player.AsV2Player().HealthRegenArtificial.multiplicativeRegenModifier
				);
			}

			double naturalHealthRegenCount =
				(player.AsV2Player().HealthRegenNatural.baseRegen * player.AsV2Player().HealthRegenNatural.additiveRegenModifier)
			   + player.AsV2Player().HealthRegenNatural.flatRegenBonus;
			double artificialHealthRegenCount =
				(player.AsV2Player().HealthRegenArtificial.baseRegen * player.AsV2Player().HealthRegenArtificial.additiveRegenModifier)
			   + player.AsV2Player().HealthRegenArtificial.flatRegenBonus;
			player.AsV2Player().healthRegenCount += naturalHealthRegenCount + artificialHealthRegenCount;
			while (player.AsV2Player().healthRegenCount >= 60.0)
			{
				player.AsV2Player().healthRegenCount -= 60.0;
				if (player.statLife < player.statLifeMax2)
				{
					player.statLife++;
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, 1);
					}
				}

				if (player.statLife > player.statLifeMax2)
					player.statLife = player.statLifeMax2;
			}

			while (player.AsV2Player().healthRegenCount <= -60.0)
			{
				if (player.AsV2Player().healthRegenCount <= -240.0)
				{
					player.AsV2Player().healthRegenCount += 240.0;
					player.statLife -= 4;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 4, dramatic: false, dot: true);
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, -4);
					}
				}
				else if (player.AsV2Player().healthRegenCount <= -180.0)
				{
					player.AsV2Player().healthRegenCount += 180.0;
					player.statLife -= 3;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 3, dramatic: false, dot: true);
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, -3);
					}
				}
				else if (player.AsV2Player().healthRegenCount <= -120.0)
				{
					player.AsV2Player().healthRegenCount += 120.0;
					player.statLife -= 2;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 2, dramatic: false, dot: true);
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, -2);
					}
				}
				else
				{
					player.AsV2Player().healthRegenCount += 60.0;
					player.statLife--;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 1, dramatic: false, dot: true);
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, -1);
					}
				}

				if (player.statLife <= 0 && player.whoAmI == Main.myPlayer)
				{
					if (player.poisoned || player.venom)
						player.KillMe(PlayerDeathReason.ByOther(9), 10.0, 0);
					else if (player.electrified)
						player.KillMe(PlayerDeathReason.ByOther(10), 10.0, 0);
					else
						player.KillMe(PlayerDeathReason.ByOther(8), 10.0, 0);

					return;
				}
			}

			// compatibility with vanilla-style health regen effects
			PlayerLoader.UpdateBadLifeRegen(player);

			player.lifeRegenTime++;
			if (player.lifeRegenTime >= 3600)
				player.lifeRegenTime = 3600;

			PlayerLoader.UpdateLifeRegen(player);
			float num5 = 0f;
			PlayerLoader.NaturalLifeRegen(player, ref num5);
			float num7 = (float)player.statLifeMax2 / 400f * 0.85f + 0.15f;
			num5 *= num7;
			player.lifeRegen += (int)Math.Round(num5);
			player.lifeRegenCount += player.lifeRegen;

			if (shinyStoneShouldEverFuckingWork && player.lifeRegen > 0 && player.statLife < player.statLifeMax2)
			{
				player.lifeRegenCount++;
				if (shinyStoneShouldEverFuckingWork && (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(30)))
				{
					int num8 = Dust.NewDust(player.position, player.width, player.height, DustID.Pixie, 0f, 0f, 200, default(Color), 0.5f);
					Main.dust[num8].noGravity = true;
					Main.dust[num8].velocity *= 0.75f;
					Main.dust[num8].fadeIn = 1.3f;
					Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
					vector.Normalize();
					vector *= (float)Main.rand.Next(50, 100) * 0.04f;
					Main.dust[num8].velocity = vector;
					vector.Normalize();
					vector *= 34f;
					Main.dust[num8].position = player.Center - vector;
				}
			}

			while (player.lifeRegenCount >= 120)
			{
				player.lifeRegenCount -= 120;
				if (player.statLife < player.statLifeMax2)
				{
					player.statLife++;
					if (player.crimsonRegen)
					{
						for (int i = 0; i < 10; i++)
						{
							int num9 = Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 0f, 0f, 175, default(Color), 1.75f);
							Main.dust[num9].noGravity = true;
							Main.dust[num9].velocity *= 0.75f;
							int num10 = Main.rand.Next(-40, 41);
							int num11 = Main.rand.Next(-40, 41);
							Main.dust[num9].position.X += num10;
							Main.dust[num9].position.Y += num11;
							Main.dust[num9].velocity.X = (float)(-num10) * 0.075f;
							Main.dust[num9].velocity.Y = (float)(-num11) * 0.075f;
						}
					}
				}

				if (player.statLife > player.statLifeMax2)
					player.statLife = player.statLifeMax2;
			}

			if (player.burned || player.suffocating || (player.tongued && Main.expertMode))
			{
				while (player.lifeRegenCount <= -600)
				{
					player.lifeRegenCount += 600;
					player.statLife -= 5;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 5, dramatic: false, dot: true);
					if (player.statLife <= 0 && player.whoAmI == Main.myPlayer)
					{
						if (player.suffocating)
							player.KillMe(PlayerDeathReason.ByOther(7), 10.0, 0);
						else
							player.KillMe(PlayerDeathReason.ByOther(8), 10.0, 0);
					}
				}

				return;
			}

			if (player.starving)
			{
				int num12 = player.statLifeMax2 / 50;
				if (num12 < 2)
					num12 = 2;

				int num13 = (player.ZoneDesert || player.ZoneSnow) ? (num12 * 2) : num12;
				int num14 = 120 * num12;
				while (player.lifeRegenCount <= -num14)
				{
					player.lifeRegenCount += num14;
					player.statLife -= num13;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, num13, dramatic: false, dot: true);
					if (player.statLife <= 0 && player.whoAmI == Main.myPlayer)
						player.KillMe(PlayerDeathReason.ByOther(18), 10.0, 0);
				}

				return;
			}

			while (player.lifeRegenCount <= -120)
			{
				if (player.lifeRegenCount <= -480)
				{
					player.lifeRegenCount += 480;
					player.statLife -= 4;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 4, dramatic: false, dot: true);
				}
				else if (player.lifeRegenCount <= -360)
				{
					player.lifeRegenCount += 360;
					player.statLife -= 3;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 3, dramatic: false, dot: true);
				}
				else if (player.lifeRegenCount <= -240)
				{
					player.lifeRegenCount += 240;
					player.statLife -= 2;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 2, dramatic: false, dot: true);
				}
				else
				{
					player.lifeRegenCount += 120;
					player.statLife--;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 1, dramatic: false, dot: true);
				}

				if (player.statLife <= 0 && player.whoAmI == Main.myPlayer)
				{
					if (player.poisoned || player.venom)
						player.KillMe(PlayerDeathReason.ByOther(9), 10.0, 0);
					else if (player.electrified)
						player.KillMe(PlayerDeathReason.ByOther(10), 10.0, 0);
					else
						player.KillMe(PlayerDeathReason.ByOther(8), 10.0, 0);
				}
			}
		}
	}
}
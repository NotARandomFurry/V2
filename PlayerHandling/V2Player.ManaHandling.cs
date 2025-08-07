using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.UI;

namespace V2.PlayerHandling
{
	public static class PlayerManaHandlingExtensions
	{
		public static void AddManaRegenEffect(
			this Player player,
			DelegateManaRegenPerSecond manaPerSecond,
			bool natural = false,
			DelegateManaRegenModifyManaRegenDelay modifyManaRegenDelayMethod = null,
			DelegateManaRegenModifyTotalManaRegen modifyTotalManaRegenMethod = null,
			DelegateManaRegenOnManaAdjustment onManaAdjustmentMethod = null
		) => player.AsV2Player().ManaRegenEffects.Add(new ManaRegenEffect(
			manaPerSecond,
			natural,
			modifyManaRegenDelayMethod,
			modifyTotalManaRegenMethod,
			onManaAdjustmentMethod
		));

		public static void AddManaRegenEffect(
			this Player player,
			double manaPerSecond,
			bool natural = false,
			DelegateManaRegenModifyManaRegenDelay modifyManaRegenDelayMethod = null,
			DelegateManaRegenModifyTotalManaRegen modifyTotalManaRegenMethod = null,
			DelegateManaRegenOnManaAdjustment onManaAdjustmentMethod = null
		) => player.AsV2Player().ManaRegenEffects.Add(new ManaRegenEffect(
			manaPerSecond,
			natural,
			modifyManaRegenDelayMethod,
			modifyTotalManaRegenMethod,
			onManaAdjustmentMethod
		));
	}

	public partial class V2Player : ModPlayer
	{
		public List<ManaRegenEffect> ManaRegenEffects { get; set; }
		public (
			double baseRegen,
			double additiveRegenModifier,
			double multiplicativeRegenModifier,
			double flatRegenBonus
		) ManaRegenNatural;
		public (
			double baseRegen,
			double additiveRegenModifier,
			double multiplicativeRegenModifier,
			double flatRegenBonus
		) ManaRegenArtificial;
		public double manaRegenDelay;
		public double manaRegenCount;

		public void ResetManaRegenTime()
		{
			manaRegenDelay = 0.0;
			manaRegenCount = 0.0;
		}

		public void ResetManaRegenEffectList()
		{
			ManaRegenEffects =
			[
				new ManaRegenEffect(
					manaPerSecond: NaturalManaRegen,
					natural: true
				),
			];
		}

		public static double NaturalManaRegen(Player player)
		{
			if (player.AsV2Player().manaRegenDelay > 0)
				return 0.0;
			else if (player.velocity.Length() > 0)
				return (double)player.statManaMax * 0.08;
			else
				return (double)player.statManaMax * 0.4;
		}

		public static void Detour_UpdateManaRegen(Player player)
		{
			player.AsV2Player().manaRegenDelay -= 1.0;
			foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
			{
				manaRegenEffect.modifyManaRegenDelayMethod?.Invoke(
					player,
					ref player.AsV2Player().manaRegenDelay
				);
			}
			double oneMinuteFrameCount = (double)V2Utils.SensibleTime(
				minutes: 1
			);
			if (player.AsV2Player().manaRegenDelay >= oneMinuteFrameCount)
				player.AsV2Player().manaRegenDelay = oneMinuteFrameCount;

			player.AsV2Player().ManaRegenNatural.baseRegen = 0.0;
			player.AsV2Player().ManaRegenNatural.additiveRegenModifier = 1.0;
			player.AsV2Player().ManaRegenNatural.flatRegenBonus = 0.0;
			player.AsV2Player().ManaRegenNatural.multiplicativeRegenModifier = 1.0;
			player.AsV2Player().ManaRegenArtificial.baseRegen = 0.0;
			player.AsV2Player().ManaRegenArtificial.additiveRegenModifier = 1.0;
			player.AsV2Player().ManaRegenArtificial.flatRegenBonus = 0.0;
			player.AsV2Player().ManaRegenArtificial.multiplicativeRegenModifier = 1.0;
			foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
			{
				if (manaRegenEffect.natural)
					player.AsV2Player().ManaRegenNatural.baseRegen += (float)manaRegenEffect.manaPerSecond.Invoke(player);
				else
					player.AsV2Player().ManaRegenArtificial.baseRegen += (float)manaRegenEffect.manaPerSecond.Invoke(player);
			}

			foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
			{
				manaRegenEffect.modifyTotalManaRegenMethod?.Invoke(
					player,
					ref player.AsV2Player().ManaRegenNatural.additiveRegenModifier,
					ref player.AsV2Player().ManaRegenNatural.multiplicativeRegenModifier,
					ref player.AsV2Player().ManaRegenArtificial.additiveRegenModifier,
					ref player.AsV2Player().ManaRegenArtificial.multiplicativeRegenModifier
				);
			}

			double naturalManaRegenCount =
				(player.AsV2Player().ManaRegenNatural.baseRegen * player.AsV2Player().ManaRegenNatural.additiveRegenModifier)
			   + player.AsV2Player().ManaRegenNatural.flatRegenBonus;
			double artificialManaRegenCount =
				(player.AsV2Player().ManaRegenArtificial.baseRegen * player.AsV2Player().ManaRegenArtificial.additiveRegenModifier)
			   + player.AsV2Player().ManaRegenArtificial.flatRegenBonus;
			player.AsV2Player().manaRegenCount += naturalManaRegenCount + artificialManaRegenCount;
			while (player.AsV2Player().manaRegenCount >= 60.0)
			{
				player.AsV2Player().manaRegenCount -= 60.0;
				if (player.statMana < player.statManaMax2)
				{
					player.statMana++;
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, 1);
					}
				}

				if (player.statMana > player.statManaMax2)
					player.statMana = player.statManaMax2;
			}

			while (player.AsV2Player().manaRegenCount <= -60.0)
			{
				if (player.AsV2Player().manaRegenCount <= -240.0)
				{
					player.AsV2Player().manaRegenCount += 240.0;
					player.statMana -= 4;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, 4, dramatic: false, dot: true);
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, -4);
					}
				}
				else if (player.AsV2Player().manaRegenCount <= -180.0)
				{
					player.AsV2Player().manaRegenCount += 180.0;
					player.statMana -= 3;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, 3, dramatic: false, dot: true);
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, -3);
					}
				}
				else if (player.AsV2Player().manaRegenCount <= -120.0)
				{
					player.AsV2Player().manaRegenCount += 120.0;
					player.statMana -= 2;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, 2, dramatic: false, dot: true);
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, -2);
					}
				}
				else
				{
					player.AsV2Player().manaRegenCount += 60.0;
					player.statMana--;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, 1, dramatic: false, dot: true);
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, -1);
					}
				}
			}

			// the followin' is for compatibility with vanilla-style mana regen effects
			if (player.nebulaLevelMana > 0)
			{
				int num = 6;
				player.nebulaManaCounter += player.nebulaLevelMana;
				if (player.nebulaManaCounter >= num)
				{
					player.nebulaManaCounter -= num;
					player.statMana++;
					if (player.statMana >= player.statManaMax2)
						player.statMana = player.statManaMax2;
				}
			}
			else
			{
				player.nebulaManaCounter = 0;
			}

			if (player.manaRegenDelay > 0f)
			{
				player.manaRegenDelay -= 1f;
				player.manaRegenDelay -= player.manaRegenDelayBonus;
				if (player.IsStandingStillForSpecialEffects || player.grappling[0] >= 0 || player.manaRegenBuff)
					player.manaRegenDelay -= 1f;

				if (player.usedArcaneCrystal)
					player.manaRegenDelay -= 0.05f;
			}

			if (player.manaRegenBuff && player.manaRegenDelay > 20f)
				player.manaRegenDelay = 20f;

			if (player.manaRegenDelay <= 0f)
			{
				player.manaRegenDelay = 0f;
				player.manaRegen = player.statManaMax2 / 3 + 1 + player.manaRegenBonus;
				if (player.IsStandingStillForSpecialEffects || player.grappling[0] >= 0 || player.manaRegenBuff)
					player.manaRegen += player.statManaMax2 / 3;

				if (player.usedArcaneCrystal)
					player.manaRegen += player.statManaMax2 / 50;

				float num2 = (float)player.statMana / (float)player.statManaMax2 * 0.8f + 0.2f;
				if (player.manaRegenBuff)
					num2 = 1f;

				player.manaRegen = (int)((double)((float)player.manaRegen * num2) * 1.15);
			}
			else
			{
				player.manaRegen = 0;
			}

			player.manaRegenCount += player.manaRegen;
			while (player.manaRegenCount >= 120)
			{
				bool flag = false;
				player.manaRegenCount -= 120;
				if (player.statMana < player.statManaMax2)
				{
					player.statMana++;
					flag = true;
				}

				if (player.statMana < player.statManaMax2)
					continue;

				if (player.whoAmI == Main.myPlayer && flag)
				{
					SoundEngine.PlaySound(SoundID.MaxMana);
					for (int i = 0; i < 5; i++)
					{
						int num3 = Dust.NewDust(player.position, player.width, player.height, DustID.ManaRegeneration, 0f, 0f, 255, default(Color), (float)Main.rand.Next(20, 26) * 0.1f);
						Main.dust[num3].noLight = true;
						Main.dust[num3].noGravity = true;
						Main.dust[num3].velocity *= 0.5f;
					}
				}

				player.statMana = player.statManaMax2;
			}
		}
	}
}
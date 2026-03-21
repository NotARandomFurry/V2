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
	}
}
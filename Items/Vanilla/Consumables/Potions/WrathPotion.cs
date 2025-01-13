using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;
using V2.StatusEffects.Vanilla.Buffs;

namespace V2.Items.Vanilla.Consumables.Potions
{
	public class WrathPotion : PotionTemplate
	{
		public override string TooltipTranslationKey => "Vanilla.Consumables.Potions.Wrath";
		public override int DigestedPotionEffectID => BuffID.Wrath;
		public override int DigestedPotionEffectDuration => V2Utils.SensibleTime(minutes: 3);
		public override int AppliesToPotionItem => ItemID.WrathPotion;

		public override dynamic TooltipVariables()
		{
			return new
			{
				WrathPotionDamageBoost = WrathBuff.DamageBonus.ToPercentage(2),
				WrathPotionTUMBoost = WrathBuff.TUMBonus,
				WrathPotionACIBoost = WrathBuff.ACIBonus,
			};
		}
	}
}

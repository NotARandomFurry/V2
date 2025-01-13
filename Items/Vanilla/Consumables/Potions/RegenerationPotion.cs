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
	public class RegenerationPotion : PotionTemplate
	{
		public override string TooltipTranslationKey => "Vanilla.Consumables.Potions.Regeneration";
		public override int DigestedPotionEffectID => BuffID.Regeneration;
		public override int DigestedPotionEffectDuration => V2Utils.SensibleTime(minutes: 6);
		public override int AppliesToPotionItem => ItemID.RegenerationPotion;

		public override dynamic TooltipVariables()
		{
			return new
			{
				RegenPotionRegenFlat = RegenerationBuff.HealthRegenFlat,
			};
		}
	}
}

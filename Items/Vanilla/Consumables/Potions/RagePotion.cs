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
	public class RagePotion : GlobalItem
	{
		public static int DigestedRageTime => V2Utils.SensibleTime(minutes: 3);
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.RagePotion;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 400;
			item.AsFood().Size = 0.15;

			item.AsFood().UpdateInStomach += UpdateInStomach;
			item.AsFood().OnBreak += OnBreak;

			item.AsFood().EdibleOnUse = true;
			item.AsFood().AlwaysEatenByUse = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead)
				pred.AddStatus(BuffID.Rage, DigestedRageTime, true);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
			return true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Consumables.Potions.Rage",
				new
				{
					RagePotionCritChanceBoost = RageBuff.CritChanceBonus.ToPercentage(2),
					RagePotionGLPBoost = RageBuff.GLPBonus,
					RagePotionABSBoost = RageBuff.ABSBonus,
				}
			);
			tooltips.FirstOrDefault(x => x.Name == "BuffTime").Hide();
		}
	}
}

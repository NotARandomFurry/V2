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
using V2.StatusEffects.Vanilla.Debuffs;

namespace V2.Items.Vanilla.Consumables.Potions
{
	public class WaterBottle : GlobalItem
	{
		public static int HealAmount => 80;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BottledWater;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 50;
			item.AsFood().Size = 0.045;

			item.AsFood().UpdateInStomach += UpdateInStomach;
			item.AsFood().OnBreak += OnBreak;

			item.AsFood().EdibleOnUse = true;
			item.AsFood().AlwaysEatenByUse = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead)
				pred.AddStatus(BuffID.PotionSickness, V2Utils.SensibleTime(minutes: 1), true);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
			if (pred is Player playerPred && !playerPred.HasBuff(BuffID.PotionSickness))
				playerPred.Heal(HealAmount);
			return true;
		}
	}
	public class HoneyBottle : GlobalItem
	{
		public static int HealAmount => 80;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BottledHoney;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 50;
			item.AsFood().Size = 0.045;

			item.AsFood().UpdateInStomach += UpdateInStomach;
			item.AsFood().OnBreak += OnBreak;

			item.AsFood().EdibleOnUse = true;
			item.AsFood().AlwaysEatenByUse = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead)
			{
				pred.AddStatus(BuffID.PotionSickness, V2Utils.SensibleTime(minutes: 1), true);
				pred.AddStatus(BuffID.Honey, V2Utils.SensibleTime(seconds: 15), true);
			}
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
			if (pred is Player playerPred && !playerPred.HasBuff(BuffID.PotionSickness))
				playerPred.Heal(HealAmount);
			return true;
		}
	}
}

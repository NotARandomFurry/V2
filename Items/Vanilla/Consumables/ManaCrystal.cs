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
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Vanilla.Consumables
{
	public class ManaCrystal : GlobalItem
	{
		public static int DigestedManaHeal => 100;
		public static int DigestedManaRegenTime => V2Utils.SensibleTime(seconds: 2, frames: 30);
		public static float StomachCapacityBonus => 0.01f;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.ManaCrystal;

		public override void SetDefaults(Item entity)
		{
			entity.AsFood().MaxHealth = 200;
			entity.AsFood().Size = 0.68;

			entity.AsFood().UpdateInStomach += UpdateInStomach;
			entity.AsFood().OnBreak += OnBreak;

			entity.AsFood().LeftClickEdible = true;
		}

		public static void UpdateInStomach(Item item, Entity pred, bool dead)
		{
			if (!dead)
				return;

			pred.AddStatus(BuffID.ManaRegeneration, DigestedManaRegenTime);
		}

		public static void OnBreak(Item item, Entity pred)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);

			if (pred is Player playerPred)
			{
				playerPred.statMana += DigestedManaHeal;
				if (playerPred.statMana > playerPred.statManaMax2)
					playerPred.statMana = playerPred.statManaMax2;

				if (playerPred.ConsumedManaCrystals >= Player.ManaCrystalMax)
					return;

				playerPred.statManaMax += 20;
				playerPred.statManaMax2 += 20;
				playerPred.ConsumedManaCrystals++;
			}
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			Color manaCrystalsUsedColor = Color.Lerp(Color.DarkBlue, Color.CornflowerBlue, (float)player.ConsumedManaCrystals / (float)Player.ManaCrystalMax);
			tooltips.AddVorariaDynamicTooltip(
				"Vanilla.Consumables.ManaCrystal",
				new
				{
					ManaCrystalEatManaHeal = DigestedManaHeal,
					ManaCrystalEatManaRegenLength = ((double)DigestedManaRegenTime / 60.0).CastToDecimalPlaces(2),
					ManaCrystalStomachCapacityBonus = StomachCapacityBonus.ConvertToPercentageString(2),
					ManaCrystalsUsedColor = (manaCrystalsUsedColor * ((int)Main.mouseTextColor / 255f)).Hex3(),
					ManaCrystalsUsed = player.ConsumedManaCrystals,
					ManaCrystalsMax = Player.ManaCrystalMax
				}
			);
		}
	}

	public class ManaCrystalPlayer : ModPlayer
	{
		public override void PreUpdateBuffs()
		{
			Player.AsPred().StomachCapacityModifier += ManaCrystal.StomachCapacityBonus * Player.ConsumedManaCrystals;
		}
	}
}

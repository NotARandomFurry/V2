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
using V2.Sounds.MuffledSounds;

namespace V2.Items.Vanilla.Consumables
{
	public class LifeCrystal : GlobalItem
	{
		public static int LifeCrystalRegenTime => 75;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.LifeCrystal;

		public override void SetDefaults(Item entity)
		{
			entity.AsFood().MaxHealth = 400;
			entity.AsFood().Size = 0.66;

			entity.AsFood().UpdateInStomach += UpdateInStomach;
			entity.AsFood().OnBreak += OnBreak;
		}

		public static void UpdateInStomach(Item item, Entity pred, bool dead)
		{
			if (!dead)
				return;

			if (pred is Player playerPred)
				playerPred.AddBuff(BuffID.Regeneration, V2Utils.SensibleTime(seconds: LifeCrystalRegenTime));
			else if (pred is NPC NPCPred)
				NPCPred.AddBuff(BuffID.Regeneration, V2Utils.SensibleTime(seconds: LifeCrystalRegenTime));
		}

		public static void OnBreak(Item item, Entity pred)
		{
			if (pred is Player playerPred)
			{
				playerPred.statLife += 20;
				if (playerPred.statLife > playerPred.statLifeMax2)
					playerPred.statLife = playerPred.statLifeMax2;

				if (playerPred.ConsumedLifeCrystals >= Player.LifeCrystalMax)
					return;

				playerPred.statLifeMax += 20;
				playerPred.statLifeMax2 += 20;
				playerPred.ConsumedLifeCrystals++;
				SoundEngine.PlaySound(MuffledMiscSounds.Shatter, playerPred.Center);

			}
			else if (pred is NPC NPCPred)
			{
				NPCPred.life += 20;
				if (NPCPred.life > NPCPred.lifeMax)
					NPCPred.life = NPCPred.lifeMax;
			}
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			Color lifeCrystalsUsedColor = Color.Lerp(Color.DarkRed, Color.HotPink, (float)player.ConsumedLifeCrystals / (float)Player.LifeCrystalMax);
			if (tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name.Contains("Tooltip")) is TooltipLine tooltipLine)
			{
				tooltips.Insert(
					tooltips.IndexOf(tooltipLine),
					new TooltipLine(
						V2.Instance,
						"FlavorText",
						Main.keyState.IsKeyDown(Keys.LeftShift)
						? Language.GetTextValueWith(
							"Mods.V2.ItemTooltip.Vanilla.Consumables.LifeCrystal.Long",
							new
							{
								HealColor = Color.HotPink.Hex3(),
								LifeCrystalEatRegenLength = LifeCrystalRegenTime,
								LifeCrystalsUsedColor = (lifeCrystalsUsedColor * ((int)Main.mouseTextColor / 255f)).Hex3(),
								LifeCrystalsUsed = player.ConsumedLifeCrystals,
								LifeCrystalsMax = Player.LifeCrystalMax
							}
						) : Language.GetTextValue("Mods.V2.ItemTooltip.Vanilla.Consumables.LifeCrystal.Short")
					)
				);
				tooltips.RemoveAll(x => x.Mod == "Terraria" && x.Name.Contains("Tooltip"));
			}
			else
			{
				tooltips.Add(
					new TooltipLine(
						V2.Instance,
						"FlavorText",
						Main.keyState.IsKeyDown(Keys.LeftShift)
						? Language.GetTextValueWith(
							"Mods.V2.ItemTooltip.Vanilla.Consumables.LifeCrystal.Long",
							new
							{
								HealColor = Color.HotPink.Hex3(),
								LifeCrystalEatRegenLength = LifeCrystalRegenTime,
								LifeCrystalsUsedColor = (lifeCrystalsUsedColor * ((int)Main.mouseTextColor / 255f)).Hex3(),
								LifeCrystalsUsed = player.ConsumedLifeCrystals,
								LifeCrystalsMax = Player.LifeCrystalMax
							}
						) : Language.GetTextValue("Mods.V2.ItemTooltip.Vanilla.Consumables.LifeCrystal.Short")
					)
				);
			}
		}
	}
}

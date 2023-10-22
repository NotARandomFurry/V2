using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Starter;
using V2.UI;

namespace V2.Items
{
	public static class PreyItemStuff
	{
		public static PreyItem AsFood(this Item item)
		{
			if (item.IsAir)
				return null;

			bool appliedAsPreyItem = item.TryGetGlobalItem(out PreyItem result);
			if (appliedAsPreyItem)
				return result;
			else
				return null;
		}

		/// <summary>
		/// Deals the given amount of DIRECT digestion damage to the given item, respecting damage variation and luck.<br/>
		/// Should not be used for items worn by eaten players; for them, use TakeIndirectDigestionDamage instead.
		/// </summary>
		/// <param name="pred">The pred currently digesting this player.</param>
		/// <param name="digestionDamage">The total amount of digestion damage to be dealt, before damage variation calculations.</param>
		/// <returns>Whether or not the resulting digestion tick "kills" (depletes the durability of) the item.</returns>
		public static bool TakeDigestionDamage(this Item item, Entity pred, double digestionDamage)
		{
			int trueDigestionDamage = Main.DamageVar((float)digestionDamage);
			if (ModContent.GetInstance<V2ServerConfig>().DefenseInDigestionCalcs)
			{
				trueDigestionDamage -= item.defense / 2;
				if (trueDigestionDamage < 0)
					trueDigestionDamage = 0;
			}
			item.AsFood().Health -= trueDigestionDamage;
			if (item.AsFood().Health <= 0)
			{
				item.AsFood().OnBreak?.Invoke(item, pred);
				item.AsFood().Digested = true;
				return true;
			}
			else
			{
				CombatText digestionText = Main.combatText[CombatText.NewText(
					item.Hitbox,
					Color.DarkGreen,
					trueDigestionDamage,
					false,
					true
				)];
				digestionText.position.X = pred.Center.X;
				digestionText.position.X += pred.direction * 14;
				if (pred.direction == -1)
					digestionText.position.X -= ChatManager.GetStringSize(FontAssets.CombatText[0].Value, digestionText.text, new Vector2(digestionText.scale)).X;
				digestionText.position.Y = item.Center.Y;
				digestionText.position.Y += item.height / 5f;
				digestionText.velocity.X = pred.direction * 2.5f;
				digestionText.velocity.Y = -4f;
				return false;
			}
		}

		public static double CalculateSnackSize(this Item item) => item.AsFood().Size * item.stack;
	}

	public class PreyItem : GlobalItem
	{
		public bool Digested { get; set; }
		public bool FullyDigested { get; set; }
		public PredEntityReference? CurrentCaptor { get; set; }

		public int MaxHealth { get; set; }
		private int _health;
		public int Health
		{
			get => _health;
			set => _health = Math.Min(value, MaxHealth);
		}
		public double Size { get; set; }
		public int AcidResistTier { get; set; }
		public string MealSizeTextOverride { get; set; }

		public delegate void DelegateOnSwallow(Item item, Entity pred);
		public DelegateOnSwallow OnSwallow { get; set; }
		public int OnSwallowDamage { get; set; }
		public string OnSwallowDeathReason { get; set; }
		public int OnSwallowSoreThroatTime { get; set; }

		public delegate void DelegateUpdateInStomach(Item item, Entity pred, bool dead);
		public DelegateUpdateInStomach UpdateInStomach { get; set; }
		public delegate void DelegateOnBreak(Item item, Entity pred);
		public DelegateOnBreak OnBreak { get; set; }

		public bool LeftClickEdible { get; set; }

		public override bool InstancePerEntity => true;

		public PreyItem()
		{
			Digested = false;
			FullyDigested = false;

			MaxHealth = -1;
			Health = -1;
			Size = 0.0;
			AcidResistTier = 0;
			MealSizeTextOverride = null;

			OnSwallow = null;
			OnSwallowDamage = 0;
			OnSwallowDeathReason = null;
			OnSwallowSoreThroatTime = 0;

			UpdateInStomach = null;
			OnBreak = null;

			LeftClickEdible = false;
		}

		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			if (item.AsFood().FullyDigested)
			{
				item.TurnToAir();
				return;
			}

			if (item.CurrentCaptor() is not null)
			{
				item.position = new Vector2(-1, -1);
				item.width = 0;
				item.height = 0;
			}
			else
			{
				item.width = ContentSamples.ItemsByType[item.type].width;
				item.height = ContentSamples.ItemsByType[item.type].height;
			}
		}

		public override void UpdateInventory(Item item, Player player)
		{
			if (item.AsFood().MaxHealth != -1)
			{
				if (item.AsFood().Health == -1 || item.AsFood().Health > item.AsFood().MaxHealth)
					item.AsFood().Health = item.AsFood().MaxHealth;
			}
		}

		public override bool CanUseItem(Item item, Player player)
		{
			if (item.AsFood().LeftClickEdible)
			{
				if (item != player.inventory[58] && PredPlayer.CanSwallow(player, item))
				{
					player.ForceDropItem(player.Center, ref item, out Item itemDrop);
					PredPlayer.Swallow(player, itemDrop);
				}
				return false;
			}
			return true;
		}

		public override bool CanStack(Item destination, Item source)
		{
			if (destination.AsFood().Health != source.AsFood().Health)
				return false;

			return true;
		}

		public override bool CanStackInWorld(Item destination, Item source)
		{
			if (destination.CurrentCaptor() is not null || destination.AsFood().Digested || destination.AsFood().FullyDigested
			 || source.CurrentCaptor() is not null || source.AsFood().Digested || source.AsFood().FullyDigested)
				return false;

			return true;
		}

		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			if (item.CurrentCaptor() is not null || item.AsFood().Digested || item.AsFood().FullyDigested)
				grabRange = 0;
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (item.CurrentCaptor() is not null || item.AsFood().Digested || item.AsFood().FullyDigested)
				return false;

			return true;
		}

		public override bool CanPickup(Item item, Player player) => !(item.CurrentCaptor() is not null || item.AsFood().Digested || item.AsFood().FullyDigested);

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (MaxHealth == -1 || Health == -1)
				return;

			if (item.favorited)
			{
				tooltips.Insert(
					tooltips.IndexOf(tooltips.FirstOrDefault(x => x.Name == "FavoriteDesc")) + 1,
					new TooltipLine(
						V2.Instance,
						"FavoriteNoNoms",
						"Swallowing from inventory will be blocked, but can still be digested by other means"
					)
				);
			}

			double healthRemainingRatio = (double)Health / (double)MaxHealth;
			Color duraPercentColor = Color.Lerp(Color.White, Color.DarkOliveGreen, (float)(1.0 - healthRemainingRatio));
			V2Utils.FindLastTooltipLineBeforeFlavorText(tooltips, out TooltipLine finalLine);
			tooltips.Insert(
				tooltips.IndexOf(finalLine) + 1,
				new TooltipLine(
					V2.Instance,
					"V2Durability",
					"Durability left: " + Health + " / " + MaxHealth + " ([c/" + (duraPercentColor * ((int)Main.mouseTextColor / 255f)).Hex3() + ":" + healthRemainingRatio.ToPercentage(2) + "])"
				)
			);

			double size = item.AsFood().Size;
			string sizeDescription = "Barely a light snack";
			if (size >= 0.04 && size < 0.08)
				sizeDescription = "Light snack";
			if (size >= 0.08 && size < 0.14)
				sizeDescription = "Snack";
			if (size >= 0.14 && size < 0.21)
				sizeDescription = "Large snack";
			if (size >= 0.21 && size < 0.3)
				sizeDescription = "Small meal";
			if (size >= 0.3 && size < 0.4)
				sizeDescription = "Somewhat-small meal";
			if (size >= 0.4 && size < 0.52)
				sizeDescription = "Modest meal";
			if (size >= 0.52 && size < 0.65)
				sizeDescription = "Medium meal";
			if (size >= 0.65 && size < 0.82)
				sizeDescription = "Noteworthy meal";
			if (size >= 0.82 && size < 1)
				sizeDescription = "Sizable meal";
			if (size >= 1 && size < 1.2)
				sizeDescription = "Large meal";
			if (size >= 1.2 && size < 1.5)
				sizeDescription = "Huge meal";
			if (size >= 1.5 && size < 2.0)
				sizeDescription = "Massive meal";
			if (size >= 2.0)
				sizeDescription = "Potentially, a vaguely satisfying meal";

			if (item.AsFood().MealSizeTextOverride is not null or "")
				sizeDescription = item.AsFood().MealSizeTextOverride;

			tooltips.Insert(
				tooltips.IndexOf(finalLine) + 2,
				new TooltipLine(
					V2.Instance,
					"V2SizeAsFood",
					sizeDescription + " (size of " + size + ")"
				)
			);

			if (item.AsFood().LeftClickEdible)
			{
				tooltips.Insert(
					tooltips.IndexOf(finalLine) + 3,
					new TooltipLine(
						V2.Instance,
						"V2EdibleByNormalUse",
						Language.GetTextValue("Mods.V2.ItemTooltip.Generic.EdibleFromNormalUse")
					)
				);
			}
		}

		public override void SaveData(Item item, TagCompound tag)
		{
			tag["VDura"] = Health;
		}

		public override void LoadData(Item item, TagCompound tag)
		{
			Health = tag.GetInt("VDura");
		}
	}
}

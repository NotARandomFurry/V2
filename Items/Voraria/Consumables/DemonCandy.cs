using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Security.Policy;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Accessories.Informational;
using V2.NPCs.Voraria.TownNPCs.Enigma;
using V2.NPCs.Voraria.Underworld.HellHarpy;
using V2.PlayerHandling;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Voraria.Consumables
{
	public class DemonCandy : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
        public static int DigestedRegenTime => V2Utils.SensibleTime(minutes: 5);
        public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.DemonCandy");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.DemonCandy");
		public override void PostUpdate()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (Item.position.Y > (Main.UnderworldLayer * 16) + 200)
				{
					if (Main.rand.NextBool(600) && !NPC.AnyNPCs(ModContent.NPCType<HellHarpy>()))
					{

						NPC harpy = NPC.NewNPCDirect(
							Item.GetSource_FromAI(),
							(int)Item.Center.X,
							(int)Item.Center.Y + 800,
                            ModContent.NPCType<HellHarpy>()
                        );
                        harpy.netUpdate = true;
					}
				}
			}
		}

		public override void SetDefaults()
		{
            Item.consumable = true;
            Item.width = 32;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;

			Item.value = Item.buyPrice(0, 0, 50);
			Item.rare = ItemRarityID.LightRed;

            Item.AsFood().MaxHealth = 500;
            Item.AsFood().Size = 1;

            Item.AsFood().EdibleOnUse = true;

            Item.AsFood().UpdateInStomach += UpdateInStomach;
        }
        public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
        {
            if (dead)
                pred.AddStatus(BuffID.Regeneration, DigestedRegenTime, true);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaDynamicItemTooltip(
				"Voraria.Consumables.DemonCandy",
				new
				{
					Regen = 8,
				}
			);
		}
	}
}
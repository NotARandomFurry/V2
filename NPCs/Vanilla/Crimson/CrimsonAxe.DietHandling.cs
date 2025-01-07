using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Enigma;
using V2.NPCs.Voraria.TownNPCs.Succubus;

namespace V2.NPCs.Vanilla.Crimson
{
	public partial class CrimsonAxe : GlobalNPC
	{
		public static List<(TargetType, int, TargetPriorityLevel)> Diet
		{
			get
			{
				List<(TargetType, int, TargetPriorityLevel)> diet = new List<(TargetType, int, TargetPriorityLevel)>
				{
					// Town NPCs
					(TargetType.NPC, NPCID.Guide, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Merchant, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Nurse, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Demolitionist, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.DyeTrader, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.BestiaryGirl, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Dryad, TargetPriorityLevel.Neutral),
					(TargetType.NPC, ModContent.NPCType<LucindaBound>(), TargetPriorityLevel.Neutral),
					(TargetType.NPC, ModContent.NPCType<Lucinda>(), TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Painter, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.GolferRescue, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Golfer, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.ArmsDealer, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.TravellingMerchant, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.BartenderUnconscious, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.DD2Bartender, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.WebbedStylist, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Stylist, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Clothier, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.BoundMechanic, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Mechanic, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PartyGirl, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.BoundWizard, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Wizard, TargetPriorityLevel.Neutral),
					(TargetType.NPC, ModContent.NPCType<CloverBound>(), TargetPriorityLevel.Neutral),
					(TargetType.NPC, ModContent.NPCType<Clover>(), TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.TaxCollector, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Pirate, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Steampunker, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Cyborg, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Princess, TargetPriorityLevel.Neutral),

					// Pirates
					(TargetType.NPC, NPCID.PirateCorsair, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PirateCrossbower, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PirateDeadeye, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PirateDeckhand, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PirateCaptain, TargetPriorityLevel.Neutral),

					// Typical cavern NPCs
					(TargetType.NPC, NPCID.LostGirl, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Nymph, TargetPriorityLevel.Neutral),

					// Hallowed creatures
					(TargetType.NPC, NPCID.Pixie, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.Unicorn, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.Gastropod, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.RainbowSlime, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.DesertGhoulHallow, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.PigronHallow, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.SandsharkHallow, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.BigMimicHallow, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.DesertLamiaLight, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.EnchantedSword, TargetPriorityLevel.Favorite),

					// Players, of course
					(TargetType.Player, -1, TargetPriorityLevel.Neutral),
				};
				return diet;
			}
		}
	}
}

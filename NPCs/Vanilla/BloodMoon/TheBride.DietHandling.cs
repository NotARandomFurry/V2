using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Vanilla.TownNPCs.Dryad;
using V2.NPCs.Voraria.TownNPCs.Enigma;
using V2.NPCs.Voraria.TownNPCs.Succubus;

namespace V2.NPCs.Vanilla.BloodMoon
{
	public partial class TheBride : GlobalNPC
	{
		public static List<(TargetType, int, TargetPriorityLevel)> Diet
		{
			get
			{
				List<(TargetType, int, TargetPriorityLevel)> diet = [
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
					(TargetType.NPC, NPCID.Princess, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Cyborg, TargetPriorityLevel.Neutral),

					// Pirates
					(TargetType.NPC, NPCID.PirateCorsair, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PirateCrossbower, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PirateDeadeye, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PirateDeckhand, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PirateCaptain, TargetPriorityLevel.Neutral),

					// Lamia
					(TargetType.NPC, NPCID.DesertLamiaDark, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.DesertLamiaLight, TargetPriorityLevel.Neutral),

					// Misc. humanoid NPCs
					(TargetType.NPC, NPCID.Harpy, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.LostGirl, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Nymph, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.HallowBoss, TargetPriorityLevel.Neutral),

					// Players, of course
					(TargetType.Player, -1, TargetPriorityLevel.Neutral),
				];
				return diet;
			}
		}
		public override void PostAI(NPC npc)
		{
			npc.DoContactGulpage(Diet);
		}
	}
}

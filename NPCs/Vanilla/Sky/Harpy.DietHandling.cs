using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Succubus;

namespace V2.NPCs.Vanilla.Sky
{
	public partial class Harpy : GlobalNPC
	{
		public static List<(TargetType, int)> Diet
		{
			get
			{
				List<(TargetType, int)> diet = new List<(TargetType, int)>
				{
					// Town NPCs
					(TargetType.NPC, NPCID.Guide),
					(TargetType.NPC, NPCID.Merchant),
					(TargetType.NPC, NPCID.Nurse),
					(TargetType.NPC, NPCID.Demolitionist),
					(TargetType.NPC, NPCID.DyeTrader),
					(TargetType.NPC, NPCID.BestiaryGirl),
					(TargetType.NPC, NPCID.Dryad),
					(TargetType.NPC, ModContent.NPCType<LucindaBound>()),
					(TargetType.NPC, ModContent.NPCType<Lucinda>()),
					(TargetType.NPC, NPCID.Painter),
					(TargetType.NPC, NPCID.GolferRescue),
					(TargetType.NPC, NPCID.Golfer),
					(TargetType.NPC, NPCID.ArmsDealer),
					(TargetType.NPC, NPCID.TravellingMerchant),
					(TargetType.NPC, NPCID.BartenderUnconscious),
					(TargetType.NPC, NPCID.DD2Bartender),
					(TargetType.NPC, NPCID.WebbedStylist),
					(TargetType.NPC, NPCID.Stylist),
					(TargetType.NPC, NPCID.Clothier),
					(TargetType.NPC, NPCID.BoundMechanic),
					(TargetType.NPC, NPCID.Mechanic),
					(TargetType.NPC, NPCID.PartyGirl),
					(TargetType.NPC, NPCID.BoundWizard),
					(TargetType.NPC, NPCID.Wizard),
					(TargetType.NPC, NPCID.TaxCollector),
					(TargetType.NPC, NPCID.Pirate),
					(TargetType.NPC, NPCID.Steampunker),

					// Pirates
					(TargetType.NPC, NPCID.PirateCorsair),
					(TargetType.NPC, NPCID.PirateCrossbower),
					(TargetType.NPC, NPCID.PirateDeadeye),
					(TargetType.NPC, NPCID.PirateDeckhand),
					(TargetType.NPC, NPCID.PirateCaptain),

					// Lamia
					(TargetType.NPC, NPCID.DesertLamiaDark),
					(TargetType.NPC, NPCID.DesertLamiaLight),

					// Misc. humanoid NPCs
					(TargetType.NPC, NPCID.LostGirl),
					(TargetType.NPC, NPCID.Nymph),

					// Players, of course
					(TargetType.Player, -1),
				};
				if (!V2.BlacklistsActive)
				{
					diet.AddRange(new List<(TargetType, int)>
					{
						(TargetType.NPC, NPCID.SleepingAngler),
						(TargetType.NPC, NPCID.Angler),
						(TargetType.NPC, NPCID.Princess),
					});
				}
				return diet;
			}
		}

		public override void PostAI(NPC npc)
		{
			npc.DoContactGulpage(Diet);
		}
	}
}

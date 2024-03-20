using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Weapons.Summon;

namespace V2.NPCs.Vanilla.TownNPCs.TravellingMerchant
{
	public class TravellingMerchant : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.TravellingMerchant && !V2.GetFooled;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Male;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
		}

		public static void SetupTravelShop_GetItem(Player playerWithHighestLuck, int[] rarity, ref int it, int minimumRarity = 0)
		{
			if (minimumRarity <= 4 && playerWithHighestLuck.RollLuck(rarity[4]) == 0)
				it = 3309;

			if (minimumRarity <= 3 && playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 3314;

			if (playerWithHighestLuck.RollLuck(rarity[5]) == 0)
				it = 1987;

			if (minimumRarity > 4)
				return;

			if (playerWithHighestLuck.RollLuck(rarity[4]) == 0 && Main.hardMode)
				it = 2270;

			if (playerWithHighestLuck.RollLuck(rarity[4]) == 0 && Main.hardMode)
				it = 4760;

			if (playerWithHighestLuck.RollLuck(rarity[4]) == 0)
				it = 2278;

			if (playerWithHighestLuck.RollLuck(rarity[4]) == 0)
				it = 2271;

			if (minimumRarity > 3)
				return;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0 && Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
				it = 2223;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 2272;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 2276;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 2284;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 2285;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 2286;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 2287;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 4744;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0 && NPC.downedBoss3)
				it = 2296;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 3628;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0 && Main.hardMode)
				it = 4091;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 4603;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 4604;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 5297;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 4605;

			if (playerWithHighestLuck.RollLuck(rarity[3]) == 0)
				it = 4550;

			if (minimumRarity > 2)
				return;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 2268;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0 && WorldGen.shadowOrbSmashed)
				it = 2269;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 1988;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 2275;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 2279;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 2277;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4555;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4321;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4323;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 5390;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4549;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4561;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4774;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 5136;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 5305;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4562;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4558;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4559;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4563;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = 4666;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0)
				it = ModContent.ItemType<PaperMaidSummon>();

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0 && (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedQueenBee || Main.hardMode))
			{
				it = 4347;
				if (Main.hardMode)
					it = 4348;
			}

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0 && NPC.downedBoss1)
				it = 3262;

			if (playerWithHighestLuck.RollLuck(rarity[2]) == 0 && NPC.downedMechBossAny)
				it = 3284;

			if (minimumRarity > 1)
				return;

			if (playerWithHighestLuck.RollLuck(rarity[1]) == 0)
				it = 2267;

			if (playerWithHighestLuck.RollLuck(rarity[1]) == 0)
				it = 2214;

			if (playerWithHighestLuck.RollLuck(rarity[1]) == 0)
				it = 2215;

			if (playerWithHighestLuck.RollLuck(rarity[1]) == 0)
				it = 2216;

			if (playerWithHighestLuck.RollLuck(rarity[1]) == 0)
				it = 2217;

			if (playerWithHighestLuck.RollLuck(rarity[1]) == 0)
				it = 3624;

			if (playerWithHighestLuck.RollLuck(rarity[1]) == 0)
			{
				if (Main.remixWorld)
					it = 671;
				else
					it = 2273;
			}

			if (playerWithHighestLuck.RollLuck(rarity[1]) == 0)
				it = 2274;

			if (minimumRarity <= 0)
			{
				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 2266;

				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 2281 + Main.rand.Next(3);

				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 2258;

				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 2242;

				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 2260;

				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 3637;

				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 4420;

				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 3119;

				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 3118;

				if (playerWithHighestLuck.RollLuck(rarity[0]) == 0)
					it = 3099;
			}
		}
	}
}

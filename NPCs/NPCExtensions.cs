using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using V2.Core;
using V2.PlayerHandling;
using V2.StatusEffects.Debuffs;

namespace V2.NPCs
{
	public static class NPCExtensions
	{
		public static bool IsFoodFor(this NPC npc, Entity pred)
		{
			if (npc.CurrentCaptor() is null)
				return false;

			if (pred is NPC predNPC)
			{
				if (PredNPC.GetStomachTracker(predNPC) is null)
					return false;

				return npc.CurrentCaptor() == PredNPC.GetStomachTracker(predNPC);
			}
			else if (pred is Player predPlayer)
			{
				if (predPlayer.AsPred().StomachTracker is null)
					return false;

				return npc.CurrentCaptor() == predPlayer.AsPred().StomachTracker;
			}
			return false;
		}

		public static List<NPC> GetNearbyResidentNPCs(this NPC npc, out int npcsWithinHouse, out int npcsWithinVillage)
		{
			List<NPC> list = new List<NPC>();
			npcsWithinHouse = 0;
			npcsWithinVillage = 0;
			Vector2 value = new Vector2(npc.homeTileX, npc.homeTileY);
			if (npc.homeless)
				value = new Vector2(npc.Center.X / 16f, npc.Center.Y / 16f);

			for (int i = 0; i < 200; i++)
			{
				if (i == npc.whoAmI)
					continue;

				NPC nPC = Main.npc[i];
				if (nPC.active && nPC.townNPC && !npc.IsNotReallyTownNPC() && !WorldGen.TownManager.CanNPCsLiveWithEachOther_ShopHelper(npc, nPC))
				{
					Vector2 value2 = new Vector2(nPC.homeTileX, nPC.homeTileY);
					if (nPC.homeless)
						value2 = nPC.Center / 16f;

					float num = Vector2.Distance(value, value2);
					if (num < 25f)
					{
						list.Add(nPC);
						npcsWithinHouse++;
					}
					else if (num < 120f)
					{
						npcsWithinVillage++;
					}
				}
			}

			return list;
		}

		public static bool IsNotReallyTownNPC(this NPC npc)
		{
			int type = npc.type;
			if (type == 37 || type == 368 || NPCID.Sets.ActsLikeTownNPC[type])
				return true;

			return false;
		}

		public static void DoContactGulpage(this NPC npc)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC preyNPC = Main.npc[i];
				if (preyNPC.active && preyNPC.life > 0 && preyNPC.whoAmI != npc.whoAmI)
				{
					if (npc.Hitbox.Intersects(preyNPC.Hitbox) && PredNPC.CanSwallow(npc, preyNPC))
					{
						if (npc.type == NPCID.HallowBoss && preyNPC.type == NPCID.PartyGirl)
							PredNPC.Swallow(preyNPC, npc);
						else
							PredNPC.Swallow(npc, preyNPC);
					}
				}
			}
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player preyPlayer = Main.player[i];
				if (preyPlayer.active && !preyPlayer.dead)
				{
					if (npc.Hitbox.Intersects(preyPlayer.Hitbox) && PredNPC.CanSwallow(npc, preyPlayer))
						PredNPC.Swallow(npc, preyPlayer);
				}
			}
			for (int i = 0; i < Main.maxItems; i++)
			{
				Item preyItem = Main.item[i];
				if (preyItem.active)
				{
					if (npc.Hitbox.Intersects(preyItem.Hitbox) && PredNPC.CanSwallow(npc, preyItem))
						PredNPC.Swallow(npc, preyItem);
				}
			}
		}

		public static int SoftenedStacks(this NPC npc) => Math.Min(Softened.MaxStacks, (int)Math.Floor((double)npc.AsFood().SoftenedDigestionDamageTaken / (npc.lifeMax * Softened.MaxHealthDigestedForOneStack)));

		public static bool CanItemsBeThievedBy(this NPC npc, Entity pred)
		{
			if (pred is Player playerPred)
			{
				if (playerPred.AsPred().charmStealPreyLoot)
					return true;
			}
			return false;
		}
	}

	public static class NPCChatHelper
	{
		public static void AddHumanoidPredMessages(this List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.1",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.2",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.3",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.4",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.5",
			});
		}
	}
}

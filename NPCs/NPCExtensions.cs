using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.StatusEffects.Voraria.Debuffs;

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
			else if (pred is Projectile predProjectile)
			{
				if (PredProjectile.GetStomachTracker(predProjectile) is null)
					return false;

				return npc.CurrentCaptor() == PredProjectile.GetStomachTracker(predProjectile);
			}
			return false;
		}

		public static void SwitchToPattern<T>(this NPC npc, Entity target) where T : NPCBehaviorPattern, new()
		{
			npc.AsV2NPC().BehaviorPattern = new T();
			npc.AsV2NPC().BehaviorPattern.DoBehavior(npc, target);
		}

		public static void TryFindNewTarget(this NPC npc, List<(TargetType, int)> specificWhitelist = null)
		{
			List<(int index, TargetType type, int aggro, float dist)> targetList = new List<(int, TargetType, int, float)>();
			foreach (Player targetPlayer in Main.ActivePlayers)
			{
				if (targetPlayer.dead || targetPlayer.npcTypeNoAggro[npc.type] || targetPlayer.aggro <= -1000)
					continue;

				bool inSpecificWhitelist = false;
				if (specificWhitelist is not null)
				{
					foreach ((TargetType type, int ID) in specificWhitelist)
					{
						if (type == TargetType.Player)
						{
							inSpecificWhitelist = true;
							break;
						}
					}
				}
				else
					inSpecificWhitelist = true;

				if (!inSpecificWhitelist)
					continue;

				float distanceToTarget = npc.Distance(targetPlayer.TrueCenter());
				float negativeAggroDistMult = 1f;
				if (targetPlayer.aggro < 0)
					negativeAggroDistMult -= (float)Math.Abs(targetPlayer.aggro) / 1000f;
				bool canTarget = distanceToTarget <= npc.AsV2NPC().TargetRange * negativeAggroDistMult;
				if (npc.AsV2NPC().TargetRequiresLineOfSight)
					canTarget &= Collision.CanHitLine(npc.TrueCenter(), npc.width, npc.height, targetPlayer.TrueCenter(), targetPlayer.width, targetPlayer.height);

				if (canTarget)
					targetList.Add((targetPlayer.whoAmI, TargetType.Player, targetPlayer.aggro, distanceToTarget));
			}
			foreach (NPC targetNPC in Main.ActiveNPCs)
			{
				if (targetNPC.life <= 0 || targetNPC.AsV2NPC().Aggro <= -1000)
					continue;

				bool inSpecificWhitelist = false;
				if (specificWhitelist is not null)
				{
					foreach ((TargetType type, int ID) in specificWhitelist)
					{
						if (type == TargetType.NPC && (ID == targetNPC.type || ID == targetNPC.netID))
						{
							inSpecificWhitelist = true;
							break;
						}
					}
				}
				else
					inSpecificWhitelist = true;

				if (!inSpecificWhitelist)
					continue;

				float distanceToTarget = npc.Distance(targetNPC.TrueCenter());
				float negativeAggroDistMult = 1f;
				if (targetNPC.AsV2NPC().Aggro < 0)
					negativeAggroDistMult -= (float)Math.Abs(targetNPC.AsV2NPC().Aggro) / 1000f;
				bool canTarget = distanceToTarget <= npc.AsV2NPC().TargetRange * negativeAggroDistMult;
				if (npc.AsV2NPC().TargetRequiresLineOfSight)
					canTarget &= Collision.CanHitLine(npc.TrueCenter(), npc.width, npc.height, targetNPC.TrueCenter(), targetNPC.width, targetNPC.height);

				if (canTarget)
					targetList.Add((targetNPC.whoAmI, TargetType.NPC, targetNPC.AsV2NPC().Aggro, distanceToTarget));
			}
			foreach (Projectile targetProjectile in Main.ActiveProjectiles)
			{
				if (targetProjectile.AsFood().Health <= 0 || targetProjectile.AsV2Proj().Aggro <= -1000)
					continue;

				bool inSpecificWhitelist = false;
				if (specificWhitelist is not null)
				{
					foreach ((TargetType type, int ID) in specificWhitelist)
					{
						if (type == TargetType.Projectile && ID == targetProjectile.type)
						{
							inSpecificWhitelist = true;
							break;
						}
					}
				}
				else
					inSpecificWhitelist = true;

				if (!inSpecificWhitelist)
					continue;

				float distanceToTarget = npc.Distance(targetProjectile.TrueCenter());
				float negativeAggroDistMult = 1f;
				if (targetProjectile.AsV2Proj().Aggro < 0)
					negativeAggroDistMult -= (float)Math.Abs(targetProjectile.AsV2Proj().Aggro) / 1000f;
				bool canTarget = distanceToTarget <= npc.AsV2NPC().TargetRange * negativeAggroDistMult;
				if (npc.AsV2NPC().TargetRequiresLineOfSight)
					canTarget &= Collision.CanHitLine(npc.TrueCenter(), npc.width, npc.height, targetProjectile.TrueCenter(), targetProjectile.width, targetProjectile.height);

				if (canTarget)
					targetList.Add((targetProjectile.whoAmI, TargetType.Projectile, targetProjectile.AsV2Proj().Aggro, distanceToTarget));
			}

			if (targetList.Count > 0)
			{
				targetList = targetList.OrderByDescending(x => x.aggro).ToList();
				if (npc.target != -1 && npc.AsV2NPC().TargetType != TargetType.None)
				{
					switch (npc.AsV2NPC().TargetType)
					{
						case TargetType.Player:
							Player previousTargetPlayer = Main.player[npc.target];
							if (previousTargetPlayer.aggro >= targetList[0].aggro)
								return;
							break;
						case TargetType.NPC:
							NPC previousTargetNPC = Main.npc[npc.target];
							if (previousTargetNPC.AsV2NPC().Aggro >= targetList[0].aggro)
								return;
							break;
						case TargetType.Projectile:
							Projectile previousTargetProjectile = Main.projectile[npc.target];
							if (previousTargetProjectile.AsV2Proj().Aggro >= targetList[0].aggro)
								return;
							break;
					}
				}
				targetList.RemoveAll(x => x.aggro < targetList[0].aggro);
				targetList = targetList.OrderBy(x => x.dist).ToList();
				npc.target = targetList[0].index;
				npc.AsV2NPC().TargetType = targetList[0].type;
			}
		}
 
		public static void TryVerifyRemainingTarget(this NPC npc, List<(TargetType, int)> specificWhitelist = null)
		{
			if (npc.target != -1)
			{
				switch (npc.AsV2NPC().TargetType)
				{
					case TargetType.Player:
						Player targetPlayer = Main.player[npc.target];
						if (!targetPlayer.active || targetPlayer.dead || targetPlayer.CurrentCaptor() is not null || (npc.AsV2NPC().TargetRequiresLineOfSight && !Collision.CanHitLine(npc.TrueCenter(), npc.width, npc.height, targetPlayer.TrueCenter(), targetPlayer.width, targetPlayer.height)))
						{
							npc.AsV2NPC().TargetType = TargetType.None;
							npc.target = -1;
						}
						break;
					case TargetType.NPC:
						NPC targetNPC = Main.npc[npc.target];
						if (!targetNPC.active || targetNPC.life <= 0 || targetNPC.CurrentCaptor() is not null || (npc.AsV2NPC().TargetRequiresLineOfSight && !Collision.CanHitLine(npc.TrueCenter(), npc.width, npc.height, targetNPC.TrueCenter(), targetNPC.width, targetNPC.height)))
						{
							npc.AsV2NPC().TargetType = TargetType.None;
							npc.target = -1;
						}
						break;
					case TargetType.Projectile:
						Projectile targetProjectile = Main.projectile[npc.target];
						if (!targetProjectile.active || targetProjectile.AsFood().Health <= 0 || targetProjectile.CurrentCaptor() is not null || (npc.AsV2NPC().TargetRequiresLineOfSight && !Collision.CanHitLine(npc.TrueCenter(), npc.width, npc.height, targetProjectile.TrueCenter(), targetProjectile.width, targetProjectile.height)))
						{
							npc.AsV2NPC().TargetType = TargetType.None;
							npc.target = -1;
						}
						break;
					case TargetType.Other:
					case TargetType.None:
					default:
						break;
				}
			}
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

		public static void DoContactGulpage(this NPC npc, List<(TargetType, int)> specificWhitelist = null)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC preyNPC = Main.npc[i];
				if (preyNPC.active && preyNPC.life > 0 && preyNPC.whoAmI != npc.whoAmI)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID) in specificWhitelist)
						{
							if (type == TargetType.NPC && ID == preyNPC.netID)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

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
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID) in specificWhitelist)
						{
							if (type == TargetType.Player)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(preyPlayer.Hitbox) && PredNPC.CanSwallow(npc, preyPlayer))
						PredNPC.Swallow(npc, preyPlayer);
				}
			}
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile preyProjectile = Main.projectile[i];
				if (preyProjectile.active)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID) in specificWhitelist)
						{
							if (type == TargetType.Projectile && ID == preyProjectile.type)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(preyProjectile.Hitbox) && PredNPC.CanSwallow(npc, preyProjectile))
						PredNPC.Swallow(npc, preyProjectile);
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

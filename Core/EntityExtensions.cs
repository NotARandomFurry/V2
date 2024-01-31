using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using V2.NPCs;
using V2.PlayerHandling;

namespace V2.Core
{
	public static class EntityExtensions
	{
		public static Vector2 TrueCenter(this Entity entity) => new Vector2(entity.position.X + ((float)entity.width / 2f), entity.position.Y + ((float)entity.height / 2f));
		
		public static double GetBellySize(this Entity entity) {
			if (entity is Player player)
				return player.AsPred().StomachSize;

			if (entity is NPC npc)
				return npc.AsPred().GetVisualBellySize.Invoke(npc);

			return 0.0;
		}

		public static void AddStatus(this Entity entity, int statusID, int intendedTime, bool fromDigestingSomething = false)
		{
			intendedTime += 1;
			if (entity is Player playerPred)
			{
				if (fromDigestingSomething)
				{
					if (Main.debuff[statusID])
						intendedTime = (int)Math.Round((double)intendedTime / playerPred.AsPred().DebuffDisextensionFactor);
				}
				playerPred.AddBuff(statusID, intendedTime);
			}
			else if (entity is NPC NPCPred)
				NPCPred.AddBuff(statusID, intendedTime);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static VoreTracker CurrentCaptor(this Entity entity)
		{
			if (entity is Player player)
			{
				if (ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(
						x => x.Prey.FirstOrDefault(
							y => y.Instance is Player preyPlayer && preyPlayer.whoAmI == player.whoAmI && !y.NoHealth
						) is not null
						  || x.PreyQueue.FirstOrDefault(
							y => y.Instance is Player preyPlayer && preyPlayer.whoAmI == player.whoAmI && !y.NoHealth
						) is not null
					) is VoreTracker tracker)
					return tracker;

				return null;
			}
			else if (entity is NPC npc)
			{
				if (ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(
						x => x.Prey.FirstOrDefault(
							y => y.Instance is NPC preyNPC && preyNPC.whoAmI == npc.whoAmI && !y.NoHealth
						) is not null
						  || x.PreyQueue.FirstOrDefault(
							y => y.Instance is NPC preyNPC && preyNPC.whoAmI == npc.whoAmI && !y.NoHealth
						) is not null
					) is VoreTracker tracker)
					return tracker;

				return null;
			}
			else if (entity is Projectile projectile)
			{
				if (ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(
						x => x.Prey.FirstOrDefault(
							y => y.Instance is Projectile preyProjectile && preyProjectile.whoAmI == projectile.whoAmI && !y.NoHealth
						) is not null
						  || x.PreyQueue.FirstOrDefault(
							y => y.Instance is Projectile preyProjectile && preyProjectile.whoAmI == projectile.whoAmI && !y.NoHealth
						) is not null
					) is VoreTracker tracker)
					return tracker;

				return null;
			}
			else if (entity is Item item)
			{
				if (ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(
						x => x.Prey.FirstOrDefault(
							y => y.Instance is Item preyItem && preyItem.type == item.type && preyItem.stack == item.stack && !y.NoHealth
						) is not null
						  || x.PreyQueue.FirstOrDefault(
							y => y.Instance is Item preyItem && preyItem.type == item.type && preyItem.stack == item.stack && !y.NoHealth
						) is not null
					) is VoreTracker tracker)
					return tracker;

				return null;
			}
			return null;
		}

		public static int GetPredStat(this Entity entity, string predStat)
		{
			if (entity is Player player)
			{
				return predStat switch
				{
					"GLP" => player.AsPred().GLP.Total,
					"TUM" => player.AsPred().TUM.Total,
					"ACI" => player.AsPred().ACI.Total,
					"ABS" => player.AsPred().ABS.Total,
					_ => 0,
				};
			}
			else if (entity is NPC npc)
			{
				return (int)Math.Max(0, Math.Floor(Math.Max(npc.AsPred().MaxStomachCapacity - 0.80, 0) / 0.04));
			}
			return 0;
		}

		public static double StruggleStrength(this Entity entity)
		{
			if (entity is Player player)
				return player.AsFood().StruggleStrength;
			else if (entity is NPC npc)
				return npc.AsFood().StruggleStrength;

			return 0;
		}
	}
}

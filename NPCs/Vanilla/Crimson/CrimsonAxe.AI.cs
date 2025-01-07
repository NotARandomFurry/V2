using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria;
using V2.Items.Voraria.Charms;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Crimson
{
	public partial class CrimsonAxe : GlobalNPC
	{
		public static bool V2CrimsonAxeAI(NPC npc)
		{
			npc.noGravity = true;
			npc.ai[3]++;
			if (npc.ai[3] > 15) npc.ai[3] = 0;
			Entity targetEntity = null;
			npc.TryFindNewTarget(Diet);
			npc.TryVerifyRemainingTarget(Diet);
			if (npc.target != -1)
			{
				targetEntity = npc.AsV2NPC().TargetType switch
				{
					TargetType.Player => Main.player[npc.target],
					TargetType.NPC => Main.npc[npc.target],
					TargetType.Projectile => Main.projectile[npc.target],
					_ => null,
				};
			}

			if (npc.AsV2NPC().BehaviorPattern is not null)
				npc.AsV2NPC().BehaviorPattern.DoBehavior(npc, targetEntity);
			else
				npc.SwitchToPattern<CrimsonAxeAI.Idle>(targetEntity);

			return false;
		}

		public static void VanillaCrimsonAxeAI(NPC npc)
		{
			npc.noGravity = true;
			npc.noTileCollide = true;
			
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.3f, 0.15f, 0.05f);

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
				npc.TargetClosest();

			Player targetPlayer = Main.player[npc.target];
			if (npc.ai[0] == 0f)
			{
				float num335 = 9f;
				Vector2 vector37 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num336 = targetPlayer.position.X + (float)(targetPlayer.width / 2) - vector37.X;
				float num337 = targetPlayer.position.Y + (float)(targetPlayer.height / 2) - vector37.Y;
				float num338 = (float)Math.Sqrt(num336 * num336 + num337 * num337);
				num338 = num335 / num338;
				num336 *= num338;
				num337 *= num338;
				npc.velocity.X = num336;
				npc.velocity.Y = num337;
				npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 0.785f;
				npc.ai[0] = 1f;
				npc.ai[1] = 0f;
				npc.netUpdate = true;
			}
			else if (npc.ai[0] == 1f)
			{
				if (npc.justHit)
				{
					npc.ai[0] = 2f;
					npc.ai[1] = 0f;
				}

				npc.velocity *= 0.99f;
				npc.ai[1] += 1f;
				if (npc.ai[1] >= 100f)
				{
					npc.netUpdate = true;
					npc.ai[0] = 2f;
					npc.ai[1] = 0f;
					npc.velocity.X = 0f;
					npc.velocity.Y = 0f;
				}
				else
				{
					npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 0.785f;
				}
			}
			else
			{
				if (npc.justHit)
				{
					npc.ai[0] = 2f;
					npc.ai[1] = 0f;
				}

				npc.velocity *= 0.96f;
				npc.ai[1] += 1f;
				float num340 = npc.ai[1] / 120f;
				num340 = 0.1f + num340 * 0.4f;
				npc.rotation += num340 * (float)npc.direction;
				if (npc.ai[1] >= 120f)
				{
					npc.netUpdate = true;
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
				}
			}
		}
	}
}

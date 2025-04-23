using System;
using Terraria;
using V2.Core;

namespace V2.NPCs.Vanilla.BloodMoon
{
	public static partial class TheBrideAI
	{
		public class AimlessWanderingWalking : NPCBehaviorPattern
		{
			public override int PatternLength => -1;

			public override void AI(NPC npc, Entity target)
			{
				if (target is not null)
				{
					npc.SwitchToPattern<ChasingNextMeal>(target);
					npc.AsV2NPC().BehaviorPattern.SecondaryTimer = 0;
					npc.netUpdate = true;
					return;
				}

				float acceleration = TheBrideStuff.GroundedAccel(npc);
				float maxWalkSpeed = TheBrideStuff.GroundedMaxSpeed(npc);

				acceleration /= 1f + (float)PredNPC.GetCurrentBellyWeight(npc);
				maxWalkSpeed /= 1f + (float)(PredNPC.GetCurrentBellyWeight(npc) / 2f);

				if (Math.Abs(npc.velocity.X) > maxWalkSpeed)
				{
					if (npc.velocity.Y == 0f)
						npc.velocity *= 0.8f;
				}
				else if (Math.Abs(npc.velocity.X) < maxWalkSpeed)
				{
					npc.velocity.X += acceleration * npc.direction;
					if (Math.Abs(npc.velocity.X) > maxWalkSpeed)
						npc.velocity.X *= maxWalkSpeed / Math.Abs(npc.velocity.X);
				}

				SecondaryTimer++;
				if (SecondaryTimer > 80 && Main.rand.NextBool(40))
				{
					npc.SwitchToPattern<AimlessWanderingStill>(target);
					npc.AsV2NPC().BehaviorPattern.SecondaryTimer = 0;
					npc.netUpdate = true;
					return;
				}
			}
		}
	}
}

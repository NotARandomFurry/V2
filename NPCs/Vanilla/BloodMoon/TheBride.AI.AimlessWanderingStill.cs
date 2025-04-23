using Terraria;
using V2.Core;

namespace V2.NPCs.Vanilla.BloodMoon
{
	public static partial class TheBrideAI
	{
		public class AimlessWanderingStill : NPCBehaviorPattern
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

				npc.velocity.X *= 0.8f;

				SecondaryTimer++;
				if (SecondaryTimer > 50 && Main.rand.NextBool(150))
				{
					SecondaryTimer = 0;
					npc.direction *= -1;
				}
				if (SecondaryTimer > 80 && Main.rand.NextBool(20))
				{
					npc.SwitchToPattern<AimlessWanderingWalking>(target);
					npc.AsV2NPC().BehaviorPattern.SecondaryTimer = 0;
					npc.netUpdate = true;
					return;
				}
			}
		}
	}
}

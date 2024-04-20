using Microsoft.Xna.Framework;
using Terraria;
using V2.Core;

namespace V2.NPCs.Vanilla.Sky
{
	public static partial class HarpyAI
	{
		public class DiveBombing : NPCBehaviorPattern
		{
			public override int PatternLength => HarpyStuff.Statistics.DiveBombLength;

			public override void AI(NPC npc, Entity target)
			{
				if (target is null)
				{
					npc.SwitchToPattern<MainFlying>(target);
					npc.netUpdate = true;
					return;
				}
				if (PatternTimer >= PatternLength)
				{
					npc.AsHarpy().WingFlapTimer = -4;
					npc.SwitchToPattern<RecoveringFromDiveBomb>(target);
					npc.netUpdate = true;
					return;
				}

				npc.velocity.X = 5f * npc.direction;
				npc.velocity.Y = 5f;
			}
		}
	}
}

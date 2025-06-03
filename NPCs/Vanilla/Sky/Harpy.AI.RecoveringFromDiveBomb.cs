using Terraria;
using V2.Core;

namespace V2.NPCs.Vanilla.Sky
{
	public static partial class HarpyAI
	{
		public class RecoveringFromDiveBomb : NPCBehaviorPattern
		{
			public override int PatternLength => HarpyStuff.Statistics.DiveBombRecoveryLength;

			public override void AI(NPC npc, Entity target)
			{
				if (PatternTimer >= PatternLength)
				{
					npc.SwitchToPattern<MainFlying>(target);
					npc.netUpdate = true;
					return;
				}

				npc.velocity *= 0.97f;

				float weightMovementModifier = ((float)PredNPC.GetCurrentBellyWeight(npc) + (float)npc.AsPred().ExtraWeight) * 0.8f;
				float verticalWeightMovementModifier = 1f + (0.75f * weightMovementModifier);
				npc.velocity.Y += 0.04f * verticalWeightMovementModifier;
				if (npc.velocity.Y > 0.55f * verticalWeightMovementModifier)
					npc.velocity.Y = 0.55f * verticalWeightMovementModifier;

				float wingFlapWeightMovementModifier = 1f + (0.35f * weightMovementModifier);
				npc.AsHarpy().WingFlapTimer++;
				int minDelay = npc.AsPred().GetVisualWeightStage.Invoke(npc) switch
				{
					0 => 80,
					1 => 65,
					2 => 50,
					3 => 35,
					4 => 20,
				};
				int flapChance = npc.AsPred().GetVisualWeightStage.Invoke(npc) switch
				{
					0 => 80,
					1 => 70,
					2 => 60,
					3 => 50,
					4 => 40,
				};
				if (npc.AsHarpy().WingFlapTimer >= minDelay && Main.rand.NextBool(flapChance))
					npc.AsHarpy().WingFlapTimer = -4;
				else if (npc.AsHarpy().WingFlapTimer == 0)
					npc.velocity.Y = -3.2f / (wingFlapWeightMovementModifier * 0.8f);
			}
		}
	}
}

using Terraria;
using V2.Core;

namespace V2.NPCs.Vanilla.Sky
{
	public static partial class HarpyAI
	{
		public class MainFlying : NPCBehaviorPattern
		{
			public override int PatternLength => -1;

			public override void AI(NPC npc, Entity target)
			{
				if (target is not null)
				{
					npc.SwitchToPattern<ChargingDiveBomb>(target);
					npc.AsV2NPC().BehaviorPattern.SecondaryTimer = 0;
					npc.netUpdate = true;
					return;
				}

				float weightMovementModifier = ((float)PredNPC.GetCurrentBellyWeight(npc) + (float)npc.AsPred().ExtraWeight) * 0.8f;

				npc.AsHarpy().DirectionChangeTimer++;
				if (npc.AsHarpy().DirectionChangeTimer > 0 && Main.rand.NextBool(180))
				{
					npc.AsHarpy().DirectionChangeTimer = -V2Utils.SensibleTime(seconds: 3);
					npc.direction *= -1;
				}
				float horizontalWeightMovementModifier = 1f + (0.4f * weightMovementModifier);
				npc.velocity.X += 0.20f / horizontalWeightMovementModifier * npc.direction;
				float maxHorizSpeed = HarpyStuff.Statistics.MaxMoveSpeed / horizontalWeightMovementModifier;
				if (npc.velocity.X > maxHorizSpeed)
					npc.velocity.X = maxHorizSpeed;
				if (npc.velocity.X < -maxHorizSpeed)
					npc.velocity.X = -maxHorizSpeed;

				float verticalWeightMovementModifier = 1f + (0.75f * weightMovementModifier);
				npc.velocity.Y += 0.06f * verticalWeightMovementModifier;
				if (npc.velocity.Y > 0.55f * verticalWeightMovementModifier)
					npc.velocity.Y = 0.55f * verticalWeightMovementModifier;

				float wingFlapWeightMovementModifier = 1f + (0.35f * weightMovementModifier);
				npc.AsHarpy().WingFlapTimer++;
				int minDelay = npc.AsPred().GetVisualWeightStage.Invoke(npc) switch
				{
					0 => 60,
					1 => 50,
					2 => 36,
					3 => 22,
					4 => 14,
					_ => 14,
				};
				int flapChance = npc.AsPred().GetVisualWeightStage.Invoke(npc) switch
				{
					0 => 60,
					1 => 50,
					2 => 36,
					3 => 22,
					4 => 14,
					_ => 14,
				};
				if (npc.AsHarpy().WingFlapTimer >= minDelay && Main.rand.NextBool(flapChance))
					npc.AsHarpy().WingFlapTimer = -4;
				else if (npc.AsHarpy().WingFlapTimer == 0)
					npc.velocity.Y = -3.2f / (wingFlapWeightMovementModifier * 0.8f);
			}
		}
	}
}

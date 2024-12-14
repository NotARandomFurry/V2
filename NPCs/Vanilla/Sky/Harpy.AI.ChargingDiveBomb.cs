using Microsoft.Xna.Framework;
using System;
using Terraria;
using V2.Core;

namespace V2.NPCs.Vanilla.Sky
{
	public static partial class HarpyAI
	{
		public class ChargingDiveBomb : NPCBehaviorPattern
		{
			public override int PatternLength => 2;

			public override void AI(NPC npc, Entity target)
			{
				if (target is null)
				{
					npc.SwitchToPattern<MainFlying>(target);
					npc.netUpdate = true;
					return;
				}
				if (SecondaryTimer >= PatternLength)
				{
					npc.SwitchToPattern<DiveBombing>(target);
					npc.netUpdate = true;
				}

				float weightMovementModifier = ((float)PredNPC.GetCurrentBellyWeight(npc) + (float)npc.AsPred().ExtraWeight) * 0.8f;

				npc.spriteDirection = npc.direction = (target.position.X >= npc.TrueCenter().X).ToDirectionInt();

				Vector2 targetPos = target.TrueCenter();
				targetPos.X += V2Utils.TileCountAsPixelCount(12.5) * -npc.direction;
				targetPos.Y -= V2Utils.TileCountAsPixelCount(12.5);

				float horizontalWeightMovementModifier = 1f + (0.4f * weightMovementModifier);
				npc.velocity.X += 0.25f / horizontalWeightMovementModifier * (targetPos.X >= npc.TrueCenter().X).ToDirectionInt();
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
				float wingFlapExtraModifier = 1f;
				npc.AsHarpy().WingFlapTimer++;
				int minDelay = npc.AsPred().GetVisualWeightStage.Invoke(npc) switch
				{
					0 => 50,
					1 => 40,
					2 => 26,
                    3 => 14,
                    4 => 12,
                };
				int flapChanceDenominator = npc.AsPred().GetVisualWeightStage.Invoke(npc) switch
                {
                    0 => 60,
                    1 => 50,
                    2 => 36,
                    3 => 22,
                    4 => 14,
                };
				if (npc.TrueCenter().Distance(targetPos) < V2Utils.TileCountAsPixelCount(6.0))
				{
					minDelay = (int)Math.Round((float)minDelay * 0.60f);
					flapChanceDenominator = (int)Math.Round((float)flapChanceDenominator * 0.60f);
					wingFlapExtraModifier = 0.60f;
				}
				if (npc.AsHarpy().WingFlapTimer >= minDelay && Main.rand.NextBool(flapChanceDenominator) && npc.position.Y > targetPos.Y)
					npc.AsHarpy().WingFlapTimer = -4;
				else if (npc.AsHarpy().WingFlapTimer == 0)
				{
					if (npc.TrueCenter().Distance(targetPos) < V2Utils.TileCountAsPixelCount(6.0))
						SecondaryTimer++;
					npc.velocity.Y = -3.2f / (wingFlapWeightMovementModifier * 0.8f) * wingFlapExtraModifier;
				}
			}
		}
	}
}

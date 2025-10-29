using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;

namespace V2.NPCs.Vanilla.Hallow
{
	public static class PrismaticLacewingStuff
	{
		public static PrismaticLacewing AsPrismaticLacewing(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out PrismaticLacewing lacewing))
				throw new Exception("this instance of a Prismatic Lacewing, supposedly, doesn't exist");

			return lacewing;
		}
	}

	public partial class PrismaticLacewing : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.EmpressButterfly;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;
			npc.AsV2NPC().NewAIMethod = V2PrismaticLacewingAI;

			npc.AsFood().DefinedBaseSize = 0.035;
			npc.AsPred().WeightGainRatio = 0.10;
			npc.AsPred().MaxStomachCapacity = 5.0;
			npc.AsPred().BaseStomachacheMeterCapacity = 10000.0;
			npc.AsFood().WellFedPower = 0.05;
			npc.AsFood().CalorieMultiplier = 0.50;

			npc.AsPred().SmallGulpThreshold = 0.00;
			npc.AsPred().BigGulps = null;
			npc.AsPred().CanBeForceFed = CanLacewingBeForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().StandardBurps = null;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
			npc.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
		}

		public static bool CanLacewingBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange([
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Hallow.EmpressButterfly.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Hallow.EmpressButterfly.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Hallow.EmpressButterfly.3",
			]);
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Hallow.EmpressButterfly.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 2.0 / 3.0;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			double baseDigestionTickDamage = 31.4;
			return baseDigestionTickDamage;
		}
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				seconds: 40
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(6.75 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				5
			);
		}

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(4.0 * Math.Sqrt(npc.AsPred().ExtraWeight)),
				0
			);
		}

		public override void PostAI(NPC npc)
		{
			if (ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress)
				npc.DoContactGulpage([(TargetType.NPC, NPCID.HallowBoss, TargetPriorityLevel.Favorite)]);
		}

		public static int GetEmpressDigestionStage(NPC npc)
		{
			if (PredNPC.GetStomachTracker(npc) is null)
				return 0;

			PreyData candyFairy = PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss);
			if (candyFairy is null || candyFairy.WeightLeftToDigest < 6.0)
				return 0;
			else
			{
				if (!candyFairy.NoHealth)
					return 1;
				else
				{
					if (candyFairy.WeightLeftToDigest > 34.0)
						return 1;
					else if (candyFairy.WeightLeftToDigest > 28.0)
						return 2;
					else if (candyFairy.WeightLeftToDigest > 16.0)
						return 3;
					else if (candyFairy.WeightLeftToDigest > 6.0)
						return 4;
					else
						return 0;
				}
			}
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
			int flapFrameTime = 7;
			int flapFrame;
			if (npc.frameCounter < (double)flapFrameTime)
				flapFrame = 0;
			else if (npc.frameCounter < (double)(flapFrameTime * 2))
				flapFrame = 1;
			else if (npc.frameCounter < (double)(flapFrameTime * 3))
				flapFrame = 2;
			else
				flapFrame = 1;

			if (GetEmpressDigestionStage(npc) > 0)
			{
				npc.frame = new Rectangle(
					70 * flapFrame,
					116 * (GetEmpressDigestionStage(npc) - 1),
					68,
					114
				);
			}
			else
			{
				npc.frame = new Rectangle(
					60 * flapFrame,
					60 * npc.AsPred().GetVisualBellySize.Invoke(npc),
					58,
					58
				);
			}
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			string tastyLightButterflyTummyTypeString = "_MainSheet";
			if (GetEmpressDigestionStage(npc) > 0)
				tastyLightButterflyTummyTypeString = "_BossBelly_EmpressOfLight";
			string tastyLightButterflyWeightString = GetVisualWeightStage(npc) > 0 ? ("_WeightGain" + GetVisualWeightStage(npc)) : "_BaseWeight";
			string exactMainBodyTextureString = "V2/NPCs/Vanilla/Hallow/PrismaticLacewing" + tastyLightButterflyWeightString + tastyLightButterflyTummyTypeString;

			Texture2D exactLacewingTexture = ModContent.Request<Texture2D>(exactMainBodyTextureString, AssetRequestMode.ImmediateLoad).Value;
			Color whiteColor = Color.White;
			float lerpRatioA = 0.5f;
			float lerpRatioB = 0f;
			int rotatingAfterimages = 6;
			float strangeConstantThatIDoNotKnowTheMeaningOf = (float)Math.Cos(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * ((float)Math.PI * 2f)) / 2f + 0.5f;
			strangeConstantThatIDoNotKnowTheMeaningOf = MathHelper.Max(strangeConstantThatIDoNotKnowTheMeaningOf, Utils.GetLerpValue(0f, 60f, npc.ai[2], clamped: true));
			float num277 = 6f;
			float num278 = 0f;
			Vector2 origin = new Vector2(30f, 12f);
			if (GetEmpressDigestionStage(npc) > 0)
				origin = new Vector2(54f, 12f);

			for (int i = 0; i < rotatingAfterimages; i++)
			{
				Color modifiedDrawColorA = drawColor;
				modifiedDrawColorA = Color.Lerp(modifiedDrawColorA, whiteColor, lerpRatioA);
				modifiedDrawColorA = npc.GetAlpha(modifiedDrawColorA);
				modifiedDrawColorA = Color.Lerp(modifiedDrawColorA, whiteColor, lerpRatioB);
				modifiedDrawColorA *= 1f - strangeConstantThatIDoNotKnowTheMeaningOf;
				Vector2 afterimageDrawPosition = npc.Center + ((float)i / (float)rotatingAfterimages * ((float)Math.PI * 2f) + npc.rotation + num278).ToRotationVector2() * num277 * strangeConstantThatIDoNotKnowTheMeaningOf - screenPos;
				spriteBatch.Draw(exactLacewingTexture, afterimageDrawPosition, npc.frame, modifiedDrawColorA, npc.rotation, origin, npc.scale, spriteEffects, 0f);
			}

			Vector2 mainDrawPosition = npc.Center - screenPos;
			spriteBatch.Draw(exactLacewingTexture, mainDrawPosition, npc.frame, npc.GetAlpha(drawColor), npc.rotation, origin, npc.scale, spriteEffects, 0f);
			num278 = MathHelper.Lerp(0f, 3f, Utils.GetLerpValue(0f, 60f, npc.ai[2], clamped: true));
			for (int i = 0; i < rotatingAfterimages; i++)
			{
				Color rainbowColor = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Main.hslToRgb((Main.GlobalTimeWrappedHourly + (float)i / (float)rotatingAfterimages) % 1f, 1f, 0.5f));
				rainbowColor = npc.GetAlpha(rainbowColor);
				rainbowColor *= 1f - strangeConstantThatIDoNotKnowTheMeaningOf * 0.5f;
				rainbowColor.A = 0;
				float num296 = 2f + npc.ai[2];
				Vector2 rainbowAfterimageDrawPosition = npc.Center + ((float)i / (float)rotatingAfterimages * ((float)Math.PI * 2f) + npc.rotation + num278).ToRotationVector2() * (num296 * strangeConstantThatIDoNotKnowTheMeaningOf + 2f) - screenPos;
				spriteBatch.Draw(exactLacewingTexture, rainbowAfterimageDrawPosition, npc.frame, rainbowColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
			}

			spriteBatch.Draw(exactLacewingTexture, mainDrawPosition, npc.frame, new Color(255, 255, 255, 0) * 0.1f, npc.rotation, origin, npc.scale, spriteEffects, 0f);

			return false;
		}
	}
}

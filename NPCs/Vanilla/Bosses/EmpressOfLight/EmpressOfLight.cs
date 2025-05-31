using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Bosses.EmpressOfLight
{
	public static class CandyFairyStuff
	{
		public static class ItemTheftRules
		{
			public static ItemTheftRule WeaponDrops => new ItemTheftRule(
				type: (npc, pred) => {
					List<int> weapons = [
						ItemID.PiercingStarlight,
						ItemID.FairyQueenRangedItem,
						ItemID.FairyQueenMagicItem,
						ItemID.RainbowWhip
					];
					return Main.rand.NextFromCollection(weapons);
				},
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 0.40,
						GameModeID.Expert => 1.0 / 3.0,
						_ => 0.25,
					};
				}
			);
			public static ItemTheftRule StarGuitar => new ItemTheftRule(
				type: (npc, pred) => ItemID.SparkleGuitar,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 0.0333,
						GameModeID.Expert => 0.025,
						_ => 0.02,
					};
				}
			);
			public static ItemTheftRule EmpressWings => new ItemTheftRule(
				type: (npc, pred) => ItemID.RainbowWings,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 0.1,
						GameModeID.Expert => 0.075,
						_ => 0.05,
					};
				}
			);
			public static ItemTheftRule PrismaticDye => new ItemTheftRule(
				type: (npc, pred) => ItemID.HallowBossDye,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 0.15,
						GameModeID.Expert => 0.125,
						_ => 0.1,
					};
				}
			);
			public static ItemTheftRule Mask => new ItemTheftRule(
				type: (npc, pred) => ItemID.FairyQueenMask,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 0.125,
						GameModeID.Expert => 0.1,
						_ => 0.0667,
					};
				}
			);
			public static ItemTheftRule Trophy => new ItemTheftRule(
				type: (npc, pred) => ItemID.FairyQueenTrophy,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 0.25,
						GameModeID.Expert => 0.20,
						_ => 0.1,
					};
				}
			);
			public static ItemTheftRule ExpertDrop => new ItemTheftRule(
				type: (npc, pred) => ItemID.EmpressFlightBooster,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 1,
						GameModeID.Expert => 0.5,
						_ => 0,
					};
				}
			);
			public static ItemTheftRule MasterTrophy => new ItemTheftRule(
				type: (npc, pred) => ItemID.FairyQueenMasterTrophy,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 1,
						_ => 0,
					};
				}
			);
			public static ItemTheftRule MasterPetItem => new ItemTheftRule(
				type: (npc, pred) => ItemID.FairyQueenPetItem,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 1.0 / 3.0,
						_ => 0,
					};
				}
			);
			public static ItemTheftRule HangrySwordDrop => new ItemTheftRule(
				type: (npc, pred) => ItemID.EmpressBlade,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => (npc.AI_120_HallowBoss_IsGenuinelyEnraged() && pred is Player) ? 1f : 0f
			);
		}
		public static CandyFairy AsCandyFairy(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out CandyFairy unreasonablyThickFairy))
				throw new Exception("this instance of the Empress of Light, sadly, can't be pred or prey. the unreasonably thick candy fairy can't be food today, I guess");

			return unreasonablyThickFairy;
		}

		public static SoundStyle MuffledCandyFairyMusic => new SoundStyle("V2/Sounds/MuffledMusic/EmpressOfLight", SoundType.Sound) with { MaxInstances = 0 };

		public static SoundStyle MuffledCandyFairyScreech1 => new SoundStyle("V2/Sounds/MuffledSounds/Item160", SoundType.Sound) with { MaxInstances = 0 };
		public static SoundStyle MuffledCandyFairyScreech2 => new SoundStyle("V2/Sounds/MuffledSounds/Item161", SoundType.Sound) with { MaxInstances = 0 };
		public static SoundStyle MuffledCandyFairyDeathScreech => new SoundStyle("V2/Sounds/MuffledSounds/NPC_Killed_65", SoundType.Sound) with { MaxInstances = 0 };
	}

	public class CandyFairy : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public static int MuffledScreechMinDelay => V2Utils.SensibleTime(seconds: 5);
		private int _muffledScreechDelay;
		public int MuffledScreechDelay
		{
			get => _muffledScreechDelay;
			set => _muffledScreechDelay = Math.Max(0, value);
		}
		public SlotId MuffledMusic { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.HallowBoss;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsFood().DefinedBaseSize = 40.0;
			npc.AsPred().MaxStomachCapacity = 1000.0;
			npc.AsPred().BaseStomachacheMeterCapacity = 50000.0;
            npc.AsFood().CalorieMultiplier = 25;
            npc.AsFood().WellFedPower = 2;

            npc.AsV2NPC().NewAIMethod = V2UnreasonablyThickFairyAI;
			npc.AsFood().SpecialPreyAI = UnreasonablyThickFairyPreyAI;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 3.75;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().CanBeForceFed = CanUnreasonablyThickFairyBeForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().OnDigestionKill = OnDigestionKill;
			npc.AsPred().MouthSoundRawOffset = npc.TrueCenter() + new Vector2(npc.direction * 0f, -40f);
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 3.75;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;
			npc.AsPred().WeightGainRatio = 0.40;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
			npc.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			npc.AsCandyFairy().MuffledScreechDelay = 0;

			npc.AsFood().DigestedDeathSound = CandyFairyStuff.MuffledCandyFairyDeathScreech;

			npc.AsFood().ItemTheftRules = [
				CandyFairyStuff.ItemTheftRules.WeaponDrops,
				CandyFairyStuff.ItemTheftRules.StarGuitar,
				CandyFairyStuff.ItemTheftRules.EmpressWings,
				CandyFairyStuff.ItemTheftRules.PrismaticDye,
				CandyFairyStuff.ItemTheftRules.Mask,
				CandyFairyStuff.ItemTheftRules.Trophy,
				CandyFairyStuff.ItemTheftRules.ExpertDrop,
				CandyFairyStuff.ItemTheftRules.MasterTrophy,
				CandyFairyStuff.ItemTheftRules.MasterPetItem,
				CandyFairyStuff.ItemTheftRules.HangrySwordDrop,
			];
		}

		public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (npc.ai[0] is 8f or 9f)
			{
				modifiers.FinalDamage *= 0f;
			}
		}

		public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
		{
			if (npc.ai[0] is 8f or 9f)
			{
				modifiers.FinalDamage *= 0f;
			}
		}

		public static bool CanUnreasonablyThickFairyBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange([
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.4",
			]);
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.Hardcore");
			}
		}

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => Main.dayTime ? 1000.0 : 100.0;
		public static double GetDigestionTickRate(NPC npc, PreyData prey)
		{
			if (npc.AI_120_HallowBoss_IsGenuinelyEnraged())
				return 12.0;
			else if (Main.bloodMoon)
			{
				if (npc.AI_120_HallowBoss_IsInPhase2())
					return 9.0;
				else
					return 6.0;
			}
			else
			{
				if (npc.AI_120_HallowBoss_IsInPhase2())
					return 4.5;
				else
					return 3.0;
			}
		}

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 0,
				seconds: 6
			);
			if (Main.dayTime)
				baseAbsorptionRate *= 10.0;
			if (npc.AI_120_HallowBoss_IsInPhase2())
				baseAbsorptionRate *= 1.5;
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(1.5 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				9
			);
		}

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(0.4 * Math.Sqrt(npc.AsPred().ExtraWeight)),
				3
			);
		}

		public static int GetKingSlimeDigestionStage(NPC npc)
		{
			if (PredNPC.GetStomachTracker(npc)?.Prey?.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.KingSlime) is not PreyData giantJelloDessert)
				return 0;

			if (!giantJelloDessert.NoHealth)
				return 1;
			else
			{
				if (giantJelloDessert.WeightLeftToDigest > 60.0)
					return 1;
				else if (giantJelloDessert.WeightLeftToDigest > 50.0)
					return 2;
				else if (giantJelloDessert.WeightLeftToDigest > 40.0)
					return 3;
				else
					return 0;
			}
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
			if (GetKingSlimeDigestionStage(npc) > 0)
			{
				npc.frame = new Rectangle(
					128 * (GetKingSlimeDigestionStage(npc) - 1),
					214 * npc.AsPred().GetVisualWeightStage.Invoke(npc),
					126,
					212
				);
			}
			else
			{
				npc.frame = new Rectangle(
					92 * npc.AsPred().GetVisualBellySize.Invoke(npc),
					170 * npc.AsPred().GetVisualWeightStage.Invoke(npc),
					90,
					168
				);
			}
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			string fatFuckingTummyTypeString = "_MainSheet";
			if (GetKingSlimeDigestionStage(npc) > 0)
				fatFuckingTummyTypeString = "_BossBelly_KingSlime";

			string exactMainBodyTexture = "V2/NPCs/Vanilla/Bosses/EmpressOfLight/EmpressOfLight_MainBody" + fatFuckingTummyTypeString;

			string fatassBitchGiantessFairyWeightString = GetVisualWeightStage(npc) > 0 ? ("_WeightGain" + GetVisualWeightStage(npc)) : "_BaseWeight";

			Texture2D mainBody = ModContent.Request<Texture2D>(exactMainBodyTexture, AssetRequestMode.ImmediateLoad).Value;
			Vector2 npcCenterOnScreen = npc.Center - screenPos;
			bool inPhase2 = npc.AI_120_HallowBoss_IsInPhase2();
			int num = (int)npc.ai[0];
			Texture2D wingsBack = TextureAssets.Extra[ExtrasID.HallowBossWingsBack].Value;
			Rectangle wingsBackFrameBounds = wingsBack.Frame(1, 11, 0, (int)(npc.localAI[0] / 4f) % 11);
			Color drawColorWithAlphaConsidered = npc.GetAlpha(drawColor);
			Texture2D leftArm = TextureAssets.Extra[ExtrasID.HallowBossArmsLeft].Value;
			Texture2D rightArm = TextureAssets.Extra[ExtrasID.HallowBossArmsRight].Value;
			Texture2D wingsFront = TextureAssets.Extra[ExtrasID.HallowBossWings].Value;
			Vector2 drawOrigin = new Vector2(npc.frame.Width / 2, 84);
			DrawNPCDirect_GetHallowBossArmFrame(npc, out var armFrame_Count, out var armFrameToUseLeft, out var armFrameToUseRight);
			Rectangle leftArmFrameBounds = leftArm.Frame(1, armFrame_Count, 0, armFrameToUseLeft);
			Rectangle rightArmFrameBounds = rightArm.Frame(1, armFrame_Count, 0, armFrameToUseRight);
			Vector2 leftArmOrigin = leftArmFrameBounds.Size() / 2f;
			Vector2 rightArmOrigin = rightArmFrameBounds.Size() / 2f;
			int num2 = 0;
			int num3 = 0;
			if (armFrameToUseLeft == 5)
				num2 = 1;

			if (armFrameToUseRight == 5)
				num3 = 1;

			float num4 = 1f;
			int num5 = 0;
			int num6 = 0;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			if (num == 8 || num == 9)
			{
				num7 = Utils.GetLerpValue(0f, 30f, npc.ai[1], clamped: true) * Utils.GetLerpValue(90f, 30f, npc.ai[1], clamped: true);
				num8 = Utils.GetLerpValue(0f, 30f, npc.ai[1], clamped: true) * Utils.GetLerpValue(90f, 70f, npc.ai[1], clamped: true);
				num9 = Utils.GetLerpValue(0f, 15f, npc.ai[1], clamped: true) * Utils.GetLerpValue(45f, 30f, npc.ai[1], clamped: true);
				drawColorWithAlphaConsidered = Color.Lerp(drawColorWithAlphaConsidered, Color.White, num7);
				num4 *= 1f - num9;
				num5 = 4;
				num6 = 3;
			}

			if (num == 10)
			{
				num7 = Utils.GetLerpValue(30f, 90f, npc.ai[1], clamped: true) * Utils.GetLerpValue(165f, 90f, npc.ai[1], clamped: true);
				num8 = Utils.GetLerpValue(0f, 60f, npc.ai[1], clamped: true) * Utils.GetLerpValue(180f, 120f, npc.ai[1], clamped: true);
				num9 = Utils.GetLerpValue(0f, 60f, npc.ai[1], clamped: true) * Utils.GetLerpValue(180f, 120f, npc.ai[1], clamped: true);
				drawColorWithAlphaConsidered = Color.Lerp(drawColorWithAlphaConsidered, Color.White, num7);
				num4 *= 1f - num9;
				num6 = 4;
			}

			if (num6 + num5 > 0)
			{
				for (int i = -num6; i <= num6 + num5; i++)
				{
					if (i == 0)
						continue;

					Color color2 = Color.White;
					Vector2 position = npcCenterOnScreen;
					if (num == 8 || num == 9)
					{
						float hue = ((float)i + 5f) / 10f;
						float num10 = 200f;
						float num11 = (float)Main.timeForVisualEffects / 60f;
						Vector3 vector2 = Vector3.Transform(matrix: Matrix.CreateRotationX((num11 - 0.3f + (float)i * 0.1f) * 0.7f * ((float)Math.PI * 2f)) * Matrix.CreateRotationY((num11 - 0.8f + (float)i * 0.3f) * 0.7f * ((float)Math.PI * 2f)) * Matrix.CreateRotationZ((num11 + (float)i * 0.5f) * 0.1f * ((float)Math.PI * 2f)), position: Vector3.Forward);
						num10 += Utils.GetLerpValue(-1f, 1f, vector2.Z, clamped: true) * 150f;
						Vector2 spinningpoint = new Vector2(vector2.X, vector2.Y) * num10 * num7;
						float lerpValue = Utils.GetLerpValue(90f, 0f, npc.ai[1], clamped: true);
						color2 = Main.hslToRgb(hue, 1f, MathHelper.Lerp(0.5f, 1f, lerpValue)) * 0.8f * num8;
						color2.A /= 3;
						position += spinningpoint.RotatedBy(npc.ai[1] / 180f * ((float)Math.PI * 2f));
					}

					if (num == 10)
					{
						if (npc.ai[1] >= 90f)
						{
							float num12 = (float)Main.timeForVisualEffects / 90f;
							int num13 = i;
							if (num13 < 0)
								num13++;

							Vector2 vector3 = (((float)num13 + 0.5f) * ((float)Math.PI / 4f) + (float)Math.PI * 2f * num12).ToRotationVector2();
							position += vector3 * new Vector2(600f * num7, 150f * num7);
						}
						else
						{
							position += 200f * new Vector2(i, 0f) * num7;
						}

						color2 = Color.White * 0.8f * num8 * num4;
						color2.A /= 3;
					}

					if (i > num6)
					{
						float lerpValue2 = Utils.GetLerpValue(30f, 70f, npc.ai[1], clamped: true);
						if (lerpValue2 == 0f)
							continue;

						position = npcCenterOnScreen + npc.velocity * -3f * ((float)i - 4f) * lerpValue2;
						color2 *= 1f - num9;
					}

					spriteBatch.Draw(wingsBack, position, wingsBackFrameBounds, color2, npc.rotation, wingsBackFrameBounds.Size() / 2f, npc.scale * 2f, SpriteEffects.None, 0f);
					spriteBatch.Draw(wingsFront, position, wingsBackFrameBounds, color2, npc.rotation, wingsBackFrameBounds.Size() / 2f, npc.scale * 2f, SpriteEffects.None, 0f);
					if (inPhase2)
					{
						Texture2D value6 = TextureAssets.Extra[ExtrasID.HallowBossTentacles].Value;
						Rectangle value7 = value6.Frame(1, 8, 0, (int)(npc.localAI[0] / 4f) % 8);
						spriteBatch.Draw(value6, position, value7, color2, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
					}

					spriteBatch.Draw(mainBody, position, npc.frame, color2, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
					for (int j = 0; j < 2; j++)
					{
						if (j == num2)
							spriteBatch.Draw(leftArm, position, leftArmFrameBounds, color2, npc.rotation, leftArmOrigin, npc.scale, SpriteEffects.None, 0f);

						if (j == num3)
							spriteBatch.Draw(rightArm, position, rightArmFrameBounds, color2, npc.rotation, rightArmOrigin, npc.scale, SpriteEffects.None, 0f);
					}
				}
			}

			drawColorWithAlphaConsidered *= num4;
			spriteBatch.Draw(wingsBack, npcCenterOnScreen, wingsBackFrameBounds, drawColorWithAlphaConsidered, npc.rotation, wingsBackFrameBounds.Size() / 2f, npc.scale * 2f, SpriteEffects.None, 0f);
			if (!npc.IsABestiaryIconDummy)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
			}

			DrawData value8 = new DrawData(wingsFront, npcCenterOnScreen, wingsBackFrameBounds, drawColorWithAlphaConsidered, npc.rotation, wingsBackFrameBounds.Size() / 2f, npc.scale * 2f, SpriteEffects.None);
			GameShaders.Misc["HallowBoss"].Apply(value8);
			value8.Draw(spriteBatch);
			Main.pixelShader.CurrentTechnique.Passes[0].Apply();
			if (!npc.IsABestiaryIconDummy)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
			}

			float num14 = (float)Math.Sin(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f) * 0.5f) * 0.5f + 0.5f;
			Color color3 = Main.hslToRgb((num14 * 0.08f + 0.6f) % 1f, 1f, 0.5f);
			color3.A = 0;
			color3 *= 0.6f;
			if (NPC.ShouldEmpressBeEnraged())
			{
				color3 = Main.OurFavoriteColor;
				color3.A = 0;
				color3 *= 0.3f;
			}

			color3 *= num4 * npc.Opacity;
			if (inPhase2)
			{
				Texture2D value9 = TextureAssets.Extra[ExtrasID.HallowBossTentacles].Value;
				Rectangle value10 = value9.Frame(1, 8, 0, (int)(npc.localAI[0] / 4f) % 8);
				spriteBatch.Draw(value9, npcCenterOnScreen, value10, drawColorWithAlphaConsidered, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
				for (float num15 = 0f; num15 < 1f; num15 += 0.25f)
				{
					Vector2 vector4 = npc.rotation.ToRotationVector2().RotatedBy(num15 * ((float)Math.PI * 2f) + (float)Math.PI / 4f) * MathHelper.Lerp(2f, 8f, num14);
					spriteBatch.Draw(value9, npcCenterOnScreen + vector4, value10, color3, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
				}
			}

			spriteBatch.Draw(mainBody, npcCenterOnScreen, npc.frame, drawColorWithAlphaConsidered, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
			if (inPhase2)
			{
				Texture2D glowySkirtOverlay = ModContent.Request<Texture2D>("V2/NPCs/Vanilla/Bosses/EmpressOfLight/EmpressOfLight_SkirtOverlay" + fatFuckingTummyTypeString, AssetRequestMode.ImmediateLoad).Value;
				for (float num16 = 0f; num16 < 1f; num16 += 0.25f)
				{
					Vector2 vector5 = npc.rotation.ToRotationVector2().RotatedBy(num16 * ((float)Math.PI * 2f) + (float)Math.PI / 4f) * MathHelper.Lerp(2f, 8f, num14);
					spriteBatch.Draw(glowySkirtOverlay, npcCenterOnScreen + vector5, npc.frame, color3, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
				}
			}

			for (int k = 0; k < 2; k++)
			{
				if (k == num2)
					spriteBatch.Draw(leftArm, npcCenterOnScreen, leftArmFrameBounds, drawColorWithAlphaConsidered, npc.rotation, leftArmOrigin, npc.scale, SpriteEffects.None, 0f);

				if (k == num3)
					spriteBatch.Draw(rightArm, npcCenterOnScreen, rightArmFrameBounds, drawColorWithAlphaConsidered, npc.rotation, rightArmOrigin, npc.scale, SpriteEffects.None, 0f);
			}
			return false;
		}

		private static void DrawNPCDirect_GetHallowBossArmFrame(NPC npc, out int armFrame_Count, out int armFrameToUseLeft, out int armFrameToUseRight)
		{
			int num = 0;
			int num2 = 1;
			int num3 = 2;
			int num4 = 3;
			int num5 = 4;
			int num6 = 5;
			int num7 = 6;
			armFrame_Count = 7;
			armFrameToUseLeft = num;
			armFrameToUseRight = num;
			float num8 = npc.ai[1];
			switch ((int)npc.ai[0])
			{
				case 6:
					armFrameToUseRight = (armFrameToUseLeft = ((num8 < 6f) ? num3 : ((num8 < 174f) ? num4 : ((!(num8 < 180f)) ? num : num3))));
					break;
				case 0:
					armFrameToUseRight = (armFrameToUseLeft = ((num8 < 106f) ? num2 : ((!(num8 < 110f)) ? num : num3)));
					break;
				case 2:
				case 11:
					armFrameToUseLeft = ((num8 < 5f) ? num3 : ((!(num8 < 65f)) ? num3 : num4));
					break;
				case 5:
					armFrameToUseRight = ((num8 < 6f) ? num3 : ((!(num8 < 54f)) ? num3 : num4));
					break;
				case 4:
				case 10:
					armFrameToUseRight = (armFrameToUseLeft = ((num8 < 6f) ? num3 : ((!(num8 < 54f)) ? num3 : num4)));
					break;
				case 8:
				case 9:
					{
						int whatIsThis = ((num8 < 10f) ? num3 : ((num8 < 20f) ? num4 : ((!(num8 < 30f)) ? num6 : num3)));
						int targetRightArmFrame = whatIsThis;
						int targetLeftArmFrame = whatIsThis;
						int num15 = (int)npc.ai[3];
						int num16 = -1;
						if (num8 < 30f)
						{
							if (num15 == -1 * num16)
								targetLeftArmFrame = num2;

							if (num15 == num16)
								targetRightArmFrame = num2;
						}

						int num17 = num6;
						int num18 = num7;
						if (num15 == num16 && targetLeftArmFrame == num17)
							targetLeftArmFrame = num18;

						if (num15 == -1 * num16 && targetRightArmFrame == num17)
							targetRightArmFrame = num18;

						armFrameToUseLeft = targetLeftArmFrame;
						armFrameToUseRight = targetRightArmFrame;
						break;
					}
				case 7:
					{
						bool isExpertMode = Main.GameModeInfo.IsExpertMode;
						int num10 = (isExpertMode ? 40 : 60);
						int num11 = 0;
						int num12 = 5;
						if (num8 < (float)(num11 + num12))
						{
							armFrameToUseLeft = num3;
							break;
						}

						num11 += num12;
						if (num8 < (float)(num11 + num10 - num12))
						{
							armFrameToUseLeft = num4;
							break;
						}

						num11 += num10 - num12;
						if (num8 < (float)(num11 + num12))
						{
							armFrameToUseLeft = num4;
							armFrameToUseRight = num3;
							break;
						}

						num11 += num12;
						if (num8 < (float)(num11 + num10 - num12))
						{
							armFrameToUseLeft = num4;
							armFrameToUseRight = num4;
							break;
						}

						num11 += num10 - num12;
						if (num8 < (float)(num11 + num10))
						{
							armFrameToUseLeft = num5;
							armFrameToUseRight = num4;
							break;
						}

						num11 += num10;
						if (num8 < (float)(num11 + num10))
						{
							armFrameToUseLeft = num5;
							armFrameToUseRight = num5;
							break;
						}

						num11 += num10;
						if (isExpertMode)
						{
							if (num8 < (float)(num11 + num12))
							{
								armFrameToUseLeft = num4;
								armFrameToUseRight = num5;
								break;
							}

							num11 += num12;
							if (num8 < (float)(num11 + num10 - num12))
							{
								armFrameToUseLeft = num2;
								armFrameToUseRight = num5;
								break;
							}

							num11 += num10 - num12;
							if (num8 < (float)(num11 + num12))
							{
								armFrameToUseLeft = num2;
								armFrameToUseRight = num4;
								break;
							}

							num11 += num12;
							if (num8 < (float)(num11 + num10 - num12))
							{
								armFrameToUseLeft = num2;
								armFrameToUseRight = num2;
								break;
							}

							num11 += num10 - num12;
						}

						if (num8 >= (float)num11)
						{
							armFrameToUseLeft = num3;
							armFrameToUseRight = num3;
						}

						break;
					}
				case 1:
				case 3:
					break;
			}
		}

		public static bool V2UnreasonablyThickFairyAI(NPC npc)
		{
			if (npc.ai[0] is 8f or 9f)
				npc.DoContactGulpage();

			if (npc.target == -1 || !Main.player[npc.target].IsFoodFor(npc, out bool pastTense) || pastTense)
				return true;

			if (npc.ai[0] is 8f or 9f)
			{
				float num = 0.5f;
				float num2 = 12f;
				int num33 = ((npc.ai[0] != 8f) ? 1 : (-1));
				if (npc.ai[1] <= 40f)
				{
					if (npc.ai[1] == 20f)
						SoundEngine.PlaySound(SoundID.Item160, npc.Center);

					NPCAimedTarget targetData3 = npc.GetTargetData();
					Vector2 destination = (targetData3.Invalid ? npc.Center : targetData3.Center) + new Vector2(num33 * -550, 0f);
					npc.SimpleFlyMovement(npc.DirectionTo(destination).SafeNormalize(Vector2.Zero) * num2, num * 2f);
					if (npc.ai[1] == 40f)
						npc.velocity *= 0.3f;
				}
				else if (npc.ai[1] <= 90f)
				{
					npc.velocity = Vector2.Lerp(value2: new Vector2(num33 * 50, 0f), value1: npc.velocity, amount: 0.05f);
					if (npc.ai[1] == 90f)
						npc.velocity *= 0.7f;
				}
				else
				{
					npc.velocity *= 0.92f;
				}

				bool flag = npc.AI_120_HallowBoss_IsInPhase2();
				bool flag2 = Main.expertMode;
				int num17 = 0;
				if (flag)
					num17 += 15;

				if (flag2)
					num17 += 5;
				float num32 = 20 - num17;
				npc.ai[1] += 1f;
				if (npc.ai[1] >= 90f + num32)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
				}
			}
			else
			{
				npc.ai[0] = 1f;
				npc.ai[1] = 0f;
				npc.velocity *= 0.85f;
				npc.netUpdate = true;
			}

			return false;
		}

		public static void UnreasonablyThickFairyPreyAI(NPC npc, Entity pred)
		{
			bool muffledMusicPlaying = SoundEngine.TryGetActiveSound(npc.AsCandyFairy().MuffledMusic, out ActiveSound muffledMusic);
			if (!muffledMusicPlaying)
			{
				npc.AsCandyFairy().MuffledMusic = SoundEngine.PlaySound(
					CandyFairyStuff.MuffledCandyFairyMusic,
					pred.TrueCenter()
				);
				SoundEngine.TryGetActiveSound(npc.AsCandyFairy().MuffledMusic, out muffledMusic);
			}

			if (muffledMusic is null)
				return;

			muffledMusic.Position = pred.TrueCenter();
			muffledMusic.Volume = (float)npc.life / (float)npc.lifeMax;

			npc.AsCandyFairy().MuffledScreechDelay -= 1;
			if (npc.AsCandyFairy().MuffledScreechDelay == 0 && Main.rand.NextBool(200))
			{
				npc.AsCandyFairy().MuffledScreechDelay = MuffledScreechMinDelay;
				SoundEngine.PlaySound(
					(
						Main.rand.NextBool()
						  ? CandyFairyStuff.MuffledCandyFairyScreech1
						  : CandyFairyStuff.MuffledCandyFairyScreech2
					)
					with
					{
						Volume = 1f,
						PitchVariance = 0.07f
					},
					pred.TrueCenter()
				);
			}
		}
	}
}

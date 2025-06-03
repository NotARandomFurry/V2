using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Sky
{
	public static class HarpyStuff
	{
		public static class Statistics
		{
			public static float MaxMoveSpeed
			{
				get
				{
					float baseMaxMoveSpeed = Main.GameMode switch
					{
						GameModeID.Master => 3.90f,
						GameModeID.Expert => 3.30f,
						GameModeID.Normal => 3.00f,
						_ => 2.00f
					};
					if (Main.zenithWorld)
						baseMaxMoveSpeed *= 1.1f;
					else if (Main.getGoodWorld)
						baseMaxMoveSpeed *= 1.05f;
					return baseMaxMoveSpeed;
				}
			}
			public static int DiveBombLength
			{
				get
				{
					int baseDiveBombLength = Main.GameMode switch
					{
						GameModeID.Master => V2Utils.SensibleTime(frames: 40),
						GameModeID.Expert => V2Utils.SensibleTime(frames: 50),
						GameModeID.Normal => V2Utils.SensibleTime(seconds: 1),
						_ => V2Utils.SensibleTime(seconds: 1)
					};
					if (Main.zenithWorld)
						baseDiveBombLength = (int)Math.Round((float)baseDiveBombLength * 0.60f);
					else if (Main.getGoodWorld)
						baseDiveBombLength = (int)Math.Round((float)baseDiveBombLength * 0.85f);
					return baseDiveBombLength;
				}
			}
			public static int DiveBombRecoveryLength
			{
				get
				{
					int baseDiveBombRecoveryLength = Main.GameMode switch
					{
						GameModeID.Master => V2Utils.SensibleTime(seconds: 2),
						GameModeID.Expert => V2Utils.SensibleTime(seconds: 3),
						GameModeID.Normal => V2Utils.SensibleTime(seconds: 4),
						_ => V2Utils.SensibleTime(seconds: 4)
					};
					if (Main.zenithWorld)
						baseDiveBombRecoveryLength = (int)Math.Round((float)baseDiveBombRecoveryLength * 0.70f);
					else if (Main.getGoodWorld)
						baseDiveBombRecoveryLength = (int)Math.Round((float)baseDiveBombRecoveryLength * 0.90f);
					return baseDiveBombRecoveryLength;
				}
			}
		}
		public static class ItemTheftRules
		{
			public static ItemTheftRule GiantHarpyFeather => new ItemTheftRule(
				type: (npc, pred) => ItemID.GiantHarpyFeather,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 1.0 / 30.0,
						GameModeID.Expert => 1.0 / 40.0,
						_ => 1.0 / 50.0,
					};
				}
			);
		}
		public static Harpy AsHarpy(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Harpy harpy))
				throw new Exception("this instance of a Harpy, supposedly, doesn't exist");

			return harpy;
		}
	}

	public partial class Harpy : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Harpy;

		public int WingFlapTimer { get; set; }
		public int DirectionChangeTimer { get; set; }

		public float BellyWeightFlightMovementModifier { get; set; }

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.lifeMax = 100;

			npc.aiStyle = -1;
			npc.AsV2NPC().NewAIMethod = V2HarpyAI;
			npc.AsV2NPC().TargetRange = V2Utils.TileCountAsPixelCount(70.0);
			npc.AsV2NPC().TargetRequiresLineOfSight = true;

			npc.AsFood().DefinedBaseSize = 1.335;
			npc.AsPred().MaxStomachCapacity = 1.9;
			npc.AsPred().BaseStomachacheMeterCapacity = 250.0;
			npc.AsPred().WeightGainRatio = 0.15;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.35;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(4.7);
			npc.AsPred().CanBeForceFed = CanHarpyBeForceFed;
			npc.AsPred().OnForceFed = OnHarpyForceFed;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
			npc.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 0.35;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantHarpyGoal;

			npc.AsFood().ItemTheftRules = [
				HarpyStuff.ItemTheftRules.GiantHarpyFeather,
			];
		}

		public override void OnSpawn(NPC npc, IEntitySource source)
		{
			npc.direction = Main.rand.NextBool().ToDirectionInt();
			npc.position.Y -= 12;
			npc.target = -1;
			npc.AsHarpy().WingFlapTimer = Main.rand.Next(-4, 70 + 1);
			npc.AsHarpy().DirectionChangeTimer = -Main.rand.Next(V2Utils.SensibleTime(seconds: 2), V2Utils.SensibleTime(seconds: 4) + 1);
			npc.AsV2NPC().BehaviorPattern = new HarpyAI.MainFlying();

			WeightedRandom<double> preSetWeight = new WeightedRandom<double>(Main.rand);
			preSetWeight.Add(0.0, 13);
			preSetWeight.Add(0.1, 6);
			preSetWeight.Add(0.5, 1);
			npc.AsPred().ExtraWeight = preSetWeight;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			AdjustHarpyFatteningStats(npc);
		}

		public static bool V2HarpyAI(NPC npc)
		{
			npc.noGravity = true;
			npc.ai[3]++;
			if (npc.ai[3] > 15) npc.ai[3] = 0;
			Entity targetEntity = null;
			if (npc.AsV2NPC().TargetIndex == -1)
				npc.TryFindNewTarget(Diet);
			else
				npc.TryVerifyRemainingTarget(Diet);
			if (npc.AsV2NPC().TargetIndex != -1)
			{
				targetEntity = npc.AsV2NPC().TargetType switch
				{
					TargetType.Player => Main.player[npc.AsV2NPC().TargetIndex],
					TargetType.NPC => Main.npc[npc.AsV2NPC().TargetIndex],
					TargetType.Projectile => Main.projectile[npc.AsV2NPC().TargetIndex],
					_ => null,
				};
			}

			if (npc.AsV2NPC().BehaviorPattern is not null)
				npc.AsV2NPC().BehaviorPattern.DoBehavior(npc, targetEntity);
			else
				npc.SwitchToPattern<HarpyAI.MainFlying>(targetEntity);

			return false;
		}

		public static void AdjustHarpyFatteningStats(NPC npc)
		{
			npc.lifeMax = 100;
			npc.defense = 8;
			npc.AsPred().MaxStomachCapacity = 1.9;
			npc.AsPred().BaseStomachacheMeterCapacity = 120.0;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(6.0);
			if (Main.GameModeInfo.IsExpertMode)
			{
				npc.lifeMax = 200;
				npc.defense = 10;
				npc.AsPred().BaseStomachacheMeterCapacity = 240.0;
			}
			if (Main.GameModeInfo.IsMasterMode)
			{
				npc.lifeMax = 300;
				npc.defense = 12;
				npc.AsPred().BaseStomachacheMeterCapacity = 360.0;
			}

			npc.lifeMax = (int)Math.Round((double)npc.lifeMax * Math.Pow(1.2, GetVisualWeightStage(npc)));
			npc.defense = (int)Math.Round((double)npc.defense * Math.Pow(1.2, GetVisualWeightStage(npc)));
			npc.lifeRegen = (int)Math.Round((double)2.0 * Math.Pow(1.2, GetVisualWeightStage(npc)));
			npc.AsPred().MaxStomachCapacity = Math.Round(1000.0 * 1.9 * Math.Pow(1.2, GetVisualWeightStage(npc))) / 1000.0;
			npc.AsPred().BaseStomachacheMeterCapacity = Math.Round(npc.AsPred().BaseStomachacheMeterCapacity * Math.Pow(1.2, GetVisualWeightStage(npc)));
		}

		public static bool CanHarpyBeForceFed(NPC npc) => true;

		public static void OnHarpyForceFed(NPC npc, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange([
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.5",
			]);
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 1.25;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			return Main.GameMode switch
			{
				GameModeID.Creative => 20 * CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>().StrengthMultiplierToGiveNPCs,
				GameModeID.Master => 30,
				GameModeID.Expert => 25,
				_ => 20,
			};
		}

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{

		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 2,
				seconds: 30
			);
			baseAbsorptionRate *= Math.Pow(1.2, GetVisualWeightStage(npc));
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				6
			);
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
			npc.frame.X = 90 * GetVisualBellySize(npc);

			if (npc.AsV2NPC().BehaviorPattern is HarpyAI.DiveBombing)
			{
				npc.frame.Y = 500;
			}
			else
			{
				if (npc.AsV2NPC().BehaviorPattern is HarpyAI.ChargingDiveBomb)
				{
					Entity target = npc.AsV2NPC().TargetType switch
					{
						TargetType.Player => Main.player[npc.AsV2NPC().TargetIndex],
						TargetType.NPC => Main.npc[npc.AsV2NPC().TargetIndex],
						TargetType.Projectile => Main.projectile[npc.AsV2NPC().TargetIndex],
						_ => null,
					};
					npc.spriteDirection = npc.direction = (target.position.X >= npc.TrueCenter().X).ToDirectionInt();
				}

				npc.frame.Y = WingFlapTimer switch
				{
					int i when i < 0 => 200,
					int i when i >= 0 && i < 5 => 300,
					int i when i >= 5 && i < 10 => 400,
					_ => 0,
				};
				if (npc.frame.Y == 0 && npc.ai[3] > 7) npc.frame.Y = 100;
			}
		}

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(Math.Sqrt(8) * Math.Sqrt(npc.AsPred().ExtraWeight)),
				4
			);
		}

		public static void OnKilledByDigestion_GrantHarpyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatHarpy>().TrySetCompletion(predPlayer);
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.CurrentCaptor() is not null)
				return false;
			if (npc.AsV2NPC().BehaviorPattern is HarpyAI.ChargingDiveBomb)
			{
				Entity target = npc.AsV2NPC().TargetType switch
				{
					TargetType.Player => Main.player[npc.AsV2NPC().TargetIndex],
					TargetType.NPC => Main.npc[npc.AsV2NPC().TargetIndex],
					TargetType.Projectile => Main.projectile[npc.AsV2NPC().TargetIndex],
					_ => null,
				};
				npc.spriteDirection = npc.direction = (target.position.X >= npc.TrueCenter().X).ToDirectionInt();
			}

			Vector2 Offset = new Vector2(-24, -12);
			if (npc.direction == 1) Offset = new Vector2(-36, -12);

			SpriteEffects spriteEffects = npc.direction != 1 ? 0 : SpriteEffects.FlipHorizontally;

			int weightStage = npc.AsPred().GetVisualWeightStage.Invoke(npc);
			/*string weightString = "_Weight" + (weightStage == 0 ? "Base" : weightStage);
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);*/
			Rectangle sourceRect = new Rectangle(npc.frame.X, npc.frame.Y, 90, 100);
			Texture2D sprite = ModContent.Request<Texture2D>("V2/NPCs/Vanilla/Sky/Harpy_Weight" + weightStage).Value;
			spriteBatch.Draw(sprite, npc.position - Main.screenPosition, sourceRect, drawColor, npc.rotation, -Offset, 1f, spriteEffects, 0f);

			//string exactMainBodyTexture = "V2/NPCs/Vanilla/Sky/Harpy_Weight" + weightStage;
			//TextureAssets.Npc[NPCID.Harpy] = ModContent.Request<Texture2D>(exactMainBodyTexture, AssetRequestMode.ImmediateLoad);
			return false;
		}

		public override void SaveData(NPC npc, TagCompound tag)
		{
			tag["BehaviorPattern"] = "Main Flying";
		}

		public override void LoadData(NPC npc, TagCompound tag)
		{
			npc.AsV2NPC().BehaviorPattern = new HarpyAI.MainFlying();
		}
	}
}

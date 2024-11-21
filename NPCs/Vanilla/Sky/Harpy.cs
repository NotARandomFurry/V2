using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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

			npc.aiStyle = -1;
			npc.AsV2NPC().NewAIMethod = V2HarpyAI;
			npc.AsV2NPC().TargetRange = V2Utils.TileCountAsPixelCount(44.0);
			npc.AsV2NPC().TargetRequiresLineOfSight = true;

			npc.AsFood().DefinedBaseSize = 1.335;
			npc.AsPred().MaxStomachCapacity = 1.75;
			npc.AsPred().BaseStomachacheMeterCapacity = 250.0;

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

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnDigestedBy += PreyNPC.HandlePreyItemTheft;
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
			npc.AsV2NPC().TargetType = TargetType.None;
			npc.AsHarpy().WingFlapTimer = Main.rand.Next(-4, 70 + 1);
			npc.AsHarpy().DirectionChangeTimer = -Main.rand.Next(V2Utils.SensibleTime(seconds: 2), V2Utils.SensibleTime(seconds: 4) + 1);
			npc.AsV2NPC().BehaviorPattern = new HarpyAI.MainFlying();

			WeightedRandom<double> preSetWeight = new WeightedRandom<double>(Main.rand);
			preSetWeight.Add(0.0, 13);
			preSetWeight.Add(0.1, 6);
			preSetWeight.Add(0.5, 1);
			npc.AsPred().ExtraWeight = preSetWeight;
		}

		public static bool V2HarpyAI(NPC npc)
		{
			npc.noGravity = true;

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
				npc.SwitchToPattern<HarpyAI.MainFlying>(targetEntity);

			return false;
		}

		public static bool CanHarpyBeForceFed(NPC npc) => true;

		public static void OnHarpyForceFed(NPC npc, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.5",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Sky.Harpy.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 1.25;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 18;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{

		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 3,
				seconds: 0
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				5
			);
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
			if (npc.AsV2NPC().BehaviorPattern is HarpyAI.DiveBombing)
			{
				npc.frame.Y = 0;
			}
			else
			{
				if (npc.AsV2NPC().BehaviorPattern is HarpyAI.ChargingDiveBomb)
				{
					Entity target = npc.AsV2NPC().TargetType switch
					{
						TargetType.Player => Main.player[npc.target],
						TargetType.NPC => Main.npc[npc.target],
						TargetType.Projectile => Main.projectile[npc.target],
						_ => null,
					};
					npc.spriteDirection = npc.direction = (target.position.X >= npc.TrueCenter().X).ToDirectionInt();
				}

				npc.frame.Y = WingFlapTimer switch
				{
					int i when i < 0 => 1 * 86,
					int i when i >= 0 && i < 3 => 2 * 86,
					int i when i >= 3 && i < 15 => 3 * 86,
					int i when i >= 15 && i < 20 => 4 * 86,
					int i when i >= 20 && i < 30 => 5 * 86,
					_ => 0 * 86,
				};
			}
		}

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(Math.Sqrt(8) * Math.Sqrt(npc.AsPred().ExtraWeight)),
				2
			);
		}

		public static void OnKilledByDigestion_GrantHarpyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatHarpy>().TrySetCompletion(predPlayer);
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.AsV2NPC().BehaviorPattern is HarpyAI.ChargingDiveBomb)
			{
				Entity target = npc.AsV2NPC().TargetType switch
				{
					TargetType.Player => Main.player[npc.target],
					TargetType.NPC => Main.npc[npc.target],
					TargetType.Projectile => Main.projectile[npc.target],
					_ => null,
				};
				npc.spriteDirection = npc.direction = (target.position.X >= npc.TrueCenter().X).ToDirectionInt();
			}

			int weightStage = npc.AsPred().GetVisualWeightStage.Invoke(npc);
			string weightString = "_Weight" + (weightStage == 0 ? "Base" : weightStage);
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);

			string exactMainBodyTexture = "V2/NPCs/Vanilla/Sky/Harpy" + weightString + bellyString;
			TextureAssets.Npc[NPCID.Harpy] = ModContent.Request<Texture2D>(exactMainBodyTexture, AssetRequestMode.ImmediateLoad);
			return true;
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

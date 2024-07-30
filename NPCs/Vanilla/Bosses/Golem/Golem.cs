using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Vanilla.TownNPCs.PartyGirl;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Bosses.Golem
{
	public static class GolemStuff
	{
		public static class ItemTheftRules
		{
			public static ItemTheftRule WeaponDrops => new ItemTheftRule(
				type: (npc, pred) => {
					List<int> weapons = new List<int>()
					{
						ItemID.PiercingStarlight,
						ItemID.FairyQueenRangedItem,
						ItemID.FairyQueenMagicItem,
						ItemID.RainbowWhip
					};
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
		public static Golem AsGolem(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Golem unreasonablyThickFairy))
				throw new Exception("this instance of the Golem can't be pred or prey. big rock candy feast has to wait 'til later");

			return unreasonablyThickFairy;
		}
	}

	public class Golem : GlobalNPC
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

			npc.AsFood().DefinedBaseSize = 41.4;
			npc.AsPred().MaxStomachCapacity = 200.0;
			npc.AsPred().BaseStomachacheMeterCapacity = 5000.0;

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

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
			npc.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			npc.AsGolem().MuffledScreechDelay = 0;

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnDigestedBy += PreyNPC.HandlePreyItemTheft;
			npc.AsFood().DigestedDeathSound = GolemStuff.MuffledGolemDeathScreech;

			npc.AsFood().ItemTheftRules = new List<ItemTheftRule>
			{
				GolemStuff.ItemTheftRules.WeaponDrops,
				GolemStuff.ItemTheftRules.StarGuitar,
				GolemStuff.ItemTheftRules.EmpressWings,
				GolemStuff.ItemTheftRules.PrismaticDye,
				GolemStuff.ItemTheftRules.Mask,
				GolemStuff.ItemTheftRules.Trophy,
				GolemStuff.ItemTheftRules.ExpertDrop,
				GolemStuff.ItemTheftRules.MasterTrophy,
				GolemStuff.ItemTheftRules.MasterPetItem,
				GolemStuff.ItemTheftRules.HangrySwordDrop,
			};
		}

		public override void PostAI(NPC npc)
		{
			if (npc.ai[0] is 8f or 9f)
				npc.DoContactGulpage();
		}

		public static bool CanUnreasonablyThickFairyBeForceFed(NPC npc) => true;

		public static void OnUnreasonablyThickFairyForceFed(NPC npc, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.4",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.Hardcore");
			}
		}

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => Main.dayTime ? 1000.0 : 120.0;
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
				seconds: 30
			);
			if (Main.dayTime)
				baseAbsorptionRate *= 10.0;
			if (npc.AI_120_HallowBoss_IsInPhase2())
				baseAbsorptionRate *= 1.75;
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(1.75 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				6
			);
		}

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(0.20 * Math.Sqrt(npc.AsPred().ExtraWeight)),
				2
			);
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			int weightStage = npc.AsPred().GetVisualWeightStage.Invoke(npc);
			string weightString = "_Weight" + (weightStage == 0 ? "Base" : weightStage);
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);

			string exactMainBodyTexture = "V2/NPCs/Vanilla/Bosses/EmpressOfLight/EmpressOfLight_MainBody" + weightString + bellyString;
			TextureAssets.Npc[NPCID.HallowBoss] = ModContent.Request<Texture2D>(exactMainBodyTexture, AssetRequestMode.ImmediateLoad);
			TextureAssets.Extra[ExtrasID.HallowBossSkirt] = ModContent.Request<Texture2D>("V2/NPCs/Vanilla/Bosses/EmpressOfLight/EmpressOfLight_SkirtOverlay", AssetRequestMode.ImmediateLoad);
			return true;
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			TextureAssets.Npc[NPCID.HallowBoss] = ModContent.Request<Texture2D>("Terraria/Images/NPC_" + NPCID.HallowBoss, AssetRequestMode.ImmediateLoad);
		}

		public static bool V2UnreasonablyThickFairyAI(NPC npc)
		{
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
			bool muffledMusicPlaying = SoundEngine.TryGetActiveSound(npc.AsGolem().MuffledMusic, out ActiveSound muffledMusic);
			if (!muffledMusicPlaying)
			{
				npc.AsGolem().MuffledMusic = SoundEngine.PlaySound(
					GolemStuff.MuffledGolemMusic,
					pred.TrueCenter()
				);
				SoundEngine.TryGetActiveSound(npc.AsGolem().MuffledMusic, out muffledMusic);
			}

			if (muffledMusic is null)
				return;

			muffledMusic.Position = pred.TrueCenter();
			muffledMusic.Volume = (float)npc.life / (float)npc.lifeMax;

			npc.AsGolem().MuffledScreechDelay -= 1;
			if (npc.AsGolem().MuffledScreechDelay == 0 && Main.rand.NextBool(200))
			{
				npc.AsGolem().MuffledScreechDelay = MuffledScreechMinDelay;
				SoundEngine.PlaySound(
					(
						Main.rand.NextBool()
						  ? GolemStuff.MuffledGolemScreech1
						  : GolemStuff.MuffledGolemScreech2
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

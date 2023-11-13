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

namespace V2.NPCs.Vanilla.Bosses.EmpressOfLight
{
	public static class CandyFairyStuff
	{
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

		public override void SetDefaults(NPC entity)
		{
			entity.AsV2NPC().Gender = EntityGender.Female;

			entity.AsFood().Size = 41.4;
			entity.AsPred().MaxStomachCapacity = 200.0;
			entity.AsPred().BaseStomachacheMeterCapacity = 5000.0;

			entity.AsPred().CanBeForceFed = CanUnreasonablyThickFairyBeForceFed;
			entity.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(12.5);
			entity.AsPred().SmallGulpThreshold = 3.75;

			entity.AsPred().DigestionType = EntityDigestionType.Acidic;
			entity.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			entity.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			entity.AsPred().SmallBurps = Burps.Humanoid.Small;
			entity.AsPred().StandardBurps = Burps.Humanoid.Standard;
			entity.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			entity.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			entity.AsPred().GetVisualBellySize = GetVisualBellySize;
			entity.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			entity.AsPred().SpecialPredAI = UnreasonablyThickFairyPredAI;
			entity.AsFood().SpecialPreyAI = UnreasonablyThickFairyPreyAI;

			entity.AsFood().OnKilledByDigestion += PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			entity.AsFood().DigestedDeathSound = CandyFairyStuff.MuffledCandyFairyDeathScreech;

			entity.AsCandyFairy().MuffledScreechDelay = 0;
		}

		public override void PostAI(NPC npc)
		{
			if (npc.ai[0] is 8f or 9f)
				npc.DoContactGulpage();
		}

		public static bool CanUnreasonablyThickFairyBeForceFed(NPC npc) => true;

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
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => Main.dayTime ? 1000.0 : 120.0;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			SoundEngine.PlaySound(
				digestedPrey.WeightLeftToDigest < npc.AsPred().SmallGulpThreshold ? npc.AsPred().SmallBurps : npc.AsPred().StandardBurps,
				npc.TrueCenter() + new Vector2(0f, -50f)
			);
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

		public static bool UnreasonablyThickFairyPredAI(NPC npc)
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
					}
				);
			}
		}
	}
}

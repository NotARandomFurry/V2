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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Vanilla.TownNPCs.PartyGirl;
using V2.PlayerHandling;
using V2.Sounds.Vore;
using static V2.Core.FoodTypeTags;

namespace V2.NPCs.Vanilla.Bosses.EmpressOfLight
{
	public static class UnreasonablyThickFairyStuff
	{
		public static UnreasonablyThickFairy AsUnreasonablyThickFairy(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out UnreasonablyThickFairy unreasonablyThickFairy))
				throw new Exception("this instance of the Empress of Light, sadly, can't be pred or prey. the unreasonably thick fairy can't be food today, I guess");

			return unreasonablyThickFairy;
		}

		public static SoundStyle MuffledFoodFairyMusic = new SoundStyle("V2/Sounds/MuffledMusic/EmpressOfLight", SoundType.Sound) with { MaxInstances = 0 };

		public static SoundStyle MuffledFoodFairyScreech1 = new SoundStyle("V2/Sounds/MuffledSounds/Item160", SoundType.Sound) with { MaxInstances = 0 };
		public static SoundStyle MuffledFoodFairyScreech2 = new SoundStyle("V2/Sounds/MuffledSounds/Item161", SoundType.Sound) with { MaxInstances = 0 };
		public static SoundStyle MuffledFoodFairyDeathScreech = new SoundStyle("V2/Sounds/MuffledSounds/NPC_Killed_65", SoundType.Sound) with { MaxInstances = 0 };
	}

	public class UnreasonablyThickFairy : GlobalNPC
	{
		public int MuffledScreechMinDelay => V2Utils.SensibleTime(seconds: 4);
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

			npc.AsPred().stomachContents = new List<Prey>();
			npc.AsPred().stomachContentsQueue = new List<Prey>();
			npc.AsPred().maxStomachCapacity = 100.0;

			npc.AsPred().CanBeForceFedMethod = CanUnreasonablyThickFairyBeForceFed;
			npc.AsPred().swallowRange = V2Utils.TileCountAsPixelCount(12.5);
			npc.AsPred().SmallGulpThreshold = 3.75;

			npc.AsPred().GetDigestionTickDamageMethod = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRateMethod = GetDigestionTickRate;

			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetDigestedPlayerAdditionalDeathMessagesMethod = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetVisualBellySizeMethod = GetVisualBellySize;

			npc.AsFood().PreyAIMethod = UnreasonablyThickFairyPreyAI;

			npc.AsFood().DigestedDeathSound = UnreasonablyThickFairyStuff.MuffledFoodFairyDeathScreech;

			npc.AsFood().FoodTypeTags = new List<FoodTypeTag>()
			{
				new MeatTag()
				{
					FoodSubtypeTags = new List<(string subtype, double weight)>
					{
						("Human", 35.0),
						("Insect", 5.0)
					}
				}
			};

			npc.AsUnreasonablyThickFairy().MuffledScreechDelay = 0;
		}

		public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
		{
			if (npc.ai[0] == 8f)
			{
				if (!npc.AsFood().IsCurrentlyEaten && npc.Hitbox.Intersects(target.Hitbox) && PredNPC.CanSwallow(npc, target))
				{
					PredNPC.Swallow(npc, target);
					return false;
				}
			}
			return true;
		}

		public override bool CanHitNPC(NPC npc, NPC target)
		{
			if (target.type == NPCID.PartyGirl)
			{
				if (!npc.AsFood().IsCurrentlyEaten && npc.Hitbox.Intersects(target.Hitbox) && target.AsPartyGirl().HungerForEmpress == PartyGirl.MaxHungerForEmpress && target.AsPred().stomachContents.Count == 0)
				{
					PredNPC.Swallow(target, npc);
					target.position.X += 14;
					target.position.Y += 40;
					PartyGirl.PartyGirlSpecialPredAI(target);
					target.position.X -= 110;
					target.position.Y -= 68;
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						Projectile projectile = Main.projectile[i];
						if (!projectile.active)
							continue;

						if (projectile.type is ProjectileID.HallowBossSplitShotCore
											or ProjectileID.HallowBossRainbowStreak
											or ProjectileID.HallowBossLastingRainbow
											or ProjectileID.FairyQueenHymn
											or ProjectileID.FairyQueenLance
											or ProjectileID.FairyQueenSunDance)
							projectile.Kill();
					}
					return false;
				}
			}

			if (npc.ai[0] == 8f)
			{
				if (!npc.AsFood().IsCurrentlyEaten && npc.Hitbox.Intersects(target.Hitbox) && PredNPC.CanSwallow(npc, target))
				{
					PredNPC.Swallow(npc, target);
					return false;
				}
			}
			return true;
		}

		public static bool CanUnreasonablyThickFairyBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.1",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.2",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.3",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.3",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Bosses.UnreasonablyThickFairy.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, Prey prey) => Main.dayTime ? 12.0 : Main.bloodMoon ? 6.0 : 3.0;
		public static double GetDigestionTickDamage(NPC npc, Prey prey) => Main.dayTime ? 100.0 : 38.0;

		public static void OnDigestionKill(NPC npc, Prey digestedPrey)
		{
			SoundEngine.PlaySound(
				digestedPrey.WeightLeftToDigest < 3.75 ? npc.AsPred().SmallBurps : npc.AsPred().StandardBurps,
				npc.TrueCenter() + new Vector2(0f, -50f)
			);
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 2,
				seconds: 0
			);
			if (npc.AI_120_HallowBoss_IsGenuinelyEnraged())
				return baseAbsorptionRate * 3.0;
			else if (npc.AI_120_HallowBoss_IsInPhase2())
				return baseAbsorptionRate * 1.5;
			else
				return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(1.15 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				6
			);
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			string exactTextureToUse = "V2/NPCs/Vanilla/Bosses/EmpressOfLight/EmpressOfLight_MainBody";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySizeMethod.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			TextureAssets.Npc[NPCID.HallowBoss] = ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
			return true;
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			TextureAssets.Npc[NPCID.HallowBoss] = ModContent.Request<Texture2D>("Terraria/Images/NPC_" + NPCID.HallowBoss, AssetRequestMode.ImmediateLoad);
		}

		public static bool UnreasonablyThickFairyPredAI(NPC npc)
		{
			if (npc.target != -1 || !Main.player[npc.target].IsFoodFor(npc, out bool pastTense) || pastTense)
				return true;

			npc.velocity *= 0.90f;
			return false;
		}

		public static void UnreasonablyThickFairyPreyAI(NPC npc, Entity pred)
		{
			bool muffledMusicPlaying = SoundEngine.TryGetActiveSound(npc.AsUnreasonablyThickFairy().MuffledMusic, out ActiveSound muffledMusic);
			if (!muffledMusicPlaying)
			{
				npc.AsUnreasonablyThickFairy().MuffledMusic = SoundEngine.PlaySound(
					UnreasonablyThickFairyStuff.MuffledFoodFairyMusic,
					pred.TrueCenter()
				);
				SoundEngine.TryGetActiveSound(npc.AsUnreasonablyThickFairy().MuffledMusic, out muffledMusic);
			}

			if (muffledMusic is null)
				return;

			muffledMusic.Position = pred.TrueCenter();
			muffledMusic.Volume = (float)npc.life / (float)npc.lifeMax;

			npc.AsUnreasonablyThickFairy().MuffledScreechDelay -= 1;
			if (npc.AsUnreasonablyThickFairy().MuffledScreechDelay == 0 && Main.rand.NextBool(230))
			{
				npc.AsUnreasonablyThickFairy().MuffledScreechDelay = npc.AsUnreasonablyThickFairy().MuffledScreechMinDelay;
				SoundEngine.PlaySound(
					(
						Main.rand.NextBool()
						  ? UnreasonablyThickFairyStuff.MuffledFoodFairyScreech1
						  : UnreasonablyThickFairyStuff.MuffledFoodFairyScreech2
					)
					with
					{
						PitchVariance = 0.07f
					}
				);
			}
		}
	}
}

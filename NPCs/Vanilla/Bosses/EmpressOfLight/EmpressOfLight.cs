using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Vanilla.TownNPCs.PartyGirl;
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
		public int MuffledScreechMinDelay => V2Utils.WriteFrameCountAsANormalFuckingTimeMeasurement(seconds: 4);
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
			npc.AsPred().maxStomachCapacity = 40.0;

			npc.AsPrey().PreyAIMethod = UnreasonablyThickFairyPreyAI;

			npc.AsPrey().DigestedDeathSound = UnreasonablyThickFairyStuff.MuffledFoodFairyDeathScreech;

			npc.AsPrey().FoodTypeTags = new List<FoodTypeTag>()
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

		public override bool CanHitNPC(NPC npc, NPC target)
		{
			if (target.type == NPCID.PartyGirl)
			{
				if (!npc.AsPrey().IsCurrentlyEaten && npc.Hitbox.Intersects(target.Hitbox) && target.AsPartyGirl().HungerForEmpress == PartyGirl.MaxHungerForEmpress && target.AsPred().stomachContents.Count == 0)
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
			return true;
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.Projectiles.Vanilla.Summons.Pets
{
	public partial class FairyPrincess : GlobalProjectile
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.FairyQueenPet;

		public override void SetDefaults(Projectile projectile)
		{
			projectile.Name = Language.GetTextValue("Mods.V2.Projectiles.DisplayName.Vanilla.Summons.Pets.FairyPrincess");

			projectile.AsV2Proj().Gender = EntityGender.Female;
			projectile.AsV2Proj().NewAIMethod = MiniCandyFairyAI;

			projectile.AsPred().MaxStomachCapacity = 20.0;
			projectile.AsPred().BaseStomachacheMeterCapacity = 10000.0;
			projectile.AsPred().CanSwallowBosses = true;

			projectile.AsFood().DefinedSize = 0.742;
			projectile.AsFood().MaxHealth = 3500;
			projectile.AsFood().Health = 3500;

			projectile.AsPred().SmallGulps = Gulps.Short;
			projectile.AsPred().SmallGulpThreshold = 0.1;
			projectile.AsPred().BigGulps = Gulps.Standard;
			projectile.AsPred().CanBeForceFed = CanMiniCandyFairyBeForceFed;
			projectile.AsPred().OnForceFed = OnMiniCandyFairyForceFed;
			projectile.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(12.5);

			projectile.AsPred().DigestionType = EntityDigestionType.Acidic;
			projectile.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			projectile.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			projectile.AsPred().OnDigestionKill = OnDigestionKill;
			projectile.AsPred().SmallBurps = Burps.Humanoid.Small;
			projectile.AsPred().StandardBurps = Burps.Humanoid.Standard;
			projectile.AsPred().BurpPitchOffset = 0.285f;
			projectile.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			projectile.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			projectile.AsPred().GetVisualBellySize = GetVisualBellySize;
			projectile.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			projectile.AsFood().OnKilledByDigestion += PreyProjectile.OnKilledByDigestion_GrantLivePreyGoal;
			projectile.AsFood().OnKilledByDigestion += OnKilledByDigestion;
		}

		public static bool CanMiniCandyFairyBeForceFed(Projectile projectile) => true;

		public static void OnMiniCandyFairyForceFed(Projectile projectile, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(Projectile projectile, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.1",
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.2",
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.3",
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.4",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.Hardcore");
			}
		}

		public static double GetDigestionTickDamage(Projectile projectile, PreyData prey) => Main.dayTime ? 80.0 : (Main.bloodMoon ? 60.0 : 40.0);
		public static double GetDigestionTickRate(Projectile projectile, PreyData prey) => Main.dayTime ? 4.0 : (Main.bloodMoon ? 3.0 : 2.0);

		public static void OnDigestionKill(Projectile projectile, PreyData digestedPrey)
		{

		}

		public static double GetPreyAbsorptionRate(Projectile projectile)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 3,
				seconds: 0
			);
			if (Main.dayTime)
				baseAbsorptionRate *= 3.0;
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(6.5 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				8
			);
		}

		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(1.4 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				0
			);
		}

		public static void OnKilledByDigestion(Projectile projectile, Entity pred)
		{
			Player ownerPlayer = Main.player[projectile.owner];
			ownerPlayer.ClearBuff(BuffID.FairyQueenPet);
		}

		public override void OnKill(Projectile projectile, int timeLeft)
		{
			int particleCount = 10;
			float rotationPerParticle = MathHelper.ToRadians(360f / (float)particleCount);
			for (int i = 0; i < particleCount; i++)
			{
				int num974 = Dust.NewDust(
					projectile.position,
					projectile.width,
					projectile.height,
					Main.rand.NextFromCollection(new List<int> {
								DustID.PinkTorch,
								DustID.PinkTorch,
								DustID.BlueTorch,
								DustID.YellowTorch,
					}),
					0f,
					0f,
					50,
					default,
					2f
				);
				Main.dust[num974].noGravity = true;
			}
		}

		public override bool PreDraw(Projectile projectile, ref Color lightColor)
		{
			if (projectile.CurrentCaptor() is not null)
				return false;

			SpriteEffects spriteEffects = projectile.direction switch
			{
				-1 => SpriteEffects.FlipHorizontally,
				_ => SpriteEffects.None,
			};
			string exactTextureToUse = "V2/Projectiles/Vanilla/Summons/Pets/FairyPrincess";
			int weightStage = projectile.AsPred().GetVisualWeightStage.Invoke(projectile);
			string weightString = "_Weight" + (weightStage == 0 ? "Base" : weightStage);
			exactTextureToUse += weightString;
			int bellySize = projectile.AsPred().GetVisualBellySize.Invoke(projectile);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			Texture2D texture = ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad).Value;
			Rectangle sourceRect = new Rectangle(0, projectile.frame * 74, 64, 74);

			Main.EntitySpriteDraw(
				texture,
				projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
				sourceRect,
				lightColor,
				projectile.rotation,
				new Vector2(28f, 28f),
				1,
				spriteEffects,
				0f
			);
			return false;
		}
	}
}

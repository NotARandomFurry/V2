using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Consumables.Potions;
using V2.PlayerHandling;
using V2.Projectiles.Vanilla.Summons.Pets;
using V2.Sounds.Vore;

namespace V2.Projectiles.Voraria.Weapons.Summon
{
	public class ShroomFairyDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.noLight = false;
			dust.color = Color.FromNonPremultiplied(new Vector4(0.35f, 0.25f, 1f, 1f));
		}

		public override bool Update(Dust dust)
		{
			// Calls every frame the dust is active
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.15f;
			dust.scale *= 0.98f;
			dust.velocity *= 0.95f;
			float light = 0.6f * dust.scale;

			Lighting.AddLight(dust.position, new Vector3(0.35f * light, 0.25f * light, light));

			if (dust.scale < 0.15f)
			{
				dust.active = false;
			}

			return false; // Return false to prevent vanilla behavior.
		}
	}

	public class ShroomFairyBuff : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Summons.ShroomFairy.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Summons.ShroomFairy.Description");

		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}


		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			rare = ItemRarityID.Green;
			tip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Summons.ShroomFairy.Description",
				new
				{
					
				}
			);
		}
		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ShroomFairy>()] > 0)
			{
				player.buffTime[buffIndex] = 18000;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public static partial class ShroomFairyStuff
	{
		public static int MaxHealth => 500;
		public static double Size => 0.88;
		public static double MaxStomachCapacity => 666.0; //she stops moving on her own after enough capacity so whatever?
		public static double DigestDamage => 11.0;
		public static double DigestRate => 4.0;
		public static double AbsorbRate => 1.0 / (double)V2Utils.SensibleTime(
			minutes: 1,
			seconds: 30
		);
	}

	public class ShroomFairy : ModProjectile
	{
		public (Projectile, NPC) target = (null, null);
		
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 28;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.minionSlots = 1.5f;
			Projectile.penetrate = -1;

			Projectile.AsV2Proj().Gender = EntityGender.Female;

			Projectile.AsPred().MaxStomachCapacity = ShroomFairyStuff.MaxStomachCapacity;
			Projectile.AsPred().BaseStomachacheMeterCapacity = -1;
			Projectile.AsPred().CanSwallowBosses = false;

			Projectile.AsFood().DefinedSize = ShroomFairyStuff.Size;
			Projectile.AsFood().MaxHealth = ShroomFairyStuff.MaxHealth;
			Projectile.AsFood().Health = ShroomFairyStuff.MaxHealth;

			Projectile.AsPred().MouthSoundRawOffset = new Vector2(0f, -14f);
			Projectile.AsPred().SmallGulps = Gulps.Short;
			Projectile.AsPred().SmallGulpThreshold = 0.1;
			Projectile.AsPred().BigGulps = Gulps.Standard;
			Projectile.AsPred().CanBeForceFed = CanShroomFairyBeForceFed;
			Projectile.AsPred().OnForceFed = OnShroomFairyForceFed;
			Projectile.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(12.5);

			Projectile.AsPred().DigestionType = EntityDigestionType.Acidic;
			Projectile.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			Projectile.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			Projectile.AsPred().SmallBurps = Burps.Humanoid.Small;
			Projectile.AsPred().StandardBurps = Burps.Humanoid.Standard;
			Projectile.AsPred().BurpPitchOffset = 0.285f;

			Projectile.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			Projectile.AsPred().GetVisualBellySize = GetVisualBellySize;
			Projectile.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			Projectile.AsFood().OnKilledByDigestion += PreyProjectile.OnKilledByDigestion_GrantLivePreyGoal;
			Projectile.AsFood().OnKilledByDigestion += OnKilledByDigestion;
		}
		public static bool CanShroomFairyBeForceFed(Projectile projectile) => true;
		public static void OnShroomFairyForceFed(Projectile projectile, Player player)
		{

		}
		public static void OnKilledByDigestion(Projectile projectile, Entity pred)
		{
			Player ownerPlayer = Main.player[projectile.owner];
			if (ownerPlayer.ownedProjectileCounts[projectile.type] <= 1)
				ownerPlayer.ClearBuff(ModContent.BuffType<ShroomFairyBuff>());
		}
		public static int GetVisualBellySize(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(4.0 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				3 + GetVisualWeightStage(projectile)
			);
		}
		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(1.4 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				3
			);
		}

		public static double GetDigestionTickDamage(Projectile projectile, PreyData prey)
		{
			double digestDamage = ShroomFairyStuff.DigestDamage;
			if (projectile.ai[0] == 1) digestDamage *= 2;

			return digestDamage;
		}
		public static double GetDigestionTickRate(Projectile projectile, PreyData prey)
		{
			double digestRate = ShroomFairyStuff.DigestRate;
			if (projectile.ai[0] == 1) digestRate *= 2;
			Player ownerPlayer = Main.player[projectile.owner];
			if (!ownerPlayer.dead && ownerPlayer.sleeping.FullyFallenAsleep)
			{
				digestRate *= 1.25f;
				bool isEveryoneAsleep = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
				if (isEveryoneAsleep)
					digestRate *= (float)Main.dayRate;
			}

			return digestRate;
		}

		public static double GetPreyAbsorptionRate(Projectile projectile)
		{
			double absorbRate = ShroomFairyStuff.AbsorbRate;
			Player ownerPlayer = Main.player[projectile.owner];
			if (projectile.ai[0] == 1) absorbRate *= 2;
			if (!ownerPlayer.dead && ownerPlayer.sleeping.FullyFallenAsleep)
			{
				absorbRate *= 1.25f;
				bool isEveryoneAsleep = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
				if (isEveryoneAsleep)
					absorbRate *= (float)Main.dayRate;
			}
			return absorbRate;
		}
		public override bool? CanCutTiles()
		{
			return false;
		}
		public override bool MinionContactDamage()
		{
			return false;
		}
		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			VoreTracker tracker = PredProjectile.GetStomachTracker(Projectile);
			if (owner.IsFoodFor(Projectile, out bool pastTense))
				Projectile.timeLeft = 2;
			if (!CheckActive(owner))
			{
				WaitOut(owner);
				return;
			}
			if (GetVisualBellySize(Projectile) >= 3 + GetVisualWeightStage(Projectile))
			{
				WaitOut(owner);
				return;
			}
			Projectile.ai[0] = 0f;
			if (Projectile.ai[1] > 0f) Projectile.ai[1] -= 1f;
            target = (null, null);
			FindTarget(owner);
			if (target.Item1 != null && Projectile.ai[1] <= 0f)
			{
				CHARGE(owner, target.Item1, SetSpeedMulti());
			}
			else if (target.Item2 != null && Projectile.ai[1] <= 0f)
			{
				CHARGE(owner, target.Item2, SetSpeedMulti());
			}
			else Chill(owner, SetSpeedMulti());
		}
		public override void PostAI()
		{
			Projectile.rotation = Projectile.velocity.X * 0.04f;
			int framerate = 5;
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= framerate)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.7f);
		}
		public float SetSpeedMulti()
		{
			return 1f * Math.Max(1 - GetVisualWeightStage(Projectile) / 10f, 0.15f) * Math.Max(1 - GetVisualBellySize(Projectile) / 10f, 0.15f);
		}
		public void FindTarget(Player owner)
		{
			Projectile closestProj = null;
			NPC closestNPC = null;
			float projDistance = 99999f;
			float npcDistance = 99999f;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc == null || !npc.active) continue;
				if (npc.CurrentCaptor() is not null) continue;
				if (npc.type == NPCID.WaterSphere || npc.type == NPCID.ChaosBall || npc.type == NPCID.ChaosBallTim || npc.type == NPCID.BurningSphere || npc.type == NPCID.VileSpit || npc.type == NPCID.VileSpitEaterOfWorlds)
				{
					float distance = npc.position.Distance(owner.position);
					if (distance < npcDistance)
					{
						closestNPC = npc;
						npcDistance = distance;
					}
				}
			}
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile proj = Main.projectile[i];
				if (proj == null || !proj.active) continue;
				if (proj.CurrentCaptor() is not null) continue;
				if ((!proj.friendly || proj.hostile) && proj.damage > 0)
				{
					float distance = proj.position.Distance(owner.position);
					if (distance < projDistance)
					{
						closestProj = proj;
						projDistance = distance;
					}
				}
			}
			if (projDistance > 320) closestProj = null;
			if (npcDistance > 320) closestNPC = null;
			if (projDistance <= npcDistance) closestNPC = null;
			else closestProj = null;
			target = (closestProj, closestNPC);
		}
		public bool CheckActive(Player owner)
		{
			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(ModContent.BuffType<ShroomFairyBuff>());
				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<ShroomFairyBuff>()))
				Projectile.timeLeft = 2;

			return true;
		}
		public void Chill(Player owner, float SpeedMulti)
		{
			Vector2 vectorToIdlePosition;
			float distanceToIdlePosition;
			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 70f;
			float minionPositionOffsetX = (-40 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX;
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
			{
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}

			float overlapVelocity = 0.04f;

			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile other = Main.projectile[i];

				if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					if (Projectile.position.X < other.position.X)
						Projectile.velocity.X -= overlapVelocity;
					else
						Projectile.velocity.X += overlapVelocity;

					if (Projectile.position.Y < other.position.Y)
						Projectile.velocity.Y -= overlapVelocity;
					else
						Projectile.velocity.Y += overlapVelocity;
				}
			}

			float speed = 10f * SpeedMulti;
			float inertia = 20f;
			if (distanceToIdlePosition > 600f)
			{
				speed = 18f * SpeedMulti;
				inertia = 60f;
			}
			else if (distanceToIdlePosition < 80f)
			{
				speed = 4f * SpeedMulti;
				inertia = 80f;
			}

			if (distanceToIdlePosition > 20f)
			{
				vectorToIdlePosition.Normalize();
				vectorToIdlePosition *= speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
			}
			else if (Projectile.velocity == Vector2.Zero)
			{
				Projectile.velocity.X = -0.15f;
				Projectile.velocity.Y = -0.05f;
			}
			Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -10, 10);
			Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -10, 10);
		}
		public void CHARGE(Player owner, Entity target, float SpeedMulti)
		{
			if (!target.active)
				return;

			float speed = 25f * SpeedMulti;
			float inertia = 25f;
			Vector2 direction = Projectile.position.DirectionTo(target.position);
			direction.Normalize();
			Vector2 direction2 = direction * 4;
			float distance = Projectile.position.Distance(target.position);
			if (distance <= 180)
			{
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);

				Dust.NewDustPerfect(Projectile.Center - new Vector2(8, 8), ModContent.DustType<ShroomFairyDust>(), new Vector2(direction2.X * 3f, direction2.Y * 3f), 0, default, 6f);

				Projectile.ai[1] = 8f / SpeedMulti;
                Projectile.position = target.position;
				PredProjectile.Swallow(Projectile, target);

				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y);
			}
			else
			{
				direction *= speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
				Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -18, 18);
				Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -18, 18);
			}
		}
		public void WaitOut(Player owner)
		{
			Projectile.ai[0] = (owner.IsFoodFor(Projectile, out bool pastTense) && !pastTense) ? 2f : 1f;
			Projectile.velocity *= 0.9f;
		}
		public override bool PreDraw(ref Color lightColor)
		{
			string text = "V2/Projectiles/Voraria/Weapons/Summon/ShroomFairy";
			int TumSize = GetVisualBellySize(Projectile);
			int FairySize = GetVisualWeightStage(Projectile);
			int frameSize = 60;
			switch (FairySize)
			{
				case 0:
					break;
				case 1 or 2:
					text += "_" + FairySize;
					frameSize = 80;
					break;
                case 3:
                    text += "_" + FairySize;
                    frameSize = 96;
                    break;
            }
			int ExtraOffset = 0;
			if (Projectile.direction == -1) ExtraOffset = frameSize / 2;
            Texture2D sprite = ModContent.Request<Texture2D>(text).Value;
			SpriteEffects val = Projectile.direction != -1 ? 0 : (SpriteEffects)1;
			SpriteEffects spriteEffects = val;
			Rectangle sourceRect = new Rectangle(frameSize * TumSize, frameSize * Projectile.frame, frameSize, frameSize);
			Main.EntitySpriteDraw(sprite, Projectile.Center - Main.screenPosition + new Vector2(-frameSize / 2 - ExtraOffset, -8 + Projectile.gfxOffY), (Rectangle)sourceRect, lightColor, Projectile.rotation, Vector2.Zero, 1f, spriteEffects, 0f);
            Texture2D sprite2 = ModContent.Request<Texture2D>(text + "_Fullbright").Value;
            Main.EntitySpriteDraw(sprite2, Projectile.Center - Main.screenPosition + new Vector2(-frameSize / 2 - ExtraOffset, -8 + Projectile.gfxOffY), (Rectangle)sourceRect, new Color(255,255,255), Projectile.rotation, Vector2.Zero, 1f, spriteEffects, 0f);
            return false;
		}
	}
}

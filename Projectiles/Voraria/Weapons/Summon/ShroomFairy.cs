using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using V2.Core;
using V2.Items.Voraria;
using V2.PlayerHandling;
using V2.Items.Voraria.Consumables.Potions;
using V2.Projectiles.Vanilla.Summons.Pets;
using V2.Projectiles.Voraria.Pets;
using V2.NPCs;
using V2.Projectiles.Voraria.Weapons.Ranged.Throwables;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Projectiles.Voraria.Weapons.Summon
{
	public class ShroomFairyDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 8, 6);
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied(255, 255, 255, 255 - dust.alpha), dust.rotation, new Vector2(4, 3), dust.scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.rotation = dust.velocity.ToRotation();
			dust.scale *= 0.98f;
			dust.velocity *= 0.95f;
			float light = dust.scale;

			Lighting.AddLight(dust.position, new Vector3(0.35f * light, 0.25f * light, light));

			if (dust.scale < 0.15f)
			{
				dust.active = false;
			}

			return false;
		}
	}
	public class ShroomFairyDust2 : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.noLight = false;
			dust.frame = new Rectangle(0, 0, 18, 18);
			dust.customData = 1f;
			dust.alpha = 0;
			if (Main.rand.NextBool(2)) dust.customData = -1f;
			dust.customData = (float)dust.customData * 0.95f;
			float light = 0.01f * (255 - dust.alpha);

			Lighting.AddLight(dust.position, new Vector3(0.3f * light, 0.4f * light, light));

			if (dust.alpha >= 255)
			{
				dust.active = false;
			}
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
			Main.persistentBuff[Type] = true;
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
		public static double Stomachache => 475.0;
		public static double DigestDamage => 11.0;
		public static double DigestRate => 1;
		public static double AbsorbRate => 1.0 / (double)V2Utils.SensibleTime(
			minutes: 1,
			seconds: 30
		);

	}

	public class ShroomFairy : ModProjectile
	{
		public (Projectile, NPC, Point) target = (null, null, Point.Zero);
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 52;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.minionSlots = 1.5f;
			Projectile.penetrate = -1;

			Projectile.AsV2Proj().Gender = EntityGender.Female;

			Projectile.AsPred().MaxStomachCapacity = ShroomFairyStuff.MaxStomachCapacity;
			Projectile.AsPred().BaseStomachacheMeterCapacity = ShroomFairyStuff.Stomachache;
			Projectile.AsPred().CanSwallowBosses = false;

			Projectile.AsFood().DefinedSize = ShroomFairyStuff.Size;
			Projectile.AsFood().MaxHealth = ShroomFairyStuff.MaxHealth;
			Projectile.AsFood().Health = ShroomFairyStuff.MaxHealth;
            Projectile.AsFood().CalorieMultiplier = 1.5;

            Projectile.AsFood().TakeDamageFromSwallow = false;

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
			Projectile.AsPred().BurpPitchOffset = 0.4f;

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
				(int)Math.Floor(3.0 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				3 + GetVisualWeightStage(projectile)
			);
		}
		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(1.4 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				8
			);
		}

		public static double GetDigestionTickDamage(Projectile projectile, PreyData prey)
		{
			double digestDamage = ShroomFairyStuff.DigestDamage;
			if (IsFairyAtMaxCapacity(projectile)) digestDamage *= 2;

			return digestDamage;
		}
		public static double GetDigestionTickRate(Projectile projectile, PreyData prey)
		{
			double digestRate = ShroomFairyStuff.DigestRate;
			if (IsFairyAtMaxCapacity(projectile)) digestRate *= 2;
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
			double absorbRate = ShroomFairyStuff.AbsorbRate * (1 + GetVisualWeightStage(projectile) / (double)1.5);
			Player ownerPlayer = Main.player[projectile.owner];
			if (IsFairyAtMaxCapacity(projectile)) absorbRate *= 2;
			if (!ownerPlayer.dead && ownerPlayer.sleeping.FullyFallenAsleep)
			{
				absorbRate *= 1.75f;
				bool isEveryoneAsleep = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
				if (isEveryoneAsleep)
					absorbRate *= (float)Main.dayRate;
			}
			if (ownerPlayer.AsV2Player().ShroomNecklace)
				absorbRate *= 1.35f;
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
		public override void OnSpawn(IEntitySource source)
		{
			DustEffect(Projectile);
        }
        public static void DustEffect(Projectile projectile)
		{
            Dust.NewDustDirect(projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), 0, 2f);
            Dust.NewDustDirect(projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), 1.5f, 1.5f);
            Dust.NewDustDirect(projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), 2f, 0);
            Dust.NewDustDirect(projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), 1.5f, -1.5f);
            Dust.NewDustDirect(projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), 0, -2f);
            Dust.NewDustDirect(projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), -1.5f, -1.5f);
            Dust.NewDustDirect(projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), -2f, 0);
            Dust.NewDustDirect(projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), -1.5f, 1.5f);
            Dust.NewDustPerfect(projectile.Center, ModContent.DustType<ShroomFairyDust2>(), Vector2.Zero);
        }
		public static Vector2 AnglePosition(int Count, int TotalCount)
		{
			Vector2 Angle = new Vector2(0, -1);
			if (TotalCount > 1)
			{
				float multiplier = (float)Count / (float)(TotalCount - 1);
                Angle = new Vector2(-1, 0);
				Angle = Angle.RotatedBy((180f * multiplier) * Math.PI / 180f);
			}
			return Angle;
		}
		public static void GetIdlePosition(Projectile projectile, out int thisProjectilePosition, out int totalFairyCount)
		{
			thisProjectilePosition = 0;
			totalFairyCount = 0;
			foreach (var proj in Main.ActiveProjectiles)
			{
				if (proj.active && proj.owner == projectile.owner && proj.type == ModContent.ProjectileType<ShroomFairy>())
				{
					totalFairyCount++;
					if (proj.whoAmI < projectile.whoAmI)
					{
						thisProjectilePosition++;
					}
				}
			}
		}
		public static bool IsFairyAtMaxCapacity(Projectile projectile)
		{
			return GetVisualBellySize(projectile) >= 3 + GetVisualWeightStage(projectile);
        }
		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			VoreTracker tracker = PredProjectile.GetStomachTracker(Projectile);
			Projectile.ai[2] += GetVisualBellySize(Projectile);
			CheckForSpore(owner);
			if (owner.IsFoodFor(Projectile, out bool pastTense))
				Projectile.timeLeft = 2;
			if (!CheckActive(owner))
			{
				WaitOut(owner, SetSpeedMulti());
				return;
			}
			if (IsFairyAtMaxCapacity(Projectile))
			{
				WaitOut(owner, SetSpeedMulti());
				return;
			}
			if (Projectile.ai[1] > 0f) Projectile.ai[1] -= 1f;
			target = (null, null, Point.Zero);
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
			if (Projectile.ai[0] > 0f && GetVisualWeightStage(Projectile) == 8)
			{
				Projectile.ai[0] *= 1f;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Item.NewItem(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Center.Y), Vector2.One, ModContent.ItemType<MushroomToken>());
				}
			}
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
			float num = 1;
			if (Main.player[Projectile.owner].AsV2Player().ShroomNecklace)
				num = 1f * Math.Max(1 - GetVisualWeightStage(Projectile) / 24f, 0.05f) * Math.Max(1 - GetVisualBellySize(Projectile) / 24f, 0.05f);
			else
				num = 1f * Math.Max(1 - GetVisualWeightStage(Projectile) / 12f, 0.05f) * Math.Max(1 - GetVisualBellySize(Projectile) / 12f, 0.05f);
			return num;
		}
		public void FindTarget(Player owner)
		{
			Projectile closestProj = null;
			NPC closestNPC = null;
            Point closestTile = Point.Zero;
            float projDistance = 99999f;
			float npcDistance = 99999f;
            float tileDistance = 99999f;

            List<(PreyType, int)> IgnoreThese = [
				(PreyType.Projectile, ProjectileID.LastPrismLaser),
				(PreyType.Projectile, ProjectileID.RainCloudRaining),
				(PreyType.Projectile, ProjectileID.RainCloudMoving),
				(PreyType.Projectile, ProjectileID.BloodCloudRaining),
				(PreyType.Projectile, ProjectileID.BloodCloudMoving),
				(PreyType.Projectile, ProjectileID.RainbowFront),
				(PreyType.Projectile, ProjectileID.SpectreWrath),
				(PreyType.Projectile, ProjectileID.SpiritHeal),
				(PreyType.Projectile, ProjectileID.TerrarianBeam),
				(PreyType.Projectile, ProjectileID.MagnetSphereBall),
				(PreyType.Projectile, ProjectileID.TinyEater),
                (PreyType.Projectile, ModContent.ProjectileType<ThrowableFungalBottleProjectile>()),
                (PreyType.Projectile, ModContent.ProjectileType<ThrowableHoneyBottleProjectile>()),
                (PreyType.Projectile, ModContent.ProjectileType<ThrowableHotSauceBottleProjectile>()),
                ];

			List<(PreyType, int)> EdibleNPCs = [
				(PreyType.NPC, NPCID.WaterSphere),
				(PreyType.NPC, NPCID.ChaosBall),
				(PreyType.NPC, NPCID.ChaosBallTim),
				(PreyType.NPC, NPCID.BurningSphere),
				(PreyType.NPC, NPCID.VileSpit),
				(PreyType.NPC, NPCID.VileSpitEaterOfWorlds),
				(PreyType.NPC, NPCID.MartianDrone),
				(PreyType.NPC, NPCID.Spore),
				(PreyType.NPC, NPCID.FungiSpore),
				(PreyType.NPC, NPCID.BlazingWheel),
				(PreyType.NPC, NPCID.SpikeBall),
				(PreyType.NPC, NPCID.ChatteringTeethBomb),
                (PreyType.NPC, NPCID.SolarFlare),
                (PreyType.NPC, NPCID.SolarGoop),
                (PreyType.NPC, NPCID.AncientLight),
                (PreyType.NPC, NPCID.AncientDoom),
                ];

			List<(string, int)> EdibleTiles = [
				(null, TileID.Spikes),
                (null, TileID.WoodenSpikes),
                (null, TileID.RollingCactus),
                ];

            List<(string, int)> IgnoreTileIfThisIsAbove = [
                (null, TileID.Trees),
                (null, TileID.Cactus),
                (null, TileID.RollingCactus),
                (null, TileID.TreeAmber),
                (null, TileID.TreeAsh),
                (null, TileID.TreeAmethyst),
                (null, TileID.TreeSapphire),
                (null, TileID.TreeEmerald),
                (null, TileID.TreeRuby),
                (null, TileID.TreeDiamond),
                (null, TileID.TreeTopaz),
                (null, TileID.MushroomTrees),
                (null, TileID.PalmTree),
                (null, TileID.VanityTreeSakura),
                (null, TileID.VanityTreeYellowWillow),
                (null, TileID.Containers),
                (null, TileID.Containers2),
                (null, TileID.FakeContainers),
                (null, TileID.FakeContainers2),
                (null, TileID.Mannequin),
                (null, TileID.Womannequin),
                (null, TileID.TrashCan),
                (null, TileID.Dressers),
                (null, TileID.HatRack),
                (null, TileID.DemonAltar),
                (null, TileID.Bamboo),
                ];

            foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.CurrentCaptor() is not null) continue;
                bool shouldIgnore = true;
                foreach ((PreyType type, int ID) in EdibleNPCs)
                {
                    if (ID == npc.type || (npc.AsFood().TastyStrange && !npc.boss))
                        shouldIgnore = false;
                }
                if (shouldIgnore)
                    continue;
                float distance = npc.position.Distance(owner.position);
					if (distance < npcDistance)
					{
						closestNPC = npc;
						npcDistance = distance;
					}
			}
			foreach (var proj in Main.ActiveProjectiles)
			{
				if (proj.CurrentCaptor() is not null) continue;
				if ((!proj.friendly || proj.hostile) && proj.damage > 0 && !proj.IsMinionOrSentryRelated)
				{
					bool shouldIgnore = false;
					foreach ((PreyType type, int ID) in IgnoreThese)
					{
						if (ID == proj.type)
							shouldIgnore = true;
					}
					if (shouldIgnore)
						continue;
					float distance = proj.position.Distance(owner.position);
					if (distance < projDistance)
					{
						closestProj = proj;
						projDistance = distance;
					}
				}
			}
            Rectangle CheckingArea = new Rectangle((int)owner.Center.X - 128, (int)owner.Center.Y - 128, 256, 256);
            List<Point> NearbyTiles = Collision.GetTilesIn(CheckingArea.TopLeft(), CheckingArea.BottomRight());
            foreach (var point in NearbyTiles)
            {
                Tile tile = Framing.GetTileSafely(point);
                if (tile.HasTile)
                {
                    bool shouldIgnore = true;
                    foreach ((string nothin, int ID) in EdibleTiles)
                    {
                        if (ID == tile.TileType)
                            shouldIgnore = false;
                    }
                    if (shouldIgnore)
                        continue;
                    Tile tile2 = Framing.GetTileSafely(point + new Point(0,-1));
					if (tile2.HasTile)
					{
						foreach ((string nothin, int ID) in IgnoreTileIfThisIsAbove)
						{
							if (ID == tile2.TileType)
								shouldIgnore = true;
						}
						if (shouldIgnore)
							continue;
					}
                    Vector2 pointReal = new(point.X * 16, point.Y * 16);
					float distance = pointReal.Distance(owner.position);
					if (distance < tileDistance)
					{
						closestTile = point;
						tileDistance = distance;
					}
                }
            }
            if (projDistance > 320) closestProj = null;
			if (npcDistance > 320) closestNPC = null;
			if (tileDistance <= projDistance && tileDistance <= npcDistance)
			{
				closestProj = null;
                closestNPC = null;
            }
            if (projDistance <= tileDistance && projDistance <= npcDistance)
            {
                closestTile = Point.Zero;
                closestNPC = null;
            }
            if (npcDistance <= projDistance && npcDistance <= tileDistance)
            {
                closestProj = null;
                closestTile = Point.Zero;
            }
			target = (closestProj, closestNPC, closestTile);
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
			bool ateOwner = owner.IsFoodFor(Projectile, out bool churnedOwner);
			Vector2 vectorToIdlePosition;
			float distanceToIdlePosition;
			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 50f;
			GetIdlePosition(Projectile, out int count, out int totalcount);
			Vector2 Extra = AnglePosition(count, totalcount);
			idlePosition += Extra * 125;
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();
			bool atPos = false;

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
			{
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
				DustEffect(Projectile);
			}

			if (!ateOwner)
			{

				float speed = 10f;
				float inertia = 20f;
				if (distanceToIdlePosition > 600f)
				{
					speed = 18f;
					inertia = 60f;
				}
				else if (distanceToIdlePosition < 80f)
				{
					speed = 4f;
					inertia = 80f;
					atPos = true;
				}

				if (distanceToIdlePosition > 20f)
				{
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					if (!atPos)
					{
						vectorToIdlePosition.X *= SpeedMulti;
						if (vectorToIdlePosition.Y < 0)
						{
							vectorToIdlePosition.Y *= SpeedMulti;
							vectorToIdlePosition.Y *= (SpeedMulti + 2) / 3;
						}
					}
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
					if (!CheckForSolidFloor()) Projectile.velocity.Y -= 0.1667f + -SpeedMulti / 6f;

				}
				else if (Projectile.velocity == Vector2.Zero)
				{
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
			else Projectile.velocity *= 0.9f;
			Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -12, 12);
			if (!CheckForSolidFloor()) Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -12, 12) + (0.2f + -SpeedMulti / 5f);
			else Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -12, 12);

		}
		public void CHARGE(Player owner, Entity target, float SpeedMulti)
		{
			if (!target.active || target.CurrentCaptor() is not null)
				return;

			float speed = 25f;
			float inertia = 25f;
			Vector2 direction = Projectile.position.DirectionTo(target.position);
			direction.Normalize();
			Vector2 direction2 = direction * 10f;
			float distance = Projectile.position.Distance(target.position);
			if (distance <= 180)
			{
				DustEffect(Projectile);

				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 1f);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 1.25f);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 1.5f);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 1.75f);
				Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 2f);

				Projectile.ai[1] = 8f / SpeedMulti;
				Projectile.position = target.position;
				PredProjectile.Swallow(Projectile, target);

				DustEffect(Projectile);
			}
			else
			{
				direction *= speed;
				direction.X *= SpeedMulti;
				if (direction.Y < 0)
				{
					direction.Y *= SpeedMulti;
					direction.Y *= (SpeedMulti + 2) / 3;
				}
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
				Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -18, 18);
				if (!CheckForSolidFloor()) Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -18, 18) + (0.2f + -SpeedMulti / 5f);
				else Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -18, 18);
			}
		}
        public void CHARGE_Tile(Player owner, Point tilePoint, float SpeedMulti)
        {
            Tile target = Framing.GetTileSafely(tilePoint);
            Vector2 GlobalPosition = new(tilePoint.X * 16, tilePoint.Y * 16);
            if (!target.HasTile)
                return;

            float speed = 25f;
            float inertia = 25f;
            Vector2 direction = Projectile.position.DirectionTo(GlobalPosition);
            direction.Normalize();
            Vector2 direction2 = direction * 10f;
            float distance = Projectile.position.Distance(GlobalPosition);
            if (distance <= 180)
            {
                DustEffect(Projectile);

                Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 1f);
                Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 1.25f);
                Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 1.5f);
                Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 1.75f);
                Dust.NewDustDirect(Projectile.Center - new Vector2(8, 8), 32, 32, ModContent.DustType<ShroomFairyDust>(), direction2.X, direction2.Y, 0, default, 2f);

                Projectile.ai[1] = 8f / SpeedMulti;
                Projectile.position = GlobalPosition;
                int takenItem = TileBullshit.HelperMethods.PickItem(target);
                if (Main.netMode != NetmodeID.MultiplayerClient && takenItem >= 0)
				{
					int DroppedItem = Item.NewItem(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Center.Y), Vector2.One, takenItem);
					PredProjectile.Swallow(Projectile, Main.item[DroppedItem]);
					WorldGen.KillTile(tilePoint.X, tilePoint.Y, false, false, true);
				}
                DustEffect(Projectile);
            }
            else
            {
                direction *= speed;
                direction.X *= SpeedMulti;
                if (direction.Y < 0)
                {
                    direction.Y *= SpeedMulti;
                    direction.Y *= (SpeedMulti + 2) / 3;
                }
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -18, 18);
                if (!CheckForSolidFloor()) Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -18, 18) + (0.2f + -SpeedMulti / 5f);
                else Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -18, 18);
            }
        }
        public void WaitOut(Player owner, float SpeedMulti)
		{
			if (CheckForSolidFloor())
			{
				Projectile.velocity.X *= 0.9f;
				Projectile.velocity.Y *= 0.8f;
			}
			else
			{
				Projectile.velocity.X *= 0.9f;
				if (Projectile.velocity.Y < 0) Projectile.velocity.Y *= 0.9f;
				Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -12, 12);
				Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -12, 12) + (0.05f + -SpeedMulti / 50f);
			}
		}
		public bool CheckForSolidFloor()
		{
			int WeightOffset = 0;
			switch (GetVisualWeightStage(Projectile))
			{
				case 0 or 1 or 2 or 3: WeightOffset = 0; break;
				case 4: WeightOffset = 10; break;
				case 5: WeightOffset = 20; break;
				case 6: WeightOffset = 30; break;
				case 7: WeightOffset = 40; break;
				case 8: WeightOffset = 50; break;
			}
			if (Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height * 2 + WeightOffset, true)) return true;
			return false;
		}
		public void CheckForSpore(Player owner)
		{
			if (Projectile.ai[2] >= 1500)
			{
				Projectile.ai[2] -= 1500;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShroomFairySpore>(), 0, 0, owner.whoAmI);
			}
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
				case 4:
					text += "_" + FairySize;
					frameSize = 108;
					break;
				case 5 or 6 or 7:
					text += "_" + FairySize;
					frameSize = 148;
					break;
				case 8:
					text += "_" + FairySize;
					frameSize = 164;
					break;
			}
			Vector2 Offset = new Vector2(-10, 0);
			if (Projectile.direction == -1) Offset = new Vector2(-10 - (frameSize - 50), 0);
			Texture2D sprite = ModContent.Request<Texture2D>(text).Value;
			SpriteEffects val = Projectile.direction != -1 ? 0 : (SpriteEffects)1;
			SpriteEffects spriteEffects = val;
			Rectangle sourceRect = new Rectangle(frameSize * TumSize, frameSize * Projectile.frame, frameSize, frameSize);
			Main.EntitySpriteDraw(sprite, Projectile.position - Main.screenPosition + new Vector2(Offset.X, Offset.Y), (Rectangle)sourceRect, lightColor, Projectile.rotation, Vector2.Zero, 1f, spriteEffects, 0f);
			Texture2D sprite2 = ModContent.Request<Texture2D>(text + "_Fullbright").Value;
			Main.EntitySpriteDraw(sprite2, Projectile.position - Main.screenPosition + new Vector2(Offset.X, Offset.Y), (Rectangle)sourceRect, new Color(255, 255, 255), Projectile.rotation, Vector2.Zero, 1f, spriteEffects, 0f);
			return false;
		}
	}
	public class ShroomFairySpore : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
		}
		public sealed override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.penetrate = -1;

			Projectile.AsFood().DefinedSize = 0.12;
			Projectile.AsFood().MaxHealth = 5;
			Projectile.AsFood().Health = 5;
			Projectile.AsFood().OnKilledByDigestion += OnKilledByDigestion;
		}
		public override void OnSpawn(IEntitySource source)
		{
			Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ShroomFairyDust2>(), Vector2.Zero);
		}
		public static void OnKilledByDigestion(Projectile projectile, Entity pred)
		{
			if (pred is Player)
			{
				Player player = (Player)pred;
				player.Heal(15);
				if (Main.player[projectile.owner].AsV2Player().ShroomNecklace)
					player.AddBuff(ModContent.BuffType<SporeRegen>(), V2Utils.SensibleTime(seconds: 12));
			}
		}
		public override bool? CanDamage()
		{
			return false;
		}
		public Player FindClosestPlayer()
		{
			Player plr = null;
			float Distance = 99999f;
			foreach (var player in Main.ActivePlayers)
			{
				if (plr == null)
				{
					plr = player;
					Distance = plr.position.Distance(Projectile.position);
				}
				else
				{
					float distance2 = plr.position.Distance(Projectile.position);
					if (distance2 < Distance)
					{
						plr = player;
						Distance = distance2;
					}
				}
			}
			return plr;
		}
		public override void AI()
		{
			Player target = FindClosestPlayer();
			if (target != null)
			{
				Vector2 direction = Projectile.position.DirectionTo(target.position);
				direction.Normalize();
				Projectile.velocity += direction / 8f;
				Projectile.velocity.X = Math.Clamp(Projectile.velocity.X, -5, 5);
				Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -5, 5);
				if (Projectile.Center.Distance(target.Center) <= 150)
				{
					Projectile.velocity *= 0.93f;
				}
				if (Projectile.Hitbox.Intersects(target.Hitbox))
				{
					target.Heal(5);
					if (Main.player[Projectile.owner].AsV2Player().ShroomNecklace)
						target.AddBuff(ModContent.BuffType<SporeRegen>(), V2Utils.SensibleTime(seconds: 4));
					ShroomFairy.DustEffect(Projectile);
					Projectile.Kill();
				}
			}
			Projectile.ai[0] += 1;
		}
		public override void PostAI()
		{
			if (Projectile.ai[0] >= 500)
			{
				ShroomFairy.DustEffect(Projectile);
				Projectile.Kill();
			}
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
		public override bool PreDraw(ref Color lightColor)
		{
			Rectangle sourceRect = new Rectangle(18 * Projectile.frame, 0, 18, 18);
			SpriteEffects val = Projectile.direction != -1 ? 0 : (SpriteEffects)1;
			SpriteEffects spriteEffects = val;
			Texture2D sprite = ModContent.Request<Texture2D>("V2/Projectiles/Voraria/Weapons/Summon/ShroomFairySpore").Value;
			Main.EntitySpriteDraw(sprite, Projectile.position - Main.screenPosition, (Rectangle)sourceRect, lightColor, Projectile.rotation, Vector2.Zero, 1f, spriteEffects, 0f);
			Texture2D sprite2 = ModContent.Request<Texture2D>("V2/Projectiles/Voraria/Weapons/Summon/ShroomFairySpore_Fullbright").Value;
			Main.EntitySpriteDraw(sprite2, Projectile.position - Main.screenPosition, (Rectangle)sourceRect, new Color(255, 255, 255), Projectile.rotation, Vector2.Zero, 1f, spriteEffects, 0f);
			return false;
		}
	}
}

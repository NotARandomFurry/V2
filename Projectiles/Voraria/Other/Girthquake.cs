using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using V2.Core;
using V2.Items;
using V2.Items.Voraria;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Projectiles.Voraria.Weapons.Ranged.Throwables;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Projectiles.Voraria.Other
{
    public class Girthquake : ModProjectile
    {
        public override string Texture => "V2/Tiles/InvisibleImage";
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = -1;

            Projectile.AsFood().CannotBeEatenDueToShenanigans = true;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (!target.AsV2NPC().CanBeDamagedByFallingPeople) return false;
            return null;
        }
        public override void AI()
        {
            float num = Projectile.ai[1];
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 9f)
            {
                Projectile.Kill();
                return;
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.position = Projectile.Center;
            Projectile.Size = new Vector2(16f, 8f) * MathHelper.Lerp(5f, num, Utils.GetLerpValue(0f, 9f, Projectile.ai[0], false));
            Projectile.Center = Projectile.position;
            Point point = Projectile.TopLeft.ToTileCoordinates();
            Point point2 = Projectile.BottomRight.ToTileCoordinates();
            int num2 = point.X / 2 + point2.X / 2;
            int num3 = Projectile.width / 2;
            if ((int)Projectile.ai[0] % 3 == 0)
            {
                int num4 = (int)Projectile.ai[0] / 3;
                for (int i = point.X; i <= point2.X; i++)
                {
                    for (int j = point.Y; j <= point2.Y; j++)
                    {
                        if (Vector2.Distance(Projectile.Center, new Vector2((float)(i * 16), (float)(j * 16))) <= (float)num3)
                        {
                            Tile tileSafely = Framing.GetTileSafely(i, j);
                            if (tileSafely.HasTile && Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType] && !Main.tileFrameImportant[tileSafely.TileType])
                            {
                                Tile tileSafely2 = Framing.GetTileSafely(i, j - 1);
                                if (!tileSafely2.HasTile || !Main.tileSolid[tileSafely2.TileType] || Main.tileSolidTop[tileSafely2.TileType])
                                {
                                    int num5 = WorldGen.KillTile_GetTileDustAmount(true, tileSafely, i, j);
                                    for (int k = 0; k < num5; k++)
                                    {
                                        Dust dust = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
                                        dust.velocity.Y = dust.velocity.Y - (3f + (float)num4 * 1.5f);
                                        dust.velocity.Y = dust.velocity.Y * Main.rand.NextFloat();
                                        dust.velocity.Y = dust.velocity.Y * 0.75f;
                                        dust.scale += (float)num4 * 0.03f;
                                    }
                                    if (num4 >= 2)
                                    {
                                        {
                                            for (int m = 0; m < num5 - 1; m++)
                                            {
                                                Dust dust5 = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
                                                dust5.velocity.Y = dust5.velocity.Y - (1f + (float)num4);
                                                dust5.velocity.Y = dust5.velocity.Y * Main.rand.NextFloat();
                                                dust5.velocity.Y = dust5.velocity.Y * 0.75f;
                                            }
                                        }
                                    }
                                    if (num5 > 0 && !Main.rand.NextBool(3))
                                    {
                                        float num7 = (float)Math.Abs(num2 - i) / (num / 2f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Projectile.damage = (int)Math.Ceiling(Projectile.damage * 0.75f);
        }
    }
    public class FallingHitbox : ModProjectile
    {
        public override string Texture => "V2/Tiles/InvisibleImage";
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 3;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;

<<<<<<< Updated upstream
            Projectile.AsFood().CannotBeEatenDueToShenanigans = true;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (!target.AsV2NPC().CanBeDamagedByFallingPeople) return false;
            return null;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.width = (int)Projectile.ai[0] + 4;
            Projectile.height = (int)Projectile.ai[1] + 4;
            Projectile.position.X -= Projectile.width / 2;
            Projectile.position.Y -= Projectile.height / 2;
=======
			Projectile.AsFood().CannotBeEatenDueToShenanigans = true;
		}
		public override bool CanHitPlayer(Player target)
		{
			return false;
		}
		public override bool? CanHitNPC(NPC target)
		{
			if (!target.AsV2NPC().CanBeDamagedByFallingPeople) return false;
			return null;
		}
		public override void OnSpawn(IEntitySource source)
		{
			Projectile.width = (int)Projectile.ai[0] + 4;
			Projectile.height = (int)Projectile.ai[1] + 4;
			Projectile.position.X -= Projectile.width / 2;
			Projectile.position.Y -= Projectile.height / 2;
		}
	}
    public class ItemRoller : ModProjectile
    {
		public static bool EvaluatingItem = false;
		public static int CurrentID = 1;
		public static int EdibleItems = 0;
		public static int EntirelyErasedItems = 0;

		public static List<int> UnobtainableItems = [
			ItemID.SolarFlareHammer,
			ItemID.SolarFlareChainsaw,
			ItemID.SolarFlareAxe,
            ItemID.VortexHammer,
            ItemID.VortexChainsaw,
            ItemID.VortexAxe,
            ItemID.NebulaHammer,
            ItemID.NebulaChainsaw,
            ItemID.NebulaAxe,
            ItemID.StardustHammer,
            ItemID.StardustChainsaw,
            ItemID.StardustAxe,
			ItemID.FirstFractal,
			ItemID.SkeletonBow,
			ItemID.BlueCultistFighterBanner,
			ItemID.WhiteCultistArcherBanner,
			ItemID.WhiteCultistCasterBanner,
			ItemID.WhiteCultistFighterBanner,
			ItemID.PoisonousSporeBanner,
			ItemID.SeveredHandBanner,
            ItemID.BoneBlock,
			ItemID.CultistBossBag,
			ItemID.BluePresent,
            ItemID.GreenPresent,
            ItemID.YellowPresent,
			ItemID.ColorOnlyDye,
			ItemID.SleepingIcon,
			ItemID.Fake_newchest1,
            ItemID.Fake_newchest2,
			ItemID.PhasicWarpEjector,
			ItemID.OgreMask,
			ItemID.GoblinMask,
			ItemID.GoblinBomberCap,
			ItemID.EtherianJavelin,
			ItemID.KoboldDynamiteBackpack,
			ItemID.ApplePieSlice,
			ItemID.BoringBow,
			ItemID.BossBagOgre,
			ItemID.BossBagDarkMage
            ];

        public override string Texture => "V2/Tiles/InvisibleImage";
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 75000;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = -1;

            Projectile.AsFood().CannotBeEatenDueToShenanigans = true;
        }
        public override void AI()
        {
			if (CurrentID <= 5455)
			{
				Item bruh = Main.item[1];
                if (!EvaluatingItem)
                {
                    EvaluatingItem = true;
					bruh.SetDefaults(CurrentID);
					bruh.active = true;
                }
                else
                {
                    EvaluatingItem = false;
					CurrentID++;
					if (bruh.Name.Length < 1 || UnobtainableItems.Contains(bruh.type))
					{
						EntirelyErasedItems++;
                        Main.NewText("Checked..?", Color.DarkGray);
                    }
					else
					if (bruh.AsFood() is not null && bruh.AsFood().Health > 0)
					{
						EdibleItems++;
						Main.NewText("Checked " + bruh.Name, Color.LightGreen);
					}
					else
						Main.NewText("Checked " + bruh.Name, Color.PaleVioletRed);
                }
            }
			else
			{
                Main.NewText(EdibleItems.ToString() + " out of " + (5455 - EntirelyErasedItems).ToString() + " items are edible! (" + (EdibleItems / (5455f - EntirelyErasedItems) * 100).CastToDecimalPlaces(3).ToString() + "%)");
                Projectile.Kill();
			}
        }
        public override void OnSpawn(IEntitySource source)
        {
			CurrentID = 1;
			EdibleItems = 0;
			EntirelyErasedItems = 0;
>>>>>>> Stashed changes
        }
    }
}

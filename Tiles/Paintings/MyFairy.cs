using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using V2.Core;
using V2.Items.Voraria.Placeables;
using V2.NPCs;
<<<<<<< Updated upstream
=======
using V2.Projectiles;
using V2.Projectiles.Voraria.Weapons.Summon;
>>>>>>> Stashed changes
using V2.Sounds.Vore;

namespace V2.Tiles.Paintings
{
    public class MyFairy : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<MyFairy_TileEntity>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Painting"));
            DustType = 41;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<MyFairy_TileEntity>().Kill(i, j);
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity))
            {
                if (tileEntity is MyFairy_TileEntity)
                {
<<<<<<< Updated upstream
                    foreach (var npc in Main.ActiveNPCs)
                    {
                        if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 2f && npc.type == ModContent.NPCType<MyFairy_NPCEntity>())
                        {
                            int tumSize = MyFairy_NPCEntity.GetVisualBellySize(npc);
                            int weightSize = MyFairy_NPCEntity.GetVisualWeightStage(npc);
=======
                    foreach (var npc in Main.ActiveProjectiles)
                    {
                        if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 2f && npc.type == ModContent.ProjectileType<MyFairy_ProjectileEntity>())
                        {
                            int tumSize = MyFairy_ProjectileEntity.GetVisualBellySize(npc);
                            int weightSize = MyFairy_ProjectileEntity.GetVisualWeightStage(npc);
>>>>>>> Stashed changes
                            Texture2D texture = ModContent.Request<Texture2D>("V2/Tiles/Paintings/MyFairy_SpriteSheet").Value;
                            Rectangle sourceRect = new Rectangle(64 * weightSize, 64 * tumSize, 64, 64);
                            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                            spriteBatch.Draw(
                                texture,
                                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                                sourceRect,
                                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
            return false;
        }
    }
    public class MyFairy_TileEntity : ModTileEntity
    {
<<<<<<< Updated upstream
        public NPC connectedNPC = null;

=======
        public Projectile connectedNPC = null;
        public double WeightOnLoad = 0;
>>>>>>> Stashed changes
        public override void Update()
        {
            if (connectedNPC is null)
            {
                Activate();
            }
<<<<<<< Updated upstream
            else if (!connectedNPC.active || connectedNPC.type != ModContent.NPCType<MyFairy_NPCEntity>())
=======
            else if (!connectedNPC.active || connectedNPC.type != ModContent.ProjectileType<MyFairy_ProjectileEntity>())
>>>>>>> Stashed changes
            {
                Activate();
            }
                
        }
        public void Activate()
        {
<<<<<<< Updated upstream
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.active && (npc.position / 16).Distance(Position.ToVector2()) < 2f && npc.type == ModContent.NPCType<MyFairy_NPCEntity>())
=======
            foreach (var npc in Main.ActiveProjectiles)
            {
                if (npc.active && (npc.position / 16).Distance(Position.ToVector2()) < 2f && npc.type == ModContent.ProjectileType<MyFairy_ProjectileEntity>())
>>>>>>> Stashed changes
                {
                    connectedNPC = npc;
                    return;
                }
            }
            //ill be honest i dont exactly know the grounds for the offset for the npc but i *think* its like, half of the X tiles and all Y tiles
<<<<<<< Updated upstream
            int num = NPC.NewNPC(new EntitySource_TileEntity(this, null), (int)(Position.X * 16) + 32, (int)(Position.Y * 16) + 64, ModContent.NPCType<MyFairy_NPCEntity>());
            connectedNPC = Main.npc[num];
            Main.npc[num].netUpdate = true;
                
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, (float)Position.X, (float)Position.Y);
=======
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int num = Projectile.NewProjectile(new EntitySource_TileEntity(this, null), new Vector2((int)(Position.X * 16) + 32, (int)(Position.Y * 16) + 32), Vector2.Zero, ModContent.ProjectileType<MyFairy_ProjectileEntity>(), 0, 0);
                connectedNPC = Main.projectile[num];
                connectedNPC.AsPred().ExtraWeight = WeightOnLoad;
                Main.projectile[num].netUpdate = true;
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, (float)Position.X, (float)Position.Y);
                }
>>>>>>> Stashed changes
            }
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<MyFairy>();
        }
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                int width = 4;
                int height = 4;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
                return -1;
            }
            int placedEntity = Place(i, j);
            return placedEntity;
        }
        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }
<<<<<<< Updated upstream
    }
    public class MyFairy_NPCEntity : ModNPC
=======

        public override void SaveData(TagCompound tag)
        {
            tag.Add("ExtraWeight", connectedNPC.AsPred().ExtraWeight);
        }

        public override void LoadData(TagCompound tag)
        {
            WeightOnLoad = tag.GetDouble("ExtraWeight");
        }
    }
    public class MyFairy_ProjectileEntity : ModProjectile
>>>>>>> Stashed changes
    {
        public override string Texture => "V2/Tiles/Paintings/InvisibleImage";
        public override void SetDefaults()
        {
<<<<<<< Updated upstream
            NPC.friendly = true;
            NPC.width = 64;
            NPC.height = 64;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.immortal = true;
            NPC.HideStrikeDamage = true;
            NPC.lifeMax = 5000;
            NPC.noGravity = true;
            NPC.ShowNameOnHover = false;

            NPC.AsFood().CannotBeEatenDueToShenanigans = true;

            NPC.AsFood().DefinedBaseSize = 8;
            NPC.AsPred().WeightGainRatio = 0.4;
            NPC.AsPred().MaxStomachCapacity = 1.5;
            NPC.AsPred().BaseStomachacheMeterCapacity = 425.0;

            NPC.AsPred().CanBeForceFed = CanPaintingBeForceFed;

            NPC.AsPred().DigestionType = EntityDigestionType.Acidic;
            NPC.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
            NPC.AsPred().GetDigestionTickRate = GetDigestionTickRate;
            NPC.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

            NPC.AsPred().GetVisualBellySize = GetVisualBellySize;
            NPC.AsPred().GetVisualWeightStage = GetVisualWeightStage;

            NPC.AsPred().SmallBurps = Burps.Humanoid.Small;
            NPC.AsPred().SmallBurpThreshold = 0.35;
            NPC.AsPred().StandardBurps = Burps.Humanoid.Standard;
            NPC.AsPred().SmallGulps = Gulps.Short;
            NPC.AsPred().SmallGulpThreshold = 0.35;
            NPC.AsPred().BigGulps = Gulps.Standard;
        }
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
        public override bool CanBeHitByNPC(NPC attacker) => false;
        public static bool CanPaintingBeForceFed(NPC npc) => true;
        public override void AI()
        {
            Tile Painting = Main.tile[NPC.position.ToTileCoordinates()];
            if (!Painting.HasTile || Painting.TileType != ModContent.TileType<MyFairy>())
            {
                NPC.active = false;
            }
            if (Main.rand.NextBool(100)) NPC.DoContactGulpage();
        }
        public static int GetVisualBellySize(NPC npc)
        {
            return Math.Min(
                (int)Math.Floor(4 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
                4
            );
        }
        public static int GetVisualWeightStage(NPC npc)
        {
            return Math.Min(
                (int)Math.Floor(2 * Math.Sqrt(npc.AsPred().ExtraWeight)),
                4
            );
        }
        public override bool NeedSaving() => true;

        public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 22;
        public static double GetDigestionTickRate(NPC npc, PreyData prey) => 1.2;
        public static double GetPreyAbsorptionRate(NPC npc)
=======
            Projectile.friendly = true;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.damage = 0;
            Projectile.timeLeft = 6000;
            Projectile.tileCollide = false;

            Projectile.AsFood().CannotBeEatenDueToShenanigans = true;

            Projectile.AsFood().DefinedSize = 8;
            Projectile.AsPred().WeightGainRatio = 0.4;
            Projectile.AsPred().MaxStomachCapacity = 1.5;
            Projectile.AsPred().BaseStomachacheMeterCapacity = 425.0;
            Projectile.AsPred().CanSwallowBosses = false;
            Projectile.AsFood().MaxHealth = 7500;
            Projectile.AsFood().Health = 7500;

            Projectile.AsPred().MouthSoundRawOffset = new Vector2(0f, -14f);
            Projectile.AsPred().SmallGulps = Gulps.Short;
            Projectile.AsPred().SmallGulpThreshold = 0.1;
            Projectile.AsPred().BigGulps = Gulps.Standard;
            Projectile.AsPred().CanBeForceFed = CanPaintingBeForceFed;
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
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
        public override bool CanHitPvp(Player target) => false;
        public static bool CanPaintingBeForceFed(Projectile projectile) => true;
        public override void AI()
        {
            Projectile.timeLeft = 6000;
            Projectile.velocity = Vector2.Zero;
            Tile Painting = Main.tile[Projectile.position.ToTileCoordinates()];
            if (!Painting.HasTile || Painting.TileType != ModContent.TileType<MyFairy>())
            {
                Projectile.active = false;
            }
            if (Main.rand.NextBool(100)) Projectile.DoContactGulpage();
        }
        public override void PostAI()
        {
            Projectile.velocity = Vector2.Zero;
        }
        public static int GetVisualBellySize(Projectile projectile)
        {
            return Math.Min(
                (int)Math.Floor(4 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
                4
            );
        }
        public static int GetVisualWeightStage(Projectile projectile)
        {
            return Math.Min(
                (int)Math.Floor(2 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
                3
            );
        }
        public static double GetDigestionTickDamage(Projectile projectile, PreyData prey) => 22;
        public static double GetDigestionTickRate(Projectile projectile, PreyData prey) => 1.2;
        public static double GetPreyAbsorptionRate(Projectile projectile)
>>>>>>> Stashed changes
        {
            double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
                seconds: 30
            );
            return baseAbsorptionRate;
        }
    }
}

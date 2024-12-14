using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using V2.Core;
using V2.NPCs;
using V2.Sounds.Vore;

namespace V2.Tiles.Paintings
{
    public class Dryadisque : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 4;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<Dryadisque_TileEntity>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Painting"));
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<Dryadisque_TileEntity>().Kill(i, j);
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity))
            {
                if (tileEntity is Dryadisque_TileEntity)
                {
                    foreach (var npc in Main.ActiveNPCs)
                    {
                        if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 2f && npc.type == ModContent.NPCType<Dryadisque_NPCEntity>())
                        {
                            int XOffset = 0;
                            if (npc.ai[0] <= 6) XOffset = 96;
                            int tumSize = Dryadisque_NPCEntity.GetVisualBellySize(npc);
                            Texture2D texture = ModContent.Request<Texture2D>("V2/Tiles/Paintings/Dryadisque_SpriteSheet").Value;
                            Rectangle sourceRect = new Rectangle(XOffset, 64 * tumSize, 96, 64);
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
    public class Dryadisque_TileEntity : ModTileEntity
    {
        public NPC connectedNPC = null;

        public override void Update()
        {
            if (connectedNPC is null)
            {
                Activate();
            }
            else if (!connectedNPC.active || connectedNPC.type != ModContent.NPCType<Dryadisque_NPCEntity>())
            {
                Activate();
            }
                
        }
        public void Activate()
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.active && (npc.position / 16).Distance(Position.ToVector2()) < 2f && npc.type == ModContent.NPCType<Dryadisque_NPCEntity>())
                {
                    connectedNPC = npc;
                    return;
                }
            }

            int num = NPC.NewNPC(new EntitySource_TileEntity(this, null), (int)(Position.X * 16) + 48, (int)(Position.Y * 16) + 64, ModContent.NPCType<Dryadisque_NPCEntity>());
            connectedNPC = Main.npc[num];
            Main.npc[num].netUpdate = true;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, (float)Position.X, (float)Position.Y);
            }
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<Dryadisque>();
        }
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
                int width = 6;
                int height = 4;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                // Sync the placement of the tile entity with other clients
                // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
                return -1;
            }

            // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
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
    }
    public class Dryadisque_NPCEntity : ModNPC
    {
        public override string Texture => "V2/Tiles/Paintings/InvisibleImage";
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 96;
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

            NPC.AsPred().WeightGainRatio = 0.111;
            NPC.AsPred().MaxStomachCapacity = 12.50;
            NPC.AsPred().BaseStomachacheMeterCapacity = 750.0;

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
            NPC.ai[0]--;
            if (NPC.ai[0] <= 0) NPC.ai[0] = Main.rand.Next(300, 600);
            Tile Painting = Main.tile[NPC.position.ToTileCoordinates()];
            if (!Painting.HasTile || Painting.TileType != ModContent.TileType<Dryadisque>())
            {
                NPC.active = false;
            }
            if (Main.rand.NextBool(100)) NPC.DoContactGulpage();
        }
        public static int GetVisualBellySize(NPC npc)
        {
            return Math.Min(
                (int)Math.Floor(3 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
                7
            );
        }
        public static int GetVisualWeightStage(NPC npc)
        {
            return Math.Min(
                (int)Math.Floor(2 * Math.Sqrt(npc.AsPred().ExtraWeight)),
                0
            );
        }
        public override bool NeedSaving() => true;

        public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 22;
        public static double GetDigestionTickRate(NPC npc, PreyData prey) => 1.2;
        public static double GetPreyAbsorptionRate(NPC npc)
        {
            double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
                minutes: 0,
                seconds: 30
            );
            return baseAbsorptionRate;
        }
    }
}

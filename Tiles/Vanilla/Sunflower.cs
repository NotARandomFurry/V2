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
using V2.Projectiles;
using V2.Sounds.Vore;

namespace V2.Tiles.Vanilla
{
	public class Sunflower : ModTile
	{
		
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.FramesOnKillWall[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.AnchorValidTiles = [TileID.Grass, TileID.CorruptGrass, TileID.HallowedGrass, TileID.CrimsonGrass, TileID.GolfGrass, TileID.GolfGrassHallowed];

			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<Sunflower_TileEntity>().Hook_AfterPlacement, -1, 0, true);
			TileObjectData.newTile.UsesCustomCanPlace = true;

			TileObjectData.addTile(Type);

			AddMapEntry(new Color(220,200,10), Language.GetText("Sunflower"));
			DustType = 2;
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			ModContent.GetInstance<Sunflower_TileEntity>().Kill(i, j);
		}
		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity))
			{
				if (tileEntity is Sunflower_TileEntity)
				{
					foreach (var npc in Main.ActiveProjectiles)
					{
						if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 2f && npc.type == ModContent.ProjectileType<Sunflower_ProjectileEntity>())
						{
							int tumSize = Sunflower_ProjectileEntity.GetVisualBellySize(npc);
							int weightSize = Sunflower_ProjectileEntity.GetVisualWeightStage(npc);
							Texture2D texture = ModContent.Request<Texture2D>("V2/Tiles/Vanilla/Sunflower_SpriteSheet").Value;
							Rectangle sourceRect = new Rectangle(32 * (int)npc.ai[0], (64 * tumSize) + 2 * tumSize, 32, 66);
							Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
							spriteBatch.Draw(
								texture,
								new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
								sourceRect,
								Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);
							Texture2D textureGlow = ModContent.Request<Texture2D>("V2/Tiles/Vanilla/Sunflower_Glowmask").Value;
							Rectangle sourceRectGlow = new Rectangle(32 * (int)npc.ai[0], 0, 32, 66);
							spriteBatch.Draw(
								textureGlow,
								new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
								sourceRectGlow,
								Color.White);
						}
					}
				}
			}
			return false;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer) return;
			Main.SceneMetrics.HasSunflower = true;
		}
	}
	public class Sunflower_TileEntity : ModTileEntity
	{
		public Projectile connectedNPC = null;
		public double WeightOnLoad = 0;
		public override void Update()
		{
			if (connectedNPC is null)
			{
				Activate();
			}
			else if (!connectedNPC.active || connectedNPC.type != ModContent.ProjectileType<Sunflower_ProjectileEntity>())
			{
				Activate();
			}

		}
		public void Activate()
		{
			foreach (var npc in Main.ActiveProjectiles)
			{
				if (npc.active && (npc.position / 16).Distance(Position.ToVector2()) < 1f && npc.type == ModContent.ProjectileType<Sunflower_ProjectileEntity>())
				{
					connectedNPC = npc;
					return;
				}
			}
			//ill be honest i dont exactly know the grounds for the offset for the npc but i *think* its like, half of the X tiles and all Y tiles
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int num = Projectile.NewProjectile(new EntitySource_TileEntity(this, null), new Vector2((int)(Position.X * 16) + 16, (int)(Position.Y * 16) + 32), Vector2.Zero, ModContent.ProjectileType<Sunflower_ProjectileEntity>(), 0, 0);
				connectedNPC = Main.projectile[num];
				connectedNPC.AsPred().ExtraWeight = WeightOnLoad;
				Main.projectile[num].netUpdate = true;
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, (float)Position.X, (float)Position.Y);
				}
			}
		}
		public override bool IsTileValidForEntity(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile.HasTile && tile.TileType == ModContent.TileType<Sunflower>();
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

		public override void SaveData(TagCompound tag)
		{
			tag.Add("ExtraWeight", connectedNPC.AsPred().ExtraWeight);
		}

		public override void LoadData(TagCompound tag)
		{
			WeightOnLoad = tag.GetDouble("ExtraWeight");
		}
	}
	public class Sunflower_ProjectileEntity : ModProjectile
	{
		public override string Texture => "V2/Tiles/InvisibleImage";
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 32;
			Projectile.height = 64;
			Projectile.aiStyle = -1;
			Projectile.damage = 0;
			Projectile.timeLeft = 6000;
			Projectile.tileCollide = false;

			Projectile.AsFood().CannotBeEatenDueToShenanigans = true;

			Projectile.AsFood().DefinedSize = 8;
			Projectile.AsPred().WeightGainRatio = 0.1;
			Projectile.AsPred().MaxStomachCapacity = 1.5;
			Projectile.AsPred().BaseStomachacheMeterCapacity = 200.0;
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
			Projectile.AsPred().BurpPitchOffset = 0.1f;

			Projectile.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			Projectile.AsPred().GetVisualBellySize = GetVisualBellySize;
			Projectile.AsPred().GetVisualWeightStage = GetVisualWeightStage;
		}
		public override bool? CanHitNPC(NPC target) => false;
		public override bool CanHitPlayer(Player target) => false;
		public override bool CanHitPvp(Player target) => false;
		public static bool CanPaintingBeForceFed(Projectile projectile) => true;
		public override void OnSpawn(IEntitySource source)
		{
			Projectile.ai[0] = Main.rand.Next(3);
		}
		public override void AI()
		{
			Projectile.timeLeft = 6000;
			Projectile.velocity = Vector2.Zero;
			Tile Painting = Main.tile[Projectile.position.ToTileCoordinates()];
			if (!Painting.HasTile || Painting.TileType != ModContent.TileType<Sunflower>())
			{
				Projectile.active = false;
			}
			if (Main.rand.NextBool(100)) Projectile.DoContactGulpage();
			Lighting.AddLight(Projectile.Center + new Vector2(0, -16), new Vector3(150, 150, 50) * 0.001f);
		}
		public override void PostAI()
		{
			Projectile.velocity = Vector2.Zero;
		}
		public static int GetVisualBellySize(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(3 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				3
			);
		}
		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(2 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				0
			);
		}
		public static double GetDigestionTickDamage(Projectile projectile, PreyData prey) => 22;
		public static double GetDigestionTickRate(Projectile projectile, PreyData prey) => 1.2;
		public static double GetPreyAbsorptionRate(Projectile projectile)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				seconds: 30
			);
			return baseAbsorptionRate;
		}
	}
}
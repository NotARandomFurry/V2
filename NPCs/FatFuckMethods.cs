using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.NPCs
{
<<<<<<< Updated upstream
    public class FatFuckMethods
    {
        public static string FatassDeathReason(NPC npc, Player victim)
        {
            List<string> deathMessageKeyList =
            [
                "Mods.V2.Death.FlattenedByAFatAss.Universal.1",
                "Mods.V2.Death.FlattenedByAFatAss.Universal.2",
                "Mods.V2.Death.FlattenedByAFatAss.Universal.3",
                "Mods.V2.Death.FlattenedByAFatAss.Universal.4",
                "Mods.V2.Death.FlattenedByAFatAss.Universal.5",
                "Mods.V2.Death.FlattenedByAFatAss.Universal.6",
                "Mods.V2.Death.FlattenedByAFatAss.Universal.7",
            ];
            string finalDeathReasonKey = Main.rand.NextFromCollection(deathMessageKeyList);

            return Language.GetTextValueWith(
                finalDeathReasonKey,
                new
                {
                    Player = victim.name,
                    NPC = npc.TypeName,
                }
            );
        }
        public static void DamageTiles(NPC npc, Rectangle Hitbox, int power = 0)
        {
            if (power < 25) return;
            List<Point> tiles = Collision.GetTilesIn(Hitbox.BottomLeft() - new Vector2(-2, -2), Hitbox.BottomRight() + new Vector2(2, 10));
            double tileCount = 0;
            foreach (var point in tiles)
            {
                Tile tile = Framing.GetTileSafely(point);
                if (tile.HasTile)
                {
                    tileCount++;
                    if (tile.TileType == TileID.Platforms || tile.TileType == TileID.Glass) tileCount -= 0.33;
                }
            }
            if (tileCount == 0) return;
            int breakChance = Math.Max((int)(50 * (tileCount / 4)) - (power / 8), 0);
            if (breakChance > 200) return;
            foreach (var point in tiles)
            {
                Tile tile = Framing.GetTileSafely(point);
                int chance = Main.rand.Next(breakChance);
                if (chance == 0)
                {
                    Tile tileBelow = Framing.GetTileSafely(point + new Point(0, 1));
                    Tile extraTile = Framing.GetTileSafely(point + new Point(0, 2));
                    if (tile.HasTile && tileBelow.HasTile && extraTile.HasTile) continue; //ground is too thick
                    if (tileBelow.HasTile)
                    {
                        if (Main.rand.Next(breakChance) > breakChance / 5) continue;
                    }
                    WorldGen.KillTile(point.X, point.Y);
                }
                else if (chance <= 33)
                {
                    Tile tileBelow = Framing.GetTileSafely(point + new Point(0, 1));
                    Tile extraTile = Framing.GetTileSafely(point + new Point(0, 2));
                    if (tile.HasTile && tileBelow.HasTile && extraTile.HasTile) continue; //ground is too thick
                    if (tileBelow.HasTile)
                    {
                        if (Main.rand.Next(breakChance) > breakChance / 5) continue;
                    }
                    WorldGen.KillTile(point.X, point.Y, true, true);
                }
            }
        }
        public static void PushPlayers(NPC npc, int heightsize = 0, int heightoffset = 0, int fallDamage = 0)
        {
            Rectangle Hitbox = npc.Hitbox;
            Hitbox.Height += heightsize;
            Hitbox.Offset(0, -heightsize + heightoffset);
            fallDamage = (Hitbox.Height + Hitbox.Width) / 3;
            Vector2 Center = Hitbox.Center();
            int Width = Hitbox.Width;
            int Height = Hitbox.Height;
            foreach (var player in Main.ActivePlayers)
            {
                if (Hitbox.Intersects(player.Hitbox) && !player.dead)
                {
                    if (player.Center.Y < Center.Y - (Height / 8) && player.Center.X < Center.X + (Width / 1.25) && player.Center.X > Center.X - (Width / 1.25))
                    {
                        player.RefreshMovementAbilities();
                        if (player.velocity.Y > 2f && (player.controlJump || player.controlUp)) player.velocity.Y = player.velocity.Y * -1.2f;
                        else if (player.velocity.Y > 2f) player.velocity.Y = player.velocity.Y * -0.93f;
                        else player.velocity.Y = -2f;
                    }
                    else if (player.Center.Y >= Center.Y + (Height / 3) && player.Center.X < Center.X + (Width / 2.25) && player.Center.X > Center.X - (Width / 2.25))
                    {
                        player.position.Y = Center.Y + (Height / 2);
                        if (player.velocity.Y < 2f) player.velocity.Y = player.velocity.Y * -0.95f;
                        else player.velocity.Y = 2f;
                    }
                    if (player.Center.X < Center.X - 1 && player.Center.Y > Center.Y - (Height / 2) && player.Center.Y < Center.Y + (Height / 2))
                    {
                        if (player.velocity.X > 2f) player.velocity.X = player.velocity.X * -0.95f;
                        else player.velocity.X = -2f;
                    }
                    else if (player.Center.X >= Center.X + 1 && player.Center.Y > Center.Y - (Height / 2) && player.Center.Y < Center.Y + (Height / 2))
                    {
                        if (player.velocity.X < -2f) player.velocity.X = player.velocity.X * -0.95f;
                        else player.velocity.X = 2f;
                    }
                    if (fallDamage > 0 && npc.velocity.Y > 1.5f && player.Center.Y >= Center.Y + (Height / 3))
                    {
                        player.Hurt(PlayerDeathReason.ByCustomReason(FatassDeathReason(npc, player)), (int)(fallDamage * (npc.velocity.Y - 1.5f)), 0, false, false, -1, false);
                    }
                }
            }
            foreach (var othernpc in Main.ActiveNPCs)
            {
                if (othernpc.whoAmI == npc.whoAmI) continue;
                if (Hitbox.Intersects(othernpc.Hitbox))
                {
                    if (!othernpc.noTileCollide)
                    {
                        if (othernpc.Center.Y < Center.Y + (Height / 16) && othernpc.Center.X < Center.X + (Width / 1.25) && othernpc.Center.X > Center.X - (Width / 1.25))
                        {
                            if (othernpc.velocity.Y > 2f) othernpc.velocity.Y = othernpc.velocity.Y * -0.95f;
                            else othernpc.velocity.Y = -2f;
                        }
                        else if (othernpc.Center.Y >= Center.Y + (Height / 3) && othernpc.Center.X < Center.X + (Width / 2.25) && othernpc.Center.X > Center.X - (Width / 2.25))
                        {
                            othernpc.position.Y = Center.Y + (Height / 2);
                            if (othernpc.velocity.Y < 2f) othernpc.velocity.Y = othernpc.velocity.Y * -0.95f;
                            else othernpc.velocity.Y = 2f;
                        }
                        if (othernpc.Center.X < Center.X - 1 && othernpc.Center.Y > Center.Y - (Height / 2) && othernpc.Center.Y < Center.Y + (Height / 2))
                        {
                            if (othernpc.velocity.X > 2f) othernpc.velocity.X = othernpc.velocity.X * -0.95f;
                            else othernpc.velocity.X = -2f;
                        }
                        else if (othernpc.Center.X >= Center.X + 1 && othernpc.Center.Y > Center.Y - (Height / 2) && othernpc.Center.Y < Center.Y + (Height / 2))
                        {
                            if (othernpc.velocity.X < -2f) othernpc.velocity.X = othernpc.velocity.X * -0.95f;
                            else othernpc.velocity.X = 2f;
                        }
                    }
                    if (fallDamage > 0 && npc.velocity.Y > 2f && othernpc.Center.Y >= Center.Y + (Height / 3))
                    {
                        if (npc.AsV2NPC().FatassCrushingIFrames == 0)
                        {
                            npc.AsV2NPC().FatassCrushingIFrames = 60;
                            NPC.HitInfo hitinfo = new NPC.HitInfo();
                            hitinfo.Damage = (int)(fallDamage * (npc.velocity.Y - 1.5f));
                            othernpc.StrikeNPC(hitinfo, false, false);
                            NetMessage.SendStrikeNPC(othernpc, hitinfo);

                        }
                    }
                }
            }
            if (ModContent.GetInstance<V2ServerConfig>().FatAssesBreakTiles)
            {
                if (npc.AsPred().FloorBreakCounter >= 60)
                {
                    npc.AsPred().FloorBreakCounter = 0;
                    DamageTiles(npc, Hitbox, fallDamage);
                }
            }
        }
        public static void OnUpdate(NPC npc)
        {
            if (!IsAirborne(npc))
            {
                npc.AsPred().FloorBreakCounter++;
            }
            else
            {
                npc.AsPred().FloorBreakCounter = 0;
            }
        }
        public static bool IsAirborne(Player player)
        {
            return !Collision.SolidCollision(player.BottomLeft, player.width, 6);
        }
        public static bool IsAirborne(NPC player)
        {
            return !player.collideY;
        }
    }
=======
	public class FatFuckMethods
	{
		public static NetworkText FatassDeathReason(NPC npc, Player victim)
		{
			List<string> deathMessageKeyList =
			[
				"Mods.V2.Death.FlattenedByAFatAss.Universal.1",
				"Mods.V2.Death.FlattenedByAFatAss.Universal.2",
				"Mods.V2.Death.FlattenedByAFatAss.Universal.3",
				"Mods.V2.Death.FlattenedByAFatAss.Universal.4",
				"Mods.V2.Death.FlattenedByAFatAss.Universal.5",
				"Mods.V2.Death.FlattenedByAFatAss.Universal.6",
				"Mods.V2.Death.FlattenedByAFatAss.Universal.7",
			];
			string finalDeathReasonKey = Main.rand.NextFromCollection(deathMessageKeyList);

			return NetworkText.FromKey(
				finalDeathReasonKey,
				victim.name,
                npc.GivenOrTypeName
            );
		}
		public static void DamageTiles(NPC npc, Rectangle Hitbox, int power = 0)
		{
			if (power < 25) return;
			List<Point> tiles = Collision.GetTilesIn(Hitbox.BottomLeft() - new Vector2(-2, -2), Hitbox.BottomRight() + new Vector2(2, 10));
			double tileCount = 0;
			foreach (var point in tiles)
			{
				Tile tile = Framing.GetTileSafely(point);
				if (tile.HasTile)
				{
					tileCount++;
					if (tile.TileType == TileID.Platforms || tile.TileType == TileID.Glass) tileCount -= 0.33;
				}
			}
			if (tileCount == 0) return;
			int breakChance = Math.Max((int)(50 * (tileCount / 4)) - (power / 8), 0);
			if (breakChance > 200) return;
			foreach (var point in tiles)
			{
				Tile tile = Framing.GetTileSafely(point);
				int chance = Main.rand.Next(breakChance);
				if (chance == 0)
				{
					Tile tileBelow = Framing.GetTileSafely(point + new Point(0, 1));
					Tile extraTile = Framing.GetTileSafely(point + new Point(0, 2));
					if (tile.HasTile && tileBelow.HasTile && extraTile.HasTile) continue; //ground is too thick
					if (tileBelow.HasTile)
					{
						if (Main.rand.Next(breakChance) > breakChance / 5) continue;
					}
					WorldGen.KillTile(point.X, point.Y);
				}
				else if (chance <= 33)
				{
					Tile tileBelow = Framing.GetTileSafely(point + new Point(0, 1));
					Tile extraTile = Framing.GetTileSafely(point + new Point(0, 2));
					if (tile.HasTile && tileBelow.HasTile && extraTile.HasTile) continue; //ground is too thick
					if (tileBelow.HasTile)
					{
						if (Main.rand.Next(breakChance) > breakChance / 5) continue;
					}
					WorldGen.KillTile(point.X, point.Y, true, true);
				}
			}
		}
		public static void PushPlayers(NPC npc, int heightsize = 0, int heightoffset = 0, int fallDamage = 0)
		{
			Rectangle Hitbox = npc.Hitbox;
			Hitbox.Height += heightsize;
			Hitbox.Offset(0, -heightsize + heightoffset);
			fallDamage = (Hitbox.Height + Hitbox.Width) / 3;
			Vector2 Center = Hitbox.Center();
			int Width = Hitbox.Width;
			int Height = Hitbox.Height;
			foreach (var player in Main.ActivePlayers)
			{
				if (Hitbox.Intersects(player.Hitbox) && !player.dead)
				{
					if (player.Center.Y < Center.Y - (Height / 8) && player.Center.X < Center.X + (Width / 1.25) && player.Center.X > Center.X - (Width / 1.25))
					{
						player.RefreshMovementAbilities();
						if (player.velocity.Y > 2f && (player.controlJump || player.controlUp)) player.velocity.Y = player.velocity.Y * -1.2f;
						else if (player.velocity.Y > 2f) player.velocity.Y = player.velocity.Y * -0.93f;
						else player.velocity.Y = -2f;
					}
					else if (player.Center.Y >= Center.Y + (Height / 3) && player.Center.X < Center.X + (Width / 2.25) && player.Center.X > Center.X - (Width / 2.25))
					{
						player.position.Y = Center.Y + (Height / 2);
						if (player.velocity.Y < 2f) player.velocity.Y = player.velocity.Y * -0.95f;
						else player.velocity.Y = 2f;
					}
					if (player.Center.X < Center.X - 1 && player.Center.Y > Center.Y - (Height / 2) && player.Center.Y < Center.Y + (Height / 2))
					{
						if (player.velocity.X > 2f) player.velocity.X = player.velocity.X * -0.95f;
						else player.velocity.X = -2f;
					}
					else if (player.Center.X >= Center.X + 1 && player.Center.Y > Center.Y - (Height / 2) && player.Center.Y < Center.Y + (Height / 2))
					{
						if (player.velocity.X < -2f) player.velocity.X = player.velocity.X * -0.95f;
						else player.velocity.X = 2f;
					}
					if (fallDamage > 0 && npc.velocity.Y > 1.5f && player.Center.Y >= Center.Y + (Height / 3))
					{
						player.Hurt(PlayerDeathReason.ByCustomReason(FatassDeathReason(npc, player)), (int)(fallDamage * (npc.velocity.Y - 1.5f)), 0, false, false, -1, false);
					}
				}
			}
			foreach (var othernpc in Main.ActiveNPCs)
			{
				if (othernpc.whoAmI == npc.whoAmI) continue;
				if (Hitbox.Intersects(othernpc.Hitbox))
				{
					if (!othernpc.noTileCollide)
					{
						if (othernpc.Center.Y < Center.Y + (Height / 16) && othernpc.Center.X < Center.X + (Width / 1.25) && othernpc.Center.X > Center.X - (Width / 1.25))
						{
							if (othernpc.velocity.Y > 2f) othernpc.velocity.Y = othernpc.velocity.Y * -0.95f;
							else othernpc.velocity.Y = -2f;
						}
						else if (othernpc.Center.Y >= Center.Y + (Height / 3) && othernpc.Center.X < Center.X + (Width / 2.25) && othernpc.Center.X > Center.X - (Width / 2.25))
						{
							othernpc.position.Y = Center.Y + (Height / 2);
							if (othernpc.velocity.Y < 2f) othernpc.velocity.Y = othernpc.velocity.Y * -0.95f;
							else othernpc.velocity.Y = 2f;
						}
						if (othernpc.Center.X < Center.X - 1 && othernpc.Center.Y > Center.Y - (Height / 2) && othernpc.Center.Y < Center.Y + (Height / 2))
						{
							if (othernpc.velocity.X > 2f) othernpc.velocity.X = othernpc.velocity.X * -0.95f;
							else othernpc.velocity.X = -2f;
						}
						else if (othernpc.Center.X >= Center.X + 1 && othernpc.Center.Y > Center.Y - (Height / 2) && othernpc.Center.Y < Center.Y + (Height / 2))
						{
							if (othernpc.velocity.X < -2f) othernpc.velocity.X = othernpc.velocity.X * -0.95f;
							else othernpc.velocity.X = 2f;
						}
					}
					if (fallDamage > 0 && npc.velocity.Y > 2f && othernpc.Center.Y >= Center.Y + (Height / 3))
					{
						if (npc.AsV2NPC().FatassCrushingIFrames == 0)
						{
							npc.AsV2NPC().FatassCrushingIFrames = 60;
							NPC.HitInfo hitinfo = new NPC.HitInfo();
							hitinfo.Damage = (int)(fallDamage * (npc.velocity.Y - 1.5f));
							othernpc.StrikeNPC(hitinfo, false, false);
							NetMessage.SendStrikeNPC(othernpc, hitinfo);
						}
					}
				}
			}
			if (ModContent.GetInstance<V2ServerConfig>().FatAssesBreakTiles)
			{
				if (npc.AsPred().FloorBreakCounter >= 60)
				{
					npc.AsPred().FloorBreakCounter = 0;
					DamageTiles(npc, Hitbox, fallDamage);
				}
			}
		}
		public static void OnUpdate(NPC npc)
		{
			if (!IsAirborne(npc))
			{
				npc.AsPred().FloorBreakCounter++;
			}
			else
			{
				npc.AsPred().FloorBreakCounter = 0;
			}
		}
		public static bool IsAirborne(Player player)
		{
			return !Collision.SolidCollision(player.BottomLeft, player.width, 6);
		}
		public static bool IsAirborne(NPC player)
		{
			return !player.collideY;
		}
	}
>>>>>>> Stashed changes
}

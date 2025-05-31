using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.UI;
using V2.Items.Voraria.Armor;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using V2.Items.Voraria.Accessories.Transformations.Baelz;
using Microsoft.Xna.Framework;
using Terraria.ID;
using V2.Projectiles.Voraria.Weapons.Ranged;
using Terraria.WorldBuilding;
using V2.StatusEffects.Voraria.Buffs;
using Terraria.Audio;
using V2.Sounds.TransformationSounds.Baelz;
using ReLogic.Utilities;
using V2.StatusEffects.Voraria.Debuffs;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.PlayerHandling
{
	public partial class V2Player : ModPlayer
	{
		public List<DelegateGeneralItemDrawingUI> generalItemUIDrawMethods;

		public int GuideHelpText = 0;
        public bool ShroomNecklace { get; set; }
		public bool BeeTransformation { get; set; }
        public bool BaeTransformation { get; set; }
		public int isAtCrushingSpeed { get; set; }
        public int CrushingDamage { get; set; }
		public Vector2 GrappleLastSpeed { get; set; }
		public SlotId LastSound { get; set; }

        public int lastWidth = 20;

        public Dictionary<string, bool> LocationsVisited { get; set; }

		public override void Initialize()
		{
			ResetHealthRegenTime();
			ResetHealthRegenEffectList();

			GrappleLastSpeed = Vector2.Zero;

			LocationsVisited = [];
		}

		public override void ResetEffects()
		{
			generalItemUIDrawMethods = [];
			setBonusActive = false;
			setBonusShouldBeDisplayed = false;
			ShroomNecklace = false;
			BeeTransformation = false;
            BaeTransformation = false;

			if (Player.name.ToLower() == "baelz" || Player.name.ToLower() == "hakosbaelz" || Player.name.ToLower() == "hakos baelz" || Player.name.ToLower() == "baelzhakos" || Player.name.ToLower() == "baelz hakos")
			{
                BaeTransformation = true;
                Player.AddBuff(ModContent.BuffType<BaelzTransformation>(), V2Utils.SensibleTime(frames: 4));
            }

            if (Player.whoAmI != Main.myPlayer)
				return;

			if (Player.talkNPC != -1)
			{
				NPC npc = Player.TalkNPC;
				if (npc.CurrentCaptor() is not null)
					Main.CloseNPCChatOrSign();
			}
            ResetHealthRegenEffectList();
		}

        public override void ModifyLuck(ref float luck)
        {
            if (Player.armor[0].type == ModContent.ItemType<CloverHeadAccessories>()) luck += 0.3f;
            if (Player.armor[1].type == ModContent.ItemType<CloverSweater>()) luck += 0.1f;
            if (Player.armor[2].type == ModContent.ItemType<CloverStockings>()) luck += 0.1f;
        }

        public override void UpdateDead()
		{
			ResetHealthRegenTime();
			ResetHealthRegenEffectList();
		}
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Player.AsV2Player().BaeTransformation)
            {
                Dust.NewDustPerfect(Player.position, ModContent.DustType<DeadBaelz>(), new Vector2(Main.rand.Next(-100, 101) / 15f, Main.rand.Next(-100, -50) / 15f));
                if (LastSound.IsValid)
                {
                    if (SoundEngine.TryGetActiveSound(LastSound, out ActiveSound? snd))
                        snd.Stop();
                }
                SoundEngine.PlaySound(
                    BaelzSounds.BaelzDeath,
                    Player.TrueCenter()
                );
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
			if (Player.HasBuff<Trance>())
				modifiers.SourceDamage *= 2;
            if (Player.AsV2Player().BaeTransformation)
            {
                modifiers.DisableSound();
                LastSound = SoundEngine.PlaySound(
                    BaelzSounds.BaelzHurt,
                    Player.TrueCenter()
                );
            }

        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
		{
			if (Player.AsV2Player().isAtCrushingSpeed > 0)
			{
				hurtInfo.Cancelled = true;
				return;
			}
			ResetHealthRegenTime();
		}

		public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
		{
			ResetHealthRegenTime();
        }
        /*public bool SurfaceBelow()
		{
            List<Point> tiles = Collision.GetTilesIn(Player.Hitbox.BottomLeft() - new Vector2(2, -2), Player.Hitbox.BottomRight() + new Vector2(2, 10));
            foreach (var point in tiles)
            {
                Tile tile = Framing.GetTileSafely(point);
                if (tile.HasTile)
                {
                    if (Main.tileSolid)
                }
            }
        }*/
        public override void UpdateVisibleAccessories()
        {
            bool OnSelectScreen = Main.gameMenu;
            if (OnSelectScreen)
            {
                for (int i = 3; i < 10; i++)
                {
                    if (Player.armor[i].type == ModContent.ItemType<BaeTransformationItem>())
                    {
                        Player.AsV2Player().BaeTransformation = true;
                    }
                }
            }
        }
        public int FallingForce(Player player)
		{
			return (int)Math.Ceiling(player.velocity.Y * PreyData.GetPreySize(player) / 2);
        }
		public bool CheckForSolidGround(Player player)
		{
            List<Point> tiles = Collision.GetTilesIn(player.Hitbox.BottomLeft() - new Vector2(-2, -2), player.Hitbox.BottomRight() + new Vector2(2, 10));
			bool HasSolidTile = false;
            foreach (var point in tiles)
            {
                Tile tile = Framing.GetTileSafely(point);
                if (tile.HasTile)
                {
					if (Main.tileSolid[tile.TileType])
						HasSolidTile = true;
                    if (Main.tileSolidTop[tile.TileType])
                        HasSolidTile = true;
                }
            }
            return HasSolidTile;
        }
        public override void PostUpdateEquips()
        {
            if (Player.AsV2Player().BeeTransformation == true) Player.width = 12;
            else if (Player.AsV2Player().BaeTransformation == true)
            {
                ModContent.GetInstance<BecomeTheRatGirl>().TrySetCompletion(Player);
                switch (BaeTransformationItem.GetVisualWeightStage(Player))
                {
                    case 0 or 1:
                        Player.width = 18;
                        break;
					case 2:
                        Player.width = 20;
                        break;
                    case 3:
                        Player.width = 22;
                        break;
                    case 4:
                        Player.width = 26;
                        break;
                    case 5:
                        Player.width = 34;
                        break;
                    case 6:
                        Player.width = 40;
                        break;
                    case 7:
                        Player.width = 48;
                        break;
                }
            }
            else Player.width = 20;
			if (Player.width != lastWidth)
			{
				int difference = Player.width - lastWidth;
				Player.position.X -= difference / 2;
			}
			lastWidth = Player.width;
            if (Main.myPlayer == Player.whoAmI)
            {
                Player.AsV2Player().isAtCrushingSpeed = Math.Max(Player.AsV2Player().isAtCrushingSpeed - 1, 0);
                if (FallingForce(Player) > 30)
				{
                    Player.AsV2Player().isAtCrushingSpeed = 3;
                    Player.AsV2Player().CrushingDamage = FallingForce(Player);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + Player.velocity, Vector2.Zero, ModContent.ProjectileType<FallingHitbox>(), Player.AsV2Player().CrushingDamage, (int)Math.Ceiling(Math.Sqrt(Player.AsV2Player().CrushingDamage)), Main.myPlayer, Player.width, Player.height);
                }
                if (!Player.IsAirborne() && CheckForSolidGround(Player) && Player.AsV2Player().isAtCrushingSpeed > 0)
				{
					Player.AsV2Player().isAtCrushingSpeed = 0;
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Girthquake>(), Player.AsV2Player().CrushingDamage, (int)Math.Ceiling(Math.Sqrt(Player.AsV2Player().CrushingDamage)), Main.myPlayer, 0, 5f + (Player.AsV2Player().CrushingDamage / 10f));
                }
				if (Player.AsV2Player().isAtCrushingSpeed == 0)
				{
                    Player.AsV2Player().CrushingDamage = 0;
                }
            }
        }
        public override void PostUpdateMiscEffects()
		{
			HandleSittingAndSleepingHealthRegenEffect();

			void AddLocationVisitMark(string place)
			{
				if (LocationsVisited.ContainsKey(place))
					LocationsVisited[place] = true;
				else
					LocationsVisited.TryAdd(place, true);
			}

			if (Player.ZoneSkyHeight)
				AddLocationVisitMark("sky");
			if (Player.ZoneForest)
				AddLocationVisitMark("forest");
			if (Player.ZoneDirtLayerHeight)
				AddLocationVisitMark("underground");
			if (Player.ZoneRockLayerHeight)
				AddLocationVisitMark("cavern");
			if (Player.ZoneUnderworldHeight)
				AddLocationVisitMark("hell");
			if (Player.ZoneSnow && Player.ZoneOverworldHeight)
				AddLocationVisitMark("tundra");
			if (Player.ZoneSnow && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight))
				AddLocationVisitMark("underground_tundra");
			if (Player.ZoneDesert)
				AddLocationVisitMark("desert");
			if (Player.ZoneUndergroundDesert)
				AddLocationVisitMark("underground_desert");
			if (Player.ZoneCorrupt)
				AddLocationVisitMark("corruption");
			if (Player.ZoneCrimson)
				AddLocationVisitMark("crimson");
			if (Player.ZoneBeach)
				AddLocationVisitMark("beach");
			if (Player.ZoneJungle && Player.ZoneOverworldHeight)
				AddLocationVisitMark("jungle");
			if (Player.ZoneJungle && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight))
				AddLocationVisitMark("underground_jungle");
			if (Player.ZoneGraveyard)
				AddLocationVisitMark("graveyard");
			if (Player.ZoneGranite)
				AddLocationVisitMark("granite");
			if (Player.ZoneMarble)
				AddLocationVisitMark("marble");
			if (Player.ZoneMeteor)
				AddLocationVisitMark("meteorite");
			if (Player.ZoneDungeon)
				AddLocationVisitMark("dungeon");
			if (Player.ZoneLihzhardTemple)
				AddLocationVisitMark("temple");
			if (!Main.dayTime)
				AddLocationVisitMark("nighttime");
			if (Player.ZoneSandstorm)
				AddLocationVisitMark("sandstorm");
			if (Main.IsItAHappyWindyDay)
				AddLocationVisitMark("windy_day");
			if (Main.IsItRaining && (Player.ZoneOverworldHeight || Player.ZoneSkyHeight))
			{
				if (Player.ZoneSnow)
					AddLocationVisitMark("snowing");
				else if (Main.IsItStorming)
					AddLocationVisitMark("thunderstorm");
				else
					AddLocationVisitMark("raining");
			}
			if (Main.IsItAHappyWindyDay)
				AddLocationVisitMark("windy_day");
			if (Main.bloodMoon)
				AddLocationVisitMark("blood_moon");
			if (Main.eclipse)
				AddLocationVisitMark("eclipse");
		}
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            foreach (PlayerDrawLayer drawLayer in PlayerDrawLayerLoader.Layers)
            {
                if ((Player.AsV2Player().BeeTransformation == true || Player.AsV2Player().BaeTransformation == true) && !drawInfo.headOnlyRender)
                {
                    if ((drawLayer != PlayerDrawLayers.HeldItem && drawLayer != PlayerDrawLayers.Carpet && drawLayer != PlayerDrawLayers.Pulley && drawLayer != PlayerDrawLayers.ForbiddenSetRing
                        && drawLayer != PlayerDrawLayers.CaptureTheGem && drawLayer != PlayerDrawLayers.BeetleBuff && drawLayer != PlayerDrawLayers.ElectrifiedDebuffFront
                        && drawLayer != PlayerDrawLayers.ElectrifiedDebuffFront && drawLayer != PlayerDrawLayers.PortableStool && drawLayer != PlayerDrawLayers.SafemanSun
                        && drawLayer != PlayerDrawLayers.SolarShield && drawLayer != PlayerDrawLayers.WebbedDebuffBack && drawLayer != PlayerDrawLayers.FrozenOrWebbedDebuff
                        && drawLayer != PlayerDrawLayers.EyebrellaCloud && drawLayer != PlayerDrawLayers.FinchNest && drawLayer != PlayerDrawLayers.IceBarrier
                        && drawLayer != PlayerDrawLayers.MountBack && drawLayer != PlayerDrawLayers.MountFront) && drawLayer.Mod == null || Player.dead)
                        drawLayer.Hide();
                }
            }
        }

        public bool HasVisitedLocation(string place)
		{
			if (LocationsVisited.TryGetValue(place, out bool value))
				return value;
			
			LocationsVisited.TryAdd(place, false);
			return false;
		}
		public override void SaveData(TagCompound tag)
		{
			if (LocationsVisited?.Count > 0)
			{
				List<string> locationsVisited = [];
				foreach (KeyValuePair<string, bool> location in LocationsVisited)
				{
					if (location.Value)
						locationsVisited.Add(location.Key);
				}
				tag["visitedLocations"] = locationsVisited;
			}
		}
		public override void LoadData(TagCompound tag)
		{
			List<string> locationsVisited = [.. tag.GetList<string>("visitedLocations")];
			if (locationsVisited.Count <= 0)
				return;

			LocationsVisited = [];
			foreach (string location in locationsVisited)
			{
				LocationsVisited.Add(location, true);
			}
		}
	}
}

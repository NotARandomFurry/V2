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

namespace V2.PlayerHandling
{
	public partial class V2Player : ModPlayer
	{
		public List<DelegateGeneralItemDrawingUI> generalItemUIDrawMethods;

		public int GuideHelpText = 0;

		public Dictionary<string, bool> LocationsVisited { get; set; }

		public override void Initialize()
		{
			ResetHealthRegenTime();
			ResetHealthRegenEffectList();

			LocationsVisited = [];
		}

		public override void ResetEffects()
		{
			generalItemUIDrawMethods = [];
			setBonusActive = false;
			setBonusShouldBeDisplayed = false;

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

		public bool HasVisitedLocation(string place)
		{
			if (LocationsVisited.ContainsKey(place))
				return LocationsVisited[place];
			
			LocationsVisited.TryAdd(place, false);
			return false;
		}
	}
}

using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Enigma;
using V2.NPCs.Voraria.TownNPCs.Succubus;

namespace V2.NPCs.Vanilla.Rain
{
	public partial class FlyingFish : GlobalNPC
	{
		public static List<(TargetType, int, TargetPriorityLevel)> Diet
		{
			get
			{
				List<(TargetType, int, TargetPriorityLevel)> diet = [
					// yummy bugs!
					(TargetType.NPC, NPCID.Worm, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GoldWorm, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.EnchantedNightcrawler, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.WaterStrider, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GoldWaterStrider, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.Grasshopper, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GoldGrasshopper, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.BlackDragonfly, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.BlueDragonfly, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GreenDragonfly, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.OrangeDragonfly, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.RedDragonfly, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.YellowDragonfly, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GoldDragonfly, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.Butterfly, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.HellButterfly, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GoldButterfly, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.Grubby, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.Sluggy, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.Buggy, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.Scorpion, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.ScorpionBlack, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.LadyBug, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GoldLadyBug, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.MushiLadybug, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.Moth, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.Mothron, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.MothronSpawn, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.Snail, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GlowingSnail, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.MagmaSnail, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.SeaSnail, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GiantShelly, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GiantShelly2, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.EmpressButterfly, TargetPriorityLevel.Favorite),
					
					// yummy smaller fish!
					(TargetType.NPC, NPCID.Goldfish, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GoldfishWalker, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.CorruptGoldfish, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.CrimsonGoldfish, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GoldGoldfish, TargetPriorityLevel.High),
					(TargetType.NPC, NPCID.GoldGoldfishWalker, TargetPriorityLevel.VeryHigh),
					(TargetType.NPC, NPCID.Pupfish, TargetPriorityLevel.High),
					
					// Players, mainly to approach and be friendly
					(TargetType.Player, -1, TargetPriorityLevel.Neutral),
				];
				return diet;
			}
		}
		public static List<(TargetType, int)> PredsInDiet
		{
			get
			{
				List<(TargetType, int)> diet = [
					(TargetType.NPC, NPCID.EmpressButterfly),
				];
				return diet;
			}
		}

		public override void PostAI(NPC npc)
		{
			npc.DoContactGulpage(Diet, PredsInDiet);
		}
	}
}

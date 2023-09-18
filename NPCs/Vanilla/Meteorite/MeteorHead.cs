using BetterDialogue;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using V2.Core;
using V2.Items.Voraria.Charms;
using V2.NPCs.Voraria.TownNPCs.Succubus.ChatButtons;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Meteorite
{
	public class MeteorHead : GlobalNPC
	{
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.netID == NPCID.MeteorHead;
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPCID.MeteorHead] = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true };
		}

		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			pool[NPCID.MeteorHead] = 0f;
		}
	}
}

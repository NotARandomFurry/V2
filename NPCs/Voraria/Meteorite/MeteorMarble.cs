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

namespace V2.NPCs.Voraria.Meteorite
{
	public static class MeteorMarbleStuff
	{
		public static int BaseOrbitalCount {
			get {
				if (Main.masterMode)
					return 6;

				if (Main.expertMode)
					return 5;

				return 4;
			}
		}
	}

	public class MeteorMarble : ModNPC
	{
		public override string Texture => "V2/NPCs/Voraria/Meteorite/MeteorMarble_Core_Heat0";
		public override void SetStaticDefaults()
		{
			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
				Direction = -1
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}
		public override void SetDefaults()
		{
			NPC.friendly = false;
			NPC.width = 10;
			NPC.height = 10;
			NPC.aiStyle = -1;
			NPC.lifeMax = 90;
			NPC.damage = 15;
			NPC.defense = 5;
			NPC.knockBackResist = 0.5f;
			NPC.HitSound = SoundID.NPCHit1;

			NPC.AsPred().maxStomachCapacity = 0.4;

			NPC.AsPred().DigestionType = EntityDigestionType.Thermal;
			NPC.AsPred().GetDigestionTickRateMethod = GetDigestionTickRate;
			NPC.AsPred().GetDigestionTickDamageMethod = GetDigestionTickDamage;

			NPC.AsPred().GetDigestedPlayerAdditionalDeathMessagesMethod = GetDigestedPlayerAdditionalDeathMessages;

			NPC.AsPred().GetPreyAbsorptionRateMethod = GetPreyAbsorptionRate;

			NPC.AsPred().GetVisualBellySizeMethod = GetVisualBellySize;
		}

		public override void ModifyTypeName(ref string typeName) => typeName = "Meteor Marble";

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Meteor,
				new FlavorTextBestiaryInfoElement("Mods.V2.Bestiary.Meteorite.MeteorMarble"),
			});
		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.1",
				"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.2",
				"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.3",
				"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.4",
				"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.5",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Meteorite.MeteorMarble.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Meteorite.MeteorMarble.2",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Meteorite.MeteorMarble.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, Prey prey) => 10;

		public static double GetDigestionTickDamage(NPC npc, Prey prey) => 15;

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				seconds: 4
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				4
			);
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			
		}
	}
}

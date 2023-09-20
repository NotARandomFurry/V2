using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Consumables.Catchables;
using V2.NPCs.Vanilla.TownNPCs.PartyGirl;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Forest
{
	public static class PinkyStuff
	{
		public static Pinky AsPinky(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Pinky cottonCandySlime))
				throw new Exception("this instance of Pinky, supposedly, doesn't exist");

			return cottonCandySlime;
		}
	}

	public class Pinky : GlobalNPC
	{
		public static int DigestedHeal => 40;
		public static int EatenHappyLength => V2Utils.SensibleTime(seconds: 35);
		public static int DigestedRegenTime => V2Utils.SensibleTime(seconds: 15);

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.BlueSlime;

		public override void SetDefaults(NPC npc)
		{
			if (npc.netID != NPCID.Pinky)
				return;

			npc.catchItem = ModContent.ItemType<CaughtPinky>();

			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().Size = 0.065;
			npc.AsPred().stomachContents = new List<Prey>();
			npc.AsPred().stomachContentsQueue = new List<Prey>();
			npc.AsPred().MaxStomachCapacity = 0.4;

			npc.AsPred().CanBeForceFedMethod = CanCottonCandySlimeBeForceFed;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(1.3);
			npc.AsPred().SmallGulpThreshold = 0.00;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamageMethod = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRateMethod = GetDigestionTickRate;

			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRateMethod = GetPreyAbsorptionRate;

			npc.AsFood().OnKilledByDigestion += PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += OnKilledByDigestion_GrantPinkyGoal;
		}

		public override bool? CanBeCaughtBy(NPC npc, Item item, Player player) {
			if (npc.netID != NPCID.Pinky)
				return null;

			return true;
		}

		public static bool CanCottonCandySlimeBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SlimePred.1",
				"Mods.V2.Death.DigestedPlayer.SlimePred.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.Pinky.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.Pinky.2",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.Pinky.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, Prey prey) => 0.10;
		public static double GetDigestionTickDamage(NPC npc, Prey prey) => 1;
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 6,
				seconds: 40
			);
			return baseAbsorptionRate;
		}

		public static void OnKilledByDigestion_GrantPinkyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<EatPinky>().TrySetCompletion(predPlayer);
			}
		}
	}

	public class PinkyDigestingPlayerBuffs : ModPlayer
	{
		public override void PostUpdateBuffs()
		{
			if (Player.AsPred().stomachContents.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == "Pinky") is Prey pinkyPrey)
			{
				Player.AddStatus(BuffID.Sunflower, Pinky.EatenHappyLength);
				if (pinkyPrey.NoHealth)
					Player.AddStatus(BuffID.Regeneration, Pinky.DigestedRegenTime);
			}
		}
	}

	public class PinkyDigestingNPCBuffs : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public override void ResetEffects(NPC npc)
		{
			if (npc.AsPred().stomachContents.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == "Pinky") is Prey pinkyPrey)
			{
				npc.AddStatus(BuffID.Sunflower, Pinky.EatenHappyLength);
				if (pinkyPrey.NoHealth)
					npc.AddStatus(BuffID.Regeneration, Pinky.DigestedRegenTime);
			}
		}
	}
}

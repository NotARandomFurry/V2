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
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Cavern
{
	public static class NymphStuff
	{
		public static Nymph AsNymph(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Nymph cuteGirlLure))
				throw new Exception("this instance of a Nymph, supposedly, doesn't exist");

			return cuteGirlLure;
		}
	}

	public class Nymph : GlobalNPC
	{
		public static int DigestedHeal => 40;
		public static int EatenHappyLength => V2Utils.SensibleTime(seconds: 35);
		public static int DigestedRegenTime => V2Utils.SensibleTime(seconds: 15);

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Nymph;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsFood().Size = 1.04;
			npc.AsPred().stomachContents = new List<Prey>();
			npc.AsPred().stomachContentsQueue = new List<Prey>();
			npc.AsPred().MaxStomachCapacity = 5.5;

			npc.AsPred().CanBeForceFedMethod = CanNymphBeForceFed;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(4.7);
			npc.AsPred().SmallGulpThreshold = 0.35;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamageMethod = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRateMethod = GetDigestionTickRate;

			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRateMethod = GetPreyAbsorptionRate;

			npc.AsFood().OnKilledByDigestion += PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += OnKilledByDigestion_GrantNymphGoal;
		}

		public static bool CanNymphBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.1",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.2",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.3",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.4",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.5",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Cavern.Nymph.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Cavern.Nymph.2",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Cavern.Nymph.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, Prey prey) => 1.4;
		public static double GetDigestionTickDamage(NPC npc, Prey prey) => 22;
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 1,
				seconds: 15
			);
			return baseAbsorptionRate;
		}

		public static void OnKilledByDigestion_GrantNymphGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<EatNymph>().TrySetCompletion(predPlayer);
			}
		}
	}
}

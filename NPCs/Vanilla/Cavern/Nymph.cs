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
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type is NPCID.LostGirl or NPCID.Nymph;

		public override void SetDefaults(NPC entity)
		{
			entity.AsV2NPC().Gender = EntityGender.Female;

			entity.AsFood().Size = 1.04;
			entity.AsPred().MaxStomachCapacity = 5.5;

			entity.AsPred().CanBeForceFed += CanNymphBeForceFed;
			entity.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(4.7);
			entity.AsPred().SmallGulpThreshold = 0.35;

			entity.AsPred().DigestionType = EntityDigestionType.Acidic;
			entity.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			entity.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			entity.AsPred().SmallBurps = Burps.Humanoid.Small;
			entity.AsPred().StandardBurps = Burps.Humanoid.Standard;
			entity.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			entity.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			entity.AsFood().OnKilledByDigestion += PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			entity.AsFood().OnKilledByDigestion += OnKilledByDigestion_GrantNymphGoal;
		}

		public static bool CanNymphBeForceFed(NPC npc) => false;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Cavern.Nymph.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Cavern.Nymph.2",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Cavern.Nymph.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 1.4;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 22;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			SoundEngine.PlaySound(
				digestedPrey.WeightLeftToDigest < 0.6 ? npc.AsPred().SmallBurps : npc.AsPred().StandardBurps,
				npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f)
			);
		}

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

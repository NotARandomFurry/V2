using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.NPCGroupUtils;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.NPCs.Vanilla.Forest
{
	public static class BlueSlimeStuff
	{
		public static BlueSlime AsBlueSlime(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out BlueSlime blueSlime))
				throw new Exception("this instance of a Blue Slime, supposedly, doesn't exist");

			return blueSlime;
		}
	}

	public partial class BlueSlime : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.BlueSlime;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;
			npc.AsV2NPC().FirstFramePreAIMethod = V2BlueSlimeFirstFrameAI;
			npc.AsV2NPC().NewAIMethod = V2BlueSlimeAI;

			npc.AsSlime().JumpSpeed = new Vector2(3f, 3.5f);
			npc.AsSlime().JumpDelayBase = V2Utils.SensibleTime(
				seconds: 2,
				frames: 20
			);
			npc.AsSlime().JumpDelayExtra = (
				V2Utils.SensibleTime(
					frames: 10
				),
				V2Utils.SensibleTime(
					frames: 40
				)
			);

			npc.AsSlime().OccasionalHighJumps = false;

			npc.AsFood().DefinedBaseSize = 0.50;
			npc.AsPred().MaxStomachCapacity = 0.75;

			npc.AsPred().SmallGulpThreshold = 0.00;
			npc.AsPred().BigGulps = null;
			npc.AsPred().CanBeForceFed = CanBlueSlimeBeForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Other;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().StandardBurps = null;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
			npc.AsFood().OnKilledByDigestion += SlimeNPC.OnKilledByDigestion_GrantSlimeMultiPreyGoal;
		}

		public static bool CanBlueSlimeBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SlimePred.1",
				"Mods.V2.Death.DigestedPlayer.SlimePred.2",
				"Mods.V2.Death.DigestedPlayer.SlimePred.3",
			});
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.65;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			double baseDigestionTickDamage = 4.0;
			baseDigestionTickDamage *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseDigestionTickDamage;
		}
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 15
			);
			baseAbsorptionRate *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseAbsorptionRate;
		}
	}
}

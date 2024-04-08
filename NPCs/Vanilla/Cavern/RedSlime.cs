using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.NPCGroupUtils;

namespace V2.NPCs.Vanilla.Cavern
{
	public static class RedSlimeStuff
	{
		public static RedSlime AsRedSlime(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out RedSlime RedSlime))
				throw new Exception("this instance of a Red Slime, supposedly, doesn't exist");

			return RedSlime;
		}
	}

	public partial class RedSlime : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.BlueSlime;

		public override void SetDefaultsFromNetId(NPC npc)
		{
			if (npc.netID != NPCID.RedSlime)
				return;

			npc.AsV2NPC().Gender = EntityGender.Other;
			npc.AsV2NPC().FirstFramePreAIMethod = V2RedSlimeFirstFrameAI;
			npc.AsV2NPC().NewAIMethod = V2RedSlimeAI;

			npc.AsSlime().JumpSpeed = new Vector2(5f, 3.75f);
			npc.AsSlime().JumpDelayBase = V2Utils.SensibleTime(
				seconds: 1,
				frames: 30
			);
			npc.AsSlime().JumpDelayExtra = (
				V2Utils.SensibleTime(
					frames: 0
				),
				V2Utils.SensibleTime(
					seconds: 1,
					frames: 30
				)
			);

			npc.AsSlime().OccasionalHighJumps = true;
			npc.AsSlime().HighJumpFrequency = 2;
			npc.AsSlime().HighJumpXModifier += 0.5f;
			npc.AsSlime().HighJumpYModifier += 1f;

			npc.AsFood().DefinedBaseSize = 0.52;
			npc.AsPred().MaxStomachCapacity = 0.78;

			npc.AsPred().SmallGulpThreshold = 0.00;
			npc.AsPred().BigGulps = null;
			npc.AsPred().CanBeForceFed = CanRedSlimeBeForceFed;

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

		public static bool CanRedSlimeBeForceFed(NPC npc) => true;

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

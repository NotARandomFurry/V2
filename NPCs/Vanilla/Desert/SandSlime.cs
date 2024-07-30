using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.V2NPCGroupUtils;

namespace V2.NPCs.Vanilla.Desert
{
	public static class SandSlimeStuff
	{
		public static SandSlime AsSandSlime(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out SandSlime SandSlime))
				throw new Exception("this instance of a Sand Slime, supposedly, doesn't exist");

			return SandSlime;
		}
	}

	public partial class SandSlime : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.SandSlime;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;
			npc.AsV2NPC().FirstFramePreAIMethod = V2SandSlimeFirstFrameAI;
			npc.AsV2NPC().NewAIMethod = V2SandSlimeAI;

			npc.AsSlime().JumpSpeed = new Vector2(3f, 3f);
			npc.AsSlime().JumpDelayBase = V2Utils.SensibleTime(
				seconds: 0,
				frames: 10
			);
			npc.AsSlime().JumpDelayExtra = (
				V2Utils.SensibleTime(
					frames: 0
				),
				V2Utils.SensibleTime(
					frames: 10 
				)
			);

			npc.AsSlime().OccasionalHighJumps = true;
			npc.AsSlime().HighJumpFrequency = 6;
			npc.AsSlime().HighJumpXModifier += 1.5f;
			npc.AsSlime().HighJumpYModifier += 1.5f;

			npc.AsFood().DefinedBaseSize = 0.52;
			npc.AsPred().MaxStomachCapacity = 0.78;

			npc.AsPred().SmallGulpThreshold = 0.00;
			npc.AsPred().BigGulps = null;
			npc.AsPred().CanBeForceFed = CanSandSlimeBeForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Other;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().StandardBurps = null;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnDigestedBy += PreyNPC.HandlePreyItemTheft;
			npc.AsFood().OnDigestedBy += SlimeNPC.OnKilledByDigestion_GrantSlimeMultiPreyGoal;
		}

		public static bool CanSandSlimeBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SlimePred.1",
				"Mods.V2.Death.DigestedPlayer.SlimePred.2",
				"Mods.V2.Death.DigestedPlayer.SlimePred.3",
			});
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.8;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			double baseDigestionTickDamage = 8.0;
			baseDigestionTickDamage *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseDigestionTickDamage;
		}
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 10
			);
			baseAbsorptionRate *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseAbsorptionRate;
		}
	}
}

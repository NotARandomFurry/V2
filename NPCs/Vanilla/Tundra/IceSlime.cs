using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.GroupDefinitions;

namespace V2.NPCs.Vanilla.Tundra
{
	public static class IceSlimeStuff
	{
		public static IceSlime AsIceSlime(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out IceSlime IceSlime))
				throw new Exception("this instance of an Ice Slime, supposedly, doesn't exist");

			return IceSlime;
		}
	}

	public partial class IceSlime : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.IceSlime;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;
			npc.AsV2NPC().FirstFramePreAIMethod = V2IceSlimeFirstFrameAI;
			npc.AsV2NPC().NewAIMethod = V2IceSlimeAI;

			npc.AsSlime().JumpSpeed = new Vector2(0.75f, 5.5f);
			npc.AsSlime().JumpDelayBase = V2Utils.SensibleTime(
				seconds: 1,
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
			npc.AsSlime().HighJumpFrequency = 4;
			npc.AsSlime().HighJumpXModifier += 4f;
			npc.AsSlime().HighJumpYModifier *= 2f / 3f;

			npc.AsFood().DefinedBaseSize = 0.5;
			npc.AsPred().MaxStomachCapacity = 0.75;

			npc.AsPred().SmallGulpThreshold = 0.00;
			npc.AsPred().BigGulps = null;
			npc.AsPred().CanBeForceFed = CanIceSlimeBeForceFed;

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

		public static bool CanIceSlimeBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SlimePred.1",
				"Mods.V2.Death.DigestedPlayer.SlimePred.2",
				"Mods.V2.Death.DigestedPlayer.SlimePred.3",
			});
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.5;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			double baseDigestionTickDamage = 12.0;
			baseDigestionTickDamage *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseDigestionTickDamage;
		}
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 20
			);
			baseAbsorptionRate *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseAbsorptionRate;
		}
	}
}

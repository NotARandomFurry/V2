using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.NPCGroupUtils;

namespace V2.NPCs.Vanilla.Forest
{
	public static class GreenSlimeStuff
	{
		public static GreenSlime AsGreenSlime(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out GreenSlime GreenSlime))
				throw new Exception("this instance of a Green Slime, supposedly, doesn't exist");

			return GreenSlime;
		}
	}

	public partial class GreenSlime : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.BlueSlime;

		public override void SetDefaultsFromNetId(NPC npc)
		{
			if (npc.netID != NPCID.GreenSlime)
				return;

			npc.AsV2NPC().Gender = EntityGender.Other;
			npc.AsV2NPC().FirstFramePreAIMethod = V2GreenSlimeFirstFrameAI;
			npc.AsV2NPC().NewAIMethod = V2GreenSlimeAI;

			npc.AsSlime().JumpSpeed = new Vector2(3f, 2.5f);
			npc.AsSlime().JumpDelayBase = V2Utils.SensibleTime(
				seconds: 3
			);
			npc.AsSlime().JumpDelayExtra = (
				V2Utils.SensibleTime(
					frames: 30
				),
				V2Utils.SensibleTime(
					seconds: 1
				)
			);

			npc.AsSlime().OccasionalHighJumps = false;

			npc.AsFood().DefinedBaseSize = 0.45;
			npc.AsPred().MaxStomachCapacity = 0.675;

			npc.AsPred().SmallGulpThreshold = 0.00;
			npc.AsPred().BigGulps = null;
			npc.AsPred().CanBeForceFed = CanGreenSlimeBeForceFed;

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

		public static bool CanGreenSlimeBeForceFed(NPC npc) => true;

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

using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;

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

	public class BlueSlime : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.BlueSlime;

		public override void SetDefaultsFromNetId(NPC npc)
		{
			if (npc.netID != NPCID.BlueSlime)
				return;

			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedSize = 0.15;
			npc.AsPred().MaxStomachCapacity = 0.85;

			npc.AsPred().CanBeForceFed = CanBlueSlimeBeForceFed;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(1.5);
			npc.AsPred().SmallGulpThreshold = 0.00;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
		}

		public static bool CanBlueSlimeBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SlimePred.1",
				"Mods.V2.Death.DigestedPlayer.SlimePred.2",
			});
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.15;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 4;
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 15,
				seconds: 0
			);
			return baseAbsorptionRate;
		}
	}
}

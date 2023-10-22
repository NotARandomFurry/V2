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
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.BlueSlime;

		public override void SetDefaults(NPC entity)
		{
			if (entity.netID < 0) // if this isn't specifically a Blue Slime, don't run
				return;

			entity.AsV2NPC().Gender = EntityGender.Other;

			entity.AsFood().Size = 0.15;
			entity.AsPred().MaxStomachCapacity = 0.85;

			entity.AsPred().CanBeForceFed = CanBlueSlimeBeForceFed;
			entity.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(1.5);
			entity.AsPred().SmallGulpThreshold = 0.00;

			entity.AsPred().DigestionType = EntityDigestionType.Acidic;
			entity.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			entity.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			entity.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			entity.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			entity.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
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
				minutes: 6,
				seconds: 40
			);
			return baseAbsorptionRate;
		}
	}
}

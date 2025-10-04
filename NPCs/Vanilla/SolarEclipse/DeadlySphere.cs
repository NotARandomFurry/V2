using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;

namespace V2.NPCs.Vanilla.Eclipse
{
	public static class DeadlySphereStuff
	{
		public static DeadlySphere AsDeadlySphere(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out DeadlySphere DeadlySphere))
				throw new Exception("this instance of a Deadly Sphere, supposedly, doesn't exist");

			return DeadlySphere;
		}
	}

	public class DeadlySphere : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.DeadlySphere;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 0.3;
			npc.AsFood().CannotBeRegurgitated = true;

			npc.AsFood().OnSwallowDamage = (int)Math.Ceiling(npc.damage * 1.25f);
			npc.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.DeadlySphere";
		}
	}
}

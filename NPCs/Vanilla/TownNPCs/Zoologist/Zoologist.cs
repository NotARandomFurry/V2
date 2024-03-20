using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;

namespace V2.NPCs.Vanilla.TownNPCs.Zoologist
{
	public class Zoologist : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.BestiaryGirl && !V2.GetFooled;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class TungstenShortsword : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.TungstenShortsword;

		public override void SetDefaults(Item entity)
		{
			entity.AsFood().MaxHealth = 274;
			entity.AsFood().Size = 0.18;

			entity.AsTaggable().Shortsword = true;
		}
	}
}

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
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TungstenShortsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 274;
			item.AsFood().Size = 0.18;
			item.AsFood().AcidResistTier = 2;

			item.AsAnItem().StruggleDamageBaseMod = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
}

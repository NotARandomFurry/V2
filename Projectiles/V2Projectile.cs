using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.Core;

namespace V2.Projectiles
{
	public class V2Projectile : GlobalProjectile
	{
		public EntityGender Gender { get; set; }

		public delegate bool DelegateNewAI(Projectile projectile);
		public DelegateNewAI NewAIMethod { get; set; }

		public delegate List<string> DelegateGetChat(Projectile projectile, Player player);
		public DelegateGetChat GetChat { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => true;

		public V2Projectile()
		{
			Gender = EntityGender.Other;

			NewAIMethod = null;

			GetChat = null;
		}

	}
}

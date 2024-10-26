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
	public class GeneralProjectile : GlobalProjectile
	{
		public EntityGender Gender { get; set; }

		public SpriteAnimation CustomSprite { get; set; } = null;

		public delegate bool DelegateNewAI(Projectile projectile);
		public DelegateNewAI NewAIMethod { get; set; }

		public delegate List<string> DelegateGetChat(Projectile projectile, Player player);
		public DelegateGetChat GetChat { get; set; }

		public int Aggro { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => true;

		public GeneralProjectile()
		{
			Gender = EntityGender.Other;

			NewAIMethod = null;

			GetChat = null;

			Aggro = 0;
		}

	}
}

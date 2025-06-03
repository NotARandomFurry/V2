using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;

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
		public float GrappleStrength { get; set; }
		public float GrappleSpeed { get; set; }


		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => true;

		public GeneralProjectile()
		{
			Gender = EntityGender.Other;

			NewAIMethod = null;

			GetChat = null;

			Aggro = 0;
			GrappleStrength = 0;
			GrappleSpeed = 0;
		}

		public override bool PreDraw(Projectile projectile, ref Color lightColor)
		{
			if (projectile.CurrentCaptor() is not null)
				return false;

			if (projectile.AsV2Proj().CustomSprite is not null)
			{
				SpriteEffects spriteEffects = projectile.direction switch
				{
					-1 => SpriteEffects.FlipHorizontally,
					_ => SpriteEffects.None,
				};
				Texture2D texture = ModContent.Request<Texture2D>(projectile.AsV2Proj().CustomSprite.Texture, AssetRequestMode.ImmediateLoad).Value;
				Rectangle sourceRect = projectile.AsV2Proj().CustomSprite.DecideFrame() ?? texture.Bounds;
				Main.spriteBatch.Draw
				(
					texture,
					projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
					sourceRect,
					lightColor,
					projectile.rotation,
					sourceRect.Size() / 2f,
					1,
					spriteEffects,
					0f
				);
				return false;
			}
			return true;
		}
	}
}

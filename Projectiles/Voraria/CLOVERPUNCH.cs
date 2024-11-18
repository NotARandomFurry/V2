using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Projectiles.Voraria
{
	public class CLOVERPUNCH : ModProjectile
	{
		public float CollisionWidth => 36f * Projectile.scale;

		public int Timer
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(18);
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.scale = 1f;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 8;
			Projectile.hide = true;
			AIType = ProjectileID.Bullet;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			SoundEngine.PlaySound(SoundID.Item175, target.position);
		}
	}
}

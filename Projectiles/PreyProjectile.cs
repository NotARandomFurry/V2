using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling.PredPlayerGoals.Starter;
using V2.UI;

namespace V2.Projectiles
{
	public partial class PreyProjectile : GlobalProjectile
	{
		public int EatenSafetyFrames { get; set; }
		public bool Digested { get; set; }

		public double DefinedSize { get; set; }
		public double MaxHealth { get; set; }
		public double Health { get; set; }

		public delegate void DelegateOnSwallowedBy(Projectile projectile, Entity pred);
		public DelegateOnSwallowedBy OnSwallowedBy { get; set; }


		public delegate void DelegatePreyAI(Projectile projectile, Entity pred);
		public DelegatePreyAI SpecialPreyAI { get; set; }
		public int STR { get; set; }
		/// <summary>
		/// Expresses, from 0 to 12, how well this projectiles struggles.<br/>
		/// Defaults to 0.<br/>
		/// </summary>
		public int StruggleEffectiveness { get; set; }
		public StatModifier StruggleStrengthModifier { get; set; }
		public double StruggleStrength
		{
			get
			{
				double baseStruggleStrength = 1.5;
				baseStruggleStrength += 0.3 * STR;
				return StruggleStrengthModifier.ApplyTo((float)baseStruggleStrength);
			}
		}

		public delegate void DelegateOnKilledByDigestion(Projectile projectile, Entity pred);
		public DelegateOnKilledByDigestion OnKilledByDigestion { get; set; }

		public SoundStyle? DigestingHitSound;
		public SoundStyle? DigestedDeathSound;

		public StatModifier TakenDigestionDamageModifier { get; set; }

		public double SoftenedDigestionDamageTaken { get; set; }
		public StatModifier SoftenedDigestionDamageModifier { get; set; }
		public int SoftenedWearoffDelay { get; set; }
		public static int SoftenedWearoffMaxDelay => V2Utils.SensibleTime(seconds: 2, frames: 30);
		public StatModifier SoftenedWearoffRateModifier { get; set; }

		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => true;

		public PreyProjectile()
		{
			DefinedSize = 0.0;
			MaxHealth = -1;
			Health = -1;
			OnSwallowedBy = null;

			SpecialPreyAI = null;
			STR = 0;
			StruggleEffectiveness = 5;

			OnKilledByDigestion = null;

			DigestingHitSound = null;
			DigestedDeathSound = null;
		}

		public override bool PreKill(Projectile projectile, int timeLeft)
		{
			if (projectile.AsFood().Digested)
				return false;

			return true;
		}

		public override bool PreDraw(Projectile projectile, ref Color lightColor)
		{
			if (projectile.CurrentCaptor() is not null || projectile.AsFood().Digested)
				return false;

			return true;
		}

		public static void OnKilledByDigestion_GrantLivePreyGoal(Projectile projectile, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<FirstLivePrey>().TrySetCompletion(predPlayer);
			}
		}
	}
}

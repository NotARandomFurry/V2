using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using V2.Core;
using V2.Sounds.Vore;

namespace V2.Projectiles
{
	public class PredProjectile : GlobalProjectile
	{
		public EntityGender Gender { get; set; }

		public static VoreTracker GetStomachTracker(Projectile projectile)
		{
			if (Main.gameMenu)
				return null;

			return ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(x => x.Predator is Projectile predProjectile && predProjectile.whoAmI == projectile.whoAmI);
		}
		public EntityDigestionType DigestionType { get; set; }
		public double MaxStomachCapacity { get; set; }
		public float MaxSwallowRange { get; set; }
		public double ExtraWeight { get; set; }
		/// <summary>
		/// Allows this projectile to eat bosses despite not being a boss themselves.<br/>
		/// Defaults to false.<br/>
		/// </summary>
		public bool CanSwallowBosses { get; set; }

		public SoundStyle? SmallBurps { get; set; }
		public SoundStyle? StandardBurps { get; set; }
		public SoundStyle? BigBurps { get; set; }

		public SoundStyle SmallGulps { get; set; }
		public double SmallGulpThreshold { get; set; }
		public SoundStyle BigGulps { get; set; }
		public bool NonPreferenceBypass { get; set; }
		public delegate bool DelegateCanBeForceFed(Projectile projectile);
		public DelegateCanBeForceFed CanBeForceFed { get; set; }

		public delegate void DelegateOnForceFed(Projectile projectile, Player player);
		public DelegateOnForceFed OnForceFed { get; set; }


		public delegate double DelegateGetDigestionTickRate(Projectile projectile, PreyData prey);
		public DelegateGetDigestionTickRate GetDigestionTickRate { get; set; }

		public delegate double DelegateGetDigestionTickDamage(Projectile projectile, PreyData prey);
		public DelegateGetDigestionTickDamage GetDigestionTickDamage { get; set; }

		private double _stomachache;
		public double Stomachache
		{
			get => _stomachache;
			set => _stomachache = Math.Min(Math.Max(0, value), StomachacheMeterCapacity);
		}
		public double BaseStomachacheMeterCapacity { get; set; }
		public StatModifier StomachacheMeterCapacityModifier;
		public double StomachacheMeterCapacity
		{
			get
			{
				double baseStomachacheMeterCapacity = BaseStomachacheMeterCapacity;
				return StomachacheMeterCapacityModifier.ApplyTo((float)baseStomachacheMeterCapacity);
			}
		}
		/// <summary>
		/// Expresses, from 0 to 12, how well this projectile keeps up with struggles as a pred.<br/>
		/// Defaults to 5.<br/>
		/// </summary>
		public int CounterStruggleEffectiveness { get; set; }

		public delegate void DelegateOnDigestionKill(Projectile projectile, PreyData digestedPrey);
		public DelegateOnDigestionKill OnDigestionKill { get; set; }

		public delegate void DelegateGetDigestedPlayerAdditionalDeathMessages(Projectile projectile, Player player, List<string> deathMessageKeyList);
		public DelegateGetDigestedPlayerAdditionalDeathMessages GetAdditionalDigestedPlayerMessages { get; set; }

		public delegate double DelegateGetPreyAbsorptionRate(Projectile projectile);
		public DelegateGetPreyAbsorptionRate GetPreyAbsorptionRate { get; set; }

		public delegate int DelegateGetVisualBellySize(Projectile projectile);
		public DelegateGetVisualBellySize GetVisualBellySize { get; set; }

		public delegate int DelegateGetVisualWeightStage(Projectile projectile);
		public DelegateGetVisualWeightStage GetVisualWeightStage { get; set; }

		public SlotId ActiveStomachNoises { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => true;

		public PredProjectile()
		{
			MaxStomachCapacity = 1.0;
			MaxSwallowRange = 36f;
			ExtraWeight = 0.0;
			CanSwallowBosses = false;

			GetDigestionTickRate = null;
			GetDigestionTickDamage = null;
			GetPreyAbsorptionRate = null;

			NonPreferenceBypass = false;
			CanBeForceFed = (Projectile projectile) => false;
			OnForceFed = null;

			Stomachache = 0;
			BaseStomachacheMeterCapacity = 100.0;
			CounterStruggleEffectiveness = 5;

			SmallBurps = null;
			StandardBurps = null;
			BigBurps = null;

			SmallGulps = Gulps.Short;
			SmallGulpThreshold = 0.2;
			BigGulps = Gulps.Standard;

			OnDigestionKill = null;

			GetVisualBellySize = null;
			GetVisualWeightStage = null;
		}
	}
}

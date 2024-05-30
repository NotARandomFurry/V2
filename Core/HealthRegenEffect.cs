using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace V2.Core
{
	public delegate double DelegateHealthRegenPerSecond(Player player);
	public delegate void DelegateHealthRegenModifyHealthRegenTime(Player player, ref double healthRegenTime);
	public delegate void DelegateHealthRegenModifyTotalHealthRegen(Player player, ref double naturalRegenAdditive, ref double naturalRegenMultiplicative, ref double artificialRegenAdditive, ref double artificialRegenMultiplicative);
	public delegate void DelegateHealthRegenOnHealthAdjustment(Player player, int healthChangeAmount);
	public struct HealthRegenEffect
	{
		public DelegateHealthRegenPerSecond healthPerSecond;
		public bool natural;
		public DelegateHealthRegenModifyHealthRegenTime modifyHealthRegenTimeMethod;
		public DelegateHealthRegenModifyTotalHealthRegen modifyTotalHealthRegenMethod;
		public DelegateHealthRegenOnHealthAdjustment onHealthAdjustmentMethod;

		public HealthRegenEffect()
		{
			healthPerSecond = (player) => { return 0.0; };
			natural = false;
			modifyHealthRegenTimeMethod = null;
			modifyTotalHealthRegenMethod = null;
			onHealthAdjustmentMethod = null;
		}

		/// <summary>
		/// A health regeneration effect using VSC's revamped system.
		/// </summary>
		/// <param name="healthPerSecond">The amount of health to be regenerated or drained per second.</param>
		/// <param name="natural">Whether or not the effect should be counted as natural.</param>
		/// <param name="extraEffects">Any extra effects that this source of health regen has.</param>
		public HealthRegenEffect(DelegateHealthRegenPerSecond healthPerSecond, bool natural = false, DelegateHealthRegenModifyHealthRegenTime modifyHealthRegenTimeMethod = null, DelegateHealthRegenModifyTotalHealthRegen modifyTotalHealthRegenMethod = null, DelegateHealthRegenOnHealthAdjustment onHealthAdjustmentMethod = null)
		{
			this.healthPerSecond = healthPerSecond;
			this.natural = natural;
			this.modifyHealthRegenTimeMethod = modifyHealthRegenTimeMethod;
			this.modifyTotalHealthRegenMethod = modifyTotalHealthRegenMethod;
			this.onHealthAdjustmentMethod = onHealthAdjustmentMethod;
		}

		/// <summary>
		/// A health regeneration effect using VSC's revamped system. This overload provides ease-of-definition for static-intensity effects.
		/// </summary>
		/// <param name="healthPerSecond">The amount of health to be regenerated or drained per second.</param>
		/// <param name="natural">Whether or not the effect should be counted as natural.</param>
		/// <param name="extraEffects">Any extra effects that this source of health regen has.</param>
		public HealthRegenEffect(double healthPerSecond, bool natural = false, DelegateHealthRegenModifyHealthRegenTime modifyHealthRegenTimeMethod = null, DelegateHealthRegenModifyTotalHealthRegen modifyTotalHealthRegenMethod = null, DelegateHealthRegenOnHealthAdjustment onHealthAdjustmentMethod = null)
		{
			this.healthPerSecond = (player) => healthPerSecond;
			this.natural = natural;
			this.modifyHealthRegenTimeMethod = modifyHealthRegenTimeMethod;
			this.modifyTotalHealthRegenMethod = modifyTotalHealthRegenMethod;
			this.onHealthAdjustmentMethod = onHealthAdjustmentMethod;
		}
	}
}

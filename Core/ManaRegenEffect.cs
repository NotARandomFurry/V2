using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace V2.Core
{
	public delegate double DelegateManaRegenPerSecond(Player player);
	public delegate void DelegateManaRegenModifyManaRegenDelay(Player player, ref double manaRegenDelay);
	public delegate void DelegateManaRegenModifyTotalManaRegen(Player player, ref double naturalRegenAdditive, ref double naturalRegenMultiplicative, ref double artificialRegenAdditive, ref double artificialRegenMultiplicative);
	public delegate void DelegateManaRegenOnManaAdjustment(Player player, int manaChangeAmount);
	public struct ManaRegenEffect
	{
		public DelegateManaRegenPerSecond manaPerSecond;
		public bool natural;
		public DelegateManaRegenModifyManaRegenDelay modifyManaRegenDelayMethod;
		public DelegateManaRegenModifyTotalManaRegen modifyTotalManaRegenMethod;
		public DelegateManaRegenOnManaAdjustment onManaAdjustmentMethod;

		public ManaRegenEffect()
		{
			manaPerSecond = (player) => { return 0.0; };
			natural = false;
			modifyManaRegenDelayMethod = null;
			modifyTotalManaRegenMethod = null;
			onManaAdjustmentMethod = null;
		}

		/// <summary>
		/// A mana regeneration effect using VSC's revamped system.
		/// </summary>
		/// <param name="manaPerSecond">The amount of mana to be regenerated or drained per second.</param>
		/// <param name="natural">Whether or not the effect should be counted as natural.</param>
		public ManaRegenEffect(DelegateManaRegenPerSecond manaPerSecond, bool natural = false, DelegateManaRegenModifyManaRegenDelay modifyManaRegenDelayMethod = null, DelegateManaRegenModifyTotalManaRegen modifyTotalManaRegenMethod = null, DelegateManaRegenOnManaAdjustment onManaAdjustmentMethod = null)
		{
			this.manaPerSecond = manaPerSecond;
			this.natural = natural;
			this.modifyManaRegenDelayMethod = modifyManaRegenDelayMethod;
			this.modifyTotalManaRegenMethod = modifyTotalManaRegenMethod;
			this.onManaAdjustmentMethod = onManaAdjustmentMethod;
		}

		/// <summary>
		/// A mana regeneration effect using VSC's revamped system. This overload provides ease-of-definition for static-intensity effects.
		/// </summary>
		/// <param name="manaPerSecond">The amount of mana to be regenerated or drained per second.</param>
		/// <param name="natural">Whether or not the effect should be counted as natural.</param>
		public ManaRegenEffect(double manaPerSecond, bool natural = false, DelegateManaRegenModifyManaRegenDelay modifyManaRegenDelayMethod = null, DelegateManaRegenModifyTotalManaRegen modifyTotalManaRegenMethod = null, DelegateManaRegenOnManaAdjustment onManaAdjustmentMethod = null)
		{
			this.manaPerSecond = (player) => manaPerSecond;
			this.natural = natural;
			this.modifyManaRegenDelayMethod = modifyManaRegenDelayMethod;
			this.modifyTotalManaRegenMethod = modifyTotalManaRegenMethod;
			this.onManaAdjustmentMethod = onManaAdjustmentMethod;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace V2.Items.Voraria.Charms
{
	public static class CharmHelpers
	{
		public static CharmGlobalItem AsCharm(this Item item) => item.GetGlobalItem<CharmGlobalItem>();
	}

	public class CharmGlobalItem : GlobalItem
	{
		/// <summary>
		/// Whether or not this item is valid for use in charm slots.<br/>
		/// Defaults to <see langword="false"/>.
		/// </summary>
		public bool IsValidCharm { get; set; } = false;

		public delegate void DelegateCharmEffects(Player player);
		/// <summary>
		/// What this item should do when placed in a charm slot.<br/>
		/// If <see cref="IsValidCharm"/> is set to <see langword="true"/>, this is assumed to NOT be <see langword="null"/>.<br/>
		/// As such, ALWAYS ensure this is filled with a delegate when <see cref="IsValidCharm"/> is <see langword="true"/>!<br/>
		/// </summary>
		public DelegateCharmEffects CharmEffects { get; set; } = null;

		public override bool InstancePerEntity => true;

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			if (item.AsCharm().IsValidCharm)
				item.AsCharm().CharmEffects.Invoke(player);
		}
	}
}

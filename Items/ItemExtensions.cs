using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace V2.Items
{
	public static class ItemExtensions
	{
		/// <summary>
		/// Fetches the current item's attached DDItem instance, allowing for access to DD-specific item fields.
		/// </summary>
		/// <param name="item">The item to fetch the attached DDItem instance for.</param>
		/// <returns>The DDItem instance on the current item, if it has one; otherwise, null.</returns>
		public static V2Item AsV2Item(this Item item)
		{
			if (item.IsAir)
				return null;

			bool appliedAsV2Item = item.TryGetGlobalItem(out V2Item result);
			if (appliedAsV2Item)
				return result;
			else
				return null;
		}

	}
}

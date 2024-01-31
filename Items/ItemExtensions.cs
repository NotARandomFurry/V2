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
		/// Fetches the current item's attached V2Item instance, allowing for access to Voraria-specific item fields.
		/// </summary>
		/// <param name="item">The item to fetch the attached V2Item instance for.</param>
		/// <returns>The V2Item instance on the current item, if it has one; otherwise, null.</returns>
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

		/// <summary>
		/// Fetches the current item's attached TaggableItem instance, allowing for access to Voraria's item tag system.
		/// </summary>
		/// <param name="item">The item to fetch the attached TaggableItem instance for.</param>
		/// <returns>The TaggableItem instance on the current item, if it has one; otherwise, null.</returns>
		public static TaggableItem AsTaggable(this Item item)
		{
			if (item.IsAir)
				return null;

			bool appliedAsTaggableItem = item.TryGetGlobalItem(out TaggableItem result);
			if (appliedAsTaggableItem)
				return result;
			else
				return null;
		}
	}
}

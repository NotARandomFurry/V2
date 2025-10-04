using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Items.Voraria.Weapons.Summon;

namespace V2.PlayerHandling.PredPlayerGoals.Amateur
{
	public class EatFungalGift : PredPlayerGoal
	{
		public override string InternalName => "EatFungalGift";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatFungalGift.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.EatFungalGift.Description";
		public override bool Available(Player pred) => pred.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<ShroomStaff>()) || Complete(pred);
		public override int StatPointsFromCompletion => 11;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

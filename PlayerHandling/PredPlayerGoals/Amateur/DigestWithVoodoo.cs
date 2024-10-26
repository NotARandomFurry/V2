using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.PlayerHandling.PredPlayerGoals.Amateur
{
	public class DigestWithVoodoo : PredPlayerGoal
	{
		public override string InternalName => "DigestWithVoodoo";
		public override string DisplayName(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.DigestWithVoodoo.Name";
		public override string Description(Player pred) => "Mods.V2.PredPlayerGoals.Amateur.DigestWithVoodoo.Description";
		public override bool HasClearDescription(Player pred) => true;
		public override bool Available(Player pred) =>
			pred.AsV2Player().HasVisitedLocation("hell")
		 || pred.HasItemInInventoryOrOpenVoidBag(ItemID.GuideVoodooDoll)
		 || pred.AsV2Player().HasVisitedLocation("dungeon")
		 || pred.HasItemInInventoryOrOpenVoidBag(ItemID.ClothierVoodooDoll)
		 || Complete(pred);

		public override int StatPointsFromCompletion => 10;

		public override ProgressionStage Stage => ModContent.GetInstance<AmateurStage>();
	}
}

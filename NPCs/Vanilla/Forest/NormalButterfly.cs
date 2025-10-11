using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Consumables.Catchables;
using V2.NPCs.Sets;
using V2.NPCs.Vanilla.TownNPCs.PartyGirl;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.Forest
{
	public static class NormalButterflyStuff
	{
		public static NormalButterfly AsANormalButterfly(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out NormalButterfly butterflyWithAStomach))
				throw new Exception("this instance of a standard-fare butterfly, supposedly, doesn't exist");

			return butterflyWithAStomach;
		}

		public enum VanillaButterflySpecies
		{
			Monarch,
			PurpleEmperor,
			RedAdmiral,
			Ulysses,
			Sulphur,
			TreeNymph,
			ZebraSwallowtail,
			Julia,
		}
	}

	public partial class NormalButterfly : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Butterfly;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;
			npc.AsV2NPC().NewAIMethod = V2NormalButterflyAI;

			npc.AsFood().DefinedBaseSize = 0.04;
			npc.AsPred().WeightGainRatio = 0.04;
			npc.AsPred().MaxStomachCapacity = 1.10;
			npc.AsFood().WellFedPower = 0.05;
			npc.AsFood().CalorieMultiplier = 0.50;

			npc.AsPred().SmallGulpThreshold = 0.00;
			npc.AsPred().BigGulps = null;
			npc.AsPred().CanBeForceFed = CanNormalButterfliesBeForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			npc.AsFood().OnSwallowedBy += Butterfly.OnSwallowedBy_GrantButterflyGroupMultiPreyGoal;
		}

		public static bool CanNormalButterfliesBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange([

			]);
/*			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.NormalButterfly.Hardcore");
			}
*/		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.40;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 5.5;

		public static double GetPreyAbsorptionRate(NPC npc) =>  1.0 / (double)V2Utils.SensibleTime(
			minutes: 12
		);

		public static int GetVisualWeightStage(NPC npc) => Math.Min(
			(int)Math.Floor(10.0 * Math.Sqrt(npc.AsPred().ExtraWeight)),
			2
		);

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			string tastyButterflySpeciesString = npc.ai[2] switch
			{
				
			};
			string tastyLightButterflyTummyTypeString = "_MainSheet";
			string tastyLightButterflyWeightString = GetVisualWeightStage(npc) > 0 ? ("_WeightGain" + GetVisualWeightStage(npc)) : "_BaseWeight";
			string exactMainBodyTextureString = "V2/NPCs/Vanilla/Hallow/PrismaticLacewing" + tastyLightButterflyWeightString + tastyLightButterflyTummyTypeString;
			return false;
		}
	}
}

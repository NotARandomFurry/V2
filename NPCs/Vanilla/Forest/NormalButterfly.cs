using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Sets;

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
			npc.AsPred().BaseStomachacheMeterCapacity = 100.0;
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

		/// <summary>
		/// Gets the current weight gain stage for a normal butterfly.<br/>
		/// </summary>
		/// <param name="npc">The butterfly to check up on the fattiness of.</param>
		/// <returns>The stage of weight the butterfly has gained.</returns>
		public static int GetVisualWeightStage(NPC npc) => Math.Min(
			(int)Math.Floor(5.0 * Math.Sqrt(npc.AsPred().ExtraWeight)),
			2
		);

		/// <summary>
		/// Sets the weight gain stage for a normal butterfly.<br/>
		/// <b>NOTE:</b> This is done via forcibly setting <see cref="PredNPC.ExtraWeight"/> to the lowest amount that achieves the desired weight gain stage.<br/>
		/// As such, <b>it should only be used when spawning the butterfly, such as when using one of the fattened butterfly items added by VSC.</b><br/>
		/// </summary>
		/// <param name="npc">The butterfly to forcibly fatten up.</param>
		/// <param name="weightGainTarget">The target stage of weight gain the butterfly in question should reach.</param>
		public static void SetVisualWeightStage(NPC npc, int weightGainTarget) => npc.AsPred().ExtraWeight = Math.Pow((double)weightGainTarget / 5.0, 2.0);

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			string tastyButterflySpeciesString = npc.ai[2] switch
			{
				(int)NormalButterflyStuff.VanillaButterflySpecies.Monarch => "_Monarch",
				(int)NormalButterflyStuff.VanillaButterflySpecies.PurpleEmperor => "_PurpleEmperor",
				(int)NormalButterflyStuff.VanillaButterflySpecies.RedAdmiral => "_RedAdmiral",
				(int)NormalButterflyStuff.VanillaButterflySpecies.Ulysses => "_Ulysses",
				(int)NormalButterflyStuff.VanillaButterflySpecies.Sulphur => "_Sulphur",
				(int)NormalButterflyStuff.VanillaButterflySpecies.TreeNymph => "_TreeNymph",
				(int)NormalButterflyStuff.VanillaButterflySpecies.ZebraSwallowtail => "_ZebraSwallowtail",
				(int)NormalButterflyStuff.VanillaButterflySpecies.Julia => "_Julia",
				_ => "_Monarch",
			};
			string tastyButterflyTummyTypeString = "_MainSheet";
			string tastyButterflyWeightString = GetVisualWeightStage(npc) > 0 ? ("_WeightGain" + GetVisualWeightStage(npc)) : "_BaseWeight";
			string exactMainBodyTextureString = "V2/NPCs/Vanilla/Forest/Butterfly" + tastyButterflySpeciesString + tastyButterflyWeightString + tastyButterflyTummyTypeString;
			return false;
		}
	}
}

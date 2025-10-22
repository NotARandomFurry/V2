using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Sets;
using V2.NPCs.Vanilla.BloodMoon;

namespace V2.NPCs.Vanilla.Forest
{
	public static partial class NormalButterflyStuff
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
		public double FrameCounter { get; set; }
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Butterfly;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;
			npc.AsV2NPC().NewAIMethod = V2NormalButterflyAI;

			npc.AsFood().DefinedBaseSize = 0.04;
			npc.AsPred().WeightGainRatio = 0.40;
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
			npc.AsPred().AssociatedStruggleChart = new NormalButterflyStuff.NormalButterflyStruggleChart();

			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
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

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.55;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 9.5;

		public static double GetPreyAbsorptionRate(NPC npc) =>  1.0 / (double)V2Utils.SensibleTime(
			minutes: 2,
			seconds: 30
		);

		/// <summary>
		/// Gets the current weight gain stage for a normal butterfly.<br/>
		/// </summary>
		/// <param name="npc">The butterfly to check up on the fattiness of.</param>
		/// <returns>The stage of weight the butterfly has gained.</returns>
		public static int GetVisualBellySize(NPC npc) => Math.Min(
			(int)Math.Floor(5.0 * Math.Sqrt(2.0) * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
			4
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

		public override void FindFrame(NPC npc, int frameHeight)
		{
			int yOffsetFactor = 50;
			int frameDelay = 7;
			npc.rotation = npc.velocity.X * 0.3105f;
			npc.spriteDirection = npc.direction;
			npc.AsANormalButterfly().FrameCounter++;
			if (npc.AsANormalButterfly().FrameCounter < (double)frameDelay)
			{
				npc.frame.Y = 0;
			}
			else if (npc.AsANormalButterfly().FrameCounter < (double)(frameDelay * 2))
			{
				npc.frame.Y = yOffsetFactor;
			}
			else if (npc.AsANormalButterfly().FrameCounter < (double)(frameDelay * 3))
			{
				npc.frame.Y = yOffsetFactor * 2;
			}
			else
			{
				npc.frame.Y = yOffsetFactor;
				if (npc.AsANormalButterfly().FrameCounter >= (double)(frameDelay * 4 - 1))
					npc.AsANormalButterfly().FrameCounter = 0.0;
			}

			switch (npc.AsPred().GetVisualBellySize?.Invoke(npc))
			{
				case 0:
				default:
					npc.frame.X = 0;
					npc.frame.Width = 22;
					npc.frame.Height = 20;
					break;
				case 1:
					npc.frame.X = 24;
					npc.frame.Width = 22;
					npc.frame.Height = 22;
					break;
				case 2:
					npc.frame.X = 48;
					npc.frame.Width = 22;
					npc.frame.Height = 28;
					break;
				case 3:
					npc.frame.X = 72;
					npc.frame.Width = 26;
					npc.frame.Height = 38;
					break;
				case 4:
					npc.frame.X = 102;
					npc.frame.Width = 30;
					npc.frame.Height = 48;
					break;
			}
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			SpriteEffects spriteEffects = npc.direction != 1 ? 0 : SpriteEffects.FlipHorizontally;
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
			string tastyButterflyWeightString = GetVisualWeightStage(npc) > 0 ? ("_WeightGain" + GetVisualWeightStage(npc)) : "_BaseWeight";
			string tastyButterflyTummyTypeString = "_MainSheet";
			string exactMainBodyTextureString = "V2/NPCs/Vanilla/Forest/Butterfly" + tastyButterflySpeciesString + tastyButterflyWeightString + tastyButterflyTummyTypeString;
			Texture2D exactButterflyTexture = ModContent.Request<Texture2D>(exactMainBodyTextureString, AssetRequestMode.ImmediateLoad).Value;
			Vector2 origin = npc.AsPred().GetVisualBellySize.Invoke(npc) switch
			{
				0 => new Vector2(11, 11),
				1 => new Vector2(11, 11),
				2 => new Vector2(11, 11),
				3 => new Vector2(13, 11),
				4 => new Vector2(17, 11),
				_ => new Vector2(11, 11),
			};
			spriteBatch.Draw(
				exactButterflyTexture,
				npc.Center - Main.screenPosition,
				npc.frame,
				drawColor,
				npc.rotation,
				origin,
				1f,
				spriteEffects,
				0f
			);
			return false;
		}
	}
}

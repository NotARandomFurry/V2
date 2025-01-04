using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.TownNPCs.Painter
{
	public static class PainterStuff
	{
		public static class ItemTheftRules
		{
			public static ItemTheftRule PaintballGun => new ItemTheftRule(
				type: (npc, pred) => ItemID.PainterPaintballGun,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => 1.0
			);
			public static ItemTheftRule JimsCap => new ItemTheftRule(
				type: (npc, pred) => ItemID.JimsCap,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => npc.GivenName == "Jim" ? 1.0 : 0.0
			);
		}

		public static Painter AsPainter(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Painter predPainter))
				throw new Exception("this instance of the Painter can't be pred or prey");

			return predPainter;
		}
		public static PainterProfile PredPainterProfile => new PainterProfile();
	}

	public class PainterProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public PainterProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/Painter/Painter_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/Painter/Painter";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			if (npc.altTexture == 1)
				exactTextureToUse += "_Party";

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.Painter;
	}

	public partial class Painter : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Painter;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Male;

			npc.AsV2NPC().GetNewDialogue = GetPainterChat;

			npc.AsFood().DefinedBaseSize = 0.988;
			npc.AsPred().WeightGainRatio = 0.095;
			npc.AsPred().MaxStomachCapacity = 2.94;
			npc.AsPred().BaseStomachacheMeterCapacity = 240.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.285;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().CanBeForceFed = CanPainterBeForceFed;
			npc.AsPred().OnForceFed = OnPainterForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			npc.AsPred().OnDigestionKill = null;
			npc.AsPred().MouthSoundRawOffset = npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f);
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 0.15;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => PainterStuff.PredPainterProfile;

		public static bool CanPainterBeForceFed(NPC npc) => true;

		public static void OnPainterForceFed(NPC npc, Player player)
		{
			PredNPC.SetChatboxText(
				npc,
				player,
				"[c/7F7F7F:<" + npc.GivenName + " seems overtly startled as you suddenly force yourself into his gullet, swiftly swallowing you down so as to keep you from getting in the way too long.>]\n"
			  + "[c/00BF00:*EEEOOOUUURP!*]\n"
			  + "I mean, if you REALLY wanna be food for a simple painter boy that bad...I guess I can't say no! Once you get back...well, IF you get back...be sure to stop by! I'm sure I'll have painted a great picture of the meal you gave me!"
			);
		}


		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Painter.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Painter.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Painter.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Painter.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Painter.5",
			});

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Painter.Hardcore");
			}
		}

		public override void PostAI(NPC npc)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			if (Main.GameUpdateCount % 60 != 0)
				return;

			static void RollForRandomGulp(ref bool gulp) => gulp |= Main.rand.NextBool(4, 200);

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			bool snackOnSalad = false;
			RollForRandomGulp(ref snackOnSalad);
			RollForRandomGulp(ref snackOnSalad);
			RollForRandomGulp(ref snackOnSalad);
			if (salad != null && salad.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && snackOnSalad)
				PredNPC.Swallow(npc, salad);
			NPC helloNurse = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Nurse);
			bool snackOnNurse = false;
			RollForRandomGulp(ref snackOnNurse);
			RollForRandomGulp(ref snackOnNurse);
			if (helloNurse != null && helloNurse.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && snackOnNurse)
				PredNPC.Swallow(npc, helloNurse);
			NPC electricityFan = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Mechanic);
			bool snackOnElectricityFan = false;
			RollForRandomGulp(ref snackOnElectricityFan);
			if (electricityFan != null && electricityFan.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && snackOnElectricityFan)
				PredNPC.Swallow(npc, electricityFan);
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			bool snackOnBestGirl = false;
			RollForRandomGulp(ref snackOnBestGirl);
			if (bestGirl != null && bestGirl.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && snackOnBestGirl)
				PredNPC.Swallow(npc, bestGirl);
			NPC suspiciouslyPersonShapedCake = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.PartyGirl);
			bool snackOnPersonShapedCake = false;
			RollForRandomGulp(ref snackOnPersonShapedCake);
			RollForRandomGulp(ref snackOnPersonShapedCake);
			if (suspiciouslyPersonShapedCake != null && suspiciouslyPersonShapedCake.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && snackOnPersonShapedCake)
				PredNPC.Swallow(npc, suspiciouslyPersonShapedCake);
			NPC steamEnjoyer = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Steampunker);
			bool snackOnSteamEnjoyer = false;
			RollForRandomGulp(ref snackOnSteamEnjoyer);
			if (steamEnjoyer != null && steamEnjoyer.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && snackOnSteamEnjoyer)
				PredNPC.Swallow(npc, steamEnjoyer);
			NPC foxBimbo = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.BestiaryGirl);
			bool snackOnFoxBimbo = false;
			RollForRandomGulp(ref snackOnFoxBimbo);
			RollForRandomGulp(ref snackOnFoxBimbo);
			if (foxBimbo != null && foxBimbo.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && snackOnFoxBimbo)
				PredNPC.Swallow(npc, foxBimbo);

			if (!ModContent.GetInstance<V2ServerConfig>().RandomGulpsAgainstPlayers)
				return;

			if (!Main.CurrentPlayer.active || Main.CurrentPlayer.dead || Main.CurrentPlayer.Distance(npc.Center) > npc.AsPred().MaxSwallowRange || Main.CurrentPlayer.CurrentCaptor() is not null)
				return;

			bool decideToHuntPlayer = false;
			RollForRandomGulp(ref decideToHuntPlayer);

			if (Main.netMode != NetmodeID.Server && Main.CurrentPlayer.whoAmI == Main.myPlayer && Main.CurrentPlayer.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && decideToHuntPlayer)
			{
				List<string> potentialRandomGulpLines = new List<string>
				{
					"Sorry! You just looked so tasty, and I needed a good meal to help me focus on my art! Hope you don't mind bein' artist food for a little while...!",
					"Ahh, that's better~! My belly was grumbling and gurgling to get at you a lot...I couldn't help it! I'll paint a SUPER nice picture of you as my lunch, I swear!",
				};
				PredNPC.SwallowWithTextIfApplicable(
					npc,
					Main.CurrentPlayer,
					"[c/7F7F7F:<An ominous gurgle rings out from " + npc.GivenName + "'s belly as he suddenly stuffs you into his maw, hastily swallowing you while trying not to knock over his art supplies.>]\n"
				  + Main.rand.NextFromCollection(potentialRandomGulpLines)
				);
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => Main.bloodMoon ? 1.2 : 0.8;

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 21.4;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 7,
				seconds: 0
			);
			return baseAbsorptionRate;
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
			npc.frame.Width = 160;
		}

		public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
		{
			boundingBox = new Rectangle(
				(int)npc.Center.X - 18,
				(int)npc.Center.Y - 27,
				36,
				54
			);
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				5
			);
		}
	}
}

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

namespace V2.NPCs.Vanilla.TownNPCs.Guide
{
	public static class GuideStuff
	{
		public static class ItemTheftRules
		{
			public static ItemTheftRule GreenCap => new ItemTheftRule(
				type: (npc, pred) => ItemID.GreenCap,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => npc.GivenName == "Andrew" ? 1.0 : 0.0
			);
		}

		public static Guide AsGuide(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Guide predGuide))
				throw new Exception("this instance of the Guide can't be pred or prey");

			return predGuide;
		}
		public static GuideProfile PredGuideProfile => new GuideProfile();
	}

	public class GuideProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public GuideProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/Guide/Guide_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/Guide/Guide";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.Guide;
	}

	public class Guide : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Guide;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Male;

			npc.AsV2NPC().GetNewDialogue = GetGuideChat;

			npc.AsFood().DefinedBaseSize = 1.0;
			npc.AsPred().MaxStomachCapacity = 1.20;
			npc.AsPred().BaseStomachacheMeterCapacity = 100.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.25;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().CanBeForceFed = CanGuideBeForceFed;
			npc.AsPred().OnForceFed = OnGuideForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			npc.AsPred().OnDigestionKill = null;
			npc.AsPred().MouthSoundRawOffset = npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f);
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 0.25;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => GuideStuff.PredGuideProfile;

		public static List<string> GetGuideChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			NPC helloNurse = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Nurse);
			NPC electricityFan = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Mechanic);
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			NPC suspiciouslyPersonShapedCake = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.PartyGirl);
			NPC steamEnjoyer = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Steampunker);
			NPC foxBimbo = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.BestiaryGirl);

			List<string> guideChatPool = new List<string>();
			V2Utils.FigureOutWhatTimeItIs(
				out bool pastMorning,
				out int hour,
				out int minute,
				out int second,
				out MealTime mealTime
			);
			double totalBellyWeight = PredNPC.GetCurrentBellyWeight(npc);
			bool playerIsFood = player.IsFoodFor(npc, out bool playerWasAlreadyDigested);
			if (playerIsFood && !playerWasAlreadyDigested)
			{
				bool noDigest = false;
				if (Main.bloodMoon)
				{
					guideChatPool.AddRange(new List<string>
					{
						"What? Too confused on [c/FF0000:how to avoid digesting?] Tough luck.",
					});
				}
				else
				{
					guideChatPool.AddRange(new List<string>
					{

					});
					if (noDigest)
					{
						guideChatPool.AddRange(new List<string>
						{

						});
					}
					else
					{
						guideChatPool.AddRange(new List<string>
						{
							"Avoid struggling too haphazardly while digesting! Struggling too aimlessly will just help you digest easier.",
						});
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					guideChatPool.AddRange(new List<string>
					{
						
					});
				}
				else
				{
					guideChatPool.AddRange(new List<string>
					{
						
					});
				}
			}
			return guideChatPool;
		}

		public static bool CanGuideBeForceFed(NPC npc) => true;

		public static void OnGuideForceFed(NPC npc, Player player)
		{
			PredNPC.SetChatboxText(
				npc,
				player,
				"[c/7F7F7F:<" + npc.GivenName + " very briefly seems surprised by his sudden forced ingestion of your arms and head, but nonetheless calms quickly, and easily gulps you down.>]\n"
			  + "This is a perfect opportunity to demonstrate the consequences of being eaten...though, perhaps you see them as positive consequences, given you fed yourself to me on a whim."
			);
		}


		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Guide.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Guide.2",
			});

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Guide.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => Main.bloodMoon ? 1.5 : 1.0;

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 10.0;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 4,
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

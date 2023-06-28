using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.Sounds.Vore;
using static V2.Core.FoodTypeTags;

namespace V2.NPCs.Vanilla.TownNPCs.Mechanic
{
	public static class MechanicStuff
	{
		public static Mechanic AsMechanic(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Mechanic predMechanic))
				throw new Exception("this instance of the Mechanic can't be pred or prey");

			return predMechanic;
		}
		public static MechanicProfile PredMechanicProfile = new MechanicProfile();
	}

	public class MechanicProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public MechanicProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/Mechanic/Mechanic_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/Mechanic/Mechanic";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySizeMethod.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			if (npc.altTexture == 1)
				exactTextureToUse += "_Party";

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.Mechanic;
	}

	public class Mechanic : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Mechanic;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsPred().maxStomachCapacity = 1.75;

			npc.AsPred().GetDigestionTickRateMethod = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamageMethod = GetDigestionTickDamage;

			
			npc.AsPred().GetPreyAbsorptionRateMethod = GetPreyAbsorptionRate;

			npc.AsV2NPC().GetChatMethod = GetMechanicChat;

			npc.AsPred().CanBeForceFedMethod = CanMechanicBeForceFed;
			npc.AsPred().OnForceFedMethod = OnMechanicForceFed;

			npc.AsPred().OnDigestionKillMethod = OnDigestionKill;
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetDigestedPlayerAdditionalDeathMessagesMethod = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetVisualBellySizeMethod = GetVisualBellySize;

			npc.AsFood().FoodTypeTags = new List<FoodTypeTag>
			{
				new MeatTag()
				{
					FoodSubtypeTags = new List<(string subtype, double weight)>
					{
						("Human", 0.93)
					}
				},
				new MetalTag()
				{
					FoodSubtypeTags = new List<(string subtype, double weight)>
					{
						("Copper", 0.02)
					}
				},
			};
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => MechanicStuff.PredMechanicProfile;

		public List<string> GetMechanicChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC hopelessRomantic = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.ArmsDealer);
			NPC bootlegChippy = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Clothier);
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			NPC steamLass = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Steampunker);
			NPC succubus = nearbyResidentNPCs.FirstOrDefault(x => x.type == ModContent.NPCType<Lucinda>());

			List<string> mechanicChatPool = new List<string>();
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
					mechanicChatPool.AddRange(new List<string>
					{
						"Oh, quit whining. My stomach's food processing protocol will melt you starting any second now.",
					});
				}
				else
				{
					mechanicChatPool.AddRange(new List<string>
					{
						"You do not need to worry about getting in the way while I work; these overalls are designed to keep ALL of my sustenance processing system's contents nice and compact.",
						"Do not make any movements without first notifying me. Unapproved movements, if strong enough, may cause damages to my system; else, they may cause damages to you.",
					});
					if (noDigest)
					{
						mechanicChatPool.AddRange(new List<string>
						{
							"Could you provide some assistance by pushing up some spare wire while you are in there? I am in need of a little bit more for this project, and I believe I ate some alongside you...",
							"Thank you again for filling out my system, " + player.name + ". Simply containing you within it is already improving my ability to focus tenfold.",
							"Your continued stay inside me is providing plenty of data on the ability for a human's food processing system to contain large meals inside undigested. Proceed as you are; I will continue to use you to generate new machine concepts.",
						});
					}
					else
					{
						mechanicChatPool.AddRange(new List<string>
						{
							"[c/00BB00:*BUOARP!*]\n"
						  + "There we go. I had to expel some heat for a second...my system is working very hard to convert you to energy, after all.",
							"Are you impressed by the strength of my internal food processor? I certainly am, although I always think I could optimize it just a bit further...",
							"There's nothing quite like some high-quality brain food to keep my attention on my work...and, thankfully, I have you available to fulfill that role in my system.",
							"I cannot say I am entirely looking forward to seeing how much potential energy you're converted into...working becomes overly difficult if my weight exceeds a critical threshold.",
						});
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					if (GetVisualBellySize(npc) >= 3)
					{
						mechanicChatPool.AddRange(new List<string>
						{
							"You happen to be interrupting my work...fortunately for you, I already had some quality brain food for the night. Spit it out; what do you want?",
							"[c/00BB00:*BUOARP!*]\n"
						  + "...what are you looking at me like that for? Gaseous expulsion of excess heat is NORMAL. Now then, idiot, did you come to buy the wire you don't have?",
							"Massage my primary processing chamber to aid in clearing the cache of sustenance currently being evaluated inside it, and I might not eat you for interrupting my workflow.",
						});
					}
					else
					{
						mechanicChatPool.AddRange(new List<string>
						{
							"Finally, someone to process who isn't already bones...yet. If you're just here to bother me, feed yourself into my processing system. NOW.",
							"The Destroyer is also an alternative name I use for my stomach. Don't understand? Stand still for a few more seconds, and I'll demonstrate why.",
							"You and wire are very similar. Easy to swallow, easy to process, and easy to explain away if you happen to disappear. Still want to bother me?",
							"Listen, I am WORKING right now. Let me work in peace, or let me take a five-second snack break.",
							"What do you WANT? I already had to eat my lunchbox just to hold me over so I can finish this blueprint draft!",
							"Let me guess. Didn't buy enough wire, idiot. Well, I ate a lot of it trying to fill my stomach, so if you want it, either pay extra or go in after it.",
							"You're interrupting my work. Quit it, or quit being outside of my system.",
						});
					}
				}
				else
				{
					mechanicChatPool.AddRange(new List<string>
					{
						"Always purchase more wire than you need. Its flavor is defined as pleasant, and it fills chambers of all kinds, including internal ones, with uncomparable cost efficiency.",
						"You know what this house needs? More blinking lights. Probably some lights inside me, too, as an energy source.",
						"My primary food processing chamber, or \"stomach\", if you will, is a well-optimized mechanism...if you would like, and are not currently busy with other tasks, I can provide you a more hands-on demonstration if requested.",
						"When I'm not busy with other tasks, would you like to take a tour through my internal food processing pipes? They're VERY efficient at transporting food and converting it into potential energy.",
						"Did you make sure your device was plugged in? Preferably NOT to any consumption cavities you may use, your navel, or your digestive system, but to an actual power outlet?",
					});
					if (bootlegChippy != null)
					{
						if (bootlegChippy.IsFoodFor(player))
						{
							mechanicChatPool.AddRange(new List<string>
							{
								"...seeing him move around in your stomach and gradually slow is...I believe the term is \"satisfying\". Continue keeping him inside you; do not let him go. Ever.",
								bootlegChippy.GivenName + " has already done enough damage as it is. It is...pleasant to see that you have fed him into your system.",
							});
						}
						else if (bootlegChippy.IsFoodFor(npc))
						{
							mechanicChatPool.AddRange(new List<string>
							{
								"...what? I have reached my maximum patience quota for people that have violated my safety and freedom protocols for years on end. I have no concern for his physical or mental states to communicate.",
								bootlegChippy.GivenName + " has already done enough damage as it is. It is...strangely pleasant to feel him inside me, and to have him beg for mercy. I have received that which I am owed; a sufficient fuel source for my internals.",
							});
						}
						else
						{
							mechanicChatPool.AddRange(new List<string>
							{
								"If you have a moment to spare, I request that you inform " + bootlegChippy.GivenName + " that he is behind on his electrical payments, and should visit me as soon as possible so I can...collect his dues, in a manner of speaking.",
								bootlegChippy.GivenName + " bothers me to no end. I find it incredibly difficult to ascertain his reason for pretending he didn't do what he did, but I plan to feed him into my system and resolve his continued presence.",
							});
						}
					}
					if (steamLass != null)
					{
						if (steamLass.IsFoodFor(player))
						{
							mechanicChatPool.AddRange(new List<string>
							{
								"I see that " + steamLass.GivenName + " is compressed into your stomach. I am...I believe the word is \"happy\", to know that she is in her rightful place; that of a battery.",
								"It is just like I told you. Electricity is factually better than steam by a significant margin. The fact that " + steamLass.GivenName + " is wasting away in your system as we speak is proof of this simple fact.",
							});
						}
						else if (steamLass.IsFoodFor(npc))
						{
							mechanicChatPool.AddRange(new List<string>
							{
								steamLass.GivenName + "? She is currently busy being processed by my factually superior system. Please allow up to 8 hours for sufficient processing and storage; she will return at some point.",
								"It is just like I told you. Electricity is factually better than steam by a significant margin. My internal food processing unit is currently demonstrating this fact to " + steamLass.GivenName + ", who doesn't seem to believe me.",
							});
						}
						else
						{
							mechanicChatPool.AddRange(new List<string>
							{
								"If you have a moment to spare, I request that you inform " + steamLass.GivenName + " that her engines, both internal and external, are outdated, and to advise her on coming over for a \"tune-up\" sometime in the next few weeks.",
								"Electricity is significantly better than steam in almost every capacity there is...save for being non-expellable in a gaseous state after consumption of sufficiently large meals. I currently discern this to be the core reason " + steamLass.GivenName + " likes it so much.",
							});
						}
					}
					if (Main.IsItAHappyWindyDay)
					{
						mechanicChatPool.AddRange(new List<string>
						{
							player.name + ", I require assistance testing something. Since wires are clearly not sufficient in this weather, can wind be combined with my internal mechanisms to generate and transfer power?",
							"Frustration levels, rising at too steep a pace. Storing these wires in my STOMACH would leave them less tangled than they're getting from the excessive force of this wind.",
						});
					}
					if (Main.IsItRaining)
					{
						mechanicChatPool.AddRange(new List<string>
						{
							"This rain makes for a wonderful electrical conductor. I...feel a compulsion to think about whether or not filling myself with it would allow for me to effectively \"swallow\" and \"digest\" common electrical currents.",
							"Exercise caution around my machines. In the current weather, one misstep may cause you to be electrocuted...which would then leave me no other option but feeding you into my system to harvest and store the resulting energy.",
						});
					}
					if (Main.IsItStorming)
					{
						mechanicChatPool.AddRange(new List<string>
						{
							"Hmm...if I could funnel all this lightning into my stomach, would I then be able to harness its power from inside me by...digesting it, in a manner of speaking? Maybe then it would quit frying my equipment...",
							player.name + ", would you mind helping me out? I need to consume one of those stormclouds so I can test if processing one lets my stomach produce electrical power instead of cellulite deposits.",
							"Securing the power grid is much easier said than done...is there anything I haven't stored yet? If there IS much more, then I will need to begin reconstituting my internal food processor for this purpose, which never ends well.",
						});
					}
					if (LanternNight.LanternsUp)
					{
						mechanicChatPool.AddRange(new List<string>
						{
							"If I could get my food processing system to produce and transfer light somehow, we could have one of these nights every night without fail, and perfectly renewably...as long as said system is, of course, given sufficient power, be it food or otherwise.",
							"I am currently wondering if I could feed some of these lanterns into my system and, through its innate processes and conversion techniques, transmute the lanterns into permanent bioluminescence. Such a feat would most certainly aid working late nights...",
						});
					}
				}
			}
			return mechanicChatPool;
		}

		public static bool CanMechanicBeForceFed(NPC npc) => true;

		public static void OnMechanicForceFed(NPC npc, Player player)
		{
			PredNPC.SetChatboxText(
				npc,
				player,
				"[c/7F7F7F:<" + npc.GivenName + " is completely silent as you feed yourself into her throat, simply helping the process along with steady, calculated swallows, until her system has completely accepted its spontaneous input.>]\n"
			  + Main.rand.NextFromCollection(new List<string>
				{
					"You seem...disproportionately eager to feed yourself into my system. Nonetheless, I won't deny you the opportunity to be a battery for me. You taste much better than my lunches often do...",
					"...and that, " + player.name + ", is a hands-on demonstration of the sustenance intake mechanisms located in my mouth and throat. As per logical progression, I will begin to demonstrate my internal food processor now.",
					"Mm...you have so many interlocking flavors. I'm very curious as to how they all come together to form such a pleasantly-flavored sustenance unit...I will be studying you as you digest. For now, though, I need to work, so quiet down.",
				})
			);
		}


		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.1",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.2",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.5",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.6",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.7",
			});

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.Hardcore");
			}
		}

		public override void PostAI(NPC npc)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				return;

			static void RollForRandomGulp(ref bool gulp) => gulp |= Main.rand.NextBool(3, 100);

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC hopelessRomantic = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.ArmsDealer);
			bool resolveHopelessRomantic = false;
			RollForRandomGulp(ref resolveHopelessRomantic);
			RollForRandomGulp(ref resolveHopelessRomantic);
			if (hopelessRomantic != null && hopelessRomantic.Distance(npc.Center) <= npc.AsPred().swallowRange && resolveHopelessRomantic)
				PredNPC.Swallow(npc, hopelessRomantic);

			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			bool resolveBestGirl = false;
			RollForRandomGulp(ref resolveBestGirl);
			RollForRandomGulp(ref resolveBestGirl);
			if (bestGirl != null && bestGirl.Distance(npc.Center) <= npc.AsPred().swallowRange && resolveBestGirl)
				PredNPC.Swallow(npc, bestGirl);

			NPC bootlegChippy = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Clothier);
			bool resolveBootlegChippy = false;
			RollForRandomGulp(ref resolveBootlegChippy);
			RollForRandomGulp(ref resolveBootlegChippy);
			RollForRandomGulp(ref resolveBootlegChippy);
			RollForRandomGulp(ref resolveBootlegChippy);
			if (bootlegChippy != null && bootlegChippy.Distance(npc.Center) <= npc.AsPred().swallowRange && resolveBootlegChippy)
				PredNPC.Swallow(npc, bootlegChippy);

			NPC steamLass = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Steampunker);
			bool resolveSteamLass = false;
			RollForRandomGulp(ref resolveSteamLass);
			RollForRandomGulp(ref resolveSteamLass);
			RollForRandomGulp(ref resolveSteamLass);
			RollForRandomGulp(ref resolveSteamLass);
			RollForRandomGulp(ref resolveSteamLass);
			if (steamLass != null && steamLass.Distance(npc.Center) <= npc.AsPred().swallowRange && resolveSteamLass)
				PredNPC.Swallow(npc, steamLass);

			if (ModContent.GetInstance<V2ServerConfig>().NoRandomGulpsAgainstPlayers)
				return;

			if (!Main.CurrentPlayer.active || Main.CurrentPlayer.dead || Main.CurrentPlayer.Distance(npc.Center) > npc.AsPred().swallowRange || Main.CurrentPlayer.AsFood().IsCurrentlyEaten)
				return;

			bool shouldHaveBrainFood = false;
			RollForRandomGulp(ref shouldHaveBrainFood);

			if (Main.netMode != NetmodeID.Server && Main.CurrentPlayer.whoAmI == Main.myPlayer && Main.CurrentPlayer.Distance(npc.Center) <= npc.AsPred().swallowRange && shouldHaveBrainFood)
			{
				List<string> potentialRandomGulpLines = new List<string>
				{
					"Sorry, " + Main.CurrentPlayer.name + ", but I need some brain food. You will have to do.",
					"I apologize for the interruption, but my body requires fuel to continue functioning. You will need to suffice.",
				};
				PredNPC.SwallowWithTextIfApplicable(
					npc,
					Main.CurrentPlayer,
					"[c/7F7F7F:<Without warning, " + npc.GivenName + " stuffs you down her throat, headfirst. With your body being compacted into a rather tight state due to her space-efficient outfit, " + npc.GivenName + " pats her belly.>]\n"
				  + Main.rand.NextFromCollection(potentialRandomGulpLines)
				);
			}
		}

		public static double GetDigestionTickRate(NPC npc, Prey prey) => Main.bloodMoon ? 6.5 : 3.25;

		public static double GetDigestionTickDamage(NPC npc, Prey prey) => 6.5;

		public static void OnDigestionKill(NPC npc, Prey digestedPrey)
		{
			SoundEngine.PlaySound(
				npc.AsPred().StandardBurps,
				npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f)
			);
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 3,
				seconds: 0
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(3.5 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				3
			);
		}
	}
}

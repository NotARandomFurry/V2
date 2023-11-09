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

namespace V2.NPCs.Vanilla.TownNPCs.Dryad
{
	public static class DryadStuff
	{
		public static Dryad AsDryad(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Dryad predStylist))
				throw new Exception("this instance of the Dryad can't be pred or prey. find another salad to snack on or get snacked on by");

			return predStylist;
		}
		public static DryadPredProfile DryadPredProfile => new DryadPredProfile();
	}

	public class DryadPredProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public DryadPredProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/Dryad/Dryad_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/Dryad/Dryad";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.Dryad;
	}

	public class Dryad : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Dryad;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsV2NPC().GetNewDialogue = GetDryadChat;

			npc.AsFood().Size = 1.118;
			npc.AsPred().MaxStomachCapacity = 12.50;
			npc.AsPred().BaseStomachacheMeterCapacity = 450.0;

			npc.AsPred().CanBeForceFed = CanDryadBeForceFed;
			npc.AsPred().OnForceFed = OnDryadForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			npc.AsPred().OnDigestionKill = OnDigestionKill;
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;

			npc.AsFood().OnKilledByDigestion += PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => DryadStuff.DryadPredProfile;

		public static List<string> GetDryadChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC succubus = nearbyResidentNPCs.FirstOrDefault(x => x.type == ModContent.NPCType<Lucinda>());

			List<string> dryadChatPool = new List<string>();
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
					dryadChatPool.AddRange(new List<string>
					{
						"A pitiful fate, albeit the only suitable one, for a whelp like you. You should be thankful you're melting inside a greater being such as I.",
						"My body will cleanse you down to your very soul and leave only that soul when it is done. Your fault for getting yourself eaten.",
						"I eat who I want and what I want, when I want! Now stop squirming and digest already!",
					});
					if (PredNPC.GetStomachTracker(npc).Prey.Count > 1)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"Do not think for but a moment I am too full to reduce you to naught but a purified, nutritional sludge.",
							"The meal currently digesting within me was simply too weak to challenge me, and thought they were not. If you want to come out of this night as anything more than padding for my rear end, do not make the same mistake.",
						});
					}
					else
					{

					}
				}
				else
				{
					dryadChatPool.AddRange(new List<string>
					{
						"Do not move too strongly within my stomach. The less you do, the more easily I am able to keep you locked away in there.",
						"As an envoy to nature, allow me to express that nature and I, alike, find you quite satisfying as food.",
					});

					if (noDigest)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"Yes...relax yourself. Allow your body to remain safe within my innermost sanctum of purity: my all-cleansing stomach.",
							"My insides tell me you are still alive and well. This is good. I do not wish to digest you...at the moment, at least.",
						});
					}
					else
					{
						dryadChatPool.AddRange(new List<string>
						{
							"Mmm...now THIS is the kind of meal I need to have more often if I'm to properly rid the world of the evils that continue to blight it.",
							"Yes, my prey. Allow yourself to be cleansed by my body...and promptly added to it.",
							"That's it, melt...let my acids burn away the vile darkness within you, and append your form to my own.",
							"What a wonderful little meal...I can feel your every cell being purified by the second. Truly, the most efficient way of purging the unclean.",
							"I really must do this more frequently...I could use more variety in my diet. The foul creatures of the " + (WorldGen.crimson ? "Crimson" : "Corruption") + " never taste very good, and they aren't very nutritious at all, either...",
							"Yes, that's it. Let your soul be rid of impurity, and your body transformed into more padding for a more suitable savior of this world.",
							"What was that about me being called a salad? I sure hope you're not too upset about being melted into MORE of this salad...",
							"You may not be aging very gracefully, but you certainly taste like a fine wine. Delectable.",
						});

						if (Main.LocalPlayer.ZoneSnow)
						{
							dryadChatPool.AddRange(new List<string>
							{
								"It feels lovely to be able to combat the snowy chill of this region by digesting a heavy, thrashing meal like you into nutrients to better fasten my roots and insulate my stem.",
							});
						}
					}

					if (Main.LocalPlayer.ZoneSnow)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"There is nothing quite like a pleasantly heavy stomach and some nice cocoa to help hibernate through the winter months.",
						});
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					dryadChatPool.AddRange(new List<string>
					{
						"What could someone as insignificant as you POSSIBLY want from me? I am currently busy trying to fend off the adversities of tonight.",
						"Why must you pull me away from my thoughts on a night such as tonight?",
						"Continue to bother me, and you will be purified. I have given you fair warning.",
						"This fury consumes my very soul...how am I supposed to save our world in a frenzied state such as this!?",
						"The great mother of nature has had her ire drawn to this world this night. Do not dare to anger her further, or you shall contend with me.",
					});

					if (Main.IsItStorming)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"Our mother casts doom upon whelps such as thee tonight. Should you value your life, I would not recommend going outside at all. Perhaps a purpose better served by filling my stomach.",
							"The crack of the roaring skies sends many a beast whimpering into their dens, yet draws out the ire of even more. You would do well to hide away somewhere...perhaps in someONE.",
						});
					}
					else if (Main.IsItRaining)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"Nature casts a bloodied sorrow upon lesser beings this night, furied at how frail many of the creatures have become with time.",
							"Fish are not meant to fly, and the wings of any fish that reasonably could should be dampened by the rain.\n"
						  + "Why, then, do the mysterious sea creatures dancing about the skies care so little for the red-tinted torrent from above?",
						});
					}
				}
				else
				{
					dryadChatPool.AddRange(new List<string>
					{
						"You may believe that Terraria needs you. The truth could not be farther from whence this notion comes...but it can be pleasant to make yourself believe you are necessary.",
						"Some people say I am all bark and no bite. Needless to say, they very quickly learn that there is no bark lining my stomach.",
						"I really need to travel out and hunt more, I should think. Just as a flower cannot grow and bloom without sunlight, I cannot grow stronger without food.",
						"The sands of time, as they are to most humans, don't seem to have been particularly kind to you. Your aging is not as...graceful as it ought to be.",
					});

					if (Main.LocalPlayer.ZoneForest)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"The wildlife here is fairly diverse, though nothing to be ecstatic over...and much of it is quite calming. I believe I could learn to like staying here.",
							"Listening to birds chirp their morning songs and squabbles alike as the grass warms to the sun is an experience one does not get often in the jungles I so often prefer to stay in. It is a welcome shift of scenery.",
						});
					}

					if (Main.LocalPlayer.ZoneRockLayerHeight && !Main.LocalPlayer.ZoneJungle)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"I must admit, I do not entirely like being this far down, yet being able to see the roots of our world and the flora and fauna that reside within them is fascinating in a way that I...do not believe you would understand.",
							"I have witnessed many beings which, on a first glance, look similar to myself...yet, when their prey draws close enough, they will don their more...personal form and devour their target alive, without a second thought. Exercise caution if you wish to stay out of a nymph's digestive tracct.",
						});
					}

					if (Main.LocalPlayer.ZoneJungle)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"This is perhaps the greatest sort of region our planet has to offer. The diverse flora and close connection to nature itself give me a great amount of pleasant comfort.",
							"My fellow plants in this area are not without their appetites, and are only sparingly against devouring the first prey they see. An unfortunate reality for an adventurer such as you...",
							"Be wary of the great swathes of oceanic wildlife here. Many of them are just as ravenous as the plants themselves, and will have no trouble turning you from a mercenary into a meal.",
						});
						if (Main.LocalPlayer.ZoneRockLayerHeight)
						{
							dryadChatPool.AddRange(new List<string>
							{
								"Within these depths lie many creatures, floral or otherwise, who will not hesitate to swallow you like a light snack. Tread carefully if you value your continued existence.",
								"Many insect hives found this deep in the jungle's roots can be quite large, and home to equally large, territorial, and predatorial bees. Do not intrude upon these nests lightly.",
								"While I cannot stop you from getting eaten by the wildlife here, I feel the need to warn you of a great being within the depths of this jungle. With three minds in one body, they can rarely ever agree...save for one thing: that all this jungle's wildlife is but a feast for them.",
							});
						}
					}

					if (Main.LocalPlayer.ZoneDesert)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"The searing heat and sparse moisture in this region would singe many plants I am most familiar with, and rather few are allowed to flourish in their place. I...do not believe I am comfortable here.",
							"I must be careful not to spend too long in this scorchingly hot sunlight. Even a greater bloom such as myself could wilt if left in this heat for too long...and it is rather easy to do so.",
							"There are some who have found these sorts of regions hospitable, with many techniques for bodily coordination and strength originating from them...but I am afraid I cannot fathom the same.",
						});
					}

					if (Main.LocalPlayer.ZoneSnow)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"This region could freeze many of my fellow plants to the roots, yet...despite the colder temperatures, the abundant, if frozen, moisture allows a uniquely diverse set of flora to thrive here. Perhaps I could as well, with enough time...",
							"The icy chill here is best remedied with the warmth and comfort of a full stomach, digesting a heavy, thrashing meal into nutrients to better fasten your roots and insulate your stem.",
						});
						if (PredNPC.GetCurrentBellyWeight(npc) > 1.25)
						{
							dryadChatPool.AddRange(new List<string>
							{
								"There is nothing quite like a pleasantly heavy stomach and some nice cocoa to help hibernate through the winter months.",
							});
						}
					}

					if (Main.IsItAHappyWindyDay)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"The wind... it is nature's way of sweeping the dust from the land. The larger impurities, of course, require more...permanent solutions.",
							"Nature's fury strips the leaves from the trees this day, well into the waiting maws of plant-eaters. Be wary you do not meet a similar fate.",
							"On occasion, parts of some flowers may find themselves drifting through the air as a result of a day like this. These are almost always a show of good fortune, granted by the mother of nature herself.",
						});
					}

					if (Main.IsItStorming)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"Some believe that a being known as the \"Grand Botanist\" has entered a fury in times like these. I more correctly believe it simply the fury of nature itself.",
							"It is unwise to traverse openly beneath the skies currently. The seething, flashing strikes from above will broil you in but an instant.",
							"Be wary you do not mistake the crack of the frenzied skies for the roar of a hungering beast. One is only a danger if the beast ensnares you; the other cares none.",
						});
					}
					else if (Main.IsItRaining)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"Nature provides rains such as these to wash away the mud in the streams and grant much-needed rain to the plants unfortunate enough to not subsist on live prey.",
							"A mysterious sort of fish becomes prevalent on " + (Main.dayTime ? "days" : "nights") + " like these, flying through the rain with nary a care in the world.",
						});
					}

					if (Main.LocalPlayer.ZoneGraveyard)
					{
						dryadChatPool.AddRange(new List<string>
						{
							"The evils of this world are easily detained and digested, but this...the vile air of death in this location almost makes one feel like wilting.",
							"This place...nature cries at and for the fallen here, even with the knowledge that many of these gravestones are most likely for bodies which no longer exist as themselves.",
						});
					}
				}
			}
			return dryadChatPool;
		}

		public static bool CanDryadBeForceFed(NPC npc) => true;

		public static void OnDryadForceFed(NPC npc, Player player)
		{
			PredNPC.SetChatboxText(
				npc,
				player,
				Main.rand.NextFromCollection(new List<string>
				{
					"So, you're in need of purification? Very well, then. Allow me to cleanse your body with the strength of my own.",
					"Mm...well, I AM rather hungry, as us dryads always are, and it is difficult to purify this world on an empty stomach. You will fix both of these issues perfectly, I should think.",
					"Look at that. Another unclean soul begs me to devour them. Ah, well. More power to me, I suppose.",
					"If you are that certain that eating you will rid your body of evil, then I am happy to add you to my pure form.",
					"Offering yourself to me to aid in the cleansing of the world? Well...I suppose it would be rude to reject free food.",
				})
			  + "\n[c/7F7F7F:<" + npc.GivenName + " smiles slyly and opens her mouth wide as you start to force your way inside, guiding you into her waiting stomach as it groans happily at its newest target to purify.>]"
			);
		}

		public override void PostAI(NPC npc)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			if (PredNPC.GetStomachTracker(npc)?.Prey.Count > 0)
				return;

			if (Main.GameUpdateCount % 60 != 0)
				return;

			static void RollForRandomGulp(ref bool purify) => purify |= Main.rand.NextBool(4, 100);

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC FORE = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Golfer);
			bool shouldSnackOnGolfer = false;
			RollForRandomGulp(ref shouldSnackOnGolfer);
			if (FORE != null && FORE.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldSnackOnGolfer)
				PredNPC.Swallow(npc, FORE);

			NPC gadgetGal = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Mechanic);
			bool shouldSnackOnGadgetGal = false;
			RollForRandomGulp(ref shouldSnackOnGadgetGal);
			RollForRandomGulp(ref shouldSnackOnGadgetGal);
			if (FORE != null && FORE.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldSnackOnGadgetGal)
				PredNPC.Swallow(npc, gadgetGal);

			NPC steamLass = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Steampunker);
			bool shouldSnackOnSteamLass = false;
			RollForRandomGulp(ref shouldSnackOnSteamLass);
			RollForRandomGulp(ref shouldSnackOnSteamLass);
			if (FORE != null && FORE.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldSnackOnSteamLass)
				PredNPC.Swallow(npc, steamLass);

			NPC funnyShroom = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Truffle);
			bool shouldSnackOnShroom = false;
			RollForRandomGulp(ref shouldSnackOnShroom);
			RollForRandomGulp(ref shouldSnackOnShroom);
			RollForRandomGulp(ref shouldSnackOnShroom);
			RollForRandomGulp(ref shouldSnackOnShroom);
			RollForRandomGulp(ref shouldSnackOnShroom);
			if (funnyShroom != null && funnyShroom.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldSnackOnShroom)
				PredNPC.Swallow(npc, funnyShroom);

			if (ModContent.GetInstance<V2ServerConfig>().NoRandomGulpsAgainstPlayers)
				return;

			if (!Main.CurrentPlayer.active || Main.CurrentPlayer.dead || Main.CurrentPlayer.Distance(npc.Center) > npc.AsPred().MaxSwallowRange || Main.CurrentPlayer.CurrentCaptor() is not null)
				return;

			bool shouldPurifyPlayer = false;
			int worldTaint = WorldGen.tEvil + WorldGen.tBlood + WorldGen.tGood;
			if (worldTaint > 0)
				RollForRandomGulp(ref shouldPurifyPlayer);
			if (worldTaint > 3)
				RollForRandomGulp(ref shouldPurifyPlayer);
			if (worldTaint > 10)
				RollForRandomGulp(ref shouldPurifyPlayer);
			if (worldTaint > 25)
				RollForRandomGulp(ref shouldPurifyPlayer);
			if (worldTaint > 50)
			{
				RollForRandomGulp(ref shouldPurifyPlayer);
				RollForRandomGulp(ref shouldPurifyPlayer);
			}
			if (worldTaint > 80)
			{
				RollForRandomGulp(ref shouldPurifyPlayer);
				RollForRandomGulp(ref shouldPurifyPlayer);
				RollForRandomGulp(ref shouldPurifyPlayer);
				RollForRandomGulp(ref shouldPurifyPlayer);
			}
			if (shouldPurifyPlayer)
			{
				switch (worldTaint)
				{
					case int i when i == 0:
						// this actually isn't able to be seen in normal play
						if (PredNPC.GetStomachTracker(npc) is not null)
							break;

						PredNPC.SwallowWithTextIfApplicable(
							npc,
							Main.CurrentPlayer,
							"[c/7F7F7F:<A calm, patient smile crosses " + npc.GivenName + "'s face as she very slowly guides you down her throat headfirst. Her stomach seems completely inert.>]\n"
						  + "To think...you have managed to cleanse this patch of all evils...you have done a great service to this land, indeed. Allow me to provide you with a comfortable place to rest after all your hard work. Let me know if you'd like to get out...or to NOT get out, of course. If you were to want to be purified badly enough, I would not dare to refuse the request of a valued hero like yourself..."
						);
						break;
					case int i when i > 0 && i <= 3:
						PredNPC.SwallowWithTextIfApplicable(
							npc,
							Main.CurrentPlayer,
							"[c/7F7F7F:<A calm, patient look crosses " + npc.GivenName + "'s facce as she picks you up and rather slowly guides you down her throat headfirst, letting out a rather plain, though satisfied, belch once your feet pass her lips.>]\n"
						  + "To think that the world has been almost entirely cleansed of evils. A shame that, as one of the last remaining vestiges of corruption...you, too, must be cleansed by my pure system.\n"
						  + "\n"
						  + "...or, more accurately, I must make sure there isn't any stowing away on your body. My stomach will ensure your cleanliness as you enter the final stretch of your effort."
						);
						break;
					case int i when i > 3 && i <= 10:
						PredNPC.SwallowWithTextIfApplicable(
							npc,
							Main.CurrentPlayer,
							"[c/7F7F7F:<As a mostly-calm, though faintly upset look crosses her face, " + npc.GivenName + " picks you up and slightly-slowly guides you down her throat headfirst, letting out a rather plain, though somewhat satisfied, belch once your feet pass her lips.>]\n"
						  + "The world has certainly become a more hospitable place in the sense that the evils that plague it have been pushed back so far...yet there still lies a substantial amount. That said, you are making tangible progress. When you are done digesting, continue making such progress."
						);
						break;
					case int i when i > 10 && i <= 25:
						PredNPC.SwallowWithTextIfApplicable(
							npc,
							Main.CurrentPlayer,
							"[c/7F7F7F:<As a mildly-frustrated frown crosses her face, " + npc.GivenName + " picks you up and nigh-effortlessly guides you down her throat headfirst, letting out a rather plain belch once your feet pass her lips.>]\n"
						  + "You have, perhaps, done a decent deal in pushing back the encroachment of the poisons of our world...yet you have still failed to cleanse so much. Perform better."
						);
						break;
					case int i when i > 25 && i <= 50:
						PredNPC.SwallowWithTextIfApplicable(
							npc,
							Main.CurrentPlayer,
							"[c/7F7F7F:<As a frustrated scowl crosses her face, " + npc.GivenName + " picks you up and effortlessly guides you down her throat headfirst, letting out a rather plain belch once your feet pass her lips.>]\n"
						  + "Our stretch of this world has become so tainted...I fail to see this as the fault of anyone other than yourself. I'm beginning to believe you will do a greater service on my hips than by healing the planet..."
						);
						break;
					case int i when i > 50 && i <= 80:
						PredNPC.SwallowWithTextIfApplicable(
							npc,
							Main.CurrentPlayer,
							"[c/7F7F7F:<As an angered scowl crosses her face, " + npc.GivenName + " picks you up and rather roughly forces you down her throat headfirst, letting out a rather plain belch once your feet pass her lips.>]\n"
						  + "You are nearing the point of no return, both from the onset of evils and from the onset of my appetite. If, and [c/FF0000:ONLY] if, that is your goal, continue as you are. Otherwise...I'd recommend learning to purify the world more effectively, lest you end up fertilizer, PERMANENTLY."
						);
						break;
					case int i when i > 80:
						PredNPC.SwallowWithTextIfApplicable(
							npc,
							Main.CurrentPlayer,
							"[c/7F7F7F:<As an infuriated scowl crosses her face, " + npc.GivenName + " picks you up, forcefully curls you into a ball, and stuffs you down her throat almost like a cheesesteak, letting out a thunderous belch as you're mercilessly forced into her stomach.>]\n"
						  + "You have [c/FF0000:FAILED]. Any semblance of use you had will be far surpassed by your new purpose as fertilizer for a woman stronger and more capable than yourself...I've no care to let you bother with your failed mockery of a cleansing. [c/FF0000:Melt, food.]"
						);
						break;
				}
			}
		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Dryad.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Dryad.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Dryad.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Dryad.4",
			});

			if (WorldGen.tEvil + WorldGen.tBlood + WorldGen.tGood > 0.25)
			{
				deathReasonKeyList.AddRange(new List<string>
				{
					"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Dryad.TaintedWorld.1",
					"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Dryad.TaintedWorld.2",
					"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Dryad.TaintedWorld.3",
				});
			}

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Dryad.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => Main.bloodMoon ? 2.95 : 1.475;

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 27;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			SoundEngine.PlaySound(
				digestedPrey.WeightLeftToDigest < 0.35 ? npc.AsPred().SmallBurps : npc.AsPred().StandardBurps,
				npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f)
			);
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 4,
				seconds: 15
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(4.8 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				4
			);
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
	}
}

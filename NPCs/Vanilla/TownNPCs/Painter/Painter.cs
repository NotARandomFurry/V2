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

	public class Painter : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Painter;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Male;

			npc.AsV2NPC().GetNewDialogue = GetPainterChat;

			npc.AsFood().DefinedBaseSize = 0.988;
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

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => PainterStuff.PredPainterProfile;

		public static List<string> GetPainterChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			NPC helloNurse = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Nurse);
			NPC electricityFan = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Mechanic);
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			NPC suspiciouslyPersonShapedCake = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.PartyGirl);
			NPC steamEnjoyer = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Steampunker);
			NPC foxBimbo = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.BestiaryGirl);

			List<string> painterChatPool = new List<string>();
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
					painterChatPool.AddRange(new List<string>
					{
						"[c/FF0000:QUIET!] I can't focus on my latest work with you [c/FF0000:screaming for help] in there!",
						"Ughhh, I KNEW I should've eaten lighter tonight...[c/FF0000:or maybe even heavier]...shut up, SHUT UP, [c/FF0000:SHUT UP IN THERE!]",
					});
				}
				else
				{
					painterChatPool.AddRange(new List<string>
					{
						"S- steady in there...if you move around too much, you'll mess up one of my brushstrokes!",
					});
					if (noDigest)
					{
						painterChatPool.AddRange(new List<string>
						{

						});
					}
					else
					{
						painterChatPool.AddRange(new List<string>
						{
							"Hey, stop kicking and- NO! Aww, GREAT! Now the best part of this piece is all messed up!",
							"Would you mind settling down and digesting? I really wanna paint this lovely gut, but all this...YOU is in the way like this...",
						});
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					painterChatPool.AddRange(new List<string>
					{
						"Tonight gives me a lot of inspiration. Inspiration to make people [c/FF0000:my beautifully blood-red meals!]",
						"If you look out at the [c/FF0000:yearning, hungering] moon in the sky, you can see it glint red onto the rivers! It's a [c/FF0000:truly great] piece idea.",
						"These sorts of nights are great for finding [c/FF0000:vampires] to paint. They're all REALLY pretty...[c/FF0000:especially all the tasty girls...]",
					});
				}
				else
				{
					painterChatPool.AddRange(new List<string>
					{
						"Steady, steady...aaaaand...done!...oh! Hi! You're just in time for my break! I just got done painting a lovely piece!",
						"Huh? Titanium white?...I'm afraid I'm all out. Have been for years. It's not easy to get people to quit painting parts of their cars with that color...and it doesn't even look good!",
						"No, no, no! There are SO many different shades of gray! Who the hell told you there were only 50? I oughta give them a trip to my stomach...",
						"Hey! Just finishing up my latest masterpiece!...a- as long as I can get these last few brushstrokes right, of course.",
					});
					if (!player.Male)
					{
						painterChatPool.AddRange(new List<string>
						{
							"Hm? Do I need a meal? Well...I think just painting you will do fine!...and it helps that you look so...SO good...a- as a subject, of course!",
						});
					}
					else
					{
						painterChatPool.AddRange(new List<string>
						{
							"Hm? Do I need a meal? Well...i- if you have any tasty-lookin' girls you don't need, I can always put 'em to better use...and paint a pretty picture of 'em, too!",
						});
					}
					if (salad != null)
					{
						if (salad.IsFoodFor(player))
						{
							painterChatPool.AddRange(new List<string>
							{
								"Hey!...oh, is that " + salad.GivenName + " in your stomach? Ugh, luckyyyyy...she's the prettiest mea- I MEAN model around!",
								"Oh, you must be havin' a GREAT time with a treat like that dryad in your belly! Do you mind if I paint you while you're still full of her?",
							});
						}
						else if (salad.IsFoodFor(npc))
						{
							painterChatPool.AddRange(new List<string>
							{
								"...mmm...mine...tasty, beautiful " + salad.GivenName + ", best treat in town...all mine...",
								"...oh! J- just a minute...! Currently digesting a REALLY tasty salad...she's perfectly happy in my gut, too!\n"
							  + "[c/BFBFBF:<You hear muffled furious shouting in " + npc.GivenName + "'s stomach which strongly suggests the opposite.>]",
							});
						}
						else
						{
							painterChatPool.AddRange(new List<string>
							{
								salad.GivenName + " looks REALLY pretty...ask her to come over and be my next art subject!...really soon!",
								"You know, doctors always tell me that one of the best options for staying in good shape to paint is to munch on salads. Can you nab the one livin' a couple doors down and get her here?",
							});
						}
					}
					if (helloNurse != null)
					{
						if (helloNurse.IsFoodFor(player))
						{
							painterChatPool.AddRange(new List<string>
							{
								"...oh, is that " + helloNurse.GivenName + " in your stomach? She makes a great centerpiece for art...and a great snack...send her my way next time!",
								"Hm? Do I need a nurse to eat? No...well, not RIGHT NOW, at least...of course, it helps that she's currently bein' a meal for you instead!",
							});
						}
						else if (helloNurse.IsFoodFor(npc))
						{
							painterChatPool.AddRange(new List<string>
							{
								"...oh, " + helloNurse.GivenName + "? W- well, I was just painting her, getting a good show of her good side...her r- REALLY good front side...and, well, I just couldn't help myself!",
								"Hiya! Oh, yeah, don't worry about this big ol' belly! Just a very HEALTHY gutful of girl settling into my stomach, haha!\n"
							  + "[c/BFBFBF:<A less-than-amused groan emanates from within " + npc.GivenName + "'s gut, the medical practitioner inside not appreciating the terrible joke.>]",
								"What? You gotta talk to " + helloNurse.GivenName + "? Well, if you talk loud enough to my belly, she miiight be able to hear you! Get it out fast, though...I don't think she has much longer, and I gotta paint a lovely picture of me and my meal before she churns up.",
							});
						}
						else
						{
							painterChatPool.AddRange(new List<string>
							{
								helloNurse.GivenName + " looks really pretty...do you think she'd be willin' to come over and model for an art piece real quick? Askin' for a friend.",
								"Nurses are always super friendly to me! They help me deal with the stomachaches from the occasional gutful of watercolors or ink, and they taste great, too!",
							});
						}
					}
					if (npc.position.Y < Main.worldSurface)
					{
						if (WorldGen.tEvil >= 5)
						{
							painterChatPool.AddRange(new List<string>
							{
								"There aren't many plants on corrupted surface soil, but when you CAN find them, they make for a great show of how life finds ways to flourish. Makes me wonder if life can find a way to flourish in my gut, too...I'd never have to eat again!\n\n"
							  + "...not that I wouldn't. Some gals are just too tasty to pass up.",
								"I've painted a landscape of the Corruption once or twice. It's beautiful, in a very dreary sort of way. Doesn't taste all that great, though, and I hear eatin' too much corrupted stuff gives you some kind of sickness.",
							});
						}
						if (WorldGen.tBlood >= 5)
						{
							painterChatPool.AddRange(new List<string>
							{
								"The Crimson makes for a really macabre sort of display. It's kinda like if you took everything that ISN'T pretty about the human body and went \"okay, but what if it was huge?\", and turned the result into a landscape.",
								"People say you get really sick if you eat too bloody stuff from the giant fleshmasses I find engagin' landscapes...I wouldn't blame them. Most of the things there are only even remotely appetizing as art subjects, NOT as easy meals.",
							});
						}
						if (WorldGen.tGood >= 5)
						{
							painterChatPool.AddRange(new List<string>
							{
								"Sometimes, when you're explorin' hallowed land...well, actually, a LOT of the time...you'll see rainbows in the sky. I hear they taste great, and I KNOW they're stunning when put to a fresh canvas.",
								"Unicorns are some of the Hallow's most majestic creatures...at least, when you can actually get them to sit still. I lost too many canvases and got a BAD stomachache the last time I tried paintin' one...but it digested well, at least!",
							});
						}
						if (Main.IsItAHappyWindyDay)
						{
							painterChatPool.AddRange(new List<string>
							{
								"Gahhh, it's so WINDY today! How am I supposed to focus on my art with these gusts messing up my form!?",
								"Feh, I can't put ANYTHING from color to canvas like this! The wind keeps knocking over my things!",
							});
						}
						if (Main.IsItRaining)
						{
							painterChatPool.AddRange(new List<string>
							{
								"I like listening to the rain pitter-patter against the windowsill. It makes for pleasant background noise while I paint, especially if you couple it with the calming gurgles of a full, still belly.",
								"Sometimes, while it's raining, you'll see special species of fish flying around. They're both really tasty and make for majestic art subjects...if you can keep them still and outside of your stomach, anyway.",
							});
							if (Main.hardMode)
							{
								painterChatPool.AddRange(new List<string>
								{
									"I tried taking a nature walk recently in a light rain, and found myself being chased by a couple angry rain clouds that wanted me drenched! They were certainly an inspiration for a piece thereafter, and fairly filling, too...but I didn't know clouds could be so mean!",
								});
							}
						}
						if (Main.IsItStorming)
						{
							painterChatPool.AddRange(new List<string>
							{
								"A lot of people get spooked by thunder, but honestly...? I find it a nice parallel to dashes of inspiration, broad brushstrokes of genius!",
								"Inspiration comes less often, but strikes as hard as lightning in weather like this! I can show you an example in my next painting, if you want!",
								"Hey, random question...if I ate a lightning bolt, or even a whole thundercloud, do you think I'd be able to paint at breakneck speeds while it digests? I could make so many pieces in a single dash of inspiration...!",
							});
						}
					}
				}
			}
			return painterChatPool;
		}

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

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

namespace V2.NPCs.Vanilla.TownNPCs.Stylist
{
	public static class StylistStuff
	{
		public static Stylist AsStylist(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Stylist predStylist))
				throw new Exception("this instance of the Stylist can't be pred or prey. best girl's best gut will have to wait, unfortunately");

			return predStylist;
		}
		public static StylistPredProfile StylistPredProfile => new StylistPredProfile();
	}

	public class StylistPredProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public StylistPredProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/Stylist/Stylist_Default_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => "Amber";

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/Stylist/Stylist";
			string outfitString = "_Default";
			if (npc.IsShimmerVariant)
				outfitString = "_Shimmer";
			exactTextureToUse += outfitString;
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySizeMethod.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			if (npc.altTexture == 1)
				exactTextureToUse += "_Party";

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.Stylist;
	}

	public class Stylist : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Stylist;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsPred().maxStomachCapacity = 4.0;

			npc.AsV2NPC().GetChatMethod = GetStylistChat;

			npc.AsPred().CanBeForceFedMethod = CanStylistBeForceFed;
			npc.AsPred().OnForceFedMethod = OnStylistForceFed;

			npc.AsPred().GetDigestionTickRateMethod = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamageMethod = GetDigestionTickDamage;

			npc.AsPred().OnDigestionKillMethod = OnDigestionKill;
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetDigestedPlayerAdditionalDeathMessagesMethod = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRateMethod = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySizeMethod = GetVisualBellySize;

			npc.AsFood().FoodTypeTags = new List<FoodTypeTag>
			{
				new MeatTag()
				{
					FoodSubtypeTags = new List<(string subtype, double weight)>
					{
						("Human", 1.035)
					}
				},
			};
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => StylistStuff.StylistPredProfile;

		public static List<string> GetStylistChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC armsDealer = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.ArmsDealer);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			NPC dyeTrader = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.DyeTrader);
			NPC succubus = nearbyResidentNPCs.FirstOrDefault(x => x.type == ModContent.NPCType<Lucinda>());
			NPC partyGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.PartyGirl);

			List<string> stylistChatPool = new List<string>();
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
					stylistChatPool.AddRange(new List<string>
					{
						"I told you to stay out of my hair tonight, hun. Now keep it down and digest!",
						"Hush up in there! You keep kickin' around in there, and I'll just dump a few bottles of hair dye down my throat to DROWN you!",
						"You were JUST what I needed, hun: a quick Gut Cut to get the hunger pangs to shut up. No refunds, and no escape. Sorry not sorry, gut fodder.",
						"[c/BFBFBF:(...I swear, if you make me fat, I'll just \"shave\" off your arms and thighs once you come back. Bet you'll be swearin' on your soul to be a good client THEN, asshole...)]",
					});
					if (npc.AsPred().stomachContents.Count > 1)
					{

					}
					else
					{

					}
				}
				else
				{
					stylistChatPool.AddRange(new List<string>
					{
						"Something about your scalp just makes it so unbelievably tasty. I gotta know, what kind of conditioner are you usin'? Hopefully, one that won't leave me with a stomachache...",
						"I really need to tell people about these gut cuts more...well, more than I already do. That, and I need to lower their prices a bit, I think...",
						"Between you and me, this is WAY better than just doing your hair with scissors and stuff. Way more fillin', too...",
						"Now, be careful not to rub against the walls too much. You wouldn't want your hair to get ruined before I get it styled.",
						"Ooh, are you usin' my favorite flavor of shampoo? I knew you couldn't resist it...and neither can I.~",
						"You really should try takin' better care of your hair, hun. All that blood from fightin' messes with your flavor...and your style, too, of course.",
						"[c/00BB00:*BWOoooORP!*]\n"
					  + "Ooo, excuuuuse me! Gotta say, hun, you're makin' me a lot gassier than I thought you would...guess it's all that scrumptious hair on your scalp.",
						"Hope you're enjoyin' my signature Gut Cut service! Remember, no refunds!",
						"Y'know, I think you'd be a REALLY good treat for Kyoko! I know she always loves to eat meals like you...\n"
					  + "...wait, who's Kyoko? OH, right! She's a really good friend of mine! I'm sure you'll get to meet her someday!",
					});
					if (player.Male)
					{
						stylistChatPool.AddRange(new List<string>
						{
							"You were delicious, sir! Unfortunately, the Gut Cut experience DOESN'T offer the option for added aftershave, on account of it tastin' awful and makin' me feel super bloated if I have any.",
							"You know, sir, I was originally gonna call this technique \"the Belly Barber experience\". Glad I went with \"the Gut Cut experience\" instead. Rolls off the tongue, and gets clients rollin' onto mine, way better.",
						});
					}
					else
					{
						stylistChatPool.AddRange(new List<string>
						{
							"You know, girl, I was originally gonna call this technique \"the Belly Barber experience\". Glad I went with \"the Gut Cut experience\" instead. Rolls off the tongue, and gets clients rollin' onto mine, way better.",
							"Ahhh...one of the tastiest gal pals I've had yet! Of course, nothing quite beats Kyoko when it's time for my weekly practice...",
						});
					}
					if (dyeTrader != null)
					{
						stylistChatPool.AddRange(new List<string>
						{
							"Hmm...I wonder...would " + dyeTrader.GivenName + " like how I look with you inside me? I remember him sayin' that a stuffed gut always looks nice in the right outfit...",
						});
					}

					if (noDigest)
					{
						stylistChatPool.AddRange(new List<string>
						{
							"Now, you relax and marinate for a while...I'll check in and gulp down a drink for you in half an hour or so.",
							"You want me to send a magazine in there for you to read while you relax inside me? I'm sure you do...well, a shame I don't have one.",
							"I hope you know you're not getting an actual haircut. My belly doesn't do hair for free, hun.",
						});
						if (player.Male)
						{
							stylistChatPool.AddRange(new List<string>
							{
								"Sir, please keep still. If I end up giving somebody a bad haircut because of you, I WON'T hesitate to make a meal outta you.",
							});
						}
						else
						{
							stylistChatPool.AddRange(new List<string>
							{
								"Just relax and let my gut straighten out those knots, honey...we GOTTA get you up to speed on the local gossip.",
								"Giiiirl, you are my favorite belly filler! Now, what were we chattin' about?",
								"You know, I think you're the tastiest gal pal I've ever had. Just about the most filling, too...\n"
							  + "[c/00BB00:*urp!*]",
							});
						}
					}
					else
					{
						stylistChatPool.AddRange(new List<string>
						{
							"Sorry, hun, no refunds! Just sit in there and marinate for a while...I'll check in to make sure you're digestin' well in half an hour or so.",
							"Well, while you wait to be digested, you've got an easy cut. Just stick your head in those acids for a bit, and your hair'll be nice and short!",
							"[c/BFBFBF:(...oh, don't make me fat, don't make me fat...make one of the three B's too big if you want, but please, please, PLEEEEEASE don't make me fat. I take too many cheat days as it is...)]",
						});
						if (player.Male)
						{
							stylistChatPool.AddRange(new List<string>
							{
								"Thank you for choosin' my services, Mr. " + player.name + "! I hope you enjoy your soon-to-be new look, because I run a strict no-refunds policy on gut cuts.",
								"Sir, I'll have to ask you to keep still. The digestive Gut Cut experience demands full cooperation from my clients, or else they end up digested AND unhappy with their hair."
							});
						}
						else
						{
							stylistChatPool.AddRange(new List<string>
							{
								"Just relax and let your split ends melt away, honey...we GOTTA get you up to speed on the local gossip before the acids get to you.",
								"Giiiirl, you were the best meal ever! Now, you take a nice, long acid soak in there, and I'll enjoy the bust you'll give me.",
								player.name + ", I gotta say, you made a great snack. Really hope your assets'll add onto mine...been feelin' a little small lately.",
							});
						}
						if (dyeTrader != null)
						{
							stylistChatPool.AddRange(new List<string>
							{
								"Try to plump up JUST the 3 B's a little for me, alright, hun? I really wanna look my best for the next time " + dyeTrader.GivenName + " comes over...",
								"Now, all I need you to do is bulk my body up a bit! Got a hunch that " + dyeTrader.GivenName + "'s got a thing for gals with good assets...and I'M gonna win his heart, just you wait!",
							});
						}
						bool bald = Main.CurrentPlayer.hair == 16 || Main.CurrentPlayer.head == ArmorIDs.Head.MonkBrows;
						if (bald)
						{
							stylistChatPool.AddRange(new List<string>
							{
								"\"Acid-worn and bald are the same\", you say? Well, lemme prove you WRONG, hun! My belly and I don't DO bald. Bald is never a good look. Acid-worn, on the other hand...",
							});
						}
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					stylistChatPool.AddRange(new List<string>
					{
						"Tipping's optional, but remember I have teeth sharper than any razor, plenty of room in my belly, and access to your head.",
						"Hun, you better stay outta my hair tonight...unless you wanna spend the night digesting to make more of it. Could really go for some good eats...",
						"I just sharpened my scissors, and your thighs look like they'd enhance mine really nicely after a quick gut cut. Are you sure you don't wanna pay extra?",
						"The longer you sit there, the more likely I am to just cut to the chase and cram you down my throat.",
						"How are you gonna get styled tonight? Better pick something fast, before I pick it for you.",
					});
					if (salad != null)
						stylistChatPool.Add("God, I could really go for a salad right about now...hun, go tell " + salad.GivenName + " to get over here and fill me up. Quickly, too, or you'll go right in with her.");
				}
				else
				{
					if (npc.AsPred().stomachContents.Count > 0)
					{
						switch (GetVisualBellySize(npc))
						{
							default:
								break;
							case 1:
								stylistChatPool.AddRange(new List<string>
								{
									"Mmm...always a bit weird to have a potbelly like this hangin' off me. Not like it matters much...I'll be back to my slim self in no time! Now, how can I get your cut done, hun?",
									"Sit down for a sec, and I'll have you steppin' razor. Maybe a salad steppin' into my belly, too...need something healthy to get this midriff flat again.",
									"Oh, this? That's not a baby! Well...not a real one. Just a food baby!...yeah, you can poke it, but not a whole lot! Just a little.",
									"Now, now, hun. A little bloating never hurt anybody. A few minutes, and this gut'll be back to its hottest shape. Thin is in right now...I think.",
									"Hm? Distracted by something? Maybe you want to fill out this gut a bit more than it is right now...?~\n"
								  + "\n"
								  + "...aw, don't be such a downer, sweetheart. I'm just teasing! Besides, I'm on a diet, anyway! Gotta keep this bod slim for the summer shores! Now, let's see about getting you the right cut...",
									"Hey! Don't mind the stuffed stomach, just had a bit of good food! How can I work my magic on your scalp today?",
									"You want some tea? Some orange juice?...maybe just some food? I've got a bit of food in me, too, so I wouldn't blame you for askin'.",
								});
								break;
							case 2:
								stylistChatPool.AddRange(new List<string>
								{
									"Mmm...always a bit weird to have a potbelly like this hangin' off me. Not like it matters much...I'll be back to my slim self in no time! Now, how can I get your cut done, hun?",
									"Gonna need a few salads steppin' into my belly at this rate...look at all this belly! Of course...a cheat day every once in a while's not bad, right?",
									"Oh my God, I look pregnant! Late into one, too...did I really eat that much? Then again, I think I know a hairstyle that'd look great with this gut...",
									"Oh, this? That's not a baby! Well...not a real one. Just a really big food baby! Be careful not to poke it, though! Wouldn't want whatever's built up in there knockin' you out...",
									"Does this belly make my hair look bad? Gimme an honest answer, and I promise I won't get...TOO mad. Gimme a lie, though...",
									"My belly looks like a bald head...I'm not sure if I like that. Then again, you could say the apron counts as a nice hairstyle for it...it should be fine.",
									"Oh, hey! Sorry about the gut; I'm just working on a nice meal I had earlier. It should be gone soon enough...in the meantime, what cut do you want?",
								});
								break;
							case 3:
								stylistChatPool.AddRange(new List<string>
								{
									"O- ooh...I look- [c/00FF00:*hic!*] -pregnant...overdue with twins, probably. I sure FEEL pregnant with twins, what with how heavy my belly is...",
									"I'm debating if this big belly looks good on me or not...what do you think, " + player.name + "? Could I make it work?...ah, who am I- [c/00FF00:*hic!*] -kidding? Of course I could.",
									"Please don't rest your head on my gut when I'm cutting your- [c/00FF00:*hic!*] -hair. It'll make the final product uneven.",
									"Hey, before I- [c/00FF00:*hic!*] -give you your cut...are my hands covered in saliva? Wouldn't want it messin' up your hair...",
								});
								if (PredNPC.AnyPreyStillAlive(npc))
								{
									stylistChatPool.AddRange(new List<string>
									{
										"Oh, you want a haircut? Just sit in the- [c/00FF00:*hic!*] -chair, I'll do yours while my belly works on the one I have it doin'...",
										"Alright, just a minute, I'll be with you once my current client's done... what kinda cut do you want? The Gut Cut experience's booked right- [c/00FF00:*hic!*] -now, so if you want one, you'll have to wait.",
									});
								}
								break;
							case 4:
								stylistChatPool.AddRange(new List<string>
								{
									"Oooh...I really ate a good bit, huh...well, I'm- [c/00FF00:*hic!*] -sure I can still cut your hair even with all this- [c/00FF00:*hic!*] -gut meat in the way, hun.",
									"...hey, could you- [c/00FF00:*hic!*] -pass me the orange juice? I think I could really- [c/00FF00:*hic-][c/00BB00:OURP!*] -use it right now...I always get bad- [c/00FF00:*hic!*] -hiccups after a good meal.",
									"Mmmf...if I have to carry this- [c/00FF00:*hic!*] -gut around much longer, I'm gonna have to- [c/00FF00:*hic!*] -cancel the rest of my clients for the day. I can't cut- [c/00FF00:*hic!*] -hair well with a belly this big!",
									"God, I feel so- [c/00FF00:*hic!*] -bloated...I'm sure there are people that are into this stuff, though. Maybe the right- [c/00FF00:*hic!*] -hairstyle can make it look better...",
									"Please don't- [c/00FF00:*hic!*] -rest your head on my gut when I'm cutting your- [c/00FF00:*hic!*] -hair. It'll make the final product uneven.",
									"Hey, before- [c/00FF00:*hic!*] -okay, I just wanted to check if my hands are- [c/00FF00:*hic!*] -...sticky with saliva...- [c/00FF00:*hic!*] -s- sorry about this, hun. I- [c/00FF00:*hic!*] -I get REAL bad hiccups when I'm this full.",
									"...eugh, all this food's gonna get me SO fat...if I eat, like, ANY more, I'm gonna need to ask Kyoko to shave off the weight later...",
								});
								if (PredNPC.AnyPreyStillAlive(npc))
								{
									stylistChatPool.AddRange(new List<string>
									{
										"O- oh, you want a- [c/00FF00:*hic!*] -haircut? A- alright, just sit in the- [c/00FF00:*hic!*] -chair, I'll do yo-[c/00BB00:*ooOOUURP!*] -...y- yours, alongside the cut my- [c/00FF00:*hic!*] -belly's working on.",
										"Alright, I- [c/00FF00:*hic!*] -I'll be with you in just a- [c/00FF00:*hic-][c/00BB00:OOORRP!*] -minute... what kinda cut do you want? The- [c/00FF00:*hic!*] -Gut Cut experience's booked right now- [c/00FF00:*hic!*] -so if you want one, you'll- [c/00FF00:*hic!*] -have to wait.",
									});
								}
								break;
						}
						if (npc.AsPred().stomachContents.FirstOrDefault(x => x.Type == PreyType.NPC && (x.Instance as NPC).type == NPCID.Dryad) is Prey dryadAsPrey)
						{
							if (!dryadAsPrey.Dead)
							{
								stylistChatPool.AddRange(new List<string>
								{
									"What? My- [c/00FF00:*hic!*] -gut? Sorry about that, just had a big, meaty- [c/00FF00:*hic!*] -salad as a healthy snack...she's still- [c/00FF00:*hic!*] -kickin' around a bit in there, too.",
									"Hey, can you- [c/00FF00:*hic!*] -help me calm down the plant gal in my gut? She's REALLY not- [c/00FF00:*hic!*] -agreeing with me...if you do, your next cut's- [c/00FF00:*hic-][c/00BB00:URP!*] -...50% off.",
									"Huh? Where's " + salad.GivenName + "? She's right- [c/00FF00:*hic!*] -here! If you want, you can always book a- [c/00FF00:*hic!*] -Gut Cut experience, too, though you- [c/00FF00:*hic!*] -might have to wait a little.",
									"A- [c/00FF00:*hic!*] -really good friend of mine says dryads are- [c/00FF00:*hic!*] -the greatest meals around! Proteins and- [c/00FF00:*hic!*] -plant fibers together, all in one- [c/00FF00:*hic-][c/00BB00:URP!*] -delicious meal! I just had to try one!",
								});
							}
							else if (GetVisualBellySize(npc) >= 3)
							{
								stylistChatPool.AddRange(new List<string>
								{
									"Mmm...I love it when a good- [c/00FF00:*hic-][c/00BB00:URP!*] -salad finally calms down. You wanna give this- [c/00FF00:*hic!*] -gut a little rub to help it break down that- [c/00FF00:*hic!*] -prissy plant gal?",
									"Hey, can you- [c/00FF00:*hic!*] -help me break down the plant gal in my gut? She's all calmed- [c/00FF00:*hic!*] -down now, but it'll take me way too- [c/00FF00:*hic!*] -long to churn her into nutrients...",
									"Mmmf...- [c/00FF00:*hic!*] -...I knew Kyoko's advice about dryads was- [c/00FF00:*hic!*] -good to follow. A few more of her type, and I'll- [c/00FF00:*hic!*] -be the hottest gal in the world in no time!",
								});
							}
						}
						if (npc.AsPred().stomachContents.FirstOrDefault(x => x.Type == PreyType.NPC && (x.Instance as NPC).type == NPCID.PartyGirl) is Prey partyGirlAsPrey)
						{
							if (!partyGirlAsPrey.Dead)
							{
								stylistChatPool.AddRange(new List<string>
								{
									"What? My- [c/00FF00:*hic!*] -gut? Sorry about that...lil' ol' " + partyGirl.GivenName + "- [c/00FF00:*hic!*] -INSISTED on spendin' some- [c/00FF00:*hic!*] -quality time in my gut as th- [c/00FF00:*hic!*] -thanks for doin' her hair so well.",
									"Huh? Where's " + partyGirl.GivenName + "? She's right- [c/00FF00:*hic!*] -here! She REALLY wanted a Gut Cut ex- [c/00FF00:*hic!*] -...experience, and I couldn't just say- [c/00FF00:*hic!*] -no! I- I'll...call it a cheat day.",
								});
							}
							else if (GetVisualBellySize(npc) >= 3)
							{
								stylistChatPool.AddRange(new List<string>
								{
									"Ooof...I know that gal wanted me to- [c/00FF00:*hic!*] -eat her, but I gotta say, hun...I'm kinda- [c/00FF00:*hic!*] -worried about how fat she's gonna make me. Don't wanna- [c/00FF00:*hic!*] -bulk up too much in the wrong places, y'know...?",
									"H- hun, I've got an honest- [c/00FF00:*hic!*] -question for you...does the cake-flavored gal currently- [c/00FF00:*hic!*] -churnin' away inside me make me look- [c/00FF00:*hic!*] -too fat? I...really hope not...",
								});
							}
						}
					}
					else
					{
						stylistChatPool.AddRange(new List<string>
						{
							"Tea? Coffee? Or do you just want some orange juice again? A lot of my clients just gulp down all three...they'd gulp me down, too, but I wasn't born yesterday.",
							"Sit down for a sec, and I'll have you steppin' razor. I guarantee, I'll give you the hottest cut around, or your next one's free.",
							"I'm tellin' you, hun, my hands are NOT coated with saliva right now. Yours, on the other hand...well, they might be, if you want my signature special cut!",
							"Hmm...yeah, definitely something low-maintenance for you. You look like the type who'd get " + (player.Male ? "him" : "her") + "self eaten just because a cute girl or guy asked.\n"
						  + "\n"
						  + "...you wouldn't mind ME askin', right? Just curious...",
							"Want me to take a little off the top? Need some scrubbed off the sides? Scissors, trimmers, razor-sharp teeth...you pick how you want what done!",
							"Welcome back! Want a quick, traditional haircut, or maybe you're here to book something more unique?\n"
						  + "\n"
						  + "...or are you just here to say hi?",
							"Either you have style, or you get styled. Gotta pick one or the other, hun."
						});
						if (player.Male)
						{
							stylistChatPool.AddRange(new List<string>
							{
								"Hey, " + player.name + "! I'm " + npc.GivenName + ", and I'll be your barber today. What can I do for you?",
								"Which aftershave can I getcha today, mister? I'm sure there's SOMETHING here that suits your style...and your flavor!",
								"So, what kinda cut are we talking? Buzz cut, pompadour, mohawk? Maybe a bit of acid-worn to go with them?",
							});
						}
						else
						{
							stylistChatPool.AddRange(new List<string>
							{
								"Oh, you poor thing...shhh, it's alright, just lean back in the chair, and lemme solve all those tangled-up knots...",
								"Giiiiiirl, we GOTTA get you some dye for that hair of yours. Would make it look even better!...and a bit tastier, but you don't have to be the tip.",
								"A pixie cut? Sounds good! Wanna keep some lady burns, or should I trim those, too?",
								"So, what kinda cut are we talking? A long ponytail, maybe some curls, a cute little bun at the back? Maybe a Gut Cut to add some acid-worn flair?",
							});
						}
						if (player.HasEaten("Terraria: Stylist", out int ateBestGirlHowMuch) && ateBestGirlHowMuch >= 3)
						{
							stylistChatPool.AddRange(new List<string>
							{
								"...hun, you've got a REAL hungry look on your face. You sure you wanna try and keep this feisty feast down? Gonna need to cancel my next few clients if I end up gut fodder for a bit...",
								"Aww, what's the matter. Gettin' all hot in the oven over lil' ol' me? Well, if you sit still for your cut, and you tip me good, I'll letcha have me for a little while, alright?",
								"Look, all I'm sayin' is that you BETTER not mess up my hair if-...no, WHEN you eat me. If my hair gets ruined because of you, you'll be a new, thick set of hips for me by sunrise tomorrow.",
							});
						}
						if (BirthdayParty.PartyIsUp)
						{
							stylistChatPool.AddRange(new List<string>
							{
								"Sure, I did my hair up just for today, but...honestly? I just wanna pop balloons with my scissors and have some cupcakes.",
								"If that birthday cake gets in ANYBODY'S hair, I think I'll just have 'em for lunch and call it a day. Every gal's gotta have a cheat day once in a while!",
							});
						}
						if (dyeTrader != null)
						{
							stylistChatPool.AddRange(new List<string>
							{
								"I tried one of " + dyeTrader.GivenName + "'s super-stylin' dyes once. Here's a tip, hun: the guy's a total hottie, but his wares are another story. That dye gave me indigestion for 4 hours straight, had me feeling like the fattest girl alive with how bad it bloated me, and went straight to my sides when it was finally over. Total disaster. Never again.",
								dyeTrader.GivenName + " always looks so stylish, no matter what he's got on...one of these days, I think I'll ask him out. Maybe show him my signature Gut Cut experience...I'm sure he'd be just the HOTTEST guy around with a nice, acid-worn scalp!~",
							});
						}
						if (partyGirl != null)
						{
							stylistChatPool.AddRange(new List<string>
							{
								"Hope you like what I did to " + partyGirl.GivenName + "'s hair! She WAS gonna give me a bunch of cupcakes as a tip, but she ate 'em all while I was doing her hair. She offered to let HERSELF be my tip instead, but...I'm on a diet.",
							});
						}
					}
				}
			}
			return stylistChatPool;
		}

		public static bool CanStylistBeForceFed(NPC npc) => true;

		public static void OnStylistForceFed(NPC npc, Player player)
		{
			PredNPC.SetChatboxText(
				npc,
				player,
				"[c/7F7F7F:<Squeaking in surprise as you start to cram yourself down her throat, " + npc.GivenName + " soon shrugs and helps you along your journey into her waiting stomach.>]\n"
			  + Main.rand.NextFromCollection(new List<string>
				{
					"Mmm...you REALLY wanted me to taste-test your scalp, didn't you- [c/00FF00:*hic!*] -hun? Lucky for you, I think it's- [c/00FF00:*hic!*] -PERFECT for a good little treat like you. Now- [c/00FF00:*hic!*] -don't move around too much...I've- [c/00FF00:*hic!*] -still got more haircuts to do, whether I'm- [c/00FF00:*hic!*] -stuffed with you or not!",
					"W- wow, you wanted one of my signature acid-worn hairdos REALLY bad, didn'tcha? Well, I'm more than happy to provide, since you don't seem to mind being my meal! Why don't you just marinate in there for a while and lemme know when you're done?",
					"Mmmf...something about how eager you are tells me you're NOT here for a haircut. If that's the case, I'm more than happy to have you served to me, " + (player.Male ? "sir" : "ma'am") + "! Enjoy your time as a meal, and remember: no refunds!",
				})
			  + "\n[c/7F7F7F:<After giving her now-full gut a comfy pat, she mutters something else about her diet under her breath. Guess it's a cheat day now.>]"
			);
		}

		public override void PostAI(NPC npc)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				return;

			if (npc.AsPred().stomachContents.Count > 0)
				return;

			if (Main.GameUpdateCount % 60 != 0)
				return;

			static void RollForRandomGulp(ref bool gutCut) => gutCut |= Main.rand.NextBool(5, 100);

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			bool shouldSnackOnSalad = false;
			RollForRandomGulp(ref shouldSnackOnSalad);
			RollForRandomGulp(ref shouldSnackOnSalad);
			if (salad != null && salad.Distance(npc.Center) <= npc.AsPred().swallowRange && shouldSnackOnSalad)
				PredNPC.Swallow(npc, salad);

			if (ModContent.GetInstance<V2ServerConfig>().NoRandomGulpsAgainstPlayers)
				return;

			if (!Main.CurrentPlayer.active || Main.CurrentPlayer.dead || Main.CurrentPlayer.Distance(npc.Center) > npc.AsPred().swallowRange || Main.CurrentPlayer.AsFood().IsCurrentlyEaten)
				return;

			bool bald = Main.CurrentPlayer.hair == 16 || Main.CurrentPlayer.head == ArmorIDs.Head.MonkBrows;
			bool hairDye = Main.CurrentPlayer.hairDye != 0;

			bool shouldGiveGutCut = false;
			RollForRandomGulp(ref shouldGiveGutCut);
			if (bald)
			{
				RollForRandomGulp(ref shouldGiveGutCut);
				RollForRandomGulp(ref shouldGiveGutCut);
			}
			else if (hairDye)
			{
				RollForRandomGulp(ref shouldGiveGutCut);
				RollForRandomGulp(ref shouldGiveGutCut);
				RollForRandomGulp(ref shouldGiveGutCut);
				RollForRandomGulp(ref shouldGiveGutCut);
			}

			if (shouldGiveGutCut)
			{
				if (bald)
				{
					PredNPC.SwallowWithTextIfApplicable(
						npc,
						Main.CurrentPlayer,
						"[c/7F7F7F:<With little warning, " + npc.GivenName + " stuffs you down her throat, headfirst. She gives a disgruntled groan as you settle into her stomach.>]\n"
					  + "Eugh, your head tasted HORRIBLE. So bland, no REAL taste to it...at least you have a decent base flavor. Serves you right for lettin' that bald BULB you call your head STRUT AROUND outside my gut! Maybe a quick \"marination\" and some time as a new set of hips for me'll make you think twice about not havin' any hair..."
					);
				}
				else if (hairDye)
				{
					PredNPC.SwallowWithTextIfApplicable(
						npc,
						Main.CurrentPlayer,
						"[c/7F7F7F:<With little warning, " + npc.GivenName + " stuffs you down her throat, headfirst. She gives a particularly gleeful hum as you settle into her stomach.>]\n"
					  + "Oooh...now THAT'S a great meal! Hate to drag you away, hun...I know I'm on a diet, and you're probably busy, but I just couldn't help myself! Your scalp looked WAY too tasty with that DELICIOUS hair dye I gave you...I just HAD to have a bite! Surely, you don't mind bein' a meal worth a cheat day for me, right?"
					);
				}
				else
				{
					PredNPC.SwallowWithTextIfApplicable(
						npc,
						Main.CurrentPlayer,
						"[c/7F7F7F:<With little warning, " + npc.GivenName + " stuffs you down her throat, headfirst. She gives a pleasant hum as you settle into her stomach.>]\n"
					  + "Oooh...now THAT'S a good meal! Sorry, hun...I know I'm supposed to be on a diet, but your scalp looked really tasty...! I bet a quick shave with the help of my belly would make you look even better, too! That, and I'm honestly pretty hungry...I'm sure ONE more cheat day can't hurt, yeah?"
					);
				}
			}
		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.1",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.2",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.3",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.5",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.6",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.7",
			});

			if (Main.bloodMoon)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.BloodMoonHaircut");
			}

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, Prey prey) => Main.bloodMoon ? 2.5 : 1.25;

		public static double GetDigestionTickDamage(NPC npc, Prey prey) => 20;

		public static void OnDigestionKill(NPC npc, Prey digestedPrey)
		{
			SoundEngine.PlaySound(
				digestedPrey.WeightLeftToDigest < 0.3 ? npc.AsPred().SmallBurps : npc.AsPred().StandardBurps,
				npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f)
			);
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 1,
				seconds: 45
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
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

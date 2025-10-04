using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Armor;
using V2.Items.Voraria.Consumables;
using V2.Items.Voraria.Accessories.Thingymajigs;
using V2.Items.Voraria.Placeables;
using V2.Items.Voraria.Weapons.Ranged;
using V2.PlayerHandling;
using V2.Projectiles.Voraria;
using V2.Sounds.Vore;

namespace V2.NPCs.Voraria.TownNPCs.Enigma
{
	public static class EnigmaStuff
	{
		public static EnigmaProfile EnigmaProfile = new EnigmaProfile();
	}

	public class EnigmaProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public EnigmaProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Voraria/TownNPCs/Enigma/Clover_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => "Clover";

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Voraria/TownNPCs/Enigma/Clover";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = 0;
			if (!V2.GetFooled)
				bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("V2/NPCs/Voraria/TownNPCs/Enigma/Clover_Head");
	}

	[AutoloadHead]
	public class Clover : ModNPC
	{
		const int BaseTownNPC = NPCID.Dryad;

		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override string Texture => "V2/NPCs/Voraria/TownNPCs/Enigma/Clover_WeightBase_BellyBase";
		public override string HeadTexture => "V2/NPCs/Voraria/TownNPCs/Enigma/Clover_Head";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 23;
			NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
			NPCID.Sets.AttackFrameCount[NPC.type] = 4;
			NPCID.Sets.DangerDetectRange[NPC.type] = 66;
			NPCID.Sets.AttackType[NPC.type] = 0;
			NPCID.Sets.AttackTime[NPC.type] = 30;
			NPCID.Sets.AttackAverageChance[NPC.type] = 1;
			NPCID.Sets.HatOffsetY[NPC.type] = -8;
			NPCID.Sets.ImmuneToRegularBuffs[NPC.type] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
				Direction = -1
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			NPC.Happiness
				.SetNPCAffection(NPCID.Truffle, AffectionLevel.Love)
				.SetNPCAffection(NPCID.Princess, AffectionLevel.Like)
				.SetNPCAffection(NPCID.Angler, AffectionLevel.Like)
				.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate)
				.SetBiomeAffection<MushroomBiome>(AffectionLevel.Love)
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Like)
				.SetBiomeAffection<JungleBiome>(AffectionLevel.Like)
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
				.SetBiomeAffection<CorruptionBiome>(AffectionLevel.Hate)
				.SetBiomeAffection<CrimsonBiome>(AffectionLevel.Hate)
				.SetBiomeAffection<DungeonBiome>(AffectionLevel.Hate);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.SurfaceMushroom,
				new FlavorTextBestiaryInfoElement("Mods.V2.Bestiary.TownNPCs.Enigma"),
			});
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.lifeMax = 500;
			NPC.damage = 35;
			NPC.defense = 38;
			NPC.knockBackResist = 0.5f;
			NPC.HitSound = SoundID.NPCHit1;
			AnimationType = BaseTownNPC;

			NPC.AsV2NPC().GetNewDialogue = GetEnigmaChat;

			NPC.AsFood().DefinedBaseSize = 1.15;
			NPC.AsPred().WeightGainRatio = 0.125;
			NPC.AsPred().MaxStomachCapacity = 2.2;
			NPC.AsPred().BaseStomachacheMeterCapacity = 90.0;
			NPC.AsFood().StruggleEffectiveness = 1; //have fun
            NPC.AsFood().WellFedPower = -7.77;

            NPC.AsPred().SmallGulps = Gulps.Short;
			NPC.AsPred().SmallGulpThreshold = 0.45;
			NPC.AsPred().BigGulps = Gulps.Standard;
			NPC.AsPred().CanBeForceFed = CanEnigmaBeForceFed;
			NPC.AsPred().OnForceFed = OnEnigmaForceFed;

			NPC.AsPred().DigestionType = EntityDigestionType.Acidic;
			NPC.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			NPC.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			NPC.AsPred().OnDigestionKill = null;
			NPC.AsPred().MouthSoundRawOffset = NPC.TrueCenter() + new Vector2(NPC.direction * 8f, -14f);
			NPC.AsPred().SmallBurps = Burps.Humanoid.Small;
			NPC.AsPred().SmallBurpThreshold = 0.75;
			NPC.AsPred().StandardBurps = Burps.Humanoid.Standard;
			NPC.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			NPC.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			NPC.AsPred().GetVisualBellySize = GetVisualBellySize;

			NPC.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
		}
		public override void ModifyTypeName(ref string typeName) => typeName = "Enigma";

		public override bool CanTownNPCSpawn(int numTownNPCs) => ModContent.GetInstance<V2MasterSystem>().freedEnigma;

		public override ITownNPCProfile TownNPCProfile() => EnigmaStuff.EnigmaProfile;

		public static List<string> GetEnigmaChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);

			List<string> EnigmaChatPool = new List<string>();
			V2Utils.FigureOutWhatTimeItIs(
				out bool pastMorning,
				out int hour,
				out int minute,
				out int second,
				out MealTime mealTime
			);
			double totalBellyWeight = PredNPC.GetCurrentBellyWeight(npc);
			bool playerIsFood = player.IsFoodFor(npc, out bool playerWasAlreadyDigested);
			bool isFoodForPlayer = npc.IsFoodFor(player);
			if (playerIsFood && !playerWasAlreadyDigested)
			{
				{
					EnigmaChatPool.AddRange(new List<string>
					{
						"So...uh...how long are you going to stay there?",
						"Hmm... I mean, I don't dislike this, I suppose.",
						"Well, I hope you're having fun doing your...thing?",
					});
				}
			}
			else
			{
				{
					/*if (PredNPC.GetStomachTracker(npc) is not null)
					{
						switch (GetVisualBellySize(npc))
						{
							default:
								EnigmaChatPool.AddRange(new List<string>
								{
									"I can't figure out if my gut is or isn't empty. How about you give me a little help solvin' that problem?",
									"The worst kind of food is the kind that doesn't bloat up your gut. Howzabout you be a better brunch than whatever's currently in or out of mine?",
								});
								break;
							case 1:
								EnigmaChatPool.AddRange(new List<string>
								{
									"Hey. Don't mind the- [c/00BB00:*urp!*] -rumbles from my waist. Just ate a little appetizer.",
									"Hm? Oh, this? Well, a day's always better with a good snack! Now, why don't you help turn that into a decent meal?",
									"Oh, this? Just had a light snack, that's all. Think you'll wanna add to that?",
									"Hey. Just had a- [c/00FF00:*hic!*] -little treat for myself. Don't mind me.",
								});
								break;
							case 2:
								EnigmaChatPool.AddRange(new List<string>
								{
									"[c/00BB00:*belch!*]\nAhhh, that's the good stuff. Nothing like a bloated little belly to make a better day. Definitely want some more, though.",
									"Just finished up a good appetizer here. Could go for a lot more, though...you offerin'?",
									"Huh? Oh, this. Just a nice little- [c/00BB00:*burp!*] -snack to tide me over until lunch. Don't worry about it, lunch.",
									"Finally got the starts of a good meal goin' here. Mind if I make it a little better with you as the next course?",
									"Hey there, f-[c/00BB00:*oourp.*]- ...food. Care to help me fix up my post-snack munchies a little better?",
								});
								break;
							case 3:
								EnigmaChatPool.AddRange(new List<string>
								{
									"[c/00BB00:*BWORP!*]\nTHERE we go...that's some good eats right in there, though I'll go for a little more, I think. Got any recommendations, such as yourself?",
									"Nice day, yeah? Even better now that I've got a good-sized meal in my gut. Then again...could always use a second course.",
									"Huh? Yeah, I could go for more. I've still got a TON of room in this gut for morsels like you, just you wait!",
								});
								break;
							case 4:
								EnigmaChatPool.AddRange(new List<string>
								{
									"[c/00BB00:*BWOOOURRP!*]\nNow THAT's a good meal. Almost makes me not wanna eatcha...almost. You free to be food?",
									"Huh? Yeah, I could go for more! Just...takin' a minute before I do. Don't wanna eat too fast and get hiccups.",
									"You know, if you're lookin' for a way to do this sorta thing yourself, the dragonfruit vine tells me there's a heavenly little treat who'd just LOVE to cram herself down your throat...unless you'd rather I get to her first.",
									"What's the matter, hotshot? Jealous of my big, food-filled belly? You should be, because I'm just- [c/00FF00:*hic!*] -...er...j- just gettin' started.",
								});
								break;
						}
					}
					else*/
					{
						EnigmaChatPool.AddRange(new List<string>
						{
							"Sup.",
							"Hey!",
							"Hello!",
							"AH! Hi, I didn't notice you!",
						});

						if (Main.dayTime)
						{
							EnigmaChatPool.AddRange(new List<string>
							{
								"Sure is bright today! Uh... Yeah. What's up?",
							});

							if (Main.IsItAHappyWindyDay)
							{
								EnigmaChatPool.AddRange(new List<string>
								{
									"Agh, so windy! I don't wanna lose my hat!! Oh, hey there!",
								});
							}
						}
						else
						{
							EnigmaChatPool.AddRange(new List<string>
							{
								"Ah, the cool breeze during nights feels so nice. Oh, 'sup.",
							});
						}

						if (Main.IsItRaining)
						{
							EnigmaChatPool.AddRange(new List<string>
							{
								"Do you ever think about what a rain cloud would taste like?",
							});
						}
						if (Main.IsItStorming)
						{
							EnigmaChatPool.AddRange(new List<string>
							{
								"Augh, the damn lightning! ...wait, do you think, if I got struck by lightning, my magic would become stronger? Never mind, that's stupid.",
							});
						}
					}
				}
			}
			return EnigmaChatPool;
		}

		public override void AddShops()
		{
			NPCShop EnigmaShop = new NPCShop(NPC.type, "Shop");
			EnigmaShop.Add<CloverHeadAccessories>();
			EnigmaShop.Add<CloverSweater>();
			EnigmaShop.Add<CloverStockings>();
<<<<<<< Updated upstream
            EnigmaShop.Add<BlankJuju>();
            EnigmaShop.Add<DemonCandy>();
            EnigmaShop.Add<GhostBall>();
            EnigmaShop.Add<MyFairy>([Condition.InGlowshroom]);
            EnigmaShop.Add<DinnerBlaster>([Condition.NpcIsPresent(NPCID.Cyborg)]);
            EnigmaShop.Register();
=======
			EnigmaShop.Add<BlankThingy>();
			EnigmaShop.Add<DemonCandy>();
			EnigmaShop.Add<GhostBall>();
			EnigmaShop.Add<MyFairy>([Condition.InGlowshroom]);
			EnigmaShop.Add<DinnerBlaster>([Condition.NpcIsPresent(NPCID.Cyborg)]);
			EnigmaShop.Register();
>>>>>>> Stashed changes
		}

		public static bool CanEnigmaBeForceFed(NPC npc) => true;

		public static void OnEnigmaForceFed(NPC npc, Player player)
		{
			PredNPC.SwallowWithTextIfApplicable(
				npc,
				player,
				"[c/7F7F7F:<After a quick glance at Clover, you jump into her mouth without warning, rocketing almost cartoonishly down her throat as she's left hacking and coughing for a moment in an attempt to recover some air (and general throat control) forced out of her by the sudden snack.>]\n"
			  + Main.rand.NextFromCollection(new List<string>
				{
					"[c/7F7F8F:*cough*] What!? Uh, wait, I... Well, I guess I don't have to make dinner for myself now? If you do plan on staying there, that is??",
					"Oh. Well, I hope this won't have any consequences on m- [c/00BF00:*hic!*] -...me.",
					"...what? On purpose? Jeez, the people here can be so weird sometimes...",
				})
			);
		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			/*deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.5",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.Hardcore");
			}*/
		}
		public override bool UsesPartyHat() => true;
		public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 47;
			knockback = 28f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 2;
			randExtraCooldown = 1;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ModContent.ProjectileType<CLOVERPUNCH>();
			attackDelay = 4;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 5f;
			randomOffset = 0f;
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.8;

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 2;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{

		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 7
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				0
			);
		}

        public int LastWalkFrame = 0;

        public override void FindFrame(int frameHeight)
		{
			NPC.frame.Width = 194;

			if (NPC.ai[0] == 1)
            {
                int walkFrame = LastWalkFrame;
                if (!Main.gamePaused)
                {
                    walkFrame = (int)(Main.GlobalTimeWrappedHourly * 8) % 6;
                }
                NPC.frame.Y = (walkFrame + 2) * NPC.frame.Height;
            }
		}

		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
		{
			boundingBox = new Rectangle(
				(int)NPC.Center.X - 16,
				(int)NPC.Center.Y - 18,
				32,
				44
			);
		}
	}
}

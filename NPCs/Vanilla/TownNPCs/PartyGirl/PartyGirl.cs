using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Accessories;
using V2.Items.Voraria.Accessories.Vanity;
using V2.NPCs.Voraria.TownNPCs;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.TownNPCs.PartyGirl
{
	public static class PartyGirlStuff
	{
		public static PartyGirl AsPartyGirl(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out PartyGirl bellyPartyGirl))
				throw new Exception("this instance of the Party Girl can't be pred or prey. that gut party you wanted to throw'll need to be rescheduled");

			return bellyPartyGirl;
		}
		public static PartyGirlPredProfile PartyGirlPredProfile => new PartyGirlPredProfile();
	}

	public class PartyGirlPredProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public PartyGirlPredProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/PartyGirl/PartyGirl_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/PartyGirl/PartyGirl";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.PartyGirl;
	}

	public partial class PartyGirl : GlobalNPC
	{
		public int HungerForEmpress { get; set; }
		public static int MaxHungerForEmpress => V2Utils.SensibleTime(seconds: 25);

		public int SpecialGutFrameCounter;
		public int SpecialGutFrame;
		public int SpecialGutFrames;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.PartyGirl && !V2.GetFooled;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.lifeMax = 400;

			npc.AsV2NPC().NewAIMethod = V2PartyGirlAI;

			npc.AsV2NPC().GetNewDialogue = GetPartyGirlChat;

			npc.AsFood().DefinedSize = 1.0;
			npc.AsPred().MaxStomachCapacity = 999999.0;
			npc.AsPred().BaseStomachacheMeterCapacity = 999999.0;

			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().CanSwallowBosses = true;
			npc.AsPred().CanBeForceFed = CanPartyGirlBeForceFed;
			npc.AsPred().OnForceFed = OnPartyGirlForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			npc.AsPred().OnDigestionKill = OnDigestionKill;
			npc.AsPred().MouthSoundRawOffset = npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f);
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => PartyGirlStuff.PartyGirlPredProfile;

		public static bool V2PartyGirlAI(NPC npc)
		{
			VoreTracker tracker = PredNPC.GetStomachTracker(npc);
			if (tracker is null)
				goto ResetFrame;

			PreyData candyFairy = null;
			if (tracker.Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss) is PreyData sprinkles && sprinkles.WeightLeftToDigest > 5.0)
				candyFairy = sprinkles;
			if (tracker.PreyQueue.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss) is PreyData sprinklesQueue && sprinklesQueue.WeightLeftToDigest > 5.0)
				candyFairy = sprinklesQueue;
			bool ateCandyFairy = tracker is not null;
			ateCandyFairy &= candyFairy is not null;
			if (ateCandyFairy)
			{
				npc.width = 110;
				npc.height = 64;
				npc.velocity.X = 0;
				npc.AsPartyGirl().SpecialGutFrames = 10;
				npc.AsPartyGirl().SpecialGutFrameCounter += 1;
				if (npc.AsPartyGirl().SpecialGutFrameCounter >= 9)
				{
					npc.AsPartyGirl().SpecialGutFrameCounter = 0;
					npc.AsPartyGirl().SpecialGutFrame += 1;
					npc.AsPartyGirl().SpecialGutFrame %= npc.AsPartyGirl().SpecialGutFrames;
				}

				if (!candyFairy.NoHealth)
				{
					for (int y = (int)Math.Round(npc.TrueCenter().Y) - 5; y < (int)Math.Round(npc.TrueCenter().Y); y++)
					{
						for (int x = (int)Math.Round(npc.TrueCenter().X) - 4; x < (int)Math.Round(npc.TrueCenter().X) + 4; x++)
						{
							WorldGen.KillTile(x, y);
						}
					}
				}
				return false;
			}

			ResetFrame:
			npc.width = 14;
			npc.height = 40;
			npc.AsPartyGirl().SpecialGutFrames = 0;
			npc.AsPartyGirl().SpecialGutFrame = 0;
			npc.AsPartyGirl().SpecialGutFrameCounter = 0;

			return true;
		}

		public override void ModifyShop(NPCShop shop)
		{
			if (shop.NpcType != NPCID.PartyGirl)
				return;

			shop.Add(
				ModContent.ItemType<BalloonBelly>(),
				new Condition("Mods.V2.ItemObtainmentDetails.Voraria.Accessories.Vanity.BalloonBelly", () => {
					if (Main.LocalPlayer.AsPred().TotalStatPoints >= 10)
						return true;

					return false;
				})
			);
		}

		public override void PostAI(NPC npc)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			if (GetEmpressDigestionStage(npc) > 0)
				return;

			if (Main.GameUpdateCount % 60 != 0)
				return;

			if (NPC.AnyNPCs(NPCID.HallowBoss))
			{
				npc.AsPartyGirl().HungerForEmpress += 1;
			}

			static void RollForRandomGulp(ref bool gulp) => gulp |= Main.rand.NextBool(4, 100);

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC foxBimbo = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.BestiaryGirl);
			bool shouldSnackOnFoxBimbo = false;
			RollForRandomGulp(ref shouldSnackOnFoxBimbo);
			if (foxBimbo != null && foxBimbo.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldSnackOnFoxBimbo)
				PredNPC.Swallow(npc, foxBimbo);

			NPC nurse = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Nurse);
			bool shouldSnackOnNurse = false;
			RollForRandomGulp(ref shouldSnackOnNurse);
			RollForRandomGulp(ref shouldSnackOnNurse);
			RollForRandomGulp(ref shouldSnackOnNurse);
			if (nurse != null && nurse.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldSnackOnNurse)
				PredNPC.Swallow(npc, nurse);
			bool haveRoutineCheckUpWithNurse = false;
			RollForRandomGulp(ref haveRoutineCheckUpWithNurse);
			if (nurse != null && nurse.Distance(npc.Center) <= nurse.AsPred().MaxSwallowRange && haveRoutineCheckUpWithNurse)
				PredNPC.Swallow(nurse, npc);

			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			bool spendQualityTimeInAmber = false;
			RollForRandomGulp(ref spendQualityTimeInAmber);
			RollForRandomGulp(ref spendQualityTimeInAmber);
			if (bestGirl != null && bestGirl.Distance(npc.Center) <= bestGirl.AsPred().MaxSwallowRange && spendQualityTimeInAmber)
				PredNPC.Swallow(bestGirl, npc);

			NPC wizard = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Wizard);
			bool shouldSnackOnWizard = false;
			RollForRandomGulp(ref shouldSnackOnWizard);
			if (wizard != null && wizard.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldSnackOnWizard)
				PredNPC.Swallow(npc, wizard);

			NPC scrooge = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.TaxCollector);
			bool shouldSnackOnScrooge = false;
			RollForRandomGulp(ref shouldSnackOnScrooge);
			RollForRandomGulp(ref shouldSnackOnScrooge);
			RollForRandomGulp(ref shouldSnackOnScrooge);
			RollForRandomGulp(ref shouldSnackOnScrooge);
			RollForRandomGulp(ref shouldSnackOnScrooge);
			if (scrooge != null && scrooge.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldSnackOnScrooge)
				PredNPC.Swallow(npc, scrooge);

			if (ModContent.GetInstance<V2ServerConfig>().NoRandomGulpsAgainstPlayers)
				return;

			if (!Main.CurrentPlayer.active || Main.CurrentPlayer.dead || Main.CurrentPlayer.Distance(npc.Center) > npc.AsPred().MaxSwallowRange || Main.CurrentPlayer.CurrentCaptor() is not null)
				return;

			bool shouldHostGutParty = false;
			RollForRandomGulp(ref shouldHostGutParty);

			if (shouldHostGutParty)
			{
				PredNPC.SwallowWithTextIfApplicable(
					npc,
					Main.CurrentPlayer,
					"[c/7F7F7F:<Out of nowhere, " + npc.GivenName + " stuffs your entire body into her mouth, gulping you down in a single, smooth swallow. She giggles as your form rockets into her all-too-eager stomach.>]\n"
				  + "There we go! Sorry if I scared ya a bit...I needed a little pre-party snack, and that cake did just the trick! Thanks for the treat! :D"
				);
			}
		}

		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
		{
			if (projectile.type is ProjectileID.HallowBossSplitShotCore
								or ProjectileID.HallowBossRainbowStreak
								or ProjectileID.HallowBossLastingRainbow
								or ProjectileID.FairyQueenHymn
								or ProjectileID.FairyQueenLance
								or ProjectileID.FairyQueenSunDance)
			{
				double mult = 1.0;
				mult -= (double)npc.AsPartyGirl().HungerForEmpress / (double)MaxHungerForEmpress;
				mult = 0.2 + (mult * 0.8);
				modifiers.FinalDamage *= (float)mult;
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey)
		{
			double tickRate = 1.25;
			if (prey.Type == PreyType.NPC && (prey.Instance as NPC).type == NPCID.HallowBoss)
				return 4.0;
			else
			{
				if (BirthdayParty.PartyIsUp)
					tickRate *= 0.4;
				if (prey.Type == PreyType.NPC && (prey.Instance as NPC).type is NPCID.BestiaryGirl or NPCID.Wizard)
					tickRate *= 0.5;
				if (prey.Type == PreyType.NPC && (prey.Instance as NPC).type == NPCID.TaxCollector)
					tickRate *= 1.5;
			}
			return tickRate;
		}

		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			double digestionDamage = 15.0;
			if (prey.Type == PreyType.NPC && (prey.Instance as NPC).type == NPCID.TaxCollector)
				digestionDamage += 10.0;
			if (prey.Type == PreyType.NPC && (prey.Instance as NPC).type == NPCID.HallowBoss)
				digestionDamage += 85.0;

			return digestionDamage;
		}

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			// add confetti belch!
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 1,
				seconds: 10
			);
			return baseAbsorptionRate;
		}

		public static int GetEmpressDigestionStage(NPC npc)
		{
			if (PredNPC.GetStomachTracker(npc) is null)
				return 0;

			PreyData sprinkles = PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss);
			if (sprinkles is null || sprinkles.WeightLeftToDigest < 5.0)
				return 0;
			else
			{
				if (!sprinkles.NoHealth)
					return 1;
				else
				{
					if (sprinkles.WeightLeftToDigest > 37.0)
						return 1;
					else if (sprinkles.WeightLeftToDigest > 34.0 && sprinkles.WeightLeftToDigest <= 37.0)
						return 2;
					else if (sprinkles.WeightLeftToDigest > 31.5 && sprinkles.WeightLeftToDigest <= 34.0)
						return 3;
					else if (sprinkles.WeightLeftToDigest > 29.0 && sprinkles.WeightLeftToDigest <= 31.5)
						return 4;
					else if (sprinkles.WeightLeftToDigest > 4.0)
						return 5;
					else
						return 0;
				}
			}
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				5
			);
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
			npc.frame.Width = 160;
		}

		public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
		{
			if (GetEmpressDigestionStage(npc) > 0)
			{
				boundingBox = new Rectangle(
					(int)npc.Center.X - 55,
					(int)npc.Center.Y - 32,
					110,
					66
				);
			}
			else
			{
				boundingBox = new Rectangle(
					(int)npc.Center.X - 18,
					(int)npc.Center.Y - 27,
					36,
					54
				);
			}
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			if (GetEmpressDigestionStage(npc) > 0)
			{
				SpriteEffects spriteEffects = npc.direction switch
				{
					-1 => SpriteEffects.None,
					_ => SpriteEffects.FlipHorizontally,
				};
				string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/PartyGirl/PartyGirl";
				string weightString = "_WeightBase";
				exactTextureToUse += weightString;
				int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
				string bellyString = "_BossBelly_EmpressOfLight_DigestionStage" + GetEmpressDigestionStage(npc);
				exactTextureToUse += bellyString;

				Texture2D texture = ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad).Value;
				Rectangle sourceRect = new Rectangle(0, npc.AsPartyGirl().SpecialGutFrame * 68, 110, 68);
				spriteBatch.Draw
				(
					texture,
					npc.Center - screenPos + new Vector2(0f, npc.gfxOffY),
					sourceRect,
					drawColor,
					npc.rotation,
					sourceRect.Size() / 2f,
					1,
					spriteEffects,
					0f
				);
				return false;
			}
			return true;
		}
	}
}

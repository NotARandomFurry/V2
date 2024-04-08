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
using V2.Items.Voraria;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.BloodMoon
{
	public static class TheGroomStuff
	{
		public static class ItemTheftRules
		{
			public static ItemTheftRule TopHat => new ItemTheftRule(
				type: (npc, pred) => ItemID.TopHat,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => 1.0
			);
			public static ItemTheftRule TuxedoShirt => new ItemTheftRule(
				type: (npc, pred) => ItemID.TuxedoShirt,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 1.0,
						GameModeID.Expert => 4.0 / 5.0,
						_ => 2.0 / 3.0,
					};
				}
			);
			public static ItemTheftRule TuxedoPants => new ItemTheftRule(
				type: (npc, pred) => ItemID.TuxedoPants,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 1.0,
						GameModeID.Expert => 4.0 / 5.0,
						_ => 2.0 / 3.0,
					};
				}
			);
		}

		public static TheGroom AsTheGroom(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out TheGroom hungryZombieWife))
				throw new Exception("this instance of The Bride, supposedly, doesn't exist");

			return hungryZombieWife;
		}
	}

	public partial class TheGroom : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.TheGroom;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Male;
			npc.AsV2NPC().NewAIMethod = V2TheGroomAI;

			npc.AsFood().DefinedBaseSize = 1.04;
			npc.AsPred().MaxStomachCapacity = 1.5;
			npc.AsPred().BaseStomachacheMeterCapacity = 145.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.5;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(4.7);
			npc.AsPred().CanBeForceFed = CanTheGroomBeForceFed;
			npc.AsPred().OnForceFed = OnTheGroomForceFed;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
			npc.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().StandardBurps = Burps.Humanoid.Zombie.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().OnKilledByDigestion = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
			npc.AsFood().OnKilledByDigestion += PreyNPC.HandlePreyItemTheft;
			npc.AsFood().OnKilledByDigestion += OnKilledByDigestion_GrantBrideAndGroomGoal;
			npc.AsFood().ItemTheftRules = new List<ItemTheftRule>()
			{
				TheGroomStuff.ItemTheftRules.TopHat,
				TheGroomStuff.ItemTheftRules.TuxedoShirt,
				TheGroomStuff.ItemTheftRules.TuxedoPants,
			};
		}

		public static bool CanTheGroomBeForceFed(NPC npc) => true;

		public static void OnTheGroomForceFed(NPC npc, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.Zombie.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.Zombie.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Forest.Zombie.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.BloodMoon.GroomAndBride.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.BloodMoon.GroomAndBride.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.BloodMoon.GroomAndBride.TheGroom.1",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.BloodMoon.GroomAndBride.TheGroom.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.7;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 18;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{

		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 12,
				seconds: 0
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

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(0.20 * Math.Sqrt(npc.AsPred().ExtraWeight)),
				0
			);
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
			npc.frame.Width = 150;
		}

		public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
		{
			boundingBox = new Rectangle(
				(int)npc.Center.X - 17,
				(int)npc.Center.Y - 26,
				34,
				52
			);
		}

		public static void OnKilledByDigestion_GrantBrideAndGroomGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				PreyData bride = predPlayer.AsPred().StomachTracker?.Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.TheBride && x.NoHealth);
				if (bride is not null)
					ModContent.GetInstance<EatBrideAndGroom>().TrySetCompletion(predPlayer);
			}
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			int weightStage = npc.AsPred().GetVisualWeightStage.Invoke(npc);
			string weightString = "_Weight" + (weightStage == 0 ? "Base" : weightStage);
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);

			string exactMainBodyTexture = "V2/NPCs/Vanilla/BloodMoon/TheGroom" + weightString + bellyString;
			TextureAssets.Npc[NPCID.TheGroom] = ModContent.Request<Texture2D>(exactMainBodyTexture, AssetRequestMode.ImmediateLoad);
			return true;
		}
	}
}

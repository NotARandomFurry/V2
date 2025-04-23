using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Vanilla.TownNPCs.Dryad;
using V2.NPCs.Voraria.TownNPCs.Enigma;
using V2.NPCs.Voraria.TownNPCs.Succubus;

namespace V2.NPCs.Vanilla.BloodMoon
{
	public partial class TheBride : GlobalNPC
	{
		public static bool V2TheBrideAI(NPC npc)
		{
			VoreTracker tracker = PredNPC.GetStomachTracker(npc);
			if (tracker is null)
				goto ResetFrame;

			PreyData candyFairy = null;
			if (tracker.Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss) is PreyData sprinkles && sprinkles.WeightLeftToDigest > 4.0)
				candyFairy = sprinkles;
			if (tracker.PreyQueue.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss) is PreyData sprinklesQueue && sprinklesQueue.WeightLeftToDigest > 4.0)
				candyFairy = sprinklesQueue;
			bool ateCandyFairy = tracker is not null;
			ateCandyFairy &= candyFairy is not null;
			if (ateCandyFairy)
			{
				if (npc.width == 18 && npc.height == 40)
				{
					npc.width = 114;
					npc.height = 54;
					npc.position.X -= 114 - 18;
					npc.position.Y -= 54 - 40;
				}
				npc.velocity.X = 0;
				if (!candyFairy.NoHealth)
				{
					NPC realCandyFairy = candyFairy.Instance as NPC;
					if (npc.AsV2NPC().CustomSprite is null)
						npc.AsV2NPC().CustomSprite = new TheBrideStuff.Animations.OVEmpressOfLight.Intact();
					for (int y = (int)Math.Round(npc.TrueCenter().Y) - 5; y < (int)Math.Round(npc.TrueCenter().Y); y++)
					{
						for (int x = (int)Math.Round(npc.TrueCenter().X) - 4; x < (int)Math.Round(npc.TrueCenter().X) + 4; x++)
						{
							WorldGen.KillTile(x, y);
						}
					}
				}
				else
				{
					if (npc.AsV2NPC().CustomSprite is null)
						npc.AsV2NPC().CustomSprite = new TheBrideStuff.Animations.OVEmpressOfLight.Intact();
					else if (npc.AsV2NPC().CustomSprite is TheBrideStuff.Animations.OVEmpressOfLight.Intact && npc.AsV2NPC().CustomSprite.CanTransitionToNewAnim && GetEmpressDigestionStage(npc) >= 2)
						npc.AsV2NPC().CustomSprite = new TheBrideStuff.Animations.OVEmpressOfLight.DigestStage1();
					else if (npc.AsV2NPC().CustomSprite is TheBrideStuff.Animations.OVEmpressOfLight.DigestStage1 && npc.AsV2NPC().CustomSprite.CanTransitionToNewAnim && GetEmpressDigestionStage(npc) >= 3)
						npc.AsV2NPC().CustomSprite = new TheBrideStuff.Animations.OVEmpressOfLight.DigestStage2();
					else if (npc.AsV2NPC().CustomSprite is TheBrideStuff.Animations.OVEmpressOfLight.DigestStage2 && npc.AsV2NPC().CustomSprite.CanTransitionToNewAnim && GetEmpressDigestionStage(npc) >= 4)
						npc.AsV2NPC().CustomSprite = new TheBrideStuff.Animations.OVEmpressOfLight.DigestStage3();
				}
				return false;
			}

			ResetFrame:
			if (npc.AsV2NPC().CustomSprite is not null)
				npc.AsV2NPC().CustomSprite = null;
			if (npc.width != 18)
				npc.width = 18;
			if (npc.height != 40)
				npc.height = 40;

			Entity targetEntity = null;
			if (npc.AsV2NPC().TargetIndex == -1)
				npc.TryFindNewTarget(Diet);
			else
				npc.TryVerifyRemainingTarget(Diet);
			if (npc.AsV2NPC().TargetIndex != -1)
			{
				targetEntity = npc.AsV2NPC().TargetType switch
				{
					TargetType.Player => Main.player[npc.AsV2NPC().TargetIndex],
					TargetType.NPC => Main.npc[npc.AsV2NPC().TargetIndex],
					TargetType.Projectile => Main.projectile[npc.AsV2NPC().TargetIndex],
					_ => null,
				};
			}

			if (npc.AsV2NPC().BehaviorPattern is not null)
				npc.AsV2NPC().BehaviorPattern.DoBehavior(npc, targetEntity);
			else
				npc.SwitchToPattern<TheBrideAI.AimlessWanderingStill>(targetEntity);

			return false;
		}
	}
}

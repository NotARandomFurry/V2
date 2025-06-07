using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.Items.Vanilla.Accessories.VoodooDolls
{
	public class GuideVoodooDoll : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GuideVoodooDoll;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 250;
			item.AsFood().Size = 0.25;

			item.AsFood().OnBreak += OnBreak_GrantVoodooDigestionGoal;
		}

		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			foreach (NPC npc in Main.ActiveNPCs)
			{
				if (npc.type == NPCID.Guide)
				{
					item.AsFood().Health = npc.life;
					break;
				}
			}
		}

		public static bool OnBreak_GrantVoodooDigestionGoal(Item item, Entity pred, bool direct)
		{
			if (pred is Player predPlayer)
			{
				// Terraria.Item.CheckLavaDeath
				if (predPlayer.AsPred().StomachTracker.ContainsLiquid(LiquidID.Lava))
				{
					NPC.SpawnWOF(pred.Center);
				}

				ModContent.GetInstance<DigestWithVoodoo>().TrySetCompletion(predPlayer);
			}

			return true;
		}
	}
}
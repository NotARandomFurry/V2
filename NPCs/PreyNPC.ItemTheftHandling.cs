using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.Items;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.Sounds.MuffledSounds;

namespace V2.NPCs
{
	public partial class PreyNPC : GlobalNPC
	{
		public static double LeftoversHealthModifier => 0.9;
		public List<DigestionLootRule> ItemTheftRules { get; set; }
		public static void HandlePreyItemTheft(NPC npc, Entity pred)
		{
			if (!npc.CanItemsBeThievedBy(pred))
				return;

			if (npc.AsFood().ItemTheftRules is null || npc.AsFood().ItemTheftRules.Count <= 0)
				return;

			foreach (DigestionLootRule rule in npc.AsFood().ItemTheftRules)
			{
				double ruleChance = rule.ItemChance.Invoke(npc, pred);
				if (pred is not Player)
					ruleChance /= 10.0;
				double chanceCheck = Main.rand.NextDouble();
				if (chanceCheck >= ruleChance)
					continue;

				Vector2 mouthOffset = Vector2.Zero;
				if (pred is Player predPlayer)
				{
					mouthOffset = new Vector2(predPlayer.direction * 8f, -14f);
				}
				else if (pred is NPC predNPC)
				{
					mouthOffset = PredNPC.MouthSoundOffset(predNPC);
				}
				int itemType = rule.ItemType.Invoke(npc, pred);
				int itemAmount = rule.ItemAmount.Invoke(npc, pred);
				int belchedUpLeftovers = CommonCode.DropItem(
					pred.TrueCenter() + mouthOffset,
					npc.GetSource_Loot(
						"V2: Digestion Kill Item Theft"
					),
					itemType,
					itemAmount
				);
				Item belchedUpItem = Main.item[belchedUpLeftovers];
				belchedUpItem.AsFood().Health = (int)Math.Round(LeftoversHealthModifier * belchedUpItem.AsFood().MaxHealth);
				belchedUpItem.position += new Vector2(Main.rand.NextFloat(1f), 0).RotatedByRandom(MathHelper.ToRadians(360));
				belchedUpItem.velocity = new Vector2(pred.direction * 10f, -2.5f);
				belchedUpItem.velocity *= Main.rand.NextFloat(0.98f, 1.02f);
				belchedUpItem.velocity = belchedUpItem.velocity.RotatedByRandom(MathHelper.ToRadians(12));
				belchedUpItem.noGrabDelay = 100;
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, belchedUpLeftovers, 1f);
			}
		}
	}
}

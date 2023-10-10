using Humanizer;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.NPCs.Vanilla.TownNPCs.Nurse;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs
{
	public static class V2NPCStuff
	{
		public static V2NPC AsV2NPC(this NPC npc, bool risky = false)
		{
			if (!npc.TryGetGlobalNPC(out V2NPC V2NPC))
			{
				if (risky)
					return null;

				throw new Exception("this NPC can't be a pred at all, and thus, doesn't have a PredNPC global attached to them. look for your favorite gut to sleep in elsewhere");
			}
			return V2NPC;
		}
	}

	public class V2NPC : GlobalNPC
	{
		public EntityGender Gender;
		public delegate bool DelegateNewAI(NPC npc);
		public DelegateNewAI NewAIMethod { get; set; }

		public delegate List<string> DelegateGetChat(NPC npc, Player player);
		public DelegateGetChat GetNewDialogue { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;

		public V2NPC()
		{
			Gender = EntityGender.Other;

			NewAIMethod = null;

			GetNewDialogue = null;
		}

		public override void ResetEffects(NPC npc)
		{
			
		}

		public static void SetChatboxText(NPC npc, Player player, string chatText)
		{
			Main.CancelHairWindow();
			Main.SetNPCShopIndex(0);
			Main.InGuideCraftMenu = false;
			player.dropItemCheck();
			Main.npcChatCornerItem = 0;
			player.sign = -1;
			Main.editSign = false;
			player.SetTalkNPC(npc.whoAmI);
			Main.playerInventory = false;
			player.chest = -1;
			Recipe.FindRecipes();
			Main.npcChatText = chatText;
		}

		public override void GetChat(NPC npc, ref string chat)
		{
			if (npc.AsV2NPC().GetNewDialogue is not null)
			{
				List<string> chatPool = npc.AsV2NPC().GetNewDialogue.Invoke(npc, Main.CurrentPlayer);
				if (chatPool is not null)
					chat = Main.rand.NextFromCollection(chatPool);
			}
		}

		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{

		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{

		}
	}
}

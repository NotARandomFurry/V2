using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;

namespace V2.NPCs
{
	public static class GeneralNPCStuff
	{
		public static GeneralNPC AsV2NPC(this NPC npc, bool risky = false)
		{
			if (!npc.TryGetGlobalNPC(out GeneralNPC generalNPC))
			{
				if (risky)
					return null;

				throw new Exception("this NPC, somehow, has been (possibly) completely untouched by VSC. how?");
			}
			return generalNPC;
		}
	}

	public partial class GeneralNPC : GlobalNPC
	{
		public EntityGender Gender;

		public SpriteAnimation CustomSprite { get; set; } = null;

		public delegate bool DelegateNewAI(NPC npc);
		/// <summary>
		/// Used to define a new AI method for existing NPCs.<br/>
		/// </summary>
		public DelegateNewAI NewAIMethod { get; set; }
		/// <summary>
		/// The <see cref="NPCBehaviorPattern"/> that this NPC is currently using to determine its behavior.<br/>
		/// Please see the <see cref="NPCBehaviorPattern"/> documentation for more information.<br/>
		/// </summary>
		public NPCBehaviorPattern BehaviorPattern { get; internal set; } = null;
		/// <summary>
		/// Denotes whether or not this NPC is on their first frame of existing.<br/>
		/// Used almost expressly to ask whether or not <see cref="FirstFramePreAIMethod"/> should run.<br/>
		/// </summary>
		public bool FirstFrame { get; set; }

		public delegate void DelegateFirstFramePreAIMethod(NPC npc);
		/// <summary>
		/// Used to define first-frame behavior for NPCs with new AI methods.<br/>
		/// </summary>
		public DelegateFirstFramePreAIMethod FirstFramePreAIMethod { get; set; }

		public delegate List<string> DelegateGetChat(NPC npc, Player player);
		public DelegateGetChat GetNewDialogue { get; set; }

		public double HealthRegenCount { get; set; }
		public double MaxHealthBoostCount { get; set; }

		public float TargetRange { get; set; }
		public bool TargetRequiresLineOfSight { get; set; }
		public TargetType TargetType { get; set; }

		public int Aggro { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;

		public GeneralNPC()
		{
			Gender = EntityGender.Other;

			NewAIMethod = null;
			FirstFrame = true;
			FirstFramePreAIMethod = null;
			TargetRange = 0f;
			TargetRequiresLineOfSight = false;
			TargetType = TargetType.None;

			Aggro = 0;

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

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			if (npc.AsV2NPC().CustomSprite is not null)
			{
				SpriteEffects spriteEffects = npc.direction switch
				{
					-1 => SpriteEffects.FlipVertically,
					_ => SpriteEffects.None,
				};
				Texture2D texture = ModContent.Request<Texture2D>(npc.AsV2NPC().CustomSprite.Texture, AssetRequestMode.ImmediateLoad).Value;
				Rectangle sourceRect = npc.AsV2NPC().CustomSprite.DecideFrame() ?? texture.Bounds;
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

using Microsoft.Xna.Framework;
using System;
using System.Drawing.Drawing2D;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Projectiles;

namespace V2
{
	public partial class V2
	{
		internal enum MessageType : byte
		{
			Dull,
			RequestSwallowPrey,
			SyncSwallowPrey,
			RequestPlayerPredStatSync,
			DeliverPlayerPredStatSync,
			SyncDigestionCombatTextForPreyNPC,
			SyncDigestionCombatTextForPreyPlayer,
			SyncDigestionCombatTextForPreyProjectile,
			RequestRegurgitatePrey,
			SyncRegurgitatePrey,
		}

		/// <summary>
		/// since I need to focus on continuin' to exist competently as a person, I've put Rose in charge of this for now<br/>
		/// just...keep an eye on her, and feel free to feed her a good mountain range or two every once in a while as a treat<br/>
		/// she'll have earned it if she does her new job well. I'll let her tell you more<br/>
		/// -Thomas<br/>
		/// ------------------------------------------------------------------------------------------------------------<br/>
		/// hi !<br/>
		/// this is my cool job where i handle<br/>
		/// uhhhhhhhhhhhhhhhh<br/>
		/// food packets or something<br/>
		/// thomas asked me to<br/>
		/// hes very nice and lets me eat all the packets as long as i make the server do what the packets want<br/>
		/// the packets help keep my tummy happy too so im super happy to do this !<br/>
		/// thomas wrote descriptions for all the different packets<br/>
		/// ill tell you more about what makes them taste the best as we go<br/>
		/// thank !<br/>
		/// -rose<br/>
		/// </summary>
		/// <param name="reader">
		/// the thingy i use to help me split the packet into smaller bites<br/>
		/// -rose<br/>
		/// </param>
		/// <param name="whoAmI">
		/// the person that sent me the packet<br/>
		/// -rose<br/>
		/// </param>
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();
			switch (msgType)
			{
				case MessageType.Dull:
					Logger.WarnFormat(
						".     .   .\n"
					  + "your message is too boring !!!\n"
					  + "super bland and basically a small crumb pile to my hungry tummy :c\n"
					  + "i need seconds of whatever your next tasty packet is to make up for it !\n"
					  + "-rose",
						msgType
					);
					break;
				case MessageType.RequestSwallowPrey:
					HandlePacket_RequestSwallowPrey(reader, whoAmI);
					break;
				case MessageType.SyncSwallowPrey:
					HandlePacket_SyncSwallowPrey(reader, whoAmI);
					break;
				case MessageType.RequestPlayerPredStatSync:
					HandlePacket_RequestPlayerPredStatSync(reader, whoAmI);
					break;
				case MessageType.DeliverPlayerPredStatSync:
					HandlePacket_DeliverPlayerPredStatSync(reader, whoAmI);
					break;
				case MessageType.SyncDigestionCombatTextForPreyNPC:
					HandlePacket_SyncDigestionCombatTextForPreyNPC(reader, whoAmI);
					break;
				case MessageType.SyncDigestionCombatTextForPreyPlayer:
					HandlePacket_SyncDigestionCombatTextForPreyPlayer(reader, whoAmI);
					break;
				case MessageType.SyncDigestionCombatTextForPreyProjectile:
					HandlePacket_SyncDigestionCombatTextForPreyProjectile(reader, whoAmI);
					break;
				case MessageType.RequestRegurgitatePrey:
					HandlePacket_RequestRegurgitatePrey(reader, whoAmI);
					break;
				case MessageType.SyncRegurgitatePrey:
					HandlePacket_SyncRegurgitatePrey(reader, whoAmI);
					break;
				default:
					Logger.WarnFormat(
						"hi !!\n"
					  + "thomas says your net work message doesnt make sense\n"
					  + "i think it was fine tho!\n"
					  + "tasted good and made my tummy make happy sounds c:\n"
					  + "-rose"
					);
					break;
			}
		}

		public void InformOfIncorrectPacketRecipe() => Logger.WarnFormat(
			"hi !!\n"
		  + "your packet wasnt a very good snack\n"
		  + "look over your recipe and try again with a new one\n"
		  + "maybe next time youll make something yummier\n"
		  + "-rose"
		);

		/// <summary>
		/// SHORT SUMMARY:<br/>
		/// Packet type 1. <b>Can only be sent to Rose from a client.</b><br/>
		/// Upon reception, Rose will tell the server to try to put something in something else's gut.<br/>
		/// ----------------------------------------------------------<br/>
		/// ARGUMENTS:<br/>
		/// 1) Pred type as a <see cref="byte"/>.<br/>
		/// - 0 => Player<br/>
		/// - 1 => NPC<br/>
		/// - 2 => Projectile<br/>
		/// - 3 => Item (not yet implemented)<br/>
		/// - 4 => Custom (not yet implemented)<br/>
		/// 2) The index of the pred to gulp down a free meal as an <see cref="int"/>.<br/>
		/// 3) Prey type as a <see cref="byte"/>.<br/>
		/// - 0 => Player<br/>
		/// - 1 => NPC<br/>
		/// - 2 => Projectile<br/>
		/// - 3 => Item<br/>
		/// - 4 => Liquid<br/>
		/// - 5 => Custom (not yet implemented)<br/>
		/// - anything else => Undefined (throws an exception)<br/>
		/// 4) The index of the entity to get got in its respective array as an <see cref="int"/>.<br/>
		/// - for liquids, this is instead two parameters:<br/>
		/// -- The type of the liquid being drank as an <see cref="int"/>.<br/>
		/// -- The amount of the liquid being drank as a <see cref="double"/>.<br/>
		/// 5) The original <see cref="Player.whoAmI"/> of the client that requested the swallow as an <see cref="int"/>.<br/>
		/// ----------------------------------------------------------<br/>
		/// hi !<br/>
		/// my tummy likes these packets a lot<br/>
		/// they taste like hot chewy pretsels<br/>
		/// -rose<br/>
		/// </summary>
		/// <param name="reader">
		/// the thingy i use to help me split the packet into smaller bites<br/>
		/// -rose<br/>
		/// </param>
		/// <param name="whoAmI">
		/// the person that sent me the packet<br/>
		/// -rose<br/>
		/// </param>
		public void HandlePacket_RequestSwallowPrey(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.Server)
				goto Fail;

			Entity pred = reader.ReadByte() switch
			{
				0 => Main.player[reader.ReadInt32()],
				1 => Main.npc[reader.ReadInt32()],
				2 => Main.projectile[reader.ReadInt32()],
				_ => null,
			};
			if (pred is null)
				goto Fail;

			PreyType preyType = (PreyType)reader.ReadByte();
			PreyData newData = preyType switch
			{
				PreyType.Player => PreyData.NewData(Main.player[reader.ReadInt32()]),
				PreyType.NPC => PreyData.NewData(Main.npc[reader.ReadInt32()]),
				PreyType.Projectile => PreyData.NewData(Main.projectile[reader.ReadInt32()]),
				PreyType.Item => PreyData.NewData(Main.item[reader.ReadInt32()]),
				PreyType.Liquid => PreyData.NewLiquidData(reader.ReadInt32(), reader.ReadDouble()),
				PreyType.Custom => null,
				PreyType.Undefined => null,
				_ => null,
			};
			if (newData is null)
				goto Fail;

			int originalClientWhoAmI = reader.ReadInt32();
			if (pred is Player predPlayer)
			{
				if (preyType == PreyType.Liquid)
					PredPlayer.Drink(predPlayer, -1, -1, newData, 2, originalClientWhoAmI);
				PredPlayer.Swallow(predPlayer, newData.Instance, 2, originalClientWhoAmI);
			}
			else if (pred is NPC predNPC)
				PredNPC.Swallow(predNPC, newData.Instance, 2, originalClientWhoAmI);
			else if (pred is Projectile predProjectile)
				PredProjectile.Swallow(predProjectile, newData.Instance, 2, originalClientWhoAmI);

			return;
			Fail:
			InformOfIncorrectPacketRecipe();
		}

		/// <summary>
		/// SHORT SUMMARY:<br/>
		/// Packet type 2. <b>Can only be sent to Rose by the server.</b><br/>
		/// Upon reception, Rose will tell the given client(s) to try to put something in something else's gut.<br/>
		/// ----------------------------------------------------------<br/>
		/// ARGUMENTS:<br/>
		/// 1) Pred type as a <see cref="byte"/>.<br/>
		/// - 0 => Player<br/>
		/// - 1 => NPC<br/>
		/// - 2 => Projectile<br/>
		/// - 3 => Item (not yet implemented)<br/>
		/// - 4 => Custom (not yet implemented)<br/>
		/// 2) The index of the pred to gulp down a free meal as an <see cref="int"/>.<br/>
		/// 3) Prey type as a <see cref="byte"/>.<br/>
		/// - 0 => Player<br/>
		/// - 1 => NPC<br/>
		/// - 2 => Projectile<br/>
		/// - 3 => Item<br/>
		/// - 4 => Liquid<br/>
		/// - 5 => Custom (not yet implemented)<br/>
		/// - anything else => Undefined (throws an exception)<br/>
		/// 4) The index of the entity to get got in its respective array as an <see cref="int"/>.<br/>
		/// - for liquids, this is instead two parameters:<br/>
		/// -- The type of the liquid being drank as an <see cref="int"/>.<br/>
		/// -- The amount of the liquid being drank as a <see cref="double"/>.<br/>
		/// 5) The original <see cref="Player.whoAmI"/> of the client that requested the swallow as an <see cref="int"/>.<br/>
		/// ----------------------------------------------------------<br/>
		/// hi !<br/>
		/// my tummy likes these packets a whole lot<br/>
		/// they taste like warm pretzals with extra salt<br/>
		/// -rose<br/>
		/// </summary>
		/// <param name="reader">
		/// the thingy i use to help me split the packet into smaller bites<br/>
		/// -rose<br/>
		/// </param>
		/// <param name="whoAmI">
		/// the person that sent me the packet<br/>
		/// -rose<br/>
		/// </param>
		public void HandlePacket_SyncSwallowPrey(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				goto Fail;

			Entity pred = reader.ReadByte() switch
			{
				0 => Main.player[reader.ReadInt32()],
				1 => Main.npc[reader.ReadInt32()],
				2 => Main.projectile[reader.ReadInt32()],
				_ => null,
			};
			if (pred is null)
				goto Fail;

			PreyType preyType = (PreyType)reader.ReadByte();
			PreyData newData = preyType switch
			{
				PreyType.Player => PreyData.NewData(Main.player[reader.ReadInt32()]),
				PreyType.NPC => PreyData.NewData(Main.npc[reader.ReadInt32()]),
				PreyType.Projectile => PreyData.NewData(Main.projectile[reader.ReadInt32()]),
				PreyType.Item => PreyData.NewData(Main.item[reader.ReadInt32()]),
				PreyType.Liquid => PreyData.NewLiquidData(reader.ReadInt32(), reader.ReadDouble()),
				PreyType.Custom => null,
				PreyType.Undefined => null,
				_ => null,
			};
			if (newData is null)
				goto Fail;

			int originalClientWhoAmI = reader.ReadInt32();
			if (pred is Player predPlayer)
			{
				if (preyType == PreyType.Liquid)
					PredPlayer.Drink(predPlayer, -1, -1, newData, 3, originalClientWhoAmI);
				PredPlayer.Swallow(predPlayer, newData.Instance, 3, originalClientWhoAmI);
			}
			else if (pred is NPC predNPC)
				PredNPC.Swallow(predNPC, newData.Instance, 3, originalClientWhoAmI);
			else if (pred is Projectile predProjectile)
				PredProjectile.Swallow(predProjectile, newData.Instance, 3, originalClientWhoAmI);

			return;
			Fail:
			InformOfIncorrectPacketRecipe();
		}

		/// <summary>
		/// SHORT SUMMARY:<br/>
		/// Packet type 3. <b>Can only be sent to Rose by a client.</b><br/>
		/// Upon reception, Rose will tell the server to try to sync a player's pred stat point changes.<br/>
		/// ----------------------------------------------------------<br/>
		/// ARGUMENTS:<br/>
		/// 1) The <see cref="Player.whoAmI"/> of the player pred messing with their stat points, as a <see cref="byte"/>.<br/>
		/// - Since this is only ever called from the player's pred stats menu, this can also be used to determine whose client the original sender of the packet was.<br/>
		/// 2-5) The new amount of points invested in each pred stat, as a set of 4 <see cref="int"/>s.<br/>
		/// ----------------------------------------------------------<br/>
		/// hi !<br/>
		/// my tummy likes these packets<br/>
		/// they taste like lamia scales with a side of salty meat<br/>
		/// -rose<br/>
		/// </summary>
		/// <param name="reader">
		/// the thingy i use to help me split the packet into smaller bites<br/>
		/// -rose<br/>
		/// </param>
		/// <param name="whoAmI">
		/// the person that sent me the packet<br/>
		/// -rose<br/>
		/// </param>
		public void HandlePacket_RequestPlayerPredStatSync(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.Server)
				goto Fail;

			int originalPlayerWhoAmI = reader.ReadByte();
			if (originalPlayerWhoAmI < 0 || originalPlayerWhoAmI > Main.maxPlayers)
				goto Fail;

			Player player = Main.player[originalPlayerWhoAmI];
			player.AsPred().GLP.Spent = reader.ReadInt32();
			player.AsPred().TUM.Spent = reader.ReadInt32();
			player.AsPred().ACI.Spent = reader.ReadInt32();
			player.AsPred().ABS.Spent = reader.ReadInt32();

			ModPacket deliveryPacket = GetPacket();
			deliveryPacket.Write((byte)MessageType.DeliverPlayerPredStatSync);
			deliveryPacket.Write((byte)originalPlayerWhoAmI);
			deliveryPacket.Write(player.AsPred().GLP.Spent);
			deliveryPacket.Write(player.AsPred().TUM.Spent);
			deliveryPacket.Write(player.AsPred().ACI.Spent);
			deliveryPacket.Write(player.AsPred().ABS.Spent);
			deliveryPacket.Send(ignoreClient: originalPlayerWhoAmI);

			return;
			Fail:
			InformOfIncorrectPacketRecipe();
		}

		/// <summary>
		/// SHORT SUMMARY:<br/>
		/// Packet type 4. <b>Can only be sent to Rose by the server.</b><br/>
		/// Upon reception, Rose will tell the given client(s) to try to sync a player's pred stat point changes.<br/>
		/// ----------------------------------------------------------<br/>
		/// ARGUMENTS:<br/>
		/// 1) The <see cref="Player.whoAmI"/> of the player pred messing with their stat points, as a <see cref="byte"/>.<br/>
		/// - Since this is only ever called from the player's pred stats menu, this can also be used to determine whose client the original sender of the packet was.<br/>
		/// 2-5) The new amount of points invested in each pred stat, as a set of 4 <see cref="int"/>s.<br/>
		/// ----------------------------------------------------------<br/>
		/// hi !<br/>
		/// my tummy likes these packets<br/>
		/// they taste like lamia scales with a side of sour candy<br/>
		/// -rose<br/>
		/// </summary>
		/// <param name="reader">
		/// the thingy i use to help me split the packet into smaller bites<br/>
		/// -rose<br/>
		/// </param>
		/// <param name="whoAmI">
		/// the person that sent me the packet<br/>
		/// -rose<br/>
		/// </param>
		public void HandlePacket_DeliverPlayerPredStatSync(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				goto Fail;

			int originalPlayerWhoAmI = reader.ReadByte();
			if (originalPlayerWhoAmI < 0 || originalPlayerWhoAmI > Main.maxPlayers)
				goto Fail;

			Player player = Main.player[originalPlayerWhoAmI];
			player.AsPred().GLP.Spent = reader.ReadInt32();
			player.AsPred().TUM.Spent = reader.ReadInt32();
			player.AsPred().ACI.Spent = reader.ReadInt32();
			player.AsPred().ABS.Spent = reader.ReadInt32();

			return;
			Fail:
			InformOfIncorrectPacketRecipe();
		}

		public void HandlePacket_SyncDigestionCombatTextForPreyNPC(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				goto Fail;

			int npcWhoAmI = reader.ReadInt32();
			if (npcWhoAmI < 0 || npcWhoAmI > Main.maxNPCs)
				goto Fail;

			if (!ModContent.GetInstance<V2ClientConfig>().ShowChurnDamageNumbers)
				return;

			NPC npc = Main.npc[npcWhoAmI];
			CombatText digestionDamageText = Main.combatText[CombatText.NewText(
				npc.Hitbox,
				npc.friendly ? Color.DarkGreen : Color.LimeGreen,
				reader.ReadInt32(),
				false,
				true
			)];
			digestionDamageText.position.X = reader.ReadSingle();
			digestionDamageText.position.Y = reader.ReadSingle();
			digestionDamageText.velocity.X = reader.ReadSingle();
			digestionDamageText.velocity.Y = reader.ReadSingle();

			return;
			Fail:
			InformOfIncorrectPacketRecipe();
		}

		public void HandlePacket_SyncDigestionCombatTextForPreyPlayer(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				goto Fail;

			int playerWhoAmI = reader.ReadInt32();
			if (playerWhoAmI < 0 || playerWhoAmI > Main.maxPlayers)
				goto Fail;

			if (!ModContent.GetInstance<V2ClientConfig>().ShowChurnDamageNumbers)
				return;

			Player player = Main.player[playerWhoAmI];
			CombatText digestionDamageText = Main.combatText[CombatText.NewText(
				player.Hitbox,
				Color.DarkGreen,
				reader.ReadInt32(),
				false,
				true
			)];
			digestionDamageText.position.X = reader.ReadSingle();
			digestionDamageText.position.Y = reader.ReadSingle();
			digestionDamageText.velocity.X = reader.ReadSingle();
			digestionDamageText.velocity.Y = reader.ReadSingle();

			return;
			Fail:
			InformOfIncorrectPacketRecipe();
		}

		public void HandlePacket_SyncDigestionCombatTextForPreyProjectile(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				goto Fail;

			int projectileWhoAmI = reader.ReadInt32();
			if (projectileWhoAmI < 0 || projectileWhoAmI > Main.maxProjectiles)
				goto Fail;

			if (!ModContent.GetInstance<V2ClientConfig>().ShowChurnDamageNumbers)
				return;

			Projectile projectile = Main.projectile[projectileWhoAmI];
			CombatText digestionDamageText = Main.combatText[CombatText.NewText(
				projectile.Hitbox,
				Color.DarkGreen,
				reader.ReadInt32(),
				false,
				true
			)];
			digestionDamageText.position.X = reader.ReadSingle();
			digestionDamageText.position.Y = reader.ReadSingle();
			digestionDamageText.velocity.X = reader.ReadSingle();
			digestionDamageText.velocity.Y = reader.ReadSingle();

			return;
			Fail:
			InformOfIncorrectPacketRecipe();
		}

		public void HandlePacket_RequestRegurgitatePrey(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.Server)
				goto Fail;

			Entity pred = reader.ReadByte() switch
			{
				0 => Main.player[reader.ReadInt32()],
				1 => Main.npc[reader.ReadInt32()],
				2 => Main.projectile[reader.ReadInt32()],
				_ => null,
			};
			if (pred is null)
				goto Fail;

			int preyIndex = reader.ReadInt32();

			int originalClientWhoAmI = reader.ReadInt32();
			if (pred is Player predPlayer)
				PredPlayer.Regurgitate(predPlayer, preyIndex, 2, originalClientWhoAmI);
			else if (pred is NPC predNPC)
				PredNPC.Regurgitate(predNPC, preyIndex, 2, originalClientWhoAmI);
			else if (pred is Projectile predProjectile)
				PredProjectile.Regurgitate(predProjectile, preyIndex, 2, originalClientWhoAmI);

			return;
			Fail:
			InformOfIncorrectPacketRecipe();
		}

		public void HandlePacket_SyncRegurgitatePrey(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				goto Fail;

			Entity pred = reader.ReadByte() switch
			{
				0 => Main.player[reader.ReadInt32()],
				1 => Main.npc[reader.ReadInt32()],
				2 => Main.projectile[reader.ReadInt32()],
				_ => null,
			};
			if (pred is null)
				goto Fail;

			int preyIndex = reader.ReadInt32();

			int originalClientWhoAmI = reader.ReadInt32();
			if (pred is Player predPlayer)
				PredPlayer.Regurgitate(predPlayer, preyIndex, 3, originalClientWhoAmI);
			else if (pred is NPC predNPC)
				PredNPC.Regurgitate(predNPC, preyIndex, 3, originalClientWhoAmI);
			else if (pred is Projectile predProjectile)
				PredProjectile.Regurgitate(predProjectile, preyIndex, 3, originalClientWhoAmI);

			return;
			Fail:
			InformOfIncorrectPacketRecipe();
		}
	}
}

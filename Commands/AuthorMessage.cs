using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using V2.Items.Voraria.CheatItems;
using V2.PlayerHandling;

namespace V2.Commands
{
	public class AuthorMessage : ModCommand
	{
		public override string Command => "authormessage";

		public override CommandType Type => CommandType.Chat;

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (caller.Player.name != "Rose" || !caller.Player.HasItem(ModContent.ItemType<ServerMessageRelay>()))
				return;

			string realInput = input.Remove(0, "authormessage".Length + 2);
			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText("[SERVER] " + realInput, V2Colors.CarmineThread);
			else
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("[SERVER] " + realInput), V2Colors.CarmineThread);
		}
	}
}

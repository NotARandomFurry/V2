using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using V2.PlayerHandling;

namespace V2
{
	public partial class V2
	{
		internal enum MessageType : byte
		{
			PredPlayerSync,
			PreyPlayerSync
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();
			switch (msgType)
			{
				case MessageType.PredPlayerSync:
					byte predPlayerIndex = reader.ReadByte();
					PredPlayer predPlayer = Main.player[predPlayerIndex].GetModPlayer<PredPlayer>();
					predPlayer.ReceivePlayerSync(reader);
					if (Main.netMode == NetmodeID.Server)
						predPlayer.SyncPlayer(-1, whoAmI, false);
					break;
				case MessageType.PreyPlayerSync:
					byte preyPlayerIndex = reader.ReadByte();
					PreyPlayer preyPlayer = Main.player[preyPlayerIndex].GetModPlayer<PreyPlayer>();
					preyPlayer.ReceivePlayerSync(reader);
					if (Main.netMode == NetmodeID.Server)
						preyPlayer.SyncPlayer(-1, whoAmI, false);
					break;
				default:
					Logger.WarnFormat(
						"Well, my word. You've gone and delivered us a message ({0}) that doesn't make sense. Be a dear and fix it up the next time you send one, alright? -Queen Cadenza Appetitia IV (Cadence, for short)",
						msgType
					);
					break;
			}
		}
	}
}

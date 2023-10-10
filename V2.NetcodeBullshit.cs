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
			SyncPlayerPredData,
			PreyPlayerSync
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();
			switch (msgType)
			{
				case MessageType.SyncPlayerPredData:
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
						"hi !!\n"
					  + "thomas says your message doesnt make sense\n"
					  + "i think it was fine tho!\n"
					  + "tasted good and made my tummy make happy sounds c:\n"
					  + "-rose",
						msgType
					);
					break;
			}
		}
	}
}

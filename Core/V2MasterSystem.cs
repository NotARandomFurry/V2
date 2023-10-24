using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core.StruggleSystem;

namespace V2.Core
{
	public class V2MasterSystem : ModSystem
	{
		public bool freedSucc;
		public bool freedAngel;

		public List<VoreTracker> VoreTrackers { get; set; } = new List<VoreTracker>();

		public override void OnWorldLoad()
		{
			VoreTrackers = new List<VoreTracker>();
			freedSucc = false;
			freedAngel = false;
		}

		public override void OnWorldUnload()
		{
			VoreTrackers = new List<VoreTracker>();
			freedSucc = false;
			freedAngel = false;
		}

		public override void PreUpdateEntities()
		{
			foreach (VoreTracker tracker in VoreTrackers)
			{
				tracker.UpdatePrey();
				if (Main.netMode == NetmodeID.SinglePlayer)
					tracker.HandleStruggleSystem();
			}

			VoreTrackers.RemoveAll(x => x.CheckClearability());
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["freedSucc"] = freedSucc;
			tag["freedAngel"] = freedAngel;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			freedSucc = tag.ContainsKey("freedSucc") && tag.GetBool("freedSucc");
			freedAngel = tag.ContainsKey("freedAngel") && tag.GetBool("freedAngel");
		}

		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte(
				freedSucc,
				freedAngel,
				false,
				false,
				false,
				false,
				false,
				false
			);
			writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			freedSucc = flags[0];
			freedAngel = flags[1];
		}

	}
}

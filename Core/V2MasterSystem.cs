using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace V2.Core
{
	public class V2MasterSystem : ModSystem
	{
		public bool freedSucc;
		public bool freedAngel;
        public bool freedEnigma;

        public List<VoreTracker> VoreTrackers { get; set; } = new List<VoreTracker>();

		public override void OnWorldLoad()
		{
			VoreTrackers = [];
			freedSucc = false;
			freedAngel = false;
			freedEnigma = false;
        }

		public override void OnWorldUnload()
		{
			VoreTrackers = [];
			freedSucc = false;
			freedAngel = false;
            freedEnigma = false;

        }

		public override void PreUpdateEntities()
		{
			foreach (VoreTracker tracker in VoreTrackers)
			{
				tracker.UpdatePrey();
				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					tracker.UpdateProgress();
					tracker.HandleStruggleSystem();
				}
			}

			VoreTrackers.RemoveAll(x => x.CheckClearability());
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["freedSucc"] = freedSucc;
			tag["freedAngel"] = freedAngel;
            tag["freedEnigma"] = freedEnigma;
        }

		public override void LoadWorldData(TagCompound tag)
		{
			freedSucc = tag.ContainsKey("freedSucc") && tag.GetBool("freedSucc");
			freedAngel = tag.ContainsKey("freedAngel") && tag.GetBool("freedAngel");
            freedEnigma = tag.ContainsKey("freedEnigma") && tag.GetBool("freedEnigma");
        }

		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte(
				freedSucc,
				freedAngel,
                freedEnigma,
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
            freedEnigma = flags[2];
        }

	}
}

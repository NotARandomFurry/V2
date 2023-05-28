using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;

namespace V2.Sounds.MuffledSounds
{
	public static class MuffledNPCSounds
	{
		public static readonly SoundStyle NPCHit1 = new SoundStyle("V2/Sounds/MuffledSounds/NPC_Hit_1", SoundType.Sound) with { MaxInstances = 0 };
		public static readonly SoundStyle NPCHit2 = new SoundStyle("V2/Sounds/MuffledSounds/NPC_Hit_2", SoundType.Sound) with { MaxInstances = 0 };
		public static readonly SoundStyle NPCHit3 = new SoundStyle("V2/Sounds/MuffledSounds/NPC_Hit_3", SoundType.Sound) with { MaxInstances = 0 };
		public static readonly SoundStyle NPCHit4 = new SoundStyle("V2/Sounds/MuffledSounds/NPC_Hit_4", SoundType.Sound) with { MaxInstances = 0 };

		public static readonly SoundStyle NPCDeath1 = new SoundStyle("V2/Sounds/MuffledSounds/NPC_Killed_1", SoundType.Sound) with { MaxInstances = 0 };
		public static readonly SoundStyle NPCDeath2 = new SoundStyle("V2/Sounds/MuffledSounds/NPC_Killed_2", SoundType.Sound) with { MaxInstances = 0 };
	}
}

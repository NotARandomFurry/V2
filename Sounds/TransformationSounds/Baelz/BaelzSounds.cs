using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;

namespace V2.Sounds.TransformationSounds.Baelz
{
	public static class BaelzSounds
	{
		public static readonly SoundStyle BaelzHurt = new SoundStyle("V2/Sounds/TransformationSounds/Baelz/Baelz_Hurt", 1, 4, SoundType.Sound) with { MaxInstances = 1 };
		public static readonly SoundStyle BaelzDeath = new SoundStyle("V2/Sounds/TransformationSounds/Baelz/Baelz_Death", 1, 2, SoundType.Sound) with { MaxInstances = 0 };
	}
}

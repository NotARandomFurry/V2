using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;

namespace V2.Sounds.Vore
{
	public static class Gulps
	{
		public static readonly SoundStyle AprilFools = new SoundStyle("V2/Sounds/Vore/Gulps/AprilFools", SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0f };
		public static readonly SoundStyle Short = new SoundStyle("V2/Sounds/Vore/Gulps/Short_", 1, 4, SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0.04f };
		public static readonly SoundStyle Standard = new SoundStyle("V2/Sounds/Vore/Gulps/Standard_", 1, 10, SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0.04f };
	}
}

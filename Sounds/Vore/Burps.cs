using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;

namespace V2.Sounds.Vore
{
	public static class Burps
	{
		public static readonly SoundStyle AprilFools = new SoundStyle("V2/Sounds/Vore/Burps/AprilFools", SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0f };

		public static class Humanoid
		{
			public static readonly SoundStyle Small = new SoundStyle("V2/Sounds/Vore/Burps/Humanoid/Small_", 1, 9, SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0.04f };
			public static readonly SoundStyle Standard = new SoundStyle("V2/Sounds/Vore/Burps/Humanoid/Standard_", 1, 14, SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0.04f };

			public static class Zombie
			{
				public static readonly SoundStyle Standard = new SoundStyle("V2/Sounds/Vore/Burps/Humanoid/Zombie/Standard_1", SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0.04f };
			}
		}
	}
}

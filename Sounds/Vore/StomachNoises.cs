using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;

namespace V2.Sounds.Vore
{
	public static class StomachNoises
	{
		public static readonly SoundStyle AprilFools = new SoundStyle("V2/Sounds/Vore/StomachNoises/AprilFools", SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0f };
		public static readonly SoundStyle Normal = new SoundStyle("V2/Sounds/Vore/StomachNoises/Normal_", 1, 3) with { MaxInstances = 0 };
		public static readonly SoundStyle Muffled = new SoundStyle("V2/Sounds/Vore/StomachNoises/Muffled_", 1, 3) with { MaxInstances = 0 };
	}
}

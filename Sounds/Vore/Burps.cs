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
		public static class Humanoid
		{
			public static readonly SoundStyle Small = new SoundStyle("V2/Sounds/Vore/Burps/Humanoid/Small_", 1, 9, SoundType.Sound);
			public static readonly SoundStyle Standard = new SoundStyle("V2/Sounds/Vore/Burps/Humanoid/Standard_", 1, 14, SoundType.Sound);
		}
	}
}

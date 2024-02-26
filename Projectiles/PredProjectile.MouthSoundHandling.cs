using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using V2.Core;
using V2.Sounds.Vore;

namespace V2.Projectiles
{
	public partial class PredProjectile : GlobalProjectile
	{
		public static void PlaySwallowGulp(Projectile pred, PreyData prey)
		{
			SoundStyle? gulpySound = pred.AsPred().BigGulps;
			if (prey.WeightLeftToDigest < pred.AsPred().SmallGulpThreshold)
				gulpySound = pred.AsPred().SmallGulps;

			SoundEngine.PlaySound(
				gulpySound,
				pred.TrueCenter() + MouthSoundOffset(pred)
			);
		}

		public static void PlayDigestionBelch(Projectile pred, PreyData prey)
		{
			SoundStyle? bworpySound = pred.AsPred().StandardBurps;
			if (prey is not null)
			{
				if (prey.WeightLeftToDigest < pred.AsPred().SmallBurpThreshold && pred.AsPred().SmallBurps is not null)
					bworpySound = pred.AsPred().SmallBurps;
				if (prey.WeightLeftToDigest < pred.AsPred().BigBurpThreshold && pred.AsPred().BigBurps is not null)
					bworpySound = pred.AsPred().BigBurps;
			}

			SoundEngine.PlaySound(
				bworpySound.Value with { Pitch = pred.AsPred().BurpPitchOffset },
				pred.TrueCenter() + MouthSoundOffset(pred)
			);
		}
	}
}

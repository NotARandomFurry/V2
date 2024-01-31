using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.Items;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.Sounds.MuffledSounds;

namespace V2.NPCs
{
	public partial class PredNPC : GlobalNPC
	{
		public static void PlaySwallowGulp(NPC pred, PreyData prey)
		{
			SoundStyle? gulpySound = pred.AsPred().BigGulps;
			if (prey.WeightLeftToDigest < pred.AsPred().SmallGulpThreshold)
				gulpySound = pred.AsPred().SmallGulps;

			SoundEngine.PlaySound(
				gulpySound,
				pred.TrueCenter() + MouthSoundOffset(pred)
			);
		}

		public static void PlayDigestionBelch(NPC pred, PreyData prey)
		{
			SoundStyle? bworpySound = pred.AsPred().StandardBurps;
			if (prey.WeightLeftToDigest < pred.AsPred().SmallBurpThreshold && pred.AsPred().SmallBurps is not null)
				bworpySound = pred.AsPred().SmallBurps;
			if (prey.WeightLeftToDigest < pred.AsPred().BigBurpThreshold && pred.AsPred().BigBurps is not null)
				bworpySound = pred.AsPred().BigBurps;

			SoundEngine.PlaySound(
				bworpySound,
				pred.TrueCenter() + MouthSoundOffset(pred)
			);
		}
	}
}

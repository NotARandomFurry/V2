using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.IO;
using V2.Core;

namespace V2.NPCs.Vanilla.TownNPCs.Dryad
{
	public static partial class DryadStuff
	{
		public static class Animations
		{
			public static class AVEmpressOfLight
			{
				public static string AnimSetSheet
				{
					get {
						foreach (ResourcePack pack in V2.EnabledResourcePacks)
						{
							switch (pack.Name)
							{
								case "True Dryad Fan":
									return "V2/NPCs/Vanilla/TownNPCs/Dryad/AltSheetSets/True Dryad Fan/Dryad_WeightBase_BossBelly_EmpressOfLight";
							}
						}

						return "V2/NPCs/Vanilla/TownNPCs/Dryad/Dryad_WeightBase_BossBelly_EmpressOfLight";
					}
				}
				public class PhaseOne : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 0, 200 ),
						( 1, 15 ),
						( 2, 25 ),
						( 3, 25 ),
						( 4, 25 ),
						( 5, 25 ),
						( 4, 25 ),
						( 5, 25 ),
						( 4, 35 ),
						( 5, 45 ),
						( 4, 55 ),
						( 5, 75 ),
						( 3, 15 ),
						( 6, 15 ),
						( 1, 25 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						86 * (FrameID % 10),
						148 * (int)Math.Floor((double)FrameID / 10.0),
						86,
						148
					);
				}

				public class PhaseTransition : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 0, 200 ),
						( 1, 15 ),
						( 2, 25 ),
						( 3, 25 ),
						( 4, 25 ),
						( 5, 25 ),
						( 4, 35 ),
						( 5, 45 ),
						( 4, 55 ),
						( 5, 75 ),
						( 10, 85 ),
						( 11, 15 ),
						( 10, 85 ),
						( 12, 25 ),
						( 13, 15 ),
						( 14, 15 ),
						( 15, 15 ),
						( 16, 15 ),
						( 17, 15 ),
						( 18, 25 ),
						( 19, 35 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						86 * (FrameID % 10),
						148 * (int)Math.Floor((double)FrameID / 10.0),
						86,
						148
					);
				}

				public class PhaseTwo : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 20, 190 ),
						( 21, 10 ),
						( 20, 245 ),
						( 21, 10 ),
						( 20, 105 ),
						( 21, 10 ),
						( 20, 45 ),
						( 21, 10 ),
						( 20, 190 ),
						( 21, 10 ),
						( 20, 275 ),
						( 21, 10 ),
						( 20, 135 ),
						( 21, 10 ),
						( 20, 225 ),
						( 21, 10 ),
						( 20, 65 ),
						( 21, 10 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						86 * (FrameID % 10),
						148 * (int)Math.Floor((double)FrameID / 10.0),
						86,
						148
					);
				}

				public class EmpressGetsChurned : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 20, 190 ),
						( 21, 10 ),
						( 22, 25 ),
						( 23, 20 ),
						( 24, 20 ),
						( 25, 25 ),
						( 26, 35 ),
						( 27, 25 ),
						( 28, 25 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						86 * (FrameID % 10),
						148 * (int)Math.Floor((double)FrameID / 10.0),
						86,
						148
					);
				}

				public class DigestStage1 : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 30, 190 ),
						( 31, 10 ),
						( 30, 245 ),
						( 31, 10 ),
						( 30, 105 ),
						( 31, 10 ),
						( 30, 45 ),
						( 31, 10 ),
						( 30, 190 ),
						( 31, 10 ),
						( 30, 275 ),
						( 31, 10 ),
						( 30, 135 ),
						( 31, 10 ),
						( 30, 225 ),
						( 31, 10 ),
						( 30, 65 ),
						( 31, 10 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						86 * (FrameID % 10),
						148 * (int)Math.Floor((double)FrameID / 10.0),
						86,
						148
					);
				}

				public class DigestStage2 : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 32, 190 ),
						( 33, 10 ),
						( 32, 245 ),
						( 33, 10 ),
						( 32, 105 ),
						( 33, 10 ),
						( 32, 45 ),
						( 33, 10 ),
						( 32, 190 ),
						( 33, 10 ),
						( 32, 275 ),
						( 33, 10 ),
						( 32, 135 ),
						( 33, 10 ),
						( 32, 225 ),
						( 33, 10 ),
						( 32, 65 ),
						( 33, 10 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						86 * (FrameID % 10),
						148 * (int)Math.Floor((double)FrameID / 10.0),
						86,
						148
					);
				}

				public class DigestStage3 : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 34, 190 ),
						( 35, 10 ),
						( 34, 245 ),
						( 35, 10 ),
						( 34, 105 ),
						( 35, 10 ),
						( 34, 45 ),
						( 35, 10 ),
						( 34, 190 ),
						( 35, 10 ),
						( 34, 275 ),
						( 35, 10 ),
						( 34, 135 ),
						( 35, 10 ),
						( 34, 225 ),
						( 35, 10 ),
						( 34, 65 ),
						( 35, 10 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						86 * (FrameID % 10),
						148 * (int)Math.Floor((double)FrameID / 10.0),
						86,
						148
					);
				}

				public class DigestStage4 : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 36, 190 ),
						( 37, 10 ),
						( 36, 245 ),
						( 37, 10 ),
						( 36, 105 ),
						( 37, 10 ),
						( 36, 45 ),
						( 37, 10 ),
						( 36, 190 ),
						( 37, 10 ),
						( 36, 275 ),
						( 37, 10 ),
						( 36, 135 ),
						( 37, 10 ),
						( 36, 225 ),
						( 37, 10 ),
						( 36, 65 ),
						( 37, 10 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						86 * (FrameID % 10),
						148 * (int)Math.Floor((double)FrameID / 10.0),
						86,
						148
					);
				}

				public class DigestStage5 : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 38, 190 ),
						( 39, 10 ),
						( 38, 245 ),
						( 39, 10 ),
						( 38, 105 ),
						( 39, 10 ),
						( 38, 45 ),
						( 39, 10 ),
						( 38, 190 ),
						( 39, 10 ),
						( 38, 275 ),
						( 39, 10 ),
						( 38, 135 ),
						( 39, 10 ),
						( 38, 225 ),
						( 39, 10 ),
						( 38, 65 ),
						( 39, 10 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						86 * (FrameID % 10),
						148 * (int)Math.Floor((double)FrameID / 10.0),
						86,
						148
					);
				}
			}
		}
	}
}

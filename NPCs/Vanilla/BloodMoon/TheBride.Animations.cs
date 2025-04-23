using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.IO;
using V2.Core;

namespace V2.NPCs.Vanilla.BloodMoon
{
	public static partial class TheBrideStuff
	{
		public static class Animations
		{
			public static class OVEmpressOfLight
			{
				public static string AnimSetSheet => "V2/NPCs/Vanilla/BloodMoon/TheBride_WeightBase_BossBelly_EmpressOfLight";

				public class Intact : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 0, 18 ),
						( 1, 18 ),
						( 2, 18 ),
						( 3, 18 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						0,
						54 * FrameID,
						114,
						54
					);
				}

				public class DigestStage1 : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 0, 18 ),
						( 1, 18 ),
						( 2, 18 ),
						( 3, 18 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						114,
						54 * FrameID,
						114,
						54
					);
				}

				public class DigestStage2 : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 0, 18 ),
						( 1, 18 ),
						( 2, 18 ),
						( 3, 18 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						228,
						54 * FrameID,
						114,
						54
					);
				}

				public class DigestStage3 : SpriteAnimation
				{
					public override string Texture => AnimSetSheet;

					public override List<(int frame, int rawDelay)> Frames => [
						( 0, 18 ),
						( 1, 18 ),
						( 2, 18 ),
						( 3, 18 ),
					];

					public override Rectangle? DecideFrame() => new Rectangle(
						432,
						54 * FrameID,
						114,
						54
					);
				}
			}
		}
	}
}

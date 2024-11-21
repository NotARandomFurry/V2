using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.Projectiles.Vanilla.Summons.Pets
{
	public static partial class FairyPrincessStuff
	{
		public static class Animations
		{
			public static class BaseWeight
			{
				public static class OVHerOwnFuckingMother
				{
					public class Alive : SpriteAnimation
					{
						public override string Texture => "V2/Projectiles/Vanilla/Summons/Pets/FairyPrincess_WeightBase_BossBelly_EmpressOfLight";

						public override List<(int frame, int rawDelay)> Frames => [
							( 0, 15 ),
							( 1, 15 ),
							( 2, 15 ),
							( 3, 15 ),
							( 4, 15 ),
							( 5, 15 ),
						];

						public override Rectangle? DecideFrame() => new Rectangle(
							0,
							118 * FrameID,
							80,
							118
						);
					}
					public class DigestStage1 : SpriteAnimation
					{
						public override string Texture => "V2/Projectiles/Vanilla/Summons/Pets/FairyPrincess_WeightBase_BossBelly_EmpressOfLight";

						public override List<(int frame, int rawDelay)> Frames => [
							( 0, 15 ),
							( 1, 15 ),
							( 2, 15 ),
							( 3, 15 ),
							( 4, 15 ),
							( 5, 15 ),
						];

						public override Rectangle? DecideFrame() => new Rectangle(
							80,
							118 * FrameID,
							80,
							118
						);
					}
					public class DigestStage2 : SpriteAnimation
					{
						public override string Texture => "V2/Projectiles/Vanilla/Summons/Pets/FairyPrincess_WeightBase_BossBelly_EmpressOfLight";

						public override List<(int frame, int rawDelay)> Frames => [
							( 0, 15 ),
							( 1, 15 ),
							( 2, 15 ),
							( 3, 15 ),
							( 4, 15 ),
							( 5, 15 ),
						];

						public override Rectangle? DecideFrame() => new Rectangle(
							160,
							118 * FrameID,
							80,
							118
						);
					}
					public class DigestStage3 : SpriteAnimation
					{
						public override string Texture => "V2/Projectiles/Vanilla/Summons/Pets/FairyPrincess_WeightBase_BossBelly_EmpressOfLight";

						public override List<(int frame, int rawDelay)> Frames => [
							( 0, 15 ),
							( 1, 15 ),
							( 2, 15 ),
							( 3, 15 ),
							( 4, 15 ),
							( 5, 15 ),
						];

						public override Rectangle? DecideFrame() => new Rectangle(
							240,
							118 * FrameID,
							80,
							118
						);
					}
				}
			}
		}
	}
}

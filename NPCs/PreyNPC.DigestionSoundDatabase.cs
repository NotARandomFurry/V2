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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.Sounds.MuffledSounds;

namespace V2.NPCs
{
	public partial class PreyNPC : GlobalNPC
	{
		public static Dictionary<SoundStyle, SoundStyle> DigestingHitSoundDatabase { get; set; } = new Dictionary<SoundStyle, SoundStyle>
		{
			{ SoundID.NPCHit1, MuffledNPCSounds.NPCHit1 },
			{ SoundID.NPCHit2, MuffledNPCSounds.NPCHit2 },
			{ SoundID.NPCHit3, MuffledNPCSounds.NPCHit3 },
			{ SoundID.NPCHit4, MuffledNPCSounds.NPCHit4 },
		};
		public static Dictionary<SoundStyle, SoundStyle> DigestedDeathSoundDatabase { get; set; } = new Dictionary<SoundStyle, SoundStyle>
		{
			{ SoundID.NPCDeath1, MuffledNPCSounds.NPCDeath1 },
			{ SoundID.NPCDeath2, MuffledNPCSounds.NPCDeath2 },
		};
	}
}

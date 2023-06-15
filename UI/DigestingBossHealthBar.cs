using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using V2.NPCs;

namespace V2.UI
{
	public class DigestingBossHealthBar : GlobalBossBar
	{
		public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
		{
			if (npc.AsFood().IsCurrentlyEaten)
				drawParams.BarTexture = ModContent.Request<Texture2D>("V2/UI/DigestingBossHealthBar", AssetRequestMode.ImmediateLoad).Value;

			return true;
		}
	}
}

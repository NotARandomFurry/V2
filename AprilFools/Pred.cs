using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;

namespace V2.AprilFools
{
	public class AprilFoolsPredNPC : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => V2.GetFooled;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;

		public override bool InstancePerEntity => true;

		public override void SetDefaults(NPC npc)
		{
			npc.AsPred().CanSwallowBosses = true;

			npc.AsPred().BaseStomachacheMeterCapacity = 9999999;
			npc.AsPred().MaxStomachCapacity = 9999999;

			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerDeathMessage;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;
		}

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => npc.damage * 0.5;
		public static double GetDigestionTickRate(NPC npc, PreyData prey) => (double)npc.lifeMax * 0.1 * ((double)npc.life / (double)npc.lifeMax) * npc.scale;
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 1,
				seconds: 0
			);
			return baseAbsorptionRate * npc.scale;
		}

		public override void PostAI(NPC npc)
		{
			npc.scale = 1.0f + ((float)npc.AsPred().ExtraWeight * 0.2f);
			if (npc.CurrentCaptor() is null)
				npc.DoContactGulpage();
		}

		public static void GetDigestedPlayerDeathMessage(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.Clear();
			deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.AprilFools");
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			SpriteEffects spriteEffects = npc.direction switch
			{
				-1 => SpriteEffects.FlipHorizontally,
				_ => SpriteEffects.None,
			};
			string exactTextureToUse = "V2/AprilFools/Belly";
			double bellySize = PredNPC.GetCurrentBellyWeight(npc);
			bellySize /= PreyData.NewData(npc).InitialSize;

			Texture2D texture = ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad).Value;
			spriteBatch.Draw
			(
				texture,
				npc.Center - screenPos + new Vector2(0f, npc.gfxOffY) + (new Vector2((npc.direction == 1 ? 6f : -26f), 2f) * (float)bellySize),
				texture.Bounds,
				drawColor,
				npc.rotation,
				new Vector2(32f, 32f),
				(float)bellySize * 0.33f,
				spriteEffects,
				0f
			);
		}
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using V2.Core;
using V2.NPCs;
using V2.NPCs.Vanilla.TownNPCs.Stylist;
using V2.PlayerHandling;

namespace V2.UI
{
	public class CursorPredInformation
	{
		public static CursorPredInformation[] CursorPreds { get; set; } = new CursorPredInformation[255];

		public int Index { get; set; }
		public VoreTracker CursorPred => ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(x => x.Predator is null && x.SecondaryPredatorContext == "fatassCursor" && x.SecondaryContextOwner == Index);
		public int CursorWeightGainStage { get; set; }
		public double CursorStomachCapacity
		{
			get => CursorWeightGainStage switch
			{
				0 => 1.25,
				1 => 5.00,
				2 => 20.0,
				3 => 80.0,
				_ => -1.0
			};
		}
		public double CursorDigestionDamage
		{
			get => CursorWeightGainStage switch
			{
				0 => 10,
				1 => 25,
				2 => 60,
				3 => 250,
				_ => -1.0
			};
		}
		public double CursorDigestionSpeed
		{
			get => CursorWeightGainStage switch
			{
				0 => 0.5,
				1 => 1,
				2 => 1.5,
				3 => 2.5,
				_ => -1.0
			};
		}
		public double CursorAbsorptionSpeed
		{
			get => CursorWeightGainStage switch
			{
				0 => 0.10,
				1 => 0.35,
				2 => 0.80,
				3 => 1.75,
				_ => 3.65
			} / V2Utils.SensibleTime(minutes: 1);
		}
		public double CursorWeightGainRatio
		{
			get => CursorWeightGainStage switch
			{
				0 => 1.0,
				1 => 0.80,
				2 => 0.80 * 0.80,
				3 => 0.80 * 0.80 * 0.80,
				_ => 0.80 * 0.80 * 0.80 * 0.80,
			};
		}
	}

	public static partial class UIOverrides
	{
		public static void DrawInterface_36_Cursor()
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
			if (Main.cursorOverride != -1)
			{
				Color color = new Color((int)((float)(int)Main.cursorColor.R * 0.2f), (int)((float)(int)Main.cursorColor.G * 0.2f), (int)((float)(int)Main.cursorColor.B * 0.2f), (int)((float)(int)Main.cursorColor.A * 0.5f));
				Color white = Main.cursorColor;
				bool flag = true;
				bool flag2 = true;
				float rotation = 0f;
				Vector2 vector = default;
				float num = 1f;
				if (Main.cursorOverride == 2)
				{
					flag = false;
					white = Color.White;
					num = 0.7f;
					vector = new Vector2(0.1f);
				}

				switch (Main.cursorOverride)
				{
					case 2:
						flag = false;
						white = Color.White;
						num = 0.7f;
						vector = new Vector2(0.1f);
						break;
					case 3:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
						flag = false;
						white = Color.White;
						break;
				}

				if (flag)
					Main.spriteBatch.Draw(TextureAssets.Cursors[Main.cursorOverride].Value, new Vector2(Main.mouseX + 1, Main.mouseY + 1), null, color, rotation, vector * TextureAssets.Cursors[Main.cursorOverride].Value.Size(), Main.cursorScale * 1.1f * num, SpriteEffects.None, 0f);

				if (flag2)
					Main.spriteBatch.Draw(TextureAssets.Cursors[Main.cursorOverride].Value, new Vector2(Main.mouseX, Main.mouseY), null, white, rotation, vector * TextureAssets.Cursors[Main.cursorOverride].Value.Size(), Main.cursorScale * num, SpriteEffects.None, 0f);
			}
			else if (CursorPredInformation.CursorPreds[Main.myPlayer].CursorPred is not null || CursorPredInformation.CursorPreds[Main.myPlayer].CursorWeightGainStage > 0.0)
			{
				CursorPredInformation myHungryCursor = CursorPredInformation.CursorPreds[Main.myPlayer];
				Color cursorColor = Main.cursorColor;
				string fatCursor = "V2/UI/CursorVore/CursorVore_BaseWeight";
				if (myHungryCursor.CursorWeightGainStage >= 1)
					fatCursor = "V2/UI/CursorVore/CursorVore_Weight1";
				Rectangle frame = new Rectangle(0, 0, 14, 14);
				float rotation = 0f;
				Vector2 origin = new Vector2(1f, 1f);

				switch (myHungryCursor.CursorPred.Prey)
				{
					case List<PreyData> cursorFood when cursorFood is null:
						break;
					case List<PreyData> cursorFood when cursorFood.Count > 1:
						fatCursor = "V2/UI/CursorVore/CursorVore_BaseWeight";
						if (myHungryCursor.CursorWeightGainStage >= 1)
							fatCursor = "V2/UI/CursorVore/CursorVore_Weight1";
						origin = new Vector2(1f, 1f);
						break;
					case List<PreyData> cursorFood when cursorFood.Count == 1 && cursorFood[1].Type == PreyType.NPC && cursorFood[1].ExactType == NPCID.Dryad:
						fatCursor = "V2/UI/CursorVore/CursorVore_BaseWeightToWeight1_Dryad";
						if (!cursorFood[1].NoHealth || cursorFood[1].SizeLeftToDigest >= 0.90)
						{
							frame = new Rectangle(0, 0, 30, 48);
							origin = new Vector2(9f, 1f);
						}
						else if (cursorFood[1].SizeLeftToDigest >= 0.75)
						{
							frame = new Rectangle(32, 0, 26, 44);
							origin = new Vector2(5f, 1f);
						}
						else if (cursorFood[1].SizeLeftToDigest >= 0.55)
						{
							frame = new Rectangle(60, 0, 30, 38);
							origin = new Vector2(7f, 1f);
						}
						else if (cursorFood[1].SizeLeftToDigest >= 0.40)
						{
							frame = new Rectangle(92, 0, 26, 32);
							origin = new Vector2(5f, 1f);
						}
						else if (cursorFood[1].SizeLeftToDigest >= 0.25)
						{
							frame = new Rectangle(120, 0, 22, 28);
							origin = new Vector2(3f, 1f);
						}
						else if (cursorFood[1].SizeLeftToDigest >= 0.15)
						{
							frame = new Rectangle(144, 0, 18, 22);
							origin = new Vector2(1f, 1f);
						}
						else if (cursorFood[1].SizeLeftToDigest >= 0.05)
						{
							frame = new Rectangle(164, 0, 18, 20);
							origin = new Vector2(1f, 1f);
						}
						break;
				}

				Main.spriteBatch.Draw(
					ModContent.Request<Texture2D>(fatCursor).Value,
					Main.MouseScreen,
					frame,
					cursorColor,
					rotation,
					origin,
					Main.cursorScale,
					SpriteEffects.None,
					0f
				);
			}
			else if (Main.SmartCursorIsUsed)
			{
				Main.DrawCursor(Main.DrawThickCursor(smart: true), smart: true);
			}
			else
			{
				Main.DrawCursor(Main.DrawThickCursor());
			}
		}
	}
}
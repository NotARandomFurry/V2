using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;

namespace V2.UI.PredStatsMenu
{
	public class PredStatsMenuUI : UIState
	{
		public static bool Visible { get; set; }

		public static bool GoalsMenuOpen { get; set; }

		private static Asset<Texture2D> _predStatsMenuBackground = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Background", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_GLP = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_GLP", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_TUM = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_TUM", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_ACI = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_ACI", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_ABS = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_ABS", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsOverviewPanel = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_StatOverviewPanel", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsGoalsMenuBook = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_GoalsBook", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsGoalsMenuExitButton = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_ExitButton", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsExitButton = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Exit", AssetRequestMode.ImmediateLoad);

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;
			if (player.AsPred().InPredStatsMenu)
				Visible = true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			Main.LocalPlayer.mouseInterface = true;
			Vector2 backdropPos = new Vector2(
				(Main.screenWidth - _predStatsMenuBackground.Value.Width) / 2,
				(Main.screenHeight - _predStatsMenuBackground.Value.Height) / 2
			);
			spriteBatch.Draw(
				_predStatsMenuBackground.Value,
				backdropPos,
				_predStatsMenuBackground.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);

			Rectangle backdropRect = _predStatsMenuBackground.Value.Bounds;
			backdropRect.X = (int)backdropPos.X;
			backdropRect.Y = (int)backdropPos.Y;

			Rectangle goalsMenuBookRect = new Rectangle(
				(int)backdropPos.X + 644,
				(int)backdropPos.Y + 394,
				30,
				30
			);

			if (GoalsMenuOpen)
			{
				spriteBatch.Draw(
					_predStatsGoalsMenuBook.Value,
					backdropPos + new Vector2(644f, 394f),
					goalsMenuBookRect.Contains(Main.MouseScreen.ToPoint())
					? new Rectangle(32, 0, 30, 30)
					: new Rectangle(0, 0, 30, 30),
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				if (goalsMenuBookRect.Contains(Main.MouseScreen.ToPoint()))
				{
					Main.instance.MouseText(
						"Return to the main pred stats menu"
					);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						SoundEngine.PlaySound(SoundID.MenuClose);
						GoalsMenuOpen = false;
					}
				}

			}
			else
			{
				string hoveredStatSlice = "none";
				#region GLP
				spriteBatch.Draw(
					_predStatsPizzaSlice_GLP.Value,
					backdropPos + new Vector2(560f, 40f),
					_predStatsPizzaSlice_GLP.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle sliceRectGLP = new Rectangle(
					backdropRect.X + 560,
					backdropRect.Y + 40,
					_predStatsPizzaSlice_GLP.Value.Width,
					_predStatsPizzaSlice_GLP.Value.Height
				);
				if (sliceRectGLP.Contains(Main.MouseScreen.ToPoint()))
					hoveredStatSlice = "GLP";
				#endregion
				#region TUM
				spriteBatch.Draw(
					_predStatsPizzaSlice_TUM.Value,
					backdropPos + new Vector2(596f, 40f),
					_predStatsPizzaSlice_TUM.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle sliceRectTUM = new Rectangle(
					backdropRect.X + 596,
					backdropRect.Y + 40,
					_predStatsPizzaSlice_TUM.Value.Width,
					_predStatsPizzaSlice_TUM.Value.Height
				);
				if (sliceRectTUM.Contains(Main.MouseScreen.ToPoint()))
					hoveredStatSlice = "TUM";
				#endregion
				#region ACI
				spriteBatch.Draw(
					_predStatsPizzaSlice_ACI.Value,
					backdropPos + new Vector2(560f, 76f),
					_predStatsPizzaSlice_ACI.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle sliceRectACI = new Rectangle(
					backdropRect.X + 560,
					backdropRect.Y + 76,
					_predStatsPizzaSlice_ACI.Value.Width,
					_predStatsPizzaSlice_ACI.Value.Height
				);
				if (sliceRectACI.Contains(Main.MouseScreen.ToPoint()))
					hoveredStatSlice = "ACI";
				#endregion
				#region ABS
				spriteBatch.Draw(
					_predStatsPizzaSlice_ABS.Value,
					backdropPos + new Vector2(596f, 76f),
					_predStatsPizzaSlice_ABS.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle sliceRectABS = new Rectangle(
					backdropRect.X + 596,
					backdropRect.Y + 76,
					_predStatsPizzaSlice_ABS.Value.Width,
					_predStatsPizzaSlice_ABS.Value.Height
				);
				if (sliceRectABS.Contains(Main.MouseScreen.ToPoint()))
					hoveredStatSlice = "ABS";
				#endregion

				spriteBatch.Draw(
					_predStatsOverviewPanel.Value,
					backdropPos + new Vector2(20f, 20f),
					_predStatsOverviewPanel.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);

				string explainHoveredStatHow = "RelevantStats.Normal";
				if (Main.keyState.IsKeyDown(Keys.LeftShift))
					explainHoveredStatHow = "Description";
				switch (hoveredStatSlice)
				{
					case "GLP":
						ChatManager.DrawColorCodedStringWithShadow(
							spriteBatch,
							FontAssets.MouseText.Value,
							Language.GetTextValueWith(
								"Mods.V2.PredStatsMenu.StatDescription.GLP." + explainHoveredStatHow,
								new
								{
									GLPTotal = Main.LocalPlayer.AsPred().GLP.Total,
									GLPSpent = Main.LocalPlayer.AsPred().GLP.Spent,
									GLPBase = Main.LocalPlayer.AsPred().GLP.Base,
									GLPExtra = Main.LocalPlayer.AsPred().GLP.Extra,
									PreySwallowSize = Main.LocalPlayer.AsPred().SwallowSize.CastToDecimalPlaces(2),
									LiquidSwallowRate = (Main.LocalPlayer.AsPred().LiquidSwallowSize / 255.0 * 60.0).CastToDecimalPlaces(2),
									StruggleGracePeriod = Main.LocalPlayer.AsPred().StruggleGraceTimeReadable,
									PredPlayer.BaseSwallowSize,
									PredPlayer.SwallowSizePerLevel,
									BaseDrinkRate = (PredPlayer.BaseLiquidSwallowSize / 255.0 * 60.0).CastToDecimalPlaces(2),
									DrinkRatePer5Levels = (PredPlayer.LiquidSwallowSizePer5Levels / 255.0 * 60.0).CastToDecimalPlaces(2),
									PredPlayer.BaseStruggleGraceTime,
									PredPlayer.StruggleGraceTimePer5Levels,
								}
							),
							backdropPos + new Vector2(30f, 30f),
							Color.White,
							0f,
							Vector2.Zero,
							new Vector2(0.8f)
						);
						break;
					case "TUM":
						ChatManager.DrawColorCodedStringWithShadow(
							spriteBatch,
							FontAssets.MouseText.Value,
							Language.GetTextValueWith(
								"Mods.V2.PredStatsMenu.StatDescription.TUM." + explainHoveredStatHow,
								new
								{
									TUMTotal = Main.LocalPlayer.AsPred().TUM.Total,
									TUMSpent = Main.LocalPlayer.AsPred().TUM.Spent,
									TUMBase = Main.LocalPlayer.AsPred().TUM.Base,
									TUMExtra = Main.LocalPlayer.AsPred().TUM.Extra,
									StomachCapacity = Main.LocalPlayer.AsPred().StomachCapacity.CastToDecimalPlaces(2),
									StomachacheMeterCapacity = Main.LocalPlayer.AsPred().StomachacheMeterCapacity.CastToDecimalPlaces(2),
									StruggleChartEstimatedDifficulty = "Something for me to figure out later",
									PredPlayer.BaseStomachCapacity,
									PredPlayer.StomachCapacityPerLevel,
									PredPlayer.BaseStomachacheMeterCapacity,
									PredPlayer.StomachacheMeterCapacityPer5Levels,
								}
							),
							backdropPos + new Vector2(30f, 30f),
							Color.White,
							0f,
							Vector2.Zero,
							new Vector2(0.8f)
						);
						break;
					case "ACI":
						string acidTierKey = "Mods.V2.PredStatsMenu.StatDescription.ACI.AcidTiers.NormalBeta";
						ChatManager.DrawColorCodedStringWithShadow(
							spriteBatch,
							FontAssets.MouseText.Value,
							Language.GetTextValueWith(
								"Mods.V2.PredStatsMenu.StatDescription.ACI." + explainHoveredStatHow,
								new
								{
									ACITotal = Main.LocalPlayer.AsPred().ACI.Total,
									ACISpent = Main.LocalPlayer.AsPred().ACI.Spent,
									ACIBase = Main.LocalPlayer.AsPred().ACI.Base,
									ACIExtra = Main.LocalPlayer.AsPred().ACI.Extra,
									DigestionDamage = Main.LocalPlayer.AsPred().DigestionTickDamage.CastToDecimalPlaces(2),
									DigestionRate = Main.LocalPlayer.AsPred().DigestionTickRate.CastToDecimalPlaces(2),
									AcidTierWithDescription = Language.GetTextValue(acidTierKey),
									BaseDigestionDamage = PredPlayer.BaseDigestionTickDamage,
									DigestionDamagePerLevel = PredPlayer.DigestionTickDamagePerLevel,
									BaseDigestionRate = PredPlayer.BaseDigestionTickRate,
									DigestionRatePer5Levels = PredPlayer.DigestionTickRatePer5Levels,
								}
							),
							backdropPos + new Vector2(30f, 30f),
							Color.White,
							0f,
							Vector2.Zero,
							new Vector2(0.8f)
						);
						break;
					case "ABS":
						ChatManager.DrawColorCodedStringWithShadow(
							spriteBatch,
							FontAssets.MouseText.Value,
							Language.GetTextValueWith(
								"Mods.V2.PredStatsMenu.StatDescription.ABS." + explainHoveredStatHow,
								new
								{
									ABSTotal = Main.LocalPlayer.AsPred().ABS.Total,
									ABSSpent = Main.LocalPlayer.AsPred().ABS.Spent,
									ABSBase = Main.LocalPlayer.AsPred().ABS.Base,
									ABSExtra = Main.LocalPlayer.AsPred().ABS.Extra,
									AbsorptionRate = Main.LocalPlayer.AsPred().PreyAbsorptionRate.CastToDecimalPlaces(2),
									BuffExtensionTime = Main.LocalPlayer.AsPred().BuffExtensionTime.ToPercentage(2),
									DebuffDisextensionTime = Main.LocalPlayer.AsPred().DebuffDisextensionTime.ToPercentage(2),
									BaseAbsorbRate = PredPlayer.BasePreyAbsorptionRate,
									AbsorbRatePerLevel = PredPlayer.PreyAbsorptionRatePerLevel,
									BuffExtendPer5Levels = PredPlayer.BuffExtensionTimePer5Levels,
									DebuffLossPer5Levels = PredPlayer.DebuffDisextensionTimePer5Levels,
								}
							),
							backdropPos + new Vector2(30f, 30f),
							Color.White,
							0f,
							Vector2.Zero,
							new Vector2(0.8f)
						);
						break;
					default:
						break;
				}
				spriteBatch.Draw(
					_predStatsGoalsMenuBook.Value,
					backdropPos + new Vector2(644f, 394f),
					goalsMenuBookRect.Contains(Main.MouseScreen.ToPoint())
					? new Rectangle(32, 0, 30, 30)
					: new Rectangle(0, 0, 30, 30),
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				if (goalsMenuBookRect.Contains(Main.MouseScreen.ToPoint()))
				{
					Main.instance.MouseText(
						"Open the pred goals menu"
					);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						SoundEngine.PlaySound(SoundID.MenuOpen);
						GoalsMenuOpen = true;
					}
				}

				spriteBatch.Draw(
					_predStatsExitButton.Value,
					backdropPos + new Vector2(350f, 10f),
					_predStatsExitButton.Value.Bounds,
					Color.White,
					0f,
					new Vector2(29, 0),
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle exitGulletRect = new Rectangle(
					(int)backdropPos.X + 343,
					(int)backdropPos.Y + 20,
					14,
					14
				);
				if (exitGulletRect.Contains(Main.MouseScreen.ToPoint()))
				{
					Main.instance.MouseText(
						"Close the pred stats menu\n"
					  + "(Are you sure your cursor can't stay a little longer?)"
					);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						PredStatsMenuMouthUI.MouthState = PredStatsMenuMouthState.RegurgitatingCursor;
					}
				}
			}
		}
	}
}
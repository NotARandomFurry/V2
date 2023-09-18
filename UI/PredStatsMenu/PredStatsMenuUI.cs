using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals;

namespace V2.UI.PredStatsMenu
{
	public class PredStatsMenuUI : UIState
	{
		public static bool Visible { get; set; }

		public static bool GoalsMenuOpen { get; set; }
		public static ProgressionStage SelectedProgressionStage { get; set; }
		public static int GoalsPage { get; set; }

		private static Asset<Texture2D> _predStatsMenuBackground = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Background", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_GLP = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_GLP", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_TUM = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_TUM", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_ACI = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_ACI", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsPizzaSlice_ABS = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_ABS", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsOverviewPanel = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_StatOverviewPanel", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsAvailablePointsPanel = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_AvailableStatPointsPanel", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsGoalsMenuBook = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_GoalsBook", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsStageDeselected = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_SectionDeselected", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsStageSelected = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_SectionSelected", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsGoalsMenuExitButton = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_ExitButton", AssetRequestMode.ImmediateLoad);
		private static Asset<Texture2D> _predStatsExitButton = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Exit", AssetRequestMode.ImmediateLoad);
		private static readonly SoundStyle AllocateSuccess = new SoundStyle("V2/Sounds/PredStatsMenu/AllocateSuccess", SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0f };
		private static readonly SoundStyle AllocateFail = new SoundStyle("V2/Sounds/PredStatsMenu/AllocateFail", SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0f };

		public override void OnInitialize()
		{
			SelectedProgressionStage = ModContent.GetInstance<StarterStage>();
		}

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

			float mouseTextColorFactor = (float)(int)Main.mouseTextColor / 255f;

			if (GoalsMenuOpen)
			{
				List<PredPlayerGoal> selectedStageGoals = ModContent.GetContent<PredPlayerGoal>().ToList();
				List<ProgressionStage> stagesOrdered = PredPlayerGoalLoader.ProgressionStages.OrderBy(x => x.Order).ToList();

				selectedStageGoals.RemoveAll(x => x.Stage != SelectedProgressionStage);

				int columnCountPerPage = 7;
				int rowCountPerPage = 4;
				int totalGoalsPerPage = columnCountPerPage * rowCountPerPage;
				int goalPages = 1 + (int)Math.Floor((double)(selectedStageGoals.Count - 1) / (double)totalGoalsPerPage);
				int firstIndexForThisPage = (GoalsPage - 1) * totalGoalsPerPage;
				int lastIndexForThisPage = firstIndexForThisPage + (columnCountPerPage * rowCountPerPage) - 1;
				if (lastIndexForThisPage >= selectedStageGoals.Count)
					lastIndexForThisPage = selectedStageGoals.Count - 1;

				for (int i = firstIndexForThisPage; i <= lastIndexForThisPage; i++)
				{
					int placementIndex = i % totalGoalsPerPage;
					int x = placementIndex % columnCountPerPage;
					int y = (int)Math.Floor((double)placementIndex / (double)columnCountPerPage);
					PredPlayerGoal goalToDraw = selectedStageGoals[i];
					Vector2 goalDrawPos = backdropPos + new Vector2(80f, 100f) + new Vector2((PredPlayerGoal.TextureBounds.Width + 2) * x, (PredPlayerGoal.TextureBounds.Height + 2) * y);
					spriteBatch.Draw(
						goalToDraw.Complete(Main.LocalPlayer)
						? goalToDraw.CompleteTexture
						: goalToDraw.IncompleteTexture,
						goalDrawPos,
						PredPlayerGoal.TextureBounds,
						Color.White,
						0f,
						Vector2.Zero,
						1f,
						SpriteEffects.None,
						0f
					);

					Rectangle goalHoverRect = new Rectangle(
						(int)goalDrawPos.X,
						(int)goalDrawPos.Y,
						PredPlayerGoal.TextureBounds.Width,
						PredPlayerGoal.TextureBounds.Height
					);
					if (goalHoverRect.Contains(Main.MouseScreen.ToPoint()))
					{
						string goalFullHoverText =
							"[c/"
						  + (goalToDraw.DisplayNameColor(Main.LocalPlayer) * mouseTextColorFactor).Hex3()
						  + ":"
						  + goalToDraw.DisplayName(Main.LocalPlayer)
						  + "]\n[c/"
						  + (new Color(127, 127, 127) * mouseTextColorFactor).Hex3()
						  + ":"
						  + (goalToDraw.Complete(Main.LocalPlayer) ? "Complete" : "Incomplete")
						  + "; worth "
						  + goalToDraw.StatPointsFromCompletion
						  + " stat point"
						  + (goalToDraw.StatPointsFromCompletion == 1 ? "" : "s") + "]\n";
						string[] goalFullHoverTextLines = Utils.WordwrapString(goalToDraw.Description(Main.LocalPlayer), FontAssets.MouseText.Value, 720, 15, out _);
						foreach (string piece in goalFullHoverTextLines)
						{
							if (piece is not null && piece != "")
							{
								goalFullHoverText += piece;
								if (!piece.Contains('\n'))
									goalFullHoverText += "\n";
							}
						}
						UICommon.TooltipMouseText(goalFullHoverText);
						Main.mouseText = true;
					}
				}

				int selectedStageIndex = stagesOrdered.IndexOf(SelectedProgressionStage);
				for (int i = 0; i < stagesOrdered.Count; i++)
				{
					ProgressionStage stageToDraw = stagesOrdered[i];
					Vector2 stageTabDrawPos = backdropPos + new Vector2(10f, 100f);
					if (i <= selectedStageIndex)
						stageTabDrawPos.Y += i * 20;
					else
						stageTabDrawPos.Y += (i + 1) * 20;

					spriteBatch.Draw(
						i == selectedStageIndex
						? _predStatsStageSelected.Value
						: _predStatsStageDeselected.Value,
						stageTabDrawPos,
						i == selectedStageIndex
						? _predStatsStageSelected.Value.Bounds
						: _predStatsStageDeselected.Value.Bounds,
						Color.White,
						0f,
						Vector2.Zero,
						1f,
						SpriteEffects.None,
						0f
					);

					Rectangle stageHoverRect = new Rectangle(
						(int)stageTabDrawPos.X,
						(int)stageTabDrawPos.Y,
						i == selectedStageIndex ? _predStatsStageSelected.Value.Width : _predStatsStageDeselected.Value.Width,
						i == selectedStageIndex ? _predStatsStageSelected.Value.Height : _predStatsStageDeselected.Value.Height
					);
					if (stageHoverRect.Contains(Main.MouseScreen.ToPoint()))
					{
						Color nameColor = Color.LimeGreen;
						Color subtitleColor = new Color(127, 127, 127);
						string stageFullHoverText =
							"[c/" + (nameColor * mouseTextColorFactor).Hex3() + ":" + stageToDraw.DisplayName + "]\n"
						  + "[c/" + (subtitleColor * mouseTextColorFactor).Hex3() + ":" + stageToDraw.DisplaySubtitle + "]\n";
						string[] stageFullHoverTextLines = Utils.WordwrapString(stageToDraw.Description, FontAssets.MouseText.Value, 600, 10, out _);
						foreach (string piece in stageFullHoverTextLines)
						{
							if (piece is not null && piece != "")
							{
								stageFullHoverText += piece;
								if (!piece.Contains('\n'))
									stageFullHoverText += "\n";
							}
						}
						UICommon.TooltipMouseText(stageFullHoverText);
						Main.mouseText = true;
						if (Main.mouseLeft && Main.mouseLeftRelease)
						{
							SoundEngine.PlaySound(SoundID.MenuTick);
							SelectedProgressionStage = stageToDraw;
						}
					}
				}

				spriteBatch.Draw(
					_predStatsGoalsMenuExitButton.Value,
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
				void SliceHoverLogic(Rectangle sliceRect, PredStat stat, string statFullName, string statShorthand)
				{
					if (!sliceRect.Contains(Main.MouseScreen.ToPoint()))
						return;

					hoveredStatSlice = statShorthand;
					UICommon.TooltipMouseText(
						Language.GetTextValueWith(
							"Mods.V2.PredStatsMenu.StatSliceHover",
							new {
								PredStatName = statFullName,
								PredStatShorthand = statShorthand,
								SpentPoints = stat.Spent,
							}
						)
					);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						if (Main.LocalPlayer.AsPred().AvailableStatPoints == 0)
						{
							SoundEngine.PlaySound(AllocateFail);
						}
						else
						{
							stat.Spent++;
							SoundEngine.PlaySound(AllocateSuccess);
						}
					}
					else if (Main.mouseRight && Main.mouseRightRelease)
					{
						if (stat.Spent == 0)
						{
							SoundEngine.PlaySound(AllocateFail);
						}
						else
						{
							stat.Spent--;
							SoundEngine.PlaySound(AllocateSuccess);
						}
					}
				}

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
				SliceHoverLogic(sliceRectGLP, Main.LocalPlayer.AsPred().GLP, "Swallow strength", "GLP");
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
				SliceHoverLogic(sliceRectTUM, Main.LocalPlayer.AsPred().TUM, "Stomach strength", "TUM");
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
				SliceHoverLogic(sliceRectACI, Main.LocalPlayer.AsPred().ACI, "Acid strength", "ACI");
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
				SliceHoverLogic(sliceRectABS, Main.LocalPlayer.AsPred().ABS, "Absorption strength", "ABS");
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

				if (hoveredStatSlice != "none")
				{
					string explainHoveredStatHow = "RelevantStats.Normal";
					if (Main.keyState.IsKeyDown(Keys.LeftShift))
						explainHoveredStatHow = "Description";

					string acidTierKey = "Mods.V2.PredStatsMenu.StatDescription.ACI.AcidTiers.NormalBeta";

					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValueWith(
							"Mods.V2.PredStatsMenu.StatDescription." + hoveredStatSlice + "." + explainHoveredStatHow,
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
						new Vector2(0.8f),
						480
					);
				}

				spriteBatch.Draw(
					_predStatsAvailablePointsPanel.Value,
					backdropPos + new Vector2(560f, 192f),
					_predStatsAvailablePointsPanel.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				ChatManager.DrawColorCodedStringWithShadow(
					Main.spriteBatch,
					FontAssets.MouseText.Value,
					"Points Available",
					backdropPos + new Vector2(564f, 198f),
					Color.White,
					0f,
					Vector2.Zero,
					new Vector2(0.8f),
					112
				);
				ChatManager.DrawColorCodedStringWithShadow(
					Main.spriteBatch,
					FontAssets.DeathText.Value,
					Main.LocalPlayer.AsPred().AvailableStatPoints.ToString(),
					backdropPos + new Vector2(620f, 252f),
					Color.White,
					0f,
					ChatManager.GetStringSize(FontAssets.DeathText.Value, Main.LocalPlayer.AsPred().AvailableStatPoints.ToString(), new Vector2(0.8f)) / 2f,
					new Vector2(0.8f)
				);

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
						GoalsPage = 1;
						SelectedProgressionStage = ModContent.GetInstance<StarterStage>();
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
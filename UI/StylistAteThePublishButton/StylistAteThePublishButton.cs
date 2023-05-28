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
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Gamepad;
using V2.NPCs;
using V2.NPCs.Vanilla.TownNPCs.Stylist;
using V2.PlayerHandling;

namespace V2.UI.StylistAteThePublishButton
{
	public static class AntiPublishProtection
	{
		public static void EnsurePublishButtonGetsGulped()
		{
			// Return if not on the Mod Sources menu, as the Publish button can't get got from anywhere else.
			if (!Main.gameMenu)
				return;

			// The Publish button is a liability. Thankfully, the Stylist is the best tool for removing liabilities...by turning them into stylish hair stylist tum fat.
			FieldInfo UIModSourcesObjectInfo =
				typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.Interface")!.GetField("modSources", BindingFlags.NonPublic | BindingFlags.Static)!;
			object UIModSources = UIModSourcesObjectInfo.GetValue(null);
			if (UIModSources is null)
				return;

			FieldInfo UIModSourceItemListInfo =
				typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIModSources")!.GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)!;
			List<UIPanel> UIModSourceItemList = (List<UIPanel>)UIModSourceItemListInfo.GetValue(UIModSources);
			if (UIModSourceItemList is null || UIModSourceItemList.Count <= 0)
				return;

			object V2ModSourceItem;
			FieldInfo modNameInfo =
				typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIModSourceItem")!.GetField("_mod", BindingFlags.NonPublic | BindingFlags.Instance)!;
			foreach (UIPanel UIModSourceItem in UIModSourceItemList)
			{
				if ((string)modNameInfo.GetValue(UIModSourceItem) == "V2")
				{
					V2ModSourceItem = UIModSourceItem;
					goto PublishButtonIsALiability;
				}
			}

			// Return, as the Publish button is not a liability for non-Voraria mods.
			return;

			PublishButtonIsALiability:
			UIPanel V2ModSourcePanel = V2ModSourceItem as UIPanel;
			string publishButtonText = Language.GetTextValue("tModLoader.MSPublish");
			UIAutoScaleTextTextPanel<string> publishButton;
			foreach (UIElement element in V2ModSourcePanel.Children)
			{
				if (element is not UIAutoScaleTextTextPanel<string>)
					continue;

				if (element.Left.Pixels == 390)
				{
					publishButton = element as UIAutoScaleTextTextPanel<string>;
					goto PublishButtonCanBeSuccessfullySwallowed;
				}
			}

			// Return, as the Publish button has evaded the Stylist's stomach by hiding away...for now. It'll be reduced to stomach sludge soon enough...
			return;

			PublishButtonCanBeSuccessfullySwallowed:
			UIImage stylistAteThePublishButton = new UIImage(ModContent.Request<Texture2D>("V2/UI/StylistAteThePublishButton/StylistAteThePublishButton_Belly"))
			{
				Height = { Pixels = 144 },
				Width = { Pixels = 56 },
				Left = { Pixels = 390 },
				Top = { Pixels = 40 },
				PaddingRight = 32,
				PaddingTop = 14
			};
			stylistAteThePublishButton.OnMouseOver += StylistAteThePublishButton_OnMouseOver;

			V2ModSourcePanel.RemoveChild(publishButton);
			V2ModSourcePanel.Append(stylistAteThePublishButton);
			V2ModSourcePanel.RecalculateChildren();
		}

		private static void StylistAteThePublishButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.instance.MouseTextNoOverride(
				"Don't worry about a misclick; [c/FFFF00:everything's okay]!\n"
			  + "The Publish button is [c/BFBF00:safe in the depths of the Stylist's hungry stomach].\n"
			  + "As such, Voraria: Second Course [c/BF3F00:can never be published to the Workshop]."
			);
		}
	}
}
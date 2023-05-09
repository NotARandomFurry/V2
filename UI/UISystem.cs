using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace V2.UI
{
	public class UISystem : ModSystem
	{
		public UserInterface HeldItemInterfaceLayer;
		public HeldItemDrawingUI HeldItemInterface;

		public UserInterface StomachCapacityBarInterfaceLayer;
		public StomachCapacityBarUI StomachCapacityBarInterface;

		public override void OnWorldLoad()
		{
			HeldItemInterfaceLayer = new UserInterface();
			HeldItemInterface = new HeldItemDrawingUI();
			HeldItemInterface.Activate();
			HeldItemInterfaceLayer.SetState(HeldItemInterface);

			StomachCapacityBarInterfaceLayer = new UserInterface();
			StomachCapacityBarInterface = new StomachCapacityBarUI();
			StomachCapacityBarInterface.Activate();
			StomachCapacityBarInterfaceLayer.SetState(StomachCapacityBarInterface);
		}
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (layers.FirstOrDefault(x => x.Name == "Vanilla: Hair Window") is LegacyGameInterfaceLayer hairStyleWindowLegacyLayer)
			{
				int hairStyleWindowLegacyLayerIndex = layers.IndexOf(hairStyleWindowLegacyLayer);
				layers.Remove(hairStyleWindowLegacyLayer);
				layers.Insert(
					hairStyleWindowLegacyLayerIndex, new LegacyGameInterfaceLayer(
						"Vanilla: Hair Window (Voraria II Override)",
						delegate
						{
							if (Main.LocalPlayer.talkNPC != -1)
							{
								NPC stylist = Main.npc[Main.LocalPlayer.talkNPC];
								if (stylist.active && stylist.type == NPCID.Stylist)
									UIOverrides.DrawInterface_21_HairWindow(stylist);
							}
							return true;
						},
						InterfaceScaleType.UI
					)
				);
			}
			if (layers.FirstOrDefault(x => x.Name == "Vanilla: Death Text") is LegacyGameInterfaceLayer deathTextLegacyLayer)
			{
				int deathTextLegacyLayerIndex = layers.IndexOf(deathTextLegacyLayer);
				layers.Remove(deathTextLegacyLayer);
				layers.Insert(
					deathTextLegacyLayerIndex, new LegacyGameInterfaceLayer(
						"Vanilla: Death Text (Voraria II Override)",
						delegate
						{
							UIOverrides.DrawInterface_35_YouDied();
							return true;
						},
						InterfaceScaleType.UI
					)
				);
			}

			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex == -1)
				return;

			AddInterfaceLayer(layers, HeldItemInterfaceLayer, HeldItemInterface, MouseTextIndex, true, "Held Item");
			AddInterfaceLayer(layers, StomachCapacityBarInterfaceLayer, StomachCapacityBarInterface, MouseTextIndex, true, "Stomach Capacity");
		}

		public void AddInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, string customName = null)
		{
			string name;
			if (customName == null)
				name = state.ToString();
			else
				name = customName;

			layers.Insert(index, new LegacyGameInterfaceLayer("Voraria II: " + name,
				delegate
				{
					if (visible)
					{
						userInterface.Update(Main._drawInterfaceGameTime);
						state.Draw(Main.spriteBatch);
					}
					return true;
				}, InterfaceScaleType.UI
			));
		}
	}
}

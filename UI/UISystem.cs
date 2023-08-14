using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using V2.UI.PredStatsMenu;
using V2.UI.StomachacheMeter;
using V2.UI.StomachCapacityMeter;

namespace V2.UI
{
	public class UISystem : ModSystem
	{
		public UserInterface HeldItemInterfaceLayer;
		public HeldItemDrawingUI HeldItemInterface;

		public UserInterface StomachCapacityBarInterfaceLayer;
		public StomachCapacityMeterUI StomachCapacityBarInterface;

		public UserInterface StomachacheMeterInterfaceLayer;
		public StomachacheMeterUI StomachacheMeterInterface;

		public UserInterface PredStatsMenuMouthInterfaceLayer;
		public PredStatsMenuMouthUI PredStatsMenuMouthInterface;
		public UserInterface PredStatsMenuInterfaceLayer;
		public PredStatsMenuUI PredStatsMenuInterface;

		public override void OnWorldLoad()
		{
			HeldItemInterfaceLayer = new UserInterface();
			HeldItemInterface = new HeldItemDrawingUI();
			HeldItemInterface.Activate();
			HeldItemInterfaceLayer.SetState(HeldItemInterface);

			StomachCapacityBarInterfaceLayer = new UserInterface();
			StomachCapacityBarInterface = new StomachCapacityMeterUI();
			StomachCapacityBarInterface.Activate();
			StomachCapacityBarInterfaceLayer.SetState(StomachCapacityBarInterface);

			StomachacheMeterInterfaceLayer = new UserInterface();
			StomachacheMeterInterface = new StomachacheMeterUI();
			StomachacheMeterInterface.Activate();
			StomachacheMeterInterfaceLayer.SetState(StomachacheMeterInterface);

			PredStatsMenuMouthInterfaceLayer = new UserInterface();
			PredStatsMenuMouthInterface = new PredStatsMenuMouthUI();
			PredStatsMenuMouthInterface.Activate();
			PredStatsMenuMouthInterfaceLayer.SetState(PredStatsMenuMouthInterface);
			PredStatsMenuInterfaceLayer = new UserInterface();
			PredStatsMenuInterface = new PredStatsMenuUI();
			PredStatsMenuInterface.Activate();
			PredStatsMenuInterfaceLayer.SetState(PredStatsMenuInterface);
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

			int OverriddenHairWindowIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hair Window (Voraria II Override)"));
			if (OverriddenHairWindowIndex != -1)
			{
				AddInterfaceLayer(layers, StomachCapacityBarInterfaceLayer, StomachCapacityBarInterface, OverriddenHairWindowIndex, true, "Stomach Capacity Meter");
				AddInterfaceLayer(layers, StomachacheMeterInterfaceLayer, StomachacheMeterInterface, OverriddenHairWindowIndex + 1, true, "Stomachache Meter");
				AddInterfaceLayer(layers, PredStatsMenuInterfaceLayer, PredStatsMenuInterface, OverriddenHairWindowIndex + 2, true, "Pred Stats Menu");
				AddInterfaceLayer(layers, PredStatsMenuMouthInterfaceLayer, PredStatsMenuMouthInterface, OverriddenHairWindowIndex + 3, true, "Pred Stats Menu's Hungry Mouth");
			}
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
				AddInterfaceLayer(layers, HeldItemInterfaceLayer, HeldItemInterface, MouseTextIndex, true, "Held Item");
		}

		public static void AddInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, string customName = null)
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

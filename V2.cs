using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core.StruggleSystem;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling.PredPlayerGoals;

namespace V2
{
	public partial class V2 : Mod
	{
		internal static V2 Instance;

		public static ModKeybind SwallowHotkey { get; set; }
		public static ModKeybind RegurgitateHotkey { get; set; }
		public static ModKeybind FeedHotkey { get; set; }
		public static ModKeybind ItemGulpHotkey { get; set; }
		public static ModKeybind StruggleUpHotkey { get; set; }
		public static ModKeybind StruggleLeftHotkey { get; set; }
		public static ModKeybind StruggleRightHotkey { get; set; }
		public static ModKeybind StruggleDownHotkey { get; set; }
		public static ModKeybind StruggleSpecialHotkey { get; set; }

		public static List<int> VoreNPCBlacklist { get; set; }

		public V2()
		{
			Instance = this;
		}

		public override void Load()
		{
			SwallowHotkey = KeybindLoader.RegisterKeybind(this, "Swallow", "V");
			RegurgitateHotkey = KeybindLoader.RegisterKeybind(this, "Regurgitate", "X");
			FeedHotkey = KeybindLoader.RegisterKeybind(this, "Feed", "G");
			ItemGulpHotkey = KeybindLoader.RegisterKeybind(this, "EatItems", "RightShift");
			StruggleUpHotkey = KeybindLoader.RegisterKeybind(this, "StruggleUp", "Up");
			StruggleLeftHotkey = KeybindLoader.RegisterKeybind(this, "StruggleLeft", "Left");
			StruggleRightHotkey = KeybindLoader.RegisterKeybind(this, "StruggleRight", "Right");
			StruggleDownHotkey = KeybindLoader.RegisterKeybind(this, "StruggleDown", "Down");
			StruggleSpecialHotkey = KeybindLoader.RegisterKeybind(this, "StruggleSpecial", "Space");

			BetterDialogue.BetterDialogue.SupportedNPCs.Add(ModContent.NPCType<Lucinda>());

			BetterDialogue.BetterDialogue.RegisterShoppableNPC(ModContent.NPCType<Lucinda>());

			StruggleChartLoader.Load();

			EngageVoraciousGameFuckery();
		}

		public override void Unload()
		{
			StruggleChartLoader.Unload();

			DisengageVoraciousGameFuckery();
		}

		public override void PostSetupContent()
		{
			VoreNPCBlacklist = new List<int>
			{
				NPCID.Angler,
				NPCID.SleepingAngler,
				NPCID.Princess,
			};
			if (ModContent.TryFind("Fargowiltas", "Deviantt", out ModNPC Deviantt))
				VoreNPCBlacklist.Add(Deviantt.Type);
		}
	}
}
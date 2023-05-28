using Ionic.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using ReLogic.Content;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;
using V2.Core.MainDetours;
using V2.NPCs;
using V2.PlayerHandling;
using V2.UI;

namespace V2
{
	public partial class V2 : Mod
	{
		internal static V2 Instance;

		public static ModKeybind SwallowHotkey;
		public static ModKeybind RegurgitateHotkey;
		public static ModKeybind FeedHotkey;

		public static Asset<Texture2D> ChatBackground;

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

			ChatBackground = ModContent.Request<Texture2D>("V2/UI/Chat_BigBack");
			EngageVoraciousGameFuckery();
		}

		public override void Unload()
		{
			ChatBackground = null;

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
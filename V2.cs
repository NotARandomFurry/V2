using Ionic.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoMod.RuntimeDetour.HookGen;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
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

			EngageGameFuckery2VoraciousBoogaloo();
		}

		public override void PostSetupContent()
		{
			VoreNPCBlacklist = new List<int>
			{
				NPCID.Angler,
				NPCID.Princess,
			};
			if (ModContent.TryFind("Fargowiltas", "Deviantt", out ModNPC Deviantt))
				VoreNPCBlacklist.Add(Deviantt.Type);
		}

		private delegate void orig_NPCAI(NPC npc);

		private delegate void hook_NPCAI(orig_NPCAI orig, NPC npc);

		private static readonly MethodInfo NPCLoader_NPCAI_MethodInfo =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.NPCLoader")!.GetMethod("NPCAI", BindingFlags.Public | BindingFlags.Static)!;

		private delegate void orig_SetChatButtons(ref string button, ref string button2);

		private delegate void hook_SetChatButtons(orig_SetChatButtons orig, ref string button, ref string button2);

		private static readonly MethodInfo NPCLoader_SetChatButtons_MethodInfo =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.NPCLoader")!.GetMethod("SetChatButtons", BindingFlags.Public | BindingFlags.Static)!;

		public static void EngageGameFuckery2VoraciousBoogaloo()
		{
			HookEndpointManager.Add<hook_NPCAI>(NPCLoader_NPCAI_MethodInfo, (orig_NPCAI orig, NPC npc) =>
			{
				PredNPC npcAsPred = npc.AsPred(risky: true);
				PreyNPC npcAsPrey = npc.AsPrey(risky: true);
				if (npcAsPred is null || npcAsPrey is null)
					orig(npc);

				if (npcAsPrey.IsCurrentlyEaten)
				{
					npc.velocity = Vector2.Zero;
					if (npcAsPrey.CurrentCaptor.HasValue)
					{
						npc.position = npcAsPrey.CurrentCaptor.Value.Predator.position;
						if (npcAsPrey.PreyAIMethod is not null)
							npcAsPrey.PreyAIMethod.Invoke(npc, npcAsPrey.CurrentCaptor.Value.Predator);
						NPCLoader.PostAI(npc);
					}
				}
				else if (npcAsPred.SpecialPredAIMethod != null)
				{
					npcAsPred.SpecialPredAIMethod.Invoke(npc);
					NPCLoader.PostAI(npc);
				}
				else
					orig(npc);
			});

			HookEndpointManager.Add<hook_SetChatButtons>(NPCLoader_SetChatButtons_MethodInfo, (orig_SetChatButtons orig, ref string button, ref string button2) =>
			{
				if (Main.player[Main.myPlayer].talkNPC >= 0)
				{
					NPC npc = Main.npc[Main.player[Main.myPlayer].talkNPC];
					npc.ModNPC?.SetChatButtons(ref button, ref button2);

					PredNPC npcAsPred = npc.AsPred(risky: true);
					if (npcAsPred is not null && npcAsPred.ModifyChatButtonsMethod is not null)
						npcAsPred.ModifyChatButtonsMethod.Invoke(npc, Main.player[Main.myPlayer], ref button, ref button2);
				}
			});

			On.Terraria.Main.UpdateAudio_DecideOnNewMusic += (orig, instance) => MainDetours.UpdateAudio_DecideOnNewMusic();

			On.Terraria.NPC.CanBeChasedBy += (orig, npc, attacker, ignoreDontTakeDamage) =>
			{
				if (npc.active)
				{
					PreyNPC npcAsPrey = npc.AsPrey(risky: true);
					if (npcAsPrey is not null)
					{
						PreyNPC.UpdateNPCEatenStatus(npc);
						if (npcAsPrey.IsCurrentlyEaten)
							return false;
					}
				}

				return orig(npc, attacker, ignoreDontTakeDamage);
			};
			On.Terraria.NPC.checkDead += (orig, npc) => NPCDetours.checkDead(npc);
			On.Terraria.NPC.NPCLoot_DropHeals += (orig, npc, closestPlayer) =>
			{
				if (!npc.AsPrey().Digested)
					orig(npc, closestPlayer);
			};
			On.Terraria.NPC.NPCLoot_DropMoney += (orig, npc, closestPlayer) =>
			{
				if (!npc.AsPrey().Digested)
					orig(npc, closestPlayer);
			};
			On.Terraria.NPC.NPCLoot_DropItems += (orig, npc, closestPlayer) =>
			{
				if (!npc.AsPrey().Digested)
					orig(npc, closestPlayer);
			};
			On.Terraria.NPC.DoDeathEvents_DropBossPotionsAndHearts += NoPotionsOrHeartsIfDigested;
			On.Terraria.NPC.DoDeathEvents_CelebrateBossDeath += (orig, npc, typeName) => NPCDetours.DoDeathEvents_CelebrateBossDeath(npc, typeName);

			On.Terraria.Player.KillMe += (orig, player, damageSource, dmg, hitDirection, pvp) => PlayerDetours.KillMe(player, damageSource, dmg, hitDirection, pvp);
		}

		private static void NoPotionsOrHeartsIfDigested(On.Terraria.NPC.orig_DoDeathEvents_DropBossPotionsAndHearts orig, NPC npc, ref string typeName)
		{
			if (!npc.AsPrey().Digested)
				orig(npc, ref typeName);
		}
	}
}
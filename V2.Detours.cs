using Ionic.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using V2.Core.MainDetours;
using V2.NPCs;
using V2.PlayerHandling;
using V2.UI;
using V2.UI.PredStatsMenu;
using V2.UI.StylistAteThePublishButton;

namespace V2
{
	public partial class V2 : Mod
	{
		/*
		private delegate void orig_UIModSourceItem_Constructor(string mod, object builtMod);
		private static Type ModLoaderCore_LocalMod_Type = typeof(Main).Assembly.GetType("Terraria.ModLoader.Core.LocalMod");
		private static Type ModLoaderUI_UIModSourceItem_Type = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIModSourceItem");
		private static Type[] ModLoaderUI_UIModSourceItem_ConstructorArgs = new Type[] { typeof(string), ModLoaderCore_LocalMod_Type };
		internal static Hook ModLoaderUI_UIModSourceItem_ConstructorHook;
		private static ConstructorInfo ModLoaderUI_UIModSourceItem_ConstructorInfo =
			ModLoaderUI_UIModSourceItem_Type!.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, ModLoaderUI_UIModSourceItem_ConstructorArgs)!;

		private delegate void orig_UpdateUI(GameTime gameTime);
		internal static Hook SystemLoader_UpdateUI_Hook;
		private static readonly MethodInfo SystemLoader_UpdateUI_MethodInfo =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.SystemLoader")!.GetMethod("UpdateUI", BindingFlags.Public | BindingFlags.Static)!;
		*/

		private delegate void orig_NPCAI(NPC npc);
		internal static Hook NPCLoader_NPCAI_Hook;
		private static readonly MethodInfo NPCLoader_NPCAI_MethodInfo =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.NPCLoader")!.GetMethod("NPCAI", BindingFlags.Public | BindingFlags.Static)!;

		public static void EngageVoraciousGameFuckery()
		{
			// the following is my attempt to feed the publish button to the Stylist so it can never get misclicked
			// NEVER touch this unless you are 130% certain you know what you're doin'
			/*
			ModLoaderUI_UIModSourceItem_ConstructorHook = new Hook(ModLoaderUI_UIModSourceItem_ConstructorInfo, (orig_UIModSourceItem_Constructor orig, object instance, string mod, object builtMod) => {
				orig(mod, builtMod);
			});

			// feed the Publish button to the Stylist, just in case
			SystemLoader_UpdateUI_Hook = new Hook(SystemLoader_UpdateUI_MethodInfo, (orig_UpdateUI orig, GameTime gameTime) =>
			{
				AntiPublishProtection.EnsurePublishButtonGetsGulped();
				orig(gameTime);
			});
			*/
			// and now, the rest of the detours
			NPCLoader_NPCAI_Hook = new Hook(NPCLoader_NPCAI_MethodInfo, (orig_NPCAI orig, NPC npc) =>
			{
				PredNPC npcAsPred = npc.AsPred(risky: true);
				PreyNPC npcAsPrey = npc.AsFood(risky: true);
				if (npcAsPred is null || npcAsPrey is null)
					orig(npc);
				else
				{
					if (npcAsPrey.IsCurrentlyEaten)
					{
						npc.velocity = Vector2.Zero;
						if (npcAsPrey.CurrentCaptor.HasValue)
						{
							npc.position = npcAsPrey.CurrentCaptor.Value.Predator.position;
							npcAsPrey.PreyAIMethod?.Invoke(npc, npcAsPrey.CurrentCaptor.Value.Predator);
							NPCLoader.PostAI(npc);
						}
					}
					else if (npcAsPred.SpecialPredAIMethod != null)
					{
						if (npcAsPred.SpecialPredAIMethod.Invoke(npc))
							orig(npc);
						else
							NPCLoader.PostAI(npc);
					}
					else
						orig(npc);
				}
			});
			NPCLoader_NPCAI_Hook.Apply();

			On_Main.UpdateAudio_DecideOnNewMusic += (orig, instance) => MainDetours.UpdateAudio_DecideOnNewMusic();
			On_Main.DrawInterface_36_Cursor += (orig) =>
			{
				if (PredStatsMenuMouthUI.MouthState is not (PredStatsMenuMouthState.EatingCursor or PredStatsMenuMouthState.RegurgitatingCursor))
					orig();
			};

			On_NPC.CanBeChasedBy += (orig, npc, attacker, ignoreDontTakeDamage) =>
			{
				if (npc.active)
				{
					PreyNPC npcAsPrey = npc.AsFood(risky: true);
					if (npcAsPrey is not null)
					{
						PreyNPC.UpdateNPCEatenStatus(npc);
						if (npcAsPrey.IsCurrentlyEaten)
							return false;
					}
				}

				return orig(npc, attacker, ignoreDontTakeDamage);
			};
			On_NPC.checkDead += (orig, npc) => NPCDetours.checkDead(npc);
			On_NPC.NPCLoot_DropHeals += (orig, npc, closestPlayer) =>
			{
				if (!npc.AsFood().Digested)
					orig(npc, closestPlayer);
			};
			On_NPC.NPCLoot_DropMoney += (orig, npc, closestPlayer) =>
			{
				if (!npc.AsFood().Digested)
					orig(npc, closestPlayer);
			};
			On_NPC.NPCLoot_DropItems += (orig, npc, closestPlayer) =>
			{
				if (!npc.AsFood().Digested)
					orig(npc, closestPlayer);
			};
			On_NPC.DoDeathEvents_DropBossPotionsAndHearts += NoPotionsOrHeartsIfDigested;
			On_NPC.DoDeathEvents_CelebrateBossDeath += (orig, npc, typeName) => NPCDetours.DoDeathEvents_CelebrateBossDeath(npc, typeName);

			On_Player.KillMe += (orig, player, damageSource, dmg, hitDirection, pvp) => PlayerDetours.KillMe(player, damageSource, dmg, hitDirection, pvp);
			On_Player.ItemCheck_ReleaseCritter += (orig, player, item) => PlayerDetours.ItemCheck_ReleaseCritter(player, item);
			On_Player.ToggleInv += (orig, player) =>
			{
				if (!player.AsPred().InPredStatsMenu || Main.gamePaused)
					orig(player);
			};
		}

		public static void DisengageVoraciousGameFuckery()
		{
			NPCLoader_NPCAI_Hook.Undo();
			NPCLoader_NPCAI_Hook = null;
		}

		private static void NoPotionsOrHeartsIfDigested(On_NPC.orig_DoDeathEvents_DropBossPotionsAndHearts orig, NPC npc, ref string typeName)
		{
			if (!npc.AsFood().Digested)
				orig(npc, ref typeName);
		}
	}
}
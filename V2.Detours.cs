using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.ModLoader;
using V2.Core;
using V2.Core.MainDetours;
using V2.Core.WorldGeneration;
using V2.Items;
using V2.NPCs;
using V2.NPCs.Vanilla.TownNPCs.TravellingMerchant;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.UI.PredStatsMenu;

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

		private delegate void orig_ProjectileAI(Projectile projectile);
		internal static Hook ProjectileLoader_ProjectileAI_Hook;
		private static readonly MethodInfo ProjectileLoader_ProjectileAI_MethodInfo =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.ProjectileLoader")!.GetMethod("ProjectileAI", BindingFlags.Public | BindingFlags.Static)!;

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
				GeneralNPC npcAsV2NPC = npc.AsV2NPC(risky: true);
				PreyNPC npcAsPrey = npc.AsFood(risky: true);
				if (npcAsV2NPC is null || npcAsPrey is null)
					orig(npc);
				else
				{
					if (npc.CurrentCaptor() is not null)
					{
						npc.velocity = Vector2.Zero;
						npc.position = npc.CurrentCaptor().Predator.position;
						npcAsPrey.SpecialPreyAI?.Invoke(npc, npc.CurrentCaptor().Predator);
					}
					else if (npcAsV2NPC.NewAIMethod is not null)
					{
						if (npcAsV2NPC.FirstFrame && npcAsV2NPC.FirstFramePreAIMethod is not null)
						{
							npcAsV2NPC.FirstFrame = false;
							npcAsV2NPC.FirstFramePreAIMethod.Invoke(npc);
						}
						if (npcAsV2NPC.NewAIMethod.Invoke(npc))
							orig(npc);
						else
							NPCLoader.PostAI(npc);
					}
					else
						orig(npc);
				}
			});
			NPCLoader_NPCAI_Hook.Apply();


			ProjectileLoader_ProjectileAI_Hook = new Hook(ProjectileLoader_ProjectileAI_MethodInfo, (orig_ProjectileAI orig, Projectile projectile) =>
			{
				V2Projectile projectileAsV2Projectile = projectile.AsV2Proj(risky: true);
				PreyProjectile projectileAsPrey = projectile.AsFood(risky: true);
				if (projectileAsV2Projectile is null || projectileAsPrey is null)
					orig(projectile);
				else
				{
					PredProjectile.ResetEffects(projectile);
					if (projectile.CurrentCaptor() is not null)
					{
						projectile.timeLeft += 1;
						projectile.velocity = Vector2.Zero;
						projectile.position = projectile.CurrentCaptor().Predator.position;
						projectileAsPrey.SpecialPreyAI?.Invoke(projectile, projectile.CurrentCaptor().Predator);
					}
					else if (projectileAsV2Projectile.NewAIMethod is not null)
					{
						if (projectileAsV2Projectile.NewAIMethod.Invoke(projectile))
							orig(projectile);
						else
							ProjectileLoader.PostAI(projectile);
					}
					else
						orig(projectile);
				}
			});
			ProjectileLoader_ProjectileAI_Hook.Apply();

			On_Chest.SetupTravelShop_GetItem += (On_Chest.orig_SetupTravelShop_GetItem orig, Player playerWithHighestLuck, int[] rarity, ref int it, int minimumRarity)
				=> TravellingMerchant.SetupTravelShop_GetItem(playerWithHighestLuck, rarity, ref it, minimumRarity);

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
					if (npc.CurrentCaptor() is not null)
						return false;
				}

				return orig(npc, attacker, ignoreDontTakeDamage);
			};
			On_NPC.checkDead += (orig, npc) => NPCDetours.CheckDead(npc);
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
			On_Player.ApplyEquipFunctional += (orig, player, item, hideVisual) =>
			{
				if (item.IsAir)
					return;

				if ((item.expertOnly && !Main.expertMode) || (item.masterOnly && !Main.masterMode))
					return;

				if (item.AsAnItem() is not null && item.AsAnItem().AccessoryEffectCode is not null)
					item.AsAnItem().AccessoryEffectCode.Invoke(item, player, hideVisual);
				else
					orig(player, item, hideVisual);
			};
			On_Player.UpdateArmorSets += (orig, player, i) =>
			{
				if (ArmorSetHandler.CheckDefinedArmorSets(player))
					player.ApplyArmorSoundAndDustChanges();
				else
					orig(player, i);
			};
			On_Player.UpdateBuffs += (orig, player, i) => PlayerDetours.Detour_UpdateBuffs(player);
			On_Player.DashMovement += (orig, player) =>
			{
				if (player.CurrentCaptor() is null)
					orig(player);
				else
				{
					player.dashDelay = 60;
					player.dashTime = 0;
				}
			};
			On_Player.ItemCheck_ReleaseCritter += (orig, player, item) => PlayerDetours.ItemCheck_ReleaseCritter(player, item);
			On_Player.ToggleInv += (orig, player) =>
			{
				if (!player.AsPred().InPredStatsMenu || Main.gamePaused)
					orig(player);
			};

			On_DeadMansChestBiome.TurnGoldChestIntoDeadMansChest += (orig, instance, position) => WorldGenDetours.TurnGoldChestIntoDeadMansChest(position);
		}

		public static void DisengageVoraciousGameFuckery()
		{
			if (NPCLoader_NPCAI_Hook is not null)
			{
				NPCLoader_NPCAI_Hook.Undo();
				NPCLoader_NPCAI_Hook = null;
			}
			if (ProjectileLoader_ProjectileAI_Hook is not null)
			{
				ProjectileLoader_ProjectileAI_Hook.Undo();
				ProjectileLoader_ProjectileAI_Hook = null;
			}
		}

		private static void NoPotionsOrHeartsIfDigested(On_NPC.orig_DoDeathEvents_DropBossPotionsAndHearts orig, NPC npc, ref string typeName)
		{
			if (!npc.AsFood().Digested)
				orig(npc, ref typeName);
		}
	}
}
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items;
using V2.NPCs;
using V2.UI;

namespace V2.PlayerHandling
{
	public partial class V2Player : ModPlayer
	{
		public static List<NPC> _hallucinationCandidates = new List<NPC>();

		public bool setBonusActive;
		public bool setBonusShouldBeDisplayed;

		public static void GrantArmorBenefits(Player player, Item armorPiece)
		{
			int type = armorPiece.type;
			player.RefreshInfoAccsFromItemType(armorPiece);
			player.RefreshMechanicalAccsFromItemType(type);
			if (armorPiece.type == ItemID.FlowerBoots || armorPiece.type == ItemID.FairyBoots)
			{
				player.flowerBoots = true;
				if (player.whoAmI == Main.myPlayer)
					player.DoBootsEffect(player.DoBootsEffect_PlaceFlowersOnTile);
			}

			if (armorPiece.type == 5001)
			{
				player.moveSpeed += 0.25f;
				player.moonLordLegs = true;
			}

			player.statDefense += armorPiece.defense;
			player.lifeRegen += armorPiece.lifeRegen;
			if (armorPiece.shieldSlot > 0)
				player.hasRaisableShield = true;

			if (armorPiece.AsV2Item().ArmorEffectCode is not null)
			{
				armorPiece.AsV2Item().ArmorEffectCode.Invoke(armorPiece, player);
				return;
			}

			switch (armorPiece.type)
			{
				case 3797:
					player.maxTurrets++;
					player.manaCost -= 0.1f;
					player.GetDamage(DamageClass.Magic) += 0.1f;
					break;
				case 3798:
					player.GetDamage(DamageClass.Magic) += 0.1f;
					player.GetDamage(DamageClass.Summon) += 0.2f;
					break;
				case 3799:
					player.GetDamage(DamageClass.Summon) += 0.1f;
					player.GetCritChance(DamageClass.Magic) += 20;
					player.moveSpeed += 0.2f;
					break;
				case 3800:
					player.maxTurrets++;
					player.lifeRegen += 4;
					break;
				case 3801:
					player.GetDamage(DamageClass.Melee) += 0.15f;
					player.GetDamage(DamageClass.Summon) += 0.15f;
					break;
				case 3802:
					player.GetDamage(DamageClass.Summon) += 0.15f;
					player.GetCritChance(DamageClass.Melee) += 15;
					player.moveSpeed += 0.15f;
					break;
				case 3806:
					player.maxTurrets++;
					player.GetAttackSpeed(DamageClass.Melee) += 0.2f;
					break;
				case 3807:
					player.GetDamage(DamageClass.Melee) += 0.2f;
					player.GetDamage(DamageClass.Summon) += 0.2f;
					break;
				case 3808:
					player.GetDamage(DamageClass.Summon) += 0.1f;
					player.GetCritChance(DamageClass.Melee) += 15;
					player.moveSpeed += 0.2f;
					break;
				case 3803:
					player.maxTurrets++;
					player.GetCritChance(DamageClass.Ranged) += 10;
					break;
				case 3804:
					player.GetDamage(DamageClass.Ranged) += 0.2f;
					player.GetDamage(DamageClass.Summon) += 0.2f;
					player.huntressAmmoCost90 = true;
					break;
				case 3805:
					player.GetDamage(DamageClass.Summon) += 0.1f;
					player.moveSpeed += 0.2f;
					break;
				case 3871:
					player.maxTurrets += 2;
					player.GetDamage(DamageClass.Melee) += 0.1f;
					player.GetDamage(DamageClass.Summon) += 0.1f;
					break;
				case 3872:
					player.GetDamage(DamageClass.Summon) += 0.3f;
					player.lifeRegen += 8;
					break;
				case 3873:
					player.GetDamage(DamageClass.Summon) += 0.2f;
					player.GetCritChance(DamageClass.Melee) += 20;
					player.moveSpeed += 0.2f;
					break;
				case 3874:
					player.maxTurrets += 2;
					player.GetDamage(DamageClass.Magic) += 0.15f;
					player.GetDamage(DamageClass.Summon) += 0.15f;
					break;
				case 3875:
					player.GetDamage(DamageClass.Summon) += 0.25f;
					player.GetDamage(DamageClass.Magic) += 0.1f;
					player.manaCost -= 0.15f;
					break;
				case 3876:
					player.GetDamage(DamageClass.Summon) += 0.2f;
					player.GetCritChance(DamageClass.Magic) += 25;
					player.moveSpeed += 0.2f;
					break;
				case 3877:
					player.maxTurrets += 2;
					player.GetDamage(DamageClass.Summon) += 0.1f;
					player.GetCritChance(DamageClass.Ranged) += 10;
					break;
				case 3878:
					player.GetDamage(DamageClass.Summon) += 0.25f;
					player.GetDamage(DamageClass.Ranged) += 0.25f;
					player.ammoCost80 = true;
					break;
				case 3879:
					player.GetDamage(DamageClass.Summon) += 0.25f;
					player.GetCritChance(DamageClass.Ranged) += 10;
					player.moveSpeed += 0.2f;
					break;
				case 3880:
					player.maxTurrets += 2;
					player.GetDamage(DamageClass.Summon) += 0.2f;
					player.GetDamage(DamageClass.Melee) += 0.2f;
					break;
				case 3881:
					player.GetAttackSpeed(DamageClass.Melee) += 0.2f;
					player.GetCritChance(DamageClass.Melee) += 5;
					player.GetDamage(DamageClass.Summon) += 0.2f;
					break;
				case 3882:
					player.GetDamage(DamageClass.Summon) += 0.2f;
					player.GetCritChance(DamageClass.Melee) += 20;
					player.moveSpeed += 0.3f;
					break;
			}

			if (armorPiece.type == 5100)
				SpawnHallucination(player, armorPiece);

			if (armorPiece.type == 268)
				player.accDivingHelm = true;

			if (armorPiece.type == 238)
			{
				player.GetDamage(DamageClass.Magic) += 0.05f;
				if (Main.tenthAnniversaryWorld)
					player.maxMinions++;
			}

			if (armorPiece.type == 3770)
				player.slowFall = true;

			if (armorPiece.type == 4404)
				player.canFloatInWater = true;

			if (armorPiece.type == 3776)
			{
				player.GetDamage(DamageClass.Magic) += 0.15f;
				player.GetDamage(DamageClass.Summon) += 0.15f;
			}

			if (armorPiece.type == 3777)
			{
				player.statManaMax2 += 40;
				player.GetDamage(DamageClass.Summon) += 0.1f;
				player.maxMinions++;
			}

			if (armorPiece.type == 3778)
			{
				player.statManaMax2 += 40;
				player.GetDamage(DamageClass.Magic) += 0.1f;
				player.maxMinions++;
			}

			if (armorPiece.type == 3212)
				player.GetArmorPenetration(DamageClass.Generic) += 5;

			if (armorPiece.type == 2277)
			{
				player.GetDamage(DamageClass.Generic) += 0.05f;
				player.GetCritChance(DamageClass.Generic) += 5;
				/*
				player.GetDamage(DamageClass.Magic) += 0.05f;
				player.GetDamage(DamageClass.Melee) += 0.05f;
				player.GetDamage(DamageClass.Ranged) += 0.05f;
				player.GetDamage(DamageClass.Summon) += 0.05f;
				player.GetCritChance(DamageClass.Magic) += 5;
				player.GetCritChance(DamageClass.Ranged) += 5;
				player.GetCritChance(DamageClass.Melee) += 5;
				*/
				player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
				player.moveSpeed += 0.1f;
			}

			if (armorPiece.type == 2279)
			{
				player.GetDamage(DamageClass.Magic) += 0.06f;
				player.GetCritChance(DamageClass.Magic) += 6;
				player.manaCost -= 0.1f;
			}

			if (armorPiece.type == 3109 || armorPiece.type == 4008)
				player.nightVision = true;

			if (armorPiece.type == 256 || armorPiece.type == 257 || armorPiece.type == 258)
			{
				player.GetCritChance(DamageClass.Generic) += 3;
				/*
				player.GetCritChance(DamageClass.Ranged) += 3;
				player.GetCritChance(DamageClass.Melee) += 3;
				player.GetCritChance(DamageClass.Magic) += 3;
				*/
			}

			if (armorPiece.type == 3374)
				player.GetCritChance(DamageClass.Ranged) += 4;

			if (armorPiece.type == 3375)
				player.GetDamage(DamageClass.Ranged) += 0.05f;

			if (armorPiece.type == 3376)
				player.GetCritChance(DamageClass.Ranged) += 4;

			if (armorPiece.type == 151 || armorPiece.type == 959 || armorPiece.type == 152 || armorPiece.type == 153)
				player.GetDamage(DamageClass.Ranged) += 0.05f;

			if (armorPiece.type == 2275)
			{
				player.GetDamage(DamageClass.Magic) += 0.06f;
				player.GetCritChance(DamageClass.Magic) += 6;
			}

			if (armorPiece.type == 123 || armorPiece.type == 124 || armorPiece.type == 125)
				player.GetDamage(DamageClass.Magic) += 0.09f;

			if (armorPiece.type == 228 || armorPiece.type == 960)
			{
				player.statManaMax2 += 40;
				player.GetCritChance(DamageClass.Magic) += 6;
			}

			if (armorPiece.type == 229 || armorPiece.type == 961)
			{
				player.statManaMax2 += 20;
				player.GetDamage(DamageClass.Magic) += 0.06f;
			}

			if (armorPiece.type == 230 || armorPiece.type == 962)
			{
				player.statManaMax2 += 20;
				player.GetCritChance(DamageClass.Magic) += 6;
			}

			if (armorPiece.type == 100 || armorPiece.type == 101 || armorPiece.type == 102)
			{
				player.GetCritChance(DamageClass.Generic) += 5;
				/*
				player.GetCritChance(DamageClass.Magic) += 5;
				player.GetCritChance(DamageClass.Melee) += 5;
				player.GetCritChance(DamageClass.Ranged) += 5;
				*/
			}

			if (armorPiece.type == 956 || armorPiece.type == 957 || armorPiece.type == 958)
			{
				player.GetCritChance(DamageClass.Generic) += 5;
				/*
				player.GetCritChance(DamageClass.Magic) += 5;
				player.GetCritChance(DamageClass.Melee) += 5;
				player.GetCritChance(DamageClass.Ranged) += 5;
				*/
			}

			if (armorPiece.type == 792 || armorPiece.type == 793 || armorPiece.type == 794)
			{
				player.GetDamage(DamageClass.Generic) += 0.03f;
				/*
				player.GetDamage(DamageClass.Melee) += 0.03f;
				player.GetDamage(DamageClass.Ranged) += 0.03f;
				player.GetDamage(DamageClass.Magic) += 0.03f;
				player.GetDamage(DamageClass.Summon) += 0.03f;
				*/
			}

			if (armorPiece.type == 231)
				player.GetCritChance(DamageClass.Melee) += 7;

			if (armorPiece.type == 232)
				player.GetDamage(DamageClass.Melee) += 0.07f;

			if (armorPiece.type == 233)
				player.GetAttackSpeed(DamageClass.Melee) += 0.07f;

			if (armorPiece.type == 371)
			{
				player.GetCritChance(DamageClass.Magic) += 9;
				player.GetDamage(DamageClass.Magic) += 0.1f;
				player.statManaMax2 += 40;
			}

			if (armorPiece.type == 372)
			{
				player.moveSpeed += 0.1f;
				player.GetDamage(DamageClass.Melee) += 0.15f;
			}

			if (armorPiece.type == 373)
			{
				player.GetDamage(DamageClass.Ranged) += 0.1f;
				player.GetCritChance(DamageClass.Ranged) += 10;
			}

			if (armorPiece.type == 374)
			{
				player.GetCritChance(DamageClass.Generic) += 5;
				/*
				player.GetCritChance(DamageClass.Magic) += 5;
				player.GetCritChance(DamageClass.Melee) += 5;
				player.GetCritChance(DamageClass.Ranged) += 5;
				*/
			}

			if (armorPiece.type == 375)
			{
				player.GetDamage(DamageClass.Generic) += 0.03f;
				/*
				player.GetDamage(DamageClass.Ranged) += 0.03f;
				player.GetDamage(DamageClass.Melee) += 0.03f;
				player.GetDamage(DamageClass.Magic) += 0.03f;
				player.GetDamage(DamageClass.Summon) += 0.03f;
				*/
				player.moveSpeed += 0.1f;
			}

			if (armorPiece.type == 376)
			{
				player.GetDamage(DamageClass.Magic) += 0.15f;
				player.statManaMax2 += 60;
			}

			if (armorPiece.type == 377)
			{
				player.GetCritChance(DamageClass.Melee) += 8;
				player.GetDamage(DamageClass.Melee) += 0.1f;
			}

			if (armorPiece.type == 378)
			{
				player.GetDamage(DamageClass.Ranged) += 0.12f;
				player.GetCritChance(DamageClass.Ranged) += 7;
			}

			if (armorPiece.type == 379)
			{
				player.GetDamage(DamageClass.Generic) += 0.07f;
				/*
				player.GetDamage(DamageClass.Ranged) += 0.07f;
				player.GetDamage(DamageClass.Melee) += 0.07f;
				player.GetDamage(DamageClass.Magic) += 0.07f;
				player.GetDamage(DamageClass.Summon) += 0.07f;
				*/
			}

			if (armorPiece.type == 380)
			{
				player.GetCritChance(DamageClass.Generic) += 10;
				/*
				player.GetCritChance(DamageClass.Magic) += 10;
				player.GetCritChance(DamageClass.Melee) += 10;
				player.GetCritChance(DamageClass.Ranged) += 10;
				*/
			}

			if (armorPiece.type >= 2367 && armorPiece.type <= 2369)
				player.fishingSkill += 5;

			if (armorPiece.type == 400)
			{
				player.GetDamage(DamageClass.Magic) += 0.12f;
				player.GetCritChance(DamageClass.Magic) += 12;
				player.statManaMax2 += 80;
			}

			if (armorPiece.type == 401)
			{
				player.GetCritChance(DamageClass.Melee) += 7;
				player.GetDamage(DamageClass.Melee) += 0.14f;
			}

			if (armorPiece.type == 402)
			{
				player.GetDamage(DamageClass.Ranged) += 0.14f;
				player.GetCritChance(DamageClass.Ranged) += 10;
			}

			if (armorPiece.type == 403)
			{
				player.GetDamage(DamageClass.Generic) += 0.08f;
				/*
				player.GetDamage(DamageClass.Ranged) += 0.08f;
				player.GetDamage(DamageClass.Melee) += 0.08f;
				player.GetDamage(DamageClass.Magic) += 0.08f;
				player.GetDamage(DamageClass.Summon) += 0.08f;
				*/
			}

			if (armorPiece.type == 404)
			{
				player.GetCritChance(DamageClass.Generic) += 7;
				/*
				player.GetCritChance(DamageClass.Magic) += 7;
				player.GetCritChance(DamageClass.Melee) += 7;
				player.GetCritChance(DamageClass.Ranged) += 7;
				*/
				player.moveSpeed += 0.05f;
			}

			if (armorPiece.type == 1205)
			{
				player.GetDamage(DamageClass.Melee) += 0.12f;
				player.GetAttackSpeed(DamageClass.Melee) += 0.12f;
			}

			if (armorPiece.type == 1206)
			{
				player.GetDamage(DamageClass.Ranged) += 0.09f;
				player.GetCritChance(DamageClass.Ranged) += 9;
			}

			if (armorPiece.type == 1207)
			{
				player.GetDamage(DamageClass.Magic) += 0.09f;
				player.GetCritChance(DamageClass.Magic) += 9;
				player.statManaMax2 += 60;
			}

			if (armorPiece.type == 1208)
			{
				player.GetDamage(DamageClass.Generic) += 0.03f;
				player.GetCritChance(DamageClass.Generic) += 2;
				/*
				player.GetDamage(DamageClass.Melee) += 0.03f;
				player.GetDamage(DamageClass.Ranged) += 0.03f;
				player.GetDamage(DamageClass.Magic) += 0.03f;
				player.GetDamage(DamageClass.Summon) += 0.03f;
				player.GetCritChance(DamageClass.Magic) += 2;
				player.GetCritChance(DamageClass.Melee) += 2;
				player.GetCritChance(DamageClass.Ranged) += 2;
				*/
			}

			if (armorPiece.type == 1209)
			{
				player.GetDamage(DamageClass.Generic) += 0.02f;
				player.GetCritChance(DamageClass.Generic) += 1;
				/*
				player.GetDamage(DamageClass.Melee) += 0.02f;
				player.GetDamage(DamageClass.Ranged) += 0.02f;
				player.GetDamage(DamageClass.Magic) += 0.02f;
				player.GetDamage(DamageClass.Summon) += 0.02f;
				player.GetCritChance(DamageClass.Magic)++;
				player.GetCritChance(DamageClass.Melee)++;
				player.GetCritChance(DamageClass.Ranged)++;
				*/
			}

			if (armorPiece.type == 1210)
			{
				player.GetDamage(DamageClass.Melee) += 0.11f;
				player.GetAttackSpeed(DamageClass.Melee) += 0.11f;
				player.moveSpeed += 0.07f;
			}

			if (armorPiece.type == 1211)
			{
				player.GetCritChance(DamageClass.Ranged) += 15;
				player.moveSpeed += 0.08f;
			}

			if (armorPiece.type == 1212)
			{
				player.GetCritChance(DamageClass.Magic) += 18;
				player.statManaMax2 += 80;
			}

			if (armorPiece.type == 1213)
			{
				player.GetCritChance(DamageClass.Generic) += 6;
				/*
				player.GetCritChance(DamageClass.Magic) += 6;
				player.GetCritChance(DamageClass.Melee) += 6;
				player.GetCritChance(DamageClass.Ranged) += 6;
				*/
			}

			if (armorPiece.type == 1214)
			{
				player.moveSpeed += 0.11f;
				player.GetDamage(DamageClass.Generic) += 0.08f;
				/*
				player.GetDamage(DamageClass.Melee) += 0.08f;
				player.GetDamage(DamageClass.Ranged) += 0.08f;
				player.GetDamage(DamageClass.Magic) += 0.08f;
				player.GetDamage(DamageClass.Summon) += 0.08f;
				*/
			}

			if (armorPiece.type == 1215)
			{
				player.GetDamage(DamageClass.Melee) += 0.09f;
				player.GetCritChance(DamageClass.Melee) += 9;
				player.GetAttackSpeed(DamageClass.Melee) += 0.09f;
			}

			if (armorPiece.type == 1216)
			{
				player.GetDamage(DamageClass.Ranged) += 0.16f;
				player.GetCritChance(DamageClass.Ranged) += 7;
			}

			if (armorPiece.type == 1217)
			{
				player.GetDamage(DamageClass.Magic) += 0.16f;
				player.GetCritChance(DamageClass.Magic) += 7;
				player.statManaMax2 += 100;
			}

			if (armorPiece.type == 1218)
			{
				player.GetDamage(DamageClass.Generic) += 0.04f;
				player.GetCritChance(DamageClass.Generic) += 3;
				/*
				player.GetDamage(DamageClass.Melee) += 0.04f;
				player.GetDamage(DamageClass.Ranged) += 0.04f;
				player.GetDamage(DamageClass.Magic) += 0.04f;
				player.GetDamage(DamageClass.Summon) += 0.04f;
				player.GetCritChance(DamageClass.Magic) += 3;
				player.GetCritChance(DamageClass.Melee) += 3;
				player.GetCritChance(DamageClass.Ranged) += 3;
				*/
			}

			if (armorPiece.type == 1219)
			{
				player.GetDamage(DamageClass.Generic) += 0.03f;
				player.GetCritChance(DamageClass.Generic) += 3;
				/*
				player.GetDamage(DamageClass.Melee) += 0.03f;
				player.GetDamage(DamageClass.Ranged) += 0.03f;
				player.GetDamage(DamageClass.Magic) += 0.03f;
				player.GetDamage(DamageClass.Summon) += 0.03f;
				player.GetCritChance(DamageClass.Magic) += 3;
				player.GetCritChance(DamageClass.Melee) += 3;
				player.GetCritChance(DamageClass.Ranged) += 3;
				*/
				player.moveSpeed += 0.06f;
			}

			if (armorPiece.type == 558 || armorPiece.type == 4898)
			{
				player.GetDamage(DamageClass.Magic) += 0.12f;
				player.GetCritChance(DamageClass.Magic) += 12;
				player.statManaMax2 += 100;
			}

			if (armorPiece.type == 559 || armorPiece.type == 4896)
			{
				player.GetCritChance(DamageClass.Melee) += 10;
				player.GetDamage(DamageClass.Melee) += 0.1f;
				player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
			}

			if (armorPiece.type == 553 || armorPiece.type == 4897)
			{
				player.GetDamage(DamageClass.Ranged) += 0.15f;
				player.GetCritChance(DamageClass.Ranged) += 8;
			}

			if (armorPiece.type == 4873 || armorPiece.type == 4899)
			{
				player.GetDamage(DamageClass.Summon) += 0.1f;
				player.maxMinions++;
			}

			if (armorPiece.type == 551 || armorPiece.type == 4900)
			{
				player.GetCritChance(DamageClass.Generic) += 7;
				/*
				player.GetCritChance(DamageClass.Magic) += 7;
				player.GetCritChance(DamageClass.Melee) += 7;
				player.GetCritChance(DamageClass.Ranged) += 7;
				*/
			}

			if (armorPiece.type == 552 || armorPiece.type == 4901)
			{
				player.GetDamage(DamageClass.Generic) += 0.07f;
				/*
				player.GetDamage(DamageClass.Ranged) += 0.07f;
				player.GetDamage(DamageClass.Melee) += 0.07f;
				player.GetDamage(DamageClass.Magic) += 0.07f;
				player.GetDamage(DamageClass.Summon) += 0.07f;
				*/
				player.moveSpeed += 0.08f;
			}

			if (armorPiece.type == 4982)
			{
				player.GetCritChance(DamageClass.Generic) += 5;
				/*
				player.GetCritChance(DamageClass.Ranged) += 5;
				player.GetCritChance(DamageClass.Melee) += 5;
				player.GetCritChance(DamageClass.Magic) += 5;
				*/
				player.manaCost -= 0.1f;
			}

			if (armorPiece.type == 4983)
			{
				player.GetDamage(DamageClass.Generic) += 0.05f;
				/*
				player.GetDamage(DamageClass.Ranged) += 0.05f;
				player.GetDamage(DamageClass.Melee) += 0.05f;
				player.GetDamage(DamageClass.Magic) += 0.05f;
				player.GetDamage(DamageClass.Summon) += 0.05f;
				*/
				player.huntressAmmoCost90 = true;
			}

			if (armorPiece.type == 4984)
			{
				player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
				player.moveSpeed += 0.2f;
			}

			if (armorPiece.type == 1001)
			{
				player.GetDamage(DamageClass.Melee) += 0.16f;
				player.GetCritChance(DamageClass.Melee) += 6;
			}

			if (armorPiece.type == 1002)
			{
				player.GetDamage(DamageClass.Ranged) += 0.16f;
				player.chloroAmmoCost80 = true;
			}

			if (armorPiece.type == 1003)
			{
				player.statManaMax2 += 80;
				player.manaCost -= 0.17f;
				player.GetDamage(DamageClass.Magic) += 0.16f;
			}

			if (armorPiece.type == 1004)
			{
				player.GetDamage(DamageClass.Generic) += 0.05f;
				player.GetCritChance(DamageClass.Generic) += 7;
				/*
				player.GetDamage(DamageClass.Melee) += 0.05f;
				player.GetDamage(DamageClass.Magic) += 0.05f;
				player.GetDamage(DamageClass.Ranged) += 0.05f;
				player.GetDamage(DamageClass.Summon) += 0.05f;
				player.GetCritChance(DamageClass.Magic) += 7;
				player.GetCritChance(DamageClass.Melee) += 7;
				player.GetCritChance(DamageClass.Ranged) += 7;
				*/
			}

			if (armorPiece.type == 1005)
			{
				player.GetCritChance(DamageClass.Generic) += 8;
				/*
				player.GetCritChance(DamageClass.Magic) += 8;
				player.GetCritChance(DamageClass.Melee) += 8;
				player.GetCritChance(DamageClass.Ranged) += 8;
				*/
				player.moveSpeed += 0.05f;
			}

			if (armorPiece.type == 2189)
			{
				player.statManaMax2 += 60;
				player.manaCost -= 0.13f;
				player.GetDamage(DamageClass.Magic) += 0.1f;
				player.GetCritChance(DamageClass.Magic) += 10;
			}

			if (armorPiece.type == 1504)
			{
				player.GetDamage(DamageClass.Magic) += 0.07f;
				player.GetCritChance(DamageClass.Magic) += 7;
			}

			if (armorPiece.type == 1505)
			{
				player.GetDamage(DamageClass.Magic) += 0.08f;
				player.moveSpeed += 0.08f;
			}

			if (armorPiece.type == 1546)
			{
				player.GetCritChance(DamageClass.Ranged) += 5;
				player.arrowDamage *= 1.15f;
			}

			if (armorPiece.type == 1547)
			{
				player.GetCritChance(DamageClass.Ranged) += 5;
				player.bulletDamage *= 1.15f;
			}

			if (armorPiece.type == 1548)
			{
				player.GetCritChance(DamageClass.Ranged) += 5;
				player.specialistDamage *= 1.15f; // rocketDamage renamed.
			}

			if (armorPiece.type == 1549)
			{
				player.GetCritChance(DamageClass.Ranged) += 13;
				player.GetDamage(DamageClass.Ranged) += 0.13f;
				player.ammoCost80 = true;
			}

			if (armorPiece.type == 1550)
			{
				player.GetCritChance(DamageClass.Ranged) += 7;
				player.moveSpeed += 0.12f;
			}

			if (armorPiece.type == 1282)
			{
				player.statManaMax2 += 20;
				player.manaCost -= 0.05f;
			}

			if (armorPiece.type == 1283)
			{
				player.statManaMax2 += 40;
				player.manaCost -= 0.07f;
			}

			if (armorPiece.type == 1284)
			{
				player.statManaMax2 += 40;
				player.manaCost -= 0.09f;
			}

			if (armorPiece.type == 1285)
			{
				player.statManaMax2 += 60;
				player.manaCost -= 0.11f;
			}

			if (armorPiece.type == 1286 || armorPiece.type == 4256)
			{
				player.statManaMax2 += 60;
				player.manaCost -= 0.13f;
			}

			if (armorPiece.type == 1287)
			{
				player.statManaMax2 += 80;
				player.manaCost -= 0.15f;
			}

			if (armorPiece.type == 1316 || armorPiece.type == 1317 || armorPiece.type == 1318)
				player.aggro += 250;

			if (armorPiece.type == 1316)
				player.GetDamage(DamageClass.Melee) += 0.06f;

			if (armorPiece.type == 1317)
			{
				player.GetDamage(DamageClass.Melee) += 0.08f;
				player.GetCritChance(DamageClass.Melee) += 8;
			}

			if (armorPiece.type == 1318)
				player.GetCritChance(DamageClass.Melee) += 4;

			if (armorPiece.type == 2199 || armorPiece.type == 2202)
				player.aggro += 250;

			if (armorPiece.type == 2201)
				player.aggro += 400;

			if (armorPiece.type == 2199)
				player.GetDamage(DamageClass.Melee) += 0.06f;

			if (armorPiece.type == 2200)
			{
				player.GetDamage(DamageClass.Melee) += 0.08f;
				player.GetCritChance(DamageClass.Melee) += 8;
				player.GetAttackSpeed(DamageClass.Melee) += 0.06f;
				player.moveSpeed += 0.06f;
			}

			if (armorPiece.type == 2201)
			{
				player.GetDamage(DamageClass.Melee) += 0.05f;
				player.GetCritChance(DamageClass.Melee) += 5;
			}

			if (armorPiece.type == 2202)
			{
				player.GetAttackSpeed(DamageClass.Melee) += 0.06f;
				player.moveSpeed += 0.06f;
			}

			if (armorPiece.type == 684)
			{
				player.GetDamage(DamageClass.Ranged) += 0.16f;
				player.GetDamage(DamageClass.Melee) += 0.16f;
			}

			if (armorPiece.type == 685)
			{
				player.GetCritChance(DamageClass.Melee) += 11;
				player.GetCritChance(DamageClass.Ranged) += 11;
			}

			if (armorPiece.type == 686)
			{
				player.moveSpeed += 0.08f;
				player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
			}

			if (armorPiece.type == 5068)
			{
				player.maxMinions++;
				player.GetDamage(DamageClass.Summon) += 0.05f;
			}

			if (armorPiece.type == 2361)
			{
				player.maxMinions++;
				player.GetDamage(DamageClass.Summon) += 0.04f;
			}

			if (armorPiece.type == 2362)
			{
				player.maxMinions++;
				player.GetDamage(DamageClass.Summon) += 0.04f;
			}

			if (armorPiece.type == 2363)
				player.GetDamage(DamageClass.Summon) += 0.05f;

			if (armorPiece.type == 3266)
				player.GetDamage(DamageClass.Summon) += 0.08f;

			if (armorPiece.type == 3267)
				player.maxMinions++;

			if (armorPiece.type == 3268)
				player.GetDamage(DamageClass.Summon) += 0.08f;

			if (armorPiece.type == 410)
				player.pickSpeed -= 0.1f;

			if (armorPiece.type == 411)
				player.pickSpeed -= 0.1f;

			if (armorPiece.type >= 1158 && armorPiece.type <= 1161)
				player.maxMinions++;

			if (armorPiece.type == 1159)
				player.whipRangeMultiplier += 0.1f;

			if (armorPiece.type >= 1159 && armorPiece.type <= 1161)
				player.GetDamage(DamageClass.Summon) += 0.1f;

			if (armorPiece.type >= 2370 && armorPiece.type <= 2371)
			{
				player.GetDamage(DamageClass.Summon) += 0.05f;
				player.maxMinions++;
			}

			if (armorPiece.type == 2372)
			{
				player.GetDamage(DamageClass.Summon) += 0.06f;
				player.maxMinions++;
			}

			if (armorPiece.type == 3381)
			{
				player.maxMinions++;
				player.maxTurrets++;
				player.GetDamage(DamageClass.Summon) += 0.22f;
			}

			if (armorPiece.type == 3382 || armorPiece.type == 3383)
			{
				player.maxMinions += 2;
				player.whipRangeMultiplier += 0.15f;
				player.GetDamage(DamageClass.Summon) += 0.22f;
			}

			if (armorPiece.type == 2763)
			{
				player.aggro += 300;
				player.GetCritChance(DamageClass.Melee) += 26;
				player.lifeRegen += 2;
			}

			if (armorPiece.type == 2764)
			{
				player.aggro += 300;
				player.GetDamage(DamageClass.Melee) += 0.29f;
				player.lifeRegen += 2;
			}

			if (armorPiece.type == 2765)
			{
				player.aggro += 300;
				player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
				player.moveSpeed += 0.15f;
				player.lifeRegen += 2;
			}

			if (armorPiece.type == 2757)
			{
				player.GetCritChance(DamageClass.Ranged) += 7;
				player.GetDamage(DamageClass.Ranged) += 0.16f;
			}

			if (armorPiece.type == 2758)
			{
				player.ammoCost75 = true;
				player.GetCritChance(DamageClass.Ranged) += 12;
				player.GetDamage(DamageClass.Ranged) += 0.12f;
			}

			if (armorPiece.type == 2759)
			{
				player.GetCritChance(DamageClass.Ranged) += 8;
				player.GetDamage(DamageClass.Ranged) += 0.08f;
				player.moveSpeed += 0.1f;
			}

			if (armorPiece.type == 2760)
			{
				player.statManaMax2 += 60;
				player.manaCost -= 0.15f;
				player.GetCritChance(DamageClass.Magic) += 7;
				player.GetDamage(DamageClass.Magic) += 0.07f;
			}

			if (armorPiece.type == 2761)
			{
				player.GetDamage(DamageClass.Magic) += 0.09f;
				player.GetCritChance(DamageClass.Magic) += 9;
			}

			if (armorPiece.type == 2762)
			{
				player.moveSpeed += 0.1f;
				player.GetDamage(DamageClass.Magic) += 0.1f;
			}

			if (armorPiece.type == 1832)
			{
				player.maxMinions++;
				player.GetDamage(DamageClass.Summon) += 0.11f;
			}

			if (armorPiece.type == 1833)
			{
				player.maxMinions += 2;
				player.GetDamage(DamageClass.Summon) += 0.11f;
			}

			// Extra patch context.
			if (armorPiece.type == 1834)
			{
				player.moveSpeed += 0.2f;
				player.maxMinions++;
				player.GetDamage(DamageClass.Summon) += 0.11f;
			}

			// Lifted from NPC.SpawnNPC for NewNPC(..., 45), which is NPCID.Tim. See usage of the flag
			if (armorPiece.type == 4256 || (armorPiece.type >= 1282 && armorPiece.type <= 1287))
				player.hasGemRobe = true;

			ItemLoader.UpdateEquip(armorPiece, player);
		}
		private static void SpawnHallucination(Player player, Item item)
		{
			if (player.whoAmI != Main.myPlayer)
				return;

			player.insanityShadowCooldown = Utils.Clamp(player.insanityShadowCooldown - 1, 0, 100);
			if (player.insanityShadowCooldown > 0)
				return;

			player.insanityShadowCooldown = Main.rand.Next(20, 101);
			float num = 500f;
			int damage = 18;
			_hallucinationCandidates.Clear();
			for (int i = 0; i < 200; i++)
			{
				NPC nPC = Main.npc[i];
				if (nPC.CanBeChasedBy(player) && !(player.Distance(nPC.Center) > num) && Collision.CanHitLine(player.position, player.width, player.height, nPC.position, nPC.width, nPC.height))
					_hallucinationCandidates.Add(nPC);
			}

			if (_hallucinationCandidates.Count != 0)
			{
				Projectile.RandomizeInsanityShadowFor(Main.rand.NextFromCollection(_hallucinationCandidates), isHostile: false, out Vector2 spawnposition, out Vector2 spawnvelocity, out float ai, out float ai2);
				Projectile.NewProjectile(new EntitySource_ItemUse(player, item), spawnposition, spawnvelocity, ProjectileID.InsanityShadowFriendly, damage, 0f, player.whoAmI, ai, ai2);
			}
		}

		public static void Detour_UpdateArmorSets(Player player)
		{
			player.setBonus = "";
			if (ArmorSetHandler.CheckDefinedArmorSets(player))
			{
				player.ApplyArmorSoundAndDustChanges();
				return;
			}

			switch (player.armor[0].type, player.armor[1].type, player.armor[2].type)
			{
				case (ItemID.BorealWoodHelmet, ItemID.BorealWoodBreastplate, ItemID.BorealWoodGreaves):
				case (ItemID.PalmWoodHelmet, ItemID.PalmWoodBreastplate, ItemID.PalmWoodGreaves):
				case (ItemID.RichMahoganyHelmet, ItemID.RichMahoganyBreastplate, ItemID.RichMahoganyGreaves):
				case (ItemID.EbonwoodHelmet, ItemID.EbonwoodBreastplate, ItemID.EbonwoodGreaves):
				case (ItemID.ShadewoodHelmet, ItemID.ShadewoodBreastplate, ItemID.ShadewoodGreaves):
				case (ItemID.PearlwoodHelmet, ItemID.PearlwoodBreastplate, ItemID.PearlwoodGreaves):
					player.setBonus = Language.GetTextValue("ArmorSetBonus.Wood");
					player.statDefense++;
					break;
				case (ItemID.AshWoodHelmet, ItemID.AshWoodBreastplate, ItemID.AshWoodGreaves):
					player.setBonus = Language.GetTextValue("ArmorSetBonus.AshWood");
					player.ashWoodBonus = true;
					break;
				case (ItemID.CopperHelmet, ItemID.CopperChainmail, ItemID.CopperGreaves):
				case (ItemID.TinHelmet, ItemID.TinChainmail, ItemID.TinGreaves):
				case (ItemID.IronHelmet or ItemID.AncientIronHelmet, ItemID.IronChainmail, ItemID.IronGreaves):
					player.setBonus = Language.GetTextValue("ArmorSetBonus.MetalTier1");
					player.statDefense += 2;
					break;
				case (ItemID.ShroomiteHelmet or ItemID.ShroomiteMask or ItemID.ShroomiteHeadgear, ItemID.ShroomiteBreastplate, ItemID.ShroomiteLeggings):
					player.setBonus = Language.GetTextValue("ArmorSetBonus.Shroomite");
					player.shroomiteStealth = true;
					break;
			}

			if ((player.head == 3 && player.body == 3 && player.legs == 3) || ((player.head == 73 || player.head == 4) && player.body == 4 && player.legs == 4) || (player.head == 48 && player.body == 29 && player.legs == 28) || (player.head == 49 && player.body == 30 && player.legs == 29))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.MetalTier2");
				player.statDefense += 3;
			}

			if (player.head == 50 && player.body == 31 && player.legs == 30)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Platinum");
				player.statDefense += 4;
			}

			if (player.head == 112 && player.body == 75 && player.legs == 64)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Pumpkin");
				player.GetDamage(DamageClass.Generic) += 0.1f;
			}

			if (player.head == 180 && player.body == 182 && player.legs == 122)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Gladiator");
				player.noKnockback = true;
			}

			if (player.head == 22 && player.body == 14 && player.legs == 14)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Ninja");
				player.moveSpeed += 0.2f;
			}

			if (player.head == 188 && player.body == 189 && player.legs == 129)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Fossil");
				player.ammoCost80 = true;
			}

			if ((player.head == 75 || player.head == 7) && player.body == 7 && player.legs == 7)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Bone");
				player.GetCritChance(DamageClass.Ranged) += 10;
			}

			if (player.head == 157 && player.body == 105 && player.legs == 98)
			{
				int num = 0;
				player.setBonus = Language.GetTextValue("ArmorSetBonus.BeetleDamage");
				player.beetleOffense = true;
				player.beetleCounter -= 3f;
				player.beetleCounter -= player.beetleCountdown / 10;
				player.beetleCountdown++;
				if (player.beetleCounter < 0f)
					player.beetleCounter = 0f;

				int num2 = 400;
				int num3 = 1200;
				int num4 = 4600;
				if (player.beetleCounter > (float)(num2 + num3 + num4 + num3))
					player.beetleCounter = num2 + num3 + num4 + num3;

				if (player.beetleCounter > (float)(num2 + num3 + num4))
				{
					player.AddBuff(100, 5, quiet: false);
					num = 3;
				}
				else if (player.beetleCounter > (float)(num2 + num3))
				{
					player.AddBuff(99, 5, quiet: false);
					num = 2;
				}
				else if (player.beetleCounter > (float)num2)
				{
					player.AddBuff(98, 5, quiet: false);
					num = 1;
				}

				if (num < player.beetleOrbs)
					player.beetleCountdown = 0;
				else if (num > player.beetleOrbs)
					player.beetleCounter += 200f;

				if (num != player.beetleOrbs && player.beetleOrbs > 0)
				{
					for (int j = 0; j < Player.MaxBuffs; j++)
					{
						if (player.buffType[j] >= 98 && player.buffType[j] <= 100 && player.buffType[j] != 97 + num)
							player.DelBuff(j);
					}
				}
			}
			else if (player.head == 157 && player.body == 106 && player.legs == 98)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.BeetleDefense");
				player.beetleDefense = true;
				player.beetleCounter += 1f;
				int num5 = 180;
				if (player.beetleCounter >= (float)num5)
				{
					if (player.beetleOrbs > 0 && player.beetleOrbs < 3)
					{
						for (int k = 0; k < Player.MaxBuffs; k++)
						{
							if (player.buffType[k] >= 95 && player.buffType[k] <= 96)
								player.DelBuff(k);
						}
					}

					if (player.beetleOrbs < 3)
					{
						player.AddBuff(95 + player.beetleOrbs, 5, quiet: false);
						player.beetleCounter = 0f;
					}
					else
					{
						player.beetleCounter = num5;
					}
				}
			}

			if (!player.beetleDefense && !player.beetleOffense)
			{
				player.beetleCounter = 0f;
			}
			else
			{
				player.beetleFrameCounter++;
				if (player.beetleFrameCounter >= 1)
				{
					player.beetleFrameCounter = 0;
					player.beetleFrame++;
					if (player.beetleFrame > 2)
						player.beetleFrame = 0;
				}

				for (int l = player.beetleOrbs; l < 3; l++)
				{
					player.beetlePos[l].X = 0f;
					player.beetlePos[l].Y = 0f;
				}

				for (int m = 0; m < player.beetleOrbs; m++)
				{
					player.beetlePos[m] += player.beetleVel[m];
					player.beetleVel[m].X += (float)Main.rand.Next(-100, 101) * 0.005f;
					player.beetleVel[m].Y += (float)Main.rand.Next(-100, 101) * 0.005f;
					float x = player.beetlePos[m].X;
					float y = player.beetlePos[m].Y;
					float num6 = (float)Math.Sqrt(x * x + y * y);
					if (num6 > 100f)
					{
						num6 = 20f / num6;
						x *= 0f - num6;
						y *= 0f - num6;
						int num7 = 10;
						player.beetleVel[m].X = (player.beetleVel[m].X * (float)(num7 - 1) + x) / (float)num7;
						player.beetleVel[m].Y = (player.beetleVel[m].Y * (float)(num7 - 1) + y) / (float)num7;
					}
					else if (num6 > 30f)
					{
						num6 = 10f / num6;
						x *= 0f - num6;
						y *= 0f - num6;
						int num8 = 20;
						player.beetleVel[m].X = (player.beetleVel[m].X * (float)(num8 - 1) + x) / (float)num8;
						player.beetleVel[m].Y = (player.beetleVel[m].Y * (float)(num8 - 1) + y) / (float)num8;
					}

					x = player.beetleVel[m].X;
					y = player.beetleVel[m].Y;
					num6 = (float)Math.Sqrt(x * x + y * y);
					if (num6 > 2f)
						player.beetleVel[m] *= 0.9f;

					player.beetlePos[m] -= player.velocity * 0.25f;
				}
			}

			if (player.head == 14 && ((player.body >= 58 && player.body <= 63) || player.body == 167 || player.body == 213))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Wizard");
				player.GetCritChance(DamageClass.Magic) += 10;
			}

			if (player.head == 159 && ((player.body >= 58 && player.body <= 63) || player.body == 167 || player.body == 213))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.MagicHat");
				player.statManaMax2 += 60;
			}

			if ((player.head == 5 || player.head == 74) && (player.body == 5 || player.body == 48) && (player.legs == 5 || player.legs == 44))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.ShadowScale");
				player.shadowArmor = true;
			}

			if (player.head == 57 && player.body == 37 && player.legs == 35)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Crimson");
				player.crimsonRegen = true;
			}

			if (player.head == 101 && player.body == 66 && player.legs == 55)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.SpectreHealing");
				player.ghostHeal = true;
				player.GetDamage(DamageClass.Magic) -= 0.4f;
			}

			if (player.head == 156 && player.body == 66 && player.legs == 55)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.SpectreDamage");
				player.ghostHurt = true;
			}

			if (player.head == 6 && player.body == 6 && player.legs == 6)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Meteor");
				player.spaceGun = true;
			}

			if (player.head == 46 && player.body == 27 && player.legs == 26)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Frost");
				player.frostBurn = true;
				player.GetDamage(DamageClass.Melee) += 0.1f;
				player.GetDamage(DamageClass.Ranged) += 0.1f;
			}

			if ((player.head == 76 || player.head == 8) && (player.body == 49 || player.body == 8) && (player.legs == 45 || player.legs == 8))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Jungle");
				player.manaCost -= 0.16f;
			}

			if (player.head == 9 && player.body == 9 && player.legs == 9)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Molten");
				player.GetDamage(DamageClass.Melee) += 0.1f;
				player.buffImmune[24] = true;
			}

			if ((player.head == 58 || player.head == 77) && (player.body == 38 || player.body == 50) && (player.legs == 36 || player.legs == 46))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Snow");
				player.buffImmune[46] = true;
				player.buffImmune[47] = true;
			}

			if (player.head == 11 && player.body == 20 && player.legs == 19)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Mining");
				player.pickSpeed -= 0.1f;
			}

			if (player.head == 216 && player.body == 20 && player.legs == 19)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Mining");
				player.pickSpeed -= 0.1f;
			}

			if (player.head == 78 && player.body == 51 && player.legs == 47)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.ChlorophyteMelee");
				player.AddBuff(60, 18000);
				player.endurance += 0.05f;
			}
			else if ((player.head == 80 || player.head == 79) && player.body == 51 && player.legs == 47)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Chlorophyte");
				player.AddBuff(60, 18000);
			}
			else if (player.crystalLeaf)
			{
				for (int n = 0; n < Player.MaxBuffs; n++)
				{
					if (player.buffType[n] == 60)
						player.DelBuff(n);
				}
			}

			if (player.head == 161 && player.body == 169 && player.legs == 104)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Angler");
				player.anglerSetSpawnReduction = true;
			}

			if (player.head == 70 && player.body == 46 && player.legs == 42)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Cactus");
				player.cactusThorns = true;
			}

			if (player.head == 99 && player.body == 65 && player.legs == 54)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Turtle");
				player.endurance += 0.15f;
				player.thorns = 1f;
				player.turtleThorns = true;
			}

			if (player.body == 17 && player.legs == 16)
			{
				if (player.head == 29)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.CobaltCaster");
					player.manaCost -= 0.14f;
				}
				else if (player.head == 30)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.CobaltMelee");
					player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
				}
				else if (player.head == 31)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.CobaltRanged");
					player.ammoCost80 = true;
				}
			}

			if (player.body == 18 && player.legs == 17)
			{
				if (player.head == 32)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.MythrilCaster");
					player.manaCost -= 0.17f;
				}
				else if (player.head == 33)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.MythrilMelee");
					player.GetCritChance(DamageClass.Melee) += 10;
				}
				else if (player.head == 34)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.MythrilRanged");
					player.ammoCost80 = true;
				}
			}

			if (player.body == 19 && player.legs == 18)
			{
				if (player.head == 35)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.AdamantiteCaster");
					player.manaCost -= 0.19f;
				}
				else if (player.head == 36)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.AdamantiteMelee");
					player.GetAttackSpeed(DamageClass.Melee) += 0.2f;
					player.moveSpeed += 0.2f;
				}
				else if (player.head == 37)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.AdamantiteRanged");
					player.ammoCost75 = true;
				}
			}

			if (player.body == 54 && player.legs == 49 && (player.head == 83 || player.head == 84 || player.head == 85))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Palladium");
				player.onHitRegen = true;
			}

			if (player.body == 55 && player.legs == 50 && (player.head == 86 || player.head == 87 || player.head == 88))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Orichalcum");
				player.onHitPetal = true;
			}

			if (player.body == 56 && player.legs == 51)
			{
				bool flag = false;
				if (player.head == 91)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.Titanium");
					flag = true;
				}
				else if (player.head == 89)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.Titanium");
					flag = true;
				}
				else if (player.head == 90)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.Titanium");
					flag = true;
				}

				if (flag)
					player.onHitTitaniumStorm = true;
			}

			if ((player.body == 24 || player.body == 229) && (player.legs == 23 || player.legs == 212) && (player.head == 42 || player.head == 41 || player.head == 43 || player.head == 254 || player.head == 257 || player.head == 256 || player.head == 255 || player.head == 258))
			{
				if (player.head == 254 || player.head == 258)
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.HallowedSummoner");
					player.maxMinions += 2;
				}
				else
				{
					player.setBonus = Language.GetTextValue("ArmorSetBonus.Hallowed");
				}

				player.onHitDodge = true;
			}

			if (player.head == 261 && player.body == 230 && player.legs == 213)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.CrystalNinja");
				player.GetDamage(DamageClass.Generic) += 0.1f;
				player.GetCritChance(DamageClass.Generic) += 10;
				player.dashType = 5;
			}

			if (player.head == 82 && player.body == 53 && player.legs == 48)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Tiki");
				player.maxMinions++;
				player.whipRangeMultiplier += 0.2f;
			}

			if (player.head == 134 && player.body == 95 && player.legs == 79)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Spooky");
				player.GetDamage(DamageClass.Summon) += 0.25f;
			}

			if (player.head == 160 && player.body == 168 && player.legs == 103)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Bee");
				player.GetDamage(DamageClass.Summon) += 0.1f;
				if (player.itemAnimation > 0 && player.inventory[player.selectedItem].type == ItemID.BeeGun)
					AchievementsHelper.HandleSpecialEvent(player, 3);
			}

			if (player.head == 162 && player.body == 170 && player.legs == 105)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Spider");
				player.GetDamage(DamageClass.Summon) += 0.12f;
			}

			if (player.head == 171 && player.body == 177 && player.legs == 112)
			{
				player.endurance += 0.12f;
				player.setSolar = true;
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Solar");
				player.solarCounter++;
				int num9 = 180;
				if (player.solarCounter >= num9)
				{
					if (player.solarShields > 0 && player.solarShields < 3)
					{
						for (int num10 = 0; num10 < Player.MaxBuffs; num10++)
						{
							if (player.buffType[num10] >= 170 && player.buffType[num10] <= 171)
								player.DelBuff(num10);
						}
					}

					if (player.solarShields < 3)
					{
						player.AddBuff(170 + player.solarShields, 5, quiet: false);
						for (int num11 = 0; num11 < 16; num11++)
						{
							Dust obj = Main.dust[Dust.NewDust(player.position, player.width, player.height, DustID.Torch, 0f, 0f, 100)];
							obj.noGravity = true;
							obj.scale = 1.7f;
							obj.fadeIn = 0.5f;
							obj.velocity *= 5f;
							obj.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
						}

						player.solarCounter = 0;
					}
					else
					{
						player.solarCounter = num9;
					}
				}

				for (int num12 = player.solarShields; num12 < 3; num12++)
				{
					player.solarShieldPos[num12] = Vector2.Zero;
				}

				for (int num13 = 0; num13 < player.solarShields; num13++)
				{
					player.solarShieldPos[num13] += player.solarShieldVel[num13];
					Vector2 value = ((float)player.miscCounter / 100f * ((float)Math.PI * 2f) + (float)num13 * ((float)Math.PI * 2f / (float)player.solarShields)).ToRotationVector2() * 6f;
					value.X = player.direction * 20;
					player.solarShieldVel[num13] = (value - player.solarShieldPos[num13]) * 0.2f;
				}

				if (player.dashDelay >= 0)
				{
					player.solarDashing = false;
					player.solarDashConsumedFlare = false;
				}

				bool flag2 = player.solarDashing && player.dashDelay < 0;
				if (player.solarShields > 0 || flag2)
					player.dashType = 3;
			}
			else
			{
				player.solarCounter = 0;
			}

			if (player.head == 169 && player.body == 175 && player.legs == 110)
			{
				player.setVortex = true;
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Vortex", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"));
			}
			else
			{
				player.vortexStealthActive = false;
			}

			if (player.head == 170 && player.body == 176 && player.legs == 111)
			{
				if (player.nebulaCD > 0)
					player.nebulaCD--;

				player.setNebula = true;
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Nebula");
			}

			if (player.head == 189 && player.body == 190 && player.legs == 130)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Stardust", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"));
				player.setStardust = true;
				if (player.whoAmI == Main.myPlayer)
				{
					if (player.FindBuffIndex(187) == -1)
						player.AddBuff(187, 3600);

					if (player.ownedProjectileCounts[623] < 1)
					{
						int num14 = 10;
						int num15 = 30;
						int num16 = Projectile.NewProjectile(player.GetSource_Misc("SetBonus_Stardust"), player.Center.X, player.Center.Y, 0f, -1f, 623, num15, num14, Main.myPlayer);
						Main.projectile[num16].originalDamage = num15;
					}
				}
			}
			else if (player.FindBuffIndex(187) != -1)
			{
				player.DelBuff(player.FindBuffIndex(187));
			}

			if (player.head == 200 && player.body == 198 && player.legs == 142)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.Forbidden", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"));
				player.setForbidden = true;
				player.UpdateForbiddenSetLock();
				Lighting.AddLight(player.Center, 0.8f, 0.7f, 0.2f);
			}

			if (player.head == 204 && player.body == 201 && player.legs == 145)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.SquireTier2");
				player.setSquireT2 = true;
				player.maxTurrets++;
			}

			if (player.head == 203 && player.body == 200 && player.legs == 144)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.ApprenticeTier2");
				player.setApprenticeT2 = true;
				player.maxTurrets++;
			}

			if (player.head == 205 && player.body == 202 && (player.legs == 147 || player.legs == 146))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.HuntressTier2");
				player.setHuntressT2 = true;
				player.maxTurrets++;
			}

			if (player.head == 206 && player.body == 203 && player.legs == 148)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.MonkTier2");
				player.setMonkT2 = true;
				player.maxTurrets++;
			}

			if (player.head == 210 && player.body == 204 && player.legs == 152)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.SquireTier3");
				player.setSquireT3 = true;
				player.setSquireT2 = true;
				player.maxTurrets++;
			}

			if (player.head == 211 && player.body == 205 && player.legs == 153)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.ApprenticeTier3");
				player.setApprenticeT3 = true;
				player.setApprenticeT2 = true;
				player.maxTurrets++;
			}

			if (player.head == 212 && player.body == 206 && (player.legs == 154 || player.legs == 155))
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.HuntressTier3");
				player.setHuntressT3 = true;
				player.setHuntressT2 = true;
				player.maxTurrets++;
			}

			if (player.head == 213 && player.body == 207 && player.legs == 156)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.MonkTier3");
				player.setMonkT3 = true;
				player.setMonkT2 = true;
				player.maxTurrets++;
			}

			if (player.head == 185 && player.body == 187 && player.legs == 127)
			{
				player.setBonus = Language.GetTextValue("ArmorSetBonus.ObsidianOutlaw");
				player.GetDamage(DamageClass.Summon) += 0.15f;
				player.whipRangeMultiplier += 0.3f;
				player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.15f;
			}

			player.ApplyArmorSoundAndDustChanges();

			ItemLoader.UpdateArmorSet(player, player.armor[0], player.armor[1], player.armor[2]);
		}
	}
}

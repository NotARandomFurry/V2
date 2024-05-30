using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.UI;

namespace V2.PlayerHandling
{
	public partial class V2Player : ModPlayer
	{
		public bool setBonusActive;
		public bool setBonusShouldBeDisplayed;
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
				case (ItemID.WoodHelmet, ItemID.WoodBreastplate, ItemID.WoodGreaves):
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

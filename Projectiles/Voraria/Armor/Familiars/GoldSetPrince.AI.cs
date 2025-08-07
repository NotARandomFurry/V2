using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Vanilla.Armor;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Projectiles.Vanilla.Summons.Pets;
using V2.Sounds.Vore;

namespace V2.Projectiles.Voraria.Armor.Familiars
{
	public partial class GoldSetPrince : ModProjectile
	{
		public int VengeanceTimer { get; set; }
		public static int VengeanceTimerMax => 120;
		public static bool GoldMiniPrinceAI(Projectile projectile)
		{
			Player ownerPlayer = Main.player[projectile.owner];
			GoldSetPrince aurifer = projectile.ModProjectile as GoldSetPrince;
			bool ateOwner = ownerPlayer.IsFoodFor(projectile, out bool churnedOwner);
			if (PredProjectile.GetStomachTracker(projectile) is VoreTracker goldPrinceTummy)
			{
				foreach (PreyData newFood in goldPrinceTummy.PreyQueue)
				{
					if (newFood.NoHealth)
						continue;

					ateOwner |= ownerPlayer.IsFoodFor(newFood.Instance, out bool foodChurnedOwner);
					churnedOwner |= foodChurnedOwner;
				}
				foreach (PreyData food in goldPrinceTummy.Prey)
				{
					if (food.NoHealth)
						continue;

					ateOwner |= ownerPlayer.IsFoodFor(food.Instance, out bool foodChurnedOwner);
					churnedOwner |= foodChurnedOwner;
				}
			}
			if (!aurifer.WaitingForChurnedOwner)
				aurifer.WaitingForChurnedOwner = ateOwner;
			float xOffset = V2Utils.TileCountAsPixelCount(2);
			float yOffset = -V2Utils.TileCountAsPixelCount(4.5);

			Vector2 offsetFromOwner = new Vector2((float)ownerPlayer.direction * xOffset, yOffset);
			Vector2 intendedLocation = ownerPlayer.MountedCenter + offsetFromOwner;
			float distanceToIntendedLocation = Vector2.Distance(projectile.Center, intendedLocation);

			if (aurifer.WaitingForChurnedOwner)
			{
				projectile.timeLeft = 2;
				goto SkipOwnerDeadCheck;
			}

			if (!ateOwner)
			{
				if (ownerPlayer.dead)
				{
					projectile.Kill();
					return false;
				}

				if (ModContent.GetInstance<GoldSetWithCrown>().Active(ownerPlayer))
					projectile.timeLeft = 2;

				if (ModContent.GetInstance<GoldSetWithOldHat>().Active(ownerPlayer))
					projectile.timeLeft = 2;

				if (ModContent.GetInstance<GoldSetWithOldOldHat>().Active(ownerPlayer))
					projectile.timeLeft = 2;

				if (distanceToIntendedLocation > 1000f)
					projectile.Center = ownerPlayer.Center + offsetFromOwner;
			}
			else
			{
				aurifer.WaitingForChurnedOwner = true;
				projectile.timeLeft = 2;
			}

			SkipOwnerDeadCheck:

			if (!GoldMiniPrinceAI_TryFindNewFood(projectile))
			{
				if (aurifer.WaitingForChurnedOwner)
				{
					projectile.velocity *= 0.925f;
					if (ownerPlayer.active && !ownerPlayer.dead && ownerPlayer.CurrentCaptor() is null && distanceToIntendedLocation <= V2Utils.TileCountAsPixelCount(20))
					{
						aurifer.WaitingForChurnedOwner = false;
						GoldMiniPrinceAI_HandleOwnerFollowing(projectile, ownerPlayer, intendedLocation);
					}
				}
				else if ((!(ateOwner && !churnedOwner)) && distanceToIntendedLocation <= V2Utils.TileCountAsPixelCount(20))
					GoldMiniPrinceAI_HandleOwnerFollowing(projectile, ownerPlayer, intendedLocation);
				else
					projectile.velocity *= 0.925f;
			}

			if (projectile.velocity.Length() > 6f)
			{
				float rotationSpeed = projectile.velocity.X * 0.1f;
				if (Math.Abs(projectile.rotation - rotationSpeed) >= (float)Math.PI)
				{
					if (rotationSpeed < projectile.rotation)
						projectile.rotation -= (float)Math.PI * 2f;
					else
						projectile.rotation += (float)Math.PI * 2f;
				}

				float rotationInertia = 12f;
				projectile.rotation = (projectile.rotation * (rotationInertia - 1f) + rotationSpeed) / rotationInertia;
				if (++projectile.frameCounter >= 3)
				{
					projectile.frameCounter = 0;
					projectile.frame++;
					if (projectile.frame >= Main.projFrames[projectile.type])
						projectile.frame = 0;
				}

				if (projectile.frameCounter == 0 || Main.rand.NextBool(15))
				{
					int num974 = Dust.NewDust(
						projectile.position,
						projectile.width,
						projectile.height,
						Main.rand.NextFromCollection(new List<int> {
							DustID.Gold,
							DustID.GoldCoin,
							DustID.GoldCritter,
							DustID.GoldCritter_LessOutline,
							DustID.GoldFlame,
						}),
						0f,
						0f,
						50,
						default,
						2f
					);
					Main.dust[num974].noGravity = true;
				}
			}
			else
			{
				if (projectile.rotation > (float)Math.PI)
					projectile.rotation -= (float)Math.PI * 2f;

				if (projectile.rotation > -0.005f && projectile.rotation < 0.005f)
					projectile.rotation = 0f;
				else
					projectile.rotation *= 0.96f;

				if (++projectile.frameCounter >= 5)
				{
					projectile.frameCounter = 0;
					projectile.frame++;
					if (projectile.frame >= Main.projFrames[projectile.type])
						projectile.frame = 0;
				}
			}
			return false;
		}

		public static void GoldMiniPrinceAI_HandleOwnerFollowing(Projectile projectile, Player ownerPlayer, Vector2 intendedLocation)
		{
			projectile.direction = projectile.spriteDirection = ownerPlayer.direction;

			float distanceToIntendedLocation = Vector2.Distance(projectile.Center, intendedLocation);

			Vector3 vector156 = new Vector3(1f, 0.6f, 1f) * 1.5f;
			DelegateMethods.v3_1 = vector156 * 0.75f;
			Utils.PlotTileLine(ownerPlayer.Center, ownerPlayer.Center + ownerPlayer.velocity * 6f, 40f, DelegateMethods.CastLightOpen);
			Utils.PlotTileLine(ownerPlayer.Left, ownerPlayer.Right, 40f, DelegateMethods.CastLightOpen);
			DelegateMethods.v3_1 = vector156 * 1.5f;
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 6f, 30f, DelegateMethods.CastLightOpen);
			Utils.PlotTileLine(projectile.Left, projectile.Right, 20f, DelegateMethods.CastLightOpen);

			Vector2 distanceFromIntendedLocation = intendedLocation - projectile.Center;
			float lockInDistance = 10f;
			if (distanceToIntendedLocation < lockInDistance)
				projectile.velocity *= 0.85f;

			if (distanceFromIntendedLocation != Vector2.Zero)
			{
				float maxSpeed = 15f;
				projectile.velocity = distanceFromIntendedLocation * 0.1f;
				if (projectile.velocity.Length() > maxSpeed)
				{
					projectile.velocity.Normalize();
					projectile.velocity *= maxSpeed;
				}
			}

			if (distanceToIntendedLocation > 50f)
			{
				projectile.direction = projectile.spriteDirection = 1;
				if (projectile.velocity.X < 0f)
					projectile.direction = projectile.spriteDirection = -1;
			}
		}
		public static bool GoldMiniPrinceAI_TryFindNewFood(Projectile projectile)
		{
			Player ownerPlayer = Main.player[projectile.owner];
			PreyType targetPreyType = PreyType.Undefined;
			int targetPreyIndex = -1;
			double maxPreyDistanceFromFairy = V2Utils.TileCountAsPixelCount(25);
			double maxPreyDistanceFromOwner = V2Utils.TileCountAsPixelCount(40);
			if (targetPreyType != PreyType.Undefined && targetPreyIndex != -1)
			{
				Entity targetPrey = targetPreyType switch
				{
					PreyType.Player => Main.player[targetPreyIndex],
					PreyType.NPC => Main.npc[targetPreyIndex],
					PreyType.Projectile => Main.projectile[targetPreyIndex],
					_ => null,
				};
				int targetPreyID = targetPreyType switch
				{
					PreyType.Player => 0,
					PreyType.NPC => Main.npc[targetPreyIndex].type,
					PreyType.Projectile => Main.projectile[targetPreyIndex].type,
					_ => -1,
				};
				float speed = 8.75f;
				float inertia = 10f;
				projectile.ai[1] += 0.05f;
				if (projectile.ai[1] > 7f)
					projectile.ai[1] = 7f;
				Vector2 direction = targetPrey.TrueCenter() - projectile.TrueCenter();
				direction.Normalize();
				direction *= speed;
				projectile.velocity = (projectile.velocity * (inertia - 1) + direction) / inertia;
				projectile.netUpdate = true;
				projectile.direction = projectile.spriteDirection = 1;
				if (projectile.velocity.X < 0f)
					projectile.direction = projectile.spriteDirection = -1;
				List<(PreyType, int)> meal = [(targetPreyType, targetPreyID)];
				projectile.DoContactGulpage(meal);
				return true;
			}
			return false;
		}
	}
}

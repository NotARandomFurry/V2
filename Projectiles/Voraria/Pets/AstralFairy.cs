using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Consumables;
using V2.Items.Voraria.Consumables.Potions;
using V2.PlayerHandling;
using V2.Projectiles.Vanilla.Summons.Pets;
using V2.Sounds.Vore;
using static System.Net.Mime.MediaTypeNames;

namespace V2.Projectiles.Voraria.Pets
{
    public class AstralFairyBuff : ModBuff
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Summons.AstralFairy.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Summons.AstralFairy.Description");

		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
			Main.persistentBuff[Type] = true;
        }

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			rare = ItemRarityID.Green;
			tip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Summons.AstralFairy.Description",
				new
				{
					
				}
			);
		}
        public override void Update(Player player, ref int buffIndex)
        { 
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<AstralFairy>());
        }
    }

	public static partial class AstralFairyStuff
	{
		public static int MaxHealth => 785000;
		public static double Size => 29;
		public static double MaxStomachCapacity => 42000.0;
        public static double Stomachache => 250000.0;
		public static double DigestDamage => 70.0;
		public static double DigestRate => 2;
		public static double AbsorbRate => 1.0 / (double)V2Utils.SensibleTime(
			seconds: 10
		);
	}

	public class AstralFairy : ModProjectile
	{
		public (Projectile, NPC) target = (null, null);
		
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			Main.projPet[Projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
            Projectile.CloneDefaults(ProjectileID.EyeOfCthulhuPet);

            Projectile.aiStyle = -1;
            Projectile.width = 64;
			Projectile.height = 162;

			Projectile.AsV2Proj().Gender = EntityGender.Female;

			Projectile.AsPred().MaxStomachCapacity = AstralFairyStuff.MaxStomachCapacity;
			Projectile.AsPred().BaseStomachacheMeterCapacity = AstralFairyStuff.Stomachache;
			Projectile.AsPred().CanSwallowBosses = true;

			Projectile.AsFood().DefinedSize = AstralFairyStuff.Size;
			Projectile.AsFood().MaxHealth = AstralFairyStuff.MaxHealth;
			Projectile.AsFood().Health = AstralFairyStuff.MaxHealth;

			Projectile.AsPred().MouthSoundRawOffset = new Vector2(0f, -14f);
			Projectile.AsPred().SmallGulps = Gulps.Short;
			Projectile.AsPred().SmallGulpThreshold = 0.1;
			Projectile.AsPred().BigGulps = Gulps.Standard;
			Projectile.AsPred().CanBeForceFed = CanAstralFairyBeForceFed;
			Projectile.AsPred().OnForceFed = OnAstralFairyForceFed;
			Projectile.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(12.5);

			Projectile.AsPred().DigestionType = EntityDigestionType.Acidic;
			Projectile.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			Projectile.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			Projectile.AsPred().SmallBurps = Burps.Humanoid.Small;
			Projectile.AsPred().StandardBurps = Burps.Humanoid.Standard;
			Projectile.AsPred().BurpPitchOffset = 0f;

			Projectile.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			Projectile.AsPred().GetVisualBellySize = GetVisualBellySize;
			Projectile.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			Projectile.AsFood().OnKilledByDigestion += PreyProjectile.OnKilledByDigestion_GrantLivePreyGoal;
			Projectile.AsFood().OnKilledByDigestion += OnKilledByDigestion;
		}
		public static bool CanAstralFairyBeForceFed(Projectile projectile) => true;
		public static void OnAstralFairyForceFed(Projectile projectile, Player player)
		{

		}
		public static void OnKilledByDigestion(Projectile projectile, Entity pred)
		{
			Player ownerPlayer = Main.player[projectile.owner];
			if (ownerPlayer.ownedProjectileCounts[projectile.type] <= 1)
				ownerPlayer.ClearBuff(ModContent.BuffType<AstralFairyBuff>());
		}
		public static int GetVisualBellySize(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(4.0 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				14
			);
		}
		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(1.4 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				0
			);
		}

		public static double GetDigestionTickDamage(Projectile projectile, PreyData prey)
		{
			double digestDamage = AstralFairyStuff.DigestDamage;
			return digestDamage;
		}
		public static double GetDigestionTickRate(Projectile projectile, PreyData prey)
		{
			double digestRate = AstralFairyStuff.DigestRate;
			Player ownerPlayer = Main.player[projectile.owner];
			if (!ownerPlayer.dead && ownerPlayer.sleeping.FullyFallenAsleep)
			{
				digestRate *= 1.5f;
				bool isEveryoneAsleep = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
				if (isEveryoneAsleep)
					digestRate *= (float)Main.dayRate;
			}

			return digestRate;
		}

		public static double GetPreyAbsorptionRate(Projectile projectile)
		{
			double absorbRate = AstralFairyStuff.AbsorbRate;
			Player ownerPlayer = Main.player[projectile.owner];
			if (!ownerPlayer.dead && ownerPlayer.sleeping.FullyFallenAsleep)
			{
				absorbRate *= 3f;
				bool isEveryoneAsleep = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
				if (isEveryoneAsleep)
					absorbRate *= (float)Main.dayRate;
			}
			return absorbRate;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}
		public override bool MinionContactDamage()
		{
			return false;
		}
		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
            bool ateOwner = owner.IsFoodFor(Projectile, out bool churnedOwner);
			VoreTracker tracker = PredProjectile.GetStomachTracker(Projectile);
			CheckActive(owner);
			Projectile.direction = owner.direction;
			if ((ateOwner && !churnedOwner) || owner.dead) Projectile.velocity *= 0.9f;
            		else 
			{
				Vector2 sitPosition = owner.Center + new Vector2(0, -160);
				Projectile.velocity = Projectile.Center.DirectionTo(sitPosition) * (Projectile.position.Distance(sitPosition) / 32f);
				if (Projectile.Center.Distance(sitPosition) < 1)
				{
					Projectile.velocity *= 0;
				}
				else if (Projectile.Center.Distance(sitPosition) < 5) Projectile.velocity *= 0.1f;
				else if (Projectile.Center.Distance(sitPosition) < 15) Projectile.velocity *= 0.25f;
				else if (Projectile.Center.Distance(sitPosition) < 25) Projectile.velocity *= 0.4f;
				else if (Projectile.Center.Distance(sitPosition) < 35) Projectile.velocity *= 0.55f;
				else if (Projectile.Center.Distance(sitPosition) < 45) Projectile.velocity *= 0.7f;
				else if (Projectile.Center.Distance(sitPosition) < 55) Projectile.velocity *= 0.85f;
			}
        }
		public static Rectangle TumBounds(int size, out int YOffset)
		{
			Rectangle bounds = new Rectangle(0, 0, 28, 24);
			YOffset = 0;
			switch (size)
			{
				case 0: bounds = new Rectangle(0, 0, 42, 24); break;
                case 1: bounds = new Rectangle(0, 24, 42, 26); break;
                case 2: bounds = new Rectangle(0, 50, 44, 28); break;
                case 3: bounds = new Rectangle(0, 78, 46, 34); break;
                case 4: bounds = new Rectangle(0, 112, 52, 40); break;
                case 5: bounds = new Rectangle(0, 152, 56, 44); break;
                case 6: bounds = new Rectangle(0, 196, 60, 48); break;
                case 7: bounds = new Rectangle(0, 244, 64, 54); break;
                case 8: bounds = new Rectangle(0, 298, 70, 62); break;
                case 9: bounds = new Rectangle(0, 360, 74, 70); break;
                case 10: bounds = new Rectangle(0, 430, 80, 76); break;
                case 11: bounds = new Rectangle(0, 506, 88, 88); break;
                case 12: bounds = new Rectangle(0, 594, 99, 102); break;
                case 13:
					bounds = new Rectangle(0, 812, 140, 140);
					YOffset = -2;
					break;
                case 14:
                    bounds = new Rectangle(0, 952, 160, 174);
                    YOffset = -4;
                    break;
            }

			return bounds;
		}
		public override void PostAI()
		{
			int framerate = 12;
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= framerate)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			Lighting.AddLight(Projectile.Center, new Vector3(111, 0, 255) * 0.005f);
        }
        public void CheckActive(Player player)
        {
            if (player.HasBuff(ModContent.BuffType<AstralFairyBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects val = Projectile.direction != -1 ? 0 : (SpriteEffects)1;
            SpriteEffects spriteEffects = val;

			Vector2 WingOffset = new Vector2(-22, -4);
			if (Projectile.direction == -1) WingOffset = new Vector2(12, -4);

            string textWings = "V2/Projectiles/Voraria/Pets/AstralFairyWings";
            Texture2D spriteWings = ModContent.Request<Texture2D>(textWings).Value;
            Rectangle sourceRectWings = new Rectangle(0, 92 * Projectile.frame, 72, 92);
            Main.EntitySpriteDraw(spriteWings, Projectile.position - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) + WingOffset, (Rectangle)sourceRectWings, lightColor, Projectile.rotation, Vector2.Zero, 1f, spriteEffects, 0f);

            int bodyFrame = Projectile.frame % 2;
			string textBody = "V2/Projectiles/Voraria/Pets/AstralFairy";
            Texture2D spriteBody = ModContent.Request<Texture2D>(textBody).Value;
			Rectangle sourceRectBody = new Rectangle(0, 162 * bodyFrame, 64, 162);
			Main.EntitySpriteDraw(spriteBody, Projectile.position - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), (Rectangle)sourceRectBody, lightColor, Projectile.rotation, Vector2.Zero, 1f, spriteEffects, 0f);

            string textTum = "V2/Projectiles/Voraria/Pets/AstralFairyTums";
            int TumSize = GetVisualBellySize(Projectile);
			Rectangle TumBox = TumBounds(TumSize, out int YOffset);
            Vector2 TumOffset = new Vector2(10, 64);
            if (Projectile.direction == -1) TumOffset = new Vector2(10 - (TumBox.Width - 44), 64);
            Texture2D spriteTum = ModContent.Request<Texture2D>(textTum).Value;
            Main.EntitySpriteDraw(spriteTum, Projectile.position - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) + (TumOffset + new Vector2(0, YOffset)), (Rectangle)TumBox, lightColor, Projectile.rotation, Vector2.Zero, 1f, spriteEffects, 0f);
            return false;
		}
	}
	public class AstralFairySummon : ModItem
    {
        public override string Texture => "V2/Items/UnspritedItem";
        public override void SetStaticDefaults()
        {
            DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
            Main.RegisterItemAnimation(Type, anim);
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<AstralFairy>(), ModContent.BuffType<AstralFairyBuff>());

            Item.width = 22;
            Item.height = 32;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(platinum: 1);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.AstralFairySummon",
                new
                {

                }
            );
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 20);

            return false;
        }
        public override void OnCreated(ItemCreationContext context)
        {
            if (context is RecipeItemCreationContext)
			{
                Item.NewItem(Main.LocalPlayer.GetSource_Misc("ThrowItem"), new Vector2(Main.LocalPlayer.position.X, Main.LocalPlayer.position.Y), new Vector2(Main.LocalPlayer.width, Main.LocalPlayer.height), ModContent.ItemType<AstralFairyController>());
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 3)
                .AddIngredient(ItemID.JungleRose, 1)
                .AddIngredient(ItemID.PixieDust, 75)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
    public class AstralFairyController : ModItem
    {
        public override void SetDefaults()
        {
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 10;
			Item.useTime = 10;
            Item.width = 30;
            Item.height = 34;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(platinum: 1);
        }
		public static void OnUse(Player player)
        {
			if (player.whoAmI == Main.myPlayer)
			{
				Vector2 position = Main.MouseWorld;
				Projectile astralFairy = null;
				foreach (var proj in Main.ActiveProjectiles)
				{
					if (proj.active && proj.type == ModContent.ProjectileType<AstralFairy>() && Main.player[proj.owner] == player)
					{
						astralFairy = proj;
					}
				}
                if (astralFairy == null) return;
				Rectangle hitbox = new Rectangle((int)position.X - 16, (int)position.Y - 16, 32, 32);
				foreach (var item in Main.ActiveItems)
				{
					if (item.active && item.CurrentCaptor() is null && hitbox.Intersects(item.Hitbox))
					{
						PredProjectile.Swallow(astralFairy, item);
					}
				}
				foreach (var item in Main.ActiveNPCs)
				{
					if (item.active && item.CurrentCaptor() is null && hitbox.Intersects(item.Hitbox))
					{
						PredProjectile.Swallow(astralFairy, item);
					}
				}
                foreach (var item in Main.ActiveProjectiles)
                {
                    if (item.active && item.CurrentCaptor() is null && hitbox.Intersects(item.Hitbox) && item.type != ModContent.ProjectileType<AstralFairy>())
                    {
                        PredProjectile.Swallow(astralFairy, item);
                    }
                }
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.AstralFairyController",
                new
                {

                }
            );
            }
        public override bool? UseItem(Player player)
        {
			OnUse(player);
            return true;
        }
    }
}

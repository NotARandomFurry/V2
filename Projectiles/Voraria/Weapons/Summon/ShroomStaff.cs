using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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
using V2.PlayerHandling;

namespace V2.Projectiles.Voraria.Weapons.Summon
{
    public class ShroomStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.SlimeStaff;
        }
        public override void SetDefaults()
        {
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item44;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<ShroomFairyBuff>();
            Item.shoot = ModContent.ProjectileType<ShroomFairy>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, 0, 0, Main.myPlayer);
            projectile.originalDamage = 0;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShroomiteBar, 2)
                .AddIngredient(ItemID.GlowingMushroom, 35)
                .AddIngredient(ItemID.PixieDust, 25)
                .Register();
        }
    }
    public class SlimeStaff : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.SlimeStaff;
        public override void SetDefaults(Item entity)
        {
            ItemID.Sets.ShimmerTransformToItem[entity.netID] = ModContent.ItemType<ShroomStaff>();
            base.SetDefaults(entity);
        }
    }
}

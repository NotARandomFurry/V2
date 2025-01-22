using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Items;
using V2.Items.Vanilla.Accessories;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.Sounds.Vore;

namespace V2.Mounts
{
    public class BeeDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.MountBack, PlayerDrawLayers.Carpet);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {

            var player = drawInfo.drawPlayer;
            int walkFrame = (int)(Main.GlobalTimeWrappedHourly * 6) % 2;
            if (walkFrame == 1) walkFrame = 1;
            else walkFrame = 2;
            bool isActuallyMoving = (player.IsAirborne()) || (player.velocity.X != 0);
            if (player.AsV2Player().BeeTransformation == true)
            {
                var tumSize = Math.Min((int)Math.Floor(player.AsPred().StomachSize / 2f), BeeTransformationItem.MaxTumSize);
                var weightSize = BeeTransformationItem.GetVisualWeightStage(player);
                Vector2 pos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - 20), (int)(drawInfo.Position.Y - Main.screenPosition.Y - 14));
                Texture2D BeeBody = ModContent.Request<Texture2D>("V2/Mounts/BeeBody_Weight" + weightSize).Value;
                Rectangle sourceRect = Rectangle.Empty;
                if (isActuallyMoving) sourceRect = new Rectangle(tumSize * BeeBody.Width / 5, (BeeBody.Height / 3) * walkFrame, BeeBody.Width / 5, BeeBody.Height / 3);
                else sourceRect = new Rectangle(tumSize * BeeBody.Width / 5, 0, BeeBody.Width / 5, BeeBody.Height / 3);
                DrawData actualDraw = new DrawData(BeeBody, pos, sourceRect, drawInfo.colorMount, player.bodyRotation, Vector2.Zero, 1f, drawInfo.playerEffect);
                actualDraw.shader = player.cMount;
                drawInfo.DrawDataCache.Add(actualDraw);
           }
        }
    }
    [AutoloadEquip(EquipType.Wings)]
    public class BeeTransformationItem : ModItem
    {
        public static int GetVisualWeightStage(Player player)
        {
            return Math.Min(
                (int)Math.Floor(0.8 * Math.Sqrt(player.AsPred().BeeTransformation_ExtraWeight)),
                1
            );
        }
        public static int MaxTumSize => 4;
        public static double WeightGainRatio => 0.005;
        public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
        public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Mounts.BeeTransformationItem");
        public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Mounts.BeeTransformationItem.Short");
        public override string Texture => "V2/Items/UnspritedItem";

        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(220, 0.8f, 0.8f);
            DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
            Main.RegisterItemAnimation(Type, anim);
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            //Item.mountType = ModContent.MountType<BeeTransformation>();
            Item.accessory = true;
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(
                gold: 3,
                silver : 15
            );
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 4f;
            acceleration = 0.3f;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f; // Falling glide speed
            ascentWhenRising = 0.15f; // Rising speed
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2f;
            constantAscend = 0.135f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AsV2Player().BeeTransformation = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.Mounts.BeeTransformationItem",
                new
                {

                }
            );
        }
    }
}

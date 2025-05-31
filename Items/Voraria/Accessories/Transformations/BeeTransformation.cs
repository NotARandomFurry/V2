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

namespace V2.Items.Voraria.Accessories.Transformations
{
    public class BeeTransformationBuff : ModBuff
    {
        public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Mounts.BeeTransformation.Name");
        public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Mounts.BeeTransformation.Description");
        public override string Texture => "V2/StatusEffects/Voraria/Buffs/BuffPlaceholder";
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Mounts.BeeTransformation.Description",
                new
                {

                }
            );
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<BeeTransformation>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
    public class BeeTransformation : ModMount
    {
        public static int MaxTumSize => 4;
        public static double WeightGainRatio => 0.01;
        public static int flightTime = 320;
        public static float movementSpeed = 7f;
        public static int jumpHeight = 14;
        public static float acceleration = 0.33f;
        public static int GetVisualWeightStage(Player player)
        {
            return Math.Min(
                (int)Math.Floor(0.3 * Math.Sqrt(player.AsPred().BeeTransformation_ExtraWeight)),
                1
            );
        }
        public override void UpdateEffects(Player player)
        {
            if (player.mount.Active && player.mount.Type == ModContent.MountType<BeeTransformation>())
            {
                float weightMovementMult = 1.0f / (float)Math.Max(1.0, (player.AsPred().StomachWeight / 1.75) + 1.0 + (player.AsPred().BeeTransformation_ExtraWeight / 3.0));
                MountData.flightTimeMax = (int)Math.Ceiling(flightTime * weightMovementMult);
                MountData.runSpeed = (int)Math.Ceiling(movementSpeed * weightMovementMult);
                MountData.dashSpeed = (int)Math.Ceiling(movementSpeed * weightMovementMult);
                MountData.jumpSpeed = (int)Math.Ceiling(movementSpeed * weightMovementMult);
                MountData.jumpHeight = (int)Math.Ceiling(jumpHeight * weightMovementMult);
                MountData.acceleration = (int)Math.Ceiling(acceleration * weightMovementMult);
            }
        }
        public override void SetStaticDefaults()
        {
            MountData.buff = ModContent.BuffType<BeeTransformationBuff>();
            MountData.heightBoost = 0;
            MountData.flightTimeMax = flightTime;
            MountData.fatigueMax = 160;
            MountData.fallDamage = 0f;
            MountData.usesHover = true;
            MountData.runSpeed = movementSpeed;
            MountData.dashSpeed = movementSpeed;
            MountData.acceleration = 0.33f;
            MountData.jumpHeight = 14;
            MountData.jumpSpeed = movementSpeed;
            MountData.blockExtraJumps = true;
            MountData.delegations = new Mount.MountDelegatesData();
            MountData.delegations.HandPosition = new Mount.MountDelegatesData.OverridePositionMethod(DelegateMethods.Mount.NoHandPosition);
            MountData.spawnDust = DustID.Honey;

            MountData.totalFrames = 3;

            MountData.playerYOffsets = Enumerable.Repeat(0, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
            MountData.xOffset = 0;
            MountData.yOffset = 0;
            MountData.playerHeadOffset = 0;

            // Standing
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            // Running
            MountData.runningFrameCount = 2;
            MountData.runningFrameDelay = 48;
            MountData.runningFrameStart = 1;
            // Flying
            MountData.flyingFrameCount = 2;
            MountData.flyingFrameDelay = 12;
            MountData.flyingFrameStart = 1;
            // In-air
            MountData.inAirFrameCount = 2;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 1;
            // Idle
            MountData.idleFrameCount = 1;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            // Swim
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
        }
    }
    public class BeeDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.MountBack, PlayerDrawLayers.Carpet);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
<<<<<<< Updated upstream:Mounts/BeeTransformation.cs
            if (player.mount.Active && player.mount.Type == ModContent.MountType<BeeTransformation>())
=======
            int walkFrame = (int)(Main.GlobalTimeWrappedHourly * 6) % 2;
            if (walkFrame == 1) walkFrame = 1;
            else walkFrame = 2;
            bool isActuallyMoving = player.IsAirborne() || player.velocity.X != 0;
            if (player.AsV2Player().BeeTransformation == true)
>>>>>>> Stashed changes:Items/Voraria/Accessories/Transformations/BeeTransformation.cs
            {
                var tumSize = Math.Min((int)Math.Floor(player.AsPred().StomachSize / 2f), BeeTransformation.MaxTumSize);
                var weightSize = BeeTransformation.GetVisualWeightStage(player);
                Vector2 pos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - 20), (int)(drawInfo.Position.Y - Main.screenPosition.Y - 14));
<<<<<<< Updated upstream:Mounts/BeeTransformation.cs
                Texture2D BeeBody = ModContent.Request<Texture2D>("V2/Mounts/BeeBody_Weight" + weightSize).Value;
                Rectangle sourceRect = new Rectangle(tumSize * BeeBody.Width / 5, player.mount._frame * (BeeBody.Height / 3), BeeBody.Width / 5, BeeBody.Height / 3);
=======
                Texture2D BeeBody = ModContent.Request<Texture2D>("V2/Items/Voraria/Accessories/Transformations/BeeBody_Weight" + weightSize).Value;
                Rectangle sourceRect = Rectangle.Empty;
                if (isActuallyMoving) sourceRect = new Rectangle(tumSize * BeeBody.Width / 5, BeeBody.Height / 3 * walkFrame, BeeBody.Width / 5, BeeBody.Height / 3);
                else sourceRect = new Rectangle(tumSize * BeeBody.Width / 5, 0, BeeBody.Width / 5, BeeBody.Height / 3);
>>>>>>> Stashed changes:Items/Voraria/Accessories/Transformations/BeeTransformation.cs
                DrawData actualDraw = new DrawData(BeeBody, pos, sourceRect, drawInfo.colorMount, player.bodyRotation, Vector2.Zero, 1f, drawInfo.playerEffect);
                actualDraw.shader = player.cMount;
                drawInfo.DrawDataCache.Add(actualDraw);
            }
        }
    }
    public class BeeTransformationItem : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
        public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Mounts.BeeTransformationItem");
        public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Mounts.BeeTransformationItem.Short");
        public override string Texture => "V2/Items/UnspritedItem";

        public override void SetStaticDefaults()
        {
            DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
            Main.RegisterItemAnimation(Type, anim);
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.mountType = ModContent.MountType<BeeTransformation>();
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(
                gold: 3,
                silver: 15
            );
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.AddVorariaDynamicItemTooltip(
                "Voraria.Accessories.Transformations.BeeTransformationItem",
                new
                {

                }
            );
        }
    }
}

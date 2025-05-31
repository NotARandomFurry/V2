using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using V2.Core;
using V2.Items.Vanilla.Accessories;
using V2.Items.Voraria.Accessories;
using V2.Items.Voraria.Accessories.Transformations;
using V2.Items.Voraria.Accessories.Transformations.Baelz;
using V2.Items.Voraria.Consumables.Potions;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Buffs
{
	public class WellFed : ModBuff
    {
        public static int Def = 2;
        public static int Crit = 2;
        public static float AtkSpd = 0.05f;
        public static float Dmg = 0.05f;
        public static float KB = 0.05f;
        public static float RunSpd = 0.2f;
        public static float MineSpd = 0.05f;

        public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.WellFed.Name.1");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Buffs.WellFed.Description.Base");
        public override bool RightClick(int buffIndex) => false;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public static string DecideIfPositive(int Amount, bool flat = false)
        {
            string text = flat ? Amount.ToString() : Amount.ToString() + "%";
            if (Amount > 0)
            {
                text = "[c/5FFF5F:+" + text + "]";
            }
            else if (Amount < 0)
            {
                text = "[c/FFBF5F:" + text + "]";
            }
            else
                text = "[c/BDBDBD:+" + text + "]";
            return text;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
            Player player = Main.LocalPlayer;
            double multiplier = Math.Clamp(player.AsPred().WellFed_Multiplier, -3, 3.5);
            int def = (int)Math.Round(Def * multiplier);
            int crit = (int)Math.Round(Crit * multiplier);
            float atkspd = (float)Math.Round((int)(AtkSpd * 100) * multiplier) / 100;
            float dmg = (float)Math.Round((int)(Dmg * 100) * multiplier) / 100;
            float kb = (float)Math.Round((int)(KB * 100) * multiplier) / 100;
            float runspd = (float)Math.Round((int)(RunSpd * 100) * multiplier) / 100;
            float minespd = (float)Math.Round((int)(MineSpd * 100) * multiplier) / 100;

            int textRarity = ItemRarityID.Gray;
            int chosenName = 0;

            if (multiplier >= 2)
            {
                textRarity = ItemRarityID.Lime;
                chosenName = 3;
            }
            else if (multiplier >= 1)
            {
                textRarity = ItemRarityID.Green;
                chosenName = 2;
            }
            else if (multiplier > 0)
            {
                textRarity = ItemRarityID.Green;
                chosenName = 1;
            }
            else if (multiplier < 0)
            {
                textRarity = ItemRarityID.LightRed;
                chosenName = -1;
            }

            rare = textRarity;

            buffName = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Buffs.WellFed.Name." + chosenName.ToString(),
                new
                {

                }
            );

            string baseTooltip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Buffs.WellFed.Description.Base",
                new
                {

                }
			);
            string statTip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Voraria.Buffs.WellFed.Description.StatChanges",
                new
                {
                    Damage = DecideIfPositive((int)(dmg * 100)),
                    Defense = DecideIfPositive(def, true),
                    Critical = DecideIfPositive(crit),
                    AttackSpeed = DecideIfPositive((int)(atkspd * 100)),
                    Knockback = DecideIfPositive((int)(kb * 100)),
                    RunSpeed = DecideIfPositive((int)(runspd* 100)),
                    MiningSpeed = DecideIfPositive((int)(minespd * 100)),

                }
            );
            tip = baseTooltip + "\n" + statTip;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            Player player = Main.LocalPlayer;
            double multiplier = Math.Clamp(player.AsPred().WellFed_Multiplier, -3, 3.5);
            int chosenImage = 1;

            if (multiplier >= 2)
            {
                chosenImage = 4;
            }
            else if (multiplier >= 1)
            {
                chosenImage = 3;
            }
            else if (multiplier > 0)
            {
                chosenImage = 2;
            }
            else if (multiplier < 0)
            {
                chosenImage = 0;
            }

            Texture2D buffTextureSheet = ModContent.Request<Texture2D>("V2/StatusEffects/Voraria/Buffs/WellFedSheet").Value;

            spriteBatch.Draw(
                buffTextureSheet,
                drawParams.Position,
                new Rectangle(32 * chosenImage, 0, 32, 32),
                drawParams.DrawColor,
                0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                0f
            );
            return false;
        }
    }
}

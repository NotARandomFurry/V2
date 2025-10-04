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

namespace V2.StatusEffects.Vanilla.Buffs
{
    public class WellFed : GlobalBuff
    {
        public override void SetStaticDefaults()
        {
            V2.ModifiedStatusEffects.Add(BuffID.WellFed, this);
        }
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type != BuffID.WellFed)
                return;
            player.wellFed = true;
        }

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            if (type != BuffID.WellFed)
                return;

            rare = ItemRarityID.Green;
            tip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Vanilla.Buffs.WellFed.Description",
                new
                {
                    FedValue = 0.5
                }
            );
        }
    }
    public class WellFed2 : GlobalBuff
    {
        public override void SetStaticDefaults()
        {
            V2.ModifiedStatusEffects.Add(BuffID.WellFed2, this);
        }
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type != BuffID.WellFed2)
                return;
            player.wellFed = true;
        }

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            if (type != BuffID.WellFed2)
                return;

            rare = ItemRarityID.Green;
            tip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Vanilla.Buffs.WellFed2.Description",
                new
                {
                    FedValue = 1
                }
            );
        }
    }
    public class WellFed3 : GlobalBuff
    {
        public override void SetStaticDefaults()
        {
            V2.ModifiedStatusEffects.Add(BuffID.WellFed3, this);
        }
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type != BuffID.WellFed3)
                return;
            player.wellFed = true;
        }

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            if (type != BuffID.WellFed3)
                return;

            rare = ItemRarityID.Green;
            tip = Language.GetTextValueWith(
                "Mods.V2.StatusEffects.Vanilla.Buffs.WellFed3.Description",
                new
                {
                    FedValue = 1.5
                }
            );
        }
    }
}
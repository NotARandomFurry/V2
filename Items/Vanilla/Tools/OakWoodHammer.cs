using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
    public class OakWoodHammer : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodenHammer;

        public override void SetDefaults(Item item)
        {
            item.SetNameOverride(Language.GetTextValue("Mods.V2.ItemName.Vanilla.Tools.OakWoodHammer"));

            item.AsFood().MaxHealth = 165;
            item.AsFood().Size = 0.45;
        }
    }
}

using Terraria;
using Terraria.ModLoader;
using V2.Items.ItemGroupUtils;

namespace V2.Items.Voraria.Placeables;

public class SillyBalloonTiles : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
        V2Utils.ItemIDSets.BalloonTiles.Contains(entity.type);

    public override void SetDefaults(Item entity)
    {
        entity.AsFood().MaxHealth = Balloons.MaxHealth;
        entity.AsFood().Size = 0.50;
        entity.AsFood().WellFedPower = 0.02;

        entity.AsFood().OnBreak += (item, pred, direct) => Balloons.OnBalloonBrokenDown(item, pred, direct, 0.1d);
    }
}
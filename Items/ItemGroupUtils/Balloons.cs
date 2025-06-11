using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.Items.ItemGroupUtils;

public class Balloons : GlobalItem
{
    public static int MaxHealth => 80;
    public static double AfterPopSize => 0.2;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
        V2Utils.ItemIDSets.EquipableBalloons.Contains(entity.type);
    public override void SetDefaults(Item entity)
    {
        entity.AsFood().MaxHealth = MaxHealth;
        entity.AsFood().Size = 0.55;
        entity.AsFood().WellFedPower = 0.01;

        entity.AsFood().OnBreak += OnBalloonBrokenDown;
    }

    public static bool OnBalloonBrokenDown(Item item, Entity pred, bool direct, double poppedSize)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath63);
        if (pred is Player predPlayer)
        {
            predPlayer.AsPred().StomachTracker.PreyFromInstance(item).WeightLeftToDigest = poppedSize;
        }

        return true;
    }
    
    public static bool OnBalloonBrokenDown(Item item, Entity pred, bool direct)
    {
        return OnBalloonBrokenDown(item, pred, direct, AfterPopSize);
    }
}
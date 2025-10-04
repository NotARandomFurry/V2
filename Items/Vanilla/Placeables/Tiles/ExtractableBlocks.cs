using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Vanilla.Placeables.Tile
{
	public class Silt : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.SiltBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 30;
			item.AsFood().Size = 0.1;
<<<<<<< Updated upstream
		}
        public static bool OnBreak(Item item, Entity pred, bool direct)
        {
            SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
            SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);

            if (pred is Player playerPred)
            {
               
            }
            else if (pred is NPC NPCPred)
            {
                
=======
            item.AsFood().OnBreak += OnBreak;
        }
		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
            int stack = item.stack;
            while (stack > 0)
            {
                stack--;
                V2Utils.ExtractItemInTum(item.type, pred);
            }
            return true;
		}
    }
    public class Slush : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.SlushBlock;

        public override void SetDefaults(Item item)
        {
            item.AsFood().MaxHealth = 30;
            item.AsFood().Size = 0.12;
            item.AsFood().OnBreak += OnBreak;
        }
        public static bool OnBreak(Item item, Entity pred, bool direct)
        {
            int stack = item.stack;
            while (stack > 0)
            {
                stack--;
                V2Utils.ExtractItemInTum(item.type, pred);
>>>>>>> Stashed changes
            }
            return true;
        }
    }
    public class Fossil : GlobalItem
<<<<<<< Updated upstream
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.DesertFossil;

        public override void SetDefaults(Item item)
        {
            item.AsFood().MaxHealth = 250;
            item.AsFood().AcidResistTier = 1;
            item.AsFood().Size = 0.15;
=======
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.DesertFossil;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 250;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().Size = 0.15;
            item.AsFood().OnBreak += OnBreak;
        }
        public static bool OnBreak(Item item, Entity pred, bool direct)
        {
            int stack = item.stack;
            while (stack > 0)
            {
                stack--;
                V2Utils.ExtractItemInTum(item.type, pred);
            }
            return true;
>>>>>>> Stashed changes
        }
    }
}

using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;

namespace V2.NPCs.Vanilla.Bosses.KingSlime
{
	public static class KingJelloDessertStuff
	{
		public static class ItemTheftRules
		{
		}

		public static KingJelloDessert AsKingSlime(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out KingJelloDessert unreasonablyThickFairy))
				throw new Exception("this instance of the King Slime, sadly, can't be pred or prey. no monstrously large jell-o dessert for you, I guess");

			return unreasonablyThickFairy;
		}
	}

	public class KingJelloDessert : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public static int MuffledScreechMinDelay => V2Utils.SensibleTime(seconds: 5);
		private int _muffledScreechDelay;
		public int MuffledScreechDelay
		{
			get => _muffledScreechDelay;
			set => _muffledScreechDelay = Math.Max(0, value);
		}
		public SlotId MuffledMusic { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.KingSlime;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Other;

			npc.AsFood().DefinedBaseSize = 70.0;
			npc.AsPred().MaxStomachCapacity = 325.0;
			npc.AsPred().BaseStomachacheMeterCapacity = 4000.0;

			npc.AsPred().SmallGulps = null;
			npc.AsPred().SmallGulpThreshold = 3.75;
			npc.AsPred().BigGulps = null;

			npc.AsPred().DigestionType = EntityDigestionType.Other;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			npc.AsPred().MouthSoundRawOffset = npc.TrueCenter() + new Vector2(npc.direction * 0f, -40f);
			npc.AsPred().SmallBurps = null;
			npc.AsPred().StandardBurps = null;

			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsFood().ItemTheftRules = [];
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.65;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			double baseDigestionTickDamage = 4.0;
			baseDigestionTickDamage *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseDigestionTickDamage;
		}
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 15
			);
			baseAbsorptionRate *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseAbsorptionRate;
		}
	}
}

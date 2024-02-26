using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using V2.Core;
using V2.PlayerHandling;

namespace V2.NPCs.Voraria.TownNPCs.Succubus
{
	public class LucindaBound : ModNPC
	{
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
		}

		public override void SetDefaults()
		{
			NPC.friendly = true;
			NPC.width = 22;
			NPC.height = 36;
			NPC.aiStyle = 0;
			NPC.lifeMax = 700;
			NPC.damage = 35;
			NPC.defense = 22;
			NPC.rarity = 4;
			NPC.knockBackResist = 0f;
			NPC.HitSound = SoundID.NPCHit1;

			NPC.AsFood().DefinedSize = 1.15;
			NPC.AsPred().MaxStomachCapacity = 2.2;
			NPC.AsPred().BaseStomachacheMeterCapacity = 155.0;

			NPC.AsPred().DigestionType = EntityDigestionType.Acidic;

			NPC.buffImmune[BuffID.OnFire] = true;
			NPC.buffImmune[BuffID.OnFire3] = true;
			NPC.buffImmune[BuffID.Burning] = true;
			NPC.buffImmune[BuffID.ShadowFlame] = true;
			NPC.lavaImmune = true;
		}

		public override void ModifyTypeName(ref string typeName) => typeName = "Bound Succubus";

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (ModContent.GetInstance<V2MasterSystem>().freedSucc)
				return 0f;

			if (!spawnInfo.Player.ZoneUnderworldHeight)
				return 0f;

			if (NPC.AnyNPCs(ModContent.NPCType<LucindaBound>()))
				return 0f;

			return 0.15f;
		}

		public override bool CanChat() => true;

		public override string GetChat()
		{
			List<string> possibleLines = new List<string>
			{
				"Hey! You! Morsel! Mind lendin' me a hand? Been stuck here since last Tuesday, havin' to munch on imps and the chips off those serpents just to keep my gut quiet.",
				"Hey there, soon-to-be snack. I know you're not all that busy, so care to help a pred out? My gut and I will be MORE than happy to make it worth your while.",
				"So WHAT!? The Convocation says I'm out for a bit because the bimbo that one of 'em wanted made good gut fodder!? Dumbasses...hey, lunch! You can tear these tacky tightropes for me, yeah?",
			};
			return Main.rand.NextFromCollection(possibleLines);
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "Free";
			button2 = "Don't Free";
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
			if (firstButton)
			{
				ModContent.GetInstance<V2MasterSystem>().freedSucc = true;
				NPC.AI_000_TransformBoundNPC(Main.CurrentPlayer.whoAmI, ModContent.NPCType<Lucinda>());
				Main.npcChatText = "There ya go! Wasn't that hard. Now, c'mere so I can reward you with some time in my gut...or, y'know, just some old trinkets from your ol' pal Lucinda to help you be a great pred just like me.";
			}
			else
			{
				ModContent.GetInstance<V2MasterSystem>().freedSucc = true;
				NPC.AI_000_TransformBoundNPC(Main.CurrentPlayer.whoAmI, ModContent.NPCType<Lucinda>());
				PredNPC.SwallowWithTextIfApplicable(
					NPC,
					Main.CurrentPlayer,
					"[c/7F7F7F:<As a scowl quickly crosses her face, the succubus whips her tail around your form and uses it to guide you headfirst into her mouth, soon letting her newly-filled stomach break the bindings for you.>]\n"
				  + "Fine! If you're gonna be such a " + (Main.CurrentPlayer.Male ? "dick" : "bitch") + " about it, you can AT LEAST be helpful enough to fatten up my thighs! Haven't had some REAL good eats in a while..."
				);
			}
		}
	}
}

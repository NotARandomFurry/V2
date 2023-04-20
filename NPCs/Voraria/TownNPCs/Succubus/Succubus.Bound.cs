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
	public class BoundSuccubus : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				Hide = true, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.friendly = true;
			NPC.width = 22;
			NPC.height = 36;
			NPC.aiStyle = 0;
			NPC.lifeMax = 250;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.rarity = 4;
			NPC.knockBackResist = 0f;
			NPC.HitSound = SoundID.NPCHit1;

			NPC.AsPred().maxStomachCapacity = 2.2;

			NPC.buffImmune[BuffID.OnFire] = true;
			NPC.buffImmune[BuffID.OnFire3] = true;
			NPC.buffImmune[BuffID.Burning] = true;
			NPC.buffImmune[BuffID.ShadowFlame] = true;
			NPC.lavaImmune = true;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (ModContent.GetInstance<MasterSystem>().freedSucc)
				return 0f;

			if (!spawnInfo.Player.ZoneUnderworldHeight)
				return 0f;

			if (NPC.AnyNPCs(ModContent.NPCType<BoundSuccubus>()))
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
				"So WHAT!? The Convocation says I'm out for a bit because the bimbo that one of 'em wanted made good gut fodder!? Dumbasses...anywho, you can tear these tacky tightropes for me, yeah?",
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
				ModContent.GetInstance<MasterSystem>().freedSucc = true;
				NPC.AI_000_TransformBoundNPC(Main.CurrentPlayer.whoAmI, ModContent.NPCType<Succubus>());
				Main.npcChatText = "There ya go! Wasn't that hard. Now, c'mere so I can reward you with some time in my gut...or, y'know, just some old trinkets from ol' Lucy to help you be a great pred like me.";
			}
			else
			{
				ModContent.GetInstance<MasterSystem>().freedSucc = true;
				NPC.AI_000_TransformBoundNPC(Main.CurrentPlayer.whoAmI, ModContent.NPCType<Succubus>());
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

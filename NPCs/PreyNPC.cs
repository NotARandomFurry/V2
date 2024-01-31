using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Starter;
using V2.StatusEffects.Debuffs;

namespace V2.NPCs
{
	public static class PreyNPCStuff
	{
		public static PreyNPC AsFood(this NPC npc, bool risky = false)
		{
			if (!npc.TryGetGlobalNPC(out PreyNPC preyNPC))
			{
				if (risky)
					return null;

				throw new Exception("this NPC can't be eaten, and thus, doesn't have a PreyNPC global attached to them. look for your favorite food elsewhere");
			}
			return preyNPC;
		}
	}

	public partial class PreyNPC : GlobalNPC
	{
		public int EatenSafetyFrames { get; set; }
		public bool Digested { get; set; }

		public delegate void DelegatePreyAI(NPC npc, Entity pred);
		public DelegatePreyAI SpecialPreyAI { get; set; }

		public double Size { get; set; }

		public int STR { get; set; }
		/// <summary>
		/// Expresses, from 0 to 12, how well this NPC struggles.<br/>
		/// Defaults to 5.<br/>
		/// </summary>
		public int StruggleEffectiveness { get; set; }
		public StatModifier StruggleStrengthModifier { get; set; }
		public double StruggleStrength {
			get {
				double baseStruggleStrength = 1.5;
				baseStruggleStrength += 0.3 * STR;
				return StruggleStrengthModifier.ApplyTo((float)baseStruggleStrength);
			}
		}

		public delegate void DelegateOnKilledByDigestion(NPC npc, Entity pred);
		public DelegateOnKilledByDigestion OnKilledByDigestion { get; set; }

		public bool CanChatAsPrey { get; set; }

		public SoundStyle? DigestingHitSound;
		public SoundStyle? DigestedDeathSound;

		public StatModifier TakenDigestionDamageModifier { get; set; }

		public double SoftenedDigestionDamageTaken { get; set; }
		public StatModifier SoftenedDigestionDamageModifier { get; set; }
		public int SoftenedWearoffDelay { get; set; }
		public static int SoftenedWearoffMaxDelay => V2Utils.SensibleTime(seconds: 2, frames: 30);
		public StatModifier SoftenedWearoffRateModifier { get; set; }

		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;

		public PreyNPC()
		{
			SpecialPreyAI = null;
			Size = 0;

			STR = 0;
			StruggleEffectiveness = 5;

			OnKilledByDigestion = null;

			CanChatAsPrey = false;
			DigestingHitSound = null;
			DigestedDeathSound = null;
		}

		public override void SetDefaults(NPC npc)
		{
			if (!NPCID.Sets.ProjectileNPC[npc.type])
				npc.AsFood().OnKilledByDigestion = OnKilledByDigestion_GrantLivePreyGoal;
		}

		public override void ResetEffects(NPC npc)
		{
			npc.AsFood().Digested = false;

			npc.AsFood().STR = (int)((double)npc.lifeMax / 40.0);
			npc.AsFood().StruggleStrengthModifier = StatModifier.Default;

			npc.AsFood().TakenDigestionDamageModifier = StatModifier.Default;

			if (npc.AsFood().EatenSafetyFrames > 0)
				npc.AsFood().EatenSafetyFrames--;

			if (!npc.HasBuff(ModContent.BuffType<Softened>()))
				npc.AddBuff(ModContent.BuffType<Softened>(), 3);
			npc.AsFood().SoftenedDigestionDamageModifier = StatModifier.Default;
			npc.AsFood().SoftenedWearoffRateModifier = StatModifier.Default;
			if (npc.AsFood().SoftenedWearoffDelay > 0)
				npc.AsFood().SoftenedWearoffDelay--;
			else if (npc.AsFood().SoftenedDigestionDamageTaken > 0)
				npc.AsFood().SoftenedDigestionDamageTaken -= npc.AsFood().SoftenedWearoffRateModifier.ApplyTo((float)(25.0 / 60.0));

			DetermineDigestingSounds(npc);
		}

		public static void DetermineDigestingSounds(NPC npc)
		{
			if (npc.HitSound is not null && DigestingHitSoundDatabase.ContainsKey(npc.HitSound.Value))
				npc.AsFood().DigestingHitSound = DigestingHitSoundDatabase[npc.HitSound.Value];
			if (npc.DeathSound is not null && DigestedDeathSoundDatabase.ContainsKey(npc.DeathSound.Value))
				npc.AsFood().DigestedDeathSound = DigestedDeathSoundDatabase[npc.DeathSound.Value];
		}

		public override bool CanHitNPC(NPC npc, NPC target)
		{
			if (npc.CurrentCaptor() is not null || target.CurrentCaptor() is not null || npc.AsFood().EatenSafetyFrames > 0 || target.AsFood().EatenSafetyFrames > 0)
				return false;

			return true;
		}

		public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
		{
			if (npc.CurrentCaptor() is not null || npc.AsFood().EatenSafetyFrames > 0)
				return false;

			return true;
		}

		public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			return null;
		}

		public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			return null;
		}

		public override bool? CanBeCaughtBy(NPC npc, Item item, Player player)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			return null;
		}

		public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
		{
			if (npc.CurrentCaptor() is not null)
				boundingBox = Rectangle.Empty;
		}

		public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (target.type == ModContent.NPCType<Lucinda>() && PredNPC.CanSwallow(target, npc))
			{
				modifiers.FinalDamage *= 0;
				modifiers.Knockback.Base = 0f;
				modifiers.DisableCrit();
			}
		}

		public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit)
		{
			if (target.type == ModContent.NPCType<Lucinda>())
			{
				PredNPC.Swallow(target, npc);
			}
		}

		public override bool? CanChat(NPC npc)
		{
			if (npc.CurrentCaptor() is not null)
				return npc.AsFood().CanChatAsPrey;

			return null;
		}

		public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			return null;
		}

		/// <summary>
		/// Deals the given amount of digestion damage to the NPC, respecting damage variation and, if their predator is a player, said player's luck.
		/// </summary>
		/// <param name="pred">The pred currently digesting this NPC.</param>
		/// <param name="digestionDamage">The total amount of digestion damage to be dealt, before damage variation calculations.</param>
		/// <returns>Whether or not the resulting digestion tick kills the NPC.</returns>
		public static bool TakeDigestionDamage(NPC npc, Entity pred, double digestionDamage)
		{
			if (npc.life <= 0)
				return true;

			if (npc.realLife != -1 && npc.realLife != npc.whoAmI)
				return false;

			int trueDigestionDamage = Main.DamageVar((float)digestionDamage, (pred is Player playerPred) ? -playerPred.luck : 0);
			if (ModContent.GetInstance<V2ServerConfig>().DefenseInDigestionCalcs)
			{
				trueDigestionDamage -= npc.defense;
				if (trueDigestionDamage < 0)
					trueDigestionDamage = 0;
			}
			trueDigestionDamage = (int)Math.Floor(npc.AsFood().TakenDigestionDamageModifier.ApplyTo(trueDigestionDamage));
			npc.AsFood().SoftenedDigestionDamageTaken += npc.AsFood().SoftenedDigestionDamageModifier.ApplyTo(trueDigestionDamage);
			npc.AsFood().SoftenedWearoffDelay = SoftenedWearoffMaxDelay;
			npc.life -= trueDigestionDamage;
			CombatText digestionText = Main.combatText[CombatText.NewText(
				npc.Hitbox,
				npc.friendly ? Color.DarkGreen : Color.LimeGreen,
				trueDigestionDamage,
				false,
				true
			)];
			digestionText.position.X = pred.Center.X;
			digestionText.position.X += pred.direction * 14;
			if (pred.direction == -1)
				digestionText.position.X -= ChatManager.GetStringSize(FontAssets.CombatText[0].Value, digestionText.text, new Vector2(digestionText.scale)).X;
			digestionText.position.Y = npc.Center.Y;
			digestionText.position.Y += npc.height / 5f;
			digestionText.velocity.X = pred.direction * 2.5f;
			digestionText.velocity.Y = -4f;

			if (npc.life <= 0)
			{
				npc.life = 0;
				npc.checkDead();
				NetMessage.TrySendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
				return true;
			}
			else
			{
				npc.netUpdate = true;
				return false;
			}
		}

		public static double GetCurrentTotalWeight(NPC npc)
		{
			double baseWeight = PreyData.GetPreySize(npc);
			double bellyWeight = PredNPC.GetCurrentBellyWeight(npc);
			return baseWeight + bellyWeight;
		}

		public override bool CheckActive(NPC npc)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			return true;
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (npc.CurrentCaptor() is not null)
				return false;

			return true;
		}

		public static void OnKilledByDigestion_GrantLivePreyGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<FirstLivePrey>().TrySetCompletion(predPlayer);
			}
		}
	}
}

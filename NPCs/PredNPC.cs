using Humanizer;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.Core.StruggleSystem;
using V2.Items;
using V2.NPCs.Vanilla.TownNPCs.Nurse;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Debuffs;

namespace V2.NPCs
{
	public static class PredNPCStuff
	{
		public static PredNPC AsPred(this NPC npc, bool risky = false)
		{
			if (!npc.TryGetGlobalNPC(out PredNPC predNPC))
			{
				if (risky)
					return null;

				throw new Exception("this NPC can't be a pred at all, as they don't have a PredNPC global attached to them. look for your favorite gut to sleep in elsewhere");
			}
			return predNPC;
		}
	}

	public partial class PredNPC : GlobalNPC
	{
		public EntityGender Gender { get; set; }

		public static VoreTracker GetStomachTracker(NPC npc)
		{
			if (Main.gameMenu)
				return null;

			return ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(x => x.Predator is NPC predNPC && predNPC.whoAmI == npc.whoAmI);
		}
		public EntityDigestionType DigestionType { get; set; }
		public double MaxStomachCapacity { get; set; }
		public float MaxSwallowRange { get; set; }
		public double ExtraWeight { get; set; }
		/// <summary>
		/// Allows this NPC to eat bosses regardless of whether or not they're a boss themselves.<br/>
		/// Defaults to false.<br/>
		/// </summary>
		public bool CanSwallowBosses { get; set; }

		public Vector2 MouthSoundRawOffset { internal get; set; }
		public static Vector2 MouthSoundOffset(NPC npc)
		{
			Vector2 happyBurpyOffsetDirectionized = npc.AsPred().MouthSoundRawOffset;
			if (npc.direction != 0)
				happyBurpyOffsetDirectionized.X *= npc.direction;
			return happyBurpyOffsetDirectionized;
		}

		public SoundStyle? SmallGulps { get; set; }
		public double SmallGulpThreshold { get; set; }
		public SoundStyle? BigGulps { get; set; }

		public SoundStyle? SmallBurps { get; set; }
		public double SmallBurpThreshold { get; set; }
		public SoundStyle? StandardBurps { get; set; }
		public double BigBurpThreshold { get; set; }
		public SoundStyle? BigBurps { get; set; }

		/// <summary>
		/// If set to true, this NPC can bypass the "Pred Non-Preference" config option, being able to gulp down the player and anything else in-game regardless of what it is set to.<br/>
		/// Defaults to false.<br/>
		/// </summary>
		public bool NonPreferenceBypass { get; set; }
		public delegate bool DelegateCanBeForceFed(NPC npc);
		public DelegateCanBeForceFed CanBeForceFed { get; set; }

		public delegate void DelegateOnForceFed(NPC npc, Player player);
		public DelegateOnForceFed OnForceFed { get; set; }


		public delegate double DelegateGetDigestionTickRate(NPC npc, PreyData prey);
		public DelegateGetDigestionTickRate GetDigestionTickRate { get; set; }

		public delegate double DelegateGetDigestionTickDamage(NPC npc, PreyData prey);
		public DelegateGetDigestionTickDamage GetDigestionTickDamage { get; set; }

		private double _stomachache;
		public double Stomachache
		{
			get => _stomachache;
			set => _stomachache = Math.Min(Math.Max(0, value), StomachacheMeterCapacity);
		}
		public double BaseStomachacheMeterCapacity { get; set; }
		public StatModifier StomachacheMeterCapacityModifier;
		public double StomachacheMeterCapacity
		{
			get
			{
				double baseStomachacheMeterCapacity = BaseStomachacheMeterCapacity;
				return StomachacheMeterCapacityModifier.ApplyTo((float)baseStomachacheMeterCapacity);
			}
		}
		/// <summary>
		/// Expresses, from 0 to 12, how well this NPC keeps up with struggles as a pred.<br/>
		/// Defaults to 5.<br/>
		/// </summary>
		public int CounterStruggleEffectiveness { get; set; }

		public delegate void DelegateOnDigestionKill(NPC npc, PreyData digestedPrey);
		public DelegateOnDigestionKill OnDigestionKill { get; set; }

		public delegate void DelegateGetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathMessageKeyList);
		public DelegateGetDigestedPlayerAdditionalDeathMessages GetAdditionalDigestedPlayerMessages { get; set; }

		public delegate double DelegateGetPreyAbsorptionRate(NPC npc);
		public DelegateGetPreyAbsorptionRate GetPreyAbsorptionRate { get; set; }

		public delegate int DelegateGetVisualBellySize(NPC npc);
		public DelegateGetVisualBellySize GetVisualBellySize { get; set; }

		public delegate int DelegateGetVisualWeightStage(NPC npc);
		public DelegateGetVisualWeightStage GetVisualWeightStage { get; set; }

		public SlotId ActiveStomachNoises { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;

		public PredNPC()
		{
			MaxStomachCapacity = 1.0;
			MaxSwallowRange = 36f;
			ExtraWeight = 0.0;
			CanSwallowBosses = false;
			
			GetDigestionTickRate = null;
			GetDigestionTickDamage = null;
			GetPreyAbsorptionRate = null;

			NonPreferenceBypass = false;
			CanBeForceFed = (NPC npc) => false;
			OnForceFed = null;

			Stomachache = 0;
			BaseStomachacheMeterCapacity = 100.0;
			CounterStruggleEffectiveness = 5;

			MouthSoundRawOffset = Vector2.Zero;
			SmallGulps = Gulps.Short;
			SmallGulpThreshold = 0.2;
			BigGulps = Gulps.Standard;
			SmallBurps = null;
			SmallBurpThreshold = 0.2;
			StandardBurps = null;
			BigBurpThreshold = 2.0;
			BigBurps = null;

			OnDigestionKill = null;

			GetVisualBellySize = null;
			GetVisualWeightStage = null;
		}

		public override void ResetEffects(NPC npc)
		{
			double stomachacheQuellPerTick = npc.AsPred().StomachacheMeterCapacity * (0.05 / (double)V2Utils.SensibleTime(seconds: 1));
			if (GetStomachTracker(npc) is null || !AnyPreyStillAlive(npc))
				stomachacheQuellPerTick *= 0.1;
			Stomachache -= stomachacheQuellPerTick;
				npc.AsPred().Stomachache -= stomachacheQuellPerTick;

			StomachacheMeterCapacityModifier = StatModifier.Default;
		}

		public static bool CanSwallow(NPC pred, Entity prey)
		{
			if (V2.VoreNPCBlacklist is not null && V2.VoreNPCBlacklist.Count > 0 && V2.VoreNPCBlacklist.Contains(pred.type))
				return false;

			if (GetCurrentBellyWeight(pred) >= pred.AsPred().MaxStomachCapacity)
				return false;

			if (!pred.AsPred().NonPreferenceBypass)
			{
				switch (ModContent.GetInstance<V2ServerConfig>().GenderBlacklist)
				{
					default:
						// do absolutely fucking nothing lmao
						break;
					case "No Male":
						if (pred.AsV2NPC().Gender == EntityGender.Male)
							return false;
						break;
					case "No Female":
						if (pred.AsV2NPC().Gender == EntityGender.Female)
							return false;
						break;
					case "No M or F...but why?":
						if (pred.AsV2NPC().Gender != EntityGender.Other)
							return false;
						break;
				}
			}

			if (prey is Player preyPlayer)
			{
				if (PreyData.GetPreySize(preyPlayer) >= pred.AsPred().MaxStomachCapacity - GetCurrentBellyWeight(pred))
					return false;

				if (preyPlayer.CurrentCaptor() is not null)
					return false;
			}
			else if (prey is NPC preyNPC)
			{
				if (V2.VoreNPCBlacklist is not null && V2.VoreNPCBlacklist.Count > 0 && V2.VoreNPCBlacklist.Contains(preyNPC.type))
					return false;

				bool tastesLikeSkittles = preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress;
				if (tastesLikeSkittles)
					return preyNPC.CurrentCaptor() is null;

				bool isThePreyAFuckingBoss = preyNPC.boss || (preyNPC.type >= NPCID.EaterofWorldsHead && preyNPC.type <= NPCID.EaterofWorldsTail);  // I hate EoW
				bool isThePredAFuckingBoss = pred.boss || (pred.type >= NPCID.EaterofWorldsHead && pred.type <= NPCID.EaterofWorldsTail);           // I hate EoW
				if (!pred.AsPred().CanSwallowBosses && isThePreyAFuckingBoss && !isThePredAFuckingBoss)
					return false;

				if (PreyData.GetPreySize(preyNPC) >= pred.AsPred().MaxStomachCapacity - GetCurrentBellyWeight(pred))
					return false;

				if (preyNPC.CurrentCaptor() is not null)
					return false;
			}
			else if (prey is Projectile preyProjectile)
			{
				if (V2.VoreNPCBlacklist is not null && V2.VoreProjectileBlacklist.Count > 0 && V2.VoreProjectileBlacklist.Contains(preyProjectile.type))
					return false;

				if (preyProjectile.AsFood().MaxHealth == -1)
					return false;

				if (PreyData.GetPreySize(preyProjectile) >= pred.AsPred().MaxStomachCapacity - GetCurrentBellyWeight(pred))
					return false;

				if (preyProjectile.CurrentCaptor() is not null)
					return false;
			}
			else if (prey is Item preyItem)
			{
				if (preyItem.AsFood().MaxHealth == -1)
					return false;

				if (preyItem.favorited)
					return false;

				if (preyItem.CurrentCaptor() is not null)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Causes the given predator NPC to swallow the given prey entity, if the given prey entity can be swallowed.
		/// </summary>
		/// <param name="pred">The predator which will attempt to swallow the given prey.</param>
		/// <param name="prey">The prey which will be attempt to be swallowed by the given predator.</param>
		public static void Swallow(NPC pred, Entity prey, int MPstate = 0, int MPwhoAmI = -1)
		{
			if (!CanSwallow(pred, prey))
				return;

			if (MPstate == 0 && Main.netMode == NetmodeID.MultiplayerClient)
			{
				MPstate = 1;
				MPwhoAmI = Main.myPlayer;
			}

			PreyData food = PreyData.NewData(prey);
			AddNewPrey(pred, food);
			PlaySwallowGulp(pred, food);
			switch (food.Type)
			{
				case PreyType.Player:
					Player player = prey as Player;
					player.AsFood().TotalTimesSwallowed += 1;
					break;
				case PreyType.NPC:
					NPC npc = prey as NPC;
					break;
				case PreyType.Projectile:
					Projectile projectile = prey as Projectile;
					break;
				case PreyType.Item:
					Item item = prey as Item;
					item.AsFood().OnSwallow?.Invoke(item, pred);
					if (item.AsFood().OnSwallowDamage > 0)
					{
						pred.StrikeNPC(
							new NPC.HitInfo
							{
								SourceDamage = item.AsFood().OnSwallowDamage,
								DamageType = DamageClass.Default,
								Crit = false,
								HideCombatText = true
							}
						);
					}
					if (item.AsFood().OnSwallowSoreThroatTime > 0)
						pred.AddBuff(ModContent.BuffType<SoreThroat>(), item.AsFood().OnSwallowSoreThroatTime);
					break;
			}
			pred.netUpdate = true;

			if (MPstate == 1)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.RequestSwallowPrey);
				packet.Write((byte)1);
				packet.Write(pred.whoAmI);
				packet.Write((byte)food.Type);
				packet.Write(prey.whoAmI);
				packet.Write(MPwhoAmI);
				packet.Send();
			}
			else if (MPstate == 2)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.SyncSwallowPrey);
				packet.Write((byte)1);
				packet.Write(pred.whoAmI);
				packet.Write((byte)food.Type);
				packet.Write(prey.whoAmI);
				packet.Write(MPwhoAmI);
				packet.Send(-1, ignoreClient: MPwhoAmI);
			}
		}

		public static void SwallowWithTextIfApplicable(NPC pred, Player prey, string chatboxText)
		{
			if (!CanSwallow(pred, prey))
				return;

			Swallow(pred, prey);
			SetChatboxText(pred, prey, chatboxText);
		}

		public static void SetChatboxText(NPC pred, Player prey, string chatText)
		{
			Main.CancelHairWindow();
			Main.SetNPCShopIndex(0);
			Main.InGuideCraftMenu = false;
			prey.dropItemCheck();
			Main.npcChatCornerItem = 0;
			prey.sign = -1;
			Main.editSign = false;
			prey.SetTalkNPC(pred.whoAmI);
			Main.playerInventory = false;
			prey.chest = -1;
			Recipe.FindRecipes();
			Main.npcChatText = chatText;
		}

		/// <summary>
		/// Runs update ticks on all food in the given predator's stomach.
		/// </summary>
		/// <param name="pred">The NPC to update all food in the stomach of.</param>
		public static void UpdatePrey(NPC pred)
		{
			if (pred.AsPred().Stomachache >= pred.AsPred().StomachacheMeterCapacity)
			{
				foreach (PreyData prey in GetStomachTracker(pred).Prey)
				{
					Entity realPrey = prey.Type switch
					{
						PreyType.Player => prey.Instance as Player,
						PreyType.NPC => prey.Instance as NPC,
						PreyType.Projectile => prey.Instance as Projectile,
						PreyType.Item => prey.Instance as Item,
						PreyType.Custom => null,
						_ => throw new NotImplementedException(),
					};
					realPrey.position = pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f);
					realPrey.velocity = new Vector2(pred.direction * 12.5f, -2.5f);
					if (realPrey is NPC realPreyNPC)
					{
						realPreyNPC.AsFood().EatenSafetyFrames = 20;
					}
					else if (realPrey is Player realPreyPlayer)
					{

					}
					else if (realPrey is Item realPreyItem)
					{
						realPreyItem.noGrabDelay = 60;
					}
				}
				SoundEngine.PlaySound(
					pred.AsPred().StandardBurps,
					pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
				);
				GetStomachTracker(pred).Prey.Clear();
				GetStomachTracker(pred).RefreshStruggleChartList();
				return;
			}
			foreach (PreyData prey in GetStomachTracker(pred).Prey)
			{
				if (!prey.NoHealth)
				{
					prey.UpdateInStomach?.Invoke(prey.Instance, pred, false);

					switch (prey.Type)
					{
						case PreyType.Player:
							Player preyPlayer = prey.Instance as Player;
							if (preyPlayer is null || !preyPlayer.active || preyPlayer.dead)
								continue;

							preyPlayer.velocity = Vector2.Zero;
							preyPlayer.position = pred.position;
							break;
						case PreyType.NPC:
							NPC preyNPC = prey.Instance as NPC;
							if (preyNPC is null || !preyNPC.active)
								continue;

							preyNPC.velocity = Vector2.Zero;
							preyNPC.position = pred.position;
							break;
						case PreyType.Projectile:
							Projectile preyProjectile = prey.Instance as Projectile;
							if (preyProjectile is null || !preyProjectile.active)
								continue;

							preyProjectile.velocity = Vector2.Zero;
							preyProjectile.position = pred.position;
							break;
						case PreyType.Item:
							Item preyItem = prey.Instance as Item;
							if (preyItem is null || !preyItem.active)
								continue;

							preyItem.velocity = Vector2.Zero;
							preyItem.position = pred.position;
							break;
					}

					if (prey.Type == PreyType.Player
					 && pred.type == NPCID.Nurse
					 && pred.AsNurse().healPlayerIndex != -1
					 && pred.AsNurse().healPlayerIndex == (prey.Instance as Player).whoAmI
					 && !pred.AsNurse().digestScamPatient)
					{
						Player healingPreyPlayer = prey.Instance as Player;
						if (healingPreyPlayer.statLife >= healingPreyPlayer.statLifeMax2)
							pred.AsNurse().healOvertime += 1;
					}

					if (pred.AsPred().GetDigestionTickRate is null || pred.AsPred().GetDigestionTickDamage is null)
					{
						if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
							Main.NewText(pred.FullName + " has invalid digestion damage/tick rate methods!");
						break;
					}
					double digestionDamage = pred.AsPred().GetDigestionTickDamage.Invoke(pred, prey);
					double digestionTickRate = pred.AsPred().GetDigestionTickRate.Invoke(pred, prey);
					int digestionTickFrameRate = (int)Math.Round(60.0 / digestionTickRate);
					if (prey.timeSpentInStomach % digestionTickFrameRate == 0)
					{
						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPlayer = prey.Instance as Player;
								bool shouldDigestPlayer = true;
								bool shouldHealPlayer = pred.type == NPCID.Nurse && pred.AsNurse().healPlayerIndex != -1 && pred.AsNurse().healPlayerIndex == preyPlayer.whoAmI && !pred.AsNurse().digestScamPatient;
								if (shouldHealPlayer)
								{
									bool shouldFurtherHealPlayer = preyPlayer.statLife < preyPlayer.statLifeMax2;
									if (shouldFurtherHealPlayer)
									{
										prey.NoHealth = false;
										preyPlayer.statLife += (int)Math.Round(digestionDamage);
										if (preyPlayer.statLife > preyPlayer.statLifeMax2)
											preyPlayer.statLife = preyPlayer.statLifeMax2;
										CombatText digestionText = Main.combatText[CombatText.NewText(
											preyPlayer.Hitbox,
											Color.LimeGreen,
											(int)Math.Round(digestionDamage),
											false,
											true
										)];
										digestionText.position.X = pred.Center.X;
										digestionText.position.X += pred.direction * 14;
										digestionText.position.Y = preyPlayer.Center.Y;
										digestionText.position.Y += preyPlayer.height / 5f;
										digestionText.velocity.X = pred.direction * 2.5f;
										digestionText.velocity.Y = -4f;
									}
								}
								else if (shouldDigestPlayer)
								{
									prey.NoHealth = preyPlayer.AsFood().TakeDigestionDamage(pred, digestionDamage);
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyPlayer.name);
									if (prey.NoHealth)
									{
										if (pred.AsPred().OnDigestionKill is not null)
											pred.AsPred().OnDigestionKill.Invoke(pred, prey);
										PlayDigestionBelch(pred, prey);
									}
								}
								else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
									Main.NewText("Failed to deal digestion damage to prey: " + preyPlayer.name);
								break;
							case PreyType.NPC:
								NPC preyNPC = prey.Instance as NPC;
								bool shouldDigestNPC = true;
								if (shouldDigestNPC)
								{
									if (preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress)
										digestionDamage *= 50.0;
									prey.NoHealth = PreyNPC.TakeDigestionDamage(preyNPC, pred, digestionDamage);
									preyNPC.netUpdate = true;
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyNPC.GivenOrTypeName);
									else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Failed to deal digestion damage to prey: " + preyNPC.GivenOrTypeName);
									if (prey.NoHealth)
									{
										if (pred.AsPred().OnDigestionKill is not null)
											pred.AsPred().OnDigestionKill.Invoke(pred, prey);
										PlayDigestionBelch(pred, prey);
									}
								}
								break;
							case PreyType.Projectile:
								Projectile preyProjectile = prey.Instance as Projectile;
								bool shouldDigestProjectile = true;
								if (shouldDigestProjectile)
								{
									prey.NoHealth = preyProjectile.TakeDigestionDamage(pred, digestionDamage);
									preyProjectile.netUpdate = true;
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyProjectile.Name);
									else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Failed to deal digestion damage to prey: " + preyProjectile.Name);
									if (prey.NoHealth)
									{
										if (pred.AsPred().OnDigestionKill is not null)
											pred.AsPred().OnDigestionKill.Invoke(pred, prey);
										PlayDigestionBelch(pred, prey);
									}
								}
								break;
						}
					}
				}
				else
				{
					prey.UpdateInStomach?.Invoke(null, pred, true);

					if (pred.AsPred().GetPreyAbsorptionRate is null)
						break;

					double digestedWeightPerTick = pred.AsPred().GetPreyAbsorptionRate.Invoke(pred) / (double)GetStomachTracker(pred).Prey.Count;
					if (prey.WeightLeftToDigest <= digestedWeightPerTick)
					{
						pred.AsPred().ExtraWeight += prey.WeightLeftToDigest * 0.4;
						prey.WeightLeftToDigest = 0;
					}
					else
					{
						pred.AsPred().ExtraWeight += digestedWeightPerTick * 0.4;
						prey.WeightLeftToDigest -= digestedWeightPerTick;
					}
				}
			}

			if (pred.CurrentCaptor() is null && pred.AsPred().GetVisualBellySize is not null)
			{
				bool stomachNoisesPlaying = SoundEngine.TryGetActiveSound(pred.AsPred().ActiveStomachNoises, out ActiveSound stomachNoises);
				if (!stomachNoisesPlaying)
				{
					pred.AsPred().ActiveStomachNoises = SoundEngine.PlaySound(
						StomachNoises.Muffled with { Volume = 0.2f + (0.1f * pred.AsPred().GetVisualBellySize.Invoke(pred)) },
						pred.TrueCenter()
					);
					SoundEngine.TryGetActiveSound(pred.AsPred().ActiveStomachNoises, out stomachNoises);
				}

				if (stomachNoises is null)
					return;

				stomachNoises.Position = pred.TrueCenter();
				stomachNoises.Volume = 0.2f;
				stomachNoises.Volume += 0.1f * pred.AsPred().GetVisualBellySize.Invoke(pred);
				if (stomachNoises.Volume > 0.75f)
					stomachNoises.Volume = 0.75f;
			}
		}

		public static void AddNewPrey(NPC pred, PreyData prey)
		{
			if (GetStomachTracker(pred) is null)
				VoreTracker.NewTracker(pred, new List<PreyData>() { prey });
			else
				GetStomachTracker(pred).QueueNewPrey(prey);
		}

		public static string GetDigestedPlayerDeathReason(NPC npc, Player player)
		{
			List<string> deathMessageKeyList = new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.Universal.1",
				"Mods.V2.Death.DigestedPlayer.Universal.2",
				"Mods.V2.Death.DigestedPlayer.Universal.3",
				"Mods.V2.Death.DigestedPlayer.Universal.4",
				"Mods.V2.Death.DigestedPlayer.Universal.5",
				"Mods.V2.Death.DigestedPlayer.Universal.6",
				"Mods.V2.Death.DigestedPlayer.Universal.7",
				"Mods.V2.Death.DigestedPlayer.Universal.8",
				"Mods.V2.Death.DigestedPlayer.Universal.9",
				"Mods.V2.Death.DigestedPlayer.Universal.10",
				"Mods.V2.Death.DigestedPlayer.Universal.11",
				"Mods.V2.Death.DigestedPlayer.Universal.12",
				"Mods.V2.Death.DigestedPlayer.Universal.13",
				"Mods.V2.Death.DigestedPlayer.Universal.14",
				"Mods.V2.Death.DigestedPlayer.Universal.15",
				"Mods.V2.Death.DigestedPlayer.Universal.16",
				"Mods.V2.Death.DigestedPlayer.Universal.17",
				"Mods.V2.Death.DigestedPlayer.Universal.18",
				"Mods.V2.Death.DigestedPlayer.Universal.19",
				"Mods.V2.Death.DigestedPlayer.Universal.20",
				"Mods.V2.Death.DigestedPlayer.Universal.21",
				"Mods.V2.Death.DigestedPlayer.Universal.22",
			};
			switch (npc.AsPred().DigestionType)
			{
				case EntityDigestionType.Acidic:
					deathMessageKeyList.AddRange(new List<string>
					{
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.1",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.2",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.3",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.4",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.5",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.6",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.7",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.8",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.9",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.10",
					});
					break;
				case EntityDigestionType.Thermal:
					deathMessageKeyList.AddRange(new List<string>
					{
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.1",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.2",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.3",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.4",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.5",
					});
					break;
				case EntityDigestionType.Other:
					break;
			}

			if (npc.AsPred().GetAdditionalDigestedPlayerMessages is not null)
				npc.AsPred().GetAdditionalDigestedPlayerMessages.Invoke(npc, player, deathMessageKeyList);
			string finalDeathReasonKey = Main.rand.NextFromCollection(deathMessageKeyList);
			
			return Language.GetTextValueWith(
				finalDeathReasonKey,
				new
				{
					Player = player.name,
					Pred = npc.GivenOrTypeName
				}
			);
		}

		public override void OnKill(NPC npc)
		{
			if (npc.CurrentCaptor() is not null && GetStomachTracker(npc) is not null)
			{
				foreach (PreyData prey in GetStomachTracker(npc).Prey)
				{
					npc.CurrentCaptor().QueueNewPrey(prey);
				}
			}
		}

		/// <summary>
		/// Calculates the current weight of the given predator's stomach, based on all the prey inside of it.<br/>
		/// Used primarily in conjunction with <see cref="MaxStomachCapacity"/> to safeguard against overeating.<br/>
		/// </summary>
		/// <param name="pred">The predator whose stomach is to be weighed.</param>
		/// <returns>The current total weight of the given predator's stomach.</returns>
		public static double GetCurrentBellyWeight(NPC pred)
		{
			if (GetStomachTracker(pred) is null)
				return 0.0;

			double totalBellyWeight = 0.0;
			foreach (PreyData prey in GetStomachTracker(pred).Prey)
			{
				totalBellyWeight += prey.WeightLeftToDigest;
				if (prey.NoHealth)
					continue;

				switch (prey.Type)
				{
					case PreyType.Player:
						Player preyPredPlayer = prey.Instance as Player;
						totalBellyWeight += preyPredPlayer.AsPred().StomachWeight;
						break;
					case PreyType.NPC:
						NPC preyPredNPC = prey.Instance as NPC;
						totalBellyWeight += GetCurrentBellyWeight(preyPredNPC);
						break;
				}
			}
			return totalBellyWeight;
		}

		public static bool AnyPreyStillAlive(NPC pred)
		{
			if (GetStomachTracker(pred) is not null)
			{
				foreach (PreyData prey in GetStomachTracker(pred).Prey)
				{
					if (!prey.NoHealth)
						return true;
				}
			}
			return false;
		}
	}
}

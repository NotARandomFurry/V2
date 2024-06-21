using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Debuffs;

namespace V2.Projectiles
{
	public partial class PredProjectile : GlobalProjectile
	{
		public static VoreTracker GetStomachTracker(Projectile projectile)
		{
			if (Main.gameMenu)
				return null;

			return ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(x => x.Predator is Projectile predProjectile && predProjectile.whoAmI == projectile.whoAmI);
		}
		public EntityDigestionType DigestionType { get; set; }
		public double MaxStomachCapacity { get; set; }
		public float MaxSwallowRange { get; set; }
		public double ExtraWeight { get; set; }
		/// <summary>
		/// Allows this projectile to eat bosses despite not being a boss themselves.<br/>
		/// Defaults to false.<br/>
		/// </summary>
		public bool CanSwallowBosses { get; set; }

		public Vector2 MouthSoundRawOffset { internal get; set; }
		public static Vector2 MouthSoundOffset(Projectile projectile)
		{
			Vector2 happyBurpyOffsetDirectionized = projectile.AsPred().MouthSoundRawOffset;
			if (projectile.direction != 0)
				happyBurpyOffsetDirectionized.X *= projectile.direction;
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
		public float BurpPitchOffset { get; set; }

		/// <summary>
		/// If set to true, this projectile can bypass the "Pred Non-Preference" config option, being able to gulp down the player and anything else in-game regardless of what it is set to.<br/>
		/// Defaults to false.<br/>
		/// </summary>
		public bool NonPreferenceBypass { get; set; }
		public delegate bool DelegateCanBeForceFed(Projectile projectile);
		public DelegateCanBeForceFed CanBeForceFed { get; set; }

		public delegate void DelegateOnForceFed(Projectile projectile, Player player);
		public DelegateOnForceFed OnForceFed { get; set; }


		public delegate double DelegateGetDigestionTickRate(Projectile projectile, PreyData prey);
		public DelegateGetDigestionTickRate GetDigestionTickRate { get; set; }

		public delegate double DelegateGetDigestionTickDamage(Projectile projectile, PreyData prey);
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
				if (BaseStomachacheMeterCapacity == -1)
					return -1;

				double baseStomachacheMeterCapacity = BaseStomachacheMeterCapacity;
				return StomachacheMeterCapacityModifier.ApplyTo((float)baseStomachacheMeterCapacity);
			}
		}
		/// <summary>
		/// Expresses, from 0 to 12, how well this projectile keeps up with struggles as a pred.<br/>
		/// Defaults to 5.<br/>
		/// </summary>
		public int CounterStruggleEffectiveness { get; set; }

		public delegate void DelegateOnDigestionKill(Projectile projectile, PreyData digestedPrey);
		public DelegateOnDigestionKill OnDigestionKill { get; set; }

		public delegate void DelegateGetDigestedPlayerAdditionalDeathMessages(Projectile projectile, Player player, List<string> deathMessageKeyList);
		public DelegateGetDigestedPlayerAdditionalDeathMessages GetAdditionalDigestedPlayerMessages { get; set; }

		public delegate double DelegateGetPreyAbsorptionRate(Projectile projectile);
		public DelegateGetPreyAbsorptionRate GetPreyAbsorptionRate { get; set; }

		public delegate int DelegateGetVisualBellySize(Projectile projectile);
		public DelegateGetVisualBellySize GetVisualBellySize { get; set; }

		public delegate int DelegateGetVisualWeightStage(Projectile projectile);
		public DelegateGetVisualWeightStage GetVisualWeightStage { get; set; }

		public SlotId ActiveStomachNoises { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => true;

		public PredProjectile()
		{
			MaxStomachCapacity = 1.0;
			MaxSwallowRange = 36f;
			ExtraWeight = 0.0;
			CanSwallowBosses = false;

			GetDigestionTickRate = null;
			GetDigestionTickDamage = null;
			GetPreyAbsorptionRate = null;

			NonPreferenceBypass = false;
			CanBeForceFed = (Projectile projectile) => false;
			OnForceFed = null;

			Stomachache = 0;
			BaseStomachacheMeterCapacity = 100.0;
			StomachacheMeterCapacityModifier = StatModifier.Default;
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
			BurpPitchOffset = 0f;

			OnDigestionKill = null;
			GetAdditionalDigestedPlayerMessages = null;

			GetVisualBellySize = null;
			GetVisualWeightStage = null;
		}

		public static void ResetEffects(Projectile projectile)
		{
			double stomachacheQuellPerTick = projectile.AsPred().StomachacheMeterCapacity * (0.05 / (double)V2Utils.SensibleTime(seconds: 1));
			if (GetStomachTracker(projectile) is null || !AnyPreyStillAlive(projectile))
				stomachacheQuellPerTick *= 0.1;
			projectile.AsPred().Stomachache -= stomachacheQuellPerTick;

			projectile.AsPred().StomachacheMeterCapacityModifier = StatModifier.Default;
		}

		public static bool CanSwallow(Projectile pred, Entity prey)
		{
			if (V2.VoreNPCBlacklist is not null && V2.VoreProjectileBlacklist.Count > 0 && V2.VoreProjectileBlacklist.Contains(pred.type))
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
						if (pred.AsV2Proj().Gender == EntityGender.Male)
							return false;
						break;
					case "No Female":
						if (pred.AsV2Proj().Gender == EntityGender.Female)
							return false;
						break;
					case "No M or F...but why?":
						if (pred.AsV2Proj().Gender != EntityGender.Other)
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
				if (!pred.AsPred().CanSwallowBosses && isThePreyAFuckingBoss)
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
		/// Causes the given predator projectile to swallow the given prey entity, if the given prey entity can be swallowed.
		/// </summary>
		/// <param name="pred">The predator which will attempt to swallow the given prey.</param>
		/// <param name="prey">The prey which will be attempt to be swallowed by the given predator.</param>
		public static void Swallow(Projectile pred, Entity prey, int MPstate = 0, int MPwhoAmI = -1)
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
					npc.AsFood().OnSwallowedBy?.Invoke(npc, pred);
					break;
				case PreyType.Projectile:
					Projectile projectile = prey as Projectile;
					projectile.AsFood().OnSwallowedBy?.Invoke(projectile, pred);
					break;
				case PreyType.Item:
					Item item = prey as Item;
					item.AsFood().OnSwallow?.Invoke(item, pred);
					if (item.AsFood().OnSwallowDamage > 0)
						pred.AsFood().Health -= item.AsFood().OnSwallowDamage;
					break;
			}
			pred.netUpdate = true;

			if (MPstate == 1)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.RequestSwallowPrey);
				packet.Write((byte)2);
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
				packet.Write((byte)2);
				packet.Write(pred.whoAmI);
				packet.Write((byte)food.Type);
				packet.Write(prey.whoAmI);
				packet.Write(MPwhoAmI);
				packet.Send(-1, ignoreClient: MPwhoAmI);
			}
		}

		public static void SwallowWithTextIfApplicable(Projectile pred, Player prey, string chatboxText)
		{
			if (!CanSwallow(pred, prey))
				return;

			Swallow(pred, prey);
			SetChatboxText(pred, prey, chatboxText);
		}

		public static void SetChatboxText(Projectile pred, Player prey, string chatText)
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
		/// <param name="pred">The projectile to update all food in the stomach of.</param>
		public static void UpdatePrey(Projectile pred)
		{
			if (pred.AsPred().StomachacheMeterCapacity > 0 && pred.AsPred().Stomachache >= pred.AsPred().StomachacheMeterCapacity)
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
				PlayDigestionBelch(pred, null);
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

					if (pred.AsPred().GetDigestionTickRate is null || pred.AsPred().GetDigestionTickDamage is null)
					{
						if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
							Main.NewText(pred.Name + " has invalid digestion damage/tick rate methods!");
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
								if (shouldDigestPlayer)
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
						(V2.GetFooled
							? StomachNoises.AprilFools
							: StomachNoises.Muffled) with
						{ Volume = 0.25f + (0.15f * pred.AsPred().GetVisualBellySize.Invoke(pred)) },
						pred.TrueCenter()
					);
					SoundEngine.TryGetActiveSound(pred.AsPred().ActiveStomachNoises, out stomachNoises);
				}

				if (stomachNoises is null)
					return;

				stomachNoises.Position = pred.TrueCenter();
				stomachNoises.Volume = 0.25f;
				stomachNoises.Volume += 0.15f * pred.AsPred().GetVisualBellySize.Invoke(pred);
			}
		}

		public static void AddNewPrey(Projectile pred, PreyData prey)
		{
			if (GetStomachTracker(pred) is null)
				VoreTracker.NewTracker(pred, new List<PreyData>() { prey });
			else
				GetStomachTracker(pred).QueueNewPrey(prey);
		}

		public static string GetDigestedPlayerDeathReason(Projectile projectile, Player player)
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
			switch (projectile.AsPred().DigestionType)
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

			if (projectile.AsPred().GetAdditionalDigestedPlayerMessages is not null)
				projectile.AsPred().GetAdditionalDigestedPlayerMessages.Invoke(projectile, player, deathMessageKeyList);
			string finalDeathReasonKey = Main.rand.NextFromCollection(deathMessageKeyList);

			return Language.GetTextValueWith(
				finalDeathReasonKey,
				new
				{
					Player = player.name,
					Pred = projectile.Name
				}
			);
		}

		public override bool PreKill(Projectile projectile, int timeLeft)
		{
			if (projectile.CurrentCaptor() is not null && GetStomachTracker(projectile) is not null)
			{
				foreach (PreyData prey in GetStomachTracker(projectile).Prey)
				{
					projectile.CurrentCaptor().QueueNewPrey(prey);
				}
			}
			return true;
		}

		/// <summary>
		/// Calculates the current weight of the given predator's stomach, based on all the prey inside of it.<br/>
		/// Used primarily in conjunction with <see cref="MaxStomachCapacity"/> to safeguard against overeating.<br/>
		/// </summary>
		/// <param name="pred">The predator whose stomach is to be weighed.</param>
		/// <returns>The current total weight of the given predator's stomach.</returns>
		public static double GetCurrentBellyWeight(Projectile pred)
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
						totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
						break;
					case PreyType.Projectile:
						Projectile preyPredProjectile = prey.Instance as Projectile;
						totalBellyWeight += GetCurrentBellyWeight(preyPredProjectile);
						break;
				}
			}
			return totalBellyWeight;
		}

		public static bool AnyPreyStillAlive(Projectile pred)
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

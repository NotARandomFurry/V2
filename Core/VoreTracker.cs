using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core.StruggleSystem;
using V2.Items;
using V2.Items.Voraria.TransformationItems.Baelz;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Projectiles;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace V2.Core
{
	/// <summary>
	/// Used to define what type of pred this vore tracker is for.
	/// </summary>
	public enum PredType
	{
		Player,
		NPC,
		Projectile,
		Item,
		Liquid,
		Custom,
		Undefined
	};

	/// <summary>
	/// Used to define what type of prey this is.
	/// </summary>
	public enum PreyType
	{
		Player,
		NPC,
		Projectile,
		Item,
		Liquid,
		Custom,
		Undefined
	};

	public class VoreTracker
	{
		public static double MaximumNoteProximityRatio => 0.10;
		public double StruggleChartProgressRate { get; set; }
		public double StruggleChartProgress { get; set; }

		public Entity Predator { get; internal set; }
		public PredType PredatorType {
			get
			{
				if (Predator is Player)
					return PredType.Player;
				if (Predator is NPC)
					return PredType.NPC;
				if (Predator is Projectile)
					return PredType.Projectile;

				return PredType.Undefined;
			}
		}

		public List<PreyData> Prey { get; internal set; }
		public List<PreyData> PreyQueue { get; internal set; }
		public StruggleChart PredatorStruggleChart { get; internal set; }

		public static void NewTracker(Entity pred, List<PreyData> prey)
		{
			VoreTracker tracker = new VoreTracker();
	
			tracker.Predator = pred;

			tracker.Prey = prey;
			tracker.PreyQueue = [];
			ModContent.GetInstance<V2MasterSystem>().VoreTrackers.Add(tracker);
			if (Main.netMode == NetmodeID.SinglePlayer)
				tracker.RefreshStruggleChartList();
		}

		public void QueueNewPrey(PreyData prey) => PreyQueue.Add(prey);

		public void RefreshStruggleChartList()
		{
			if (Prey.FirstOrDefault(x => !x.NoHealth && x.Type != PreyType.Item) is null)
				return;

			StruggleChartProgress = -2.0;
			StruggleChartProgressRate = 1.0;
			PredatorStruggleChart = null;
			if (Predator is Player predPlayer)
			{
				StruggleChartProgress = -predPlayer.AsPred().StruggleGraceTime;
			}
			else if (Predator is NPC predNPC)
			{
				PredatorStruggleChart = predNPC.AsPred().AssociatedStruggleChart;
				StruggleChartProgressRate = PredatorStruggleChart.ProgressRate;
			}
			if (PredatorStruggleChart is not null)
			{
				PredatorStruggleChart.ConnectedTracker = this;
				PredatorStruggleChart.ForPredator = true;
				PredatorStruggleChart.OnStartup();
			}
			foreach (PreyData prey in Prey)
			{
				if (prey.NoHealth)
				{
					prey.AssignedStruggleChart = null;
					continue;
				}
				else if (PredatorStruggleChart is not null)
				{
					prey.AssignedStruggleChart = PredatorStruggleChart;
					prey.AssignedStruggleChart.ConnectedTracker = this;
					prey.AssignedStruggleChart.ForPredator = false;
					prey.AssignedStruggleChart.OnStartup();
				}
			}
		}

		public void UpdateProgress()
		{
			if (PredatorStruggleChart is null)
				StruggleChartProgress = -1.0;
			else
			{
				StruggleChartProgress += StruggleChartProgressRate / V2Utils.SensibleTime(seconds: 1);
				StruggleChartProgress %= PredatorStruggleChart.Notes.Count;
			}
		}

		public void UpdatePrey()
		{
			if (!Predator.active)
				return;
			if (PreyQueue.Count > 0)
			{
				Prey = [.. Prey, .. PreyQueue];
				PreyQueue.Clear();
				if (Main.netMode == NetmodeID.SinglePlayer)
					RefreshStruggleChartList();
			}

			if (Prey.FirstOrDefault(x => !x.NoHealth) is null)
				PredatorStruggleChart = null;

			Prey.RemoveAll(x => x.NoHealth && x.WeightLeftToDigest <= 0.0);
			if (Prey.Count <= 0)
				return;

			foreach (PreyData prey in Prey)
			{
				prey.timeSpentInStomach++;
			}

			if (Predator is Player predPlayer)
				PredPlayer.UpdatePrey(predPlayer);
			else if (Predator is NPC predNPC)
				PredNPC.UpdatePrey(predNPC);
			else if (Predator is Projectile predProjectile)
				PredProjectile.UpdatePrey(predProjectile);
		}

		/// <summary>
		/// Checks ALL inputs tracked by the given vore tracker.<br/>
		/// <b>This is ONLY to be called in singleplayer; if called outside of singleplayer, it will not run, as there is (currently) no struggle system support in multiplayer.</b><br/>
		/// </summary>
		public void HandleStruggleSystem()
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
				return;

			if (PredatorStruggleChart is null)
				return;

			PredatorStruggleChart.RefreshPressedNotes();
			foreach (PreyData prey in Prey)
			{
				if (!prey.NoHealth && prey.AssignedStruggleChart is not null)
					prey.AssignedStruggleChart.RefreshPressedNotes();
			}

			List<(StruggleChartNote note, double proximity)> closeNotes = CheckCloseNotes(-1);

			double GetParticipantStruggleDamage(int index, double proximity)
			{
				if (Prey?.Count <= 0 || index >= Prey.Count)
					return 0.0;

				if (index == -1)
				{
					if (Predator is Player playerPred)
						return 10.0; // tie to TUM scalin' later
				}
				else if (!Prey[index].NoHealth && Prey[index].Instance is not null)
				{
					Entity prey = Prey[index].Instance;
					if (prey is Player preyPlayer)
						return preyPlayer.AsFood().StruggleDamage;
				}
				return 0.0;
			}

			bool TryPressNote(NoteDirection direction, int index)
			{
				bool pred = index == -1;
				if (closeNotes.FirstOrDefault(x => x.note.Direction == direction) is (StruggleChartNote note, double proximity) noteData)
				{
					double absoluteProximity = Math.Abs(noteData.proximity);
					double timingModifier = absoluteProximity switch
					{
						double i when i <= MaximumNoteProximityRatio * StruggleChartProgressRate * 0.20 => 1.20,
						double i when i <= MaximumNoteProximityRatio * StruggleChartProgressRate * 0.40 => 1.00,
						double i when i <= MaximumNoteProximityRatio * StruggleChartProgressRate * 0.70 => 0.75,
						double i when i <= MaximumNoteProximityRatio * StruggleChartProgressRate * 1.00 => 0.50,
						_ => 0.00,
					};
					if (timingModifier > 0)
					{
						ModifyPredStomachacheMeter(-pred.ToDirectionInt() * GetParticipantStruggleDamage(index, absoluteProximity) * timingModifier);
						SignifyNotePressed(noteData);
						return true;
					}
				}

				ModifyPredStomachacheMeter(pred.ToDirectionInt() * 1.0);
				return false;
			}

			static void SignifyNotePressed((StruggleChartNote note, double proximity) noteData)
			{
				noteData.note.CorrectlyPressed = true;
				noteData.note.PressedPosition = noteData.proximity;
			}
			if (Predator is Player playerPred)
			{
				if (V2.StruggleUpHotkey.JustPressed) TryPressNote(NoteDirection.Up, -1);
				if (V2.StruggleDownHotkey.JustPressed) TryPressNote(NoteDirection.Down, -1);
				if (V2.StruggleLeftHotkey.JustPressed) TryPressNote(NoteDirection.Left, -1);
				if (V2.StruggleRightHotkey.JustPressed) TryPressNote(NoteDirection.Right, -1);
				if (V2.StruggleSpecialHotkey.JustPressed) TryPressNote(NoteDirection.Special, -1);
			}
			else if (Predator is NPC npcPredator)
			{
				int counterSkill = npcPredator.AsPred().CounterStruggleEffectiveness;
				if (closeNotes is null)
					goto HandlePreyNotes;
				foreach ((StruggleChartNote note, double proximity) noteData in closeNotes)
				{
					double absoluteProximity = Math.Abs(noteData.proximity);
					double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
					switch (counterSkill)
					{
						case 0:
							break;
						case 1:
							break;
						case 2:
						case 3:
						case 4:
							break;
						case 5:
						default:
							break;
						case 6:
						case 7:
						case 8:
						case 9:
						case 10:
						case 11:
						case 12:
							break;
					}
				}
			}
			else if (Predator is Projectile projectilePredator)
			{
				int counterSkill = projectilePredator.AsPred().CounterStruggleEffectiveness;
				if (closeNotes is null)
					goto HandlePreyNotes;
				foreach ((StruggleChartNote note, double proximity) noteData in closeNotes)
				{
					double absoluteProximity = Math.Abs(noteData.proximity);
					double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
					switch (counterSkill)
					{
						case 0:
							break;
						case 1:
							break;
						case 2:
						case 3:
						case 4:
							break;
						case 5:
						default:
							break;
						case 6:
						case 7:
						case 8:
						case 9:
						case 10:
						case 11:
						case 12:
							break;
					}
				}
			}

			HandlePreyNotes:
			for (int i = 0; i < Prey.Count; i++)
			{
				PreyData prey = Prey[i];
				if (prey.NoHealth || prey.AssignedStruggleChart is null)
					continue;

				closeNotes = CheckCloseNotes(i);
				Entity preyEntity = prey.Instance;
				if (preyEntity is Player playerPrey)
				{
					if (V2.StruggleUpHotkey.JustPressed) TryPressNote(NoteDirection.Up, i);
					if (V2.StruggleDownHotkey.JustPressed) TryPressNote(NoteDirection.Down, i);
					if (V2.StruggleLeftHotkey.JustPressed) TryPressNote(NoteDirection.Left, i);
					if (V2.StruggleRightHotkey.JustPressed) TryPressNote(NoteDirection.Right, i);
					if (V2.StruggleSpecialHotkey.JustPressed) TryPressNote(NoteDirection.Special, i);
				}
			}
		}

		public void ModifyPredStomachacheMeter(double amount)
		{
			switch (PredatorType)
			{
				case PredType.Player:
					Player player = Predator as Player;
					amount -= player.AsPred().StomachacheDefense.Base;
					amount /= player.AsPred().StomachacheDefense.Additive;
					amount /= player.AsPred().StomachacheDefense.Multiplicative;
					amount -= player.AsPred().StomachacheDefense.Flat;
					player.AsPred().Stomachache += amount;
					break;
				case PredType.NPC:
					NPC npc = Predator as NPC;
					npc.AsPred().Stomachache += amount;
					break;
				case PredType.Projectile:
					Projectile projectile = Predator as Projectile;
					projectile.AsPred().Stomachache += amount;
					break;
			}
		}

		public List<(StruggleChartNote note, double proximity)> CheckCloseNotes(int preyIndex, bool forUI = false)
		{
			List<(StruggleChartNote note, double proximity)> closeNotes = [];
			StruggleChart targetChart = PredatorStruggleChart;
			if (preyIndex != -1)
			{
				if (Prey[preyIndex].NoHealth)
					return null;

				targetChart = Prey[preyIndex].AssignedStruggleChart;
			}

			if (targetChart is null)
				return null;

			if (targetChart.Notes is null)
				return null;

			for (int noteSetIndex = (int)Math.Max(0, Math.Round(StruggleChartProgress) - 3); noteSetIndex <= Math.Round(StruggleChartProgress) + 6; noteSetIndex++)
			{
				StruggleChartNote[] noteSet = noteSetIndex >= targetChart.Notes.Count ? targetChart.Notes[noteSetIndex % targetChart.Notes.Count] : targetChart.Notes[noteSetIndex];

				if (noteSet is null)
					continue;

				if (noteSet.FirstOrDefault(x => x is not null) is null)
					continue;

				for (int noteIndex = 0; noteIndex < noteSet.Length; noteIndex++)
				{
					StruggleChartNote note = noteSet[noteIndex];
					if (note is null)
						continue;

					if (note.CorrectlyPressed && !forUI)
						continue;

					closeNotes.Add((note, (double)noteSetIndex - StruggleChartProgress));
				}
			}

			if (!forUI)
			{
				closeNotes.RemoveAll(x => x.proximity >= StruggleChartProgressRate * MaximumNoteProximityRatio);
				closeNotes.RemoveAll(x => x.proximity <= -StruggleChartProgressRate * MaximumNoteProximityRatio);
			}
			return closeNotes;
		}

		public bool CheckClearability()
		{
			if (Prey is null)
				return true;
			if (Prey.Count <= 0)
				return true;

			if (!Predator.active)
				return true;

			if (Predator is Player playerPred)
			{
				if (playerPred.dead)
					return true;
			}
			else if (Predator is NPC NPCPred)
			{
				if (NPCPred.life <= 0)
					return true;
			}
			else if (Predator is Projectile projectilePred)
			{
				if (projectilePred.AsFood().Health <= 0)
					return true;
			}

			return false;
		}
	}

	public class PreyData
	{
		public PreyType Type { get; set; }
		public Entity Instance { get; set; }
		public int ExactType { get; set; }
		public string Name { get; set; }
		public bool NoHealth { get; set; }
		public bool InventoryItem { get; set; }
        public bool CannotBeRegurgitated { get; set; }
        public double InitialWeight { get; set; }
		public double InitialSize { get; set; }
		public double WeightLeftToDigest { get; set; }
		public double CalorieMultiplier { get; set; }
		public double WellFedPower { get; set; }
		public double SizeLeftToDigest => WeightLeftToDigest / InitialWeight * InitialSize;
		public StruggleChart AssignedStruggleChart { get; set; }

		public int timeSpentInStomach;
		public delegate void DelegateUpdateInStomach(Entity prey, Entity pred, bool dead);
		public DelegateUpdateInStomach UpdateInStomach { get; set; }

		public VoreTracker ConnectedTracker { get; set; }

		public PreyData()
		{
			Type = PreyType.Undefined;
		}

		/// <summary>
		/// Creates a new set of prey data based on the provided information.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="exactType"></param>
		/// <param name="weightRemainingIfDead"></param>
		/// <returns></returns>
		public static PreyData NewData(PreyType type, int exactType, string name, double weightRemainingIfDead = -1, double calmul = 1.0, double fedadd = 0.0, bool cantRegurg = false)
		{
			PreyData data = new PreyData();
			data.Type = type;
			data.ExactType = exactType;
			data.Name = name;
			data.CalorieMultiplier = calmul;
			data.WellFedPower = fedadd;
			data.CannotBeRegurgitated = cantRegurg;
			if (weightRemainingIfDead != -1)
			{
				data.WeightLeftToDigest = weightRemainingIfDead;
				data.NoHealth = true;
			}

			return data;
		}

		public static PreyData NewLiquidData(int liquidType, int liquidAmount)
		{
			PreyData data = new PreyData(liquidType, liquidAmount);
			data.Type = PreyType.Liquid;
			data.Instance = null;
			data.NoHealth = true;
			data.ExactType = liquidType;
			data.Name = liquidType switch
			{
				LiquidID.Water => "Water",
				LiquidID.Lava => "Lava",
				LiquidID.Honey => "Honey",
				LiquidID.Shimmer => "Shimmer",
				_ => "Some Other Liquid",
			};
			data.CalorieMultiplier = liquidType switch
			{
				LiquidID.Water => 0.1,
				LiquidID.Lava => 3,
				LiquidID.Honey => 1.5,
				LiquidID.Shimmer => -1.5,
				_ => 0.1,
			};
			data.WellFedPower = liquidType switch
			{
				LiquidID.Water => 0,
				LiquidID.Lava => 0.1,
				LiquidID.Honey => 0.3,
				LiquidID.Shimmer => -3,
				_ => 0,
			};
			data.WeightLeftToDigest = liquidAmount;
			return data;
		}

		public static PreyData NewLiquidData(int liquidType, double liquidAmount)
		{
			PreyData data = new PreyData();
			data.Type = PreyType.Liquid;
			data.Instance = null;
			data.NoHealth = true;
			data.ExactType = liquidType;
			data.Name = liquidType switch
			{
				LiquidID.Water => "Water",
				LiquidID.Lava => "Lava",
				LiquidID.Honey => "Honey",
				LiquidID.Shimmer => "Shimmer",
				_ => "Some Other Liquid",
			};
			data.CalorieMultiplier = liquidType switch
			{
				LiquidID.Water => 0.01,
				LiquidID.Lava => 3,
				LiquidID.Honey => 1.5,
				LiquidID.Shimmer => -3,
				_ => 0.1,
			};
			data.WellFedPower = liquidType switch
			{
				LiquidID.Water => 0,
				LiquidID.Lava => 0.1,
				LiquidID.Honey => 0.3,
				LiquidID.Shimmer => -3,
				_ => 0,
			};
			data.WeightLeftToDigest = liquidAmount;
			return data;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="preyEntity"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static PreyData NewData(Entity preyEntity, VoreTracker tracker = null)
		{
			if (preyEntity is Player preyPlayer)
			{
				PreyData data = NewData(
					type: PreyType.Player,
					exactType: 0,
					name: "Player " + preyPlayer.name,
					calmul: 1
				);
				data.Instance = preyPlayer;
				if (tracker is not null)
					data.ConnectedTracker = tracker;
				data.Recalculate();
				return data;
			}
			else if (preyEntity is NPC preyNPC)
			{
				PreyData data = NewData(
					type: PreyType.NPC,
					exactType: preyNPC.type,
					name: preyNPC.GivenOrTypeName,
					calmul: preyNPC.AsFood().CalorieMultiplier,
					fedadd: preyNPC.AsFood().WellFedPower,
					cantRegurg: preyNPC.AsFood().CannotBeRegurgitated
				);
				data.Instance = preyNPC;
				if (tracker is not null)
					data.ConnectedTracker = tracker;
				data.Recalculate();
				return data;
			}
			else if (preyEntity is Projectile preyProjectile)
			{
				PreyData data = NewData(
					type: PreyType.Projectile,
					exactType: preyProjectile.type,
					name: preyProjectile.Name,
					calmul: preyProjectile.AsFood().CalorieMultiplier,
					fedadd: preyProjectile.AsFood().WellFedPower
					calmul: preyProjectile.AsFood().CalorieMultiplier,
					fedadd: preyProjectile.AsFood().WellFedPower,
                    cantRegurg: preyProjectile.AsFood().CannotBeRegurgitated
                );
				data.Instance = preyProjectile;
				if (tracker is not null)
					data.ConnectedTracker = tracker;
				data.Recalculate();
				return data;
			}
			else if (preyEntity is Item preyItem)
			{
				PreyData data = NewData(
					type: PreyType.Item,
					exactType: preyItem.type,
					name: preyItem.AffixName(),
					calmul: preyItem.AsFood().CalorieMultiplier,
					fedadd: preyItem.AsFood().WellFedPower,
                    cantRegurg: preyItem.AsFood().CannotBeRegurgitated
                );
				data.Instance = preyItem;
				if (tracker is not null)
					data.ConnectedTracker = tracker;
				data.Recalculate();
				return data;
			}
			else
			{
				throw new Exception(
					"hi !!\n"
				  + "thomas says that the thing you asked to make is wrong\n"
				  + "something about a pa ram being an	 in valid enter tee?\n"
				  + "I asked if I could take what was left from him though and he said yeah\n"
				  + "give me and my tummy another snack whenever :D"
				  + "-rose"
				);
			}
		}

		/// <summary>
		/// Properly sets a vast number of fields for this prey data set based on the intended prey type and the provided instance (or lack thereof, if applicable).<br/>
		/// Generally intended to only be called on or shortly after creation of the data set, since calling it later on can cause the type to change unintentionally or render the set invalid.<br/>
		/// <br/>
		/// The data set is rendered invalid (and subsequently fed to Rose) if:<br/>
		/// - <see cref="Type"/> is still set to <see cref="PreyType.Undefined"/>.<br/>
		/// - <see cref="Type"/> is set to something that requires an instance (<see cref="PreyType.Player"/>, <see cref="PreyType.NPC"/>, <see cref="PreyType.Projectile"/>, <see cref="PreyType.Item"/>), and <see cref="Instance"/> is null.<br/>
		/// - <see cref="Type"/> is set to something that does not require an instance (<see cref="PreyType.Liquid"/>, <see cref="PreyType.Custom"/>), and <see cref="Instance"/> is NOT <see langword="null"/>.<br/>
		/// </summary>
		public void Recalculate()
		{
			double refPlayerWidth = 20.0;
			double refPlayerHeight = 42.0;
			switch (Type)
			{
				case PreyType.Player:
					if (Instance is null || Instance is not Player preyPlayer)
						break;

					ExactType = 0;
                    double playerToPlayerWidthRatio = (double)preyPlayer.width / refPlayerWidth;
                    double playerToPlayerHeightRatio = (double)preyPlayer.height / refPlayerHeight;
                    if (preyPlayer.AsV2Player().HasTransformation)
					{
						playerToPlayerWidthRatio = 1;
                        playerToPlayerHeightRatio = 1;
						if (preyPlayer.AsV2Player().BaeTransformation)
							playerToPlayerWidthRatio = BaelzInfo.BaseWeight;
                    }
					InitialWeight = InitialSize = WeightLeftToDigest = playerToPlayerWidthRatio * playerToPlayerHeightRatio;
					if (ConnectedTracker is not null)
					{
						AssignedStruggleChart = null; // new ProceduralStruggleChart();
						AssignedStruggleChart.ConnectedTracker = ConnectedTracker;
						AssignedStruggleChart.ForPredator = false;
						AssignedStruggleChart.OnStartup();
					}
					return;
				case PreyType.NPC:
					if (Instance is null || Instance is not NPC preyNPC)
						break;

					ExactType = preyNPC.netID;
					if (preyNPC.AsFood().DefinedEffectiveSize != 0)
						InitialWeight = InitialSize = WeightLeftToDigest = preyNPC.AsFood().DefinedEffectiveSize;
					else if (preyNPC.AsFood().DefinedBaseSize != 0)
						InitialWeight = InitialSize = WeightLeftToDigest = preyNPC.AsFood().DefinedBaseSize + preyNPC.AsPred().ExtraWeight;
					else
					{
						double playerToNPCWidthRatio = (double)preyNPC.width / refPlayerWidth;
						double playerToNPCHeightRatio = (double)preyNPC.height / refPlayerHeight;
						InitialWeight = InitialSize = WeightLeftToDigest = playerToNPCWidthRatio * playerToNPCHeightRatio;
					}
					if (ConnectedTracker is not null)
					{
						AssignedStruggleChart = null; // new ProceduralStruggleChart();
						AssignedStruggleChart.ConnectedTracker = ConnectedTracker;
						AssignedStruggleChart.ForPredator = false;
						AssignedStruggleChart.OnStartup();
					}
					CalorieMultiplier = preyNPC.AsFood().CalorieMultiplier;
					return;
				case PreyType.Projectile:
					if (Instance is null || Instance is not Projectile preyProjectile)
						break;

					ExactType = preyProjectile.type;
					if (preyProjectile.AsFood().DefinedSize != 0)
						InitialWeight = InitialSize = WeightLeftToDigest = preyProjectile.AsFood().DefinedSize + preyProjectile.AsPred().ExtraWeight;
					else
					{
						double playerToProjWidthRatio = (double)preyProjectile.width / refPlayerWidth;
						double playerToProjHeightRatio = (double)preyProjectile.height / refPlayerHeight;
						InitialWeight = InitialSize = WeightLeftToDigest = playerToProjWidthRatio * playerToProjHeightRatio;
					}
					if (ConnectedTracker is not null)
					{
						AssignedStruggleChart = null; // new ProceduralStruggleChart();
						AssignedStruggleChart.ConnectedTracker = ConnectedTracker;
						AssignedStruggleChart.ForPredator = false;
						AssignedStruggleChart.OnStartup();
					}
					CalorieMultiplier = preyProjectile.AsFood().CalorieMultiplier;
					return;
				case PreyType.Item:
					if (Instance is null || Instance is not Item preyItem)
						break;

					ExactType = preyItem.type;
					InitialWeight = InitialSize = WeightLeftToDigest = preyItem.CalculateSnackSize();
					UpdateInStomach += preyItem.AsFood().UpdateInStomach;
					CalorieMultiplier = preyItem.AsFood().CalorieMultiplier;
					return;
				case PreyType.Liquid:
					if (Instance is not null)
						Instance = null;

					return;
				case PreyType.Custom:
					if (Instance is not null)
						Instance = null;

					return;
			}

			throw new Exception(
				"hi !!\n"
			  + "thomas says that the thing you asked to re\n"
			  + "uh	recal			  reclam\n"
			  + "umm		   re   calendar ?\n"
			  + "anyway i thought it was yummy but thomas didnt like it\n"
			  + "send me more cool snacks please :D"
			  + "-rose"
			);
		}

        public PreyData(int liquidType, int liquidAmount)
        {
            double liquidAmountReal = liquidAmount / 256.0 * (liquidType switch
            {
                LiquidID.Lava => 4.0,
                LiquidID.Honey => 1.5,
                LiquidID.Shimmer => 0.75,
                _ => 1.0,
            });
            Type = PreyType.Liquid;
            Instance = null;
            NoHealth = true;
            ExactType = liquidType;
            Name = liquidType switch
            {
                LiquidID.Water => "Water",
                LiquidID.Lava => "Lava",
                LiquidID.Honey => "Honey",
                LiquidID.Shimmer => "Shimmer",
                _ => "Some Other Liquid",
            };
            CalorieMultiplier = liquidType switch
            {
                LiquidID.Water => 0.01,
                LiquidID.Lava => 3,
                LiquidID.Honey => 1.5,
                LiquidID.Shimmer => -3,
                _ => 0.1,
            };
            WellFedPower = liquidType switch
            {
                LiquidID.Water => 0,
                LiquidID.Lava => 0.1,
                LiquidID.Honey => 0.3,
                LiquidID.Shimmer => -3,
                _ => 0,
            };
            InitialWeight = InitialSize = WeightLeftToDigest = liquidAmountReal;
        }

        public PreyData(int liquidType, double liquidAmount)
        {
            Type = PreyType.Liquid;
            Instance = null;
            NoHealth = true;
            ExactType = liquidType;
            Name = liquidType switch
            {
                LiquidID.Water => "Water",
                LiquidID.Lava => "Lava",
                LiquidID.Honey => "Honey",
                LiquidID.Shimmer => "Shimmer",
                _ => "Some Other Liquid",
            };
            CalorieMultiplier = liquidType switch
            {
                LiquidID.Water => 0.01,
                LiquidID.Lava => 3,
                LiquidID.Honey => 1.5,
                LiquidID.Shimmer => -3,
                _ => 0.1,
            };
            WellFedPower = liquidType switch
            {
                LiquidID.Water => 0,
                LiquidID.Lava => 0.1,
                LiquidID.Honey => 0.3,
                LiquidID.Shimmer => -3,
                _ => 0,
            };
            InitialWeight = InitialSize = WeightLeftToDigest = liquidAmount;
        }

        /// <summary>
        /// Allows you to check what the size of something as a snack would be by creating a new dummy <see cref="PreyData"/> for a few moments.<br/>
        /// Accounts for anything that might be in the given snack's belly.<br/>
        /// </summary>
        /// <param name="preyEntity">The snack-size entity to check the size of.</param>
        /// <returns>The size of the given soon-to-be stomach fodder.</returns>
        public static double GetPreySize(Entity preyEntity)
        {
            double initialSize = NewData(preyEntity).InitialSize;
            if (preyEntity is Player preyPlayer)
            {
                double actualSize = preyPlayer.AsPred().StomachFullness;
                if (preyPlayer.AsV2Player().BaeTransformation)
                    actualSize += preyPlayer.AsPred().BaeTransformation_ExtraWeight;
                else if (preyPlayer.AsV2Player().KroniiTransformation)
                    actualSize += preyPlayer.AsPred().KroniiTransformation_ExtraWeight;
                else if (preyPlayer.AsV2Player().OllieTransformation)
                    actualSize += preyPlayer.AsPred().OllieTransformation_ExtraWeight;
                else if (preyPlayer.AsV2Player().SoraTransformation)
                    actualSize += preyPlayer.AsPred().SoraTransformation_ExtraWeight;
                else if (preyPlayer.AsV2Player().MintTransformation)
                    actualSize += preyPlayer.AsPred().MintTransformation_ExtraWeight;
                return initialSize + actualSize;
            }
            if (preyEntity is NPC preyNPC)
                return initialSize + preyNPC.AsPred().ExtraWeight + PredNPC.GetCurrentBellyWeight(preyNPC);
            if (preyEntity is Projectile preyProjectile)
                return initialSize + preyProjectile.AsPred().ExtraWeight + PredProjectile.GetCurrentBellyWeight(preyProjectile);

            return initialSize;
        }
	}
}

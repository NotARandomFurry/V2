using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core.StruggleSystem;
using V2.Items;
using V2.NPCs;
using V2.PlayerHandling;

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

	/// <summary>
	/// Used to store a reference to whatever's eaten a given prey entity.
	/// </summary>
	public struct PredEntityReference
	{
		public Entity Predator { get; set; }
		public PreyData PreyInstance { get; set; }
	}

	public class VoreTracker
	{
		public static double MaximumNoteProximityRatio => 5.0;
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
			tracker.PreyQueue = new List<PreyData>();
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
			if (Predator is Player predPlayer)
				StruggleChartProgress = -predPlayer.AsPred().StruggleGraceTime;
			StruggleChartProgressRate = 1.75 / (double)V2Utils.SensibleTime(seconds: 1);
			PredatorStruggleChart = new ProceduralStruggleChart();
			PredatorStruggleChart.ConnectedTracker = this;
			PredatorStruggleChart.ForPredator = true;
			PredatorStruggleChart.OnStartup();
			foreach (PreyData prey in Prey)
			{
				if (prey.NoHealth)
				{
					prey.AssignedStruggleChart = null;
					continue;
				}
				else
				{
					switch (prey.Type)
					{
						case PreyType.Player:
							prey.AssignedStruggleChart = new ProceduralStruggleChart();
							prey.AssignedStruggleChart.ConnectedTracker = this;
							prey.AssignedStruggleChart.ForPredator = false;
							prey.AssignedStruggleChart.OnStartup();
							break;
						case PreyType.NPC:
							prey.AssignedStruggleChart = new ProceduralStruggleChart();
							prey.AssignedStruggleChart.ConnectedTracker = this;
							prey.AssignedStruggleChart.ForPredator = false;
							prey.AssignedStruggleChart.OnStartup();
							break;
						case PreyType.Projectile:
							prey.AssignedStruggleChart = new ProceduralStruggleChart();
							prey.AssignedStruggleChart.ConnectedTracker = this;
							prey.AssignedStruggleChart.ForPredator = false;
							prey.AssignedStruggleChart.OnStartup();
							break;
					}
					continue;
				}
			}
		}

		public void UpdateProgress()
		{
			if (PredatorStruggleChart is null)
				StruggleChartProgress = -1.0;
			else
			{
				StruggleChartProgress += StruggleChartProgressRate;
				if (StruggleChartProgress > (double)PredatorStruggleChart.Notes.Count + 2.0)
					StruggleChartProgress -= (double)PredatorStruggleChart.Notes.Count + 4.0;
			}
		}

		public void UpdatePrey()
		{
			if (PreyQueue.Count > 0)
			{
				Prey = Prey.Concat(PreyQueue).ToList();
				PreyQueue.Clear();
				if (Main.netMode == NetmodeID.SinglePlayer)
					RefreshStruggleChartList();
			}

			if (Prey.FirstOrDefault(x => !x.NoHealth) is null)
				PredatorStruggleChart = null;

			Prey.RemoveAll(x => x.NoHealth && x.WeightLeftToDigest <= 0.0);
			if (Predator is Player predPlayer)
				PredPlayer.UpdatePrey(predPlayer);
			else if (Predator is NPC predNPC)
				PredNPC.UpdatePrey(predNPC);
			else if (Predator is Projectile predProjectile)
				return;
		}

		public int TotalPreySTR
		{
			get
			{
				int STR = 0;
				if (Prey is not null && Prey.Count > 0)
				{
					foreach (PreyData prey in Prey)
					{
						if (prey is null)
							continue;

						if (prey.NoHealth || prey.Instance is null)
							continue;

						switch (prey.Type)
						{
							case PreyType.Player:
								STR += (prey.Instance as Player).AsFood().STR.Total;
								break;
							case PreyType.NPC:
								STR += (prey.Instance as NPC).AsFood().STR;
								break;
						}
					}
				}
				if (PreyQueue is not null && PreyQueue.Count > 0)
				{
					foreach (PreyData prey in PreyQueue)
					{
						if (prey is null)
							continue;

						if (prey.NoHealth || prey.Instance is null)
							continue;

						switch (prey.Type)
						{
							case PreyType.Player:
								STR += (prey.Instance as Player).AsFood().STR.Total;
								break;
							case PreyType.NPC:
								STR += (prey.Instance as NPC).AsFood().STR;
								break;
						}
					}
				}
				return STR;
			}
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

			static void SignifyNotePressed((StruggleChartNote note, double proximity) noteData)
			{
				noteData.note.CorrectlyPressed = true;
				noteData.note.PressedPosition = noteData.proximity;
			}

			List<(StruggleChartNote note, double proximity)> closeNotes = CheckCloseNotes(-1);
			if (Predator is Player playerPredator)
			{
				if (V2.StruggleUpHotkey.JustPressed)
				{
					if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Up) is (StruggleChartNote note, double proximity) noteData)
					{
						double absoluteProximity = Math.Abs(noteData.proximity);
						double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
						ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
						SignifyNotePressed(noteData);
					}
					else
					{
						ModifyPredStomachacheMeter(1.0);
					}
				}
				if (V2.StruggleDownHotkey.JustPressed)
				{
					if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Down) is (StruggleChartNote note, double proximity) noteData)
					{
						double absoluteProximity = Math.Abs(noteData.proximity);
						double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
						ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
						SignifyNotePressed(noteData);
					}
					else
					{
						ModifyPredStomachacheMeter(1.0);
					}
				}
				if (V2.StruggleLeftHotkey.JustPressed)
				{
					if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Left) is (StruggleChartNote note, double proximity) noteData)
					{
						double absoluteProximity = Math.Abs(noteData.proximity);
						double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
						ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
						SignifyNotePressed(noteData);
					}
					else
					{
						ModifyPredStomachacheMeter(1.0);
					}
				}
				if (V2.StruggleRightHotkey.JustPressed)
				{
					if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Right) is (StruggleChartNote note, double proximity) noteData)
					{
						double absoluteProximity = Math.Abs(noteData.proximity);
						double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
						ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
						SignifyNotePressed(noteData);
					}
					else
					{
						ModifyPredStomachacheMeter(1.0);
					}
				}
				if (V2.StruggleSpecialHotkey.JustPressed)
				{
					if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Special) is (StruggleChartNote note, double proximity) noteData)
					{
						double absoluteProximity = Math.Abs(noteData.proximity);
						double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
						ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
						SignifyNotePressed(noteData);
					}
					else
					{
						ModifyPredStomachacheMeter(1.0);
					}
				}
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
							if (absoluteProximity > MaximumNoteProximityRatio)
								break;

							if (Main.rand.NextBool(35) && Math.Abs(noteData.proximity) < MaximumNoteProximityRatio && !noteData.note.Failed)
							{
								ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
								SignifyNotePressed(noteData);
							}
							else
							{
								ModifyPredStomachacheMeter(0.4);
								noteData.note.Failed = true;
							}
							break;
						case 2:
						case 3:
						case 4:
							break;
						case 5:
						default:
							if (absoluteProximity >= MaximumNoteProximityRatio)
								break;

							if (absoluteProximity > 4.0)
							{
								if (Main.rand.NextBool(20))
								{
									ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
									SignifyNotePressed(noteData);
								}
							}
							if (4.0 >= absoluteProximity && absoluteProximity > 3.0)
							{
								if (Main.rand.NextBool(15))
								{
									ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
									SignifyNotePressed(noteData);
								}
							}
							if (3.0 >= absoluteProximity && absoluteProximity > 2.0)
							{
								if (Main.rand.NextBool(12))
								{
									ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
									SignifyNotePressed(noteData);
								}
							}
							if (2.0 >= absoluteProximity && absoluteProximity > 1.0)
							{
								if (Main.rand.NextBool(10))
								{
									ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
									SignifyNotePressed(noteData);
								}
							}
							if (1.0 >= absoluteProximity)
							{
								if (Main.rand.NextBool(8))
								{
									ModifyPredStomachacheMeter(-(TotalPreySTR * 0.96) * proximityEffectivenessMultiplier / PredatorStruggleChart.DifficultyCoeff);
									SignifyNotePressed(noteData);
								}
							}
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

			bool predEmpress = PredatorType == PredType.NPC && (Predator as NPC).type == NPCID.HallowBoss;
			for (int i = 0; i < Prey.Count; i++)
			{
				PreyData prey = Prey[i];
				if (prey.NoHealth || prey.AssignedStruggleChart is null)
					continue;

				closeNotes = CheckCloseNotes(i);
				Entity preyEntity = prey.Instance;
				if (preyEntity is Player playerPrey)
				{
					if (V2.StruggleUpHotkey.JustPressed)
					{
						if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Up) is (StruggleChartNote note, double proximity) noteData)
						{
							double absoluteProximity = Math.Abs(noteData.proximity);
							double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
							if (predEmpress)
								proximityEffectivenessMultiplier *= 500.0;
							ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
							SignifyNotePressed(noteData);
						}
						else
						{
							ModifyPredStomachacheMeter(-0.9);
						}
					}
					if (V2.StruggleDownHotkey.JustPressed)
					{
						if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Down) is (StruggleChartNote note, double proximity) noteData)
						{
							double absoluteProximity = Math.Abs(noteData.proximity);
							double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
							if (predEmpress)
								proximityEffectivenessMultiplier *= 500.0;
							ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
							SignifyNotePressed(noteData);
						}
						else
						{
							ModifyPredStomachacheMeter(-0.9);
						}
					}
					if (V2.StruggleLeftHotkey.JustPressed)
					{
						if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Left) is (StruggleChartNote note, double proximity) noteData)
						{
							double absoluteProximity = Math.Abs(noteData.proximity);
							double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
							if (predEmpress)
								proximityEffectivenessMultiplier *= 500.0;
							ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
							SignifyNotePressed(noteData);
						}
						else
						{
							ModifyPredStomachacheMeter(-0.9);
						}
					}
					if (V2.StruggleRightHotkey.JustPressed)
					{
						if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Right) is (StruggleChartNote note, double proximity) noteData)
						{
							double absoluteProximity = Math.Abs(noteData.proximity);
							double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
							if (predEmpress)
								proximityEffectivenessMultiplier *= 500.0;
							ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
							SignifyNotePressed(noteData);
						}
						else
						{
							ModifyPredStomachacheMeter(-0.9);
						}
					}
					if (V2.StruggleSpecialHotkey.JustPressed)
					{
						if (closeNotes.FirstOrDefault(x => x.note.Lane == NoteLane.Special) is (StruggleChartNote note, double proximity) noteData)
						{
							double absoluteProximity = Math.Abs(noteData.proximity);
							double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
							if (predEmpress)
								proximityEffectivenessMultiplier *= 500.0;
							ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
							SignifyNotePressed(noteData);
						}
						else
						{
							ModifyPredStomachacheMeter(-0.9);
						}
					}
				}
				else if (preyEntity is NPC npcPrey)
				{
					int struggleSkill = npcPrey.AsFood().StruggleEffectiveness;
					if (closeNotes is null)
						continue;

					bool preyEmpress = npcPrey.type == NPCID.HallowBoss;

					foreach ((StruggleChartNote note, double proximity) noteData in closeNotes)
					{
						double absoluteProximity = Math.Abs(noteData.proximity);
						double proximityEffectivenessMultiplier = (MaximumNoteProximityRatio - absoluteProximity) / MaximumNoteProximityRatio;
						if (preyEmpress)
							proximityEffectivenessMultiplier /= 500.0;
						if (predEmpress)
							proximityEffectivenessMultiplier *= 500.0;
						switch (struggleSkill)
						{
							case 0:
								break;
							case 1:
								if (absoluteProximity > MaximumNoteProximityRatio)
									break;

								if (Main.rand.NextBool(35) && Math.Abs(noteData.proximity) < MaximumNoteProximityRatio && !noteData.note.Failed)
								{
									ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
									SignifyNotePressed(noteData);
								}
								else
								{
									ModifyPredStomachacheMeter(0.4);
									noteData.note.Failed = true;
								}
								break;
							case 2:
							case 3:
							case 4:
							case 5:
							default:
								if (absoluteProximity > MaximumNoteProximityRatio)
									break;

								if (absoluteProximity > 4.0)
								{
									if (Main.rand.NextBool(20))
									{
										ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
										SignifyNotePressed(noteData);
									}
								}
								if (4.0 >= absoluteProximity && absoluteProximity > 3.0)
								{
									if (Main.rand.NextBool(15))
									{
										ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
										SignifyNotePressed(noteData);
									}
								}
								if (3.0 >= absoluteProximity && absoluteProximity > 2.0)
								{
									if (Main.rand.NextBool(12))
									{
										ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
										SignifyNotePressed(noteData);
									}
								}
								if (2.0 >= absoluteProximity && absoluteProximity > 1.0)
								{
									if (Main.rand.NextBool(10))
									{
										ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
										SignifyNotePressed(noteData);
									}
								}
								if (1.0 >= absoluteProximity)
								{
									if (Main.rand.NextBool(8))
									{
										ModifyPredStomachacheMeter(preyEntity.StruggleStrength() * proximityEffectivenessMultiplier / prey.AssignedStruggleChart.DifficultyCoeff);
										SignifyNotePressed(noteData);
									}
								}
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
			}
		}

		public void ModifyPredStomachacheMeter(double amount)
		{
			switch (PredatorType)
			{
				case PredType.Player:
					Player player = Predator as Player;
					player.AsPred().Stomachache += amount;
					break;
				case PredType.NPC:
					NPC npc = Predator as NPC;
					npc.AsPred().Stomachache += amount;
					break;
			}
		}

		public List<(StruggleChartNote note, double proximity)> CheckCloseNotes(int preyIndex, bool forUI = false)
		{
			List<(StruggleChartNote note, double proximity)> closeNotes = new List<(StruggleChartNote note, double proximity)>();
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

			for (int noteSetIndex = (int)Math.Max(0, Math.Round(StruggleChartProgress) - 3); noteSetIndex <= Math.Min(Math.Round(StruggleChartProgress) + 6, targetChart.Notes.Count - 1); noteSetIndex++)
			{
				StruggleChartNote[] noteSet = targetChart.Notes[noteSetIndex];

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
				closeNotes.RemoveAll(x => x.proximity >= StruggleChartProgressRate * 8.0);
				closeNotes.RemoveAll(x => x.proximity <= -StruggleChartProgressRate * 8.0);
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

			return false;
		}
	}

	public class PreyData
	{
		public PreyType Type { get; set; }
		public Entity Instance { get; set; }
		public string ExactType { get; set; }
		public string Name { get; set; }
		public bool NoHealth { get; set; }
		public bool InventoryItem { get; set; }
		public double InitialWeight { get; set; }
		public double InitialSize { get; set; }
		public double WeightLeftToDigest { get; set; }
		public double SizeLeftToDigest => WeightLeftToDigest / InitialWeight * InitialSize;
		public StruggleChart AssignedStruggleChart { get; set; }

		public int timeSpentInStomach;

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
		public static PreyData NewData(PreyType type, string exactType, double weightRemainingIfDead = -1)
		{
			PreyData data = new PreyData();
			data.Type = type;
			data.ExactType = exactType;
			if (weightRemainingIfDead != -1)
				data.WeightLeftToDigest = weightRemainingIfDead;

			return data;
		}

		public static PreyData NewLiquidData(int liquidType, double liquidAmount)
		{
			PreyData data = new PreyData();
			data.Type = PreyType.Liquid;
			data.Instance = null;
			data.NoHealth = true;
			data.ExactType = liquidType switch
			{
				LiquidID.Water => "Water",
				LiquidID.Lava => "Lava",
				LiquidID.Honey => "Honey",
				LiquidID.Shimmer => "Shimmer",
				_ => "Some Other Liquid",
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
					exactType: "Player " + preyPlayer.name
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
					exactType: preyNPC.GivenOrTypeName
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
					exactType: preyProjectile.Name
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
					exactType: preyItem.AffixName()
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
				  + "something about a pa ram being an     in valid enter tee?\n"
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
					if (Instance is null)
						break;

					if (Instance is not Player preyPlayer)
						break;

					ExactType = preyPlayer.name;
					InitialWeight = InitialSize = WeightLeftToDigest = 1.0;
					if (ConnectedTracker is not null)
					{
						AssignedStruggleChart = new ProceduralStruggleChart();
						AssignedStruggleChart.ConnectedTracker = ConnectedTracker;
						AssignedStruggleChart.ForPredator = false;
						AssignedStruggleChart.OnStartup();
					}
					return;
				case PreyType.NPC:
					if (Instance is null)
						break;

					if (Instance is not NPC preyNPC)
						break;

					ExactType = preyNPC.FullName;
					if (preyNPC.AsFood().Size != 0)
						InitialWeight = InitialSize = WeightLeftToDigest = preyNPC.AsFood().Size;
					else
					{
						double playerToNPCWidthRatio = (double)preyNPC.width / refPlayerWidth;
						double playerToNPCHeightRatio = (double)preyNPC.height / refPlayerHeight;
						InitialWeight = InitialSize = WeightLeftToDigest = playerToNPCWidthRatio * playerToNPCHeightRatio;
					}
					if (ConnectedTracker is not null)
					{
						AssignedStruggleChart = new ProceduralStruggleChart();
						AssignedStruggleChart.ConnectedTracker = ConnectedTracker;
						AssignedStruggleChart.ForPredator = false;
						AssignedStruggleChart.OnStartup();
					}
					return;
				case PreyType.Projectile:
					if (Instance is null)
						break;

					if (Instance is not Projectile preyProjectile)
						break;

					ExactType = preyProjectile.Name;
					double playerToProjWidthRatio = (double)preyProjectile.width / refPlayerWidth;
					double playerToProjHeightRatio = (double)preyProjectile.height / refPlayerHeight;
					InitialWeight = InitialSize = WeightLeftToDigest = playerToProjWidthRatio * playerToProjHeightRatio;
					if (ConnectedTracker is not null)
					{
						AssignedStruggleChart = new ProceduralStruggleChart();
						AssignedStruggleChart.ConnectedTracker = ConnectedTracker;
						AssignedStruggleChart.ForPredator = false;
						AssignedStruggleChart.OnStartup();
					}
					return;
				case PreyType.Item:
					if (Instance is null)
						break;

					if (Instance is not Item preyItem)
						break;

					ExactType = preyItem.Name;
					InitialWeight = InitialSize = WeightLeftToDigest = preyItem.CalculateSnackSize();
					return;
				case PreyType.Liquid:
					if (Instance is not null)
						break;
					return;
				case PreyType.Custom:
					if (Instance is not null)
						break;
					return;
			}

			throw new Exception(
				"hi !!\n"
			  + "thomas says that the thing you asked to re\n"
			  + "uh    recal              reclam\n"
			  + "umm           re   calendar ?\n"
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
			ExactType = liquidType switch
			{
				LiquidID.Water => "Water",
				LiquidID.Lava => "Lava",
				LiquidID.Honey => "Honey",
				LiquidID.Shimmer => "Shimmer",
				_ => "Some Other Liquid",
			};
			InitialWeight = InitialSize = WeightLeftToDigest = liquidAmountReal;
		}

		public PreyData(int liquidType, double liquidAmount)
		{
			Type = PreyType.Liquid;
			Instance = null;
			NoHealth = true;
			ExactType = liquidType switch
			{
				LiquidID.Water => "Water",
				LiquidID.Lava => "Lava",
				LiquidID.Honey => "Honey",
				LiquidID.Shimmer => "Shimmer",
				_ => "Some Other Liquid",
			};
			InitialWeight = InitialSize = WeightLeftToDigest = liquidAmount;
		}

		/// <summary>
		/// Allows you to check what the initial size of something as a snack would be by creating a new dummy <see cref="PreyData"/> for a few moments.
		/// </summary>
		/// <param name="preyEntity">The snack-size entity to check the size of.</param>
		/// <returns>The size of the given soon-to-be stomach fodder.</returns>
		public static double GetInitialPreySize(Entity preyEntity) => NewData(preyEntity).InitialSize;
	}
}

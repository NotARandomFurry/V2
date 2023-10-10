using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace V2.Core.StruggleSystem
{
	public class StruggleTracker
	{
		public double ProgressRate { get; set; }
		public double Progress { get; set; }

		public Entity Predator { get; internal set; }
		public List<Entity> Prey { get; internal set; }
		public StruggleChart PredatorChart { get; internal set; }
		public List<StruggleChart> PreyCharts { get; internal set; }

		public static void NewTracker(Entity pred, List<Entity> prey, StruggleChart predChart, List<StruggleChart> preyCharts)
		{
			StruggleTracker tracker = new StruggleTracker();
			tracker.Progress = -2.0;
			tracker.ProgressRate = 1.0;
			tracker.Predator = pred;
			tracker.Prey = prey;
			tracker.PredatorChart = predChart;
			tracker.PreyCharts = preyCharts;
			ModContent.GetInstance<V2MasterSystem>().StruggleTrackers.Add(tracker);
		}

		public void UpdateProgress()
		{
			Progress += ProgressRate;
		}

		public void CheckAllInputs()
		{
			#region Pred notes
			if (Predator is Player playerPredator)
			{
				if (V2.StruggleUpHotkey.JustPressed)
				{

				}
				if (V2.StruggleDownHotkey.JustPressed)
				{

				}
				if (V2.StruggleLeftHotkey.JustPressed)
				{

				}
				if (V2.StruggleRightHotkey.JustPressed)
				{

				}
				if (V2.StruggleSpecialHotkey.JustPressed)
				{

				}
			}
			else if (Predator is NPC npcPredator)
			{
				
			}
			#endregion

			#region Prey notes
			foreach (Entity prey in Prey)
			{
				if (prey is Player playerPrey && playerPrey.whoAmI == Main.myPlayer)
				{
					if (V2.StruggleUpHotkey.JustPressed)
					{

					}
					if (V2.StruggleDownHotkey.JustPressed)
					{

					}
					if (V2.StruggleLeftHotkey.JustPressed)
					{

					}
					if (V2.StruggleRightHotkey.JustPressed)
					{

					}
					if (V2.StruggleSpecialHotkey.JustPressed)
					{

					}
				}
				else if (prey is NPC npcPrey)
				{

				}
			}
			#endregion
		}

		public void CheckClientSideInputs()
		{
			#region Pred notes
			if (Predator is Player playerPredator && playerPredator.whoAmI == Main.myPlayer)
			{
				if (V2.StruggleUpHotkey.JustPressed)
				{

				}
				if (V2.StruggleDownHotkey.JustPressed)
				{

				}
				if (V2.StruggleLeftHotkey.JustPressed)
				{

				}
				if (V2.StruggleRightHotkey.JustPressed)
				{

				}
				if (V2.StruggleSpecialHotkey.JustPressed)
				{

				}
			}
			#endregion

			#region Prey notes
			foreach (Entity prey in Prey)
			{
				if (prey is Player playerPrey && playerPrey.whoAmI == Main.myPlayer)
				{
					if (V2.StruggleUpHotkey.JustPressed)
					{

					}
					if (V2.StruggleDownHotkey.JustPressed)
					{

					}
					if (V2.StruggleLeftHotkey.JustPressed)
					{

					}
					if (V2.StruggleRightHotkey.JustPressed)
					{

					}
					if (V2.StruggleSpecialHotkey.JustPressed)
					{

					}
				}
			}
			#endregion
		}

		public void CheckServerSideInputs()
		{
			#region Pred notes
			if (Predator is NPC npcPredator)
			{
				if (V2.StruggleUpHotkey.JustPressed)
				{

				}
			}
			#endregion

			#region Prey notes
			foreach (Entity prey in Prey)
			{
				if (prey is NPC npcPrey)
				{
					
				}
			}
			#endregion
		}

		public List<StruggleChartNote> CheckCloseNotes(StruggleChart target)
		{
			List<StruggleChartNote> closeNotes = new List<StruggleChartNote>();
			return closeNotes;
		}
	}
}

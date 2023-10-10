using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.Core.StruggleSystem;
using V2.NPCs;

namespace V2.PlayerHandling
{
	public partial class PreyPlayer : ModPlayer
	{
		public StatModifier StruggleStrengthModifier { get; set; }
		public double StruggleStrength {
			get {
				double baseStruggleStrength = 3;
				return StruggleStrengthModifier.ApplyTo((float)baseStruggleStrength);
			}
		}

		public void CheckStruggleInputs(StruggleTracker tracker, StruggleChart chart)
		{
			List<StruggleChartNote> closeNotes = tracker.CheckCloseNotes(chart);
			void HandleLanePress(NoteLane lane, ModKeybind key)
			{
				if (!key.Current)
					return;

				List<StruggleChartNote> closeNote = closeNotes.FindAll(x => Math.Abs(x.Position - tracker.Progress) < tracker.ProgressRate * 8.0 && x.Lane == lane);
				if (closeNote is null)
				{
					// do nothing, currently
				}
				else
				{
					Entity pred = tracker.Predator;
					if (pred is Player predPlayer)
					{
						predPlayer.AsPred().Stomachache += StruggleStrength;
					}
					else if (pred is NPC predNPC)
					{
						predNPC.AsPred().Stomachache += StruggleStrength;
					}
				}
			}

			HandleLanePress(NoteLane.Up, V2.StruggleUpHotkey);
			HandleLanePress(NoteLane.Down, V2.StruggleDownHotkey);
			HandleLanePress(NoteLane.Left, V2.StruggleLeftHotkey);
			HandleLanePress(NoteLane.Right, V2.StruggleRightHotkey);
			HandleLanePress(NoteLane.Special, V2.StruggleSpecialHotkey);
		}
	}
}

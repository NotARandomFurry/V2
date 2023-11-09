using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Core.StruggleSystem
{
	public class ProceduralStruggleChart : StruggleChart
	{
		public override List<StruggleChartNote[]> Notes => RandomNoteSpan;
		private List<StruggleChartNote[]> RandomNoteSpan { get; set; }
		private static int MaxRandomNoteSpanLength => 128;

		public override void OnStartup()
		{
			RandomNoteSpan = new List<StruggleChartNote[]>();
			if (ConnectedTracker is null)
				return;

			int notesAdded = 0;
			for (int i = 0; i < MaxRandomNoteSpanLength; i++)
			{
				StruggleChartNote[] noteSet = new StruggleChartNote[5] { null, null, null, null, null };
				if (Main.rand.NextFloat(3f) <= DifficultyCoeff)
				{
					List<NoteLane> lanes = new List<NoteLane> { NoteLane.Up, NoteLane.Left, NoteLane.Right, NoteLane.Down };
					NoteLane noteLaneToFill = Main.rand.NextFromCollection(lanes);
					noteSet[(int)noteLaneToFill] = new StruggleChartNote(noteLaneToFill);
					notesAdded++;
					lanes.Remove(noteLaneToFill);
					if (Main.rand.NextFloat(3f) <= DifficultyCoeff - 0.4f)
					{
						noteLaneToFill = Main.rand.NextFromCollection(lanes);
						noteSet[(int)noteLaneToFill] = new StruggleChartNote(noteLaneToFill);
						notesAdded++;
						lanes.Remove(noteLaneToFill);
						if (Main.rand.NextFloat(3f) <= DifficultyCoeff - 0.8f)
						{
							noteLaneToFill = Main.rand.NextFromCollection(lanes);
							noteSet[(int)noteLaneToFill] = new StruggleChartNote(noteLaneToFill);
							notesAdded++;
							lanes.Remove(noteLaneToFill);
							if (Main.rand.NextFloat(3f) <= DifficultyCoeff - 1.2f)
							{
								noteLaneToFill = Main.rand.NextFromCollection(lanes);
								noteSet[(int)noteLaneToFill] = new StruggleChartNote(noteLaneToFill);
								notesAdded++;
								lanes.Remove(noteLaneToFill);
							}
						}
					}
				}

				RandomNoteSpan.Add(noteSet);
			}
			if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
			{
				string debugText = "New procedural chart of difficulty " + DifficultyCoeff + " constructed for " + (ForPredator ? "a hungry pred" : "a soon-to-be meal") + " with " + notesAdded + " notes in total.";
				if (Main.netMode == NetmodeID.SinglePlayer)
					Main.NewText(debugText, Color.PaleVioletRed);
				else if (Main.netMode == NetmodeID.Server)
					ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(debugText), Color.PaleVioletRed);
			}
		}
	}
}

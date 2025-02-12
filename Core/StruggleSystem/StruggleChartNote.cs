using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2.Core.StruggleSystem
{
	public class StruggleChartNote
	{
		/// <summary>
		/// The lane that this note should arrive in.<br/>
		/// </summary>
		public NoteDirection Direction { get; private set; }

		private int _holdLength;
		/// <summary>
		/// The amount of time, in quarters of a beat, that this note must be held for following the initial keystroke.<br/>
		/// Defaults to 0, which means that this note does not need to be held down following the initial keystroke.<br/>
		/// </summary>
		public int HoldLength
		{
			get => _holdLength;
			private set => _holdLength = Math.Max(value, 0);
		}

		private double _position;
		/// <summary>
		/// Where this note is placed in progression on the struggle chart it's on.
		/// </summary>
		public double Position {
			get => _position;
			private set => _position = Math.Max(value, 0);
		}

		/// <summary>
		/// Whether or not this note is a "bad note", inverting the stomachache meter drain/fill based on context and counting as a misstroke if pressed.<br/>
		/// Bad notes do not trigger any on-struggle effects that normal notes would.<br/>
		/// </summary>
		public bool Bad { get; private set; }

		public bool Failed { get; set; }
		public bool CorrectlyPressed { get; set; }
		public double PressedPosition { get; set; }
		public int PressAnimTimer { get; set; }

		public StruggleChartNote(NoteDirection lane, int holdLength = 0, bool bad = false)
		{
			Direction = lane;
			HoldLength = holdLength;
			Bad = bad;
			Position = 0.0;

			CorrectlyPressed = false;
			PressedPosition = 0.0;
			PressAnimTimer = 0;
		}
	}
}

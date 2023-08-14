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
		public NoteLane Lane { get; private set; }

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

		/// <summary>
		/// Whether or not this note is a "bad note", inverting the stomachache meter drain/fill based on context and counting as a misstroke if pressed.<br/>
		/// Bad notes do not trigger any on-struggle effects that normal notes would.<br/>
		/// </summary>
		public bool Bad { get; private set; }

		public StruggleChartNote(NoteLane lane, int holdLength = 0, bool bad = false)
		{
			Lane = lane;
			HoldLength = holdLength;
			Bad = bad;
		}
	}
}

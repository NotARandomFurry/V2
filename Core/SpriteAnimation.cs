using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace V2.Core
{
	/// <summary>
	/// This is a custom-built type for VSC which allows sprite animations to be easily defined and used.<br/>
	/// </summary>
	public abstract class SpriteAnimation : ModTexturedType
	{
		protected sealed override void Register() { }

		/// <summary>
		/// The path of the texture file to draw from for this animation.<br/>
		/// </summary>
		public abstract override string Texture { get; }

		public abstract Rectangle? DecideFrame();

		/// <summary>
		/// Determines if, on the current frame, the current animation has just completed a loop.<br/>
		/// </summary>
		public bool CanTransitionToNewAnim => FrameDictPos == 0 && FrameDelay == Frames[0].rawDelay * 0.6;

		public abstract List<(int frame, int rawDelay)> Frames { get; }

		public int FrameDictPos { get; set; }

		/// <summary>
		/// Shorthand for Frames[FrameDictPost].frame.<br/>
		/// Used primarily for <see cref="DecideFrame"/>.<br/>
		/// </summary>
		public int FrameID => Frames is not null ? Frames[FrameDictPos].frame : 0;
		public int FrameDelay { get; set; }

		public void Initialize()
		{
			FrameDictPos = 0;
			FrameDelay = (int)Math.Round(Frames[0].rawDelay * 0.6);
		}

		public void Advance(int speed = 1)
		{
			FrameDelay -= speed;
			if (FrameDelay <= 0)
			{
				FrameDictPos += 1;
				FrameDictPos %= Frames.Count;
				FrameDelay = (int)Math.Round(Frames[FrameDictPos].rawDelay * 0.6);
			}
		}
	}
}

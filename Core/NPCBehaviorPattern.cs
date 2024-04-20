using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace V2.Core
{
	/// <summary>
	/// This class is used to define and utilize behavioral patterns for NPCs.
	/// </summary>
	public abstract class NPCBehaviorPattern
	{
		/// <summary>
		/// Can be used to easily define how long a pattern should last before a switch to another can/should happen.<br/>
		/// Unused by default; set this, and then use it in <see cref="AI"/> when needed.<br/>
		/// </summary>
		public virtual int PatternLength { get; set; }
		public int PatternTimer = 0;
		public int SecondaryTimer = 0;
		public List<Vector2> TempData;

		public NPCBehaviorPattern()
		{
			PatternTimer = 0;
			SecondaryTimer = 0;
		}

		/// <summary>
		/// Handles most behavioral code for this behavior pattern, save for drawing-related code.<br/>
		/// Call this as part of the overall revamped AI method for the NPC in question.<br/>
		/// </summary>
		/// <param name="npc">
		/// The NPC that this behavior pattern is to be run for.
		/// </param>
		/// <param name="target">
		/// The entity that the NPC that this behavior pattern is being run on is currently targeting.<br/>
		/// Set to null if the NPC lacks a target.<br/>
		/// </param>
		public void DoBehavior(NPC npc, Entity target)
		{
			if (PatternTimer == 0)
				Initialize(npc, target);

			AI(npc, target);
			PatternTimer++;
		}

		/// <summary>
		/// Allows you to initialize important data for the first frame that this behavior pattern is active.<br/>
		/// </summary>
		/// <param name="npc">
		/// The NPC that this behavior pattern is being run for.
		/// </param>
		/// <param name="target">
		/// The entity that the NPC that this behavior pattern is being run on is currently targeting.<br/>
		/// Set to null if the NPC lacks a target.<br/>
		/// </param>
		public virtual void Initialize(NPC npc, Entity target) { }

		/// <summary>
		/// Can be used to define behavior which is run when this behavior pattern is active.<br/>
		/// </summary>
		/// <param name="npc">
		/// The NPC that this behavior pattern is being run for.
		/// </param>
		/// <param name="target">
		/// The entity that the NPC that this behavior pattern is being run on is currently targeting.<br/>
		/// Set to null if the NPC lacks a target.<br/>
		/// </param>
		public virtual void AI(NPC npc, Entity target) { }

		/// <summary>
		/// Can be used to apply visual effects to the given NPC when this behavior pattern is active.<br/>
		/// </summary>
		/// <param name="npc">
		/// The NPC that this behavior pattern is being run for.
		/// </param>
		/// <param name="spriteBatch">
		/// The <see cref="SpriteBatch"/> to be used to draw the NPC that this behavior pattern is being run on.
		/// </param>
		public virtual void VisualEffects(NPC npc, SpriteBatch spriteBatch) { }
	}
}

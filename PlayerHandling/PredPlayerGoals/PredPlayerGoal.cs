using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.PlayerHandling.PredPlayerGoals.Starter;

namespace V2.PlayerHandling.PredPlayerGoals
{
	public abstract class PredPlayerGoal : ModType
	{
		protected sealed override void Register()
		{
			ModTypeLookup<PredPlayerGoal>.Register(this);

			PredPlayerGoalLoader.PredPlayerGoals.Add(this);
		}

		public static Rectangle TextureBounds => new Rectangle(0, 0, 52, 52);

		public virtual Texture2D IncompleteTexture => ModContent.Request<Texture2D>("V2/PlayerHandling/PredPlayerGoals/PlaceholderIncomplete").Value;
		public virtual Texture2D CompleteTexture => ModContent.Request<Texture2D>("V2/PlayerHandling/PredPlayerGoals/PlaceholderComplete").Value;

		/// <summary>
		/// The name used for this goal in saving and loading.<br/>
		/// </summary>
		public abstract string InternalName { get; }

		/// <summary>
		/// The name used for this goal in the player pred goals menu.<br/>
		/// </summary>
		/// <param name="pred">
		/// The predatory player that has the player pred goals menu open.<br/>
		/// Can be used to change this goal's name based on player conditions.<br/>
		/// </param>
		public abstract string DisplayName(Player pred);

		/// <summary>
		/// The name used for this goal in the player pred goals menu.<br/>
		/// </summary>
		/// <param name="pred">
		/// The predatory player that has the player pred goals menu open.<br/>
		/// Can be used to change this goal's name based on player conditions.<br/>
		/// </param>
		public virtual Color DisplayNameColor(Player pred) => Complete(pred) ? Color.LimeGreen : Color.Red;

		/// <summary>
		/// The description used for this goal in the player pred goals menu.<br/>
		/// </summary>
		/// <param name="pred">
		/// The predatory player that has the player pred goals menu open.<br/>
		/// Can be used to change this goal's description based on player conditions.<br/>
		/// </param>
		public abstract string Description(Player pred);

		/// <summary>
		/// The number of stat points granted by the completion of this pred goal.<br/>
		/// </summary>
		public abstract int StatPointsFromCompletion { get; }

		/// <summary>
		/// The progression category which this goal is to be filed under.<br/>
		/// </summary>
		public abstract ProgressionStage Stage { get; }

		/// <summary>
		/// Whether or not this goal should be visible in the player pred goals menu to the given player at all.<br/>
		/// Does not prevent completion. Can be used to create "hidden" goals that show once complete.<br/>
		/// </summary>
		/// <param name="pred">
		/// The predatory player to check this goal's availability for.<br/>
		/// </param>
		public virtual bool Available(Player pred) => true;

		public void TrySetCompletion(Player pred, bool intendedCompletionState = true)
		{
			if (!pred.AsPred().GoalsCompleted.ContainsKey(InternalName))
				pred.AsPred().GoalsCompleted.Add(InternalName, !intendedCompletionState);

			if (pred.AsPred().GoalsCompleted[InternalName] == intendedCompletionState)
				return;

			pred.AsPred().GoalsCompleted[InternalName] = intendedCompletionState;
			if (intendedCompletionState)
			{
				
			}
			else
			{
				
			}
		}

		/// <summary>
		/// Checks whether this goal has been completed by the given predatory player.<br/>
		/// </summary>
		/// <param name="pred">
		/// The predatory player to check this goal's completion status for.<br/>
		/// </param>
		public bool Complete(Player pred)
		{
			if (!pred.AsPred().GoalsCompleted.ContainsKey(InternalName))
				pred.AsPred().GoalsCompleted.Add(InternalName, false);

			return pred.AsPred().GoalsCompleted[InternalName];
        }
        /// <summary>
        /// Checks whether a specific goal has been completed by the given predatory player.<br/>
        /// </summary>
        /// <param name="pred">
        /// The predatory player to check for.<br/>
        /// </param>
        /// <param name="internalName">
        /// The internal name of the goal to check.<br/>
        /// </param>
        public bool HasCompleted(Player pred, string internalName)
        {
            if (!pred.AsPred().GoalsCompleted.ContainsKey(internalName))
                pred.AsPred().GoalsCompleted.Add(internalName, false);

            return pred.AsPred().GoalsCompleted[internalName];
        }
    }
}

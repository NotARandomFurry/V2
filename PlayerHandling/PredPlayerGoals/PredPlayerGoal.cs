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
		public static FirstLivePrey FirstLivePrey { get; set; } = new FirstLivePrey();
		public static FirstItemEaten FirstItemEaten { get; set; } = new FirstItemEaten();

		protected sealed override void Register()
		{
			ModTypeLookup<PredPlayerGoal>.Register(this);

			PredPlayerGoalLoader.PredPlayerGoals.Add(this);
		}

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
		/// </summary>
		/// <param name="pred">
		/// The predatory player to check this goal's availability for.<br/>
		/// </param>
		public virtual bool Available(Player pred) => true;

		/// <summary>
		/// Checks whether this goal has been completed by the given predatory player.<br/>
		/// </summary>
		/// <param name="pred">
		/// The predatory player to check this goal's completion status for.<br/>
		/// </param>
		public bool Complete(Player pred) => pred.AsPred().Goals.Find(x => x.goalName == DisplayName(pred)).complete;
	}
}

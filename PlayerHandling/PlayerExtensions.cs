using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;

namespace V2.PlayerHandling
{
	public static class PlayerExtensions
	{
		public static V2Player AsV2Player(this Player player) => player.GetModPlayer<V2Player>();
		public static PredPlayer AsPred(this Player player) => player.GetModPlayer<PredPlayer>();
		public static PreyPlayer AsPrey(this Player player) => player.GetModPlayer<PreyPlayer>();

		public static bool IsFoodFor(this Player player, Entity entity, out bool pastTense)
		{
			pastTense = false;
			if (entity is NPC predNPC)
			{
				List<Prey> playerAsPreyList = predNPC.AsPred().stomachContents.FindAll(x => x.Type == PreyType.Player && x.Index == player.whoAmI);
				if (playerAsPreyList != null && playerAsPreyList.Count > 0)
				{
					if (playerAsPreyList.FirstOrDefault(x => !x.Dead) == null)
						pastTense = true;
					return true;
				}
			}
			else if (entity is Player predPlayer)
			{
				List<Prey> playerAsPreyList = predPlayer.AsPred().stomachContents.FindAll(x => x.Type == PreyType.Player && x.Index == player.whoAmI);
				if (playerAsPreyList != null && playerAsPreyList.Count > 0)
				{
					if (playerAsPreyList.FirstOrDefault(x => !x.Dead) == null)
						pastTense = true;
					return true;
				}
			}
			return false;
		}

		public static bool HasEaten(this Player player, string entity, out int howManyTimes)
		{
			howManyTimes = 0;
			if (!player.AsPred().mealCount.ContainsKey(entity))
				return false;
			if (player.AsPred().mealCount[entity] <= 0)
				return false;

			howManyTimes = player.AsPred().mealCount[entity];
			return true;
		}

		public static Vector2 TrueMountedCenter(this Player player)
			=> new Vector2(
				player.position.X + ((float)player.width / 2f),
				player.position.Y + 21f + player.HeightOffsetHitboxCenter
			);

		public static bool IsAirborne(this Player player)
		{
			if (player.mount.Active)
				return !MountID.Sets.Cart[player.mount.Type];

			if (player.velocity.Y == 0f)
				return false;

			if (player.AsPrey().IsCurrentlyEaten)
				return false;

			return true;
		}
	}
}

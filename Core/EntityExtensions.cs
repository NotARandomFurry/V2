using Microsoft.Xna.Framework;
using Terraria;

namespace V2.NPCs
{
	public static class EntityExtensions
	{
		public static Vector2 TrueCenter(this Entity entity) => new Vector2(entity.position.X + ((float)entity.width / 2f), entity.position.Y + ((float)entity.height / 2f));

		public static void AddStatus(this Entity entity, int statusID, int intendedTime)
		{
			intendedTime += 1;
			if (entity is Player playerPred)
				playerPred.AddBuff(statusID, intendedTime);
			else if (entity is NPC NPCPred)
				NPCPred.AddBuff(statusID, intendedTime);
		}
	}
}

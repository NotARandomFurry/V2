using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using V2.Core;

namespace V2.NPCs
{
	public static class EntityExtensions
	{
		public static Vector2 TrueCenter(this Entity entity) => new Vector2(entity.position.X + ((float)entity.width / 2f), entity.position.Y + ((float)entity.height / 2f));
	}
}

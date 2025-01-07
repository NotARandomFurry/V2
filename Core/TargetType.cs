using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2.Core
{
	public enum TargetType
	{
		None,
		Player,
		NPC,
		Projectile,
		Other,
	}
	public enum TargetPriorityLevel
	{
		None,
		Neutral,
		High,
		VeryHigh,
		Favorite,
	}
}

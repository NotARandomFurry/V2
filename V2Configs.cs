using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace V2
{
	[Label("Server-Side Options")]
	public class V2ServerSideConfigs : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[Header("Insight Options\n"
			 + "[c/7F7F7F:(these let you make bug reports and other such things easier)]")]

		[Label("Send Debug Chat Messages")]
		[Tooltip("Displays debug messages in chat relating to the various vore mechanics.\n"
			   + "Note that these may spam your chat, and should only be turned on if something is wrong.\n"
			   + "Can help greatly in bug reporting.\n"
			   + "Defaults to false.")]
		[DefaultValue(false)]
		public bool DebugChatMessages { get; set; }

		[Header("Personalization Options\n"
			 + "[c/7F7F7F:(these let you fine-tune the experience to your preferences, without breakin' the balance of the game)]")]

		[Label("Selectively Hungry Town NPCs")]
		[Tooltip("Prevents town NPCs from randomly eating players.\n"
			   + "Included for players who prefer being pred.\n"
			   + "Does not prevent non-town NPCs from eating players where applicable.\n"
			   + "Does not prevent town NPCs from randomly eating other town NPCs.\n"
			   + "Defaults to false.")]
		[DefaultValue(false)]
		public bool NoRandomGulpsAgainstPlayer { get; set; }

		[Label("Pred Non-Preference")]
		[Tooltip("Allows you to prevent preds of a certain gender from actively eating others in-game.\n"
			   + "Applies to all entities, players included.\n"
		//	   + "Will not affect any in-universe establishments about predators of the blacklisted gender.\n"
		//	   + "Does not work for hermaphrodites, androgynous menaces, or otherwise functionally-genderless individuals.\n"
			   + "By default, does not blacklist any gender.")]
		[OptionStrings(new string[] {
			"Default (No Blacklist)",
			"No Female",
			"No Male",
			"No M or F...but why?",
		})]
		[DefaultValue("Default (No Blacklist)")]
		public string GenderBlacklist { get; set; }

		[Header("Options Just For Fun\n"
			 + "[c/7F7F7F:(these let you tell my balance intentions to go fuck themselves and just have some dumb fun with the game)]")]

		[Label("Include Defense In Digestion Damage Calcs")]
		[Tooltip("BALANCE BREAKAGE: High\n"
			   + "Determines whether or not digestion damage is affected by defense.\n"
			   + "Affects all preds.\n"
			   + "Defaults to true.")]
		[DefaultValue(true)]
		public bool DefenseInDigestionCalcs { get; set; }

		[Label("Invoke The Second Law")]
		[Tooltip("BALANCE BREAKAGE: Who cares?\n"
			   + "Makes it possible for the Empress of Light to fit in stomachs of any capacity, and greatly increases digestion damage against her.\n"
			   + "Defaults to false.\n"
			   + "'Fairies are food, not friends'")]
		[DefaultValue(false)]
		public bool EasilyEdibleEmpress { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core.StruggleSystem;
using V2.Items.Voraria;
using V2.Items.Voraria.Accessories;
using V2.Items.Voraria.Accessories.Informational;
using V2.Items.Voraria.Accessories.Vanity;
using V2.Items.Voraria.Consumables.Potions;

namespace V2.Core
{
	public class V2RecipeSystem : ModSystem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override void AddRecipes()
		{
			// surprisingly, I don't actually usw this as a recipe method; that title belongs to the shitshow that is EstablishRecipeCollection below
			// this is instead used to set custom bestiary star amounts, since I damn well can't do it elsewhere (lol!)
			// ContentSamples.NpcBestiaryRarityStars[NPC type here] = X;
		}

		public override void PostAddRecipes()
		{
			EstablishRecipeCollection();
		}
		public override void AddRecipeGroups()
		{
			RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Evil Wood", new int[]
			{
				ItemID.Ebonwood,
				ItemID.Shadewood,
			});
			RecipeGroup.RegisterGroup("V2:EvilWood", group);

			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Ordinary Fish", new int[]
			{
				ItemID.Bass,
				ItemID.Tuna,
				ItemID.Trout,
				ItemID.Salmon,
				ItemID.AtlanticCod,
			});
			RecipeGroup.RegisterGroup("V2:Fish", group);

			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Copper Bar", new int[]
			{
				ItemID.TinBar,
				ItemID.CopperBar,
			});
			RecipeGroup.RegisterGroup("V2:Copper", group);

			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Silver Bar", new int[]
			{
				ItemID.SilverBar,
				ItemID.TungstenBar,
			}); ;
			RecipeGroup.RegisterGroup("V2:Silver", group);

			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Gold Bar", new int[]
			{
				ItemID.GoldBar,
				ItemID.PlatinumBar,
			});
			RecipeGroup.RegisterGroup("V2:Gold", group);

			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Ice", new int[]
			{
				ItemID.IceBlock,
				ItemID.PurpleIceBlock,
				ItemID.RedIceBlock,
				ItemID.PinkIceBlock,
			});
			RecipeGroup.RegisterGroup("V2:Ice", group);

			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Sand", new int[]
			{
				ItemID.SandBlock,
				ItemID.EbonsandBlock,
				ItemID.CrimsandBlock,
				ItemID.PearlsandBlock,
			});
			RecipeGroup.RegisterGroup("V2:Sand", group);

			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Dungeon Brick", new int[]
			{
				ItemID.PinkBrick,
				ItemID.GreenBrick,
				ItemID.BlueBrick,
			});
			RecipeGroup.RegisterGroup("V2:DungeonBrick", group);

			group = new RecipeGroup(() => "Cobalt or Palladium Bars", new int[]
			{
				ItemID.CobaltBar,
				ItemID.PalladiumBar,
			});
			RecipeGroup.RegisterGroup("V2:T1AltarMetals", group);

			group = new RecipeGroup(() => "Mythril or Orichalcum Bars", new int[]
			{
				ItemID.MythrilBar,
				ItemID.OrichalcumBar,
			});
			RecipeGroup.RegisterGroup("V2:T2AltarMetals", group);

			group = new RecipeGroup(() => "Titanium or Adamantite Bars", new int[]
			{
				ItemID.TitaniumBar,
				ItemID.AdamantiteBar,
			});
			RecipeGroup.RegisterGroup("V2:T3AltarMetals", group);

			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Hardmode Anvil", new int[]
			{
				ItemID.MythrilAnvil,
				ItemID.OrichalcumAnvil,
			});
			RecipeGroup.RegisterGroup("V2:HMAnvils", group);
		}

		/// <summary>
		/// Removes all existing recipes which result in the given item type.<br/>
		/// Used to instate VSC recipes instead of vanilla/othermod ones.<br/>
		/// ONLY CALL IN <see cref="EstablishRecipeCollection"/>! This is used only in <see cref="Mod.PostAddRecipes"/>, for stability reasons.<br/>
		/// </summary>
		/// <param name="type">
		/// The item type to remove all existing recipes for.
		/// </param>
		public static void RemoveExistingRecipesForItem(int type)
		{
			for (int i = 0; i < Recipe.numRecipes; i++)
			{
				Recipe recipe = Main.recipe[i];
				if (recipe.HasResult(type))
					recipe.DisableRecipe();
			}
		}

		/// <summary>
		/// Removes any instances of the given item type from all existing recipes' ingredient lists.<br/>
		/// ONLY CALL IN <see cref="EstablishRecipeCollection"/>! This is used only in <see cref="Mod.PostAddRecipes"/>, for stability reasons.<br/>
		/// </summary>
		/// <param name="type">
		/// The item type to remove from all existing recipes.
		/// </param>
		public static void RemoveItemFromExistingRecipes(int type)
		{
			for (int i = 0; i < Main.recipe.Length; i++)
			{
				Recipe recipe = Main.recipe[i];
				recipe.requiredItem.RemoveAll(x => x.type == type);
			}
		}

		public static void EstablishRecipeCollection()
		{
			#region Vanilla
			#region Accessories
			#region Informational
			#region Defensive
			#region Adhesive Bandage
			RemoveExistingRecipesForItem(ItemID.AdhesiveBandage);
			#endregion
			#endregion
			#endregion
			#endregion
			#region Dyes
			#region Sky Blue Dye
			Recipe.Create(ItemID.SkyBlueDye, 3)
				.AddIngredient(ModContent.ItemType<NymphHairStrand>())
				.AddTile(TileID.DyeVat)
				.Register();
			#endregion
			#endregion
			#endregion
			#region Voraria
			#region Accessories
			#region Informational
			#region Sizemic Scanner
			RemoveExistingRecipesForItem(ModContent.ItemType<MealSizeScanner>());
			#endregion
			#region Servant's Scanner
			RemoveExistingRecipesForItem(ModContent.ItemType<PredCapacityScanner>());
			#endregion
			#endregion
			#region Vanity
			#region Belly-Shaped Balloon
			RemoveExistingRecipesForItem(ModContent.ItemType<BalloonBelly>());
			#endregion
			#endregion
			#region Sly Huntress' Scarf
			RemoveExistingRecipesForItem(ModContent.ItemType<NymphHairScarf>());
			Recipe.Create(ModContent.ItemType<NymphHairScarf>())
				.AddIngredient(ModContent.ItemType<NymphHairStrand>(), 5)
				.AddIngredient(ItemID.FallenStar, 7)
				.AddIngredient(ItemID.Sapphire, 3)
				.AddTile(TileID.Loom)
				.Register();
			#endregion
			#endregion
			#region Consumables
			#region Potions
			#region Digestive Aid Potion
			RemoveExistingRecipesForItem(ModContent.ItemType<FastDigestionPotion>());
			#endregion
			#region Elasticity Potion
			RemoveExistingRecipesForItem(ModContent.ItemType<StomachCapacityPotion>());
			Recipe.Create(ModContent.ItemType<StomachCapacityPotion>(), 2)
				.AddIngredient(ItemID.BottledHoney, 2)
				.AddIngredient(ItemID.VariegatedLardfish, 1)
				.AddIngredient(ItemID.DoubleCod, 1)
				.AddTile(TileID.Bottles)
				.Register();
			#endregion
			#region Internal Endurance Potion
			RemoveExistingRecipesForItem(ModContent.ItemType<StomachacheMeterCapacityPotion>());
			#endregion
			#endregion
			#endregion
			#region Misc.
			#region Sky Blue Strand
			RemoveExistingRecipesForItem(ModContent.ItemType<NymphHairStrand>());
			#endregion
			#endregion
			#endregion
		}
	}
}

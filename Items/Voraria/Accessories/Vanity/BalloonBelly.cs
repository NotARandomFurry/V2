using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Accessories.Vanity
{
	public class BalloonBelly : ModItem
	{
		public static SoundStyle InflationSound => new SoundStyle("V2/Sounds/Item/BalloonBellyInflate", SoundType.Sound);
		public static SoundStyle DeflationSound => new SoundStyle("V2/Sounds/Item/BalloonBellyDeflate", SoundType.Sound);
		public static int MaximumInflatedSize => 5;
		public int InflatedSize { get; set; }
		public Color SkinColor { get; set; }
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Accessories.Vanity.BalloonBelly");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Accessories.Vanity.BalloonBelly.Short");
		public override string Texture => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size0";
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.useStyle = ItemUseStyleID.Swing;

			Item.vanity = true;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(
				gold: 5
			);

			InflatedSize = 0;
			SkinColor = Color.White;
		}

		public override ModItem Clone(Item newEntity)
		{
			BalloonBelly belly = newEntity.ModItem as BalloonBelly;
			belly.InflatedSize = InflatedSize;
			belly.SkinColor = SkinColor;
			return base.Clone(newEntity);
		}

		public override Color? GetAlpha(Color lightColor) => SkinColor;

		public override bool AltFunctionUse(Player player) => true;

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			SkinColor = Color.White;
		}

		public override void UpdateInventory(Player player)
		{
			SkinColor = player.skinColor;
		}

		public override bool CanUseItem(Player player)
		{
			if (Main.mouseLeft && Main.mouseLeftRelease && InflatedSize < MaximumInflatedSize)
			{
				InflatedSize += 1;
				float inflatePitch = InflatedSize switch
				{
					1 => 0f,
					2 => -0.1f,
					3 => -0.2f,
					4 => -0.3f,
					5 => -0.4f,
					_ => -1f,
				};
				SoundEngine.PlaySound(
					InflationSound with { Pitch = inflatePitch },
					player.TrueCenter()
				);
			}
			
			if (Main.mouseRight && Main.mouseRightRelease && InflatedSize > 0)
			{
				InflatedSize -= 1;
				float deflatePitch = InflatedSize switch
				{
					0 => 0f,
					1 => -0.1f,
					2 => -0.2f,
					3 => -0.3f,
					4 => -0.4f,
					_ => -1f,
				};
				SoundEngine.PlaySound(
					DeflationSound with { Pitch = deflatePitch },
					player.TrueCenter()
				);
			}
			return false;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			SkinColor = player.skinColor;
		}

		public override void UpdateVanity(Player player)
		{
			SkinColor = player.skinColor;
			player.AsPred().FlatBellySizeModifier += InflatedSize;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Voraria.Accessories.Vanity.BalloonBelly",
				new
				{
					InflatedSize,
					MaximumInflatedSize
				}
			);
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			TextureAssets.Item[Type] = ModContent.Request<Texture2D>(
				InflatedSize switch
				{
					1 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size1",
					2 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size2",
					3 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size3",
					4 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size4",
					5 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size5",
					_ => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size0",
				},
				AssetRequestMode.ImmediateLoad
			);
			return true;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			TextureAssets.Item[Type] = ModContent.Request<Texture2D>(
				InflatedSize switch
				{
					1 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size1",
					2 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size2",
					3 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size3",
					4 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size4",
					5 => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size5",
					_ => "V2/Items/Voraria/Accessories/Vanity/BalloonBelly_Size0",
				},
				AssetRequestMode.ImmediateLoad
			);
			return true;
		}

		public override void SaveData(TagCompound tag)
		{
			tag["BalloonBellySize"] = InflatedSize;
			tag["BalloonBellySkinColorR"] = SkinColor.R;
			tag["BalloonBellySkinColorG"] = SkinColor.G;
			tag["BalloonBellySkinColorB"] = SkinColor.B;
			tag["BalloonBellySkinColorA"] = SkinColor.A;
		}

		public override void LoadData(TagCompound tag)
		{
			if (!tag.ContainsKey("BalloonBellySize"))
				return;

			InflatedSize = tag.GetInt("BalloonBellySize");
			SkinColor = new Color(
				tag.GetByte("BalloonBellySkinColorR"),
				tag.GetByte("BalloonBellySkinColorG"),
				tag.GetByte("BalloonBellySkinColorB"),
				tag.GetByte("BalloonBellySkinColorA")
			);
		}
	}
}

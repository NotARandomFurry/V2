using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace V2.UI
{
	public class UIHoverImageButton : UIImageButton
	{
		internal string HoverText;

		public UIHoverImageButton(Asset<Texture2D> texture, string hoverText) : base(texture)
		{
			HoverText = hoverText;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering)
			{
				Main.hoverItemName = HoverText;
			}
		}
	}

	public class UIPanelButton : UIElement
	{
		private object _text;
		private MouseEvent _leftClickAction;
		private UIPanel _uiPanel;
		private UIText _uiText;

		public string Text
		{
			get => _uiText?.Text ?? string.Empty;
			set => _text = value;
		}

		public UIPanelButton(object text, MouseEvent leftClickAction) : base()
		{
			_text = text != null ? text.ToString() : null;
			_leftClickAction = leftClickAction;
		}

		public override void OnInitialize()
		{
			_uiPanel = new UIPanel();
			_uiPanel.Width = StyleDimension.Fill;
			_uiPanel.Height = StyleDimension.Fill;
			_uiText = new UIText("");
			_uiText.VAlign = _uiText.HAlign = 0.5f;
			_uiPanel.OnLeftClick += _leftClickAction;
			_uiPanel.Append(_uiText);

			Append(_uiPanel);
		}

		public override void Update(GameTime gameTime)
		{
			if (_text != null)
			{
				_uiText.SetText(_text.ToString());
				_text = null;
				Recalculate();
				MinWidth = _uiText.MinWidth;
				MinHeight = _uiText.MinHeight;
			}
		}
	}
}

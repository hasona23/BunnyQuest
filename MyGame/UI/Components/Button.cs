using Microsoft.Xna.Framework.Graphics;
using SpaceCup.Utils;
using System;

namespace MyGame.UI.Components;
internal class Button(Label label)
{

    private Label _label = label;
    public UiStyle HoverStyle { get; set; } = label.Style;
    public UiStyle ClickStyle { get; set; } = label.Style;
    public UiStyle DefaultStyle = label.Style;

    public event Action OnClick;
    //public event Action OnHover;
    public void Update()
    {
        if (_label.Bounds.Contains(InputManager.ScreenMousePos))
        {
            if (InputManager.IsMouseLeftClick())
            {
                _label.SetStyle(ClickStyle);
                OnClick?.Invoke();
            }
            else
            {
                _label.SetStyle(HoverStyle);
                //OnHover?.Invoke(this);
            }
        }
        else
        {
            _label.SetStyle(DefaultStyle);
        }

    }
    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        _label.Draw(spriteBatch, font);
    }
}

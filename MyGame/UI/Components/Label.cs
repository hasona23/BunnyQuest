using Microsoft.Xna.Framework.Graphics;
using MyGame.Utils;

namespace MyGame.UI.Components;
internal class Label(string text, Vector2 pos, float scale)
{
    public string Text { get; set; } = text;
    public float TextScale { get; set; } = scale;
    public Vector2 Pos { get; set; } = pos;
    public Alignment TextAlignment { get; set; }
    public Vector2 Padding { get; set; }
    public Rectangle Bounds;
    public UiStyle Style { get; set; }
    public Label SetAligntment(Alignment alignment)
    {
        TextAlignment = alignment;
        return this;
    }
    public Label SetStyle(UiStyle style)
    {
        //Console.WriteLine($"BEFORE:{Style.BackgroundColor}");
        Style = style;
        //Console.WriteLine($"After:{Style.BackgroundColor}");
        return this;
    }
    public Label AddPadding(Vector2 padding)
    {
        Padding = padding;
        return this;
    }
    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        Vector2 size = font.MeasureString(Text);
        size *= TextScale;




        Bounds = new Rectangle(Pos.ToPoint(), (size + Padding).ToPoint());
        Bounds.Location -= (Bounds.Size.ToVector2() / 2).ToPoint();

        spriteBatch.Draw(AssetManager.Pixel, Bounds, Style.BackgroundColor);


        Vector2 textPos = Bounds.Location.ToVector2();
        textPos.Y += Padding.Y / 2;
        if (TextAlignment == Alignment.Left)
            textPos.X += Padding.X / 8;
        if (TextAlignment == Alignment.Centre)
            textPos.X += Padding.X / 2;
        if (TextAlignment == Alignment.Right)
            textPos.X += Padding.X * 7 / 8;

        spriteBatch.DrawString(font, Text, textPos.ToPoint().ToVector2(), Style.TextColor, 0, Vector2.Zero, TextScale, SpriteEffects.None, 0);
        if (Style.Border != null)
        {
            DebugRect rect = new DebugRect(Bounds, Style.Border.Value.Color);
            rect.DrawHollow(spriteBatch, Style.Border.Value.Thickness);

        }


    }
}

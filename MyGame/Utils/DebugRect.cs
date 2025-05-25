using Microsoft.Xna.Framework.Graphics;
namespace MyGame.Utils;
internal struct DebugRect
{
    public Rectangle Rect;
    public Color Color;
    public DebugRect(Rectangle rect, Color color)
    {
        Rect = rect;
        Color = color;
    }

    public void Draw(SpriteBatch spriteBatch, float transparency = 1)
    {
        spriteBatch.Draw(AssetManager.Pixel, Rect, Color * transparency);
    }

    public void DrawHollow(SpriteBatch spriteBatch, float transparency = 1)
    {
        //TOP
        spriteBatch.Draw(AssetManager.Pixel, new Rectangle(Rect.Left, Rect.Top, Rect.Width, 1), Color * transparency);
        //BOTTOM
        spriteBatch.Draw(AssetManager.Pixel, new Rectangle(Rect.Left, Rect.Bottom - 1, Rect.Width, 1), Color * transparency);
        //RIGHT
        spriteBatch.Draw(AssetManager.Pixel, new Rectangle(Rect.Right - 1, Rect.Top, 1, Rect.Height), Color * transparency);
        //LEFT
        spriteBatch.Draw(AssetManager.Pixel, new Rectangle(Rect.Left, Rect.Top, 1, Rect.Height), Color * transparency);
    }
}

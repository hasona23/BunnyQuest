using Microsoft.Xna.Framework.Graphics;
using MyGame.Data;
using System.Collections.Generic;

namespace MyGame;
internal static class AssetManager
{
    private static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

    public static void Init(Game game)
    {
        Textures["Pixel"] = new Texture2D(game.GraphicsDevice, 1, 1);
        Textures["Pixel"].SetData([Color.White]);
        Textures["SpriteSheet"] = game.Content.Load<Texture2D>("Sprites/tilemap_packed");

    }
    public static Texture2D Pixel => Textures["Pixel"];

    public static Texture2D SpriteSheet => Textures["SpriteSheet"];
    public static Rectangle GetSrc(int id) => new Rectangle(id % (SpriteSheet.Width / GameData.TILE_SIZE) * 16,
        id / (SpriteSheet.Width / GameData.TILE_SIZE) * GameData.TILE_SIZE, GameData.TILE_SIZE, GameData.TILE_SIZE);
}

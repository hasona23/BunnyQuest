using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Data;
using System.Collections.Generic;

namespace MyGame;
internal static class AssetManager
{
    private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
    public static Dictionary<string, SoundEffectInstance> Sounds = new Dictionary<string, SoundEffectInstance>();
    public static void Init(Game game)
    {
        _textures["Pixel"] = new Texture2D(game.GraphicsDevice, 1, 1);
        _textures["Pixel"].SetData([Color.White]);
        _textures["SpriteSheet"] = game.Content.Load<Texture2D>("Sprites/tilemap_packed");
        Sounds["Jump"] = game.Content.Load<SoundEffect>("Sounds/jump").CreateInstance();
        Sounds["Shoot"] = game.Content.Load<SoundEffect>("Sounds/laserShoot").CreateInstance();
        Sounds["PowerUp"] = game.Content.Load<SoundEffect>("Sounds/powerUp").CreateInstance();
        Sounds["Explode"] = game.Content.Load<SoundEffect>("Sounds/explosion").CreateInstance();
    }
    public static Texture2D Pixel => _textures["Pixel"];

    public static Texture2D SpriteSheet => _textures["SpriteSheet"];
    public static Rectangle GetSrc(int id) => new Rectangle(id % (SpriteSheet.Width / GameData.TILE_SIZE) * 16,
        id / (SpriteSheet.Width / GameData.TILE_SIZE) * GameData.TILE_SIZE, GameData.TILE_SIZE, GameData.TILE_SIZE);
}

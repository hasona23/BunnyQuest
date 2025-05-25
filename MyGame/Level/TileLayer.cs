using Microsoft.Xna.Framework.Graphics;
using MyGame.Data;
using MyGame.Utils;
using System.Collections.Generic;

namespace MyGame.Level;
internal class TileLayer
{
    //Pos and Id
    public Dictionary<Vector2, int> Tiles { get; set; }
    public string Name { get; set; }

    public bool IsCollidable { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public void Draw(SpriteBatch spriteBatch)
    {
        int spriteSheetWidth = AssetManager.SpriteSheet.Width / GameData.TILE_SIZE;

        foreach (var tile in Tiles)
        {

            int srcX = tile.Value % spriteSheetWidth;
            int srcY = tile.Value / spriteSheetWidth;
            srcX *= GameData.TILE_SIZE;
            srcY *= GameData.TILE_SIZE;
            spriteBatch.Draw(AssetManager.SpriteSheet,
            new Rectangle((tile.Key * GameData.TILE_SIZE).ToPoint(), new(GameData.TILE_SIZE)),
            AssetManager.GetSrc(tile.Value),
            Color.White);
            if (IsCollidable && GameData.IsDebug)
            {
                DebugRect rect = new DebugRect(new Rectangle((tile.Key * GameData.TILE_SIZE).ToPoint(), new(GameData.TILE_SIZE))
                    , Color.Black);
                rect.DrawHollow(spriteBatch);
            }
        }

    }
}
internal struct Tile
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

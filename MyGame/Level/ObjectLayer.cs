using Microsoft.Xna.Framework.Graphics;
using MyGame.Data;
using System.Text.Json.Serialization;

namespace MyGame.Level;
internal class ObjectLayer
{
    public Obj[] Objects { get; set; }
    public void Draw(SpriteBatch spriteBatch)
    {
        int spriteSheetWidth = AssetManager.SpriteSheet.Width / GameData.TILE_SIZE;
        foreach (var obj in Objects)
        {


            spriteBatch.Draw(AssetManager.SpriteSheet,
             new Rectangle(new(obj.X, obj.Y), new(GameData.TILE_SIZE)),
             AssetManager.GetSrc(obj.Id),
             Color.White);
        }
    }
}

public struct Obj
{
    public int X { get; set; }
    public int Y { get; set; }
    [JsonPropertyName("gid")]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
}

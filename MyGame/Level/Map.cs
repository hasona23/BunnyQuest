using Microsoft.Xna.Framework.Graphics;
using MyGame.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MyGame.Level;
internal class Map
{

    public Dictionary<string, TileLayer> Tilemap;
    public List<string> collisionLayers = new List<string>(4);
    public Dictionary<string, ObjectLayer> ObjectMap;
    public int Width;
    public int Height;

    public Map(string name)
    {
        string path = $"./{GameData.BASE_MAP_PATH}/{name}";
        XmlDocument xml = new XmlDocument();
        xml.Load(path);
        Width = int.Parse(xml.GetElementsByTagName("map")[0].Attributes["width"].Value);
        Height = int.Parse(xml.GetElementsByTagName("map")[0].Attributes["height"].Value);

        ParseTiles(xml);
        ParseObjects(xml);


    }
    private void ParseTiles(XmlDocument xml)
    {
        var tileLayers = xml.GetElementsByTagName("layer");
        Tilemap = new Dictionary<string, TileLayer>(tileLayers.Count);
        for (int i = 0; i < tileLayers.Count; i++)
        {
            XmlNode layer = tileLayers[i];
            string layerName = layer.Attributes["name"].Value;
            bool isCollidable = layer.Attributes["class"] != null && layer.Attributes["class"].Value == "Collision";
            Tilemap[layerName] = new()
            {
                Width = Width,
                Height = Height,

                Name = layer.Attributes["name"].Value,
                IsCollidable = isCollidable,
            };


            if (isCollidable)
                collisionLayers.Add(Tilemap[layerName].Name);
            IEnumerable<int> buffer;

            buffer = layer["data"].InnerText.Split(",").Select(i => int.Parse(i) - 1);

            Tilemap[layerName].Tiles = new Dictionary<Vector2, int>(buffer.Count());
            for (int j = 0; j < buffer.Count(); j++)
            {
                if (buffer.ElementAt(j) >= 0)
                {
                    int x = j % Width;
                    int y = j / Width;
                    Tilemap[layerName].Tiles[new Vector2(x, y)] = buffer.ElementAt(j);
                }

            }
        }
    }
    private void ParseObjects(XmlDocument xml)
    {
        var objectLayers = xml.GetElementsByTagName("objectgroup");
        ObjectMap = new Dictionary<string, ObjectLayer>(objectLayers.Count);
        for (int i = 0; i < objectLayers.Count; i++)
        {
            XmlNode item = objectLayers[i];

            string layerName = item.Attributes["name"].Value;
            ObjectMap[layerName] = new ObjectLayer();

            ObjectMap[layerName].Objects = new Obj[item.ChildNodes.Count];
            for (int j = 0; j < item.ChildNodes.Count; j++)
            {
                XmlNode node = item.ChildNodes[j];

                ObjectMap[layerName].Objects[j] = new Obj()
                {
                    X = int.Parse(node.Attributes["x"].Value),
                    Y = int.Parse(node.Attributes["y"].Value) - GameData.TILE_SIZE,
                    Id = int.Parse(node.Attributes["gid"].InnerText) - 1,
                    Name = node.Attributes["name"] == null ? "" : node.Attributes["name"].Value,
                    Type = node.Attributes["class"] == null ? "" : node.Attributes["class"].Value,
                };

            }
        }
    }
    public Obj Player()
    {
        return ObjectMap[GameData.PLAYER_GROUP].Objects[0];
    }
    public Obj[] Enemies()
    {
        return ObjectMap[GameData.ENEMIES_GROUP].Objects;
    }
    public Obj[] Checkpoints()
    {
        return ObjectMap[GameData.CHECKPOINTS_GROUP].Objects;
    }
    public Obj[] Items()
    {
        return ObjectMap[GameData.ITEMS_GROUP].Objects;
    }
    public List<Rectangle> GetNearRects(Vector2 pos)
    {
        pos = Vector2.Floor(pos / 16);
        Vector2[] offsets = [new(1, 1), new(1, 0), new(1, -1), new(0, 1), new(0, 0), new(0, -1), new(-1, 0), new(-1, 1), new(-1, 1)];
        List<Rectangle> rects = new List<Rectangle>(9);
        foreach (var offset in offsets)
        {
            if (Tilemap[collisionLayers[0]].Tiles.ContainsKey(pos + offset))
            {
                rects.Add(new(((pos + offset) * GameData.TILE_SIZE).ToPoint(), new Point(GameData.TILE_SIZE)));
            }
        }
        return rects;
    }
    public void Draw(SpriteBatch spriteBatch, bool drawObjects = false)
    {
        foreach (var item in Tilemap)
        {
            item.Value.Draw(spriteBatch);
        }
        if (drawObjects)
        {
            foreach (var item in ObjectMap)
            {
                item.Value.Draw(spriteBatch);
            }
        }
    }
}

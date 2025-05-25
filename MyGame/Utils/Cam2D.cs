using MyGame.Data;

namespace MyGame.Utils;
internal class Cam2D
{
    public Vector2 Pos;
    public Vector2? Min;
    public Vector2? Max;

    private Vector2 Offset = new Vector2(GameData.WINDOW_SIZE.X / 2, GameData.WINDOW_SIZE.Y / 2);
    private float scale = 1f;
    public Cam2D(Vector2 pos, Vector2? min = null, Vector2? max = null)
    {
        Pos = pos; Min = min; Max = max;
    }
    public void Follow(Vector2 target, float lerpFactor = 1)
    {
        Pos = Vector2.Lerp(Pos, target - Offset, lerpFactor);

        if (Min.HasValue)
        {
            Pos = Vector2.Max(Pos, Min.Value);
        }
        if (Max.HasValue)
        {
            Pos = Vector2.Min(Pos, Max.Value - new Vector2(320, 180) / scale);
        }


    }
    public Matrix GetCamMatrix()
    {
        return
              Matrix.CreateTranslation(new Vector3(-Pos, 0))
            * Matrix.CreateScale(new Vector3(scale, scale, 1));
    }
    public Matrix GetCamParallaxMatrix(int parallexLevel)
    {
        return
             Matrix.CreateTranslation(new Vector3(-Pos / parallexLevel, 0))
           * Matrix.CreateScale(new Vector3(scale, scale, 1));
    }
}

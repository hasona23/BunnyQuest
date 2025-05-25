using Microsoft.Xna.Framework.Input;
using MyGame.Utils;

namespace SpaceCup.Utils;
internal static class InputManager
{
    private static KeyboardState prevKState;
    private static KeyboardState currentKState;
    private static MouseState prevMouseState;
    private static MouseState currentMouseState;
    public static Vector2 WorldMousePos { get; private set; }
    public static Vector2 ScreenMousePos { get; private set; }
    public static void Update(WindowResolutionHandler windowResolutionHandler, Cam2D cam = null)
    {
        prevKState = currentKState;
        currentKState = Keyboard.GetState();
        prevMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
        WorldMousePos = windowResolutionHandler.GetMousePos();
        if (cam != null)
        {
            WorldMousePos = Vector2.Transform(WorldMousePos, Matrix.Invert(cam.GetCamMatrix()));
        }
        ScreenMousePos = windowResolutionHandler.GetMousePos();
    }
    public static bool IsKeyPressed(Keys key)
    {
        return currentKState.IsKeyDown(key) && prevKState.IsKeyUp(key);
    }
    public static bool IsKeyDown(Keys key)
    {
        return currentKState.IsKeyDown(key);
    }
    public static bool IsKeyUp(Keys key)
    {
        return currentKState.IsKeyUp(key);
    }
    public static bool IsMouseRightClick()
    {
        return currentMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton != ButtonState.Pressed;
    }
    public static bool IsMouseLeftClick()
    {
        return currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed;
    }
    public static bool IsMouseLeftDown()
    {
        return currentMouseState.LeftButton == ButtonState.Pressed;
    }
    public static bool IsMouseRightDown()
    {
        return currentMouseState.RightButton == ButtonState.Pressed;
    }

}

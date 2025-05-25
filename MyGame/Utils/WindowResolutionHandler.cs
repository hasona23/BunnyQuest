using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace MyGame.Utils
{
    internal class WindowResolutionHandler : IDisposable
    {
        public RenderTarget2D Screen;
        public Rectangle DestinationRect;
        public bool IsResizing;
        private readonly int _resolutionWidth, _resolutionHeight;
        public float ScaleFactor { get; private set; }
        private GraphicsDevice _graphicsDevice;
        public WindowResolutionHandler(GraphicsDeviceManager graphics, GameWindow window, int resolutionWidth, int resolutionHeight)
        {
            Screen = new RenderTarget2D(graphics.GraphicsDevice, resolutionWidth, resolutionHeight);
            DestinationRect = new Rectangle(0, 0, resolutionWidth, resolutionHeight);
            IsResizing = false;
            _resolutionWidth = resolutionWidth;
            _resolutionHeight = resolutionHeight;
            _graphicsDevice = graphics.GraphicsDevice;
            graphics.PreferredBackBufferWidth = resolutionWidth;
            graphics.PreferredBackBufferHeight = resolutionHeight;
            graphics.ApplyChanges();
            window.AllowUserResizing = true;
            window.ClientSizeChanged += ResizeWindow;
            Console.WriteLine($"Initialised Window Resolution Handler with default Resolution {_resolutionWidth}x{_resolutionHeight}");
            ScaleFactor = 1;
        }
        private void CalculateDestinationRectangle()
        {
            float scaleX = (float)_graphicsDevice.Viewport.Width / _resolutionWidth;
            float scaleY = (float)_graphicsDevice.Viewport.Height / _resolutionHeight;
            ScaleFactor = Math.Min(scaleX, scaleY);
            DestinationRect.Width = (int)(_resolutionWidth * ScaleFactor);
            DestinationRect.Height = (int)(_resolutionHeight * ScaleFactor);
            DestinationRect.X = (_graphicsDevice.Viewport.Width - DestinationRect.Width) / 2;
            DestinationRect.Y = (_graphicsDevice.Viewport.Height - DestinationRect.Height) / 2;
        }
        public void ResizeWindow(object sender, EventArgs args)
        {
            if (!IsResizing)
            {
                IsResizing = true;
                CalculateDestinationRectangle();
                IsResizing = false;
            }
        }
        public Vector2 GetMousePos()
        {
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();
            mousePos -= DestinationRect.Location.ToVector2();
            mousePos /= DestinationRect.Size.ToVector2() / new Vector2(_resolutionWidth, _resolutionHeight);
            return mousePos;
        }

        public void Dispose()
        {
            Screen.Dispose();
            _graphicsDevice = null;
        }

    }
}
global using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Data;
using MyGame.EntitySystem;
using MyGame.UI;
using MyGame.UI.Components;
using MyGame.Utils;
using SpaceCup.Utils;
using System;
using System.IO;

namespace MyGame
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private WindowResolutionHandler _windowResolutionHandler;

        private SpriteFont _font;
        private MainMenu _mainMenu;
        private PauseMenu _pauseMenu;
        private DeathScreen _deathScreen;
        private WinningScreen _winningScreen;
        private World _world;
        private int _totalLevels;
        private int currentLevel = 2;
        private float _fps = 0;
        public static GameState GameState = GameState.MainMenu;
        private Button _pauseButton;
        private Effect _outLineEffect;
        private RenderTarget2D _textScreen;
        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            _totalLevels = Directory.GetFiles(Path.GetFullPath(GameData.BASE_MAP_PATH)).Length;



        }

        public void ResetLevel()
        {
            _world.Reset();
            GameState = GameState.Game;

        }
        protected override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization logic here
            _world = new World($"level{currentLevel}.tmx");
            _mainMenu = new MainMenu(this, _font);
            _pauseMenu = new PauseMenu(this, _font);
            _deathScreen = new DeathScreen(this, _font);
            _pauseButton = new Button(new Label("||", new Vector2(10, 10), GameData.TEXT_SMALL)
                .AddPadding(new Vector2(10, 5)).SetAligntment(Alignment.Centre)
                .SetStyle(new UiStyle(Color.Black, Color.White, new Border(Color.White, 1))));
            _pauseButton.HoverStyle = new UiStyle(Color.DarkGray, Color.White, new Border(Color.White, 1));
            _pauseButton.OnClick += () => GameState = GameState.Pause;
            _winningScreen = new WinningScreen(this, _font);

            _textScreen = new RenderTarget2D(GraphicsDevice, GameData.WINDOW_SIZE.X, GameData.WINDOW_SIZE.Y);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _windowResolutionHandler = new WindowResolutionHandler(_graphics, Window, GameData.WINDOW_SIZE.X, GameData.WINDOW_SIZE.Y);
            // TODO: use this.Content to load your game content here
            AssetManager.Init(this);
            _font = Content.Load<SpriteFont>("Fonts/Font");
            _graphics.IsFullScreen = true;
            _outLineEffect = Content.Load<Effect>("Effects/Effect");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Window.Title = $"FPS:{1 / dt:00}";
            InputManager.Update(_windowResolutionHandler, _world.Cam);
            _fps = 1 / dt;

            if (!IsActive && GameState == GameState.Game)
                GameState = GameState.Pause;

            if (GameState == GameState.MainMenu)
                _mainMenu.Update();
            if (GameState == GameState.Pause)
                _pauseMenu.Update();
            if (GameState == GameState.Death)
                _deathScreen.Update();
            if (GameState == GameState.Won)
                _winningScreen.Update();
            if (GameState == GameState.Game || GameState == GameState.Death)
            {

                // TODO: Add your update logic here
                _pauseButton.Update();
                _world.Update(dt);
                if (_world.Finish)
                {
                    if (currentLevel < _totalLevels)
                    {
                        currentLevel++;
                        _world = new World($"level{currentLevel}.tmx");
                    }
                    else
                    {
                        GameState = GameState.Won;
                    }
                }
            }
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GameState == GameState.Game)
            {

                GraphicsDevice.SetRenderTarget(_textScreen);
                GraphicsDevice.Clear(Color.Transparent);
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                Vector2 size;

                _spriteBatch.DrawString(_font, $"Ammo:{_world.Player.GetComponent<Gun>()?.Ammo}",
                       new Vector2(10, 24 + (Keyboard.GetState().IsKeyDown(Keys.Y) ? 10 : 0)), Color.White, 0, Vector2.Zero, 1 / 6f, SpriteEffects.None, 0);


                size = _font.MeasureString($"{_world.Name}");

                _spriteBatch.DrawString(_font, $"{_world.Name}",
                    new Vector2(GameData.WINDOW_SIZE.X / 2f, 20), Color.White, 0, size / 2, 1 / 6f, SpriteEffects.None, 0);

                _spriteBatch.End();



            }

            GraphicsDevice.SetRenderTarget(_windowResolutionHandler.Screen);
            GraphicsDevice.Clear(new Color(252, 223, 205));


            // TODO: Add your drawing code here
            if (GameState != GameState.MainMenu)
            {
                _world.Draw(_spriteBatch, dt);
            }
            //UI
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (GameState == GameState.MainMenu)
                _mainMenu.Draw(_spriteBatch);

            if (GameState == GameState.Pause)
                _pauseMenu.Draw(_spriteBatch);

            if (GameState == GameState.Death)
                _deathScreen.Draw(_spriteBatch);

            if (GameState == GameState.Won)
                _winningScreen.Draw(_spriteBatch);

            if (GameState == GameState.Game)
            {

                _pauseButton.Draw(_spriteBatch, _font);



            }

            _spriteBatch.End();
            if (GameState == GameState.Game)
            {
                _outLineEffect.Parameters["xOutlineColour"]?.SetValue(Color.MonoGameOrange.ToVector4());
                _outLineEffect.Parameters["xTextureSize"]?.SetValue(_textScreen.Bounds.Size.ToVector2());
                _outLineEffect.Parameters["xTime"]?.SetValue(MathF.PI / 10);
                _outLineEffect.Parameters["xFontColour"]?.SetValue((Color.Lerp(Color.White, Color.AntiqueWhite,
                    MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds * 2)).ToVector4()));
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _outLineEffect);
                _spriteBatch.Draw(_textScreen, _textScreen.Bounds, Color.White);
                _spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_windowResolutionHandler.Screen, _windowResolutionHandler.DestinationRect, Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}



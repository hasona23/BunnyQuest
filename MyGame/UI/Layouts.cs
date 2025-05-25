using Microsoft.Xna.Framework.Graphics;
using MyGame.Data;
using MyGame.UI.Components;

namespace MyGame.UI;
public static class Style
{
    internal static readonly UiStyle ButtonStyle = new UiStyle(Color.MonoGameOrange * .25f, Color.White, new Border(Color.White, 2));
    internal static readonly UiStyle ButtonHoverStyle = new UiStyle(Color.MonoGameOrange * .5f, Color.White, new Border(Color.Black, 2));
    internal static readonly UiStyle ButtonClickStyle = new UiStyle(Color.MonoGameOrange * .75f, Color.White, new Border(Color.Black, 2));
    internal static readonly float TransparencyFactor = .5f;
}



internal class MainMenu
{
    public Label Title;
    public Button StartButton;
    public Button ExitButton;
    public SpriteFont Font;
    public MainMenu(Main main, SpriteFont font)
    {
        Font = font;
        StartButton = new Button(new Label("Start_Game", GameData.WINDOW_SIZE.ToVector2() / 2 + Vector2.UnitY * 30, GameData.TEXT_SMALL)
            .SetAligntment(Alignment.Centre)
            .AddPadding(new Vector2(20, 5))
            .SetStyle(Style.ButtonStyle));
        StartButton.HoverStyle = Style.ButtonHoverStyle;
        StartButton.ClickStyle = Style.ButtonClickStyle;
        StartButton.OnClick += () => { main.ResetLevel(); };

        ExitButton = new Button(new Label("Exit", GameData.WINDOW_SIZE.ToVector2() / 2 + Vector2.UnitY * 50, GameData.TEXT_SMALL)
            .SetAligntment(Alignment.Centre)
            .AddPadding(new Vector2(40, 5))
            .SetStyle(Style.ButtonStyle));
        ExitButton.HoverStyle = Style.ButtonHoverStyle;
        ExitButton.ClickStyle = Style.ButtonClickStyle;
        ExitButton.OnClick += () => main.Exit();

        Title = new Label(GameData.TITLE, GameData.WINDOW_SIZE.ToVector2() / 2 - Vector2.UnitY * 40, GameData.TEXT_LARGE)
            .SetAligntment(Alignment.Centre)
            .SetStyle(new UiStyle(Color.Transparent, Color.White));


    }

    public void Update()
    {

        StartButton.Update();
        ExitButton.Update();
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        //spriteBatch.Draw(AssetManager.Pixel, new Rectangle(Point.Zero, GameData.WINDOW_SIZE), Color.DarkGray);
        StartButton.Draw(spriteBatch, Font);
        ExitButton.Draw(spriteBatch, Font);
        Title.Draw(spriteBatch, Font);
    }
}
internal class PauseMenu
{

    public Button resumeB;
    public Button BackToMenu;
    public SpriteFont Font;
    public PauseMenu(Main main, SpriteFont font)
    {
        Font = font;
        resumeB = new Button(new Label("Resume", GameData.WINDOW_SIZE.ToVector2() / 2 + Vector2.UnitY * 30, GameData.TEXT_SMALL)
            .SetAligntment(Alignment.Centre)
            .AddPadding(new Vector2(57.5f, 5))
            .SetStyle(Style.ButtonStyle));
        resumeB.HoverStyle = Style.ButtonHoverStyle;
        resumeB.ClickStyle = Style.ButtonClickStyle;
        resumeB.OnClick += () => Main.GameState = GameState.Game;


        BackToMenu = new Button(new Label("Back to menu", GameData.WINDOW_SIZE.ToVector2() / 2 + Vector2.UnitY * 50, GameData.TEXT_SMALL)
           .SetAligntment(Alignment.Centre)
           .AddPadding(new Vector2(20, 5))
           .SetStyle(Style.ButtonStyle));
        BackToMenu.HoverStyle = Style.ButtonHoverStyle;
        BackToMenu.ClickStyle = Style.ButtonClickStyle;
        BackToMenu.OnClick += () => { Main.GameState = GameState.MainMenu; };



    }

    public void Update()
    {

        resumeB.Update();
        BackToMenu.Update();
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(AssetManager.Pixel, new Rectangle(Point.Zero, GameData.WINDOW_SIZE), Color.DarkGray * Style.TransparencyFactor);
        resumeB.Draw(spriteBatch, Font);
        BackToMenu.Draw(spriteBatch, Font);
        Vector2 size = Font.MeasureString("PAUSE") * GameData.TEXT_LARGE;
        spriteBatch.DrawString(Font, "PAUSE", GameData.WINDOW_SIZE.ToVector2() / 2 - size / 2 - Vector2.UnitY * 30,
            Color.White, 0, Vector2.Zero, GameData.TEXT_LARGE, SpriteEffects.None, 0);
    }
}
internal class DeathScreen
{

    public Button Reset;
    public Button BackToMenu;
    public SpriteFont Font;
    public DeathScreen(Main main, SpriteFont font)
    {
        Font = font;
        Reset = new Button(new Label("Restart", GameData.WINDOW_SIZE.ToVector2() / 2 + Vector2.UnitY * 30, GameData.TEXT_SMALL)
            .SetAligntment(Alignment.Centre)
            .AddPadding(new Vector2(60f, 5))
            .SetStyle(Style.ButtonStyle));
        Reset.HoverStyle = Style.ButtonHoverStyle;
        Reset.ClickStyle = Style.ButtonClickStyle;
        Reset.OnClick += main.ResetLevel;


        BackToMenu = new Button(new Label("Back to menu", GameData.WINDOW_SIZE.ToVector2() / 2 + Vector2.UnitY * 50, GameData.TEXT_SMALL)
           .SetAligntment(Alignment.Centre)
           .AddPadding(new Vector2(20, 5))
           .SetStyle(Style.ButtonStyle));
        BackToMenu.HoverStyle = Style.ButtonHoverStyle;
        BackToMenu.ClickStyle = Style.ButtonClickStyle;
        BackToMenu.OnClick += () => { Main.GameState = GameState.MainMenu; };


    }

    public void Update()
    {

        Reset.Update();
        BackToMenu.Update();
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(AssetManager.Pixel, new Rectangle(Point.Zero, GameData.WINDOW_SIZE), Color.DarkGray * Style.TransparencyFactor);
        Reset.Draw(spriteBatch, Font);
        BackToMenu.Draw(spriteBatch, Font);
        Vector2 size = Font.MeasureString("YOU DIED!") * GameData.TEXT_LARGE;
        spriteBatch.DrawString(Font, "YOU DIED!", GameData.WINDOW_SIZE.ToVector2() / 2 - size / 2 - Vector2.UnitY * 30,
            Color.White, 0, Vector2.Zero, GameData.TEXT_LARGE, SpriteEffects.None, 0);
    }
}
internal class WinningScreen
{



    public Button BackToMenu;
    public SpriteFont Font;

    public WinningScreen(Main main, SpriteFont font)
    {
        Font = font;

        BackToMenu = new Button(new Label("Back to menu", GameData.WINDOW_SIZE.ToVector2() / 2 + Vector2.UnitY * 30, GameData.TEXT_SMALL)
           .SetAligntment(Alignment.Centre)
           .AddPadding(new Vector2(20, 5))
           .SetStyle(Style.ButtonStyle));
        BackToMenu.HoverStyle = Style.ButtonHoverStyle;
        BackToMenu.ClickStyle = Style.ButtonClickStyle;
        BackToMenu.OnClick += () => { Main.GameState = GameState.MainMenu; };

    }

    public void Update()
    {
        BackToMenu.Update();
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(AssetManager.Pixel, new Rectangle(Point.Zero, GameData.WINDOW_SIZE), Color.DarkGray * Style.TransparencyFactor);
        BackToMenu.Draw(spriteBatch, Font);
        Vector2 size = Font.MeasureString("YOU WON!") * GameData.TEXT_LARGE;
        spriteBatch.DrawString(Font, "YOU WON!", GameData.WINDOW_SIZE.ToVector2() / 2 - size / 2 - Vector2.UnitY * 30,
            Color.White, 0, Vector2.Zero, GameData.TEXT_LARGE, SpriteEffects.None, 0);
    }

}
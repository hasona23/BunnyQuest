namespace MyGame.UI.Components;
internal readonly struct Border(Color borderColor, float thickness)
{
    public readonly Color Color = borderColor;
    public readonly float Thickness = thickness;
}
internal readonly struct UiStyle(Color backGroundColor, Color textColor, Border? border = null)
{
    public readonly Color BackgroundColor = backGroundColor;
    public readonly Color TextColor = textColor;
    public readonly Border? Border = border;
}
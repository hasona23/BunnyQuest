namespace MyGame.Data;
internal static class GameData
{
    public static string TITLE = "BUNNY_QUEST";
    public static Point WINDOW_SIZE => new(320, 180);
    public static string BASE_MAP_PATH = "./Content/Maps";
    public static int TILE_SIZE = 16;
    public static bool IsDebug = false;

    //Object groups
    public static string ENEMIES_GROUP = "Enemies";
    public static string PLAYER_GROUP = "Player";
    public static string CHECKPOINTS_GROUP = "Checkpoints";
    public static string ITEMS_GROUP = "Items";

    //UI
    public const float TEXT_LARGE = 1 / 3F;
    public const float TEXT_MEDUIM = 1 / 4F;
    public const float TEXT_SMALL = 1 / 6F;
}

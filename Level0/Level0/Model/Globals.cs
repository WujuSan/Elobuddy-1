using EloBuddy.SDK.Menu;

namespace LevelZero.Model
{
    static class Globals
    {

        public static readonly string ADDON_NAME = "Level 0";
        public static Menu MENU = MainMenu.AddMenu(ADDON_NAME, ADDON_NAME);

    }
}

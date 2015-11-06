using EloBuddy;
using EloBuddy.SDK.Menu;

namespace OneForWeek.Plugin
{
    public abstract class PluginModel
    {
        #region Global Variables

        /*
         Config
         */

        public static readonly string GVersion = "1.3.1";
        public static readonly string GCharname = _Player.ChampionName;

        /*
         Menus
         */

        public static Menu Menu,
            ComboMenu,
            LaneClearMenu,
            LastHitMenu,
            HarassMenu,
            MiscMenu,
            DrawMenu;


        /*
         Misc
         */

        public static AIHeroClient Target;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion
    }
}

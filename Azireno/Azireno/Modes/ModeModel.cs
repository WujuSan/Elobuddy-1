﻿using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace Azireno.Modes
{
    abstract class ModeModel
    {
        #region Global Variables

        /*
         Config
         */
        
        public static string G_version = "1.1.1";
        public static string G_charname = _Player.ChampionName;

        /*
         Spells
         */
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Skillshot R;

        /*
         Menus
         */

        public static Menu Menu,
            ComboMenu,
            LaneClearMenu,
            HarassMenu,
            MiscMenu,
            DrawMenu;

        /*
         Misc
         */

        public static AIHeroClient _target;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion
    }
}
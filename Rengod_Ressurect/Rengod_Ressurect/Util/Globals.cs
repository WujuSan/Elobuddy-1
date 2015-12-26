using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;

namespace Rengod_Ressurect.Util
{
    static class Globals
    {
        #region Menu Util Stuff
        public static readonly string CHAR_NAME = Player.Instance.ChampionName;
        public static readonly string VERSION = "0.1.1";
        #endregion

        #region SpellNames and Buffs
        public static readonly string RENGAR_PASSIVE_NAME = "rengarpassivebuff";
        public static readonly string RENGAR_PASSIVE_DISPLAY_NAME = "RengarPassiveBuff";
        public static readonly string RENGAR_UTIMATE_DISPLAY_NAME = "RengarRBuff";
        public static readonly string RENGAR_UTIMATE_NAME = "RengarR";
        public static readonly string RENGAR_Q_NAME = "RengarQ";
        public static readonly string RENGAR_W_NAME = "RengarW";
        public static readonly string RENGAR_E_NAME = "RengarE";
        #endregion

        #region Instace Utils
        public static int COMBO_MODE = 1;
        public static bool HARASS_FEROCITY = false;
        public static int HARASS_FEROCITY_MODE = 3;
        #endregion

    }
}
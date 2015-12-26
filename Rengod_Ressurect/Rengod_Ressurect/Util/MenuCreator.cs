using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Rengod_Ressurect.Util
{
    static class MenuCreator
    {
        public static Menu Menu;
        public static Menu DrawMenu;
        public static Menu ComboMenu;
        public static Menu HarassMenu;
        public static Menu LaneClearMenu;
        public static Menu JungleClearMenu;
        public static Menu LastHitMenu;
        public static Menu MiscMenu;

        public static void Init()
        {
            Menu = MainMenu.AddMenu(Globals.CHAR_NAME, Globals.CHAR_NAME);

            Menu.AddLabel("Version: " + Globals.VERSION);
            Menu.AddSeparator();
            Menu.AddLabel("By MrArticuno");

            DrawMenu = Menu.AddSubMenu("Draw - " + Globals.CHAR_NAME, Globals.CHAR_NAME + "Draw");
            DrawMenu.AddGroupLabel("Draw");
            DrawMenu.Add("draw.disable", new CheckBox("Turn off all drawings", false));
            DrawMenu.Add("draw.damage", new CheckBox("Draw Damage indicator", false));
            DrawMenu.Add("draw.w", new CheckBox("Draw W Range", false));
            DrawMenu.Add("draw.e", new CheckBox("Draw E Range", false));

            ComboMenu = Menu.AddSubMenu("Combo - " + Globals.CHAR_NAME, Globals.CHAR_NAME + "Combo");
            ComboMenu.AddGroupLabel("Combo");
            ComboMenu.AddLabel("Type Combo (1 - Burst; 2 - Continous; 3 - Survival;)");
            var comboType = ComboMenu.Add("combo.type",new Slider("Combo Type", 1, 1, 3));
            comboType.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                Globals.COMBO_MODE = comboType.CurrentValue;
            };
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("Burst: Focus on do maximum damage usually focus Q. \n" +
                               "Continous: Focus on keep doing damage usually focus on E. \n" +
                               "Survival: Focus on regen of life, usually focus on W for AP Rengar.");

            HarassMenu = Menu.AddSubMenu("Harass - " + Globals.CHAR_NAME, Globals.CHAR_NAME + "Harass");
            HarassMenu.AddGroupLabel("Harass");
            HarassMenu.Add("hs.q", new CheckBox("Use Q", false));
            HarassMenu.Add("hs.w", new CheckBox("Use W", true));
            HarassMenu.Add("hs.e", new CheckBox("Use E", true));
            var ferocity = HarassMenu.Add("hs.ferocity", new CheckBox("Save Ferocity", false));
            ferocity.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                Globals.HARASS_FEROCITY = ferocity.CurrentValue;
            };
            HarassMenu.AddLabel("Use Ferocity on (1 - Q; 2 - W; 3 - E;)");
            var ferocityType = HarassMenu.Add("hs.ferocity.type", new Slider("Ferocity Chooser", 3, 1 , 3));
            ferocityType.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                Globals.HARASS_FEROCITY_MODE = ferocityType.CurrentValue;
            };

            LaneClearMenu = Menu.AddSubMenu("Lane Clear - " + Globals.CHAR_NAME, Globals.CHAR_NAME + "LaneClear");
            LaneClearMenu.AddGroupLabel("Lane Clear");
            LaneClearMenu.Add("lc.q", new CheckBox("Use Q", true));
            LaneClearMenu.Add("lc.w", new CheckBox("Use W", true));
            LaneClearMenu.Add("lc.e", new CheckBox("Use E", true));
            LaneClearMenu.Add("lc.ferocity", new CheckBox("Save Ferocity", true));
            LaneClearMenu.AddLabel("Use Ferocity on (1 - Q; 2 - W; 3 - E;)");
            LaneClearMenu.Add("lc.ferocity.type", new Slider("Ferocity Chooser", 2, 1, 3));

            JungleClearMenu = Menu.AddSubMenu("Jungle Clear - " + Globals.CHAR_NAME, Globals.CHAR_NAME + "JungleClear");
            JungleClearMenu.AddGroupLabel("Jungle Clear");
            JungleClearMenu.Add("jc.q", new CheckBox("Use Q", true));
            JungleClearMenu.Add("jc.w", new CheckBox("Use W", true));
            JungleClearMenu.Add("jc.e", new CheckBox("Use E", true));
            JungleClearMenu.Add("jc.ferocity", new CheckBox("Save Ferocity", false));
            JungleClearMenu.AddLabel("Use Ferocity on (1 - Q; 2 - W; 3 - E;)");
            JungleClearMenu.Add("jc.ferocity.type", new Slider("Ferocity Chooser", 1, 1, 3));

            MiscMenu = Menu.AddSubMenu("MISC - " + Globals.CHAR_NAME, Globals.CHAR_NAME + "Misc");
            MiscMenu.AddGroupLabel("MISC");
            MiscMenu.Add("misc.hydra", new CheckBox("Use Tiamat/Hydra", true));
            MiscMenu.Add("misc.yomumu", new CheckBox("Use Yomumu", true));
        }

        public static bool IsChecked(Menu obj, string value)
        {
            return obj[value].Cast<CheckBox>().CurrentValue;
        }

        public static int GetSliderValue(Menu obj, string value)
        {
            return obj[value].Cast<Slider>().CurrentValue;
        }

        public static bool IsKeyBindActive(Menu obj, string value)
        {
            return obj[value].Cast<KeyBind>().CurrentValue;
        }


    }
}

using System;
using System.Drawing;
using Ass_Zed.Common.AManager;
using Ass_Zed.Controller;
using Ass_Zed.Model.Enum;
using Ass_Zed.Model.Languages;
using BRSelector;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using OneForWeek.Draw.Notifications;
using OneForWeek.Model.Notification;
using OneForWeek.Util.Misc;

namespace Ass_Zed.Model
{
    class Champion : PluginModel
    {
        public new EnumModeManager ActiveMode { get; set; }

        public override void Init()
        {
            InitVariables();
            InitEvents();
            ModeManager.Initialize();
            Notification.DrawNotification(new NotificationModel(Game.Time, 5f, 1f, Player.Instance.ChampionName + " Loaded.", Color.Green));
        }

        public override void InitVariables()
        {
            Selector.Init();

            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Circular, 250, int.MaxValue);
            W = new Spell.Skillshot(SpellSlot.W, 550, SkillShotType.Linear, 500, int.MaxValue);
            E = new Spell.Active(SpellSlot.E, 290);
            R = new Spell.Targeted(SpellSlot.R, 625);

            ActionQueue = new ActionQueue();
            ComboQueue = new ActionQueueList();

            InitMenu();
        }

        public override void InitEvents()
        {
            Drawing.OnDraw += OnDraw;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Game.OnTick += OnGameUpdate;
        }

        public override void InitMenu()
        {
            Menu = MainMenu.AddMenu(GCharname, GCharname);

            Menu.AddLabel("Version: " + GVersion);
            Menu.AddSeparator();
            Menu.AddLabel("By MrArticuno");

            #region Language Selector

            MiscMenu = Menu.AddSubMenu("Misc - " + GCharname, GCharname + "Misc");
            var sliderValue = MiscMenu.Add("language", new Slider("Language", 0, 0, 4));
            sliderValue.OnValueChange += delegate
            {
                sliderValue.DisplayName = "Language: " + System.Enum.GetName(typeof(EnumLanguage), Misc.GetSliderValue(MiscMenu, "language"));
            };
            MiscMenu.AddLabel("After select your language press F5");

            LanguageController language;

            switch ((EnumLanguage)Misc.GetSliderValue(MiscMenu, "language"))
            {
                case EnumLanguage.English:
                    language = new English();
                    break;
                case EnumLanguage.Portugues:
                    language = new Portugues();
                    break;
                default:
                    language = new English();
                    break;
            }

            #endregion

            DrawMenu = Menu.AddSubMenu(language.Dictionary[EnumContext.Draw] + " - " + GCharname, GCharname + "Draw");
            DrawMenu.AddGroupLabel(language.Dictionary[EnumContext.Draw]);
            DrawMenu.Add("drawDisable", new CheckBox(language.Dictionary[EnumContext.TurnOffDraws], true));
            DrawMenu.Add("drawDmg", new CheckBox(language.Dictionary[EnumContext.Draw] + language.Dictionary[EnumContext.Range] + " Dmg", true));
            DrawMenu.Add("drawQ", new CheckBox(language.Dictionary[EnumContext.Draw] + language.Dictionary[EnumContext.Range] + " Q", true));
            DrawMenu.Add("drawW", new CheckBox(language.Dictionary[EnumContext.Draw] + language.Dictionary[EnumContext.Range] + " W", true));
            DrawMenu.Add("drawE", new CheckBox(language.Dictionary[EnumContext.Draw] + language.Dictionary[EnumContext.Range] + " E", true));
            DrawMenu.Add("drawR", new CheckBox(language.Dictionary[EnumContext.Draw] + language.Dictionary[EnumContext.Range] + " R", true));

            ComboMenu = Menu.AddSubMenu(language.Dictionary[EnumContext.Combo] + " - " + GCharname, GCharname + "Combo");
            ComboMenu.AddGroupLabel(language.Dictionary[EnumContext.Combo]);
            ComboMenu.Add("comboQ", new CheckBox(language.Dictionary[EnumContext.Use] + " Q", true));
            ComboMenu.Add("comboW", new CheckBox(language.Dictionary[EnumContext.Use] + " W", true));
            ComboMenu.Add("comboE", new CheckBox(language.Dictionary[EnumContext.Use] + " E", true));
            ComboMenu.Add("comboR", new CheckBox(language.Dictionary[EnumContext.Use] + " R", true));
            ComboMenu.AddLabel("Checked = All Star | Unchecked = Line Combo");
            ComboMenu.Add("comboStyle", new CheckBox(language.Dictionary[EnumContext.ComboStyle], true));

            PermaActiveMenu = Menu.AddSubMenu("Perma Active" + " - " + GCharname, GCharname + "PermaActive");
            PermaActiveMenu.AddGroupLabel("Perma Active");
            PermaActiveMenu.Add("paQ", new CheckBox(language.Dictionary[EnumContext.Use] + " Q to KS", true));
            PermaActiveMenu.Add("paW", new CheckBox(language.Dictionary[EnumContext.Use] + " W to KS", true));
            PermaActiveMenu.Add("paE", new CheckBox(language.Dictionary[EnumContext.Use] + " E to KS", true));

            HarassMenu = Menu.AddSubMenu(language.Dictionary[EnumContext.Harass] + " - " + GCharname, GCharname + "Harass");
            HarassMenu.AddGroupLabel(language.Dictionary[EnumContext.Harass]);
            HarassMenu.AddGroupLabel("Q " + language.Dictionary[EnumContext.Settings]);
            HarassMenu.Add("hsQ", new CheckBox(language.Dictionary[EnumContext.Use] + " Q", true));
            HarassMenu.AddGroupLabel("W " + language.Dictionary[EnumContext.Settings]);
            HarassMenu.Add("hsW", new CheckBox(language.Dictionary[EnumContext.Use] + " W", true));
            HarassMenu.AddGroupLabel("E " + language.Dictionary[EnumContext.Settings]);
            HarassMenu.Add("hsE", new CheckBox(language.Dictionary[EnumContext.Use] + " E", true));

            LastHitMenu = Menu.AddSubMenu(language.Dictionary[EnumContext.LastHit] + " - " + GCharname, GCharname + "LastHit");
            LastHitMenu.AddGroupLabel(language.Dictionary[EnumContext.LastHit]);
            LastHitMenu.AddGroupLabel("Q " + language.Dictionary[EnumContext.Settings]);
            LastHitMenu.Add("lhQ", new CheckBox(language.Dictionary[EnumContext.Use] + " Q", true));
            LastHitMenu.AddGroupLabel("E " + language.Dictionary[EnumContext.Settings]);
            LastHitMenu.Add("lhE", new CheckBox(language.Dictionary[EnumContext.Use] + " E", true));

            LaneClearMenu = Menu.AddSubMenu(language.Dictionary[EnumContext.LaneClear] + " - " + GCharname, GCharname + "LaneClear");
            LaneClearMenu.AddGroupLabel(language.Dictionary[EnumContext.LaneClear]);
            LaneClearMenu.Add("lcQ", new CheckBox(language.Dictionary[EnumContext.Use] + " Q", true));
            LaneClearMenu.Add("lcW", new CheckBox(language.Dictionary[EnumContext.Use] + " W", true));
            LaneClearMenu.Add("lcE", new CheckBox(language.Dictionary[EnumContext.Use] + " E", true));

            JungleClearMenu = Menu.AddSubMenu(language.Dictionary[EnumContext.JungleClear] + " - " + GCharname, GCharname + "JungleClear");
            JungleClearMenu.AddGroupLabel(language.Dictionary[EnumContext.JungleClear]);
            JungleClearMenu.Add("jcQ", new CheckBox(language.Dictionary[EnumContext.Use] + " Q", true));
            JungleClearMenu.Add("jcW", new CheckBox(language.Dictionary[EnumContext.Use] + " W", true));
            JungleClearMenu.Add("jcE", new CheckBox(language.Dictionary[EnumContext.Use] + " E", true));
        }

        #region Events Region

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if(Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.None) return;

            if(!ItemManager.CanUseHydra()) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ItemManager.UseHydra(target);
                Core.DelayAction(Orbwalker.ResetAutoAttack, 250);
            }

        }

        public override void OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.HasBuff("zedwhandler"))
            {
                LastWCast = Game.Time;
            }
        }

        public override void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            
        }

        public override void OnDraw(EventArgs args)
        {
                
            if (Misc.IsChecked(DrawMenu, "drawDisable"))
                return;

            if (Misc.IsChecked(DrawMenu, "drawQ"))
                Circle.Draw(Q.IsReady() ? SharpDX.Color.Blue : SharpDX.Color.Red, Q.Range, Player.Instance.Position);

            if (Misc.IsChecked(DrawMenu, "drawW"))
                Circle.Draw(W.IsReady() ? SharpDX.Color.Blue : SharpDX.Color.Red, W.Range, Player.Instance.Position);

            if (Misc.IsChecked(DrawMenu, "drawE"))
                Circle.Draw(E.IsReady() ? SharpDX.Color.Blue : SharpDX.Color.Red, E.Range, Player.Instance.Position);

            if (Misc.IsChecked(DrawMenu, "drawR"))
                Circle.Draw(R.IsReady() ? SharpDX.Color.Blue : SharpDX.Color.Red, R.Range, Player.Instance.Position);
        }

        #endregion
    }
}

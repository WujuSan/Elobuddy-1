using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using OneForWeek.Util.Misc;
using SharpDX;
using System.Collections.Generic;

namespace OneForWeek.Plugin.Hero
{
    class Vayne : PluginModel, IChampion
    {
        public static Spell.Active Q;
        public static Spell.Targeted E;
        public static Spell.Active R;
        private static List<Spell.SpellBase> spells;

        public Vayne()
        {
            Init();
        }

        public void Init()
        {
            spells = new List<Spell.SpellBase>();
            InitVariables();
        }

        public void InitVariables()
        {
            Q = new Spell.Active(SpellSlot.Q, 325);
            spells.Add(Q);
            E = new Spell.Targeted(SpellSlot.E, 550);
            spells.Add(E);
            R = new Spell.Active(SpellSlot.R);
            spells.Add(R);

            InitMenu();

            Orbwalker.OnPostAttack += OnAfterAttack;
            Gapcloser.OnGapcloser += OnGapCloser;
            Interrupter.OnInterruptableSpell += OnPossibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;

            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
        }

        public void InitMenu()
        {
            Menu = MainMenu.AddMenu(GCharname, GCharname);

            Menu.AddLabel("Version: " + GVersion);
            Menu.AddSeparator();
            Menu.AddLabel("By MrArticuno");

            DrawMenu = Menu.AddSubMenu("Draw - " + GCharname, GCharname + "Draw");
            DrawMenu.AddGroupLabel("Draw");
            DrawMenu.Add("drawDisable", new CheckBox("Turn off all drawings", false));
            DrawMenu.Add("drawQ", new CheckBox("Draw Q Range", true));
            DrawMenu.Add("drawE", new CheckBox("Draw E Range", true));
            DrawMenu.Add("drawCondemn", new CheckBox("Draw E Condemn Line", true));

            ComboMenu = Menu.AddSubMenu("Combo - " + GCharname, GCharname + "Combo");
            ComboMenu.AddGroupLabel("Combo");
            ComboMenu.Add("comboQ", new CheckBox("Use Q", true));
            ComboMenu.Add("comboE", new CheckBox("Use E", true));
            ComboMenu.Add("comboR", new CheckBox("Use R", true));
            ComboMenu.Add("minEnemiesInRange", new Slider("Min enemies in range for R: ", 2, 1, 5));

            HarassMenu = Menu.AddSubMenu("Harass - " + GCharname, GCharname + "Harass");
            HarassMenu.AddGroupLabel("Harass");
            HarassMenu.Add("hsQ", new CheckBox("Use Q", true));
            HarassMenu.Add("hsE", new CheckBox("Use E", true));
            HarassMenu.Add("minManaPercent", new Slider("Min Mana Percent to use Skills: ", 50, 0, 100));

            LaneClearMenu = Menu.AddSubMenu("Lane Clear - " + GCharname, GCharname + "LaneClear");
            LaneClearMenu.AddGroupLabel("Lane Clear");
            LaneClearMenu.Add("lcQ", new CheckBox("Use Q", true));
            LaneClearMenu.Add("minManaPercent", new Slider("Min Mana Percent to use Skills: ", 50, 0, 100));

            LastHitMenu = Menu.AddSubMenu("Last Hit - " + GCharname, GCharname + "LastHit");
            LastHitMenu.AddGroupLabel("Last Hit");
            LastHitMenu.Add("lhQ", new CheckBox("Use Q", true));
            LastHitMenu.Add("minManaPercent", new Slider("Min Mana Percent to use Skills: ", 50, 0, 100));

            MiscMenu = Menu.AddSubMenu("Misc - " + GCharname, GCharname + "Misc");
            MiscMenu.Add("miscAntiGapQ", new CheckBox("Anti Gap Closer Q", true));
            MiscMenu.Add("miscAntiGapE", new CheckBox("Anti Gap Closer E", true));
            MiscMenu.Add("miscInterruptDangerous", new CheckBox("Try Interrupt Dangerous Spells", true));

            MiscMenu.AddGroupLabel("Condemn Options");
            MiscMenu.Add("interruptDangerousSpells", new CheckBox("Interrupt Dangerous Spells", true));
            MiscMenu.Add("fastCondemn",
                new KeyBind("Fast Condemn HotKey", false, KeyBind.BindTypes.PressToggle, 'W'));
            MiscMenu.AddGroupLabel("Auto Condemn");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy))
            {
                MiscMenu.Add("dnCondemn" + enemy.ChampionName.ToLower(), new CheckBox("Don't Condemn " + enemy.ChampionName, false));
            }
            MiscMenu.AddGroupLabel("Priority Condemn");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy))
            {
                MiscMenu.Add("priorityCondemn" + enemy.ChampionName.ToLower(), new Slider(enemy.ChampionName + " Priority", 1, 1, 5));
            }
            MiscMenu.Add("condenmErrorMargin", new Slider("Subtract Condemn Push by: ", 20, 0, 100));

        }

        public void OnCombo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget(E.Range)) return;

        }

        public void OnLastHit()
        {

        }

        public void OnHarass()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget(E.Range) || Player.Instance.ManaPercent < Misc.GetSliderValue(HarassMenu, "minManaPercent")) return;

            
        }

        public void OnLaneClear()
        {
            if(Player.Instance.ManaPercent < Misc.GetSliderValue(LaneClearMenu, "minManaPercent")) return;

            if (Misc.IsChecked(LaneClearMenu, "lcQ") && Q.IsReady())
            {
                
            }
        }

        public void OnFlee()
        {

        }

        public void OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
                return;
            }
                
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
                return;
            }
                
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                OnFlee();
                return;
            }
                
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                OnLaneClear();
                return;
            }
                
        }

        public void OnDraw(EventArgs args)
        {
            if (Misc.IsChecked(DrawMenu, "drawDisable"))
                return;

            if (Misc.IsChecked(DrawMenu, "drawQ"))
                Circle.Draw(Q.IsReady() ? Color.Blue : Color.Red, Q.Range, Player.Instance.Position);
            
            if (Misc.IsChecked(DrawMenu, "drawE"))
                Circle.Draw(E.IsReady() ? Color.Blue : Color.Red, E.Range, Player.Instance.Position);

            if (Misc.IsChecked(DrawMenu, "drawR"))
                Circle.Draw(R.IsReady() ? Color.Blue : Color.Red, R.Range, Player.Instance.Position);

        }

        public void OnAfterAttack(AttackableUnit target, EventArgs args){}

        public void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy) return;
            
        }

        public void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            
        }

        public void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(!sender.IsMe) return;

        }

        public void GameObjectOnCreate(GameObject sender, EventArgs args)
        {

        }

        public void GameObjectOnDelete(GameObject sender, EventArgs args)
        {

        }

        public static double PossibleDamage(this AIHeroClient target)
        {
            var damage = 0d;
            var targetMaxHealth = target.MaxHealth;

            var silverBoltDmg = (new float[] { 0, 20, 30, 40, 50, 60 }[Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level] + new float[] { 0, targetMaxHealth / 4, targetMaxHealth / 5, targetMaxHealth / 6, targetMaxHealth / 7, targetMaxHealth / 8 }[Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level]);

            if (Orbwalker.CanAutoAttack) damage += _Player.GetAutoAttackDamage(target, true);

            if (target.GetBuffCount("vaynesilvereddebuff") == 2) damage += silverBoltDmg;

            return damage;
        }

        private static bool CanCastSpell(SpellSlot spell, Obj_AI_Base target)
        {
            return spells.Any(aux => aux.Slot == spell && aux.IsReady() && aux.IsInRange(target));
        }

        private static float PossibleDamage(Obj_AI_Base target, SpellSlot spell)
        {
            return _Player.GetSpellDamage(target, spell);
        }
    }
}

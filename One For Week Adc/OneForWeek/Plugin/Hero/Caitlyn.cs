using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using OneForWeek.Util.Misc;
using SharpDX;
using System.Collections.Generic;

namespace OneForWeek.Plugin.Hero
{
    class Caitlyn : PluginModel, IChampion
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Targeted R;
        private static List<Spell.SpellBase> spells;

        public Caitlyn()
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
            Q = new Spell.Skillshot(SpellSlot.Q, 1240, SkillShotType.Linear, 250, 2000, 60);
            spells.Add(Q);
            W = new Spell.Skillshot(SpellSlot.W, 820, SkillShotType.Circular, 500, int.MaxValue, 80);
            spells.Add(W);
            E = new Spell.Skillshot(SpellSlot.E, 800, SkillShotType.Linear, 250, 1600, 80);
            spells.Add(E);
            R = new Spell.Targeted(SpellSlot.R, 2000);
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
            DrawMenu.Add("drawW", new CheckBox("Draw W Range", true));
            DrawMenu.Add("drawE", new CheckBox("Draw E Range", true));
            DrawMenu.Add("drawR", new CheckBox("Draw R Range", true));

            ComboMenu = Menu.AddSubMenu("Combo - " + GCharname, GCharname + "Combo");
            ComboMenu.AddGroupLabel("Combo");
            ComboMenu.Add("comboQ", new CheckBox("Use Q", true));
            ComboMenu.Add("comboW", new CheckBox("Use W", true));
            ComboMenu.Add("comboE", new CheckBox("Use E", true));
            ComboMenu.Add("comboR", new CheckBox("Use R", true));

            HarassMenu = Menu.AddSubMenu("Harass - " + GCharname, GCharname + "Harass");
            HarassMenu.AddGroupLabel("Harass");
            HarassMenu.Add("hsQ", new CheckBox("Use Q", true));
            HarassMenu.Add("hsW", new CheckBox("Use W", true));
            HarassMenu.Add("hsE", new CheckBox("Use E", true));
            HarassMenu.Add("minManaPercent", new Slider("Min Mana Percent to use Skills: ", 50, 0, 100));

            LaneClearMenu = Menu.AddSubMenu("Lane Clear - " + GCharname, GCharname + "LaneClear");
            LaneClearMenu.AddGroupLabel("Lane Clear");
            LaneClearMenu.Add("lcQ", new CheckBox("Use Q", true));
            LaneClearMenu.Add("minManaPercent", new Slider("Min Mana Percent to use Skills: ", 50, 0, 100));

            MiscMenu = Menu.AddSubMenu("Misc - " + GCharname, GCharname + "Misc");
            MiscMenu.Add("useNetForMousePos", new KeyBind("Use net for mouse Pos", false, KeyBind.BindTypes.HoldActive, 'T'));
            MiscMenu.Add("minRangeForE", new Slider("Distance for cast E: ", 300, 0, 800));
            MiscMenu.Add("ksOn", new CheckBox("Try to KS", true));
            MiscMenu.Add("miscAntiGapQ", new CheckBox("Anti Gap Closer Q", false));
            MiscMenu.Add("miscAntiGapW", new CheckBox("Anti Gap Closer W", true));
            MiscMenu.Add("miscAntiGapE", new CheckBox("Anti Gap Closer E", true));
            MiscMenu.Add("miscInterruptDangerous", new CheckBox("Try Interrupt Dangerous Spells", true));

        }

        public void OnCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget(Q.Range)) return;

            if(target.Distance(Player.Instance) < Player.Instance.GetAutoAttackRange() / 2 && 
                (E.IsReady() && Misc.IsChecked(ComboMenu, "comboE")) && 
                (Q.IsReady() && Misc.IsChecked(ComboMenu, "comboQ")))
            {
                var predictionE = E.GetPrediction(target);

                if(predictionE.HitChance < HitChance.Medium) return;

                E.Cast(predictionE.CastPosition);
                Core.DelayAction(() => Q.Cast(Q.GetPrediction(target).CastPosition), 250);
            }

            if(W.IsReady() && Misc.IsChecked(ComboMenu, "comboW"))
            {
                var predictionPos = Prediction.Position.PredictUnitPosition(target, 500).To3D();

                if (W.IsInRange(predictionPos))
                {
                    W.Cast(predictionPos);
                }
            }

            if (Misc.IsChecked(ComboMenu, "comboE") && E.IsReady() && !Q.IsReady() && target.Distance(Player.Instance) < Misc.GetSliderValue(MiscMenu, "minRangeForE"))
            {
                var predictionE = E.GetPrediction(target);

                if (predictionE.HitChance < HitChance.Medium) return;

                E.Cast(E.GetPrediction(target).CastPosition);
            }

            if (Misc.IsChecked(ComboMenu, "comboQ") && Q.IsReady())
            {
                Q.Cast(Q.GetPrediction(target).CastPosition);
            }
            
        }

        public void OnHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget(Q.Range) || Player.Instance.ManaPercent < Misc.GetSliderValue(HarassMenu, "minManaPercent")) return;

            if (target.Distance(Player.Instance) < Misc.GetSliderValue(MiscMenu, "minRangeForE") &&
                (E.IsReady() && Misc.IsChecked(HarassMenu, "hsE")) &&
                (Q.IsReady() && Misc.IsChecked(HarassMenu, "hsQ")))
            {
                E.Cast(E.GetPrediction(target).CastPosition);
                Core.DelayAction(() => Q.Cast(Q.GetPrediction(target).CastPosition), 250);
            }

            if (W.IsReady() && Misc.IsChecked(HarassMenu, "hsW"))
            {
                var predictionPos = Prediction.Position.PredictUnitPosition(target, 500).To3D();

                if (W.IsInRange(predictionPos))
                {
                    W.Cast(predictionPos);
                }
            }

            if (Misc.IsChecked(HarassMenu, "hsE") && E.IsReady() && !Q.IsReady() && target.Distance(Player.Instance) < Player.Instance.GetAutoAttackRange() / 2)
            {
                E.Cast(E.GetPrediction(target).CastPosition);
            }

            if (Misc.IsChecked(HarassMenu, "hsQ") && Q.IsReady() && !E.IsReady())
            {
                Q.Cast(Q.GetPrediction(target).CastPosition);
            }
        }

        public void OnLaneClear()
        {
            if(Player.Instance.ManaPercent < Misc.GetSliderValue(LaneClearMenu, "minManaPercent")) return;

            if (Misc.IsChecked(LaneClearMenu, "lcQ") && Q.IsReady())
            {
                var bestpos = Misc.GetBestLineFarmLocation(EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => Q.IsInRange(m) && !m.IsDead).Select(o => o.ServerPosition.To2D()).ToList(), Q.Width, Q.Range);
                if(bestpos.MinionsHit >= 3)
                {
                    Q.Cast(bestpos.Position.To3D());
                }
            }
        }

        public void OnFlee(){}

        public void OnGameUpdate(EventArgs args)
        {
            if (Misc.IsKeyBindOn(MiscMenu, "useNetForMousePos") && E.IsReady())
            {
                Player.CastSpell(SpellSlot.E, Game.CursorPos.Extend(Player.Instance.Position, Player.Instance.Distance(Game.CursorPos) + 250).To3D());
            }

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

            if (Misc.IsChecked(MiscMenu, "ksOn"))
            {
                KS();
            }
                
        }

        public void OnDraw(EventArgs args)
        {
            if (Misc.IsChecked(DrawMenu, "drawDisable"))
                return;

            if (Misc.IsChecked(DrawMenu, "drawQ"))
                Circle.Draw(Q.IsReady() ? Color.Blue : Color.Red, Q.Range, Player.Instance.Position);

            if (Misc.IsChecked(DrawMenu, "drawW"))
                Circle.Draw(W.IsReady() ? Color.Blue : Color.Red, W.Range, Player.Instance.Position);

            if (Misc.IsChecked(DrawMenu, "drawE"))
                Circle.Draw(E.IsReady() ? Color.Blue : Color.Red, E.Range, Player.Instance.Position);

            if (Misc.IsChecked(DrawMenu, "drawR"))
                Circle.Draw(R.IsReady() ? Color.Blue : Color.Red, R.Range, Player.Instance.Position);

        }

        public void OnAfterAttack(AttackableUnit target, EventArgs args){}

        public void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy) return;

            if (interruptableSpellEventArgs.DangerLevel >= DangerLevel.High && Misc.IsChecked(MiscMenu, "miscInterruptDangerous") && W.IsReady())
            {
                W.Cast(sender.Position);
            }
            
        }

        public void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if(!sender.IsEnemy || e.End.Distance(Player.Instance) <= 50) return;

            if (W.IsReady() && W.IsInRange(sender) && Misc.IsChecked(MiscMenu, "miscAntiGapW"))
            {
                W.Cast(Prediction.Position.PredictUnitPosition(sender, 250).To3D());
            }

            if (E.IsReady() && E.IsInRange(sender) && Misc.IsChecked(MiscMenu, "miscAntiGapE"))
            {
                E.Cast(E.GetPrediction(sender).CastPosition);
                if(Q.IsReady() && Misc.IsChecked(MiscMenu, "miscAntiGapQ"))
                {
                    Core.DelayAction(() => Q.Cast(Q.GetPrediction(sender).CastPosition) ,250);
                }
            }
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

        private static void KS()
        {
            var targets = EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(R.Range));

            foreach(var target in targets)
            {
                if(target.Health < PossibleDamage(target, SpellSlot.R) &&
                    !Player.Instance.IsInAutoAttackRange(target) && 
                    CanCastSpell(SpellSlot.R, target) && 
                    Misc.IsChecked(ComboMenu, "comboR"))
                {
                    R.Cast(target);
                }

                if (target.Health < PossibleDamage(target, SpellSlot.Q) && !Player.Instance.IsInAutoAttackRange(target) && CanCastSpell(SpellSlot.Q, target))
                {
                    Q.Cast(Q.GetPrediction(target).CastPosition);
                }

                if (target.Health < PossibleDamage(target, SpellSlot.E) && CanCastSpell(SpellSlot.E, target))
                {
                    E.Cast(E.GetPrediction(target).CastPosition);
                }
            }
        }

        private static float PossibleDamage(Obj_AI_Base target)
        {
            var damage = 0f;
            if (R.IsReady())
                damage = _Player.GetSpellDamage(target, SpellSlot.R);
            if (E.IsReady())
                damage = _Player.GetSpellDamage(target, SpellSlot.E);
            if (W.IsReady())
                damage = _Player.GetSpellDamage(target, SpellSlot.W);
            if (Q.IsReady())
                damage = _Player.GetSpellDamage(target, SpellSlot.Q);

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

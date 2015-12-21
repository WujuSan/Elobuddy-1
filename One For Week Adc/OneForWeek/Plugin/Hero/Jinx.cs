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
    class Jinx : PluginModel, IChampion
    {
        public static Spell.Active Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        private static List<Spell.SpellBase> spells;
        private static int normalAttackRange = 525;

        public Jinx()
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
            Q = new Spell.Active(SpellSlot.Q, 700);
            spells.Add(Q);
            W = new Spell.Skillshot(SpellSlot.W, 1450, SkillShotType.Linear);
            spells.Add(W);
            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Linear);
            E.AllowedCollisionCount = int.MaxValue;
            spells.Add(E);
            R = new Spell.Skillshot(SpellSlot.R, 25000, SkillShotType.Linear, 500, 1700, 250);
            R.AllowedCollisionCount = int.MaxValue;
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
            MiscMenu.AddGroupLabel("Q Options");
            MiscMenu.Add("qOptionDistance", new CheckBox("Cast Q for distance", true));
            MiscMenu.Add("qOptionAOE", new CheckBox("Cast Q for AOE", true));
            MiscMenu.Add("qOptionAOEMin", new Slider("Min enemies for AOE: ", 3, 1, 5));
            MiscMenu.AddGroupLabel("E Options");
            MiscMenu.Add("autoEImmobile", new Slider("Auto E on immobile: ", 300, 0, 800));
            MiscMenu.Add("miscAntiGapE", new CheckBox("Anti Gap Closer E", true));
            MiscMenu.AddGroupLabel("R Options");
            MiscMenu.Add("minRangeForR", new Slider("Distance for cast R: ", 750, 600, 5000));
            MiscMenu.AddGroupLabel("ETC");            
            MiscMenu.Add("ksOn", new CheckBox("Try to KS", true));

        }

        public void OnCombo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (target != null || target.IsValidTarget(W.Range))
            {
                if(Misc.IsChecked(ComboMenu, "comboQ") && canCastQ(target) && Q.IsReady())
                {
                    Q.Cast();
                }
                if (Misc.IsChecked(ComboMenu, "comboW") && W.IsReady() && W.IsInRange(target))
                {
                    var predictionW = W.GetPrediction(target);

                    if(predictionW.HitChance >= HitChance.Medium)
                    {
                        W.Cast(predictionW.CastPosition);
                    }
                }

                if (Misc.IsChecked(ComboMenu, "comboE") && E.IsReady() && E.IsInRange(target))
                {
                    var predictionE = E.GetPrediction(target);

                    if (predictionE.HitChance >= HitChance.High || predictionE.HitChance == HitChance.Immobile)
                    {
                        E.Cast(predictionE.CastPosition);
                    }
                }
            }

            if (!Misc.IsChecked(ComboMenu, "rCombo") || !R.IsReady()) return;

            target = TargetSelector.GetTarget(Misc.GetSliderValue(MiscMenu, "minRangeForR"), DamageType.Physical);

            if (target != null || target.IsValidTarget(R.Range))
            {
                var predictionR = R.GetPrediction(target);

                if (predictionR.HitChance >= HitChance.Medium)
                {
                    R.Cast(predictionR.CastPosition);
                }
            }
        }

        public void OnHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget(Q.Range) || Player.Instance.ManaPercent < Misc.GetSliderValue(HarassMenu, "minManaPercent")) return;

            if (Misc.IsChecked(HarassMenu, "hsQ") && canCastQ(target) && Q.IsReady())
            {
                Q.Cast();
            }
            if (Misc.IsChecked(HarassMenu, "hsW") && W.IsReady() && W.IsInRange(target))
            {
                var predictionW = W.GetPrediction(target);

                if (predictionW.HitChance >= HitChance.Medium)
                {
                    W.Cast(predictionW.CastPosition);
                }
            }

            if (Misc.IsChecked(HarassMenu, "hsE") && E.IsReady() && E.IsInRange(target))
            {
                var predictionE = E.GetPrediction(target);

                if (predictionE.HitChance >= HitChance.High || predictionE.HitChance == HitChance.Immobile)
                {
                    E.Cast(predictionE.CastPosition);
                }
            }
        }

        public void OnLaneClear()
        {
            var target = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(x => x.IsValidTarget(Q.Range) && !x.IsDead && x.Health > 40 && Q.IsInRange(x));

            if (Player.Instance.ManaPercent < Misc.GetSliderValue(LaneClearMenu, "minManaPercent") || target == null || !target.IsValidTarget())
            {
                if (isFishBones())
                {
                    Q.Cast();
                }

                return;
            }


            if (Misc.IsChecked(HarassMenu, "lcQ") && canCastQ(target) && Q.IsReady())
            {
                Q.Cast();
            }
        }

        public void OnFlee(){}

        public void OnGameUpdate(EventArgs args)
        {
            if (Misc.IsChecked(MiscMenu, "ksOn"))
            {
                KS();
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
                Circle.Draw(R.IsReady() ? Color.Blue : Color.Red, Misc.GetSliderValue(MiscMenu, "minRangeForR"), Player.Instance.Position);

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
            
            if (E.IsReady() && E.IsInRange(sender) && Misc.IsChecked(MiscMenu, "miscAntiGapE"))
            {
                
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
                        
        }

        private static bool isFishBones()
        {
            return Player.Instance.GetAutoAttackRange() > normalAttackRange;
        }

        private static bool canCastQ(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget() || target.IsDead) return isFishBones();

            if (isFishBones())
            {
                if (Misc.IsChecked(MiscMenu, "qOptionDistance"))
                {
                    if (target.Distance(Player.Instance.Position) > normalAttackRange)
                    {
                        return false;
                    }
                }
                if (Misc.IsChecked(MiscMenu, "qOptionAOE"))
                {
                    if (target.CountEnemiesInRange(300f) >= Misc.GetSliderValue(MiscMenu, "qOptionAOEMin"))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (Misc.IsChecked(MiscMenu, "qOptionDistance"))
                {
                    if (target.Distance(Player.Instance.Position) > normalAttackRange)
                    {
                        return true;
                    }
                }
                if (Misc.IsChecked(MiscMenu, "qOptionAOE"))
                {
                    if (target.CountEnemiesInRange(300f) >= Misc.GetSliderValue(MiscMenu, "qOptionAOEMin"))
                    {
                        return true;
                    }
                }
            }

            return isFishBones();
        }

        private static float PossibleDamage(Obj_AI_Base target)
        {
            var damage = 0f;
            if (R.IsReady())
                damage += _Player.GetSpellDamage(target, SpellSlot.R);

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

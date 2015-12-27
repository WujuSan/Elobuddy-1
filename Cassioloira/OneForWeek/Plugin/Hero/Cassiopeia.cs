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

namespace OneForWeek.Plugin.Hero
{
    class Cassiopeia : PluginModel, IChampion
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;

        private float _lastECast = 0f;

        public void Init()
        {
            InitVariables();
        }

        public void InitVariables()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Circular, castDelay: 400, spellWidth: 75);
            W = new Spell.Skillshot(SpellSlot.W, 850, SkillShotType.Circular, spellWidth: 125);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Skillshot(SpellSlot.R, 825, SkillShotType.Cone, spellWidth: 80);
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
            Menu.AddLabel("By Vector");

            DrawMenu = Menu.AddSubMenu("Draw - " + GCharname, GCharname + "Draw");
            DrawMenu.AddGroupLabel("Draw");
            DrawMenu.Add("drawDisable", new CheckBox("Turn off all drawings", true));
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
            ComboMenu.AddGroupLabel("Combo Misc");
            ComboMenu.Add("disableAA", new CheckBox("Disable AA while combo", false));
            ComboMenu.AddLabel("This option overrides min enemies for R");
            ComboMenu.Add("flashCombo", new CheckBox("Flash R Combo if Killable", false));
            ComboMenu.Add("rsMinEnemiesForR", new Slider("Min Enemies Facing for cast R: ", 2, 1, 5));

            HarassMenu = Menu.AddSubMenu("Harass - " + GCharname, GCharname + "Harass");
            HarassMenu.AddGroupLabel("Harass");
            HarassMenu.Add("hsQ", new CheckBox("Use Q", true));
            HarassMenu.Add("hsW", new CheckBox("Use W", true));
            HarassMenu.Add("hsE", new CheckBox("Use E", true));
            HarassMenu.AddGroupLabel("Harass Misc");
            HarassMenu.Add("disableAAHS", new CheckBox("Disable AA while harass", false));
            HarassMenu.Add("hsPE", new CheckBox("Only E if poisoned", true));

            LaneClearMenu = Menu.AddSubMenu("Lane Clear - " + GCharname, GCharname + "LaneClear");
            LaneClearMenu.AddGroupLabel("Lane Clear");
            LaneClearMenu.Add("lcQ", new CheckBox("Use Q", true));
            LaneClearMenu.Add("lcW", new CheckBox("Use W", true));
            LaneClearMenu.Add("lcE", new CheckBox("Use E", true));
            LaneClearMenu.Add("lcKE", new CheckBox("Only E if killable", false));
            LaneClearMenu.Add("lcPE", new CheckBox("Only E if poisoned", true));

            MiscMenu = Menu.AddSubMenu("Misc - " + GCharname, GCharname + "Misc");
            MiscMenu.Add("poisonForE", new CheckBox("Only Cast E in Poisoned targets", true));
            MiscMenu.Add("miscDelayE", new Slider("Delay E Cast by: ", 150, 0, 500));
            MiscMenu.Add("ksOn", new CheckBox("Try to KS", true));
            MiscMenu.Add("miscAntiGapW", new CheckBox("Anti Gap Closer W", true));
            MiscMenu.Add("miscAntiGapR", new CheckBox("Anti Gap Closer R", true));
            MiscMenu.Add("miscAntiMissR", new CheckBox("Block R if Miss", true));
            MiscMenu.Add("miscMinHpAntiGap", new Slider("Min HP to Anti Gap Closer R: ", 40, 0, 100));
            MiscMenu.Add("miscInterruptDangerous", new CheckBox("Interrupt Dangerous Spells", true));

        }

        public void OnCombo()
        {
            var target = TargetSelector.GetTarget(R.Range + 400, DamageType.Magical);

            if (target == null || !target.IsValidTarget(Q.Range)) return;

            var flash = Player.Spells.FirstOrDefault(a => a.SData.Name == "summonerflash");

            if (Misc.IsChecked(ComboMenu, "comboQ") && Q.IsReady() && target.IsValidTarget(Q.Range) && !IsPoisoned(target))
            {
                var predictionQ = Q.GetPrediction(target);

                if (predictionQ.HitChancePercent >= 75)
                {
                    Q.Cast(predictionQ.CastPosition);
                }
            }
            else if (Misc.IsChecked(ComboMenu, "comboQ") && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                     IsPoisoned(target))
            {
                var predictionQ = Q.GetPrediction(target);

                if (predictionQ.HitChancePercent >= 75)
                {
                    Q.Cast(predictionQ.CastPosition);
                }
            }

            if (Misc.IsChecked(ComboMenu, "comboW") && W.IsReady() && target.IsValidTarget(W.Range) && (!IsPoisoned(target) || target.Distance(_Player) < W.Range - 150 || !Q.IsReady()))
            {
                var predictionW = W.GetPrediction(target);

                if (predictionW.HitChancePercent >= 70)
                {
                    W.Cast(predictionW.CastPosition);
                }
            }

            if (Misc.IsChecked(ComboMenu, "comboE") && E.IsReady() && target.IsValidTarget(E.Range) && (IsPoisoned(target) || !Misc.IsChecked(MiscMenu, "poisonForE")))
            {
                E.Cast(target);
            }

            if (Misc.IsChecked(ComboMenu, "comboR") && R.IsReady())
            {
                if (Misc.IsChecked(ComboMenu, "flashCombo") && PossibleDamage(target) > target.Health && target.IsFacing(_Player) && target.Distance(_Player) > R.Range && (flash != null && flash.IsReady))
                {
                    Player.CastSpell(flash.Slot, target.Position);
                    Core.DelayAction(() => R.Cast(target), 250);
                }

                var countFacing = EntityManager.Heroes.Enemies.Count(t => t.IsValidTarget(R.Range) && t.IsFacing(_Player) && ProbablyFacing(t));

                if (Misc.GetSliderValue(ComboMenu, "rsMinEnemiesForR") <= countFacing && target.IsFacing(_Player) && target.IsValidTarget(R.Range - 50))
                {
                    R.Cast(target);
                }
            }
        }

        private bool ProbablyFacing(Obj_AI_Base target)
        {
            var predictPos = Prediction.Position.PredictUnitPosition(target, 250);

            return predictPos.Distance(Player.Instance.ServerPosition)  < target.ServerPosition.Distance(Player.Instance.ServerPosition);
        }

        public void OnHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget(Q.Range)) return;

            if (Misc.IsChecked(HarassMenu, "hsQ") && Q.IsReady() && target.IsValidTarget(Q.Range) && !IsPoisoned(target))
            {
                var predictionQ = Q.GetPrediction(target);

                if (predictionQ.HitChancePercent >= 70)
                {
                    Q.Cast(predictionQ.CastPosition);
                }
            }
            else if (Misc.IsChecked(HarassMenu, "hsQ") && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                     IsPoisoned(target))
            {
                var predictionQ = Q.GetPrediction(target);

                if (predictionQ.HitChancePercent >= 70)
                {
                    Q.Cast(predictionQ.CastPosition);
                }
            }

            if (Misc.IsChecked(HarassMenu, "hsW") && W.IsReady() && target.IsValidTarget(W.Range) && (!IsPoisoned(target) || target.Distance(_Player) < W.Range - 150 || !Q.IsReady()))
            {
                var predictionW = W.GetPrediction(target);

                if (predictionW.HitChancePercent >= 70)
                {
                    W.Cast(predictionW.CastPosition);
                }
            }

            if (Misc.IsChecked(HarassMenu, "hsE") && E.IsReady() && target.IsValidTarget(E.Range) && (IsPoisoned(target) || !Misc.IsChecked(MiscMenu, "hsPE")))
            {
                E.Cast(target);
            }
        }

        public void OnLaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions;

            if (minions == null || !minions.Any()) return;

            var bestFarmQ =
                Misc.GetBestCircularFarmLocation(
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.Distance(_Player) <= Q.Range)
                        .Select(xm => xm.ServerPosition.To2D())
                        .ToList(), Q.Width, Q.Range);
            var bestFarmW =
                Misc.GetBestCircularFarmLocation(
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.Distance(_Player) <= W.Range)
                        .Select(xm => xm.ServerPosition.To2D())
                        .ToList(), W.Width, W.Range);

            if (Misc.IsChecked(LaneClearMenu, "lcQ") && Q.IsReady() && bestFarmQ.MinionsHit > 0)
            {
                Q.Cast(bestFarmQ.Position.To3D());
            }

            if (Misc.IsChecked(LaneClearMenu, "lcW") && W.IsReady() && bestFarmW.MinionsHit > 0)
            {
                W.Cast(bestFarmW.Position.To3D());
            }

            if (Misc.IsChecked(LaneClearMenu, "lcE") && E.IsReady())
            {
                if (Misc.IsChecked(LaneClearMenu, "lcKE"))
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.First(
                            t =>
                                t.IsValidTarget(E.Range) && _Player.GetSpellDamage(t, SpellSlot.E) > t.Health &&
                                (!Misc.IsChecked(LaneClearMenu, "lcPE") || IsPoisoned(t)));

                    if (minion != null)
                        E.Cast(minion);
                }
                else
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.First(
                            t =>
                                t.IsValidTarget(E.Range) &&
                                (Misc.IsChecked(LaneClearMenu, "lcPE") || IsPoisoned(t)));

                    if (minion != null)
                        E.Cast(minion);
                }
            }

        }

        public void OnFlee()
        {

        }

        public void OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    if (Misc.IsChecked(ComboMenu, "disableAA"))
                        Orbwalker.DisableAttacking = true;

                    OnCombo();
                    break;
                case Orbwalker.ActiveModes.Flee:
                    OnFlee();
                    break;
                case Orbwalker.ActiveModes.Harass:
                    if (Misc.IsChecked(ComboMenu, "disableAAHS"))
                        Orbwalker.DisableAttacking = true;
                    OnHarass();
                    break;
            }

            if (Orbwalker.DisableAttacking && (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.Combo && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.Harass))
                Orbwalker.DisableAttacking = false;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                OnLaneClear();

            if(Misc.IsChecked(MiscMenu, "ksOn"))
                KS();
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

        public void OnAfterAttack(AttackableUnit target, EventArgs args)
        {

        }

        public void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy) return;

            if (Misc.IsChecked(MiscMenu, "miscInterruptDangerous") && interruptableSpellEventArgs.DangerLevel >= DangerLevel.High && R.IsReady() && R.IsInRange(sender))
            {
                R.Cast(sender);
            }
        }

        public void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if(!sender.IsEnemy) return;

            if ((e.End.Distance(_Player) < 50 || e.Sender.IsAttackingPlayer) && Misc.IsChecked(MiscMenu, "miscAntiGapR") &&
                _Player.HealthPercent < Misc.GetSliderValue(MiscMenu, "miscMinHpAntiGap") && R.IsReady() && R.IsInRange(sender))
            {
                R.Cast(sender);
            }else if ((e.End.Distance(_Player) < 50 || e.Sender.IsAttackingPlayer) && Misc.IsChecked(MiscMenu, "miscAntiGapW") && W.IsReady() && W.IsInRange(sender))
            {
                W.Cast(e.End);
            }
        }

        public void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            if(!sender.IsMe) return;

            if (args.SData.Name == "CassiopeiaTwinFang")
            {
                var diffTime = Misc.GetSliderValue(MiscMenu, "miscDelayE") / 100f + _lastECast - Game.Time;

                if (diffTime > 0)
                {
                    args.Process = false;
                }
                else
                {
                    _lastECast = Game.Time;
                }
            }

            if (args.SData.Name == "CassiopeiaPetrofyingGaze" && Misc.IsChecked(MiscMenu, "miscAntiMissR"))
            {
                if (EntityManager.Heroes.Enemies.Count(t => t.IsValidTarget(R.Range) && t.IsFacing(_Player)) < 1)
                {
                    args.Process = false;
                }
            }

        }

        public void GameObjectOnCreate(GameObject sender, EventArgs args)
        {

        }

        public void GameObjectOnDelete(GameObject sender, EventArgs args)
        {

        }

        private static void KS()
        {

            if (E.IsReady() && EntityManager.Heroes.Enemies.Any(t => t.IsValidTarget(E.Range) && t.Health < _Player.GetSpellDamage(t, SpellSlot.E)))
            {
                E.Cast(EntityManager.Heroes.Enemies.FirstOrDefault(t => t.IsValidTarget(E.Range) && t.Health < _Player.GetSpellDamage(t, SpellSlot.E)));
            }

            if (Q.IsReady() && EntityManager.Heroes.Enemies.Any(t => t.IsValidTarget(Q.Range) && t.Health < _Player.GetSpellDamage(t, SpellSlot.Q)))
            {
                var predictionQ = Q.GetPrediction(EntityManager.Heroes.Enemies.FirstOrDefault(t => t.IsValidTarget(Q.Range) && t.Health < _Player.GetSpellDamage(t, SpellSlot.Q)));

                if (predictionQ.HitChancePercent >= 70)
                {
                    Q.Cast(predictionQ.CastPosition);
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

        public bool IsPoisoned(Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Poison);
        }
    }
}

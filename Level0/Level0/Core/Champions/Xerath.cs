using System;
using LevelZero.Model;
using EloBuddy;
using EloBuddy.SDK.Events;
using LevelZero.Model.Values;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using LevelZero.Controller;
using LevelZero.Util;
using SharpDX;

namespace LevelZero.Core.Champions
{
    class Xerath : PluginModel
    {
        #region Scope Variables

        public bool castingR = false;

        #endregion

        public override void Init()
        {
            InitVariables();
            InitEvents();
        }

        public override void InitEvents()
        {
            base.InitEvents();
            Drawing.OnDraw += OnDraw;

        }

        public override void InitVariables()
        {
            Spells = new List<Spell.SpellBase>
            {
                new Spell.Chargeable(SpellSlot.Q, 750, 1550, 1500, 500, int.MaxValue, 100) { AllowedCollisionCount = int.MaxValue },
                new Spell.Skillshot(SpellSlot.W, 1100, SkillShotType.Circular, 750, int.MaxValue, 100) { AllowedCollisionCount = int.MaxValue },
                new Spell.Skillshot(SpellSlot.E, 1050, SkillShotType.Linear, 250, 1100, 60),
                new Spell.Skillshot(SpellSlot.R, 3200, SkillShotType.Circular, 500, int.MaxValue, 120) { AllowedCollisionCount = int.MaxValue }
        };
            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Spells[0], new float[]{ 0, 80 , 120 , 160 , 200 , 240 }, new [] { 0, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f }, DamageType.Magical),
                new SpellDamage(Spells[1], new float[]{ 0, 60 , 90 , 120 , 150 , 180 },  new [] { 0, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f }, DamageType.Magical),
                new SpellDamage(Spells[2], new float[]{ 0, 80 , 110 , 140 , 170 , 200 }, new [] { 0, 0.45f, 0.45f, 0.45f, 0.45f, 0.45f }, DamageType.Magical),
                new SpellDamage(Spells[3], new float[]{ 0, 190 * 3 , 245 * 3, 300 * 3 }, new [] { 0, 0.433f, 0.433f, 0.433f }, DamageType.Magical)
            };
            InitMenu();
            DamageIndicator.Initialize(DamageUtil.GetComboDamage);
        }

        public override void InitMenu()
        {
            var feature = new Feature
            {
                NameFeature = "Draw",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(false, "disable", "Disable"),
                    new ValueCheckbox(true, "dmgIndicator", "Show Damage Indicator"),
                    new ValueCheckbox(false, "draw.q", "Draw Q"),
                    new ValueCheckbox(false, "draw.w", "Draw W"),
                    new ValueCheckbox(false, "draw.e", "Draw E"),
                    new ValueCheckbox(false, "draw.r", "Draw R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Combo",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "combo.q", "Combo Q"),
                    new ValueCheckbox(true,  "combo.w", "Combo W"),
                    new ValueCheckbox(true,  "combo.e", "Combo E"),
                    new ValueCheckbox(true,  "combo.r", "Combo R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Harass",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "harass.mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "harass.q", "Harass Q"),
                    new ValueCheckbox(true,  "harass.w", "Harass W"),
                    new ValueCheckbox(true,  "harass.e", "Harass E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Lane Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "laneclear.mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "laneclear.q", "Lane Clear Q"),
                    new ValueCheckbox(true,  "laneclear.w", "Lane Clear W"),
                    new ValueCheckbox(false, "laneclear.e", "Lane Clear E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Jungle Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "jungleclear.mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "jungleclear.q", "Jungle Clear Q"),
                    new ValueCheckbox(true, "jungleclear.w", "Jungle Clear W"),
                    new ValueCheckbox(true,  "jungleclear.e", "Jungle Clear E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Misc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueKeybind(false, "misc.tapToUlt", "Tap to Cast Utl"),
                    new ValueCheckbox(true,  "misc.antiGapE", "E on gap closers"),
                    new ValueSlider(100, 0 , 40, "misc.antiGapE.antigap", "Minimum HP % to cast R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);
        }

        public override void OnDraw(EventArgs args)
        {
            var draw = Features.Find(f => f.NameFeature == "Draw");

            if (draw.IsChecked("disable"))
            {
                DamageIndicator.Enabled = false;
                return;
            }

            if (draw.IsChecked("draw.q"))
                Circle.Draw(Spells[0].IsReady() ? Color.Blue : Color.Red, Spells[0].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.w"))
                Circle.Draw(Spells[1].IsReady() ? Color.Blue : Color.Red, Spells[1].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.e"))
                Circle.Draw(Spells[2].IsReady() ? Color.Blue : Color.Red, Spells[2].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.r"))
                Circle.Draw(Spells[3].IsReady() ? Color.Blue : Color.Red, Spells[3].Range, Player.Instance.Position);

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        /*
            Spells[0] = Q
            Spells[1] = W
            Spells[2] = E
            Spells[3] = R
        */

        public override void OnCombo()
        {
            var target = TargetSelector.GetTarget(Spells[3].Range, DamageType.Magical);

            if(target == null || !target.IsValidTarget()) return;

            var combo = Features.Find(f => f.NameFeature == "Combo");
            var Q = ((Spell.Chargeable) Spells[0]);
            var W = ((Spell.Skillshot)  Spells[1]);
            var E = ((Spell.Skillshot)  Spells[2]);
            var R = ((Spell.Skillshot)  Spells[3]);

            if (combo.IsChecked("combo.q") && Q.IsReady() && target.Distance(Player.Instance) < Q.MaximumRange && !castingR)
            {
                if (!Q.IsCharging)
                {
                    Q.StartCharging();
                }
                else if (Q.Range == Q.MaximumRange)
                {
                    if (Q.Cast(target.ServerPosition))
                    {
                    }
                }
                else
                {
                    var qPrediction = Q.GetPrediction(target);
                    var predictionPost = Prediction.Position.PredictUnitPosition(target, 300);

                    if(predictionPost.Distance(Player.Instance) > Q.Range && Q.IsCharging) return;

                    if (qPrediction.HitChancePercent >= 85)
                    {
                        Q.Cast(qPrediction.CastPosition);
                    }
                }
            }

            if (Q.IsCharging)
            {
                return;
            }

            if (combo.IsChecked("combo.r") && R.IsReady() && R.IsInRange(target) && ((!Q.IsInRange(target) && DamageUtil.Killable(target, SpellSlot.R, 50)) || castingR))
            {
                var predictionR = R.GetPrediction(target);

                if (predictionR.HitChancePercent >= 80)
                {
                    if (!castingR)
                    {
                        castingR = true;
                        Orbwalker.DisableMovement = true;
                        Orbwalker.DisableAttacking = true;
                    }

                    R.Cast(predictionR.CastPosition);
                     
                }else if (castingR && predictionR.HitChancePercent >= 70)
                {
                    R.Cast(predictionR.CastPosition);
                }
            }else if (!R.IsReady())
            {
                castingR = false;
                Orbwalker.DisableMovement = false;
                Orbwalker.DisableAttacking = false;
            }

            if (combo.IsChecked("combo.w") && W.IsReady() && W.IsInRange(target) && !castingR)
            {
                var predictionW = W.GetPrediction(target);
                var posPrediction = Prediction.Position.PredictUnitPosition(target, 500);

                if (predictionW.HitChancePercent >= 85 && predictionW.CastPosition.Distance(posPrediction) < 70)
                {
                    W.Cast(predictionW.CastPosition);
                }
            }

            if (combo.IsChecked("combo.e") && E.IsReady() && E.IsInRange(target) && !castingR)
            {
                var predictionE = E.GetPrediction(target);

                if (predictionE.HitChancePercent >= 85)
                {
                    E.Cast(predictionE.CastPosition);
                }
            }
        }

        public override void OnHarass()
        {
            var target = TargetSelector.GetTarget(1550, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            var harass = Features.Find(f => f.NameFeature == "Harass");
            var Q = ((Spell.Chargeable)Spells[0]);
            var W = ((Spell.Skillshot)Spells[1]);
            var E = ((Spell.Skillshot)Spells[2]);

            if (harass.IsChecked("harass.q") && Q.IsReady() && target.Distance(Player.Instance) < Q.MaximumRange)
            {
                if (!Q.IsCharging)
                {
                    Q.StartCharging();
                }
                else if (Q.Range == Q.MaximumRange)
                {
                    if (Q.Cast(target.ServerPosition))
                    {
                    }
                }
                else
                {
                    var qPrediction = Q.GetPrediction(target);
                    var predictionPost = Prediction.Position.PredictUnitPosition(target, 300);

                    if (predictionPost.Distance(Player.Instance) > Q.Range && Q.IsCharging) return;

                    if (qPrediction.HitChancePercent >= 85)
                    {
                        Q.Cast(qPrediction.CastPosition);
                    }
                }
            }

            if (Q.IsCharging)
            {
                return;
            }

            if (harass.IsChecked("harass.w") && W.IsReady() && W.IsInRange(target))
            {
                var predictionW = W.GetPrediction(target);
                var posPrediction = Prediction.Position.PredictUnitPosition(target, 500);

                if (predictionW.HitChancePercent >= 85 && predictionW.CastPosition.Distance(posPrediction) < 70)
                {
                    W.Cast(predictionW.CastPosition);
                }
            }

            if (harass.IsChecked("harass.e") && E.IsReady() && E.IsInRange(target))
            {
                var predictionE = E.GetPrediction(target);

                if (predictionE.HitChancePercent >= 85)
                {
                    E.Cast(predictionE.CastPosition);
                }
            }
        }

        public override void OnLaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsValidTarget(1000));

            if (minions == null || !minions.Any(t => t.IsValidTarget(1000))) return;

            var laneclear = Features.Find(f => f.NameFeature == "Lane Clear");

            var Q = ((Spell.Chargeable)Spells[0]);
            var W = ((Spell.Skillshot)Spells[1]);
            var E = ((Spell.Skillshot)Spells[2]);

            if (laneclear.IsChecked("laneclear.q") && Q.IsReady())
            {
                var bestFarmPostion = PredictionUtil.GetBestLineFarmLocation(minions.Select(o => o.ServerPosition.To2D()).ToList(), Q.Width, Q.MaximumRange);
                if (!Q.IsCharging && bestFarmPostion.MinionsHit >= 2)
                {
                    Q.StartCharging();
                }
                else if (Q.Range == Q.MaximumRange)
                {
                    if (Q.Cast(minions.FirstOrDefault().ServerPosition))
                    {
                    }
                }
                else
                {
                    if (bestFarmPostion.Position.Distance(Player.Instance) > Q.Range && Q.IsCharging) return;

                    Q.Cast(bestFarmPostion.Position.To3D());
                }
            }

            if (Q.IsCharging)
            {
                return;
            }

            if (laneclear.IsChecked("laneclear.w") && W.IsReady())
            {
                var bestFarmPosition =
                    PredictionUtil.GetBestCircularFarmLocation(minions.Select(o => o.ServerPosition.To2D()).ToList(),
                        W.Width, W.Range);

                if (bestFarmPosition.MinionsHit >= 2)
                {
                    W.Cast(bestFarmPosition.Position.To3D());
                }
            }

            if (laneclear.IsChecked("laneclear.e") && E.IsReady() && E.IsInRange(minions.FirstOrDefault()))
            {
                var predictionE = E.GetPrediction(minions.FirstOrDefault());

                if (predictionE.HitChancePercent >= 85)
                {
                    E.Cast(predictionE.CastPosition);
                }
            }
        }

        public override void OnJungleClear()
        {
            var minions = EntityManager.MinionsAndMonsters.Monsters.Where(t => t.IsValidTarget(1000));

            if (minions == null || !minions.Any(t => t.IsValidTarget(1000))) return;

            var jungleclear = Features.Find(f => f.NameFeature == "Jungle Clear");

            var Q = ((Spell.Chargeable)Spells[0]);
            var W = ((Spell.Skillshot)Spells[1]);
            var E = ((Spell.Skillshot)Spells[2]);

            if (jungleclear.IsChecked("jungleclear.q") && Q.IsReady())
            {
                var bestFarmPostion = PredictionUtil.GetBestLineFarmLocation(minions.Select(o => o.ServerPosition.To2D()).ToList(), Q.Width, Q.MaximumRange);
                if (!Q.IsCharging)
                {
                    Q.StartCharging();
                }
                else if (Q.Range == Q.MaximumRange)
                {
                    if (Q.Cast(minions.FirstOrDefault().ServerPosition))
                    {
                    }
                }
                else
                {
                    if (bestFarmPostion.Position.Distance(Player.Instance) > Q.Range && Q.IsCharging) return;

                    Q.Cast(bestFarmPostion.Position.To3D());
                }
            }

            if (Q.IsCharging)
            {
                return;
            }

            if (jungleclear.IsChecked("jungleclear.w") && W.IsReady())
            {
                var bestFarmPosition =
                    PredictionUtil.GetBestCircularFarmLocation(minions.Select(o => o.ServerPosition.To2D()).ToList(),
                        W.Width, W.Range);

                W.Cast(bestFarmPosition.Position.To3D());
            }

            if (jungleclear.IsChecked("jungleclear.e") && E.IsReady() && E.IsInRange(minions.FirstOrDefault()))
            {
                var predictionE = E.GetPrediction(minions.FirstOrDefault());

                if (predictionE.HitChancePercent >= 85)
                {
                    E.Cast(predictionE.CastPosition);
                }
            }
        }

        public override void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            base.OnGapCloser(sender, e);

            var misc = Features.Find(f => f.NameFeature == "Misc");

            if(!misc.IsChecked("misc.antiGapE")) return;

            if (e.End.Distance(Player.Instance) < 50 && Player.Instance.HealthPercent <= misc.SliderValue("misc.antiGapE.antigap"))
            {
                Spells[2].Cast(sender.Position);
            }
        }

        public override void OnPlayerLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            base.OnPlayerLevelUp(sender, args);

            Spells[3].Range = (uint) (2000 + (Spells[3].Level * 1200));
        }

        public override void OnUpdate(EventArgs args)
        {
            if (Features.Find(f => f.NameFeature == "Misc").IsChecked("misc.tapToUlt"))
            {
                var target = TargetSelector.GetTarget(Spells[3].Range, DamageType.Magical);

                if (target == null || !target.IsValidTarget()) return;

                var R = ((Spell.Skillshot) Spells[3]);

                if (R.IsReady() && R.IsInRange(target) || castingR)
                {
                    var predictionR = R.GetPrediction(target);

                    if (predictionR.HitChancePercent >= 80)
                    {
                        if (!castingR)
                        {
                            castingR = true;
                            Orbwalker.DisableMovement = true;
                            Orbwalker.DisableAttacking = true;
                        }

                        R.Cast(predictionR.CastPosition);

                    }
                    else if (castingR && predictionR.HitChancePercent >= 70)
                    {
                        R.Cast(predictionR.CastPosition);
                    }
                }
                else if (!R.IsReady())
                {
                    castingR = false;
                    Orbwalker.DisableMovement = false;
                    Orbwalker.DisableAttacking = false;
                }
            }

            base.OnUpdate(args);
        }
    }
}

using System;
using LevelZero.Model;
using EloBuddy;
using EloBuddy.SDK.Events;
using LevelZero.Model.Values;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Rendering;
using LevelZero.Controller;
using LevelZero.Util;
using SharpDX;

namespace LevelZero.Core.Champions
{
    class Tristana : PluginModel
    {
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
                new Spell.Active(SpellSlot.Q),
                new Spell.Skillshot(SpellSlot.W, 1100, SkillShotType.Circular),
                new Spell.Targeted(SpellSlot.E, (uint) Player.Instance.AttackRange),
                new Spell.Targeted(SpellSlot.R, (uint) Player.Instance.AttackRange)
            };
            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Spells[1], new float[]{ 60 , 110 , 160 , 210 , 260 }, new [] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f }, DamageType.Magical),
                new SpellDamage(Spells[2], new float[]{ 110 , 150 , 190 , 230 , 270 }, new [] { 1f, 1f, 1f, 1f, 1f }, DamageType.Magical),
                new SpellDamage(Spells[3], new float[]{ 300 , 400 , 500 }, new [] { 1f, 1f, 1f }, DamageType.Magical)
            };
            InitMenu();
            //DamageIndicator.Initialize(DamageUtil.GetComboDamage);
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
                    new ValueCheckbox(true, "draw.w", "Draw W"),
                    new ValueCheckbox(true, "draw.e", "Draw E"),
                    new ValueCheckbox(true, "draw.r", "Draw R")
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
                    new ValueCheckbox(false, "combo.w", "Combo W"),
                    new ValueCheckbox(true,  "combo.e", "Combo E"),
                    new ValueCheckbox(true,  "combo.r", "Combo R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "ComboMisc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(false,  "combo.misc.qifE", "Only use Q if Cast E"),
                    new ValueCheckbox(false,  "combo.misc.forceAA", "Force AA on E casted target"),
                    new ValueSlider(200, 0 , 50, "combo.misc.rCorrection", "Damage Correction of R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Harass",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "harass.q", "Harass Q"),
                    new ValueCheckbox(false, "harass.w", "Harass W"),
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
                    new ValueCheckbox(true,  "laneclear.q", "Lane Clear Q"),
                    new ValueCheckbox(false, "laneclear.w", "Lane Clear W"),
                    new ValueCheckbox(true,  "laneclear.e", "Lane Clear E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Jungle Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "jungleclear.q", "Jungle Clear Q"),
                    new ValueCheckbox(false, "jungleclear.w", "Jungle Clear W"),
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
                    new ValueCheckbox(true,  "misc.antiGapR", "R on gap closers"),
                    new ValueSlider(100, 0 , 40, "misc.antiGapR.antigap", "Minimum HP % to cast R")
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

            if(draw.IsChecked("draw.w"))
                Circle.Draw(Spells[1].IsReady() ? Color.Blue : Color.Red, Spells[1].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.e"))
                Circle.Draw(Spells[2].IsReady() ? Color.Blue : Color.Red, Spells[2].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.r"))
                Circle.Draw(Spells[3].IsReady() ? Color.Blue : Color.Red, Spells[3].Range, Player.Instance.Position);

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        public override void OnCombo()
        {
            var target = TargetSelector.GetTarget(900, DamageType.Physical);

            if(target == null || !target.IsValidTarget() || !target.IsValidTargetUtil()) return;

            var combo = Features.Find(f => f.NameFeature == "Combo");
            var comboMisc = Features.Find(f => f.NameFeature == "ComboMisc");

            if (comboMisc.IsChecked("combo.misc.forceAA") && target.HasBuff("tristanaecharge") && Player.Instance.IsInAutoAttackRange(target))
            {
                Orbwalker.ForcedTarget = target;
            }
            else
            {
                Orbwalker.ForcedTarget = null;
            }

            if (combo.IsChecked("combo.q") && Spells[0].IsReady() && Player.Instance.IsInAutoAttackRange(target))
            {
                if (comboMisc.IsChecked("combo.misc.qifE") && comboMisc.IsChecked("combo.e") && Spells[2].IsReady() &&
                    Spells[2].IsInRange(target))
                {
                    if (target.HasBuff("tristanaecharge"))
                    {
                        Spells[2].Cast(target);
                        EloBuddy.SDK.Core.DelayAction(() => Spells[0].Cast(), 150);
                    }
                }
                else
                {
                    Spells[0].Cast();
                }
            }

            if (combo.IsChecked("combo.e") && Spells[2].IsReady() && Spells[2].IsInRange(target))
            {
                Spells[2].Cast(target);
            }

            if (combo.IsChecked("combo.r") && Spells[3].IsReady() && Spells[3].IsInRange(target))
            {
                if (DamageUtil.Killable(target, SpellSlot.R, combo.SliderValue("combo.misc.rCorrection")) || (DamageUtil.Killable(target, SpellSlot.R, combo.SliderValue("combo.misc.rCorrection") - (target.GetBuffCount("tristanaecharge") * 0.3f))))
                {
                    Spells[3].Cast(target);
                }
            }

            if (combo.IsChecked("combo.w") && Spells[1].IsReady() && !Player.Instance.IsInAutoAttackRange(target)
                && ((DamageUtil.Killable(target, SpellSlot.R, combo.SliderValue("combo.misc.rCorrection")) && Spells[3].IsReady()) || Player.Instance.GetAutoAttackDamage(target) > target.Health))
            {
                var targetMovPrediction = Prediction.Position.PredictUnitPosition(target, 250);

                if (targetMovPrediction.Distance(Player.Instance.ServerPosition) <
                    Spells[1].Range + Player.Instance.AttackRange)
                {
                    Spells[1].Cast(targetMovPrediction.To3D());
                }
            }
        }

        public override void OnHarass()
        {
            var target = TargetSelector.GetTarget(900, DamageType.Physical);

            if (target == null || !target.IsValidTarget() || !target.IsValidTargetUtil()) return;

            var harass = Features.Find(f => f.NameFeature == "Harass");

            if (harass.IsChecked("harass.q") && Spells[0].IsReady() && Player.Instance.IsInAutoAttackRange(target))
            {
                
                Spells[0].Cast();
            }

            if (harass.IsChecked("harass.e") && Spells[2].IsReady() && Spells[2].IsInRange(target))
            {
                Spells[2].Cast(target);
            }

            if (harass.IsChecked("harass.w") && Spells[1].IsReady() && !Player.Instance.IsInAutoAttackRange(target)
                && Player.Instance.GetAutoAttackDamage(target) > target.Health)
            {
                var targetMovPrediction = Prediction.Position.PredictUnitPosition(target, 250);

                if (targetMovPrediction.Distance(Player.Instance.ServerPosition) <
                    Spells[1].Range + Player.Instance.AttackRange)
                {
                    Spells[1].Cast(targetMovPrediction.To3D());
                }
            }
        }

        public override void OnLaneClear()
        {
            var target = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(t => t.IsValidTarget(Player.Instance.AttackRange));

            if (target == null || !target.IsValidTarget() || !target.IsValidTargetUtil()) return;

            var laneclear = Features.Find(f => f.NameFeature == "Lane Clear");

            if (laneclear.IsChecked("laneclear.q") && Spells[0].IsReady() && Player.Instance.IsInAutoAttackRange(target))
            {
                Spells[0].Cast();
            }

            if (laneclear.IsChecked("laneclear.e") && Spells[2].IsReady() && Spells[2].IsInRange(target))
            {
                var eTarget = EntityManager.MinionsAndMonsters.EnemyMinions.Aggregate((curMax, x) => ((curMax == null && x.IsValidTarget(Player.Instance.AttackRange)) || x.MaxHealth > curMax.MaxHealth ? x : curMax));
                if (eTarget != null)
                {
                    Spells[2].Cast(eTarget);
                    Orbwalker.ForcedTarget = eTarget;
                }
            }

            if (laneclear.IsChecked("laneclear.w") && Spells[1].IsReady() && !Player.Instance.IsInAutoAttackRange(target))
            {
                var bestFarm = PredictionUtil.GetBestCircularFarmLocation(
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.Distance(Player.Instance) <= Spells[1].Range)
                        .Select(xm => xm.ServerPosition.To2D())
                        .ToList(), 250, Spells[1].Range);

                Spells[1].Cast(bestFarm.Position.To3D());

            }
        }

        public override void OnJungleClear()
        {
            if(!EntityManager.MinionsAndMonsters.Monsters.Any(m => m.Distance(Player.Instance) < Spells[1].Range)) return;

            var target = EntityManager.MinionsAndMonsters.Monsters.FirstOrDefault(t => t.IsValidTarget(Player.Instance.AttackRange));

            if (target == null || !target.IsValidTarget() || !target.IsValidTargetUtil()) return;

            var jungleclear = Features.Find(f => f.NameFeature == "Jungle Clear");

            if (jungleclear.IsChecked("jungleclear.q") && Spells[0].IsReady() && Player.Instance.IsInAutoAttackRange(target))
            {
                Spells[0].Cast();
            }

            if (jungleclear.IsChecked("jungleclear.e") && Spells[2].IsReady() && Spells[2].IsInRange(target))
            {
                var eTarget = EntityManager.MinionsAndMonsters.Monsters.Aggregate((curMax, x) => ((curMax == null && x.IsValid) || x.MaxHealth > curMax.MaxHealth ? x : curMax));
                if (eTarget != null)
                {
                    Spells[2].Cast(eTarget);
                    Orbwalker.ForcedTarget = eTarget;
                }
            }

            if (jungleclear.IsChecked("jungleclear.w") && Spells[1].IsReady() && !Player.Instance.IsInAutoAttackRange(target)
                && Player.Instance.GetAutoAttackDamage(target) > target.Health)
            {
                var bestFarm = PredictionUtil.GetBestCircularFarmLocation(
                    EntityManager.MinionsAndMonsters.Monsters.Where(x => x.Distance(Player.Instance) <= Spells[1].Range)
                        .Select(xm => xm.ServerPosition.To2D())
                        .ToList(), 250, Spells[1].Range);

                Spells[1].Cast(bestFarm.Position.To3D());
            }
        }

        public override void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            base.OnGapCloser(sender, e);

            var misc = Features.Find(f => f.NameFeature == "Misc");

            if (e.End.Distance(Player.Instance) < 50 && Player.Instance.HealthPercent < misc.SliderValue("misc.antiGapR.antigap"))
            {
                Spells[3].Cast(sender);
            }
        }

        public override void OnPlayerLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            base.OnPlayerLevelUp(sender, args);

            Spells[2].Range = (uint) Player.Instance.AttackRange;
            Spells[3].Range = (uint) Player.Instance.AttackRange;
        }
    }
}

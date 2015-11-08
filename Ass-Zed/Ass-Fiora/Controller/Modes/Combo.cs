using System;
using System.Collections.Generic;
using System.Linq;
using Ass_Zed.Helpers;
using Ass_Zed.Model;
using BRSelector.Model;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using OneForWeek.Util.Misc;

namespace Ass_Zed.Controller.Modes
{
    public sealed class Combo : ModeBase
    {

        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {

            if (PluginModel.ActionQueue.ExecuteNextAction(PluginModel.ComboQueue))
            {
                return;
            }

            var q = PluginModel.Q;
            var r = PluginModel.R;
            var w = PluginModel.W;
            var e = PluginModel.E;

            var target = AdvancedTargetSelector.GetTarget(q.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget())
            {
                if (Player.Instance.HasBuff("zedr2") && r.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboR") && ShadowManager.CanCastR(false))
                {
                    var nearEnemyTower =
                        EntityManager.Turrets.Enemies.FirstOrDefault(t => t.Distance(Player.Instance) < 1050 && !t.IsDead);
                    if(nearEnemyTower == null) return;

                    r.Cast();
                }

                return;
            }

            if (Misc.IsChecked(PluginModel.ComboMenu, "comboStyle"))
            {
                #region All In

                if (Player.Instance.HasBuff("zedr2") && r.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboR"))
                {
                    if (w.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboW") && w.State != SpellState.NoMana)
                    {
                        if (Player.Instance.HasBuff("zedwhandler") &&
                            Player.Instance.AttackRange <= Player.Instance.Distance(target) && !Player.Instance.HasBuff("zedr2"))
                        {
                            var shadows = ShadowManager.GetShadows();

                            if (shadows.Count > 0)
                            {
                                foreach (
                                    var shadow in
                                        shadows.Where(
                                            shadow => shadow.Distance(target) < Player.Instance.Distance(target)))
                                {
                                    ShadowManager.Cast(target);
                                }
                            }
                        }
                        else
                        {
                            if (!Player.Instance.HasBuff("zedwhandler"))
                            {
                                Player.CastSpell(SpellSlot.W,
                                    Prediction.Position.PredictUnitPosition(target, 250).To3D());
                            }
                        }
                    }

                    if (e.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboE") && e.State != SpellState.NoMana)
                    {
                        var shadows = ShadowManager.GetShadows();
                        if (Player.Instance.Distance(target) < e.Range)
                        {
                            e.Cast();
                        }
                        else if (shadows.Count > 0)
                        {
                            foreach (
                                var shadow in
                                    shadows.Where(shadow => shadow.Distance(target) < Player.Instance.Distance(target)))
                            {
                                e.Cast();
                            }
                        }
                    }

                    if (q.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboQ") && q.IsInRange(target) &&
                        q.State != SpellState.NoMana)
                    {
                        var predictionQ = q.GetPrediction(target);

                        if (predictionQ.HitChance >= HitChance.Medium)
                        {
                            q.Cast(predictionQ.CastPosition);
                        }
                    }

                    if (!q.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboQ") &&
                        (!w.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboW") ||
                         Player.Instance.HasBuff("zedwhandler")) && !e.IsReady() &&
                        Misc.IsChecked(PluginModel.ComboMenu, "comboE") && Orbwalker.CanAutoAttack &&
                        Player.Instance.IsInAutoAttackRange(target))
                    {
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                    else if (!q.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboQ") &&
                             (!w.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboW") ||
                              Player.Instance.HasBuff("zedwhandler")) && !e.IsReady() &&
                             Misc.IsChecked(PluginModel.ComboMenu, "comboE") &&
                             Orbwalker.CanAutoAttack && !Player.Instance.IsInAutoAttackRange(target))
                    {
                        Orbwalker.OrbwalkTo(target.Position);
                    }
                }
                else if (r.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboR") && !Player.Instance.HasBuff("zedr2") && ShadowManager.CanCastR(true))
                {
                    if (r.IsInRange(target) && !Player.Instance.HasBuff("zedr2"))
                    {
                        r.Cast(target);
                    }
                }
                else
                {
                    if (w.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboW") && w.State != SpellState.NoMana)
                    {
                        if (Player.Instance.HasBuff("zedwhandler") && Player.Instance.IsInAutoAttackRange(target))
                        {
                            var shadows = ShadowManager.GetShadows();

                            if (shadows.Count > 0)
                            {
                                var turret = EntityManager.Turrets.Enemies.FirstOrDefault(t => t.Distance(target) < 1050);
                                foreach (
                                    var shadow in
                                        shadows.Where(
                                            shadow =>
                                                shadow.Distance(target) < Player.Instance.Distance(target) &&
                                                (turret == null || turret.IsDead)))
                                {
                                    ShadowManager.Cast(target);
                                }
                            }
                        }
                        else
                        {
                            if (!Player.Instance.HasBuff("zedwhandler"))
                            {
                                Player.CastSpell(SpellSlot.W,
                                    Prediction.Position.PredictUnitPosition(target, 250).To3D());
                            }
                        }
                    }

                    if (e.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboE") &&
                        Player.Instance.Distance(target) < e.Range && e.State != SpellState.NoMana)
                    {
                        e.Cast();
                    }

                    if (q.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboQ") && q.IsInRange(target) &&
                        q.State != SpellState.NoMana)
                    {
                        var predictionQ = q.GetPrediction(target);

                        if (predictionQ.HitChance >= HitChance.Medium)
                        {
                            q.Cast(predictionQ.CastPosition);
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region lineCombo

                if (Player.Instance.HasBuff("zedr2") && r.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboR") && ShadowManager.CanCastR(false))
                {
                    ItemManager.UseAll(target);
                    if (w.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboW") && w.State != SpellState.NoMana)
                    {
                        if (Player.Instance.HasBuff("zedwhandler") && Player.Instance.AttackRange <= Player.Instance.Distance(target))
                        {
                            var shadows = ShadowManager.GetShadows();

                            if (shadows.Count > 0)
                            {
                                foreach (var shadow in shadows.Where(shadow => shadow.Distance(target) < Player.Instance.Distance(target)))
                                {
                                    ShadowManager.Cast(target);
                                }
                            }
                        }
                        else
                        {
                            if (!Player.Instance.HasBuff("zedwhandler") && PluginModel.StartPosCombo.IsZero)
                            {
                                var extendedAfterStartCombo = PluginModel.StartPosCombo.Extend(target,
                                    target.Distance(PluginModel.StartPosCombo) + w.Range);

                                Player.CastSpell(SpellSlot.W, extendedAfterStartCombo.To3D());
                            }
                        }
                    }

                    if (e.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboE") && e.State != SpellState.NoMana)
                    {
                        var shadows = ShadowManager.GetShadows();
                        if (Player.Instance.Distance(target) < e.Range)
                        {
                            e.Cast();
                        }
                        else if (shadows.Count > 0)
                        {
                            foreach (var shadow in shadows.Where(shadow => shadow.Distance(target) < Player.Instance.Distance(target)))
                            {
                                e.Cast();
                            }
                        }
                    }

                    if (q.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboQ") && q.IsInRange(target) && q.State != SpellState.NoMana)
                    {
                        var predictionQ = q.GetPrediction(target);

                        if (predictionQ.HitChance >= HitChance.Medium)
                        {
                            q.Cast(predictionQ.CastPosition);
                        }
                    }

                    if (!q.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboQ") && (!w.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboW") || Player.Instance.HasBuff("zedwhandler")) && !e.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboE") && Orbwalker.CanAutoAttack && Player.Instance.IsInAutoAttackRange(target))
                    {
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                    else if (!q.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboQ") && (!w.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboW") || Player.Instance.HasBuff("zedwhandler")) && !e.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboE") &&
                             Orbwalker.CanAutoAttack && !Player.Instance.IsInAutoAttackRange(target))
                    {
                        Orbwalker.OrbwalkTo(target.Position);
                    }
                }
                else if (r.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboR") && r.State != SpellState.NoMana && !Player.Instance.HasBuff("zedr2") && ShadowManager.CanCastR(true))
                {
                    if (r.IsInRange(target))
                    {
                        PluginModel.StartPosCombo = Player.Instance.ServerPosition;
                        r.Cast(target);
                        PluginModel.LastRCast = Game.Time;
                    }
                }
                else
                {
                    if (w.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboW") && w.State != SpellState.NoMana)
                    {
                        if (Player.Instance.HasBuff("zedwhandler") && Player.Instance.IsInAutoAttackRange(target))
                        {
                            var shadows = ShadowManager.GetShadows();

                            if (shadows.Count > 0)
                            {
                                var turret = EntityManager.Turrets.Enemies.FirstOrDefault(t => t.Distance(target) < 1050);
                                foreach (var shadow in shadows.Where(shadow => shadow.Distance(target) < Player.Instance.Distance(target) && (turret == null || turret.IsDead)))
                                {
                                    ShadowManager.Cast(target);
                                }
                            }
                        }
                        else
                        {
                            if (!Player.Instance.HasBuff("zedwhandler"))
                            {
                                Player.CastSpell(SpellSlot.W, Prediction.Position.PredictUnitPosition(target, 250).To3D());
                            }
                        }
                    }

                    if (e.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboE") && Player.Instance.Distance(target) < e.Range && e.State != SpellState.NoMana)
                    {
                        e.Cast();
                    }

                    if (q.IsReady() && Misc.IsChecked(PluginModel.ComboMenu, "comboQ") && q.IsInRange(target) && q.State != SpellState.NoMana)
                    {
                        var predictionQ = q.GetPrediction(target);

                        if (predictionQ.HitChance >= HitChance.Medium)
                        {
                            q.Cast(predictionQ.CastPosition);
                        }
                    }
                }

                #endregion
            }
        }
    }
}

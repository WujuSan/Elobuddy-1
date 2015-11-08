using System;
using System.Linq;
using Ass_Zed.Common.DamageIndicator;
using Ass_Zed.Helpers;
using Ass_Zed.Model;
using BRSelector.Model;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using OneForWeek.Util.Misc;

namespace Ass_Zed.Controller.Modes
{
    public sealed class PermaActive : ModeBase
    {

        public override bool ShouldBeExecuted()
        {
            return true;
        }

        public override void Execute()
        {
            var q = PluginModel.Q;
            var w = PluginModel.W;
            var e = PluginModel.E;

            var target = AdvancedTargetSelector.GetTarget(q.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget() || Player.Instance.HasBuff("zedr2") || target.IsDead) return;

            if(DmgLib.PossibleDamage(target) < target.Health) return;

            if (w.IsReady() && e.IsReady() && q.IsReady() && Misc.IsChecked(PluginModel.PermaActiveMenu, "paQ") && Misc.IsChecked(PluginModel.PermaActiveMenu, "paW") && Misc.IsChecked(PluginModel.PermaActiveMenu, "paE") && !Player.Instance.HasBuff("zedwhandler") && !target.IsDead)
            {
                Player.CastSpell(SpellSlot.W, target.Position);
            }

            if (e.IsReady() && Misc.IsChecked(PluginModel.PermaActiveMenu, "paE") && !target.IsDead)
            {
                if (e.IsInRange(target))
                {
                    e.Cast();
                }
                else
                {
                    var shadows = ShadowManager.GetShadows();

                    if (shadows.Count > 0)
                    {
                        if (shadows.FirstOrDefault(shadow => shadow.Distance(target) < e.Range) != null)
                            e.Cast();
                    }
                }
            }

            if (q.IsReady() && Misc.IsChecked(PluginModel.PermaActiveMenu, "paQ") && !target.IsDead)
            {
                var predictionQ = q.GetPrediction(target);
                var shadows = ShadowManager.GetShadows();

                if (predictionQ.HitChance >= HitChance.High)
                {
                    q.Cast(predictionQ.CastPosition);
                }
                else if (shadows.Count > 0)
                {
                    var predictionPos = Prediction.Position.PredictUnitPosition(target, q.CastDelay);
                    if (shadows.FirstOrDefault(shadow => shadow.Distance(target) < q.Range) != null)
                        q.Cast(predictionPos.To3D());
                }
            }
        }
    }
}

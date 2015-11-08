using System;
using System.Linq;
using Ass_Zed.Helpers;
using Ass_Zed.Model;
using Ass_Zed.Model.Enum;
using BRSelector.Model;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using OneForWeek.Util.Misc;

namespace Ass_Zed.Controller.Modes
{
    public sealed class Harass : ModeBase
    {

        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            var q = PluginModel.Q;
            var w = PluginModel.W;
            var e = PluginModel.E;

            var target = AdvancedTargetSelector.GetTarget(q.Range + w.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget()) return;

            if (w.IsReady() && e.IsReady() && q.IsReady() && Misc.IsChecked(PluginModel.HarassMenu, "hsQ") && Misc.IsChecked(PluginModel.HarassMenu, "hsW") && Misc.IsChecked(PluginModel.HarassMenu, "hsE") && !Player.Instance.HasBuff("zedwhandler"))
            {
                var predictionW = w.GetPrediction(target);

                if (predictionW.HitChance >= HitChance.Medium && !Player.Instance.HasBuff("zedwhandler") && ShadowManager.CanCastW)
                {
                    Player.CastSpell(SpellSlot.W, predictionW.CastPosition);
                }
            }

            if (e.IsReady() && Misc.IsChecked(PluginModel.HarassMenu, "hsE"))
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

            if (q.IsReady() && Misc.IsChecked(PluginModel.HarassMenu, "hsQ"))
            {
                var predictionQ = q.GetPrediction(target);

                if (predictionQ.HitChance >= HitChance.High)
                {
                    q.Cast(predictionQ.CastPosition);
                }
            }

        }
    }
}

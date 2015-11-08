using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ass_Zed.Controller;
using Ass_Zed.Model;
using BRSelector.Util;
using EloBuddy;
using EloBuddy.SDK;

namespace Ass_Zed.Helpers
{
    public class ZedComboMaker : PluginModel
    {

        public void AllStarCombo(Obj_AI_Base target)
        {
            ActionQueue.EnqueueAction(
                ComboQueue,
                () => R.IsReady() && Player.Instance.IsVisible,
                () =>
                {
                    R.Cast(target);
                    LastRCast = Game.Time;
                },
                () => R.IsReady() && !Player.Instance.IsVisible && !ShadowManager.CanCastR(true));
            ActionQueue.EnqueueAction(
                ComboQueue,
                () => true,
                () => ItemManager.UseAll(target),
                () => true);
            ActionQueue.EnqueueAction(
                ComboQueue,
                () => Misc.IsChecked(ComboMenu, "comboW") && ShadowManager.CanCastW,
                () => ShadowManager.Cast(target.ServerPosition),
                () => target.IsDead || target.IsZombie || !Misc.IsChecked(ComboMenu, "comboW"));
            ActionQueue.EnqueueAction(
                ComboQueue,
                () => Misc.IsChecked(ComboMenu, "comboQ") && Q.IsReady(),
                () => Q.Cast(Q.GetPrediction(target).CastPosition),
                () => target.IsDead || target.IsZombie || !Q.IsReady() || !Misc.IsChecked(ComboMenu, "comboQ"));
            ActionQueue.EnqueueAction(
                ComboQueue,
                () => Misc.IsChecked(ComboMenu, "comboE") && E.IsReady() && Player.Instance.GetAutoAttackRange() >= Player.Instance.Distance(target),
                () => E.Cast(),
                () => target.IsDead || target.IsZombie || !E.IsReady() || !Misc.IsChecked(ComboMenu, "comboE") || !E.IsInRange(target));
        }

    }
}

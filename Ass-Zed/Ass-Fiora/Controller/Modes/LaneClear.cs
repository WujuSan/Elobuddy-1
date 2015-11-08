using System.Linq;
using Ass_Zed.Helpers;
using Ass_Zed.Model;
using BRSelector.Util;
using EloBuddy;
using EloBuddy.SDK;

namespace Ass_Zed.Controller.Modes
{
    public sealed class LaneClear : ModeBase
    {

        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            var q = PluginModel.Q;
            var w = PluginModel.W;
            var e = PluginModel.E;

            var minionTarget = EntityManager.MinionsAndMonsters.EnemyMinions.Aggregate((curMin, x) => (curMin == null || x.Health < curMin.Health ? x : curMin));

            if (minionTarget == null || !minionTarget.IsValidTarget()) return;

            if (q.IsReady() && q.IsInRange(minionTarget) && Misc.IsChecked(PluginModel.LaneClearMenu, "lcQ"))
            {
                q.Cast(minionTarget.Position);
            }

            if(!Player.Instance.IsInAutoAttackRange(minionTarget) && w.IsReady() && w.IsInRange(minionTarget) && Misc.IsChecked(PluginModel.LaneClearMenu, "lcW") && ShadowManager.CanCastW)
            {
                w.Cast(minionTarget.Position);
            }

            if (e.IsReady() && EntityManager.MinionsAndMonsters.EnemyMinions.Count(m => !m.IsDead && m.Distance(Player.Instance) < 290 ) >= 2 && Misc.IsChecked(PluginModel.LaneClearMenu, "lcE"))
            {
                e.Cast();
            }

        }
    }
}

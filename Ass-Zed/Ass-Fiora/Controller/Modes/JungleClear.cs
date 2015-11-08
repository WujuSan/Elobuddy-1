using System.Linq;
using Ass_Zed.Helpers;
using Ass_Zed.Model;
using BRSelector.Util;
using EloBuddy;
using EloBuddy.SDK;

namespace Ass_Zed.Controller.Modes
{
    public sealed class JungleClear : ModeBase
    {

        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            var q = PluginModel.Q;
            var w = PluginModel.W;
            var e = PluginModel.E;

            var minionTarget = EntityManager.MinionsAndMonsters.Monsters.Aggregate((curMin, x) => (curMin == null || x.Health < curMin.Health ? x : curMin));

            if (minionTarget == null || !minionTarget.IsValidTarget()) return;

            if (q.IsReady() && q.IsInRange(minionTarget) && Misc.IsChecked(PluginModel.JungleClearMenu, "jcQ"))
            {
                q.Cast(minionTarget.Position);
            }

            if (!Player.Instance.IsInAutoAttackRange(minionTarget) && w.IsReady() && w.IsInRange(minionTarget) && Misc.IsChecked(PluginModel.JungleClearMenu, "jcW") && ShadowManager.CanCastW)
            {
                w.Cast(minionTarget.Position);
            }

            if (e.IsReady() && e.IsInRange(minionTarget) && Misc.IsChecked(PluginModel.JungleClearMenu, "jcE"))
            {
                e.Cast();
            }

        }
    }
}

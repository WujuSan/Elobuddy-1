using System.Linq;
using Ass_Zed.Model;
using BRSelector.Util;
using EloBuddy;
using EloBuddy.SDK;

namespace Ass_Zed.Controller.Modes
{
    public sealed class LastHit : ModeBase
    {

        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
            var q = PluginModel.Q;
            var e = PluginModel.E;

            var minionTarget = EntityManager.MinionsAndMonsters.EnemyMinions.Aggregate((curMin, x) => (curMin == null || x.Health < curMin.Health ? x : curMin));

            if (minionTarget == null || !minionTarget.IsValidTarget()) return;

            if (!Player.Instance.IsInAutoAttackRange(minionTarget) && q.IsReady() && q.IsInRange(minionTarget) && Misc.IsChecked(PluginModel.LastHitMenu, "lhQ") && Player.Instance.GetSpellDamage(minionTarget, SpellSlot.Q) > minionTarget.Health && !minionTarget.IsDead)
            {
                q.Cast(minionTarget.Position);
            }

            if (!Orbwalker.CanAutoAttack && e.IsReady() && e.IsInRange(minionTarget) && Misc.IsChecked(PluginModel.LastHitMenu, "lhE") && Player.Instance.GetSpellDamage(minionTarget, SpellSlot.E) > minionTarget.Health && !minionTarget.IsDead)
            {
                e.Cast();
            }
        }
    }
}

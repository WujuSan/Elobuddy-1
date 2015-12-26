using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Rengod_Ressurect.Util;

namespace Rengod_Ressurect.Instances
{
    internal static class LaneClear
    {
        public static void DoClear()
        {
            var target =
                EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(x => !x.IsDead && Spells.E.IsInRange(x));

            if (!target.IsValidTarget(Spells.E.Range) ||
                (MenuCreator.IsChecked(MenuCreator.LaneClearMenu, "lc.ferocity") && Player.Instance.Mana > 4)) return;

            switch (MenuCreator.GetSliderValue(MenuCreator.LaneClearMenu, "lc.ferocity.type"))
            {
                case 1:
                    FocusQ(target);
                    break;
                case 2:
                    FocusW(target);
                    break;
                case 3:
                    FocusE(target);
                    break;
            }
        }

        private static void FocusQ(Obj_AI_Base target)
        {
            if ((Player.HasBuff(Globals.RENGAR_UTIMATE_DISPLAY_NAME) || Player.HasBuff(Globals.RENGAR_UTIMATE_NAME)) &&
                Player.Instance.IsInAutoAttackRange(target))
            {
                Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }

            if (Player.Instance.Mana < 5)
            {
                if (Spells.E.IsReady() && Spells.E.IsInRange(target))
                {
                    var ePredict = Spells.E.GetPrediction(target);
                    if (ePredict.HitChancePercent >= 75)
                    {
                        Spells.E.Cast(ePredict.CastPosition);
                    }
                }

                if (Spells.Q.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                {
                    Spells.Q.Cast();
                }

                if (Spells.W.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                {
                    Spells.W.Cast(target);
                }
            }
            else
            {
                if (Spells.Q.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                {
                    Spells.Q.Cast();
                }
            }
        }

        private static void FocusW(Obj_AI_Base target)
        {
            if (Player.Instance.Mana < 5)
            {
                if (Spells.E.IsReady() && Spells.E.IsInRange(target))
                {
                    var ePredict = Spells.E.GetPrediction(target);
                    if (ePredict.HitChancePercent >= 70)
                    {
                        Spells.E.Cast(ePredict.CastPosition);
                    }
                }

                if (Spells.Q.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                {
                    Spells.Q.Cast();
                }

                if (Spells.W.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                {
                    Spells.W.Cast(target);
                }
            }
            else
            {
                if (Spells.W.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                {
                    Spells.W.Cast(target);
                }
            }
        }

        private static void FocusE(Obj_AI_Base target)
        {
            if (Player.Instance.Mana < 5)
            {
                if (Spells.E.IsReady() && Spells.E.IsInRange(target))
                {
                    var ePredict = Spells.E.GetPrediction(target);
                    if (ePredict.HitChancePercent >= 70)
                    {
                        Spells.E.Cast(ePredict.CastPosition);
                    }
                }

                if (Spells.Q.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                {
                    Spells.Q.Cast();
                }

                if (Spells.W.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                {
                    Spells.W.Cast(target);
                }
            }
            else
            {
                if (Spells.E.IsReady() && Spells.E.IsInRange(target))
                {
                    var ePredict = Spells.E.GetPrediction(target);
                    if (ePredict.HitChance >= HitChance.High)
                    {
                        Spells.E.Cast(ePredict.CastPosition);
                    }
                }
            }

        }
    }
}
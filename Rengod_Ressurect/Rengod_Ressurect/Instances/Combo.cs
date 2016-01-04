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
    static class Combo
    {
        public static int CheckCount = 0;
        public static bool useQ = false;

        static Combo()
        {
            Orbwalker.OnPostAttack += AfterAutoAttack;
        }

        private static void AfterAutoAttack(AttackableUnit target, EventArgs args)
        {
            if (!useQ) return;

            Spells.Q.Cast();

            useQ = false;
        }

        public static void DoCombo()
        {
            var target = TargetSelector.GetTarget(Spells.E.Range, DamageType.Physical);

            if (!target.IsValidTarget(Spells.E.Range)) return;

            if(target.Distance(Player.Instance) < 350)
            {
                var predictPos = Prediction.Position.PredictUnitPosition(target, 200);
                Orbwalker.OrbwalkTo(predictPos.To3D());
            }

            if (Player.HasBuff(Globals.RENGAR_UTIMATE_DISPLAY_NAME) || Player.HasBuff(Globals.RENGAR_UTIMATE_NAME))
            {
                if (Player.Instance.IsInAutoAttackRange(target))
                {
                    Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
                else
                {
                    return;
                }
            }

            switch (MenuCreator.GetSliderValue(MenuCreator.ComboMenu, "combo.type"))
            {
                case 1:
                    FocusQ(target);
                    break;
                case 2:
                    FocusE(target);
                    break;
                case 3:
                    FocusW(target);
                    break;
            }

        }

        private static void FocusQ(Obj_AI_Base target)
        {
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
                    useQ = true;
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
                    useQ = true;
                }
                else if (Spells.E.IsReady() && Spells.E.IsInRange(target))
                {
                    var ePredict = Spells.E.GetPrediction(target);
                    if (ePredict.HitChancePercent >= 70)
                    {
                        Spells.E.Cast(ePredict.CastPosition);
                    }
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
                    useQ = true;
                }

                if (Spells.W.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                {
                    Spells.W.Cast(target);
                }
            }
            else
            {
                if (Spells.W.IsReady())
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
                    useQ = true;
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
                    if (ePredict.HitChancePercent >= 65)
                    {
                        Spells.E.Cast(ePredict.CastPosition);
                    }
                }
                else
                {
                    if (Spells.Q.IsReady() && Player.Instance.IsInAutoAttackRange(target))
                    {
                        useQ = true;
                    }
                }

            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Ass_Zed.Model;
using BRSelector.Model;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using OneForWeek.Util.Misc;
using SharpDX;

namespace Ass_Zed.Helpers
{
    public class ShadowManager : PluginModel
    {
        public static bool CanCastW
        {
            get
            {
                var currentShadows = GetShadows().Count();
                return ((!Player.Instance.HasBuff("zedwhandler") && W.IsReady() && Game.Time > LastWCast + 0.3F
                         && Game.Time > LastBuffWTime + 1F) && W.IsReady()
                        && !Player.Instance.HasBuff("zedwhandler")
                        && ((Player.Instance.HasBuff("zedr2") && currentShadows == 1) || currentShadows == 0));
            }
        }

        public static bool CanSwitch
        {
            get
            {
                return !CanCastW &&  W.IsReady()
                       && !ObjectManager.Get<Obj_AI_Turret>()
                               .Any(ob => ob.Distance(Instance.Position) < 775F && ob.IsEnemy && !ob.IsDead);
            }
        }

        public static Obj_AI_Base Instance
        {
            get
            {
                var shadow = GetShadows().FirstOrDefault();
                return shadow ?? Player.Instance;
            }
        }

        public static bool CanCastR(bool isFirstR = true)
        {
            return !isFirstR || (!(LastRCast + 6 - Game.Time > 0));
        }

        public static List<Obj_AI_Base> GetShadows()
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(obj => obj.BaseSkinName.ToLowerInvariant().Contains("shadow") && !obj.IsDead).ToList();
        }

        public static void Cast(Vector3 position)
        {
            if (CanCastW)
            {
                W.Cast(position);
                LastWCast = Game.Time;
            }
        }

        public static void Cast(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }

            Cast(target.Position);
        }

        public static void Switch()
        {
            if (CanSwitch)
            {
                W.Cast();
            }
        }

        public static void Combo()
        {
            var shadows = GetShadows();

            if (!shadows.Any()
                || (!Misc.IsChecked(ComboMenu, "comboQ") && !Misc.IsChecked(ComboMenu, "comboE"))
                || (!E.IsReady() && !E.IsReady()))
            {
                return;
            }

            foreach (var objAiBase in shadows)
            {
                if (((Misc.IsChecked(ComboMenu, "comboQ") && !Q.IsReady()) || !Misc.IsChecked(ComboMenu, "comboQ"))
                    && ((Misc.IsChecked(ComboMenu, "comboE") && !E.IsReady()) || !Misc.IsChecked(ComboMenu, "comboE")))
                {
                    break;
                }

                if (Misc.IsChecked(ComboMenu, "comboQ") && Q.IsReady())
                {
                    var target = AdvancedTargetSelector.GetTarget(
                        Q.Range,
                        DamageType.Physical,
                        true,
                        objAiBase.Position);

                    if (target != null)
                    {
                        var predictionQ = Q.GetPrediction(target);

                        if (predictionQ.HitChance >= HitChance.Medium)
                        {
                            Q.Cast(predictionQ.CastPosition);
                        }
                    }
                }

                if (Misc.IsChecked(ComboMenu, "comboE") && E.IsReady())
                {
                    var target = AdvancedTargetSelector.GetTarget(E.Range, DamageType.Physical, true, objAiBase.Position);

                    if (target != null)
                    {
                        E.Cast();
                    }
                }
            }
        }
    }
}

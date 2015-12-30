using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelZero.Util
{
    static class TargetUtil
    {
        //TODO: Popular com buffs que invalidam o alvo (Trindamere, Kayle, Kindred's etc...)
        public static string[] invalidTargetBuffs = { "" };

        /*
            Checkage if target is valid.
        */

        public static bool IsValidTargetUtil(this Obj_AI_Base target)
        {
            foreach (var buff in invalidTargetBuffs)
            {
                if (target.HasBuff(buff))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsValidTargetUtil(this Obj_AI_Base target, float range)
        {
            foreach (var buff in invalidTargetBuffs)
            {
                if (target.HasBuff(buff))
                {
                    return false;
                }
            }

            if (Player.Instance.Distance(target) > range)
            {
                return false;
            }

            return true;
        }

    }
}

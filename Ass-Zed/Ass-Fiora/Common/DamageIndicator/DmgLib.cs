using Ass_Zed.Model;
using EloBuddy;
using EloBuddy.SDK;

namespace Ass_Zed.Common.DamageIndicator
{
    class DmgLib : PluginModel
    {

        public static double PossibleDamage(Obj_AI_Base target)
        {
            var damage = 0D;
            if (Q.IsReady())
                damage += QDamage(target);
            if (E.IsReady())
                damage += EDamage(target);
            if (R.IsReady())
                damage += Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float) (((new[] { 25, 35, 50 }[R.Level - 1]) * damage) * Player.Instance.FlatPhysicalDamageMod + 1f));

            return damage;
        }

        public static float QDamage(Obj_AI_Base target)
        {
            if (!Q.IsLearned) return 0f;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)
                (new[] { 75, 115, 155, 195, 235 }[Q.Level - 1] + 1 * Player.Instance.FlatPhysicalDamageMod));
        }

        public static float EDamage(Obj_AI_Base target)
        {
            if (!E.IsLearned) return 0f;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)
                (new[] { 75, 115, 155, 195, 235 }[E.Level - 1] + 1 * Player.Instance.FlatPhysicalDamageMod));
        }
    }
}

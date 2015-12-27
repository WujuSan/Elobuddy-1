    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;

namespace Rengod_Ressurect.Util
{
    static class Spells
    {
        public static readonly Spell.Active Q;
        public static readonly Spell.Skillshot W;
        public static readonly Spell.Skillshot E;
        public static readonly Spell.Active R;
        public static int[] qDamage = { 20, 40, 60, 80, 100 };
        public static float[] qDamagePercent = { 100, 105, 100, 115, 120 };
        public static int[] wDamage = { 50, 80, 110, 140, 170 };
        public static int[] eDamage = { 50, 100, 150, 200, 250 };

        static Spells()
        {
            Q = new Spell.Active(SpellSlot.Q, 150);
            W = new Spell.Skillshot(SpellSlot.W, 500, SkillShotType.Circular, 250, 2000, 100)
            {
                AllowedCollisionCount = -1
            };
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 250, 1500, 140);
            R = new Spell.Active(SpellSlot.R);
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0f;

            if (Q.IsReady())
                damage += QDamage(enemy);

            if (W.IsReady())
                damage += WDamage(enemy);

            if (E.IsReady())
                damage += EDamage(enemy);

            return damage;
        }

        public static float QDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                   qDamage[Q.Level - 1] + (qDamagePercent[Q.Level - 1] / 100f) * Player.Instance.FlatPhysicalDamageMod);
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                wDamage[W.Level - 1] + 0.8f * Player.Instance.FlatMagicDamageMod);
        }

        public static float EDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                eDamage[E.Level - 1] + 0.7f * Player.Instance.FlatPhysicalDamageMod);
        }

    }
}

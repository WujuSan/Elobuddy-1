using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LevelZero.Model;

namespace LevelZero.Util
{
    static class DamageUtil
    {
        public static List<SpellDamage> SpellsDamage { get; set; }

         public static float GetComboDamage(Obj_AI_Base enemy)
         {
             return SpellsDamage.Sum(spellBase => Player.Instance.CalculateDamageOnUnit(enemy, spellBase.DamageType, spellBase.SpellDamageValue[spellBase.Spell.Level - 1] + spellBase.SpellDamageModifier[spellBase.Spell.Level - 1]*(spellBase.DamageType == DamageType.Magical ? Player.Instance.FlatMagicDamageMod : Player.Instance.FlatPhysicalDamageMod)));
         }

        public static bool Killable(Obj_AI_Base enemy, SpellSlot slot, float damageDecrease = 0)
        {
            var spellBase = SpellsDamage.Find(s => s.Spell.Slot == slot);
            var possibleDamage = Player.Instance.CalculateDamageOnUnit(enemy, spellBase.DamageType,
                spellBase.SpellDamageValue[spellBase.Spell.Level - 1] +
                spellBase.SpellDamageModifier[spellBase.Spell.Level - 1]*
                (spellBase.DamageType == DamageType.Magical
                    ? Player.Instance.FlatMagicDamageMod
                    : Player.Instance.FlatPhysicalDamageMod));

            return enemy.Health < (possibleDamage - damageDecrease);
        }
    }
}

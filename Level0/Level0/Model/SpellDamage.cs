using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace LevelZero.Model
{
    class SpellDamage
    {
        public Spell.SpellBase Spell { get; set; }
        public float[] SpellDamageValue { get; set; }
        public float[] SpellDamageModifier { get; set; }
        public DamageType DamageType { get; set; }

        public SpellDamage(Spell.SpellBase spell, float[] spellDamageValue, float[] spellDamageModifier, DamageType damageType)
        {
            Spell = spell;
            SpellDamageValue = spellDamageValue;
            SpellDamageModifier = spellDamageModifier;
            DamageType = damageType;
        }
    }
}

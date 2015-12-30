using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using LevelZero.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EloBuddy.SDK.Spell;
using EloBuddy.SDK.Events;

namespace LevelZero.Model
{
    abstract class PluginModel : IChampion
    {
        public static List<SpellBase> Spells { get; set; }
        public static List<Feature> Features { get; set; }

        public PluginModel()
        {
            Spells = new List<SpellBase>();
            Features = new List<Feature>();
        }

        public SpellBase findSpell(SpellSlot spellSlot)
        {
            var spell = Spells.Find(s => s.Slot == spellSlot);

            return spell != null ? spell : null;
        }

        public virtual void Init()
        {
            throw new NotImplementedException();
        }

        public virtual void InitVariables()
        {
            throw new NotImplementedException();
        }

        public virtual void InitMenu()
        {
            throw new NotImplementedException();
        }

        public virtual void InitEvents()
        {
            throw new NotImplementedException();
        }

        public virtual void OnCombo()
        {
            throw new NotImplementedException();
        }

        public virtual void OnHarass()
        {
            throw new NotImplementedException();
        }

        public virtual void OnLaneClear()
        {
            throw new NotImplementedException();
        }

        public virtual void OnLastHit()
        {
            throw new NotImplementedException();
        }

        public virtual void OnFlee()
        {
            throw new NotImplementedException();
        }

        public virtual void OnUpdate(EventArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual void OnDraw(EventArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            throw new NotImplementedException();
        }

        public virtual void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual void GameObjectOnDelete(GameObject sender, EventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}

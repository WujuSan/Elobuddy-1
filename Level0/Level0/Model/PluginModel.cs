using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using LevelZero.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK.Events;

namespace LevelZero.Model
{
    abstract class PluginModel : IChampion
    {
        public static List<Spell.SpellBase> Spells { get; set; }
        public static List<Feature> Features { get; set; }

        protected PluginModel()
        {
            Spells = new List<Spell.SpellBase>();
            Features = new List<Feature>();
            Init();
        }

        public Spell.SpellBase findSpell(SpellSlot spellSlot)
        {
            var spell = Spells.Find(s => s.Slot == spellSlot);

            return spell != null ? spell : null;
        }

        public virtual void Init()
        {
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
            Game.OnTick += OnUpdate;
            Obj_AI_Base.OnLevelUp += OnPlayerLevelUp;
        }

        public virtual void OnPlayerLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (!sender.IsMe) return;
        }

        public virtual void OnCombo()
        {
        }

        public virtual void OnHarass()
        {
        }

        public virtual void OnLaneClear()
        {
        }

        public virtual void OnJungleClear()
        {
        }

        public virtual void OnLastHit()
        {
        }

        public virtual void OnFlee()
        {
        }

        public virtual void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None)) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) OnCombo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) OnHarass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) OnLaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)) OnLastHit();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) OnFlee();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) OnJungleClear();
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
            if(sender.IsAlly) return;
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

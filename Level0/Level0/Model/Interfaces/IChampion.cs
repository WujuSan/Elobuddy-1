using EloBuddy;
using EloBuddy.SDK.Events;
using System;

namespace LevelZero.Model.Interfaces
{
    public interface IChampion
    {
        void Init();
        void InitVariables();
        void InitMenu();
        void InitEvents();
        void OnCombo();
        void OnHarass();
        void OnLaneClear();
        void OnLastHit();
        void OnFlee();
        void OnUpdate(EventArgs args);
        void OnDraw(EventArgs args);
        void OnAfterAttack(AttackableUnit target, EventArgs args);
        void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs);
        void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e);
        void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args);
        void GameObjectOnCreate(GameObject sender, EventArgs args);
        void GameObjectOnDelete(GameObject sender, EventArgs args);
    }
}

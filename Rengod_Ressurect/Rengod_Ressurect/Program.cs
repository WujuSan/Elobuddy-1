using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using Rengod_Ressurect.Instances;
using Rengod_Ressurect.Util;

namespace Rengod_Ressurect
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnGameComplete;
        }

        private static void OnGameComplete(EventArgs args)
        {
            InitRengod();
        }


        static void InitRengod()
        {
            if (!Player.Instance.ChampionName.ToLower().Equals("rengar")) return;

            MenuCreator.Init();
            ItemManager.Init();

            Game.OnTick += OnTick;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
        }

        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || Player.Instance.IsDead) return;

        }

        private static void OnTick(EventArgs args)
        {
            if (Player.Instance.IsDead) return;
            InstanceControl();
        }

        static void InstanceControl()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.DoCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass.DoHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear.DoClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear.DoClear();
            }
        }
    }
}

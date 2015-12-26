using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace Rengod_Ressurect.Util
{
    static class ItemManager
    {
        static Item _tiamat = new Item(3077, 300);
        static Item _hydra = new Item(3074, 300);
        static Item _yomumu = new Item(3142, 0);

        public static void Init()
        {
            Orbwalker.OnPreAttack += BeforeAutoAttack;
            Orbwalker.OnPostAttack += AfterAutoAttack;
            Console.WriteLine("Item Initialized");
        }

        private static void AfterAutoAttack(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || !MenuCreator.IsChecked(MenuCreator.MiscMenu, "misc.hydra")) return;

            if (_tiamat.IsOwned() && _tiamat.IsReady())
            {
                _tiamat.Cast();
            }
            else if(_hydra.IsOwned() && _hydra.IsReady())
            {
                _hydra.Cast();
            }
        }

        private static void BeforeAutoAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || !MenuCreator.IsChecked(MenuCreator.MiscMenu, "misc.yomumu"))
                return;

            if (!target.IsValidTarget(Player.Instance.GetAutoAttackRange())) return;

            if (_yomumu.IsOwned() && _yomumu.IsReady())
            {
                _yomumu.Cast();
            }
        }
    }
}

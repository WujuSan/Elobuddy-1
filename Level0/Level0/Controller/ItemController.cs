using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LevelZero.Util;

namespace LevelZero.Controller
{
    class ItemController
    {
        #region Offensive Itens

        public bool CastTiamatHydra()
        {
            var tiamatHydra = ItemUtil.GetItem(ItemId.Tiamat_Melee_Only);

            if (tiamatHydra.IsOwned() && tiamatHydra.IsReady())
            {
                tiamatHydra.Cast();
                return true;
            }

            tiamatHydra = ItemUtil.GetItem(ItemId.Ravenous_Hydra_Melee_Only);

            if (!tiamatHydra.IsOwned() || !tiamatHydra.IsReady())
            {
                return false;
            }

            tiamatHydra.Cast();

            return true;
        }

        public bool CastYomumu()
        {
            var yomumu = ItemUtil.GetItem(ItemId.Youmuus_Ghostblade);

            if (!yomumu.IsOwned() || !yomumu.IsReady()) return false;

            yomumu.Cast();

            return true;
        }

        public bool CastBtrk(Obj_AI_Base target)
        {
            var bilgewaterBtrk = ItemUtil.GetItem(ItemId.Bilgewater_Cutlass);

            if (bilgewaterBtrk.IsOwned() && bilgewaterBtrk.IsReady())
            {
                bilgewaterBtrk.Cast(target);
                return true;
            }

            bilgewaterBtrk = ItemUtil.GetItem(ItemId.Blade_of_the_Ruined_King);

            if (!bilgewaterBtrk.IsOwned() || !bilgewaterBtrk.IsReady())
            {
                return false;
            }

            bilgewaterBtrk.Cast(target);

            return true;
        }

        #endregion
        #region Defensive Itens

        public bool CastZhonya()
        {
            var zhonya = ItemUtil.GetItem(ItemId.Zhonyas_Hourglass);

            if (!zhonya.IsOwned() || !zhonya.IsReady()) return false;

            zhonya.Cast();

            return true;
        }

        public bool CastSeraphEmbrace()
        {
            var seraph = ItemUtil.GetItem(3040);

            if (!seraph.IsOwned() || !seraph.IsReady()) return false;

            seraph.Cast();

            return true;
        }

        #endregion
    }
}

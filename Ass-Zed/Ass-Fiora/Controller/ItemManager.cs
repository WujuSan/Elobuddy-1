using EloBuddy;
using EloBuddy.SDK;

namespace Ass_Zed.Controller
{
    public static class ItemManager
    {

        public static Item Hydra { get; set; }
        public static Item Botrk { get; set; }
        public static Item Cutl { get; set; }
        public static Item Tiamat { get; set; }
        public static Item Yomu { get; set; }
        public static Item Sheen { get; private set; }
        public static Item TriForce { get; private set; }

        static ItemManager()
        {
            Hydra = new Item((int)ItemId.Ravenous_Hydra_Melee_Only, 400);
            Botrk = new Item((int)ItemId.Blade_of_the_Ruined_King, 450);
            Cutl = new Item((int)ItemId.Bilgewater_Cutlass, 450);
            Tiamat = new Item((int)ItemId.Tiamat_Melee_Only, 400);
            Yomu = new Item((int)ItemId.Youmuus_Ghostblade);
            Sheen = new Item((int)ItemId.Sheen);
            TriForce = new Item((int)ItemId.Trinity_Force);
        }

        public static void UseHydra(AttackableUnit target)
        {
            if (Tiamat.IsOwned() || Hydra.IsOwned())
            {
                if ((Tiamat.IsReady() || Hydra.IsReady()) && Player.Instance.Distance(target) <= Hydra.Range)
                {
                    Tiamat.Cast();
                    Hydra.Cast();
                }
            }
        }

        public static bool CanUseHydra()
        {
            if (!Tiamat.IsOwned() && !Hydra.IsOwned()) return false;

            return Tiamat.IsReady() || Hydra.IsReady();
        }

        public static void UseHydraNot(AttackableUnit target)
        {
            if (!Tiamat.IsOwned() && !Hydra.IsOwned()) return;
            if (!Tiamat.IsReady() && !Hydra.IsReady()) return;
            Tiamat.Cast();
            Hydra.Cast();
        }

        public static void UseYomu()
        {
            if (Yomu.IsOwned() && Yomu.IsReady())
                Yomu.Cast();
        }

        public static void UseCastables(Obj_AI_Base target)
        {
            if (Botrk.IsOwned() || Cutl.IsOwned())
            {
                if (target == null || !target.IsValidTarget()) return;

                if (Botrk.IsReady() || Cutl.IsReady())
                {
                    Botrk.Cast(target);
                    Cutl.Cast(target);
                }
            }
        }

        public static void UseAll(Obj_AI_Base target)
        {
            UseHydra(target);
            UseCastables(target);
            UseYomu();
        }

        public static void Initialize()
        {

        }
    }
}

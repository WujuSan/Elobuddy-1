using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelZero.Util
{
    static class ItemUtil
    {
        public static Item GetItem(ItemId id)
        {
            return new Item(id);
        }

        public static Item GetItem(int id)
        {
            return new Item(id);
        }
    }
}

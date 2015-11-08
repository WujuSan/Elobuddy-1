using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK;
using SharpDX;

namespace Ass_Zed.Helpers
{
    public static class VectorHelper
    {
        public static double AngleBetween(Vector2 a, Vector2 b, Vector2 c)
        {
            var a1 = c.Distance(b);
            var b1 = a.Distance(c);
            var c1 = b.Distance(a);
            if (a1 == 0 || c1 == 0) { return 0; }

            return Math.Acos((a1 * a1 + c1 * c1 - b1 * b1) / (2 * a1 * c1)) * (180 / Math.PI);
        }

        public static bool InTheCone(this Vector3 pos, List<Vector3> poses, Vector2 targetpos)
        {
            var x = true;

            foreach (var i in poses.Where(i => AngleBetween(pos.To2D(), targetpos, i.To2D()) > 90))
            {
                x = false;
            }
            return x;
        }

    }
}

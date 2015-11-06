using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using OneForWeek.Plugin;
using SharpDX;

namespace OneForWeek.Util.Misc
{
    public static class O4WExtensions
    {
        public class Circle : Geometry.Polygon
        {
            public Vector2 Center;

            public float Radius;

            private readonly int _quality;

            public Circle(Vector3 center, float radius, int quality = 20) : this(center.To2D(), radius, quality) { }

            
            public Circle(Vector2 center, float radius, int quality = 20)
            {
                Center = center;
                Radius = radius;
                _quality = quality;
                UpdatePolygon();
            }
            
            public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
            {
                Points.Clear();
                var outRadius = (overrideWidth > 0
                    ? overrideWidth
                    : (offset + Radius) / (float)Math.Cos(2 * Math.PI / _quality));
                for (var i = 1; i <= _quality; i++)
                {
                    var angle = i * 2 * Math.PI / _quality;
                    var point = new Vector2(
                        Center.X + outRadius * (float)Math.Cos(angle), Center.Y + outRadius * (float)Math.Sin(angle));
                    Points.Add(point);
                }
            }
        }

        public static Vector3 GetTumblePos(this Obj_AI_Base target)
        {
            if (!target.IsMelee && Player.Instance.CountEnemiesInRange(800) == 1) return Game.CursorPos;

            var aRc = new Circle(Player.Instance.ServerPosition.To2D(), 300).ToClipperPath();
            var cursorPos = Game.CursorPos;
            var targetPosition = target.ServerPosition;
            var pList = new List<Vector3>();
            var additionalDistance = (0.106 + Game.Ping / 2000f) * target.MoveSpeed;

            if (!cursorPos.IsDangerousPosition()) return cursorPos;

            foreach (var v3 in aRc.Select(p => new Vector2(p.X, p.Y).To3D()))
            {
                if (target.IsFacing(Player.Instance))
                {
                    if (!v3.IsDangerousPosition() && v3.Distance(targetPosition) < 550) pList.Add(v3);
                }
                else
                {
                    if (!v3.IsDangerousPosition() && v3.Distance(targetPosition) < 550 - additionalDistance) pList.Add(v3);
                }
            }
            if (Player.Instance.UnderTurret() || Player.Instance.CountEnemiesInRange(800) == 1)
            {
                return pList.Count > 1 ? pList.OrderBy(el => el.Distance(cursorPos)).FirstOrDefault() : Vector3.Zero;
            }
            if (!cursorPos.IsDangerousPosition())
            {
                return pList.Count > 1 ? pList.OrderBy(el => el.Distance(cursorPos)).FirstOrDefault() : Vector3.Zero;
            }
            return pList.Count > 1 ? pList.OrderByDescending(el => el.Distance(cursorPos)).FirstOrDefault() : Vector3.Zero;
        }

        public static int VayneWStacks(this Obj_AI_Base o)
        {
            return o.GetBuffCount("vaynesilvereddebuff");
        }

        public static Vector3 Randomize(this Vector3 pos)
        {
            var r = new Random(Environment.TickCount);
            return new Vector2(pos.X + r.Next(-150, 150), pos.Y + r.Next(-150, 150)).To3D();
        }

        public static bool IsDangerousPosition(this Vector3 pos)
        {
            return
                EntityManager.Heroes.Enemies.Any(
                    e => e.IsValidTarget() && e.IsVisible &&
                        e.Distance(pos) < Misc.GetSliderValue(PluginModel.MiscMenu, "QMinDist")) ||
                (pos.UnderTurret(true) && !Player.Instance.UnderTurret(true)) || pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall);
        }

        public static bool IsKillable(this AIHeroClient hero)
        {
            return Player.Instance.GetAutoAttackDamage(hero) * 2 < hero.Health;
        }
        
        public static bool IsValidState(this AIHeroClient target)
        {
            return !target.HasBuffOfType(BuffType.SpellShield) && !target.HasBuffOfType(BuffType.SpellImmunity) &&
                   !target.HasBuffOfType(BuffType.Invulnerability);
        }

        public static bool UnderTurret(this Obj_AI_Base unit)
        {
            return UnderTurret(unit.Position, true);
        }

        public static bool UnderTurret(this Obj_AI_Base unit, bool enemyTurretsOnly)
        {
            return UnderTurret(unit.Position, enemyTurretsOnly);
        }

        public static bool UnderTurret(this Vector3 position, bool enemyTurretsOnly)
        {
            return
                ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.IsValidTarget(950));
        }

        public static int CountHerosInRange(this Obj_AI_Base target, bool checkteam, float range = 1200f)
        {
            var objListTeam =
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        x => x.IsValidTarget(range));

            return objListTeam.Count(hero => checkteam ? hero.Team != target.Team : hero.Team == target.Team);
        }
    }
}

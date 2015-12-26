using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK.Rendering;

namespace Rengod_Ressurect.Util
{
    static class Draw
    {
        public static void Init()
        {
            Drawing.OnDraw += OnDraw;
            DamageIndicator.Initialize(Spells.GetComboDamage);
        }

        private static void OnDraw(EventArgs args)
        {
            if (MenuCreator.IsChecked(MenuCreator.DrawMenu, "draw.disable"))
            {
                DamageIndicator.Enabled = false;
                return;
            }

            if (MenuCreator.IsChecked(MenuCreator.DrawMenu, "draw.w"))
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = Spells.W.Range }.Draw(Player.Instance.Position);
            }
            if (MenuCreator.IsChecked(MenuCreator.DrawMenu, "draw.e"))
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = Spells.E.Range }.Draw(Player.Instance.Position);
            }

            DamageIndicator.Enabled = MenuCreator.IsChecked(MenuCreator.DrawMenu, "draw.damage");

        }
    }
}

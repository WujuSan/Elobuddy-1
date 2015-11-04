using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK.Events;
using OneForWeek.Draw.Notifications;
using OneForWeek.Model.Notification;
using OneForWeek.Plugin.Hero;
using OneForWeek.Util.Misc;
using OneForWeek.Plugin;

namespace OneForWeek
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadCompleted;
        }

        private static void OnLoadCompleted(EventArgs args)
        {
            if(Player.Instance.ChampionName == "Caitlyn")
            {
                new Caitlyn().Init();
                Notification.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, Player.Instance.ChampionName + " injected !", Color.DeepSkyBlue));
                Notification.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, "Addon by: MrArticuno", Color.Purple));

                Igniter.Init();
            }
            else
            {
                Notification.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, Player.Instance.ChampionName + " is Not Supported", Color.Red));
            }
        }
    }
}
